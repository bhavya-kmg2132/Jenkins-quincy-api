using System;
using Application.Common.Mappings;

namespace Application.Notification.Queries
{
    public class InSystemNotificationDto : IMapFrom<Domain.Entities.InSystemNotification>
    {
        public string Id { get; set; }
        public string Message { get; set; }
        //public bool IsShown { get; set; }
        public string UserId { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public int UnreadCount { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Domain.Entities.InSystemNotification, InSystemNotificationDto>();
        }
    }
}
