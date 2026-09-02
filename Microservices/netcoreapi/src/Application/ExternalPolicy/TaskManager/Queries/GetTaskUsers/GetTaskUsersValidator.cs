namespace Application.ExternalPolicy.TaskManager.Queries.GetTaskUsers
{
    public class GetTaskUsersValidator : AbstractValidator<GetTaskUsersQuery>
    {
        public GetTaskUsersValidator()
        {
            RuleFor(x => x.TaskRole)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.GetTaskUsersValidator_TaskRole_NotEmpty);
        }
    }
}
