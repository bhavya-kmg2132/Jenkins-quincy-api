using Domain.Common;
using Domain.Entities;

namespace Domain.Events
{
    public class VersionTrackCreatedEvent : DomainEvent
    {
        public VersionTrackCreatedEvent(VersionTrack VersionTrack)
        {
            VersionTrackCreatedObject = VersionTrack;
        }
        public VersionTrack VersionTrackCreatedObject { get; }
    }
}
