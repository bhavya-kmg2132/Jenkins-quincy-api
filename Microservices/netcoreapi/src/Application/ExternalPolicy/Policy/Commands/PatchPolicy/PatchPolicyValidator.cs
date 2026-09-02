namespace Application.ExternalPolicy.Policy.Commands.PatchPolicy
{
    public class PatchPolicyValidator : AbstractValidator<PatchPolicyRequest>
    {
        public PatchPolicyValidator()
        {
            RuleFor(x => x.PolicyNumber)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.PatchPolicyValidator_PolicyNumber_NotEmpty);
        }
    }
}
