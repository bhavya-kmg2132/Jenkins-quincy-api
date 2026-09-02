using System.Text.Json.Serialization;
using NetAuth.Domain.Common;

namespace NetAuth.Domain.Entities
{
    internal class AuthReferenceLookup : AuditableEntity, IHasDomainEvent
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

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
                    //  DomainEvents.Add(new TodoItemCompletedEvent(this));
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
