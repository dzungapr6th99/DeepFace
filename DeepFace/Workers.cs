using FaceDetectInterface;
using PreProcess;
using PreProcess.Interface;
using FaceDetectInterface.Interface;
using MilvusDA.Interface;
namespace DeepFace
{
    public class Workers:BackgroundService
    {
        private IDetectorModel _detectorModel;
        private IFaceDetect _faceDetect;
        private IMilvusHelper _milvusHelper;
        public Workers(IDetectorModel p_DetectorModel,  IFaceDetect p_FaceDetect, IMilvusHelper milvusHelper)
        {
            _detectorModel = p_DetectorModel;
            _faceDetect = p_FaceDetect;
            _milvusHelper = milvusHelper;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _detectorModel.LoadModel();
            _faceDetect.LoadModel();
            _milvusHelper.StartManageCollection();
            return  Task.CompletedTask;
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
