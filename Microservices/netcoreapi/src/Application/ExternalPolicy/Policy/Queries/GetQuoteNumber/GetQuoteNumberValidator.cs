namespace Application.ExternalPolicy.Policy.Queries.GetQuoteNumber
{
    public class GetQuoteNumberValidator : AbstractValidator<GetQuoteNumberQuery>
    {
        public GetQuoteNumberValidator()
        {
            RuleFor(x => x.WinsProductCode)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.GetQuoteNumberValidator_WinsProductCode_NotEmpty);

            RuleFor(x => x.SubSystem)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.GetQuoteNumberValidator_SubSystem_NotEmpty);

            RuleFor(x => x.WinsTransactionCode)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.GetQuoteNumberValidator_WinsTransactionCode_NotEmpty);
        }
    }
}
