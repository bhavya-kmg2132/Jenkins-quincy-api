using Domain.Common;
using Domain.Entities;

namespace Domain.Events
{
    public class AcmeProductDetailUpdatedEvent : DomainEvent
    {
        public AcmeProductDetailUpdatedEvent(AcmeProduct newObject, AcmeProduct oldObject)
        {
            NewObject = newObject;
            OldObject = oldObject;
        }

        public AcmeProduct NewObject { get; }
        public AcmeProduct OldObject { get; }
    }
}
