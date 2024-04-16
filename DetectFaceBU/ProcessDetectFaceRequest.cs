using CommonLib;
using DetectFaceObject;
using FaceDetectInterface;
using PreProcess;
using System.Diagnostics;

namespace DetectFaceBU
{
    public interface IProcessDetectFaceRequest
    {
        public Task<DetectorFaceResponse> Api1DetectFaceBU(DetectFaceRequest request);
    }
    public class ProcessDetectFaceRequest:IProcessDetectFaceRequest
    {
        private readonly IDetectorModel c_DetectorModel;
        private readonly IFaceDetect c_FaceDetect;
        public ProcessDetectFaceRequest(IDetectorModel p_DetectorModel , IFaceDetect p_FaceDetect)
        {
            c_DetectorModel = p_DetectorModel;  
            c_FaceDetect = p_FaceDetect;
        }
        public async Task<DetectorFaceResponse> Api1DetectFaceBU(DetectFaceRequest request)
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
