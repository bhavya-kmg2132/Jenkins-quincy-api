namespace Application.ExternalPolicy.Notepad.Queries.GetNotepadDetail
{
    public class GetNotepadDetailValidator : AbstractValidator<GetNotepadDetailQuery>
    {
        public GetNotepadDetailValidator()
        {
            RuleFor(x => x.NotepadId)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.GetNotepadDetailValidator_NotepadId_NotEmpty);
        }
    }
}
