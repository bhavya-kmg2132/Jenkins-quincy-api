namespace Application.ExternalPolicy.Notepad.Queries.GetNotepads
{
    public class GetNotepadsValidator : AbstractValidator<GetNotepadsQuery>
    {
        public GetNotepadsValidator()
        {
            RuleFor(x => x.PolicyNumber)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.GetNotepadsValidator_PolicyNumber_NotEmpty);

            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(0)
                .WithMessage(Resources.ErrorMessages.GetNotepadsValidator_PageNumber_Invalid);

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(0)
                .WithMessage(Resources.ErrorMessages.GetNotepadsValidator_PageSize_Invalid);
        }
    }
}
