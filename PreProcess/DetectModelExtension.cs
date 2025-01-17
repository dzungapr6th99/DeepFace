using PreProcess.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PreProcess
{
    public unsafe class DetectModelExtension : IDetectModelExtension
    {
        public DetectModelExtension() 
        {

        }
        public byte[] GetImgDataArray(int numFaces, int width, int height, IntPtr dataPointer)
        {

            GCHandle pinedGCHandle = GCHandle.Alloc(dataPointer, GCHandleType.Pinned);
            Span<byte> byteSpan = new Span<byte>(dataPointer.ToPointer(), numFaces * width * height * 3);
            byte[] returnData = byteSpan.ToArray();
            pinedGCHandle.Free();
            return returnData;
        }

        public List<Rectangle> GetFaceCoordinates(int numFaces, IntPtr dataPointer)
        {
            List<Rectangle> result = new List<Rectangle>();
            GCHandle pinedGCHandle = GCHandle.Alloc(dataPointer, GCHandleType.Pinned);
            Span<int> intSpan = new Span<int>(dataPointer.ToPointer(), numFaces * 4); // 4 ở đây là tương ứng với x, y, w, h của tọa độ
            pinedGCHandle.Free();
            for (int i = 0; i < numFaces; i++)
            {
                result.Add(new Rectangle(intSpan[4 * i], intSpan[4 * i + 1], intSpan[4 * i + 2], intSpan[4 * i + 3]));
            }
            return result;
        }
    }
}
