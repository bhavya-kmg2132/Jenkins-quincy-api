//using Application.Common.Utilities;
////using Application.Users.Command.UpdateUserProfile;
//using FluentValidation;
//using System.Collections.Generic;

//namespace Application.Users.Commands.UpdateUserProfile
//{
//    public class UpdateUserProfileValidatorRequest : AbstractValidator</*UpdateUserProfileRequest*/>
//    {
//        /// <summary>
//        /// Validates User update request
//        /// </summary>
//        public UpdateUserProfileValidatorRequest()
//        {

//            //1) oid
//            //It is mandatory.
//            RuleFor(x => x.UserId).Cascade(CascadeMode.Stop)
//              .NotEmpty()
//              .WithMessage(Resources.ErrorMessages.UpdateUserRequestValidator_UserId_NotEmpty);

//            ////2) BranchId
//            ////It is mandatory.
//            //RuleFor(x => x.BranchId).Cascade(CascadeMode.Stop)
//            //   .NotEmpty()
//            //   .WithMessage(Resources.ErrorMessages.UpdateUserRequestValidator_BranchId_NotEmpty);

//            ////3) EPICLookupCode
//            ////It is mandatory.
//            ////Format validation applied.
//            //RuleFor(x => x.EPICLookupCode).Cascade(CascadeMode.Stop)
//            //   .NotEmpty()
//            //   .WithMessage(Resources.ErrorMessages.UpdateUserRequestValidator_EPICLookupCode_NotEmpty);

//            ////3) AccessLevel
//            ////It is mandatory.
//            ////Format validation applied.
//            //When(x => string.IsNullOrEmpty(x.AccessLevel), () =>
//            //{
//            //    RuleFor(x => x.AccessLevel)
//            //   .NotEmpty()
//            //   .WithMessage(Resources.ErrorMessages.UpdateUserRequestValidator_AccessLevel_NotEmpty);
//            //});
//        }
//    }
//}


