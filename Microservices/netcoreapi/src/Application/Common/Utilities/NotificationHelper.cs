using System.Collections.Generic;
using System.Linq;
using Domain.Entities;

public class NotificationHelper
{
    private readonly Dictionary<NotificationProvider, INotificationBuilder> _notificationBuildersMap;

    public NotificationHelper(IEnumerable<INotificationBuilder> builders)
    {
        _notificationBuildersMap = builders.ToDictionary(x => x.Provider);
    }

    public NotificationPayload BuildNotification(NotificationProvider provider, NotificationData data)
    {
        return _notificationBuildersMap[provider].BuildPayloadForSelectedNotificationProvider(data);
    }
}