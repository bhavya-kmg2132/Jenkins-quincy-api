using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Api.Entities
{
    public class Notification
    {
        public string Id { get; set; }
        public string ApiKey { get; set; }
        public string notification_type { get; set; }
        public string email_from { get; set; }
        public string email_to { get; set; }
        public List<string> email_toName { get; set; }
        public string email_cc { get; set; }
        public List<string> email_ccName { get; set; }
        public string email_bcc { get; set; }
        public List<string> email_bccName { get; set; }
        public string email_subject { get; set; }
        public string email_body { get; set; }
        public List<IFormFile> email_attachments { get; set; }
        public string email_messageId { get; set; }
        public string sms_receiver { get; set; }
        public string sms_receiverName { get; set; }
        public string sms_message { get; set; }
        public string sms_sender { get; set; }
        public DateTime NotificationRequestDateTime { get; set; }
        public DateTime NotificationResponseDateTime { get; set; }
        public NotificationStatus notification_delivery { get; set; }
    }

    public class NotificationStatus
    {
        public bool isDelivered { get; set; }
        public string DeliveryReport { get; set; }
    }
}
