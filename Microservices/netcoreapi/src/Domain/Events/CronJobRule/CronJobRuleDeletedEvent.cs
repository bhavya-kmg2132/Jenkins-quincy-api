using Domain.Common;
using Domain.Entities;

namespace Domain.Events
{
    public class CronJobRuleDeletedEvent : DomainEvent
    {
        public CronJobRuleDeletedEvent(CronJobRule acmeObject)
        {
            CronJobRuleDeletedObject = acmeObject;
        }

        public CronJobRule CronJobRuleDeletedObject { get; }
    }
}
