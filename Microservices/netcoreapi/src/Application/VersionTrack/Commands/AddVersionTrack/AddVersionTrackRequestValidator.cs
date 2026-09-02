using Application.Common.Interfaces;

namespace Application.VersionTrack.Commands.AddVersionTrack
{
    public class AddVersionTrackRequestValidator : AbstractValidator<AddVersionTrackRequest>
    {
        /// <summary>
        /// Validates VersionTrack Add request
        /// </summary>
        public AddVersionTrackRequestValidator(IVersionTrackDataAccess VersionTrackDataAcces)
        {
            ////1)Name
            ////It is mandatory.
            //RuleFor(x => x.Name)
            //  .NotEmpty()
            //  //.WithMessage(Resources.ErrorMessages.VersionTrackAddRequestValidator_FirstName_NotEmpty)
            //  .MaximumLength(150)
            //  //.WithMessage(Resources.ErrorMessages.VersionTrackAddRequestValidator_FirstName_Length);
        }
    }
}





