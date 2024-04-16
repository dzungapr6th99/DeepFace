using DetectFaceObject;
using FaceDetectInterface;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using CommonLib;
namespace ApiServer
{
    [Route("detectface")]
    [ApiController]
    public class Api1DetectFaceController : ControllerBase
    {
        private readonly IFaceDetect c_FaceDetect;
        public Api1DetectFaceController(IFaceDetect p_FaceDetect)
        {
            c_FaceDetect = p_FaceDetect;
        }
        [HttpPost]
        [Route("api1/verifyface")]
        public async Task<DetectorFaceResponse> Api1DetectFaceProcess(DetectFaceRequest request)
        {
            try
            {

                LOG.log.Info("Start Process request {0}", request.RequestID);

                Stopwatch stopwatch = Stopwatch.StartNew();
                stopwatch.Start();
                bool IsVerified = c_FaceDetect.Verify(request.Base64ImgVerify, request.Base64ImgCheck);
                stopwatch.Stop();
                DetectorFaceResponse response = new DetectorFaceResponse()
                {
                    Verified = IsVerified,
                    ReturnCode = 1,
                    ReturnMessage = "Success"
                };
                LOG.log.Info("Process request {0} success in {1} ms", request.RequestID, stopwatch.ElapsedMilliseconds);
                return response;
            }
            catch (Exception ex)
            {
                LOG.log.Error(ex);
                return new DetectorFaceResponse()
                {
                    Verified = false,
                    ReturnCode = -999,
                    ReturnMessage = "Service get unknown error"

                };
            }

        }
    }
}
