namespace Application.Policy.Commands.CreatePolicy
{
    public class CreatePolicyValidator : AbstractValidator<CreatePolicyRequest>
    {
        public CreatePolicyValidator()
        {
            RuleFor(x => x.InsuredName)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.CreatePolicyValidator_InsuredName_NotEmpty);

            RuleFor(x => x.PolicyType)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.CreatePolicyValidator_PolicyType_NotEmpty)
                .Must(t => t == "Marine" || t == "Cargo" || t == "Aviation")
                .WithMessage(Resources.ErrorMessages.CreatePolicyValidator_PolicyType_Invalid)
                .When(x => !string.IsNullOrWhiteSpace(x.PolicyType));

            RuleFor(x => x.EffectiveDate)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.CreatePolicyValidator_EffectiveDate_NotEmpty);

            RuleFor(x => x.ExpirationDate)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.CreatePolicyValidator_ExpirationDate_NotEmpty)
                .GreaterThan(x => x.EffectiveDate)
                .WithMessage(Resources.ErrorMessages.CreatePolicyValidator_ExpirationDate_AfterEffective);
        }
    }
}
