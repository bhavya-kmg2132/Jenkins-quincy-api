namespace Application.ExternalPolicy.Policy.Queries.GetPolicyHistory
{
    public class GetPolicyHistoryValidator : AbstractValidator<GetPolicyHistoryQuery>
    {
        public GetPolicyHistoryValidator()
        {
            RuleFor(x => x.PolicyNumber)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.GetPolicyHistoryValidator_PolicyNumber_NotEmpty);

            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(0)
                .WithMessage(Resources.ErrorMessages.GetPolicyHistoryValidator_PageNumber_Invalid);

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(0)
                .WithMessage(Resources.ErrorMessages.GetPolicyHistoryValidator_PageSize_Invalid);
        }
    }
}
