namespace Application.ExternalPolicy.Policy.Commands.UpdatePolicyInfo
{
    public class UpdatePolicyInfoValidator : AbstractValidator<UpdatePolicyInfoRequest>
    {
        public UpdatePolicyInfoValidator()
        {
            RuleFor(x => x.PolicyData)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.UpdatePolicyInfoValidator_PolicyData_NotEmpty);
        }
    }
}
