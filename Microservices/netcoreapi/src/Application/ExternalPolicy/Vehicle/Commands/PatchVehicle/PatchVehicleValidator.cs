namespace Application.ExternalPolicy.Vehicle.Commands.PatchVehicle
{
    public class PatchVehicleValidator : AbstractValidator<PatchVehicleRequest>
    {
        public PatchVehicleValidator()
        {
            RuleFor(x => x.PolicyNumber)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.PatchVehicleValidator_PolicyNumber_NotEmpty);

            RuleFor(x => x.EffectiveDate)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.PatchVehicleValidator_EffectiveDate_NotEmpty);

            RuleFor(x => x.WinsTransactionCode)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.PatchVehicleValidator_WinsTransactionCode_NotEmpty);

            RuleFor(x => x.Vehicles)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.PatchVehicleValidator_Vehicles_NotEmpty);

            RuleForEach(x => x.Vehicles).ChildRules(vehicle =>
            {
                vehicle.RuleFor(v => v.VehicleId)
                    .NotEmpty()
                    .WithMessage(Resources.ErrorMessages.PatchVehicleValidator_VehicleId_NotEmpty);
            }).When(x => x.Vehicles != null);
        }
    }
}
