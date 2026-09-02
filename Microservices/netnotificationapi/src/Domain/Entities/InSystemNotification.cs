using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Domain.Common;

namespace Domain.Entities
{
    public class InSystemNotification : AuditableEntity, IHasDomainEvent
    {
        public string Id { get; set; }
        public string Message { get; set; }
        //public bool IsShown { get; set; }
        public string UserId { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public int UnreadCount { get; set; }

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
                    // DomainEvents.Add(new MilestoneCompletedEvent(this));
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