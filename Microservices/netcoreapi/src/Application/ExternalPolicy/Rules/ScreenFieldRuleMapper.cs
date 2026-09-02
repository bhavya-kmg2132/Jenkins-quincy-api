using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;

namespace Application.ExternalPolicy.Rules
{
    /// <summary>
    /// Everything extracted from one DB2 API payload, ready for rule evaluation.
    /// </summary>
    public class ScreenRuleEvaluationContext
    {
        public PolicyScreenModel Policy { get; set; }
        public List<VehicleScreenModel> Vehicles { get; } = new List<VehicleScreenModel>();
        public List<DriverScreenModel> Drivers { get; } = new List<DriverScreenModel>();

        public bool HasData => Policy != null || Vehicles.Any() || Drivers.Any();
    }

    /// <summary>
    /// Maps the loosely-typed DB2 policy payloads (tableName + rows of column/value pairs)
    /// onto the screen rule models. Column names follow the DB2 tables:
    /// DWXP110PY (policy), DMBPRATPY (policy rating), DMBP155 (underwriter questions),
    /// DMBP130P (vehicles), DMBP131 (drivers). DMBPSTATPY (statistical data) is not validated.
    /// </summary>
    public static class ScreenFieldRuleMapper
    {
        private enum TableKind { Unknown, Policy, Vehicle, Driver, Ignored }

        public static ScreenRuleEvaluationContext Map(IEnumerable<(string TableName, IEnumerable<Dictionary<string, object>> Rows)> tables)
        {
            var context = new ScreenRuleEvaluationContext();
            if (tables == null)
                return context;

            var policy = new PolicyScreenModel();
            bool policyHasData = false;

            foreach (var (tableName, rows) in tables)
            {
                if (rows == null)
                    continue;

                var kind = Classify(tableName);
                if (kind == TableKind.Ignored)
                    continue;

                foreach (var row in rows.Where(r => r != null))
                {
                    var lookup = new Dictionary<string, object>(row, StringComparer.OrdinalIgnoreCase);
                    switch (kind)
                    {
                        case TableKind.Vehicle:
                            context.Vehicles.Add(MapVehicle(lookup));
                            break;
                        case TableKind.Driver:
                            context.Drivers.Add(MapDriver(lookup));
                            break;
                        default:
                            policyHasData |= MergePolicy(policy, lookup);
                            break;
                    }
                }
            }

            if (policyHasData)
                context.Policy = policy;

            PropagatePolicyContext(context, policy);
            return context;
        }

        private static TableKind Classify(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                return TableKind.Unknown;

            var name = tableName.Trim().ToUpperInvariant();

            if (name == "DMBPSTATPY" || name.Contains("STAT"))
                return TableKind.Ignored;
            if (name.Contains("DMBP130") || name.Contains("VEH"))
                return TableKind.Vehicle;
            if (name.Contains("DMBP131") || name.Contains("DRV") || name.Contains("DRIVER"))
                return TableKind.Driver;
            if (name.Contains("DWXP110") || name.Contains("DMBPRATPY") || name.Contains("DMBP155") || name.Contains("POL"))
                return TableKind.Policy;

            return TableKind.Policy;
        }

