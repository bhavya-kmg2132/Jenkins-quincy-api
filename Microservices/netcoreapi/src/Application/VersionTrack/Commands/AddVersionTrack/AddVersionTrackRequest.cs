using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.VersionTrack.Commands.AddVersionTrack
{
    public class AddVersionTrackRequest : IRequest<string>
    {
        public string PlatformType { get; set; }
        public string VersionNumber { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ReleaseNotes { get; set; }
        public string ReleasedBy { get; set; }
        public string ReleasedTo { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request , created CreateClientRequest class
    /// That implements the IRequestHandler interface as shown below.
    /// </summary>
    public class AddVersionTrackRequestHandler : IRequestHandler<AddVersionTrackRequest, string>
    {
        private readonly ILogger<AddVersionTrackRequest> _logger;
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUserService;
        private readonly IVersionTrackDataAccess _dataAccess;

        /// <summary>
        /// Instantiates the class CreateClientRequest
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="dataAccess"></param>
        /// <param name="currentUserService"></param>
        public AddVersionTrackRequestHandler(IConfiguration configuration, ILogger<AddVersionTrackRequest> logger, IVersionTrackDataAccess dataAccess, ICurrentUserService currentUserService)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._dataAccess = dataAccess;
            _currentUserService = currentUserService;
        }


        /// <summary>
        /// Handler will recieve request ,process it and will return the response.  
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>int</returns>
        public async Task<string> Handle(AddVersionTrackRequest request, CancellationToken cancellationToken)
        {
            // Logging Information In Process
            _logger.LogInformation("AddVersionTrackRequest.Handle - In Process");

            // Assign requested Client values 
            var versiontrack = new Domain.Entities.VersionTrack();

            versiontrack.PlatformType = request.PlatformType;
            versiontrack.VersionNumber = request.VersionNumber;
            versiontrack.ReleaseDate = request.ReleaseDate;
            versiontrack.CreatedDate = request.CreatedDate;
            versiontrack.ReleasedBy = request.ReleasedBy;
            versiontrack.ReleasedTo = request.ReleasedTo;
            versiontrack.ReleaseNotes = request.ReleaseNotes;

            // Add the VersionTrack to the data access layer for persistence
            var versionTrack = await _dataAccess.Add(versiontrack);

            // Logging Information Completed
            _logger.LogInformation("AddVersionTrackRequestRequest.Handle - Completed");

            // Return generated Client id
            return versionTrack;
        }
    }
}
