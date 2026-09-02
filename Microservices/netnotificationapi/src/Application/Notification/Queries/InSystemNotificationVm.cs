using System.Collections.Generic;

namespace Application.Notification.Queries
{
    public class InSystemNotificationVm
    {
        public IList<InSystemNotificationDto> RecentNotifications { get; set; }
        public int UnreadCount { get; set; }
    }
}
