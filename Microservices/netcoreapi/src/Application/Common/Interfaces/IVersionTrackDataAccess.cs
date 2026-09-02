using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IVersionTrackDataAccess
    {
        Task<List<Domain.Entities.VersionTrack>> GetVersionTrack();
        Task<string> Add(Domain.Entities.VersionTrack versionTrack);
    }
}
