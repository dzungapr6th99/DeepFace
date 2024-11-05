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
            RuleFor(x => x.Base64ImgVerify).Must((s, checkBase64ImgVerify) =>
            {
                if (string.IsNullOrWhiteSpace(checkBase64ImgVerify))
                    return false;
                else return true;
            }).WithMessage("'Base64ImgVerify' is not null").WithName("Base64ImgVerify").WithErrorCode("-001");
            RuleFor(x => x.Base64ImgCheck).Must((s, Base64ImgCheck) =>
          {
              if (string.IsNullOrWhiteSpace(Base64ImgCheck))
                  return false;
              else return true;
          }).WithMessage("'Base64ImgVerify' is not null").WithName("Base64ImgVerify").WithErrorCode("-001");
            RuleFor(x => x.RequestID).Must((s, RequestID) =>
          {
              if (string.IsNullOrWhiteSpace(RequestID))
                  return false;
              else return true;
          }).WithMessage("'Base64ImgVerify' is not null").WithName("Base64ImgVerify").WithErrorCode("-001");

        }
    }
}
