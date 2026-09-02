namespace Application.ExternalPolicy.Vehicle.Commands.DeleteCoverages
{
    public class DeleteCoveragesValidator : AbstractValidator<DeleteCoveragesRequest>
    {
        public DeleteCoveragesValidator()
        {
            RuleFor(x => x.PolicyNumber)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.DeleteCoveragesValidator_PolicyNumber_NotEmpty);

            RuleFor(x => x.EffectiveDate)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.DeleteCoveragesValidator_EffectiveDate_NotEmpty);

            RuleFor(x => x.WinsTransactionCode)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.DeleteCoveragesValidator_WinsTransactionCode_NotEmpty);

            RuleFor(x => x.Location)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.DeleteCoveragesValidator_Location_NotEmpty);
        }
    }
}
