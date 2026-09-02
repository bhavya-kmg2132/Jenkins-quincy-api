using System;
using System.Globalization;

namespace Application.ExternalPolicy.PolicyCancellation.Queries.GetPolicyCancellation
{
    public class GetPolicyCancellationValidator : AbstractValidator<GetPolicyCancellationQuery>
    {
        public GetPolicyCancellationValidator()
        {
            RuleFor(x => x.PolicyNumber)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.GetPolicyCancellationValidator_PolicyNumber_NotEmpty);

            RuleFor(x => x.WinsTransactionCode)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.GetPolicyCancellationValidator_WinsTransactionCode_NotEmpty);

            RuleFor(x => x.EffectiveDate)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.GetPolicyCancellationValidator_EffectiveDate_NotEmpty);

            RuleFor(x => x.CancelDate)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.GetPolicyCancellationValidator_CancelDate_NotEmpty);

            RuleFor(x => x.CancelDate)
                .Must((request, cancelDate) =>
                    !TryParseDb2Date(request.EffectiveDate, out var effectiveDate) ||
                    !TryParseDb2Date(cancelDate, out var parsedCancelDate) ||
                    parsedCancelDate >= effectiveDate)
                .WithMessage(Resources.ErrorMessages.GetPolicyCancellationValidator_CancelDate_BeforeEffectiveDate)
                .When(x => !string.IsNullOrEmpty(x.EffectiveDate) && !string.IsNullOrEmpty(x.CancelDate));
        }

        // DB2 sends dates as unseparated "yyyyMMdd" (e.g. "20260401"); generic DateTime.TryParse
        // silently fails on that format, so try it first before falling back.
        private static bool TryParseDb2Date(string value, out DateTime date)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                date = default;
                return false;
            }

            var trimmed = value.Trim();
            return DateTime.TryParseExact(trimmed, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)
                || DateTime.TryParse(trimmed, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
        }
    }
}
