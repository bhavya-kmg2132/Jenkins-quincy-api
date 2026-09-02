namespace Application.ExternalPolicy.TaskManager.Queries.GetTasks
{
    public class GetTasksValidator : AbstractValidator<GetTasksQuery>
    {
        public GetTasksValidator()
        {
            RuleFor(x => x.PolicyNumber)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.GetTasksValidator_PolicyNumber_NotEmpty);

            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(0)
                .WithMessage(Resources.ErrorMessages.GetTasksValidator_PageNumber_Invalid);

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(0)
                .WithMessage(Resources.ErrorMessages.GetTasksValidator_PageSize_Invalid);
        }
    }
}
