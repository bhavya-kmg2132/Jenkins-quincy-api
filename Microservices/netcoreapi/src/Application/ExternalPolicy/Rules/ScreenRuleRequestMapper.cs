using System;
using System.Collections.Generic;
using System.Globalization;
using Application.ExternalPolicy.Driver.Commands.AddDriver;
using Application.ExternalPolicy.Driver.Commands.PatchDriver;
using Application.ExternalPolicy.Models;
using Application.ExternalPolicy.Policy.Commands.PatchPolicy;
using Application.ExternalPolicy.Policy.Commands.SavePolicyInfo;
using Application.ExternalPolicy.Policy.Commands.UpdateUnderwriterQuestions;
using Application.ExternalPolicy.Vehicle.Commands.AddVehicle;
using Application.ExternalPolicy.Vehicle.Commands.PatchVehicle;

namespace Application.ExternalPolicy.Rules
{
    /// <summary>
    /// Adapts the strongly-typed Db2 Vehicle/Driver/Policy command payloads into a
    /// ScreenRuleEvaluationContext, for handlers whose request isn't already a raw
    /// PolicyDataTable payload (that case goes through ScreenFieldRuleMapper instead).
    /// </summary>
    public static class ScreenRuleRequestMapper
    {
        public static ScreenRuleEvaluationContext ForPatchVehicle(PatchVehicleRequest request)
        {
            var context = new ScreenRuleEvaluationContext();
            if (request?.Vehicles == null)
                return context;

            var effectiveDate = ParseDate(request.EffectiveDate);
            var common = request.Common;

            foreach (var item in request.Vehicles)
            {
                context.Vehicles.Add(new VehicleScreenModel
                {
                    EffectiveDate = effectiveDate,
                    VehicleId = item.VehicleId,
                    ModelYear = item.ModelYear,
                    ClassCode = item.ClassCode,
                    VehicleType = ScreenFieldRuleMapper.ResolveVehicleType(item.VehicleType, item.ClassCode, null),
                    PlateNumber = item.PlateNumber,
                    AntiTheft = item.AntiTheft,
                    FleetVehicles = item.FleetYN,
                    Collision = FirstNonEmpty(item.CollisionDeductAmt, common?.CollisionDeductAmt),
                    WaiverOfDeductible = item.CollsnDeductWaiver,
                    LimitedCollision = item.LimitedCollDed,
                    Comprehensive = FirstNonEmpty(item.CompDeductAmount, common?.CompDeductAmount),
                    Rental = item.RentalReimbursement,
                    Towing = item.TowingLimDisab,
                    LoanLeaseCoverage = item.LoanLeaseCoverage,
                    CostNew = item.OriginalCost,
                    Underinsured = item.SplitBiUnderLimit,
                    BodilyInjury = common?.SplitBiLiabLimit,
                    OptionalBodilyInjury = ScreenFieldRuleMapper.MapOptionalBiLimit(common?.OptBiToOthers),
                    PropertyDamage = common?.SplitPdLiabLimit,
                    Uninsured = common?.SplitBiUninsLimit,
                    PipLimit = common?.PipLimit1,
                    MedicalPayments = common?.MedicalLimit
                });
            }

            return context;
        }

        // AddVehicle carries VINs and plate registrations as two separate lists rather than
        // paired vehicle rows; each becomes its own screen row so the existing VEHID/PLATEN
        // duplicate check (blank fields are ignored) covers both without any special-casing.
        public static ScreenRuleEvaluationContext ForAddVehicle(AddVehicleRequest request)
        {
            var context = new ScreenRuleEvaluationContext();
            if (request == null)
                return context;

            var effectiveDate = ParseDate(request.EffectiveDate);

            foreach (var vin in request.VinRequest ?? new List<AddVinItemRequest>())
                context.Vehicles.Add(new VehicleScreenModel { EffectiveDate = effectiveDate, VehicleId = vin.Vin });

            foreach (var registration in request.RegistrationRequest ?? new List<AddRegistrationRequest>())
                context.Vehicles.Add(new VehicleScreenModel { EffectiveDate = effectiveDate, PlateNumber = registration.PlateNumber });

            return context;
        }

