namespace Application.ExternalPolicy.Vehicle.Queries.GetVehicleDetail
{
    public class GetVehicleDetailValidator : AbstractValidator<GetVehicleDetailQuery>
    {
        public GetVehicleDetailValidator()
        {
            RuleFor(x => x.PolicyNumber)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.GetVehicleDetailValidator_PolicyNumber_NotEmpty);
        }
    }
}