        private static bool MergePolicy(PolicyScreenModel policy, Dictionary<string, object> row)
        {
            bool found = false;

            found |= Set(row, v => policy.PolicyNumber ??= v, "POLICY", "POLICYNUMBER");
            found |= Set(row, v => policy.TransactionType ??= v, "TRANSTYPE", "TRANSACTIONTYPE");
            found |= Set(row, v => policy.RelatedPolicy ??= v, "RELPOL", "RELATEDPOLICY");
            found |= Set(row, v => policy.AnticipatedEffectiveDate ??= v, "ANTEFFDTE", "ANTICIPATEDEFFDATE", "ANTICIPATEDEFFECTIVEDATE");
            found |= Set(row, v => policy.AccountCredit ??= v, "ACCOUNTCREDIT", "ACCTCREDIT");
            found |= Set(row, v => policy.Comments ??= v, "COMMENTS", "EDSCMT");
            found |= Set(row, v => policy.FleetVehicles ??= v, "FLEET", "FLEETVEHICLES", "FLEETSTATUS");
            found |= Set(row, v => policy.IccPucFilings ??= v, "FILING", "ICCPUCFILINGS", "ICCPUC");
            found |= Set(row, v => policy.HazardousMaterialsTransport ??= v, "HAZARD", "HAZMAT", "HAZARDOUSMATERIALSTRANSPORT");
            found |= Set(row, v => policy.SnowRemovalForFee ??= v, "SNOWPLOW", "SNOWREMOVALFORFEE");
            found |= Set(row, v => policy.SnowRemovalDetail ??= v, "SNOWPLOWD", "SNOWREMOVALDETAIL");
            found |= Set(row, v => policy.PriorPolicyDeclined ??= v, "CANCNON", "PRIORPOLICYDECLINED");
            found |= Set(row, v => policy.PriorPolicyDeclinedExplanation ??= v, "CANCNOND", "PRIORPOLICYDECLINEDEXPLANATION");
            found |= Set(row, v => policy.DeliveryService ??= v, "DELIVER", "DELIVERYSERVICE");
            found |= Set(row, v => policy.DeliveryServiceExplanation ??= v, "DELIVERD", "DELIVERYSERVICEEXPLANATION");
            found |= Set(row, v => policy.TimeConstraintDeliveries ??= v, "DELCON", "TIMECONSTRAINTDELIVERIES");
            found |= Set(row, v => policy.TimeConstraintDeliveriesExplanation ??= v, "DELCOND", "TIMECONSTRAINTDELIVERIESEXPLANATION");
            found |= Set(row, v => policy.PersonalAutoDeliveries ??= v, "DELAUTO", "PERSONALAUTODELIVERIES");
            found |= Set(row, v => policy.PersonalAutoDeliveriesExplanation ??= v, "DELAUTOD", "PERSONALAUTODELIVERIESEXPLANATION");
            found |= Set(row, v => policy.CancelReason ??= v, "CANRSN", "CANCELREASON");
            found |= Set(row, v => policy.NewCarrier ??= v, "NEWCARRIER", "RWTPOL");
            found |= Set(row, v => policy.RetainedByAgency ??= v, "RETAINEDBYAGENCY", "DSPRET");

            var irpm = GetDecimal(row, "IRPM1", "IRPM", "IRPMADJ", "IRPMADJUSTMENT");
            if (irpm.HasValue && !policy.IrpmAdjustment.HasValue)
            {
                // DMBPRATPY stores IRPM as a rating factor (e.g. 0.60 - 1.25); the screen rule
                // is expressed as a +/- percentage adjustment. Convert factors, keep percentages.
                policy.IrpmAdjustment = irpm.Value > 0 && irpm.Value <= 3
                    ? (irpm.Value - 1m) * 100m
                    : irpm.Value;
                found = true;
            }

            var effDate = GetDate(row, "EFFDTE", "EFFDATE", "EFFECTIVEDATE");
            if (effDate.HasValue && !policy.EffectiveDate.HasValue)
            {
                policy.EffectiveDate = effDate;
                found = true;
            }

            return found;
        }

