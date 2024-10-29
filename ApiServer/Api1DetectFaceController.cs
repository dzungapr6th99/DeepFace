using DetectFaceObject;
using FaceDetectInterface;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using CommonLib;
using FluentValidation;
using DetectFaceBU;
namespace ApiServer
{
    [Route("detectface")]
    [ApiController]
    public class Api1DetectFaceController : ControllerBase
    {
        public IProcessDetectFaceRequest c_ProcessDetectFaceReq;
        public IValidator<VerifyFaceRequest> c_Validator;
        public Api1DetectFaceController(IProcessDetectFaceRequest p_ProcessDetectFaceReq, IValidator<VerifyFaceRequest> c_DetecFaceRequestValidator)
        {
            c_ProcessDetectFaceReq = p_ProcessDetectFaceReq;
            c_Validator = c_DetecFaceRequestValidator;

        }
        [HttpPost]
        [Route("api1/verifyface")]
        public async Task<VerifyFaceResponse> Api1DetectFaceProcess(VerifyFaceRequest request)
        {
            VerifyFaceResponse response = await c_ProcessDetectFaceReq.Api1DetectFaceBU(request);
            return response;
        }

        [HttpPost]
        [Route("api1/embeding")]
        public async Task<EmbedingFaceResponse> Api2Embeding(EmbedingFaceRequest request)
        {
            return new EmbedingFaceResponse();
        }
    }
}
