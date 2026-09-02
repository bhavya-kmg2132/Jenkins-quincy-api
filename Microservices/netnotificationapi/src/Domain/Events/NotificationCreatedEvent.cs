using Domain.Common;
using Domain.Entities;

namespace Domain.Events
{
    public class NotificationCreatedEvent : DomainEvent
    {
        public NotificationCreatedEvent(PostgreNotification Notification)
        {
            NotificationCreatedObject = Notification;
        }
        public PostgreNotification NotificationCreatedObject { get; }
    }
}
