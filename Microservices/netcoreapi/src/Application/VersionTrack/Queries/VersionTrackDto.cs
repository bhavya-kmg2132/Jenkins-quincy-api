using System;
using Application.Common.Mappings;

namespace Application.VersionTrack.Queries
{
    public class VersionTrackDto : IMapFrom<Domain.Entities.VersionTrack>
    {
        public string PlatformType { get; set; }
        public string VersionNumber { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ReleaseNotes { get; set; }
        public string ReleasedBy { get; set; }
        public string ReleasedTo { get; set; }


        // Mappint dto with domain class
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Domain.Entities.VersionTrack, VersionTrackDto>();
        }
    }
}
