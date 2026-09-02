using Domain.Common;

namespace Domain.Events
{
    public class CronJobRuleUpdatedEvent : DomainEvent
    {
        public CronJobRuleUpdatedEvent(Entities.CronJobRule newObject, Entities.CronJobRule oldObject)
        {
            CronJobRuleNewObject = newObject;
            CronJobRuleOldObject = oldObject;
        }

        public Entities.CronJobRule CronJobRuleNewObject { get; }
        public Entities.CronJobRule CronJobRuleOldObject { get; }
    }
}
