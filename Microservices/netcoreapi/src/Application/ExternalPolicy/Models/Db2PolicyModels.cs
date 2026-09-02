using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Application.ExternalPolicy.Models
{
    // A DB2 policy/vehicle/coverage table: a name (e.g. "DMBP130P") plus its rows, each a
    // free-form column/value map. Used as-is for RateMCAData and updatepolicyinfo since the
    // set of DB2 columns varies by table and isn't modeled field-by-field on this side.
    public class PolicyDataTable
    {
        [JsonPropertyName("tableName")]
        public string TableName { get; set; }

        [JsonPropertyName("tableValue")]
        public List<Dictionary<string, object>> TableValue { get; set; }
    }

    public class AddVinItemRequest
    {
        [JsonPropertyName("vin")]
        public string Vin { get; set; }
    }

    public class AddRegistrationRequest
    {
        [JsonPropertyName("plateNumber")]
        public string PlateNumber { get; set; }

        [JsonPropertyName("plateType")]
        public string PlateType { get; set; }
    }

    public class DeleteVehicleItemRequest
    {
        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("effectiveDate")]
        public string EffectiveDate { get; set; }

        [JsonPropertyName("winsTransactionCode")]
        public string WinsTransactionCode { get; set; }

        [JsonPropertyName("vehicleId")]
        public string VehicleId { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }
    }

    public class DeleteDriverItem
    {
        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("effectiveDate")]
        public string EffectiveDate { get; set; }

        [JsonPropertyName("winsTransactionCode")]
        public string WinsTransactionCode { get; set; }

        [JsonPropertyName("licenseNumber")]
        public string LicenseNumber { get; set; }
    }

    public class DriverLicenseRequest
    {
        [JsonPropertyName("licenseNumber")]
        public string LicenseNumber { get; set; }

        [JsonPropertyName("licenseState")]
        public string LicenseState { get; set; }
    }

    // Shared shape for a single task referral, reused standalone (ReferTask) and as the
    // array item for a bulk referral (ReferAllTasks).
    public class ReferTaskItem
    {
        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("effectiveDate")]
        public string EffectiveDate { get; set; }

        [JsonPropertyName("winsTransactionCode")]
        public string WinsTransactionCode { get; set; }

        [JsonPropertyName("taskCode")]
        public string TaskCode { get; set; }

        [JsonPropertyName("sequenceNumber")]
        public int? SequenceNumber { get; set; }

        [JsonPropertyName("userIdAdd")]
        public string UserIdAdd { get; set; }

        [JsonPropertyName("assignedTo")]
        public string AssignedTo { get; set; }

        [JsonPropertyName("referredTo")]
        public string ReferredTo { get; set; }

        [JsonPropertyName("referredFrom")]
        public string ReferredFrom { get; set; }

        [JsonPropertyName("referralReason")]
        public string ReferralReason { get; set; }

        [JsonPropertyName("referralComment")]
        public string ReferralComment { get; set; }

        [JsonPropertyName("paymentDueDate")]
        public string PaymentDueDate { get; set; }
    }

    // Nested shapes for SavePolicyInfo, matching DB2's Insured/Address/CoverageIndicators/
    // UnderwriterQuestions schemas.
    public class Insured
    {
        [JsonPropertyName("namedInsured")]
        public string NamedInsured { get; set; }

        [JsonPropertyName("businessType")]
        public string BusinessType { get; set; }

        [JsonPropertyName("licenseNumber")]
        public string LicenseNumber { get; set; }
    }

    public class Address
    {
        [JsonPropertyName("line1")]
        public string Line1 { get; set; }

        [JsonPropertyName("line2")]
        public string Line2 { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("postalCode")]
        public string PostalCode { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }
    }

    public class CoverageIndicators
    {
        [JsonPropertyName("nonOwnedAuto")]
        public string NonOwnedAuto { get; set; }

        [JsonPropertyName("hiredAuto")]
        public string HiredAuto { get; set; }

        [JsonPropertyName("driveOtherCar")]
        public string DriveOtherCar { get; set; }

        [JsonPropertyName("fleetStatus")]
        public string FleetStatus { get; set; }
    }

    public class UnderwriterQuestions
    {
        [JsonPropertyName("hazardousMaterialsTransport")]
        public string HazardousMaterialsTransport { get; set; }

        [JsonPropertyName("validFeinFid")]
        public string ValidFeinFid { get; set; }

        [JsonPropertyName("snowRemovalForFee")]
        public string SnowRemovalForFee { get; set; }

        [JsonPropertyName("iccPucFilings")]
        public string IccPucFilings { get; set; }
    }

    // Matches DB2's PatchDriversRequest schema (item shape for the PatchDriver "drivers" array).
    public class PatchDriverItem
    {
        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("effectiveDate")]
        public string EffectiveDate { get; set; }

        [JsonPropertyName("licenseNumber")]
        public string LicenseNumber { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("middleName")]
        public string MiddleName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("driverSex")]
        public string DriverSex { get; set; }

        [JsonPropertyName("birthDate")]
        public string BirthDate { get; set; }

        [JsonPropertyName("licenseState")]
        public string LicenseState { get; set; }

        [JsonPropertyName("licenseStatus")]
        public string LicenseStatus { get; set; }
    }

    // Matches DB2's PatchVehicleRequest schema (item shape for the PatchVehicle "vehicles" array).
    public class PatchVehicleItem
    {
        [JsonPropertyName("vehicleId")]
        public string VehicleId { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("modelYear")]
        public string ModelYear { get; set; }

        [JsonPropertyName("makeCode")]
        public string MakeCode { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("vehicleType")]
        public string VehicleType { get; set; }

        [JsonPropertyName("bodyStyle")]
        public string BodyStyle { get; set; }

        [JsonPropertyName("plateNumber")]
        public string PlateNumber { get; set; }

        [JsonPropertyName("plateType")]
        public string PlateType { get; set; }

        [JsonPropertyName("registrationStatus")]
        public string RegistrationStatus { get; set; }

        [JsonPropertyName("garage")]
        public string Garage { get; set; }

        [JsonPropertyName("locationCity")]
        public string LocationCity { get; set; }

        [JsonPropertyName("locationZipCode")]
        public string LocationZipCode { get; set; }

        [JsonPropertyName("territory")]
        public string Territory { get; set; }

        [JsonPropertyName("taxTerritory")]
        public string TaxTerritory { get; set; }

        [JsonPropertyName("originalCost")]
        public string OriginalCost { get; set; }

        [JsonPropertyName("antiTheft")]
        public string AntiTheft { get; set; }

        [JsonPropertyName("splitBiUnderLimit")]
        public string SplitBiUnderLimit { get; set; }

        [JsonPropertyName("collisionDeductAmt")]
        public string CollisionDeductAmt { get; set; }

        [JsonPropertyName("collsnDeductWaiver")]
        public string CollsnDeductWaiver { get; set; }

        [JsonPropertyName("limitedCollDed")]
        public string LimitedCollDed { get; set; }

        [JsonPropertyName("compDeductAmount")]
        public string CompDeductAmount { get; set; }

        [JsonPropertyName("fullGlass")]
        public string FullGlass { get; set; }

        [JsonPropertyName("rentalReimbursement")]
        public string RentalReimbursement { get; set; }

        [JsonPropertyName("towingLimDisab")]
        public string TowingLimDisab { get; set; }

        [JsonPropertyName("loanLeaseCoverage")]
        public string LoanLeaseCoverage { get; set; }

        [JsonPropertyName("ownerFirstName")]
        public string OwnerFirstName { get; set; }

        [JsonPropertyName("ownerLastName")]
        public string OwnerLastName { get; set; }

        [JsonPropertyName("ownerBirthDate")]
        public string OwnerBirthDate { get; set; }

        [JsonPropertyName("ownerLicenseNumber")]
        public string OwnerLicenseNumber { get; set; }

        [JsonPropertyName("ownerLicenseState")]
        public string OwnerLicenseState { get; set; }

        [JsonPropertyName("classCode")]
        public string ClassCode { get; set; }

        [JsonPropertyName("mortgageType")]
        public string MortgageType { get; set; }

        [JsonPropertyName("mortgageeName")]
        public string MortgageeName { get; set; }

        [JsonPropertyName("mortgageeAdr1")]
        public string MortgageeAdr1 { get; set; }

        [JsonPropertyName("mortgageeCity")]
        public string MortgageeCity { get; set; }

        [JsonPropertyName("mortgageeState")]
        public string MortgageeState { get; set; }

        [JsonPropertyName("mortgageeZip")]
        public string MortgageeZip { get; set; }

        [JsonPropertyName("mortgageeAdr2")]
        public string MortgageeAdr2 { get; set; }

        [JsonPropertyName("extendNonOwnedCov")]
        public string ExtendNonOwnedCov { get; set; }

        [JsonPropertyName("hiredAutomobileCoverage")]
        public string HiredAutomobileCoverage { get; set; }

        [JsonPropertyName("driveOtherCarCoverage")]
        public string DriveOtherCarCoverage { get; set; }

        [JsonPropertyName("fleetYN")]
        public string FleetYN { get; set; }
    }

    // Coverage fields shared across all vehicles in a single PatchVehicle request.
    public class PatchVehicleCommon
    {
        [JsonPropertyName("compDeductAmount")]
        public string CompDeductAmount { get; set; }

        [JsonPropertyName("collisionDeductAmt")]
        public string CollisionDeductAmt { get; set; }

        [JsonPropertyName("splitBiLiabLimit")]
        public string SplitBiLiabLimit { get; set; }

        [JsonPropertyName("optBiToOthers")]
        public string OptBiToOthers { get; set; }

        [JsonPropertyName("splitPdLiabLimit")]
        public string SplitPdLiabLimit { get; set; }

        [JsonPropertyName("splitBiUninsLimit")]
        public string SplitBiUninsLimit { get; set; }

        [JsonPropertyName("pipLimit1")]
        public string PipLimit1 { get; set; }

        [JsonPropertyName("medicalLimit")]
        public string MedicalLimit { get; set; }
    }
}
