namespace Application.Policy.Commands.UpdatePolicy
{
    public class UpdatePolicyValidator : AbstractValidator<UpdatePolicyRequest>
    {
        public UpdatePolicyValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.UpdatePolicyValidator_Id_NotEmpty);

            RuleFor(x => x.InsuredName)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.UpdatePolicyValidator_InsuredName_NotEmpty);

            RuleFor(x => x.PolicyType)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.UpdatePolicyValidator_PolicyType_NotEmpty)
                .Must(t => t == "Marine" || t == "Cargo" || t == "Aviation")
                .WithMessage(Resources.ErrorMessages.UpdatePolicyValidator_PolicyType_Invalid)
                .When(x => !string.IsNullOrWhiteSpace(x.PolicyType));

            RuleFor(x => x.EffectiveDate)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.UpdatePolicyValidator_EffectiveDate_NotEmpty);

            RuleFor(x => x.ExpirationDate)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.UpdatePolicyValidator_ExpirationDate_NotEmpty)
                .GreaterThan(x => x.EffectiveDate)
                .WithMessage(Resources.ErrorMessages.UpdatePolicyValidator_ExpirationDate_AfterEffective);
        }
    }
}
