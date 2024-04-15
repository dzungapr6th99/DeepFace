using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Configuration;
using PreProcess;
using FaceDetectInterface;
//using OpenCVClr;
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
            string path = "ModelAI\\haarcascade_frontalface_default.xml";
            string PathEyes = "ModelAI\\haarcascade_frontalface_default.xml";
           
            DetectorModel detectorModel = new DetectorModel(path, PathEyes);
            string ImgBase64Db = "/9j/4AAQSkZJRgABAQAAAQABAAD/2wBDAAIBAQEBAQIBAQECAgICAgQDAgICAgUEBAMEBgUGBgYFBgYGBwkIBgcJBwYGCAsICQoKCgoKBggLDAsKDAkKCgr/2wBDAQICAgICAgUDAwUKBwYHCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgr/wAARCABwAHADASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD8Hy+/+Einj5Y8A++aYeBupWbjaB9TQA5HQHc3Wnx8NnbkDr9KhqRRiMKO9Amrk8arMTNINsYbaufSnLLb28QAh3nDAkn16f1qqXYNgMcelLQLlLNnqEcMu6e13qQQUPTpSbkulSBYSX8wBG3dj2qsZAMLzxSh2AyOlA0rEjQNFK0DdQxGc5HFE6uoEjRkrjjHfFOjaOWH964VgRsA9KdbTs222c55YAMehPegYkREbkEFcrwBXoPwF8RTaf4kbS5JdkdyAFjJ4LgH+nFcFcJkL5DgEMQRjHpzn86taLrM+kaxZatath4LlHyD7jP9aipFTptEzjzKx9SqqsVcRqvyjAJqzDAFh3KeSuB7VT06/S6tI7tUJ82MMWHQ8Zq5FKDGFYkEdOOK8NqzMbW0PjiljOCd3c0lKoycV750AxBORS5cYH5UbB6mnpG0jBUBJz0A5NO2lwGIH35YHOeKcFPUCtWw8Ia7fzCOGxfe3Kq3HH41v23wO8b3yE2GlS3RRN0ptkLhB7kcZ9qz54p2N6WGr1fgi38jijEzkFVOac0Lp14GMgYrtNN+EXjR7hWbwdfNGODvjMe8Z7FunT8s1g+JbS7g1mWG6tkikVirwxkER44wD3xSVTWzKnhqlJXkmvXQyQGByMjHekJJOSTk85ra0rwdrurabdaxaafLJbWiqZ5gnyrk4AJ9azLqBkk2DscZIqlKL2MpU5q11uReeQfnc89qTzHDghhnPHFO2lItxQEN90+tOkYPOJI4wgUAqFPpQ2iGmj6j+H6l/Clk7owP2VODz/DnIraQqyDZ0B44rA+Hs0h8G6Y7ybsWUYyepwoBrdWcgYZgVJ6CvBn8TOZ7nx75YA+Wk2H1FPpYwGfbtJ9hX0B0pXZNpmn3WoTfZbWLzGbjaPevZvg58I/Ad/pf2rxFqirc7wHwwHr09OwrN+Enwzm1WOC3iR1NwN00h6Kveu1uP2c9dvbkLpF9ZBAQAks7oOo7AcmuWrOV7I97B5fiIRVWMOa57N8CvgN8LPFFyRfyWwCffS6lQlBngcHqevNfVvgb4I/B3w/aw6faQ6cIPKwoCL8pPc461+ZvjXwh8Z/AF3Fc6Z4hQqvCGzvNm3C45HGelZOh/Hz9oDwzfi5g8daqHtydyNcF1P1Ga0w9aMNJRufRYHiCjl8uSpQs+rR+yuk/Cv4Dy+H7jRtS0GzcyWjIt0qYZcrhQD6Z5r4b8R/8EtvDniP4u6p9l8Qyw2P2ncFUggFic4yORnNcF8If29fiHqs8Gi+Mbp5VLIuIsqX/AKV678Qf2sLbwfbLr9re5d4hnLHIOOOe9dU6uHqxTatY+plVyPN8J7eaVo9H0Or+Of7K3w3+Fv7MM/wo8JxwG4Fu1xe3uwLLLIWTcW9goYAV+cL/AA5l134hyeFLe5W2hM74nuPl+Re/1r0j4t/te/Gn4r39zptjqVwkNyfLK27MC6ehNZXw9/Zx+P3je7/t600ySGLG4z310o3gnnAPJFclWUXPmjofB5ziMJmNeFPBUm1HR2OP+PGgaN4V16x8NaFdJNDaaag8xMfM5JYk479K43SrOXUdQisUQkyuFAA5OeP612Pxy+G2vfDXxPDpniGVJZ7m2Mm+HO0AOVxn8P5VH8C9CGteP4HaMslrEZnB4HGB/M/pUttQbPm8XCVOq01byPdfD9mdK0u3sApJjhQHjPYA1pGJgQwQ4JznFECZXcy4JAz9amCBUDgq2Tjr0ryHqzzT47rd+H3hS48W+KbTSYTw8oLnGcAcmsVVIGD3Ndt8BNa/sP4mafdEAq8pjIP+0MV7s3ZWPSwVONXFQjLZtH158K/gbt8NNMsZjuHQiIFslfb2rjPin8Lvjt4fLw+HdbEEZHBQfvHB64btXtvwu8Y2knl3TurZZlz6H3r1PxH4GTxBo66lYRQsCo3Rtg54ojR5o3R+wSyuFWhGEHZd0fGnjf4TfCTX/wBn5NI0jSdXtviJBcLcXd5rcs00d4cMGijdW2IhDZGVGCAPp414c+FPiJdWj0/7PdJqH2gs9usLGMR4IIJPBr7xs/APh6+vn0u/huI5GbayZymfbPrXqPwx/ZU+D/h+7TxZ4tWO5uDHviiLZEY989TUqhOTPJlwbCdRVFNny/8AC/8AYxk1a/sfs9nE9xdsA0gjA2EjPPYGtD9vH9hL4qfCaw0O7bSp57G+RGMqoSFVs4OB24x+NfbFjN4eh1KMeH7S3it7eVTH9nUAk9M1778bp9H8ZfBPwd4kvbaOW6tGls7iCZcjaG3IST1GCfpiulYWKhvqz3auT0IYZYaCspdV0PyI+E37AGtfED4Da3qF/oj2fiu4RJPD6asHgtlRGztUh1BZ1BGWHHGPWuA0z9jP48eF7WDX9P8AENvpeszX+I9Gs9Qkka2i/h3yKSoHAAyS2FGfb9Ifi1oFhcW51LS7B1iQHdHAR8oHUgDk+30rz/w3/Y13r8WoXSSmON8/vmwo/DtWcsLGC8zhhwdg6VTnUpJ+T0v3Phn9vj4R+KPAel+DPEHie88++v7S4ivCDkB08tsjPrvP5Vxn7N2gpZ6ddazNEfMnIWN9v8IPOD+FfQn/AAVm8R2Xii78I6FpqAyfaLmTZj+AiNc/n/KvNPAWg22heHbWwiVcInJ9SeSa8/GTUPdifm3E0IUc1nCDvsbtucJ8qlufy75qTy3b5ccerDNJBEoJVOx/Sppo0ijV5HHLdDXnnztmkfHagZG4HrXoHwB8HapqvxF0q7W1P2eO4EsjsMAoP88V582C3Cgewr6C/ZAkt9WvobdoU3QqULHn6f1r2Z7HvZRRjWx0E+9z3Hw1qFx4c1E2MgIiZ9yllxn/AAr3b4e+NNO1Kyh0y61Z4C/BAG7FeOePPB7rYR3tqcPH3Q9RS+BdUuBFBMHbKcMBweOo/lW0Kjiz9lwdd024PY+qrLwh4LtLU3l1M8sroNjMg55+9WB8UPFWm+DdA85WeaWU7IbdedwPHSuX8K+L9QvreNYZ3BUcqZDlSPbsKr/GWe+0FNF8TSRtdvb3P2m4jU52RqG2g8+vOPYV1c8eW60PWlKFOk5Jnqn7P/g/xTraHWdc0+W3jyuIyPUZHHWvq3X/AAHp2o/slP4mhnZJ7DXViaMkn70ZIOeg71+f+mf8FIbbwvcLezeD782rqI57uSylWFWGcEsANtdvrn/BXXwYnw7ufCE3lwWdxcJd3McFzuErqpC/X6/hVe0w/KknsefWxNGdnGpFWabu+i6fM6HSvHVtJr2p+ENYtTHc2kzAA5HmI2CrA985rk/iFbeFfD9rJqBTywfmVkHGR3we1Zlt441f4waFpnxz07QJtPtrlGjhknIX7TCWJDAD+HdkZPqa5L4w65LrFhbaNb3TGR5lQorcnccf1rCtVUYts68RiqcMM5pnyf8AtS+JZ/FXxWs5jITFbW2yHI7MxJ47Z4/KtHRn22UakgfIAcCsH9oVI3+O91ZwxhVt3ESqB2HH+FdBZwRpapknIHQV4FV81rn89ZpXeIx9Sq+rZoRyRxod3zbeACMZpJmMgy+0DGdvT6VHb42/KPxPentGuwtuXI7e9ZHC22fIW1d23HfHSvd/2RbmOz07UZ4EAmtZ4pGcDnZtOfyGa8X1DTm0y/ksJEPyt8pPp1r6T/Zd0W3g+H9lqrWcayXLypJzkyqGwN3H145r1pzTirH03D9KVXMFy9Fc9/ttXsNW0Xa7hi0QMZIHIx/+quSWG50u+dkbapc4AbkEe9Q6ZqkmgaydBmUCLbusXkbqjfw/nn8qv6hbTXkxugAewIPAOOtaJ3Z+pU5RnBNbrc7f4b+Irfykn1lguzHIGSR6f/Xq94i+LWnavqU9tHOVG/ZCWIPC8AV5jeajeaRooFiGeRSdwU8Hn0rkbF/jQb2fXtJi08QLkCO7iLuc/wAQwcUpTl8JGMxdaLjCN7eSO58TaB41vppp9C01p1uAxKpFvDN2Ueuen410Og/sK+JfEPw5t/GPxX+GVzotqkQktrXywktypJPOfmCgfjXhXiXVP2pdVdXj1iO4jT/VxWvmLz0GAvINbvi1/wDgoQ9tpd1qreJFt7e3UWEctzM6kA8HD4+XtzSjRT9+VzyFRoqo5zpylfyPry0+LfhTQfh1a/Dizs47eCwtEgtLYDBhAGAB6+/1rwX4nfFDTvA97L4x1e5QLZzKIEkPDyE5AH5V5v8AYv2irLxlb+IfihBFD9tHzpE4XBx1IBrhP2grPXfHOvw6JBLN9ltNzOvYyN398D+tZ16rklFnPm2dKjgpJRceiT0MPWfHK/Ev4s3XimCEoLmcttHPU5/pXotqwMOdjdOeMYNcF4C+HjaDKLicDcDzuX3rtnvra2QMT908qMgfzrzqjV9D8qnOU5Ns0YTGV3k4A5+8Kpaj4gsrFWxJyAcEmuc13xskS/Z7acHrmuYvdZmvZOXbGOxqUrklo/C3TPHutwabBYu11ORHGIu59T+FfQ/wt+GNt4N8L2nhi1keSK33kTMRnJY811fwQ/Z6k+GWknxF4vRDrt1DhbXIP2KNhnB/2z39MY71t22khL2ULGoCnlSMc+tdtNTjbmP0LhnKauEiq9Ray29Dyfx7YXUFwqOpEsLb4ZW65o8H+Obe+It76TBOFuAD0PrXd+PPC8epWZWOPbIpypznnFeL6zpFzpGpm4hby3jJEmOOPQ1td810fV1pTw1Tmjt1PQdRs3vZhLbyeaq4AUHGB/WtzTdet49JdY9NWORRxgcEg9MGuH8EeOBbyC11ALkjCszZBFek+D/Fngi41BbTWbUSJnPmIw2qT/Cc/UVrGV9zvw9aMtbnEeKP2iPi34Tljs/DEdqwDcg2iuB+OM1s+GvjP8fvHd+h8YmQ2wUOxaDyzn1HHIr37wrP8M9PlF9pnhLSZmEYPnSxqx+p3VgfFj4i+FEgKSLaxvgLIIlX5efwz6Yx3rRuajrPTsTJ4mlJ1JVXy9tDwf4yeO9Q0zS7jxfqyB4rBStpG3PnTMcKMDt/QV4bo3xGiupWm1RGEkh3SOQcdef6/nX394c/Ys1D4ofDqz8b+ONCe2sb3M2m2VxEVPlYwshX/azkZ7DPeuI+Kf7HPww8N+GLqWCzijdI2+ZUxg4rz60Jy94/P8+p1M2rucJe7HzPk6PxxokqOkM67h13HHNc5rnie4uZmSGVgN3Y1zniC3OkeNbrTLKXMUc5UAHqAathJHduDySa5j4ecXCVmG5mkEjPnHft+tLEquC+PpinRxOCFaMZz1YVaS0lkYKIuvvS0RDdj//Z";
            string ImgBase64Input = "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";
            FaceDetect faceDetect = new(detectorModel, "VGGFace.onnx", 224, 224);
            bool verify = faceDetect.Verify(ImgBase64Db, ImgBase64Input);
            Console.WriteLine(verify);
            /* try
             {
                 string OpencvPath = ConfigData.OpenCVPath *//*"D:\\NAVISOFT Project\\DeepFace\\lib\\opencv\\sources\\data\\haarcascades"*//*;
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
                 byte[] imageArray = System.IO.File.ReadAllBytes(ConfigData.ImagePath *//*"D:\\NAVISOFT Project\\DeepFace\\PyFaceDetect\\resources\\faces\\24085f5c-6fd4-42ad-93de-d2fa932c76c1.jpg"*//*);
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
             }*/
            Console.WriteLine("Complete call C++");
            //Console.ReadKey();
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