        public static ScreenRuleEvaluationContext ForAddDriver(AddDriverRequest request)
        {
            var context = new ScreenRuleEvaluationContext();
            if (request?.DriverRequest == null)
                return context;

            var effectiveDate = ParseDate(request.EffectiveDate);

            foreach (var driver in request.DriverRequest)
            {
                context.Drivers.Add(new DriverScreenModel
                {
                    EffectiveDate = effectiveDate,
                    LicenseNumber = driver.LicenseNumber,
                    LicenseState = driver.LicenseState
                });
            }

            return context;
        }

        public static ScreenRuleEvaluationContext ForPatchDriver(PatchDriverRequest request)
        {
            var context = new ScreenRuleEvaluationContext();
            if (request?.Drivers == null)
                return context;

            foreach (var driver in request.Drivers)
            {
                context.Drivers.Add(new DriverScreenModel
                {
                    EffectiveDate = ParseDate(driver.EffectiveDate),
                    LicenseNumber = driver.LicenseNumber,
                    LicenseState = driver.LicenseState,
                    RegStatus = driver.LicenseStatus
                });
            }

            return context;
        }

        public static ScreenRuleEvaluationContext ForUpdateUnderwriterQuestions(UpdateUnderwriterQuestionsRequest request)
        {
            if (request == null)
                return new ScreenRuleEvaluationContext();

            return new ScreenRuleEvaluationContext
            {
                Policy = new PolicyScreenModel
                {
                    EffectiveDate = ParseDate(request.EffectiveDate),
                    PolicyNumber = request.PolicyNumber,
                    IccPucFilings = request.Filing,
                    HazardousMaterialsTransport = request.Hazard,
                    SnowRemovalForFee = request.SnowPlowOrRemovalFee,
                    SnowRemovalDetail = request.SnowPlowOrRemFeeDesc,
                    PriorPolicyDeclined = request.DeclineCancelNonRen,
                    PriorPolicyDeclinedExplanation = request.DeclineCancelNonRenDesc,
                    DeliveryService = request.DeliverService,
                    DeliveryServiceExplanation = request.DeliverServiceDesc,
                    TimeConstraintDeliveries = request.DeliverTimeLim,
                    TimeConstraintDeliveriesExplanation = request.DeliverTimeLimDesc,
                    PersonalAutoDeliveries = request.PersonalAutoDel,
                    PersonalAutoDeliveriesExplanation = request.PersonalAutoDelDesc
                }
            };
        }

        public static ScreenRuleEvaluationContext ForSavePolicyInfo(SavePolicyInfoRequest request)
        {
            if (request?.UnderwriterQuestions == null && request?.CoverageIndicators == null)
                return new ScreenRuleEvaluationContext();

            return new ScreenRuleEvaluationContext
            {
                Policy = new PolicyScreenModel
                {
                    PolicyNumber = request.PolicyNumber,
                    IccPucFilings = request.UnderwriterQuestions?.IccPucFilings,
                    HazardousMaterialsTransport = request.UnderwriterQuestions?.HazardousMaterialsTransport,
                    SnowRemovalForFee = request.UnderwriterQuestions?.SnowRemovalForFee,
                    FleetVehicles = request.CoverageIndicators?.FleetStatus
                }
            };
        }

        public static ScreenRuleEvaluationContext ForPatchPolicy(PatchPolicyRequest request)
        {
            if (request == null)
                return new ScreenRuleEvaluationContext();

            return new ScreenRuleEvaluationContext
            {
                Policy = new PolicyScreenModel
                {
                    EffectiveDate = ParseDate(request.EffectiveDate),
                    PolicyNumber = request.PolicyNumber,
                    RelatedPolicy = request.RelatedPolicy,
                    AccountCredit = request.Account
                }
            };
        }

        private static string FirstNonEmpty(string primary, string fallback)
            => string.IsNullOrWhiteSpace(primary) ? fallback : primary;

        // Mirrors ScreenFieldRuleMapper.GetDate - DB2 sends effective dates as unseparated
        // "yyyyMMdd" (e.g. "20260401"), which the generic DateTime.TryParse doesn't recognize.
        private static DateTime? ParseDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Trim() == "0")
                return null;

            var trimmed = value.Trim();
            if (DateTime.TryParseExact(trimmed, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var db2Date))
                return db2Date;

            return DateTime.TryParse(trimmed, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed) ? parsed : (DateTime?)null;
        }
    }
}
