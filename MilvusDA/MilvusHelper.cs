using Milvus.Client;
using MilvusDA.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
namespace MilvusDA
{
    public class MilvusHelper : IMilvusHelper
    {
        private readonly MilvusClient client;
        private Dictionary<string, CollectionManagement> loadedCollections;
        private readonly bool _isLoadMode;
        private readonly bool _isReleaseCollection;
        private readonly long _tickPerScanPeriod;
        private Thread? _manageCollectionThread;
        private readonly IHttpClientFactory _httpClientFactory;
        private string address;
      
        private readonly string _authen;
        private Dictionary<string, string> _dictApiRout = new Dictionary<string, string>()
        {
            {"Insert", "/v2/vectordb/entities/insert" },
            {"Delete", "/v2/vectordb/entities/delete" },
            {"Get", "/v2/vectordb/entities/get" },
            {"Query", "/v2/vectordb/entities/query" },
            {"Search", "/v2/vectordb/entities/search" },
            {"Upsert", "/v2/vectordb/entities/upsert" },
        };
        /// <summary>
        /// Function create MilvusHelper
        /// </summary>
        /// <param name="ip">IP Address</param>
        /// <param name="port">Port</param>
        /// <param name="useSSL">is use SSL?</param>
        /// <param name="isLoadMode">is use load mode? if true, the service will request milvus load collection to improve performance, but the milvus server will consume more resource</param>
        /// <param name="isReleaseCollection">is use mode release collection, if true, when collection is not called after tickPerScanPeriod, it will be released</param>
        /// <param name="tickPerScanPeriod">count by tick</param>
        public MilvusHelper(string ip = "localhost", int port = 19530, bool useSSL = false, bool isLoadMode = false, bool isReleaseCollection = true, long tickPerScanPeriod = 30 * 60 * TimeSpan.TicksPerSecond, string userName = null, string password = null)
        {
            // Kết nối đến Milvus
            address = ip + ":" + port;
            client = new MilvusClient(ip, port, useSSL);
            loadedCollections = new Dictionary<string, CollectionManagement>();
            _isLoadMode = isLoadMode;
            _isReleaseCollection = isReleaseCollection;
            _tickPerScanPeriod = tickPerScanPeriod;
            _authen = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));
        }

        public void StartManageCollection()
        {
            _manageCollectionThread = new Thread(ManageLoadCollection);
            _manageCollectionThread.IsBackground = true;
            _manageCollectionThread.Start();
        }

        private void ManageLoadCollection()
        {
            while (_isLoadMode && _isReleaseCollection)
            {
                foreach (var manageCollection in loadedCollections.Values)
                {
                    if (DateTime.Now.Ticks - manageCollection.LastTimeUsed.Ticks > _tickPerScanPeriod)
                    {
                        var released = manageCollection.ReleaseCollection();
                    }
                }
            }
        }

        public virtual MilvusCollection? GetOrCreateCollection(string collectionName, int dim, string vectorFieldName, params Tuple<string, Type>[] otherField)
        {
            /*
            Ở đâu thiết kế vector db đơn giản là mỗi collection (collection trong vector db thì tương ứng với bảng trong sql db)
            tương ứng cho 1 loại model (do mỗi model thì có đầu ra là ma trận vector có số chiều khác nhau). 1 collection hay bảng thì 
            có 2 field (field tương ứng với cột trong sql db), 1 cột là id, cột còn lại là thông tin vector
             */
            // Tạo tên collection dựa modelname

            // Kiểm tra collection có tồn tại không
            var hasCollection = client.HasCollectionAsync(collectionName).Result;

            if (!hasCollection)
            {
                if (dim == 0)
                {
                    return null;
                }
                // Định nghĩa schema cho collection
                var createCollection = client.CreateCollectionAsync(
                collectionName, CreateCollectionDetail(collectionName, dim, otherField)

                ).Result;
                if (createCollection != null)
                {
                    var collectionManagement = new CollectionManagement(createCollection);
                    loadedCollections.Add(collectionName, collectionManagement);
                    return collectionManagement.GetCollection();
                }
                else
                {
                    return createCollection;
                }
            }
            else
            {

                return GetCollection(collectionName);
            }
        }

        public virtual MilvusCollection? GetCollection(string collectionName)
        {

            // Kiểm tra xem collection đã được load chưa
            if (loadedCollections.ContainsKey(collectionName))
            {
                return loadedCollections[collectionName].GetCollection();
            }

            // Kiểm tra collection có tồn tại không
            var hasCollection = client.HasCollectionAsync(collectionName).Result;
            if (hasCollection)
            {
                var milvusCollection = client.GetCollection(collectionName);
                CollectionManagement collectionManage = new CollectionManagement(milvusCollection);
                if (_isLoadMode)
                {
                    collectionManage.LoadColection();
                }
                loadedCollections.Add(collectionName, collectionManage);
                return collectionManage.GetCollection();
            }
            else
            {
                return null;
            }
        }



        public async Task<bool> Insert(string collectionName, Tuple<string, ReadOnlyMemory<float>> vector, params Tuple<string, object>[] fields)
        {
            var collection = GetCollection(collectionName);
            if (collection == null)
            {
                return false;
            }

            MutationResult result = await collection.InsertAsync(CreateRecord(vector, fields), collectionName);
            return result.InsertCount == 1;
        }




        public async Task<object> Search(ReadOnlyMemory<float> queryVector, string collectionName, string vectorFieldName, int topK = 1, float threshold = float.MaxValue, params string[] fieldNames)
        {


            // Kiểm tra xem collection cho số chiều này có tồn tại không
            var collection = GetCollection(collectionName);
            MilvusCollectionDescription collecitonInfo = await collection.DescribeAsync();

            // Định nghĩa tham số tìm kiếm
            SearchParameters parammeters = new SearchParameters();
            var searchParameters = new SearchParameters()
            {
                OutputFields = { "*" },
                ConsistencyLevel = ConsistencyLevel.Strong,
                Offset = 5,
                ExtraParameters = { ["nprobe"] = "1024" }
            };
            List<ReadOnlyMemory<float>> inputQueryVector = new List<ReadOnlyMemory<float>>() { queryVector };

            var result = await collection.SearchAsync<float>(vectorFieldName: vectorFieldName, vectors: inputQueryVector, SimilarityMetricType.Cosine, limit: 1,
                                                                parammeters = searchParameters);
            result.FieldsData[0].ToString();
            //result.FieldsData.OrderBy()
            return result.Ids.StringIds?.FirstOrDefault();
        }

        private FieldData[] CreateRecord(Tuple<string, ReadOnlyMemory<float>> vector, params Tuple<string, object>[] fields)
        {
            List<FieldData> fieldDatas = new List<FieldData>();
            for (int i = 0; i < fields.Count(); i++)
            {
                if (fields[i].Item2 is string)
                {
                    string[] fieldItem = new string[] { (string)fields[i].Item2 };
                    fieldDatas.Add(FieldData.CreateVarChar(fields[i].Item1, fieldItem));
                }
                else if (fields[i].Item2 is long)
                {
                    long[] fieldItem = new long[] { (long)fields[i].Item2 };
                    fieldDatas.Add(FieldData.Create<long>(fields[i].Item1, fieldItem));
                }
                else if (fields[i].Item2 is int)
                {
                    int[] fieldItem = new int[] { (int)fields[i].Item2 };
                    fieldDatas.Add(FieldData.Create<int>(fields[i].Item1, fieldItem));
                }
            }
            fieldDatas.Add(FieldData.CreateFloatVector(vector.Item1, new List<ReadOnlyMemory<float>>() { vector.Item2 }));
            return fieldDatas.ToArray();
        }

        private FieldSchema[] CreateCollectionDetail(string vectorName, int dim, params Tuple<string, Type>[] fields)
        {
            List<FieldSchema> fieldDatas = new List<FieldSchema>();
            for (int i = 0; i < fields.Count(); i++)
            {
                if (fields[i].Item2 == typeof(string))
                {
                    fieldDatas.Add(FieldSchema.CreateVarchar(fields[i].Item1, 32));
                }
                else if (fields[i].Item2 == typeof(long))
                {
                    fieldDatas.Add(FieldSchema.Create<long>(fields[i].Item1));
                }
                else if (fields[i].Item2 == typeof(int))
                {
                    fieldDatas.Add(FieldSchema.Create<int>(fields[i].Item1));
                }
            }
            fieldDatas.Add(FieldSchema.CreateFloatVector(vectorName, dim));
            return fieldDatas.ToArray();
        }

        public async Task<string> PostAsync(string method, string jsonObject, string collectionName)
        {
            MilvusCollection? collection = GetCollection(collectionName);

            if (collection == null)
            {
                return string.Empty;
            }
            using (HttpClient httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _authen);
                StringContent packageContent = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                using (HttpResponseMessage response = await httpClient.PostAsync( address + '/' + _dictApiRout[method], packageContent))
                {
                    string responseString = await response.Content.ReadAsStringAsync();                   
                    return responseString;

                }
            }

        }

        internal class CollectionManagement
        {
            private MilvusCollection Collection { get; set; }

            public DateTime LastTimeUsed { get; set; }

            private bool isLoaded { get; set; } = false;

            public CollectionManagement(MilvusCollection collection)
            {
                this.Collection = collection;
                LastTimeUsed = DateTime.Now;
            }

            public MilvusCollection GetCollection()
            {
                LastTimeUsed = DateTime.Now;
                return this.Collection;
            }

            public bool LoadColection()
            {
                if (isLoaded)
                {
                    LastTimeUsed = DateTime.Now;
                    return true;
                }

                else
                {
                    var result = Collection.LoadAsync();
                    isLoaded = true;
                    return isLoaded;
                }
            }

            public bool ReleaseCollection()
            {
                if (isLoaded)
                {
                    var result = Collection.ReleaseAsync();
                    isLoaded = false;
                    return true;
                }
                else
                {
                    isLoaded = false;
                    return true;
                }

            }
        }

    }

}