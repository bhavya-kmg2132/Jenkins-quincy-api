using Domain.Common;
using Domain.Entities;

namespace Domain.Events
{
    public class AcmeProductCompletedEvent : DomainEvent
    {
        public AcmeProductCompletedEvent(AcmeProduct acme)
        {
            AcmeCompletedObject = acme;
        }
        public AcmeProduct AcmeCompletedObject { get; }
    }
}
