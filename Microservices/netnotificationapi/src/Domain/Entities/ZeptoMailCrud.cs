using System;
using System.Collections.Generic;
using Domain.Common;
using Newtonsoft.Json;

namespace Domain.Entities
{
    public class ZeptoMailCrud : AuditableEntity, IHasDomainEvent
    {
        public string Id { get; set; }
        public string ApiKey { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string To { get; set; }
        public string? Cc { get; set; }
        public string? Bcc { get; set; }
        public string Subject { get; set; }
        public string? HtmlBody { get; set; }
        public string? TextBody { get; set; }
        public string Attachments { get; set; }
        public string NotificationErrorMessage { get; set; }
        public string NotificationDelivery { get; set; }
        public DateTime NotificationResponseDateTime { get; set; }

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
