using System;
using System.Collections.Generic;

namespace Application.ExternalPolicy.PolicyCancellation
{
    /// <summary>
    /// DB2's fixed code lists for the Policy Cancellation screens (Cancel Reason, New Carrier).
    /// Sourced from the business's Cancellation reference workbook - update here if DB2's
    /// valid-values list changes, rather than editing each validator individually.
    /// </summary>
    public static class PolicyCancellationCodes
    {
        public static readonly HashSet<string> CancellationReasonCodes = new HashSet<string>
        {
            "01", // Non Payment of premium
            "04", // Underwriting reason
            "05", // Internal cancel transaction
            "06", // Insured request
            "07", // Vehicle sold
            "08", // Rewritten with other company - 2A
            "09", // Plate returned
            "10", // Rewritten with Quincy Mutual
            "12"  // Cancel rewrite/add from cancel
        };

        public static readonly HashSet<string> CancellationCarrierCodes = new HashSet<string>(StringComparer.Ordinal)
        {
            "ALLSTATE",
            "AMICA",
            "ARBELLA",
            "COMMERCE/MAPFRE",
            "GEICO",
            "HANOVER",
            "LIBERTY",
            "METLIFE",
            "NORFOLK AND DEDHAM",
            "PLYMOUTH ROCK",
            "PREFERRED MUTUAL",
            "PREMIER",
            "PROGRESSIVE",
            "SAFETY",
            "TRAVELERS/STANDARD FIRE",
            "USAA",
            "VERMONT MUTUAL",
            "OTHER",
            "UNKNOWN"
        };

        // CancellationCarrier / PolicyRetainedByAgency only apply when the policy is actually
        // moving to another carrier: "06" (Insured request) or "08" (Rewritten with other company - 2A).
        public static readonly HashSet<string> ReasonCodesRequiringCarrierInfo = new HashSet<string>
        {
            "06",
            "08"
        };
    }
}
