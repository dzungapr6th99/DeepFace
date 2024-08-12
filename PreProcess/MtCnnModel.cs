using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CommonLib;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
namespace PreProcess
{
    

    public unsafe class MtCnnModel:IDetectorModel
    {
        public string MtCnnPath;
        private int height;
        private int width;
        private bool IsloadedModel = false;
        public static IntPtr DetectModel;
#if !UNIX
        [DllImport("MtCnnDll.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr CreateMTCnnModel(sbyte * modelPath);
        [DllImport("MtCnnDll.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int DetectFace(IntPtr model, sbyte* Base64Img, int length, int width, int height, out IntPtr ListFaceData);
#else
        [DllImport("libMtCnnModel.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr CreateMTCnnModel(sbyte * modelPath);
        [DllImport("libMtCnnModel.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern int DetectFace(IntPtr model, sbyte* Base64Img, int length, int width, int height, out IntPtr ListFaceData);
#endif
        private GCHandle pinedGCHandle;
        public MtCnnModel()
        {
            MtCnnPath = ConfigData.MtCnnPath;
            IsloadedModel = false;
        }
        public void LoadModel()
        {
            if (!IsloadedModel)
            {
                byte[] _Path = Encoding.UTF8.GetBytes(MtCnnPath);
                sbyte* _BufferPath;
                GCHandle _pinnedHandle = GCHandle.Alloc(_Path, GCHandleType.Pinned);
                _BufferPath = (sbyte*)_pinnedHandle.AddrOfPinnedObject().ToPointer();
                Marshal.Copy(_Path, 0, (IntPtr)(_BufferPath + 0), _Path.Length);

                DetectModel = CreateMTCnnModel(_BufferPath);
                LOG.log.Info("Create Detect model success");
                IsloadedModel = true;
            }
        }
        public (int, byte[]) Detect(string Base64Image, int width, int height)
        {
            try
            {

                IntPtr ListFaceData;
                byte[] base64ImgRaw = Encoding.UTF8.GetBytes(Base64Image);
                sbyte* dataImg = (sbyte*)GCHandle.Alloc(base64ImgRaw, GCHandleType.Pinned).AddrOfPinnedObject().ToPointer();
                Marshal.Copy(base64ImgRaw, 0, (IntPtr)(dataImg + 0), Base64Image.Length);

                int NumFaces = DetectFace(DetectModel, dataImg, Base64Image.Length, width, height, out ListFaceData);
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
            catch (Exception ex)
            {
                LOG.log.Error(ex);
                throw;
            }
            catch
            {
                throw;
                
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
