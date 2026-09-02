namespace Application.ExternalPolicy.Driver.Commands.AddDriver
{
    public class AddDriverValidator : AbstractValidator<AddDriverRequest>
    {
        public AddDriverValidator()
        {
            RuleFor(x => x.PolicyNumber)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.AddDriverValidator_PolicyNumber_NotEmpty);

            RuleFor(x => x.EffectiveDate)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.AddDriverValidator_EffectiveDate_NotEmpty);

            RuleFor(x => x.WinsTransactionCode)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.AddDriverValidator_WinsTransactionCode_NotEmpty);

            RuleFor(x => x.DriverRequest)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.AddDriverValidator_DriverRequest_NotEmpty);
        }
    }
}
