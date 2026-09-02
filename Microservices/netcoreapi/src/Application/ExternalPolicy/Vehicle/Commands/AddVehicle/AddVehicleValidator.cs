namespace Application.ExternalPolicy.Vehicle.Commands.AddVehicle
{
    public class AddVehicleValidator : AbstractValidator<AddVehicleRequest>
    {
        public AddVehicleValidator()
        {
            RuleFor(x => x.PolicyNumber)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.AddVehicleValidator_PolicyNumber_NotEmpty);

            RuleFor(x => x.EffectiveDate)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.AddVehicleValidator_EffectiveDate_NotEmpty);

            RuleFor(x => x.WinsTransactionCode)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.AddVehicleValidator_WinsTransactionCode_NotEmpty);

            RuleFor(x => x)
                .Must(r => (r.VinRequest != null && r.VinRequest.Count > 0) ||
                           (r.RegistrationRequest != null && r.RegistrationRequest.Count > 0))
                .WithMessage(Resources.ErrorMessages.AddVehicleValidator_VehicleData_Required);
        }
    }
}
