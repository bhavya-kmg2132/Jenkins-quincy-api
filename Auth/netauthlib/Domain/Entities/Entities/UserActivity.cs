using System.Text.Json.Serialization;
using NetAuth.Domain.Common;
using NetAuth.Domain.Enums;

namespace NetAuth.Domain.Entities
{
    internal class UserActivity : AuditableEntity, IHasDomainEvent
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public DateTime? LastLoginDateTime { get; set; }
        public DateTime? LastLogoutDateTime { get; set; }
        public DateTime? LastActivityDateTime { get; set; }
        public string LastActivityModule { get; set; }
        public UserActionType? LastActionType { get; set; }
        public string LastActivityDetail { get; set; }

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
                    // DomainEvents.Add(new TodoItemCompletedEvent(this));
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
