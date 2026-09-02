using Application.Users.Commands.AddRoles;

namespace Application.Users.Commands.UserActivity
{
    public class AddUserActivityValidatorRequest : AbstractValidator<AddUserActivityRequest>
    {
        /// <summary>
        /// Validates UserActivity Add request
        /// </summary>
        public AddUserActivityValidatorRequest()
        {
            //1) UserId
            //It is mandatory.
            RuleFor(x => x.UserId)
              .NotEmpty()
              .WithMessage(Resources.ErrorMessages.AddUserActivityRequestValidator_UserId_NotEmpty);
        }
    }
}


