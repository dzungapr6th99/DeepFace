using DetectFaceObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetectFaceBU.Interface
{
    public interface IProcessDetectFaceRequest
    {
        public Task<VerifyFaceResponse> Api1VerifyFaceBU(VerifyFaceRequest request);
        public Task<EmbedingFaceResponse> Api2EmbedingFaceBU(EmbedingFaceRequest request);
        public Task<DetectFaceResponse> Api3DetectFaceBU(DetectFaceRequest request);

    }
}
