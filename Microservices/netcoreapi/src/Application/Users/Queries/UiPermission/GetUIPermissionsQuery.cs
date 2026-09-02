using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.UiPermission.GetUiPermissionsQuery
{
    /// <summary>
    /// class GetUiPermissionsQueryHandler extends the IRequest interface of MediatR
    /// </summary>
    public class GetUiPermissionsQuery : IRequest<UiPermissionListVm>
    {

    }

    /// <summary>
    /// For Creating handler for the above request, created GetUiPermissionsQueryHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class GetUiPermissionsQueryHandler : IRequestHandler<GetUiPermissionsQuery, UiPermissionListVm>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUiPermissionDataAccess _dataAccess;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates of GetUiPermissionsQueryHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public GetUiPermissionsQueryHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUiPermissionDataAccess dataAccess)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._dataAccess = dataAccess;
            this._mapper = mapper;
        }

        /// <summary>
        /// Handler will recieve request, process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<UiPermissionListVm> Handle(GetUiPermissionsQuery query, CancellationToken cancellationToken)
        {
            //Get All UiPermissions
            return new UiPermissionListVm
            {
                //Mapping UiPermissionDto with UiPermission entity
                UiPermissionList = _mapper.Map<List<UiPermissionDto>>(await _dataAccess.GetUiPermissions())
            };

        }
    }
}

