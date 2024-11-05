using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilvusDA.ObjectApi
{
    public class InsertResponse
    {
        public int Code { get; set; }
        public InsertResponseData Data { get; set; }

    }
    public class InsertResponseData
    {
        public int InsertCount { get; set; }
        public string Message { get; set; }
        public List<long> InsertIds { get; set; }
    } 
}
