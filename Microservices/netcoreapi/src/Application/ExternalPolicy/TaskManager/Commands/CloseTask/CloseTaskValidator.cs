namespace Application.ExternalPolicy.TaskManager.Commands.CloseTask
{
    public class CloseTaskValidator : AbstractValidator<CloseTaskRequest>
    {
        public CloseTaskValidator()
        {
            RuleFor(x => x.PolicyNumber)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.CloseTaskValidator_PolicyNumber_NotEmpty);

            RuleFor(x => x.TaskCode)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.CloseTaskValidator_TaskCode_NotEmpty);
        }
    }
}
