using System;
using System.Globalization;
using System.Linq;

namespace Application.ExternalPolicy.Rules
{
    /// <summary>
    /// Base for the screen-field rule engine inputs. Exposes the null-safe helpers the
    /// lambda expressions in RuleEngine/ScreenFieldConditions/ScreenFieldConditionRules.json rely on,
    /// so every rule is a no-op unless its triggering field is present in the DB2 payload.
    /// </summary>
    public abstract class ScreenRuleModelBase
    {
        // Qol2.0 coverage option cutover ("before / after 1 July 2025" in the requirement sheet).
        private static readonly DateTime CoverageCutoverDate = new DateTime(2025, 7, 1);

        public DateTime? EffectiveDate { get; set; }

        public bool HasEffectiveDate => EffectiveDate.HasValue;

        public bool IsEffectiveOnOrAfterJuly2025 => EffectiveDate.HasValue && EffectiveDate.Value >= CoverageCutoverDate;

        public bool HasValue(string value) => !string.IsNullOrWhiteSpace(value);

        public bool Is(string value, string expected)
            => value != null && string.Equals(Normalize(value), Normalize(expected), StringComparison.OrdinalIgnoreCase);

        public bool In(string value, string pipeSeparatedOptions)
            => !string.IsNullOrWhiteSpace(value)
               && pipeSeparatedOptions.Split('|').Any(option => string.Equals(Normalize(option), Normalize(value), StringComparison.OrdinalIgnoreCase));

        public bool IsYes(string value)
            => Is(value, "Y") || Is(value, "Yes") || Is(value, "True") || Is(value, "1");

        public bool IsNo(string value)
            => Is(value, "N") || Is(value, "No") || Is(value, "False") || Is(value, "0");

        public bool Contains(string value, string fragment)
            => value != null && value.IndexOf(fragment, StringComparison.OrdinalIgnoreCase) >= 0;

        public bool InRange(decimal? value, decimal min, decimal max)
            => value.HasValue && value.Value >= min && value.Value <= max;

        // Named IsAbove because "GreaterThan" is a reserved operator alias in the
        // Dynamic LINQ parser used by RulesEngine expressions.
        public bool IsAbove(decimal? value, decimal min)
            => value.HasValue && value.Value > min;

        // A limit is treated as "blank" (no coverage selected) whether the field was
        // omitted entirely or the screen option is explicitly "No Coverage".
        public bool IsBlank(string value)
            => !HasValue(value) || Is(value, "No Coverage");

        // Split limits (e.g. "25/50") are DB2 shorthand for thousands ($25,000/$50,000);
        // single limits (e.g. "100,000") are already stated in full dollars. The
        // per-accident (second) figure of a split limit is the comparable "ceiling" for
        // cross-field checks, so both formats are normalized to a real-dollar amount here.
        public decimal? EffectiveLimitValue(string value)
        {
            if (IsBlank(value) || Is(value, "Included"))
                return null;

            var trimmed = value.Trim();
            if (trimmed.Contains('/'))
            {
                var parts = trimmed.Split('/');
                var perAccident = ParseDecimal(parts[parts.Length - 1]);
                return perAccident.HasValue ? perAccident.Value * 1000m : (decimal?)null;
            }

            return ParseDecimal(trimmed);
        }

        public bool IsHigherThan(string value, string other)
        {
            var left = EffectiveLimitValue(value);
            var right = EffectiveLimitValue(other);
            return left.HasValue && right.HasValue && left.Value > right.Value;
        }

        public bool IsLowerThan(string value, string other)
            => IsHigherThan(other, value);

        public bool IsLimitAbove(string value, decimal threshold)
        {
            var parsed = EffectiveLimitValue(value);
            return parsed.HasValue && parsed.Value > threshold;
        }

        // Money/count fields (Cost New, Stated Amount) are "not entered" when omitted or zero;
        // DB2 writes an unset numeric as "0" rather than leaving it blank.
        public bool IsPositiveAmount(string value)
        {
            var parsed = ParseWholeNumber(value);
            return parsed.HasValue && parsed.Value > 0m;
        }

        public bool IsZeroOrBlank(string value) => !IsPositiveAmount(value);

