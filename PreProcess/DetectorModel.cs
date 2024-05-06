using CommonLib;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace PreProcess
{
    public interface IDetectorModel
    {
        public (int, byte[]) Detect(string Base64Image, int width, int height);
        public void LoadModel();
    }
    public unsafe class DetectorModel : IDisposable, IDetectorModel
    {
        
        public string c_PathFace;
        public string c_PathEyes;
        private int height;
        private int width;
        private bool IsloadedModel = false;
        public static IntPtr DetectModel;
#if !UNIX
        [DllImport("DetectorDll.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr CreateModel(sbyte* path, sbyte* pathEyes);
        [DllImport("DetectorDll.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int DetectImage(IntPtr model, sbyte* Base64Img, int length, int width, int height, out IntPtr ListFaceData);
#else
        [DllImport("libDetectFace.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr CreateModel(sbyte* path, sbyte* pathEyes);
        [DllImport("libDetectFace.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern int DetectImage(IntPtr model, sbyte* Base64Img, int length, int width, int height, out IntPtr ListFaceData);
#endif
        private GCHandle pinedGCHandle;
        public DetectorModel()
        {
            c_PathFace = ConfigData.ModelDetector_Face_Path;
            c_PathEyes = ConfigData.ModelDetector_Eye_Path;
            IsloadedModel = false;
        }
        public void LoadModel()
        {
            if (!IsloadedModel)
            {
                byte[] _Path = Encoding.UTF8.GetBytes(c_PathFace);
                byte[] _PathEye = Encoding.ASCII.GetBytes(c_PathEyes);
                sbyte* _BufferPath;
                sbyte* _BufferEye;
                GCHandle _pinnedHandle = GCHandle.Alloc(_Path, GCHandleType.Pinned);
                _BufferPath = (sbyte*)_pinnedHandle.AddrOfPinnedObject().ToPointer();
                Marshal.Copy(_Path, 0, (IntPtr)(_BufferPath + 0), _Path.Length);
                _pinnedHandle = GCHandle.Alloc(_PathEye, GCHandleType.Pinned);
                _BufferEye = (sbyte*)_pinnedHandle.AddrOfPinnedObject().ToPointer();
                Marshal.Copy(_PathEye, 0, (IntPtr)(_BufferEye + 0), _PathEye.Length);
                 
                DetectModel = CreateModel(_BufferPath, _BufferEye);
                LOG.log.Info("Create Detect model success");
                IsloadedModel = true;
            }
        }
        public (int, byte[]) Detect(string Base64Image, int width, int height)
        {
            IntPtr ListFaceData;
            byte[] base64ImgRaw = Encoding.UTF8.GetBytes(Base64Image);
            sbyte* dataImg = (sbyte*)GCHandle.Alloc(base64ImgRaw, GCHandleType.Pinned).AddrOfPinnedObject().ToPointer();
            Marshal.Copy(base64ImgRaw, 0, (IntPtr)(dataImg + 0), Base64Image.Length);

            int NumFaces = DetectImage(DetectModel, dataImg, Base64Image.Length, width, height, out ListFaceData);
            if (NumFaces > 0)
            {
                //Tức là detect ra có.     
                return (NumFaces, GetImgDataArray(NumFaces, width, height, ListFaceData));
            }
            else
            {
                return (0, null);
            }
        }
        public byte[] GetImgDataArray(int NumFaces, int width, int height, IntPtr dataPointer)
        {

            GCHandle pinedGCHandle = GCHandle.Alloc(dataPointer, GCHandleType.Pinned);
            Span<byte> byteSpan = new Span<byte>(dataPointer.ToPointer(), NumFaces * width * height * 3);
            byte[] returnData = byteSpan.ToArray();
            pinedGCHandle.Free();
            return returnData;
        }

        public void Dispose()
        {

        }
    }
}