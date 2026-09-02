namespace Application.ExternalPolicy.Vehicle.Commands.AddCoverages
{
    public class AddCoveragesValidator : AbstractValidator<AddCoveragesRequest>
    {
        public AddCoveragesValidator()
        {
            RuleFor(x => x.PolicyNumber)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.AddCoveragesValidator_PolicyNumber_NotEmpty);

            RuleFor(x => x.EffectiveDate)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.AddCoveragesValidator_EffectiveDate_NotEmpty);

            RuleFor(x => x.WinsTransactionCode)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.AddCoveragesValidator_WinsTransactionCode_NotEmpty);

            RuleFor(x => x.Location)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.AddCoveragesValidator_Location_NotEmpty);
        }
    }
}
