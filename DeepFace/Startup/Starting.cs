using ApiServer;
using ApiServer.Validation;
using CommonLib;
using DetectFaceBU;
using DetectFaceObject;
using FaceDetectInterface;
using FluentValidation;
using PreProcess;

namespace DeepFace
{
    public class Starting
    {
        public static void InitProject(IServiceCollection services)
        {
            services.AddSingleton<IDetectorModel, DetectorModel>();
            services.AddSingleton<IFaceDetect, FaceDetect>();
            services.AddSingleton<IProcessDetectFaceRequest, ProcessDetectFaceRequest>();
            services.AddSingleton<IValidator<VerifyFaceRequest>,Api1DetectFaceValidation>();  
            services.AddHostedService<Workers>();  
        }
    }
}
