namespace Application.ExternalPolicy.Driver.Queries.GetDriverDetail
{
    public class GetDriverDetailValidator : AbstractValidator<GetDriverDetailQuery>
    {
        public GetDriverDetailValidator()
        {
            RuleFor(x => x.PolicyNumber)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.GetDriverDetailValidator_PolicyNumber_NotEmpty);
        }
    }
}
