namespace Application.ExternalPolicy.Notepad.Commands.CreateNotepad
{
    public class CreateNotepadValidator : AbstractValidator<CreateNotepadRequest>
    {
        public CreateNotepadValidator()
        {
            RuleFor(x => x.PolicyNumber)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.CreateNotepadValidator_PolicyNumber_NotEmpty);

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.CreateNotepadValidator_Title_NotEmpty);
        }
    }
}
