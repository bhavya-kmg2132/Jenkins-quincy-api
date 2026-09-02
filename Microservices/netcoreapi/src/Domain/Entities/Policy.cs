using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Domain.Common;
using Domain.Events;

namespace Domain.Entities
{
    public class Policy : AuditableEntity, IHasDomainEvent
    {
        public string Id { get; set; }

        // ── Core ──────────────────────────────────────────────────────────────────
        public string PolicyNumber { get; set; }
        public string PolicyName { get; set; }
        public string LineOfBusinessCode { get; set; }   // always "MCA"
        public string PolicyType { get; set; }           // Marine | Cargo | Aviation
        public string StatusCode { get; set; }           // Active | Pending | Cancelled | Expired | Lapsed
        public string TransactionType { get; set; }      // NewBusiness | Renewal | Endorsement | Cancellation
        public string QuoteId { get; set; }
        [FieldPermission(view: FieldPermission.Core_Policy_RenewalStatus_View, edit: FieldPermission.Core_Policy_RenewalStatus_Edit, throwError: false)]
        public string RenewalStatus { get; set; }

        // ── Insured ───────────────────────────────────────────────────────────────
        public string InsuredId { get; set; }
        public string InsuredName { get; set; }
        public string InsuredAddress { get; set; }

        // ── Dates ─────────────────────────────────────────────────────────────────
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime? OriginalEffectiveDate { get; set; }
        public DateTime? AccountingDate { get; set; }
        public DateTime? CancellationDate { get; set; }
        public string CancelReasonDescription { get; set; }

        // ── Financial ─────────────────────────────────────────────────────────────
        [FieldPermission(view: FieldPermission.Core_Policy_TotalPremium_View, edit: FieldPermission.Core_Policy_TotalPremium_Edit, throwError: false)]
        public decimal? TotalPremium { get; set; }
        public decimal? SumInsured { get; set; }
        public decimal? Deductible { get; set; }
        public string Currency { get; set; }

        // ── Parties ───────────────────────────────────────────────────────────────
        public string ProducerCode { get; set; }
        public string ProducerName { get; set; }
        public string UnderwriterId { get; set; }
        public string UnderwriterName { get; set; }
        public string AgentCode { get; set; }

        // ── MCA-specific ──────────────────────────────────────────────────────────
        public string VesselName { get; set; }           // Marine / Cargo
        public string VesselType { get; set; }           // Marine
        public string CargoType { get; set; }            // Cargo
        public string RouteFrom { get; set; }            // Origin port or airport
        public string RouteTo { get; set; }              // Destination port or airport
        public string AircraftRegistration { get; set; } // Aviation
        public string FlightNumber { get; set; }         // Aviation
        public string RiskDescription { get; set; }
        public string SurveyorName { get; set; }
        public string Remarks { get; set; }

        // ── Custom fields ─────────────────────────────────────────────────────────
        public new List<CustomField> CustomFields { get; set; }
        public string CustomFieldJson { get; set; }

        // ── Domain event machinery ────────────────────────────────────────────────

        [NonSerialized]
        [JsonIgnore]
        [NotMapped]
        private bool _done;

        [JsonIgnore]
        [NotMapped]
        public bool Done
        {
            get => _done;
            set
            {
                if (value == true && _done == false)
                    DomainEvents.Add(new PolicyCompletedEvent(this));
                _done = value;
            }
        }

        [NonSerialized]
        [NotMapped]
        [JsonIgnore]
        private List<DomainEvent> _domainEvents;

        [JsonIgnore]
        [NotMapped]
        public List<DomainEvent> DomainEvents
        {
            get
            {
                if (_domainEvents == null)
                    _domainEvents = new List<DomainEvent>();
                return _domainEvents;
            }
            set { _domainEvents = value; }
        }
    }
}
