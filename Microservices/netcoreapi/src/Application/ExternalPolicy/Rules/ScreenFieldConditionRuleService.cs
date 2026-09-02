using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.ExternalPolicy.Models;
using Domain.Dto;
using FluentValidation.Results;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.ExternalPolicy.Rules
{
    public class ScreenFieldConditionRuleService : IScreenFieldConditionRuleService
    {
        private const string DefaultRuleFile = "RuleEngine/ScreenFieldConditions/ScreenFieldConditionRules.json";

        private const string PolicyWorkflow = "PolicyScreenConditions";
        private const string VehicleWorkflow = "VehicleScreenConditions";
        private const string DriverWorkflow = "DriverScreenConditions";

        // Maps each rule to the field it validates, so API consumers get the errors
        // dictionary keyed by the same camelCase name used on the wire (e.g. "towing")
        // instead of the internal rule name or DB2's raw column code.
        private static readonly IReadOnlyDictionary<string, string> RuleFieldMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // PolicyScreenConditions
            ["RelatedPolicyTbdRequiresAnticipatedEffDate"] = "anticipatedEffectiveDate",
            ["AccountCreditRequiresRelatedPolicy"] = "relatedPolicy",
            ["IrpmAdjustmentRangeNonFleet"] = "irpmAdjustment",
            ["IrpmAdjustmentRangeFleet"] = "irpmAdjustment",
            ["IccPucFilingsBlocksQuote"] = "iccPucFilings",
            ["HazardousMaterialsBlocksQuote"] = "hazardousMaterialsTransport",
            ["SnowRemovalRequiresDetail"] = "snowRemovalDetail",
            ["PriorDeclinedRequiresExplanation"] = "priorPolicyDeclinedExplanation",
            ["DeliveryServiceRequiresDetails"] = "deliveryServiceExplanation",
            ["TimeConstraintDeliveriesRequiresExplanation"] = "timeConstraintDeliveriesExplanation",
            ["PersonalAutoDeliveriesRequiresExplanation"] = "personalAutoDeliveriesExplanation",
            ["EndorsementRequiresComments"] = "comments",
            ["CancelReasonRequiresNewCarrierFields"] = "newCarrier",

            // VehicleScreenConditions
            ["PrivatePassengerClassCodeNonFleet"] = "classCode",
            ["PrivatePassengerClassCodeFleet"] = "classCode",
            ["PrivatePassengerSizeUseRadiusHidden"] = "size",
            ["TowingOnlyValidForPrivatePassenger"] = "towing",
            ["TowingRequiresAutoPlusEndorsementOff"] = "towing",
            ["RentalRequiresAutoPlusEndorsementOff"] = "rental",
            ["TrailerSizeHidden"] = "size",
            ["BodilyInjuryAllOrNoneConflict"] = "bodilyInjury",
            ["PipAllOrNoneConflict"] = "pipLimit",
            ["PipLimitOptions"] = "pipLimit",
            ["CollisionDeductibleOptions"] = "collision",
            ["LimitedCollisionDeductibleOptions"] = "limitedCollision",
            ["ComprehensiveDeductibleOptions"] = "comprehensive",
            ["TowingCoverageOptions"] = "towing",
            ["BodilyInjuryOptionsBeforeJuly2025"] = "bodilyInjury",
            ["BodilyInjuryOptionsOnOrAfterJuly2025"] = "bodilyInjury",
            ["OptionalBodilyInjuryOptionsBeforeJuly2025"] = "optionalBodilyInjury",
            ["OptionalBodilyInjuryOptionsOnOrAfterJuly2025"] = "optionalBodilyInjury",
            ["OptionalBodilyInjuryRequiresBodilyInjury"] = "optionalBodilyInjury",
            ["UninsuredNotLowerThanBodilyInjury"] = "uninsured",
            ["UnderinsuredNotLowerThanBodilyInjury"] = "underinsured",
            ["UninsuredExceedsOptionalBodilyInjury"] = "uninsured",
            ["UnderinsuredExceedsOptionalBodilyInjury"] = "underinsured",
            ["PropertyDamageIncludedOnlyWithQualifyingSingleLimitBI"] = "propertyDamage",
            ["PropertyDamageIncludedWithSingleLimitBI"] = "propertyDamage",
            ["PropertyDamageOptionsBeforeJuly2025"] = "propertyDamage",
            ["PropertyDamageOptionsOnOrAfterJuly2025"] = "propertyDamage",
            ["PropertyDamageRequiredWithOptionalBodilyInjury"] = "propertyDamage",
            ["UninsuredOptionsBeforeJuly2025"] = "uninsured",
            ["UninsuredOptionsOnOrAfterJuly2025"] = "uninsured",
            ["UnderinsuredOptionsBeforeJuly2025"] = "underinsured",
            ["UnderinsuredOptionsOnOrAfterJuly2025"] = "underinsured",
            ["MedicalPaymentsOptions"] = "medicalLimit",
            ["MedicalPaymentsRestrictedForTruck"] = "medicalLimit",
            ["FreeformVehicleLicenseState"] = "licenseState",
            ["CollisionAndLimitedCollisionExclusive"] = "collision",
            ["HiredCarExcessBlankClassCode"] = "classCode",
            ["HiredCarExcessPositiveClassCode"] = "classCode",
            ["NonOwnedAddlInsuredClassCode"] = "classCode",
            ["NonOwnedEmployeeCountClassCode"] = "classCode",
            ["NumberOfIndividualsRequiredForDriveOtherCar"] = "numberOfIndividuals",
            ["LoanLeaseRequiresPhysicalDamage"] = "loanLeaseCoverage",
            ["WaiverOfDeductibleRequiresCollision"] = "waiverOfDeductible",
            ["StatedAmountRequiresPhysicalDamage"] = "statedAmount",
            ["CostNewRequiredWithPhysicalDamage"] = "costNew",
            ["PlateNumberCannotContainEmbeddedSpaces"] = "plateNumber",
            ["OemPartsNotAllowedOnVehicleOverTenYears"] = "oemCollision",
            ["AntiTheftRestrictedForMotorcycleAntiqueEquipment"] = "antiTheft",

            // DriverScreenConditions
            ["NonMaLicenseRequiresRegStatus"] = "regStatus",
        };

        private readonly IRuleEngine _ruleEngine;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ScreenFieldConditionRuleService> _logger;

        public ScreenFieldConditionRuleService(IRuleEngine ruleEngine, IConfiguration configuration,
            ILogger<ScreenFieldConditionRuleService> logger)
        {
            _ruleEngine = ruleEngine;
            _configuration = configuration;
            _logger = logger;
        }

        public Task Validate(IEnumerable<PolicyDataTable> policyData)
            => Validate(ScreenFieldRuleMapper.Map(policyData?.Select(t => (t.TableName, (IEnumerable<Dictionary<string, object>>)t.TableValue))));

        public async Task Validate(ScreenRuleEvaluationContext context)
        {
            if (context == null || !context.HasData)
                return;

            var ruleFile = _configuration["RuleEngine:ScreenFieldConditions"] ?? DefaultRuleFile;
            var failures = new List<ValidationFailure>();

            if (context.Policy != null)
            {
                var result = await _ruleEngine.Run(context.Policy, ruleFile, PolicyWorkflow).ConfigureAwait(false);
                failures.AddRange(ToFieldMappedFailures(result));
            }

            foreach (var vehicle in context.Vehicles)
            {
                var result = await _ruleEngine.Run(vehicle, ruleFile, VehicleWorkflow).ConfigureAwait(false);
                failures.AddRange(ToFieldMappedFailures(result));
            }

            foreach (var driver in context.Drivers)
            {
                var result = await _ruleEngine.Run(driver, ruleFile, DriverWorkflow).ConfigureAwait(false);
                failures.AddRange(ToFieldMappedFailures(result));
            }

            failures.AddRange(ValidateNoDuplicateVehiclesOrDrivers(context));

            if (failures.Any())
            {
                _logger.LogWarning("Screen field condition validation failed with {FailureCount} rule failure(s)", failures.Count);
                throw new Application.Common.Exceptions.ValidationException(failures);
            }
        }

        // The same physical vehicle or driver must not appear twice on one quote. The legacy
        // screen compares each new RMV result against the rows already staged; here the whole
        // payload arrives at once, so the equivalent check is for duplicates within it.
        // Virtual vehicle rows share placeholder identifiers ("N/A") and are excluded.
        private static IEnumerable<ValidationFailure> ValidateNoDuplicateVehiclesOrDrivers(ScreenRuleEvaluationContext context)
        {
            var failures = new List<ValidationFailure>();

            var physicalVehicles = context.Vehicles?.Where(v => !v.IsVirtualVehicleEntry).ToList();
            if (physicalVehicles != null && physicalVehicles.Count > 1)
            {
                failures.AddRange(CheckNoDuplicates(
                    physicalVehicles.Select(v => v.VehicleId), "vehicleId", "Duplicate VIN."));
                failures.AddRange(CheckNoDuplicates(
                    physicalVehicles.Select(v => v.PlateNumber), "plateNumber", "Duplicate Plate."));
            }

            if (context.Drivers != null && context.Drivers.Count > 1)
            {
                failures.AddRange(CheckNoDuplicates(
                    context.Drivers.Select(d => d.LicenseNumber), "licenseNumber", "Duplicate License."));
            }

            return failures;
        }

        private static IEnumerable<ValidationFailure> CheckNoDuplicates(
            IEnumerable<string> values, string field, string message)
        {
            var duplicates = values
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value.Trim())
                // "N/A" is the DB2 placeholder written when no real identifier exists yet.
                .Where(value => !string.Equals(value, "N/A", StringComparison.OrdinalIgnoreCase))
                .GroupBy(value => value, StringComparer.OrdinalIgnoreCase)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            if (duplicates.Any())
            {
                yield return new ValidationFailure(field, $"{message} ({string.Join(", ", duplicates)})");
            }
        }

        private static List<ValidationFailure> ToFieldMappedFailures(List<RuleEngineResult> ruleResults)
        {
            var failures = new List<ValidationFailure>();
            CollectFieldMappedFailures(ruleResults, failures);
            return failures;
        }

        private static void CollectFieldMappedFailures(List<RuleEngineResult> ruleResults, List<ValidationFailure> failures)
        {
            foreach (var result in ruleResults)
            {
                if (!result.IsSuccess)
                {
                    var field = RuleFieldMap.TryGetValue(result.RuleName, out var mappedField) ? mappedField : result.RuleName;
                    failures.Add(new ValidationFailure(field, result.Outcome));
                }

                if (result.ChildResult != null && result.ChildResult.Any())
                    CollectFieldMappedFailures(result.ChildResult, failures);
            }
        }
    }
}
