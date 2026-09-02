using System;
using System.Collections.Generic;
using Application.Common.Mappings;
using Domain.Common;

namespace Application.Policy.Queries
{
    public class PolicyDto : IMapFrom<Domain.Entities.Policy>
    {
        public string Id { get; set; }
        public string PolicyNumber { get; set; }
        public string PolicyName { get; set; }
        public string LineOfBusinessCode { get; set; }
        public string PolicyType { get; set; }
        public string StatusCode { get; set; }
        public string TransactionType { get; set; }
        public string QuoteId { get; set; }
        public string RenewalStatus { get; set; }

        public string InsuredId { get; set; }
        public string InsuredName { get; set; }
        public string InsuredAddress { get; set; }

        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime? OriginalEffectiveDate { get; set; }
        public DateTime? AccountingDate { get; set; }
        public DateTime? CancellationDate { get; set; }
        public string CancelReasonDescription { get; set; }

        [FieldPermission(view: FieldPermission.Core_Policy_TotalPremium_View)]
        public decimal? TotalPremium { get; set; }
        public decimal? SumInsured { get; set; }
        public decimal? Deductible { get; set; }
        public string Currency { get; set; }

        public string ProducerCode { get; set; }
        public string ProducerName { get; set; }
        public string UnderwriterId { get; set; }
        public string UnderwriterName { get; set; }
        public string AgentCode { get; set; }

        public string VesselName { get; set; }
        public string VesselType { get; set; }
        public string CargoType { get; set; }
        public string RouteFrom { get; set; }
        public string RouteTo { get; set; }
        public string AircraftRegistration { get; set; }
        public string FlightNumber { get; set; }
        public string RiskDescription { get; set; }
        public string SurveyorName { get; set; }
        public string Remarks { get; set; }

        public List<CustomField> CustomFields { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDateTime { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Domain.Entities.Policy, PolicyDto>();
        }
    }
}
