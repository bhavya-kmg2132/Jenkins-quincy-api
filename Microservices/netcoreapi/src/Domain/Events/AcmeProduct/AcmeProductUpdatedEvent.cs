using Domain.Common;

namespace Domain.Events
{
    public class AcmeProductUpdatedEvent : DomainEvent
    {
        public AcmeProductUpdatedEvent(Entities.AcmeProduct newObject, Entities.AcmeProduct oldObject)
        {
            AcmeNewObject = newObject;
            AcmeOldObject = oldObject;
        }

        public Entities.AcmeProduct AcmeNewObject { get; }
        public Entities.AcmeProduct AcmeOldObject { get; }
    }
}
