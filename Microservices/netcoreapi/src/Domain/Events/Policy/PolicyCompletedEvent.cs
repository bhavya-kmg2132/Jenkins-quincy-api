using Domain.Common;
using Domain.Entities;

namespace Domain.Events
{
    public class PolicyCompletedEvent : DomainEvent
    {
        public PolicyCompletedEvent(Policy policy)
        {
            PolicyCompletedObject = policy;
        }
        public Policy PolicyCompletedObject { get; }
    }
}
