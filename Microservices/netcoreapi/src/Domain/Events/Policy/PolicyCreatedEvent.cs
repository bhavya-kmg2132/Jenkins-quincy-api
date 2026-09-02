using Domain.Common;
using Domain.Entities;

namespace Domain.Events
{
    public class PolicyCreatedEvent : DomainEvent
    {
        public PolicyCreatedEvent(Policy policyObject)
        {
            PolicyCreatedObject = policyObject;
        }
        public Policy PolicyCreatedObject { get; }
    }
}
