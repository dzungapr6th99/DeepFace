using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetectInterface.Interface
{
    public interface IFaceDetect
    {
        public bool Verify(IFormFile ImageCheck, IFormFile ImageVerify);
        public void LoadModel();
        bool Detect(IFormFile ImgBase);
        List<List<float>>? Embeding(IFormFile ImageEmbed, out List<Rectangle> faceCoordinates);
    }
}
