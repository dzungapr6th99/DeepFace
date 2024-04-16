using ApiServer;
using CommonLib;
using FaceDetectInterface;
using PreProcess;

namespace DeepFace
{
    public class Starting
    {
        public static void InitProject(IServiceCollection services)
        {
            services.AddSingleton<IDetectorModel, DetectorModel>();
            services.AddSingleton<IFaceDetect, FaceDetect>();
            services.AddSingleton<Api1DetectFaceController>();  
            services.AddHostedService<Workers>();  
        }
    }
}
