﻿namespace DetectFaceObject
{
    public class DetectFaceRequest
    {
        public string Base64ImgVerify { get; set; } = string.Empty; //Ảnh gốc
        public string Base64ImgCheck { get; set; } = string.Empty; //Ảnh hiện tại xem có đúng 1 người ko?
        public string RequestID { get; set; } = string.Empty; 
        public DetectFaceRequest()
        {

        }
    }
}
