namespace Application.ExternalPolicy.Notepad.Commands.UpdateNotepad
{
    public class UpdateNotepadValidator : AbstractValidator<UpdateNotepadRequest>
    {
        public UpdateNotepadValidator()
        {
            RuleFor(x => x.NotepadId)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.UpdateNotepadValidator_NotepadId_NotEmpty);
        }
    }
}