        private static VehicleScreenModel MapVehicle(Dictionary<string, object> row)
        {
            var vehicleName = GetString(row, "VEHNAM", "VEHICLENAME");
            var classCode = GetString(row, "CLASX", "CLASSCD", "CLASSCODE", "CLASS_CODE");

            var vehicle = new VehicleScreenModel
            {
                VehicleId = GetString(row, "VEHID", "VEHICLEID"),
                LocationNumber = GetString(row, "LOCNUM"),
                VehicleName = vehicleName,
                VehicleType = ResolveVehicleType(GetString(row, "VHTYPE", "VEHICLETYPE", "TYPE"), classCode, vehicleName),
                ClassCode = classCode,
                FleetVehicles = GetString(row, "FLEET", "FLEETVEHICLES"),
                Size = GetString(row, "SIZE", "VEHSIZE", "GVCGCW"),
                Use = GetString(row, "VEHUSE", "USE"),
                Radius = GetString(row, "RADIUS"),
                Rental = GetString(row, "RRECOV", "RENTAL"),
                Towing = GetString(row, "TOWLPD", "TOWING"),
                AutoPlusEndorsement = GetString(row, "ENHCOV", "AUTOPLUSENDORSEMENT"),
                FreeformVehicle = GetString(row, "FRMCDE", "FREEFORM", "FREEFORMVEHICLE"),
                LicenseState = GetString(row, "LICSTATE", "REGSTATE", "LICENSESTATE"),
                BodilyInjury = GetString(row, "BILLMD", "BODILYINJURY"),
                OptionalBodilyInjury = MapOptionalBiLimit(GetString(row, "OBILMD", "OPTIONALBI", "OPTIONALBODILYINJURY")),
                PropertyDamage = GetString(row, "PDLLMD", "PROPERTYDAMAGE"),
                Uninsured = GetString(row, "UMBLMD", "UNINSURED"),
                Underinsured = GetString(row, "UNBLMD", "UNDERINSURED"),
                MedicalPayments = GetString(row, "MEDLMD", "MEDICAL", "MEDICALPAYMENTS"),
                PipLimit = GetString(row, "PIPLM1", "PIPLIMIT"),
                Collision = GetString(row, "COLDED", "COLLISION"),
                LimitedCollision = GetString(row, "LCLDED", "LCL", "LIMITEDCOLLISION"),
                Comprehensive = GetString(row, "CMPDED", "COMPREHENSIVE"),
                WaiverOfDeductible = GetString(row, "COLDWV", "WAIVER", "WAIVEROFDEDUCTIBLE"),
                StatedAmount = GetString(row, "SRTCST", "STATEDAMOUNT"),
                LoanLeaseCoverage = GetString(row, "LNLSCOV", "LOANLEASE", "AUTOLOANLEASE"),
                CostNew = GetString(row, "NEWCST", "COSTNEW"),
                OemCollision = GetString(row, "OEMCOL"),
                OemComprehensive = GetString(row, "OEMCMP"),
                ModelYear = GetString(row, "MODLYR", "MODELYEAR"),
                PlateNumber = GetString(row, "PLATEN", "PLATENUMBER"),
                AntiTheft = GetString(row, "ANTTFT", "ANTITHEFT"),
                NonOwnedEmployeeCount = GetDecimal(row, "NEMPL", "NOOFEMPLOYEES", "EMPLOYEECOUNT"),
                EmployeesAsAdditionalInsured = GetString(row, "EMPADDL", "EMPLOYEESASADDLINSURED"),
                ExcessHired = GetDecimal(row, "EXHIRED", "EXCESSHIRED"),
                NumberOfIndividuals = GetString(row, "NINDIV", "NUMBEROFINDIVIDUALS"),
                EffectiveDate = GetDate(row, "EFFDTE", "EFFDATE", "EFFECTIVEDATE")
            };
            return vehicle;
        }

        private static DriverScreenModel MapDriver(Dictionary<string, object> row)
            => new DriverScreenModel
            {
                DriverNumber = GetString(row, "DRVNUM", "DRIVERNUMBER"),
                LicenseNumber = GetString(row, "LICNUM", "LICENSENUMBER"),
                LicenseState = GetString(row, "LICSTE", "LICSTATE", "LICENSESTATE"),
                RegStatus = GetString(row, "LICSTA", "REGSTATUS", "REGSTS"),
                EffectiveDate = GetDate(row, "EFFDTE", "EFFDATE", "EFFECTIVEDATE")
            };

        private static void PropagatePolicyContext(ScreenRuleEvaluationContext context, PolicyScreenModel policy)
        {
            foreach (var vehicle in context.Vehicles)
            {
                vehicle.FleetVehicles ??= policy.FleetVehicles;
                vehicle.EffectiveDate ??= policy.EffectiveDate;
            }

            foreach (var driver in context.Drivers)
                driver.EffectiveDate ??= policy.EffectiveDate;
        }

