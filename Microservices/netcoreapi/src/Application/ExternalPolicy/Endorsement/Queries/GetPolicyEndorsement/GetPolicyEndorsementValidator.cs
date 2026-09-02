namespace Application.ExternalPolicy.Endorsement.Queries.GetPolicyEndorsement
{
    public class GetPolicyEndorsementValidator : AbstractValidator<GetPolicyEndorsementQuery>
    {
        public GetPolicyEndorsementValidator()
        {
            RuleFor(x => x.PolicyNumber)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.GetPolicyEndorsementValidator_PolicyNumber_NotEmpty);

            RuleFor(x => x.WinsTransactionCode)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.GetPolicyEndorsementValidator_WinsTransactionCode_NotEmpty);

            RuleFor(x => x.EffectiveDate)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.GetPolicyEndorsementValidator_EffectiveDate_NotEmpty);

            RuleFor(x => x.EndorsementDate)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.GetPolicyEndorsementValidator_EndorsementDate_NotEmpty);
        }
    }
}
