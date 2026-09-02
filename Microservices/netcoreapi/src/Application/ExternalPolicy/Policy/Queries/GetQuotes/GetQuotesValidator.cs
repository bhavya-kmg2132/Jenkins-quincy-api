namespace Application.ExternalPolicy.Policy.Queries.GetQuotes
{
    public class GetQuotesValidator : AbstractValidator<GetQuotesQuery>
    {
        public GetQuotesValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(0)
                .WithMessage(Resources.ErrorMessages.GetQuotesValidator_PageNumber_Invalid);

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(0)
                .WithMessage(Resources.ErrorMessages.GetQuotesValidator_PageSize_Invalid);
        }
    }
}
