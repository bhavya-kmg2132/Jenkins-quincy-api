namespace Application.ExternalPolicy.PolicyCancellation.Commands.DeleteTransPolicy
{
    public class DeleteTransPolicyValidator : AbstractValidator<DeleteTransPolicyRequest>
    {
        public DeleteTransPolicyValidator()
        {
            RuleFor(x => x.PolicyNumber)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.DeleteTransPolicyValidator_PolicyNumber_NotEmpty);

            RuleFor(x => x.WinsTransactionCode)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.DeleteTransPolicyValidator_WinsTransactionCode_NotEmpty);

            RuleFor(x => x.EffectiveDate)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.DeleteTransPolicyValidator_EffectiveDate_NotEmpty);
        }
    }
}
