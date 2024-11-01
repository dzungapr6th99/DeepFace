using Milvus.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilvusDA.Interface
{
    public interface IMilvusHelper
    {
        void StartManageCollection();
        MilvusCollection? GetOrCreateCollection(string collectionName, int dim, string vectorFieldName, params Tuple<string, Type>[] otherField);
        MilvusCollection? GetCollection(string collectionName);
        Task<bool> Insert(string collectionName, Tuple<string, ReadOnlyMemory<float>> vector, params Tuple<string, object>[] fields);
        Task<object> Search(ReadOnlyMemory<float> queryVector, string collectionName, string vectorFieldName, int topK = 1, float threshold = float.MaxValue, params string[] fieldNames);
        Task<string> PostAsync(string method, string jsonObject, string collectionName);
    }
}
