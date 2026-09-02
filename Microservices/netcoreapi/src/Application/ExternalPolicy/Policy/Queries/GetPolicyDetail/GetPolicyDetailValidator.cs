namespace Application.ExternalPolicy.Policy.Queries.GetPolicyDetail
{
    public class GetPolicyDetailValidator : AbstractValidator<GetPolicyDetailQuery>
    {
        public GetPolicyDetailValidator()
        {
            RuleFor(x => x.PolicyNumber)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.GetPolicyDetailValidator_PolicyNumber_NotEmpty);
        }
    }
}
