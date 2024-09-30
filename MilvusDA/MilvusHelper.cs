
using Milvus.Client;
using System;
using System.Collections.Generic;
using System.Linq;

public class MilvusHelper
{
    private readonly MilvusClient client;
    private readonly Dictionary<string, string> collectionNames; // Bản đồ giữa số chiều và tên collection

    public MilvusHelper(string host = "localhost", int port = 19530)
    {
        // Kết nối đến Milvus

        client = new MilvusClient(host);

        // Bản đồ số chiều và tên collection
        collectionNames = new Dictionary<string, string>();
    }

    public virtual MilvusCollection GetOrCreateCollection(string modelName)
    {
        /*
        Ở đâu thiết kế vector db đơn giản là mỗi collection (collection trong vector db thì tương ứng với bảng trong sql db)
        tương ứng cho 1 loại model (do mỗi model thì có đầu ra là ma trận vector có số chiều khác nhau). 1 collection hay bảng thì 
        có 2 field (field tương ứng với cột trong sql db), 1 cột là id, cột còn lại là thông tin vector
         */
        // Tạo tên collection dựa modelname
        string collectionName = $"{modelName}";

        // Kiểm tra collection có tồn tại không
        var hasCollection =  client.HasCollectionAsync(collectionName).Result;

        // Thêm collection vào database 
        if (!collectionNames.ContainsKey(modelName))
        {
            collectionNames.Add(modelName, collectionName);
        }
        if (!hasCollection)
        {
            // Định nghĩa schema cho collection
            var result = client.CreateCollectionAsync(
            collectionName,
            new[] {
                FieldSchema.Create<long>("face_id", isPrimaryKey: true, autoId: true),
                FieldSchema.CreateVarchar("face_name", 256),
                FieldSchema.CreateFloatVector("face_vector", 2,)
            }).Result;
            return result;
        }
        else
        {
            var milvusCollection = client.GetCollection(collectionName);
            return milvusCollection;
        }
    }

    public async void InsertVectors(string faceid, List<ReadOnlyMemory<float>> vectors, string modelName)
    {
        var collection = GetOrCreateCollection(modelName);
        MutationResult result = await collection.InsertAsync(
    new FieldData[]
    {

        FieldData.CreateFloatVector(faceid, vectors),
    },
    modelName);
    }

    public List<long> SearchNearestVector(float[] queryVector, int topK = 1)
    {
        // Lấy số chiều của vector đầu vào
        int dim = queryVector.Length;

        // Kiểm tra xem collection cho số chiều này có tồn tại không
        if (!collectionNames.ContainsKey(dim))
        {
            throw new Exception($"No collection found for vectors with dimension {dim}.");
        }

        // Lấy tên collection tương ứng
        string collectionName = collectionNames[dim];

        // Định nghĩa tham số tìm kiếm
        var searchParam = new SearchParam.Builder(collectionName)
            .WithVectors(new List<float[]> { queryVector })
            .WithTopK(topK)
            .WithVectorField("vector")
            .WithMetricType(MetricType.L2)
            .Build();

        // Thực hiện tìm kiếm
        var result = client.Search(searchParam);

        // Lấy ID của vector gần nhất
        var searchResults = result.Data.Results;
        var nearestIds = searchResults.Select(r => r.Id).ToList();

        return nearestIds;
    }
}


}