        // A plate number may not contain spaces between characters (leading/trailing are
        // trimmed by DB2's fixed-width padding and are not an error).
        public bool HasEmbeddedSpace(string value)
            => HasValue(value) && value.Trim().Contains(' ');

        protected static decimal? ParseWholeNumber(string value)
            => ParseDecimal(value);

        private static decimal? ParseDecimal(string value)
        {
            var digits = value?.Replace(",", string.Empty).Replace(" ", string.Empty).Trim();
            return decimal.TryParse(digits, NumberStyles.Number, CultureInfo.InvariantCulture, out var result)
                ? result
                : (decimal?)null;
        }

        // DB2 stores limits without thousands separators (e.g. 100000) while the screen
        // dropdowns show "100,000" - compare with commas and spaces removed on both sides.
        private static string Normalize(string value)
            => value?.Replace(",", string.Empty).Replace(" ", string.Empty).Trim();
    }

    /// <summary>
    /// Policy-level screen fields, merged from the DB2 policy tables:
    /// DWXP110PY (policy), DMBPRATPY (policy rating) and DMBP155 (underwriter questions).
    /// </summary>
    public class PolicyScreenModel : ScreenRuleModelBase
    {
        public string PolicyNumber { get; set; }
        public string TransactionType { get; set; }

        // Policy Information screen (DWXP110PY: RELPOL, ACCOUNT)
        public string RelatedPolicy { get; set; }
        public string AnticipatedEffectiveDate { get; set; }
        public string AccountCredit { get; set; }
        public string Comments { get; set; }

        // Premium Transaction Summary (DMBPRATPY: FLEET, IRPM1 - factor converted to +/- percent)
        public string FleetVehicles { get; set; }
        public decimal? IrpmAdjustment { get; set; }

        // RMV Prefill questions (DMBP155: FILING, HAZARD, SNOWPLOW/SNOWPLOWD)
        public string IccPucFilings { get; set; }
        public string HazardousMaterialsTransport { get; set; }
        public string SnowRemovalForFee { get; set; }
        public string SnowRemovalDetail { get; set; }

        // ACORD Application Questionnaire (DMBP155: CANCNON/CANCNOND)
        public string PriorPolicyDeclined { get; set; }
        public string PriorPolicyDeclinedExplanation { get; set; }

        // Quincy Supplemental Application (DMBP155: DELIVER/DELIVERD, DELCON/DELCOND, DELAUTO/DELAUTOD)
        public string DeliveryService { get; set; }
        public string DeliveryServiceExplanation { get; set; }
        public string TimeConstraintDeliveries { get; set; }
        public string TimeConstraintDeliveriesExplanation { get; set; }
        public string PersonalAutoDeliveries { get; set; }
        public string PersonalAutoDeliveriesExplanation { get; set; }

        // Cancellation screen (DWXP110PY: CANRSN, RWTPOL, DSPRET)
        public string CancelReason { get; set; }
        public string NewCarrier { get; set; }
        public string RetainedByAgency { get; set; }
    }

    /// <summary>
    /// Vehicle Information screen fields - one instance per DMBP130P row. Red Non-Owned
    /// ("NONOWN-*") and Hired Car rows carry their own class-code rules (NEMPL/EMPADDL/EXHIRED).
    /// </summary>
    public class VehicleScreenModel : ScreenRuleModelBase
    {
        public string VehicleId { get; set; }
        public string LocationNumber { get; set; }
        public string VehicleName { get; set; }
        public string VehicleType { get; set; }
        public string ClassCode { get; set; }
        public string FleetVehicles { get; set; }
        public string Size { get; set; }
        public string Use { get; set; }
        public string Radius { get; set; }
        public string Rental { get; set; }
        public string Towing { get; set; }
        public string FreeformVehicle { get; set; }
        public string LicenseState { get; set; }

        // Auto Plus Endorsement (DMBP130P: ENHCOV) - confirmed by the DB2 field mapping
        // response: Rental/Towing coverage conflicts with this endorsement being active.
        public string AutoPlusEndorsement { get; set; }

