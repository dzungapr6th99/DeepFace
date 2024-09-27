
using Milvus.Client;
using System;
using System.Collections.Generic;
using System.Linq;

public class MilvusHelper
{
    private readonly MilvusClient client;
    private readonly Dictionary<int, string> collectionNames; // Bản đồ giữa số chiều và tên collection

    public MilvusHelper(string host = "localhost", int port = 19530)
    {
        // Kết nối đến Milvus
       
        client = new MilvusClient(host);

        // Bản đồ số chiều và tên collection
        collectionNames = new Dictionary<int, string>();
    }

    private async void CreateCollection(int dim)
    {
        // Tạo tên collection dựa trên số chiều của vector
        string collectionName = $"vector_collection_{dim}";

        // Kiểm tra collection có tồn tại không
        var milvusCollection = client.GetCollection(collectionName);
        var hasCollection =  await client.HasCollectionAsync(collectionName);

        if (!hasCollection)
        {
            // Định nghĩa schema cho collection
            await client.CreateCollectionAsync(
            collectionName,
            new[] {
                FieldSchema.Create<long>("book_id", isPrimaryKey:true),
                FieldSchema.Create<long>("word_count"),
                FieldSchema.CreateVarchar("book_name", 256),
                FieldSchema.CreateFloatVector("book_intro", 2)
            }
        );
        }
        else
        {

        }
        // Thêm collection vào bản đồ
        if (!collectionNames.ContainsKey(dim))
        {
            collectionNames.Add(dim, collectionName);
        }
    }

    public void InsertVectors(long id, List<float[]> vectors)
    {
        // Lấy số chiều của vector đầu tiên
        int dim = vectors[0].Length;

        // Tạo collection nếu chưa tồn tại
        CreateCollection(dim);

        // Lấy tên collection tương ứng với số chiều
        string collectionName = collectionNames[dim];

        // Tạo dữ liệu để thêm vào collection
        var ids = Enumerable.Repeat(id, vectors.Count).ToList();
        var insertParam = new InsertParam.Builder(collectionName)
            .WithField("id", ids)
            .WithField("vector", vectors)
            .Build();

        // Thêm dữ liệu vào collection
        var result = client.Insert(insertParam);
        Console.WriteLine($"Inserted {vectors.Count} vectors with ID {id} in collection {collectionName}.");
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

// Ví dụ sử dụng
class Program
{
    static void Main(string[] args)
    {
        MilvusHelper milvusHelper = new MilvusHelper();

        // Tạo danh sách vector có số chiều 128
        var vectors128 = new List<float[]>();
        Random rand = new Random();
        for (int i = 0; i < 10; i++)
        {
            float[] vector = new float[128];
            for (int j = 0; j < 128; j++)
            {
                vector[j] = (float)rand.NextDouble();
            }
            vectors128.Add(vector);
        }

        // Thêm vector 128 chiều vào Milvus
        milvusHelper.InsertVectors(1, vectors128);

        // Tạo danh sách vector có số chiều 256
        var vectors256 = new List<float[]>();
        for (int i = 0; i < 10; i++)
        {
            float[] vector = new float[256];
            for (int j = 0; j < 256; j++)
            {
                vector[j] = (float)rand.NextDouble();
            }
            vectors256.Add(vector);
        }

        // Thêm vector 256 chiều vào Milvus
        milvusHelper.InsertVectors(1, vectors256);

        // Tìm kiếm vector 128 chiều gần nhất
        float[] queryVector128 = new float[128];
        for (int j = 0; j < 128; j++)
        {
            queryVector128[j] = (float)rand.NextDouble();
        }
        var nearestId128 = milvusHelper.SearchNearestVector(queryVector128);
        Console.WriteLine($"ID của vector 128 chiều gần nhất: {string.Join(", ", nearestId128)}");

        // Tìm kiếm vector 256 chiều gần nhất
        float[] queryVector256 = new float[256];
        for (int j = 0; j < 256; j++)
        {
            queryVector256[j] = (float)rand.NextDouble();
        }
        var nearestId256 = milvusHelper.SearchNearestVector(queryVector256);
        Console.WriteLine($"ID của vector 256 chiều gần nhất: {string.Join(", ", nearestId256)}");
    }
}
