using Domain.Common;
using Domain.Entities;

namespace Domain.Events
{
    public class CronJobRuleCreatedEvent : DomainEvent
    {
        public CronJobRuleCreatedEvent(CronJobRule acme)
        {
            CronJobRuleCreatedObject = acme;
        }
        public CronJobRule CronJobRuleCreatedObject { get; }
    }
}
