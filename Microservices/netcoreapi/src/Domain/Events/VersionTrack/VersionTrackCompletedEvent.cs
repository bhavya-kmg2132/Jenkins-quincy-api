using Domain.Common;
using Domain.Entities;

namespace Domain.Events
{
    public class VersionTrackCompletedEvent : DomainEvent
    {
        public VersionTrackCompletedEvent(VersionTrack VersionTrack)
        {
            VersionTrackCompletedObject = VersionTrack;
        }
        public VersionTrack VersionTrackCompletedObject { get; }
    }
}
