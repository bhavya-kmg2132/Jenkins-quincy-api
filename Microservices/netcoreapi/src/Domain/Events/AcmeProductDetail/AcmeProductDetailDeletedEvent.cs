using Domain.Common;
using Domain.Entities;

namespace Domain.Events
{
    public class AcmeProductDetailDeletedEvent : DomainEvent
    {
        public AcmeProductDetailDeletedEvent(AcmeProduct newObject)
        {
            NewObject = newObject;
        }

        public AcmeProduct NewObject { get; }
    }
}
