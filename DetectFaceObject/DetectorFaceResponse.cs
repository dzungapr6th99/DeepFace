using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DetectFaceObject
{
    public class DetectorFaceResponse
    {
        public bool Verified { get; set; } = false;
        public string ReturnMessage { get; set; } = string.Empty;   
        public int ReturnCode {  get; set; }    
        public DetectorFaceResponse() 
        {
            
        }
    }
}
