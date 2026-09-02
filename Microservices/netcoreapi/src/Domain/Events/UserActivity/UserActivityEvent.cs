using Domain.Common;
using NetAuth.Contract.DataContract.Requests;

namespace Domain.Events
{
    public class UserActivityEvent : DomainEvent
    {
        public UserActivityEvent(AddUserActivity userActivity)
        {
            UserActivityCompletedObject = userActivity;
        }

        public AddUserActivity UserActivityCompletedObject { get; }
    }
}
