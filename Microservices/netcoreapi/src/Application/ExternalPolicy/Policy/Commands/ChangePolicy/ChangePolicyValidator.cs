namespace Application.ExternalPolicy.Policy.Commands.ChangePolicy
{
    public class ChangePolicyValidator : AbstractValidator<ChangePolicyRequest>
    {
        public ChangePolicyValidator()
        {
            RuleFor(x => x.PolicyNumber)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.ChangePolicyValidator_PolicyNumber_NotEmpty);
        }
    }
}
