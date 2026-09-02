using Domain.Common;
using NetAuth.Contract.DataContract.Entities;

namespace Domain.Events
{
    public class UserAccessLevelChangedEvent : DomainEvent
    {
        public UserAccessLevelChangedEvent(UserAccessLevel accessLevel)
        {
            UserAccessLevelDetails = accessLevel;
        }
        public UserAccessLevel UserAccessLevelDetails { get; }
    }
}
