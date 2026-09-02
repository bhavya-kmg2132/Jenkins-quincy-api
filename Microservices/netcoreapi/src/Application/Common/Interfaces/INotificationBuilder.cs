using Domain.Entities;

public interface INotificationBuilder
{
    NotificationProvider Provider { get; }

    NotificationPayload BuildPayloadForSelectedNotificationProvider(NotificationData data);
}