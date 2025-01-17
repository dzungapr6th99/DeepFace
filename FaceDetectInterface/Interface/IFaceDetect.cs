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
        public bool Verify(string ImgBase64Db, string ImgBase64Input);
        public void LoadModel();
        bool Detect(string ImgBase64);
        List<List<float>>? Embeding(string ImgBase64, out List<Rectangle> faceCoordinates);
    }
}
