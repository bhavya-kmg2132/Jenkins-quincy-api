namespace Application.ExternalPolicy.TaskManager.Commands.UpdateTask
{
    public class UpdateTaskValidator : AbstractValidator<UpdateTaskRequest>
    {
        public UpdateTaskValidator()
        {
            RuleFor(x => x.PolicyNumber)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.UpdateTaskValidator_PolicyNumber_NotEmpty);

            RuleFor(x => x.TaskCode)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.UpdateTaskValidator_TaskCode_NotEmpty);
        }
    }
}
