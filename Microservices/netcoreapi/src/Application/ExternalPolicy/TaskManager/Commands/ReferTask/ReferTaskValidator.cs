namespace Application.ExternalPolicy.TaskManager.Commands.ReferTask
{
    public class ReferTaskValidator : AbstractValidator<ReferTaskRequest>
    {
        public ReferTaskValidator()
        {
            RuleFor(x => x.PolicyNumber)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.ReferTaskValidator_PolicyNumber_NotEmpty);

            RuleFor(x => x.TaskCode)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.ReferTaskValidator_TaskCode_NotEmpty);

            RuleFor(x => x.ReferredTo)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.ReferTaskValidator_ReferredTo_NotEmpty);
        }
    }
}
