using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Common.Interfaces
{
    public interface IEmailNotificationService
    {
        Task<Domain.Entities.PostgreNotification> SendEmailNotification(Domain.Entities.PostgreNotification notification);
        Task DispatchEvents(Domain.Entities.PostgreNotification entity);
        //Task<List<InSystemNotification>> GetRecentNotifications();
        //void Broadcast(string message);
        Task<(int, List<InSystemNotification>)> GetRecentNotifications(string userId);
        Channel<string> Register();
        Channel<string> RegisterMultipleChannelsPerUser(string userId);
        Task SaveAndBroadcastAsync(InSystemNotification notification);
        void UnregisterChannel(string userId, Channel<string> channel);
        Task<string> MarkAsRead(string userId, string notificationId);
        Task<PostgreNotification> SendEmailNotificationUsingMicrosoftGraph(PostgreNotification notification);
        Task<List<PostgreNotification>> SendBatchEmailNotificationUsingMicrosoftGraph(List<PostgreNotification> notifications);

    }
}
