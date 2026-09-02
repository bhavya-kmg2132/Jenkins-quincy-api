namespace Application.ExternalPolicy.Vehicle.Commands.DeleteVehicle
{
    public class DeleteVehicleValidator : AbstractValidator<DeleteVehicleRequest>
    {
        public DeleteVehicleValidator()
        {
            RuleFor(x => x.Vehicles)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.DeleteVehicleValidator_Vehicles_NotEmpty);

            RuleForEach(x => x.Vehicles).ChildRules(vehicle =>
            {
                vehicle.RuleFor(v => v.PolicyNumber)
                    .NotEmpty()
                    .WithMessage(Resources.ErrorMessages.DeleteVehicleValidator_PolicyNumber_NotEmpty);

                vehicle.RuleFor(v => v.VehicleId)
                    .NotEmpty()
                    .WithMessage(Resources.ErrorMessages.DeleteVehicleValidator_VehicleId_NotEmpty);
            }).When(x => x.Vehicles != null);
        }
    }
}