        // Coverages (DMBP130P: BILLMD, OBILMD, PDLLMD, UMBLMD, UNBLMD, COLDED, LCLDED)
        public string BodilyInjury { get; set; }
        public string OptionalBodilyInjury { get; set; }
        public string PropertyDamage { get; set; }
        public string Uninsured { get; set; }
        public string Underinsured { get; set; }
        public string Collision { get; set; }
        public string LimitedCollision { get; set; }

        // Medical Payments (DMBP130P: MEDLMD) - capped at $5,000 for Type = Truck.
        public string MedicalPayments { get; set; }

        // PIP (DMBP130P: PIPLM1) - compulsory in MA; "No Coverage" alongside Collision is
        // an All-or-None conflict.
        public string PipLimit { get; set; }

        // Physical Damage (DMBP130P: CMPDED, COLDWV, SRTCST, LNLSCOV, NEWCST)
        public string Comprehensive { get; set; }
        public string WaiverOfDeductible { get; set; }
        public string StatedAmount { get; set; }
        public string LoanLeaseCoverage { get; set; }
        public string CostNew { get; set; }

        // OEM parts endorsements (DMBP130P: OEMCOL, OEMCMP) - not allowed past 10 years.
        public string OemCollision { get; set; }
        public string OemComprehensive { get; set; }

        // Registration (DMBP130P: MODLYR, PLATEN, ANTTFT)
        public string ModelYear { get; set; }
        public string PlateNumber { get; set; }
        public string AntiTheft { get; set; }

        // Non-Owned / Hired Car entries (DMBP130P: VEHNAM, NEMPL, EMPADDL, EXHIRED)
        public decimal? NonOwnedEmployeeCount { get; set; }
        public string EmployeesAsAdditionalInsured { get; set; }
        public decimal? ExcessHired { get; set; }

        // Drive Other Car entry (DMBP130P: NINDIV) - number of individuals covered.
        public string NumberOfIndividuals { get; set; }

        public bool IsNonOwnedEntry => Contains(VehicleName, "NONOWN");
        public bool IsHiredCarEntry => Contains(VehicleName, "HIRED");
        public bool IsDriveOtherCarEntry => Contains(VehicleName, "DRIVE OTHER CAR");

        // Non-Owned / Hired / Drive Other Car rows are policy-level coverage exposures the
        // Product Processor creates, not real cars - they carry no VIN, plate or Cost New.
        public bool IsVirtualVehicleEntry => IsNonOwnedEntry || IsHiredCarEntry || IsDriveOtherCarEntry;

        public bool HasComprehensive => !IsBlank(Comprehensive);
        public bool HasCollision => !IsBlank(Collision);
        public bool HasLimitedCollision => !IsBlank(LimitedCollision);
        public bool HasPhysicalDamage => HasComprehensive || HasCollision || HasLimitedCollision;

        // "All or None" is the Massachusetts rule that the compulsory coverages are taken
        // together or not at all. A row carrying no coverage at all is simply not configured
        // yet and is left alone; a row carrying other coverages while Bodily Injury is blank
        // is the conflict the screen rejects.
        public bool HasAnyCoverageSelected =>
            !IsBlank(PropertyDamage)
            || !IsBlank(Uninsured)
            || !IsBlank(Underinsured)
            || !IsBlank(PipLimit)
            || !IsBlank(MedicalPayments)
            || HasPhysicalDamage;

        // Vehicle age drives the OEM-parts eligibility cut-off. Derived from the model year
        // against the transaction effective date, matching the legacy "AGE OF VEHICLE" calc.
        public int? VehicleAge
        {
            get
            {
                if (!EffectiveDate.HasValue)
                    return null;

                var year = ParseWholeNumber(ModelYear);
                return year.HasValue ? EffectiveDate.Value.Year - (int)year.Value : (int?)null;
            }
        }

        public bool IsOlderThan(int years) => VehicleAge.HasValue && VehicleAge.Value > years;
    }

    /// <summary>Driver Information screen fields - one instance per DMBP131 row.</summary>
    public class DriverScreenModel : ScreenRuleModelBase
    {
        public string DriverNumber { get; set; }
        public string LicenseNumber { get; set; }
        public string LicenseState { get; set; }
        public string RegStatus { get; set; }
    }
}
