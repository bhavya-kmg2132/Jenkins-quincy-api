using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface ICronJobScheduler
    {
        //Task SyncCronJobRuleAndCronJobs(Domain.Entities.CronJobRule oldCronJobRule, Domain.Entities.CronJobRule newCronJobRule);
        Task ScheduleCronJobAsync(Domain.Entities.CronJobRule rule);
        Task RemoveOldCronJobByFrequencyAsync(string baseJobId, Domain.Entities.CronJobRule rule);

        Task<string> InsertCronJobRuleAsync(Domain.Entities.CronJobRule notification);
        Task<int> UpdateCronJobRuleAsync(Domain.Entities.CronJobRule newCronJobRule, Domain.Entities.CronJobRule oldNotificationule);
        Task<int> DeleteCronJobRuleAsync(Domain.Entities.CronJobRule notification);
        Task<List<Domain.Entities.CronJobRule>> GetCronJobRulesAsync();
        Task<Domain.Entities.CronJobRule> GetCronJobRuleByIdAsync(string Id);

        Task<int> UpsertNotificationUserSubscriptionAsync(List<NotificationUserSubscription> notificationUserSubscription, string userEmail);
        Task<List<NotificationUserSubscription>> GetNotificationUserSubscriptionsAsync(List<string> userIds);


    }
}