        // DB2 doesn't reliably keep VHTYPE in sync with the screen's Type selection (e.g.
        // commercial vans stored with VHTYPE=PP). Per the Vehicle Type / Class Code matrix,
        // the assigned Class Code prefix is authoritative: 7xxxxx -> Private Passenger,
        // 6xxxxx -> Service/Utility Trailer, 0-4xxxxx -> Truck. Non-Owned/Hired Car rows
        // aren't a Type selection at all, and a vehicle with no Class Code yet (new quote,
        // not yet rated) falls back to the raw VHTYPE mapping.
        // Internal (not private) so ScreenRuleRequestMapper can apply the same Class-Code-authoritative
        // resolution to strongly-typed requests, which carry the raw VehicleType/ClassCode fields directly.
        internal static string ResolveVehicleType(string rawCode, string classCode, string vehicleName)
        {
            if (!string.IsNullOrWhiteSpace(vehicleName)
                && (vehicleName.IndexOf("NONOWN", StringComparison.OrdinalIgnoreCase) >= 0
                    || vehicleName.IndexOf("HIRED", StringComparison.OrdinalIgnoreCase) >= 0))
            {
                return MapVehicleType(rawCode);
            }

            var prefix = classCode?.Trim();
            if (!string.IsNullOrEmpty(prefix))
            {
                switch (prefix[0])
                {
                    case '7': return "Private Passenger";
                    case '6': return "Service/Utility Trailer";
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                        return "Truck";
                }
            }

            return MapVehicleType(rawCode);
        }

        // Confirmed by the DB2 field mapping response for the 3-option Type dropdown:
        // Private Passenger -> PP, Truck -> PU, Service/Utility Trailer -> UT.
        // Other VHTYPE codes exist in production (AM, AN, LS, TT, MC, MH, RT, VN, UO) but are
        // outside this dropdown's scope, so they pass through unmapped.
        private static string MapVehicleType(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return code;

            switch (code.Trim().ToUpperInvariant())
            {
                case "PP": return "Private Passenger";
                case "PU": return "Truck";
                case "UT": return "Service/Utility Trailer";
                default: return code;
            }
        }

        // DMBP130P stores short internal codes in OBILMD (e.g. "1000") that are not the screen
        // dropdown values; only pass through values that look like screen selections
        // (split limits such as 25/50 or single limits of 100,000 and above).
        // Internal (not private) so ScreenRuleRequestMapper can apply the same filter to
        // strongly-typed requests that pass OBI through unchanged from the client.
        internal static string MapOptionalBiLimit(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            var trimmed = value.Trim();
            if (trimmed.Contains('/'))
                return trimmed;

            var digits = trimmed.Replace(",", string.Empty);
            return digits.Length >= 6 && digits.All(char.IsDigit) ? trimmed : null;
        }

        private static bool Set(Dictionary<string, object> row, Action<string> assign, params string[] aliases)
        {
            var value = GetString(row, aliases);
            if (string.IsNullOrWhiteSpace(value))
                return false;

            assign(value);
            return true;
        }

        private static string GetString(Dictionary<string, object> row, params string[] aliases)
        {
            foreach (var alias in aliases)
            {
                if (row.TryGetValue(alias, out var raw))
                {
                    var value = ToStringValue(raw);
                    if (!string.IsNullOrWhiteSpace(value))
                        return value.Trim();
                }
            }
            return null;
        }

        private static decimal? GetDecimal(Dictionary<string, object> row, params string[] aliases)
        {
            var value = GetString(row, aliases);
            if (value == null)
                return null;

            value = value.Replace(",", string.Empty);
            return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var result)
                ? result
                : (decimal?)null;
        }

        private static DateTime? GetDate(Dictionary<string, object> row, params string[] aliases)
        {
            var value = GetString(row, aliases);
            if (string.IsNullOrWhiteSpace(value) || value.Trim() == "0")
                return null;

            value = value.Trim();
            if (DateTime.TryParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var db2Date))
                return db2Date;
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                return parsed;

            return null;
        }

        private static string ToStringValue(object raw)
        {
            switch (raw)
            {
                case null:
                    return null;
                case string s:
                    return s;
                case JsonElement element:
                    switch (element.ValueKind)
                    {
                        case JsonValueKind.String: return element.GetString();
                        case JsonValueKind.Number: return element.GetRawText();
                        case JsonValueKind.True: return "Y";
                        case JsonValueKind.False: return "N";
                        default: return null;
                    }
                case bool b:
                    return b ? "Y" : "N";
                default:
                    return Convert.ToString(raw, CultureInfo.InvariantCulture);
            }
        }
    }
}
