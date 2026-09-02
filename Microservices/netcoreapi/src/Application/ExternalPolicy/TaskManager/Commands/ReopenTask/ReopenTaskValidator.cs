namespace Application.ExternalPolicy.TaskManager.Commands.ReopenTask
{
    public class ReopenTaskValidator : AbstractValidator<ReopenTaskRequest>
    {
        public ReopenTaskValidator()
        {
            RuleFor(x => x.PolicyNumber)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.ReopenTaskValidator_PolicyNumber_NotEmpty);

            RuleFor(x => x.TaskCode)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.ReopenTaskValidator_TaskCode_NotEmpty);
        }
    }
}
