using Keras;
using System.Net.WebSockets;
using System.IO;
using System.Drawing;
using Python;
using Python.Runtime;
using Keras.Models;
using Numpy;
using CommonLib;
using System.Linq;
namespace FaceDetectInterface
{
    
    public class FaceDetect
    {
        private Dictionary<string, BaseModel> models;
        


        public void Init(string _Path)
        {
            
        }
        /*public NDarray FaceDetect(byte[] img)
        {
            Image<Rgb, Byte> image = new Image<Rgb, byte>(224, 224);
            image.Bytes = img;

            return default;
            
        }*/

        public NDarray Detect(byte[] img, int width, int heigt, string model)
        {
            
        }
    }
}