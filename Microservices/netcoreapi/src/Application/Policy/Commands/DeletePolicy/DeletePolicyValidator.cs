namespace Application.Policy.Commands.DeletePolicy
{
    public class DeletePolicyValidator : AbstractValidator<DeletePolicyRequest>
    {
        public DeletePolicyValidator()
        {
            RuleFor(x => x.Policy)
                .NotNull()
                .WithMessage("Policy is required");

            When(x => x.Policy != null, () =>
            {
                RuleFor(x => x.Policy.Id)
                    .NotEmpty()
                    .WithMessage(Resources.ErrorMessages.DeletePolicyValidator_Id_NotEmpty);
            });
        }
    }
}
