using DetectFaceObject;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiServer.Validation
{
    public class Api1DetectFaceValidation : AbstractValidator<VerifyFaceRequest>
    {
        public Api1DetectFaceValidation()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(x => x.ImageVerify).Must((s, checkBase64ImgVerify) =>
            {
                if (checkBase64ImgVerify == null | checkBase64ImgVerify.Length == 0)
                    return false;
                else return true;
            }).WithMessage("'ImageVerify' is not null").WithName("ImageVerify").WithErrorCode("-001");
            RuleFor(x => x.ImageCheck).Must((s, Base64ImgCheck) =>
          {
              if (Base64ImgCheck == null || Base64ImgCheck.Length == 0)
                  return false;
              else return true;
          }).WithMessage("'ImageCheck' is not null").WithName("ImageCheck").WithErrorCode("-001");
            RuleFor(x => x.RequestID).Must((s, RequestID) =>
          {
              if (string.IsNullOrWhiteSpace(RequestID))
                  return false;
              else return true;
          }).WithMessage("'RequestID' is not null").WithName("RequestID").WithErrorCode("-001");

        }
    }
}
