using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreProcess.Interface
{
    public interface IDetectorModel
    {
        public (int, byte[]) Detect(string Base64Image, int width, int height, out List<Rectangle>? faceCoordinates);
        public void LoadModel();
    }
}
