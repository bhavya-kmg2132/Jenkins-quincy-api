namespace Application.ExternalPolicy.Policy.Commands.ChangeTransaction
{
    public class ChangeTransactionValidator : AbstractValidator<ChangeTransactionRequest>
    {
        public ChangeTransactionValidator()
        {
            RuleFor(x => x.PolicyNumber)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.ChangeTransactionValidator_PolicyNumber_NotEmpty);

            RuleFor(x => x.WinsTransactionCode)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.ChangeTransactionValidator_WinsTransactionCode_NotEmpty);

            RuleFor(x => x.EffectiveDate)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.ChangeTransactionValidator_EffectiveDate_NotEmpty);
        }
    }
}
