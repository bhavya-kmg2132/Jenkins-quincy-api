namespace Application.ExternalPolicy.TaskManager.Queries.GetTaskDetail
{
    public class GetTaskDetailValidator : AbstractValidator<GetTaskDetailQuery>
    {
        public GetTaskDetailValidator()
        {
            RuleFor(x => x.PolicyNumber)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.GetTaskDetailValidator_PolicyNumber_NotEmpty);

            RuleFor(x => x.TaskCode)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.GetTaskDetailValidator_TaskCode_NotEmpty);
        }
    }
}
