<<<<<<< HEAD
﻿using CommonLib;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace PreProcess
{
    public interface IDetectorModel
    {
        public (int, byte[]) Detect(string Base64Image, int width, int height);
    }
    public unsafe class DetectorModel : IDisposable, IDetectorModel
    {
        
        public static IntPtr DetectModel;
        [DllImport("DetectorDLL.dll")]
        public static extern IntPtr CreateModel(sbyte * path, sbyte* pathEyes);
        [DllImport("DetectorDLL.dll")]
        public static extern int DetectImage(IntPtr model, sbyte* Base64Img, int length, int width, int height, out IntPtr ListFaceData);

        private GCHandle pinedGCHandle;
        public DetectorModel(string path, string PathEyes)
        {
            byte[] _Path = Encoding.UTF8.GetBytes(path);
            byte[] _PathEye = Encoding.ASCII.GetBytes(PathEyes);
            sbyte* _BufferPath;
            sbyte* _BufferEye;
            GCHandle _pinnedHandle = GCHandle.Alloc(_Path, GCHandleType.Pinned);
            _BufferPath = (sbyte*)_pinnedHandle.AddrOfPinnedObject().ToPointer();
            Marshal.Copy(_Path, 0, (IntPtr)(_BufferPath + 0), _Path.Length);
            _pinnedHandle = GCHandle.Alloc(_PathEye, GCHandleType.Pinned);
            _BufferEye = (sbyte*)_pinnedHandle.AddrOfPinnedObject().ToPointer();
            Marshal.Copy(_PathEye, 0, (IntPtr)(_BufferEye + 0), _PathEye.Length);

            sbyte* pathPointer = (sbyte*)GCHandle.Alloc(path.ToCharArray(), GCHandleType.Pinned).AddrOfPinnedObject().ToPointer();
            sbyte* PathEyesPointer = (sbyte*)GCHandle.Alloc(PathEyes.ToCharArray(), GCHandleType.Pinned).AddrOfPinnedObject().ToPointer();
            DetectModel = CreateModel(_BufferPath, _BufferEye);
            LOG.log.Info("Create Detect model success");
        }

        public (int, byte[]) Detect(string Base64Image, int width, int height)
        {
            IntPtr ListFaceData;
            byte[] base64ImgRaw = Encoding.UTF8.GetBytes(Base64Image);
            sbyte* dataImg = (sbyte*)GCHandle.Alloc(base64ImgRaw, GCHandleType.Pinned).AddrOfPinnedObject().ToPointer();
            Marshal.Copy(base64ImgRaw, 0, (IntPtr)(dataImg + 0), Base64Image.Length);

            int NumFaces = DetectImage(DetectModel, dataImg, Base64Image.Length, width, height,out ListFaceData);
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
            Span<byte> byteSpan = new Span<byte>(dataPointer.ToPointer(), NumFaces * 224 * 224 * 3);
            byte[] returnData = byteSpan.ToArray();
            return returnData;
        }

        public void Dispose()
        {
            
        }
=======
﻿using Microsoft.ML.OnnxRuntime;
using CommonLib;
using Tensorflow;
using System.Numerics.Tensors;
using System.Numerics;
using Tensorflow.Keras.Engine;

namespace PreProcess
{
    public class DetectorModel
    {
        public DetectorModel(string path)
        {

        }

        public void Detect(string Base64Image1, string Base64Image2)
        {
            
        }

>>>>>>> d52732b9f3bc244a910e59c5744ae2efbae3bac8
    }
}