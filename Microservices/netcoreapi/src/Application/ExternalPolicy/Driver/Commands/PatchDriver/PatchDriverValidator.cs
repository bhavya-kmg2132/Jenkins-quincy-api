namespace Application.ExternalPolicy.Driver.Commands.PatchDriver
{
    public class PatchDriverValidator : AbstractValidator<PatchDriverRequest>
    {
        public PatchDriverValidator()
        {
            RuleFor(x => x.Drivers)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.PatchDriverValidator_Drivers_NotEmpty);

            RuleForEach(x => x.Drivers).ChildRules(driver =>
            {
                driver.RuleFor(d => d.PolicyNumber)
                    .NotEmpty()
                    .WithMessage(Resources.ErrorMessages.PatchDriverValidator_PolicyNumber_NotEmpty);

                driver.RuleFor(d => d.LicenseNumber)
                    .NotEmpty()
                    .WithMessage(Resources.ErrorMessages.PatchDriverValidator_LicenseNumber_NotEmpty);
            }).When(x => x.Drivers != null);
        }
    }
}
