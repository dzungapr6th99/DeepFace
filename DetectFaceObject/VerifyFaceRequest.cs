using Microsoft.AspNetCore.Http;

namespace DetectFaceObject
{
    public class VerifyFaceRequest
    {
        public IFormFile ImageVerify { get; set; } //Ảnh gốc
        public IFormFile ImageCheck { get; set; } //Ảnh hiện tại xem có đúng 1 người ko?
        public string RequestID { get; set; } = string.Empty; 
        public VerifyFaceRequest()
        {

        }
    }
    public class DetectFaceRequest
    {
        public IFormFile ImageDetect{ get; set; } //Ảnh gốc
        public string RequestID { get; set; } = string.Empty;
        public DetectFaceRequest()
        {

        }
    }
}
