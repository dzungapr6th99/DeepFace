using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreProcess.Interface
{
    public interface IDetectModelExtension
    {
        byte[] GetImgDataArray(int numFaces, int width, int height, IntPtr dataPointer);
        List<Rectangle> GetFaceCoordinates(int numFaces, IntPtr dataPointer);
    }
}
