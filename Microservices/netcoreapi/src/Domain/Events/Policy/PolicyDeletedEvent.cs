using Domain.Common;
using Domain.Entities;

namespace Domain.Events
{
    public class PolicyDeletedEvent : DomainEvent
    {
        public PolicyDeletedEvent(Policy policyObject)
        {
            PolicyDeletedObject = policyObject;
        }

        public Policy PolicyDeletedObject { get; }
    }
}
