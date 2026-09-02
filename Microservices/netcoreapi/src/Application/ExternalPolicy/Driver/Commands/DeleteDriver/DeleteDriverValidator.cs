namespace Application.ExternalPolicy.Driver.Commands.DeleteDriver
{
    public class DeleteDriverValidator : AbstractValidator<DeleteDriverRequest>
    {
        public DeleteDriverValidator()
        {
            RuleFor(x => x.Drivers)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.DeleteDriverValidator_Drivers_NotEmpty);

            RuleForEach(x => x.Drivers).ChildRules(driver =>
            {
                driver.RuleFor(d => d.PolicyNumber)
                    .NotEmpty()
                    .WithMessage(Resources.ErrorMessages.DeleteDriverValidator_PolicyNumber_NotEmpty);

                driver.RuleFor(d => d.LicenseNumber)
                    .NotEmpty()
                    .WithMessage(Resources.ErrorMessages.DeleteDriverValidator_LicenseNumber_NotEmpty);
            }).When(x => x.Drivers != null);
        }
    }
}
