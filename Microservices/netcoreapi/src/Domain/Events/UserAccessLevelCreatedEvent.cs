using Domain.Common;
using NetAuth.Contract.DataContract.Entities;

namespace Domain.Events
{
    public class UserAccessLevelCreatedEvent : DomainEvent
    {
        public UserAccessLevelCreatedEvent(UserAccessLevel accessLevel)
        {
            UserAccessLevelDetails = accessLevel;
        }
        public UserAccessLevel UserAccessLevelDetails { get; }
    }
}
