using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DetectFaceObject
{
    public class VerifyFaceResponse
    {
        public bool Verified { get; set; } = false;
        public string ReturnMessage { get; set; } = string.Empty;   
        public int ReturnCode {  get; set; }    
        public VerifyFaceResponse() 
        {
            
        }
    }

    public class DetectFaceResponse
    {
        public bool Detected { get; set; } = false;
        public string ReturnMessage { get; set; } = string.Empty;
        public int ReturnCode { get; set; }
        public DetectFaceResponse() { }
    }
}
