using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Domain.Common;
using Domain.Events.Master;

namespace Domain.Entities
{
    public class GenericMasterList : AuditableEntity, IHasDomainEvent
    {
        public string Id { get; set; }
        public int? Sequence { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string ShortName { get; set; }
        public string Type { get; set; }
        public string Group { get; set; }
        public string SubGroup { get; set; }
        public string ParentId { get; set; }
        public string Description { get; set; }
        public string SearchTerm { get; set; }

        [NonSerialized]
        [JsonIgnore]
        private bool _done;

        [JsonIgnore]
        public bool Done
        {
            get => _done;
            set
            {
                if (value == true && _done == false)
                {
                    DomainEvents.Add(new GenericMasterCompletedEvent(this));
                }

                _done = value;
            }
        }

        [NonSerialized]
        [JsonIgnore]
        private List<DomainEvent> _domainEvents;

        [JsonIgnore]
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
