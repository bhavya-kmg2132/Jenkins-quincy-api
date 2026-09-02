using Domain.Common;
using Domain.Entities;

namespace Domain.Events
{
    public class AcmeProductDeletedEvent : DomainEvent
    {
        public AcmeProductDeletedEvent(AcmeProduct acmeObject)
        {
            AcmeDeletedObject = acmeObject;
        }

        public AcmeProduct AcmeDeletedObject { get; }
    }
}
