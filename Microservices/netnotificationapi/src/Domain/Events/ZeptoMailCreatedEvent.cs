using Domain.Common;
using Domain.Entities;

namespace Domain.Events
{
    public class ZeptoMailCreatedEvent : DomainEvent
    {
        public ZeptoMailCreatedEvent(ZeptoMail Notification)
        {
            NotificationCreatedObject = Notification;
        }
        public ZeptoMail NotificationCreatedObject { get; }
    }
}
