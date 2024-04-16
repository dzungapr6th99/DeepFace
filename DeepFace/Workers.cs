using FaceDetectInterface;
using PreProcess;

namespace DeepFace
{
    public class Workers:BackgroundService
    {
        private IDetectorModel c_DetectorModel;
        private IFaceDetect c_FaceDetect;
        public Workers(IDetectorModel p_DetectorModel,  IFaceDetect p_FaceDetect)
        {
            c_DetectorModel = p_DetectorModel;
            c_FaceDetect = p_FaceDetect;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            c_DetectorModel.LoadModel();
            c_FaceDetect.LoadModel();
            return  Task.CompletedTask;
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
