using Domain.Common;
using Domain.Entities;

namespace Domain.Events.Master
{
    public class GenericMasterCompletedEvent : DomainEvent
    {
        public GenericMasterCompletedEvent(GenericMasterList GenericMasterObject)
        {
            GenericMasterCompletedObject = GenericMasterObject;
        }

        public GenericMasterList GenericMasterCompletedObject { get; }
    }
}
