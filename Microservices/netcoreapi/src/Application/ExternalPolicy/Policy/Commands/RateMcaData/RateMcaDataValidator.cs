namespace Application.ExternalPolicy.Policy.Commands.RateMcaData
{
    public class RateMcaDataValidator : AbstractValidator<RateMcaDataRequest>
    {
        public RateMcaDataValidator()
        {
            RuleFor(x => x.PolicyData)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.RateMcaDataValidator_PolicyData_NotEmpty);
        }
    }
}
