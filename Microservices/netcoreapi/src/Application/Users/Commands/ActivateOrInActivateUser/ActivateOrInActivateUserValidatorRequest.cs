namespace Application.Users.Commands.ActivateOrInActivateUser
{
    public class ActivateOrInActivateUserValidatorRequest : AbstractValidator<ActivateOrInActivateUserRequest>
    {
        /// <summary>
        /// Validates User create request
        /// </summary>
        public ActivateOrInActivateUserValidatorRequest()
        {
            //1) Id
            //It is mandatory.
            //RuleFor(x => x.Id).Cascade(CascadeMode.Stop)
            //   .NotEmpty()
            //   .WithMessage(Resources.ErrorMessages.ActivateOrInActivateUserRequestValidator_Id_NotEmpty);

            //2) oid
            //It is mandatory.
            //RuleFor(x => x.oid).Cascade(CascadeMode.Stop)
            //  .NotEmpty()
            //  .WithMessage(Resources.ErrorMessages.ActivateOrInActivateUserRequestValidator_oid_NotEmpty);

            ////3) dipslay_name
            ////It is mandatory.
            //RuleFor(x => x.display_name).Cascade(CascadeMode.Stop)
            //   .NotEmpty()
            //   .WithMessage(Resources.ErrorMessages.ActivateOrInActivateUserRequestValidator_display_name_NotEmpty);

            ////4) Email
            ////It is mandatory.
            ////Format validation applied.
            //RuleFor(x => x.Email).Cascade(CascadeMode.Stop)
            //   .NotEmpty()
            //   .WithMessage(Resources.ErrorMessages.ActivateOrInActivateUserRequestValidator_Email_NotEmpty)
            //   .Matches(RegexConstant.email)
            //   .WithMessage(Resources.ErrorMessages.CreateProspectRequestValidator_EmailAddress_Format);

            ////5) Mobile
            ////It is mandatory.
            //RuleFor(x => x.Mobile).Cascade(CascadeMode.Stop)
            //   .NotEmpty()
            //   .WithMessage(Resources.ErrorMessages.CreateDialsRequestValidator_Mobile_NotEmpty);

            ////6) FirstName
            ////It is mandatory.
            //RuleFor(x => x.FirstName).Cascade(CascadeMode.Stop)
            //   .NotEmpty()
            //   .WithMessage(Resources.ErrorMessages.CreateDialsRequestValidator_FirstName_NotEmpty);

            ////7) LadtName
            ////It is mandatory.
            //RuleFor(x => x.LastName).Cascade(CascadeMode.Stop)
            //   .NotEmpty()
            //   .WithMessage(Resources.ErrorMessages.CreateDialsRequestValidator_LastName_NotEmpty);
        }
    }
}


