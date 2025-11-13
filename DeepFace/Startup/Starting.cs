using ApiServer;
using ApiServer.Validation;
using CommonLib;
using DetectFaceBU;
using DetectFaceBU.Interface;
using DetectFaceObject;
using FaceDetectInterface;
using FaceDetectInterface.Interface;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MilvusDA;
using MilvusDA.Interface;
using PreProcess;
using PreProcess.Interface;

namespace DeepFace
{
    public class Starting
    {
        public static void InitProject(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddSingleton<IMilvusHelper, MilvusHelper>();
            services.AddSingleton<IDetectorModel, MtCnnModel>();
            services.AddSingleton<IFaceDetect, FaceDetect>();
            services.AddSingleton<IProcessDetectFaceRequest, ProcessDetectFaceRequest>();
            services.AddSingleton<IValidator<VerifyFaceRequest>,Api1DetectFaceValidation>();  
            services.AddHostedService<Workers>();  
        }
    }
}
