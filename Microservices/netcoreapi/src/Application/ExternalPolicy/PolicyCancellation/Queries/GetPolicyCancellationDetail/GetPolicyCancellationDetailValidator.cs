using Application.ExternalPolicy.PolicyCancellation;

namespace Application.ExternalPolicy.PolicyCancellation.Queries.GetPolicyCancellationDetail
{
    public class GetPolicyCancellationDetailValidator : AbstractValidator<GetPolicyCancellationDetailQuery>
    {
        public GetPolicyCancellationDetailValidator()
        {
            RuleFor(x => x.PolicyNumber)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.GetPolicyCancellationDetailValidator_PolicyNumber_NotEmpty);

            RuleFor(x => x.WinsTransactionCode)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.GetPolicyCancellationDetailValidator_WinsTransactionCode_NotEmpty);

            RuleFor(x => x.EffectiveDate)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.GetPolicyCancellationDetailValidator_EffectiveDate_NotEmpty);

            RuleFor(x => x.CancellationReason)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.GetPolicyCancellationDetailValidator_CancellationReason_NotEmpty)
                .MaximumLength(2)
                .WithMessage(Resources.ErrorMessages.GetPolicyCancellationDetailValidator_CancellationReason_MaxLength);

            RuleFor(x => x.CancellationReason)
                .Must(x => PolicyCancellationCodes.CancellationReasonCodes.Contains(x))
                .WithMessage(Resources.ErrorMessages.GetPolicyCancellationDetailValidator_CancellationReason_Invalid)
                .When(x => !string.IsNullOrEmpty(x.CancellationReason));

            RuleFor(x => x.CancellationDescription)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.GetPolicyCancellationDetailValidator_CancellationDescription_NotEmpty)
                .MaximumLength(50)
                .WithMessage(Resources.ErrorMessages.GetPolicyCancellationDetailValidator_CancellationDescription_MaxLength);

            // CancellationCarrier / PolicyRetainedByAgency are only required when the cancellation
            // reason means the policy is actually moving to another carrier ("06"/"08").
            RuleFor(x => x.CancellationCarrier)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.GetPolicyCancellationDetailValidator_CancellationCarrier_NotEmpty)
                .When(x => PolicyCancellationCodes.ReasonCodesRequiringCarrierInfo.Contains(x.CancellationReason));

            RuleFor(x => x.CancellationCarrier)
                .Must(x => PolicyCancellationCodes.CancellationCarrierCodes.Contains(x))
                .WithMessage(Resources.ErrorMessages.GetPolicyCancellationDetailValidator_CancellationCarrier_Invalid)
                .When(x => !string.IsNullOrEmpty(x.CancellationCarrier));

            RuleFor(x => x.PolicyRetainedByAgency)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.GetPolicyCancellationDetailValidator_PolicyRetainedByAgency_NotEmpty)
                .When(x => PolicyCancellationCodes.ReasonCodesRequiringCarrierInfo.Contains(x.CancellationReason));

            RuleFor(x => x.PolicyRetainedByAgency)
                .Must(x => x == "Y" || x == "N")
                .WithMessage(Resources.ErrorMessages.GetPolicyCancellationDetailValidator_PolicyRetainedByAgency_Invalid)
                .When(x => !string.IsNullOrEmpty(x.PolicyRetainedByAgency));

            RuleFor(x => x.CancelMethod)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.GetPolicyCancellationDetailValidator_CancelMethod_NotEmpty);

            RuleFor(x => x.CancelMethod)
                .Length(1)
                .WithMessage(Resources.ErrorMessages.GetPolicyCancellationDetailValidator_CancelMethod_Length)
                .Must(x => x == "P" || x == "S")
                .WithMessage(Resources.ErrorMessages.GetPolicyCancellationDetailValidator_CancelMethod_Invalid)
                .When(x => !string.IsNullOrEmpty(x.CancelMethod));
        }
    }
}
