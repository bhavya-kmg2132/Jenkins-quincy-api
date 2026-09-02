namespace Application.ExternalPolicy.PolicyCancellation.Commands.HoldTransaction
{
    public class HoldTransactionValidator : AbstractValidator<HoldTransactionRequest>
    {
        public HoldTransactionValidator()
        {
            RuleFor(x => x.PolicyNumber)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.HoldTransactionValidator_PolicyNumber_NotEmpty);

            RuleFor(x => x.WinsTransactionCode)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.HoldTransactionValidator_WinsTransactionCode_NotEmpty);

            RuleFor(x => x.EffectiveDate)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.HoldTransactionValidator_EffectiveDate_NotEmpty);
        }
    }
}
