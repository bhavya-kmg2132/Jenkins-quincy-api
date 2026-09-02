using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Domain.Common;

namespace Domain.Entities
{
    public class ZeptoMail : AuditableEntity, IHasDomainEvent, NotificationPayload
    {
        public string Id { get; set; }
        public string ApiKey { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public List<ZeptoMailRecipient> To { get; set; } = new();
        public List<ZeptoMailRecipient>? Cc { get; set; }
        public List<ZeptoMailRecipient>? Bcc { get; set; }
        public string Subject { get; set; }
        public string? HtmlBody { get; set; }
        public string? TextBody { get; set; }
        public List<ZeptoMailAttachment>? Attachments { get; set; }
        public string NotificationErrorMessage { get; set; }
        public NotificationStatus NotificationDelivery { get; set; }
        public DateTime NotificationResponseDateTime { get; set; }
        public string EventStoreId { get; set; }

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

    public class ZeptoMailAddress
    {
        public string address { get; set; }
        public string? name { get; set; }
    }

    public class ZeptoMailRecipient
    {
        public ZeptoMailAddress email_address { get; set; }
    }

    public class ZeptoMailAttachment
    {
        public string FileName { get; set; }           // e.g., "invoice.pdf"
        public string ContentType { get; set; }      // e.g., "application/pdf"
        public byte[] Content { get; set; }        // Base64-encoded file
    }

}
