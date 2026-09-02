using Domain.Common;
using Domain.Entities;

namespace Domain.Events
{
    public class CronJobRuleCompletedEvent : DomainEvent
    {
        public CronJobRuleCompletedEvent(CronJobRule acmeOrder)
        {
            CronJobRuleCompletedObject = acmeOrder;
        }
        public CronJobRule CronJobRuleCompletedObject { get; }
    }
}
