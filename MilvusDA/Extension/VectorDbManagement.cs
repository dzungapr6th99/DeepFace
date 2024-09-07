using MilvusDA.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milvus.Client;
using System.Net;
namespace MilvusDA.Extension
{
    public class VectorDbManagement : IVectorDbManagement
    {
        string endpoint = "localhost";
        int port = 19530;
        static string userName = "dungnt";
        static string password =  "password";

        MilvusClient milvusClient = new MilvusClient(new Uri("localhost:19530"), userName, password, null);
      



    }
}
