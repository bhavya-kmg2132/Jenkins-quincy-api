using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.VersionTrack.Queries.GetVersionTrack
{
    public class GetVersionTrackQuery : IRequest<VersionTrackListVm>
    {

    }

    /// <summary>
    /// For Creating handler for the above request , created GetVersionTrackByIdQueryHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class GetVersionTrackQueryHandler : IRequestHandler<GetVersionTrackQuery, VersionTrackListVm>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IVersionTrackDataAccess _dataAccess;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates GetVersionTrackByIdQueryHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        /// <param name="dataAccess"></param>
        public GetVersionTrackQueryHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IVersionTrackDataAccess dataAccess)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._dataAccess = dataAccess;
            this._mapper = mapper;
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>VersionTrackListVm</returns>
        public async Task<VersionTrackListVm> Handle(GetVersionTrackQuery request, CancellationToken cancellationToken)
        {
            var result = new VersionTrackListVm();

            result = new VersionTrackListVm
            {
                //Mapping VersionTrackDto with VersionTrack entity
                VersionTrackList = _mapper.Map<List<VersionTrackDto>>(await _dataAccess.GetVersionTrack())
            };
            return result;
        }
    }
}
