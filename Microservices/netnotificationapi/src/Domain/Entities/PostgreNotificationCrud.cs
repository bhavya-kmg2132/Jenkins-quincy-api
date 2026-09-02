using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Domain.Common;

namespace Domain.Entities
{
    public class PostgreNotificationCrud : AuditableEntity, IHasDomainEvent
    {
        public string Id { get; set; }
        public string ApiKey { get; set; }
        public string EmailFrom { get; set; }
        public string EmailTo { get; set; }
        public string EmailCc { get; set; }
        public string EmailBcc { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string EmailAttachments { get; set; }
        public string NotificationErrorMessage { get; set; }
        public string NotificationDelivery { get; set; }
        public DateTime? ScheduledDateTime { get; set; }
        public DateTime NotificationResponseDateTime { get; set; }
        public string EntityJson { get; set; }

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
