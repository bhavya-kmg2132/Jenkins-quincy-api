using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Domain.Common;

namespace Domain.Entities
{
    public class PolicyDemo : AuditableEntity, IHasDomainEvent
    {
        public string Id { get; set; }
        public string NamedInsured { get; set; }
        public string PolicyType { get; set; }
        public DateTime PolicyExpirationDate { get; set; }
        public string PolicyNumber { get; set; }
        public string TransactionType { get; set; }
        public DateTime PolicyEffectiveDate { get; set; }
        public string CIANumber { get; set; }
        public string TransactionStatus { get; set; }
        public string ProducerCode { get; set; }

        public new List<CustomField> CustomFields { get; set; }

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
                {
                    // PolicyCompletedEvent now targets Policy, not PolicyDemo
                }

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
                {
                    _domainEvents = new List<DomainEvent>();
                }

                return _domainEvents;

            }
            set { _domainEvents = value; }
        }
    }
}
