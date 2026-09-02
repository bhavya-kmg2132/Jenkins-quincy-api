using System;
using System.Collections.Generic;
using Application.Common.Mappings;
using Domain.Entities;

namespace Application.ZeptoMail.Queries
{
    public class ZeptoMailDto : IMapFrom<Domain.Entities.ZeptoMail>
    {
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
        public string CreatedBy { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDateTime { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Domain.Entities.ZeptoMail, ZeptoMailDto>();
        }
    }
}
