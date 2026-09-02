using Domain.Common;
using Domain.Entities;

namespace Domain.Events
{
    public class UserCreatedEvent : DomainEvent
    {
        public UserCreatedEvent(User marketing)
        {
            UsersDetails = marketing;
        }
        public User UsersDetails { get; }
    }
}
