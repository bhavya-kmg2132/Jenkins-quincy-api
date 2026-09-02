using Domain.Common;
using Domain.Entities;

namespace Domain.Events
{
    public class AcmeProductCreatedEvent : DomainEvent
    {
        public AcmeProductCreatedEvent(AcmeProduct acme)
        {
            AcmeCreatedObject = acme;
        }
        public AcmeProduct AcmeCreatedObject { get; }
    }
}
