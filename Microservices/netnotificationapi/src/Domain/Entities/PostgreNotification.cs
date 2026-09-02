using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Domain.Common;

namespace Domain.Entities
{
    public class PostgreNotification : AuditableEntity, IHasDomainEvent
    {
        public string Id { get; set; }
        public string ApiKey { get; set; }
        public string EmailFrom { get; set; }
        public string EmailTo { get; set; }
        public string EmailCc { get; set; }
        public string EmailBcc { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public List<EmailAttachment> EmailAttachments { get; set; }
        public string NotificationErrorMessage { get; set; }
        public NotificationStatus NotificationDelivery { get; set; }
        public DateTime? ScheduledDateTime { get; set; }
        public DateTime NotificationResponseDateTime { get; set; }
        public NotificationEntityJson EntityJson { get; set; } = new();
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

    public class NotificationEntityJson
    {
        public string NotificationType { get; set; }
        public List<string> EmailToName { get; set; }
        public List<string> EmailCcName { get; set; }
        public List<string> EmailBccName { get; set; }
        public string EmailMessageId { get; set; }
        public string SmsReceiver { get; set; }
        public string SmsReceiverName { get; set; }
        public string SmsMessage { get; set; }
        public string SmsSender { get; set; }
        public DateTime? NotificationRequestDateTime { get; set; }
        public bool IsKafkaNotificationSent { get; set; }
        public bool IsNotificationRead { get; set; }
    }

    public class NotificationStatus
    {
        public bool isDelivered { get; set; }
        public string DeliveryReport { get; set; }
    }
    public class EmailAttachment
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
    }
}