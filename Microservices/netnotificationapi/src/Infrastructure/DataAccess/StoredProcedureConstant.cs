namespace Infrastructure.DataAccess
{
    public static class StoredProcedureConstant
    {
        public static readonly string Trident_GetBuildingBaseMLR = "Trident_GetBuildingBaseMLR";
        public static readonly string Trident_GetContentsBaseMLR = "Trident_GetContentsBaseMLR";
        public static readonly string Trident_GetBuildingAddtlDefCharge = "Trident_GetBuildingAddtlDefCharge";
        public static readonly string Trident_GetContentsAddtlDefCharge = "Trident_GetContentsAddtlDefCharge";
        public static readonly string Trident_GetBuildingMajorLossRateCharge = "Trident_GetBuildingMajorLossRateCharge";
        public static readonly string Trident_GetContentsMajorLossRateCharge = "Trident_GetContentsMajorLossRateCharge";
        public static readonly string Trident_GetDollarDeductibleFactor = "Trident_GetDollarDeductibleFactor";//"Trident_GetAOPDeductible";
        public static readonly string Trident_GetCoinsurance = "Trident_GetCoinsurance";
        public static readonly string Trident_GetMarginClause = "Trident_GetMarginClause";
        public static readonly string Trident_GetValuation = "Trident_GetValuation";
        public static readonly string Trident_GetModifiedNormalLossRate = "Trident_GetModifiedNormalLossRate";
        public static readonly string Trident_GetEBLimitFactor = "Trident_GetEBLimitFactor";
        public static readonly string Trident_GetEBDeductibleFactor = "Trident_GetEBDeductibleFactor";
        public static readonly string Trident_GetEBBIDeductibleFactor = "Trident_GetEBBIDeductibleFactor";
        public static readonly string Trident_GetPollutantCleanUpLimitFactor = "Trident_GetPollutantCleanUpLimitFactor";
        public static readonly string Trident_GetRefrigerantContaminationLimitFactor = "Trident_GetRefrigerantContaminationLimitFactor";
        public static readonly string Trident_GetSpoilageTypeLimitFactor = "Trident_GetSpoilageTypeLimitFactor";
        //public static readonly string Trident_GetSpoilageType2LimitFactor = "Trident_GetSpoilageType2LimitFactor";
        public static readonly string Trident_GetEBSpoilagePercentageDeductibleFactor = "Trident_GetEBSpoilagePercentageDeductibleFactor";
        public static readonly string Trident_GetEBSpoilageDollarDeductibleFactor = "Trident_GetEBSpoilageDollarDeductibleFactor";
        public static readonly string Trident_GetEquipmentBreakdownTotalPremium = "Trident_GetEquipmentBreakdownTotalPremium";
        public static readonly string Trident_GetLCM = "Trident_GetLCM";
        public static readonly string Trident_GetLOBTotalPremium = "Trident_GetLOBTotalPremium";
        public static readonly string Trident_GetMiscFactor = "Trident_GetMiscFactor";
        public static readonly string Trident_GetTerrorismFactor = "Trident_GetTerrorismFactor";
        public static readonly string Trident_GetMNFireSafetySurcharge = "Trident_GetMNFireSafetySurcharge";

        // 360 Property
        public static readonly string Trident_GetCoverageDetail = "Trident_GetCoverageDetail";
        public static readonly string Trident_GetBIEEDeductibleFactor = "Trident_GetBIEEDeductibleFactor";
        public static readonly string Trident_GetBIEEDaysFactor = "Trident_GetBIEEDaysFactor";
        public static readonly string Trident_GetCoverageDeductibleFactor = "Trident_GetCoverageDeductibleFactor";
        public static readonly string Trident_GetSpoilageRate = "Trident_GetSpoilageRate";
        public static readonly string Trident_GetTeeToGreenFactor = "Trident_GetTeeToGreenFactor";


        // NY Property StoredProcedure 

        public static readonly string Trident_GetCauseOfLossFactor = "Trident_GetCauseOfLossFactor";
        public static readonly string Trident_GetInflationGuardFactor = "Trident_GetInflationGuardFactor";
        public static readonly string Trident_GetNYBaseRates = "Trident_GetNYBaseRates";
        public static readonly string Trident_GetNYFireInsuranceFee = "Trident_GetNYFireInsuranceFee";

        // OCP

        public static readonly string Trident_GetOCPRate = "Trident_GetOCPRate";
        public static readonly string Trident_GetOCPMiscFactor = "Trident_GetOCPMiscFactor";

        // Auto
        public static readonly string Trident_GetAutoBaseRate = "Trident_GetAutoBaseRate";
        public static readonly string Trident_GetAutoLimitFactor = "Trident_GetAutoLimitFactor";
        public static readonly string Trident_GetAutoDeductibleFactor = "Trident_GetAutoDeductibleFactor";
        public static readonly string Trident_GetAutoPIPRate = "Trident_GetAutoPIPRate";
        public static readonly string Trident_GetAutoMedRate = "Trident_GetAutoMedRate";
        public static readonly string Trident_GetAutoUMRate = "Trident_GetAutoUMRate";
        public static readonly string Trident_GetAutoUIMRate = "Trident_GetAutoUIMRate";
        public static readonly string Trident_GetAutoPopulationFactor = "Trident_GetAutoPopulationFactor";
        public static readonly string Trident_GetAutoTierFactor = "Trident_GetAutoTierFactor";
        public static readonly string Trident_GetAutoSurchargeRate = "Trident_GetAutoSurchargeRate";
        public static readonly string Trident_GetAutoNYFeeVehiclesCount = "Trident_GetAutoNYFeeVehiclesCount";
        public static readonly string Trident_GetAutoHiredAndNonOwnedPremium = "Trident_GetAutoHiredAndNonOwnedPremium";
        public static readonly string Trident_GetAutoOptionalCoveragePremium = "Trident_GetAutoOptionalCoveragePremium";
        public static readonly string Trident_GetAutoMinimumPremium = "Trident_GetAutoMinimumPremium";

        public static readonly string Trident_GetMinMaxValueForFactor = "Trident_GetMinMaxValueForFactor";

        public static readonly string Trident_GetCoverageDetailForAdditionalBenefits = "Trident_GetCoverageDetailForAdditionalBenefits";
        public static readonly string Trident_GetStateList = "Trident_GetStateList";
        public static readonly string Trident_GetMinMaxValueForHazard = "Trident_GetMinMaxValueForHazard";
        public static readonly string Trident_GetAutoApplicableDeductible = "Trident_GetAutoApplicableDeductible";
        public static readonly string Trident_GetAutoApplicableMedPay = "Trident_GetAutoApplicableMedPay";
        public static readonly string Trident_GetAutoApplicableUMCoverage = "Trident_GetAutoApplicableUMCoverage";
        public static readonly string Trident_GetAutoApplicableUIMCoverage = "Trident_GetAutoApplicableUIMCoverage";
        public static readonly string Trident_GetAutoApplicablePIPCoverage = "Trident_GetAutoApplicablePIPCoverage";
        public static readonly string Trident_GetAutoApplicableOptionalCoverage = "Trident_GetAutoApplicableOptionalCoverage";
        public static readonly string Trident_GetAutoApplicableSurcharge = "Trident_GetAutoApplicableSurcharge";
        public static readonly string Trident_GetAutoApplicableClassCode = "Trident_GetAutoApplicableClassCode";
        public static readonly string Trident_GetAutoSurchargeVehiclesCount = "Trident_GetAutoSurchargeVehiclesCount";

        // EmploymentPractices and EPS
        public static readonly string Trident_GetProfLinesOptionalCoverageRate = "Trident_GetProfLinesOptionalCoverageRate";
        public static readonly string Trident_GetProfLinesLiabilityLimitRate = "Trident_GetProfLinesLiabilityLimitRate";
        public static readonly string Trident_GetProfLinesInclExclRate = "Trident_GetProfLinesInclExclRate";
        public static readonly string Trident_GetProfLinesAggregateLimitRate = "Trident_GetProfLinesAggregateLimitRate";
        public static readonly string Trident_GetProfLinesExposureRate = "Trident_GetProfLinesExposureRate";

        public static readonly string Trident_GetProfLinesRetentionRate = "Trident_GetProfLinesRetentionRate";
        public static readonly string Trident_GetProfLinesPopulationRate = "Trident_GetProfLinesPopulationRate";
        public static readonly string Trident_GetProfLinesLocationRate = "Trident_GetProfLinesLocationRate";
        public static readonly string Trident_GetProfLinesCMPolicyYearRate = "Trident_GetProfLinesCMPolicyYearRate";  //  ?
        public static readonly string Trident_GetProfLinesPolicyTypeRate = "Trident_GetProfLinesPolicyTypeRate";

        public static readonly string Trident_GetProfLinesRetroDateRate = "Trident_GetProfLinesRetroDateRate";
        public static readonly string Trident_GetProfLinesLossExperienceRate = "Trident_GetProfLinesLossExperienceRate";
        public static readonly string Trident_GetProfLinesTierRate = "Trident_GetProfLinesTierRate";
        public static readonly string Trident_GetProfLinesIRPMRate = "Trident_GetProfLinesIRPMRate";

        public static readonly string Trident_GetProfLinesOtherModRate = "Trident_GetProfLinesOtherModRate";
        public static readonly string Trident_GetProfLinesOptionalCoveragePremium = "Trident_GetProfLinesOptionalCoveragePremium"; // eeoc///non-mon
        public static readonly string Trident_GetProfLinesMinimumPremium = "Trident_GetProfLinesMinimumPremium";

        public static readonly string Trident_GetProfLinesApplicableDeductible = "Trident_GetProfLinesApplicableDeductible";
        public static readonly string Trident_GetAutoValuationFactor = "Trident_GetAutoValuationFactor";

        //PO
        public static readonly string Trident_GetRatingBasisParameter = "Trident_GetRatingBasisParameter";
        //public static readonly string Trident_GetProfLinesExposureRate = "Trident_GetProfLinesExposureRate";
        public static readonly string Trident_GetProfLinesPOInclExclRate = "Trident_GetProfLinesPOInclExclRate";
        public static readonly string Trident_GetProfLinesApplicableOptionalCoverage = "Trident_GetProfLinesApplicableOptionalCoverage";

    }
}
