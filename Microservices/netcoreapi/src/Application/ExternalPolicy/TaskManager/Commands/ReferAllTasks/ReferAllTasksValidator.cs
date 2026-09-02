namespace Application.ExternalPolicy.TaskManager.Commands.ReferAllTasks
{
    public class ReferAllTasksValidator : AbstractValidator<ReferAllTasksRequest>
    {
        public ReferAllTasksValidator()
        {
            RuleFor(x => x.Refers)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.ReferAllTasksValidator_Refers_NotEmpty);

            RuleForEach(x => x.Refers).ChildRules(refer =>
            {
                refer.RuleFor(r => r.PolicyNumber)
                    .NotEmpty()
                    .WithMessage(Resources.ErrorMessages.ReferAllTasksValidator_PolicyNumber_NotEmpty);

                refer.RuleFor(r => r.TaskCode)
                    .NotEmpty()
                    .WithMessage(Resources.ErrorMessages.ReferAllTasksValidator_TaskCode_NotEmpty);

                refer.RuleFor(r => r.ReferredTo)
                    .NotEmpty()
                    .WithMessage(Resources.ErrorMessages.ReferAllTasksValidator_ReferredTo_NotEmpty);
            }).When(x => x.Refers != null);
        }
    }
}
