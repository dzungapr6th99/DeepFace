using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetectFaceObject
{
    public class EmbedingFaceRequest
    {
        public string ImgBase64 { get; set; }
        public string FaceId { get; set; }
        public string RequestId { get; set; }
    }

    public class EmbedingFaceResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
    } 
}
