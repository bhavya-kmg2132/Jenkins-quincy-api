using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Domain.Common;
using Domain.Events;

namespace Domain.Entities
{
    public class VersionTrack : AuditableEntity, IHasDomainEvent
    {
        public string Id { get; set; }
        public string PlatformType { get; set; }
        public string VersionNumber { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ReleaseNotes { get; set; }
        public string ReleasedBy { get; set; }
        public string ReleasedTo { get; set; }

        [NonSerialized]
        [JsonIgnore]
        [NotMapped]
        private bool _done;

        [JsonIgnore]
        [NotMapped]
        public bool Done
        {
            get => _done;
            set
            {
                if (value == true && _done == false)
                {
                    DomainEvents.Add(new VersionTrackCompletedEvent(this));
                }

                _done = value;
            }
        }


        [NonSerialized]
        [NotMapped]
        [JsonIgnore]
        private List<DomainEvent> _domainEvents;

        [JsonIgnore]
        [NotMapped]
        public List<DomainEvent> DomainEvents
        {
            get
            {
                if (_domainEvents == null)
                {
                    _domainEvents = new List<DomainEvent>();
                }

                return _domainEvents;

            }
            set { _domainEvents = value; }
        }
    }
}
