namespace Messaging.Contract.Notification;

public record CreateNotificationMessage
{
    public Guid NotificationId { get; init; }
    public string Type { get; init; } = default!;
    public string PayloadJson { get; init; } = default!;
    public DateTime CreatedAtUtc { get; init; }

}
