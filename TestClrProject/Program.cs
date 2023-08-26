using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Configuration;
using OpenCVClr;
namespace TestClrProject
{
    public class Program
    {
        public static unsafe void Main()
        {

            
            var builder = new ConfigurationBuilder().SetBasePath(Path.Combine(Environment.CurrentDirectory))
                .AddJsonFile("appseting.json", optional: false, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            ConfigData.InitConfig(configuration);

            try
            {
                string OpencvPath = ConfigData.OpenCVPath /*"D:\\NAVISOFT Project\\DeepFace\\lib\\opencv\\sources\\data\\haarcascades"*/;
                byte[] _Path = Encoding.UTF8.GetBytes(OpencvPath);
                byte[] _PathEye = Encoding.ASCII.GetBytes(OpencvPath + Path.DirectorySeparatorChar +"haarcascades.yaml");
                sbyte* _BufferPath;
                sbyte* _BufferEye;
                GCHandle _pinnedHandle = GCHandle.Alloc(_Path,GCHandleType.Pinned);
                _BufferPath = (sbyte*)_pinnedHandle.AddrOfPinnedObject().ToPointer();
                Marshal.Copy(_Path, 0, (IntPtr)(_BufferPath + 0), _Path.Length);
                _pinnedHandle = GCHandle.Alloc(_PathEye, GCHandleType.Pinned);
                _BufferEye = (sbyte*)_pinnedHandle.AddrOfPinnedObject().ToPointer();
                Marshal.Copy(_PathEye, 0, (IntPtr)(_BufferEye + 0), _Path.Length);

                OpenCVModel Model = new OpenCVModel(_BufferPath, _BufferEye);


                long avgTime = 0;
                long time1;
                long time2;

                time1 = DateTime.Now.Ticks;
                byte[] imageArray = System.IO.File.ReadAllBytes(ConfigData.ImagePath /*"D:\\NAVISOFT Project\\DeepFace\\PyFaceDetect\\resources\\faces\\24085f5c-6fd4-42ad-93de-d2fa932c76c1.jpg"*/);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                byte[] base64DecodeImage = Convert.FromBase64String(base64ImageRepresentation);
                List<int> EyesCoordinate = new List<int>();
                GCHandle _pinnedGCHandle = GCHandle.Alloc(base64DecodeImage, GCHandleType.Pinned);
                sbyte* _pBuffer = (sbyte*)_pinnedGCHandle.AddrOfPinnedObject().ToPointer();
                Model.Detect(_pBuffer, base64DecodeImage.Length, true, ref EyesCoordinate);
                for (int i = 0; i < 100; i++)
                {
                    time1 = DateTime.Now.Ticks;
                    Model.Detect(_pBuffer, base64DecodeImage.Length, true, ref EyesCoordinate);
                    time2 = DateTime.Now.Ticks;
                    avgTime += time2 - time1;
                    //Console.WriteLine(time2 - time1);
                }
                Console.WriteLine(avgTime / 100);

                Console.WriteLine($"PreProcess average in {avgTime / 100}");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Complete call C++");
            Console.ReadKey();
        }

        public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }
    }
}