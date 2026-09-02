using Domain.Common;
using Domain.Entities;

namespace Domain.Events
{
    public class UserCompletedEvent : DomainEvent
    {
        public UserCompletedEvent(User user)
        {
            UsersDetails = user;
        }
        public User UsersDetails { get; }
    }
}
