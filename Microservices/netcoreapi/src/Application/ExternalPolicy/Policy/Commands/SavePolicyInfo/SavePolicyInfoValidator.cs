namespace Application.ExternalPolicy.Policy.Commands.SavePolicyInfo
{
    public class SavePolicyInfoValidator : AbstractValidator<SavePolicyInfoRequest>
    {
        public SavePolicyInfoValidator()
        {
            RuleFor(x => x.PolicyNumber)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.SavePolicyInfoValidator_PolicyNumber_NotEmpty);
        }
    }
}
