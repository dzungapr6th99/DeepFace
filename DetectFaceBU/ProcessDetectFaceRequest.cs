using CommonLib;
using DetectFaceObject;
using FaceDetectInterface;
using PreProcess;
using System.Diagnostics;

namespace DetectFaceBU
{
    public interface IProcessDetectFaceRequest
    {
        public Task<VerifyFaceResponse> Api1DetectFaceBU(VerifyFaceRequest request);
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
        public async Task<VerifyFaceResponse> Api1DetectFaceBU(VerifyFaceRequest request)
        {

            try
            {

                LOG.log.Info("Start Process request {0}", request.RequestID);
                LOG.log.Debug("Image check: {0}", request.Base64ImgCheck);
                LOG.log.Debug("Image verify: {0}", request.Base64ImgVerify);
                Stopwatch stopwatch = Stopwatch.StartNew();
                stopwatch.Start();
                bool IsVerified = c_FaceDetect.Verify(request.Base64ImgVerify, request.Base64ImgCheck);
                stopwatch.Stop();
                VerifyFaceResponse response = new VerifyFaceResponse()
                {
                    Verified = IsVerified,
                    ReturnCode = 1,
                    ReturnMessage = "Success"
                };
                LOG.log.Info("Process request {0} success in {1} ms and get result {2}", request.RequestID, stopwatch.ElapsedMilliseconds, response.Verified);
                return response;
            }
            catch (Exception ex)
            {
                LOG.log.Error(ex);
                return new VerifyFaceResponse()
                {
                    Verified = false,
                    ReturnCode = -999,
                    ReturnMessage = "Service get unknown error"

                };
            }
            catch
            {
                return new VerifyFaceResponse
                {
                    Verified = false,
                    ReturnCode = -999,
                    ReturnMessage = "Service get Cls error"
                };
            }

        }

        public async Task<DetectFaceResponse> Api2DetectFaceValid(DetectFaceRequest request)
        {
            try
            {

                LOG.log.Info("Start Process request {0}", request.RequestID);

                Stopwatch stopwatch = Stopwatch.StartNew();
                stopwatch.Start();
                bool IsVerified = c_FaceDetect.Detect(request.Base64ImgDetect);
                stopwatch.Stop();
                DetectFaceResponse response = new DetectFaceResponse()
                {
                    Detected = IsVerified,
                    ReturnCode = 1,
                    ReturnMessage = "Success"
                };
                LOG.log.Info("Process request {0} success in {1} ms", request.RequestID, stopwatch.ElapsedMilliseconds);
                return response;
            }
            catch (Exception ex)
            {
                LOG.log.Error(ex);
                return new DetectFaceResponse()
                {
                    Detected = false,
                    ReturnCode = -999,
                    ReturnMessage = "Service get unknown error"

                };
            }
            catch
            {
                return new DetectFaceResponse
                {
                    Detected = false,
                    ReturnCode = -999,
                    ReturnMessage = "Service get Cls error"
                };
            }
        }
    }
}
