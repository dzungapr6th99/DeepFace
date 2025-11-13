using CommonLib;
using NLog;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using PreProcess.Interface;
using System.Drawing;
namespace PreProcess
{
    
    public unsafe class HaarCascadeModel : DetectModelExtension, IDisposable, IDetectorModel
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
        private static extern int DetectImage(IntPtr model, sbyte* base64Img, int length, int width, int height, out IntPtr listFaceData, out IntPtr listFaceCoordinate);
#else
        [DllImport("libDetectFace.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr CreateModel(sbyte* path, sbyte* pathEyes);
        [DllImport("libDetectFace.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern int DetectImage(IntPtr model, sbyte* Base64Img, int length, int width, int height, out IntPtr listFaceData, out IntPtr listFaceCoordinate);
#endif
        private GCHandle pinedGCHandle;
        public HaarCascadeModel()
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
        public (int, byte[]) Detect(byte[] base64ImgRaw, int width, int height, out List<Rectangle>? faceCoordinates)
        {
            try
            {
                IntPtr listFaceData;
                IntPtr listFaceCoordinate;
                sbyte* dataImg = (sbyte*)GCHandle.Alloc(base64ImgRaw, GCHandleType.Pinned).AddrOfPinnedObject().ToPointer();
                Marshal.Copy(base64ImgRaw, 0, (IntPtr)(dataImg + 0), base64ImgRaw.Length);

                int NumFaces = DetectImage(DetectModel, dataImg, base64ImgRaw.Length, width, height, out listFaceData, out listFaceCoordinate);
                if (NumFaces > 0)
                {
                    //Tức là detect ra có.     
                    faceCoordinates = GetFaceCoordinates(NumFaces, listFaceCoordinate);
                    return (NumFaces, GetImgDataArray(NumFaces, width, height, listFaceData));
                }
                else
                {
                    faceCoordinates = null;
                    return (0, null);
                }
            }
            catch (Exception e)
            {
                faceCoordinates = null;
                LOG.log.Error(e);
                throw;
            }
        }

        

        public void Dispose()
        {

        }
    }
}