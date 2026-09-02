using Domain.Common;
using Domain.Entities;

namespace Domain.Events
{
    public class PolicyUpdatedEvent : DomainEvent
    {
        public PolicyUpdatedEvent(Policy newobject, Policy oldObject)
        {
            PolicyNewObject = newobject;
            PolicyOldObject = oldObject;
        }

        public Policy PolicyNewObject { get; }
        public Policy PolicyOldObject { get; }
    }
}
