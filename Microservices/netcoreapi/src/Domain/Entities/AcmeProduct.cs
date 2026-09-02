using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Domain.Common;
using Domain.Events;

namespace Domain.Entities
{
    public class AcmeProduct : AuditableEntity, IHasDomainEvent
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public string ProductType { get; set; }
        public decimal BaseCost { get; set; }
        public decimal BasePrice { get; set; }
        public string Currency { get; set; }
        public string ProductOwnerId { get; set; }
        public string ProductOwner { get; set; }
        public string ProductCode { get; set; }
        public bool? ProductActive { get; set; }
        public DateTime? SalesStartDate { get; set; }
        public DateTime? SalesEndDate { get; set; }
        public DateTime? SupportStartDate { get; set; }
        public DateTime? SupportEndDate { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string ProductCategoryId { get; set; }
        public string ProductCategory { get; set; }
        public string ManufacturerId { get; set; }
        public string Manufacturer { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? CommissionRate { get; set; }
        public string TaxId { get; set; }
        public string Tax { get; set; }
        public string UsageUnitId { get; set; }
        public string UsageUnit { get; set; }
        public decimal? QuantityInStock { get; set; }
        public string HandlerId { get; set; }
        public string Handler { get; set; }
        public decimal? QuantityOrdered { get; set; }
        public decimal? ReorderLevel { get; set; }
        public decimal? QuantityInDemand { get; set; }
        public new List<CustomField> CustomFields { get; set; }
        public string CustomFieldJson { get; set; }



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
                    DomainEvents.Add(new AcmeProductCompletedEvent(this));
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
