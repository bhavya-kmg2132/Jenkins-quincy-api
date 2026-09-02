using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.Permission.GetPermissionsQueryAsync
{
    /// <summary>
    /// class GetPermissionQueryHandler extends the IRequest interface of MediatR
    /// </summary>
    public class GetPermissionsQueryAsync : IRequest<PermissionListVm>
    {

    }

    /// <summary>
    /// For Creating handler for the above request , created GetPermissionQueryHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class GetPermissionsQueryAsyncHandler : IRequestHandler<GetPermissionsQueryAsync, PermissionListVm>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IIdentityManager _dataAccess;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates GetUsersQueryHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public GetPermissionsQueryAsyncHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IIdentityManager dataAccess)
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
        /// <returns></returns>
        public async Task<PermissionListVm> Handle(GetPermissionsQueryAsync request, CancellationToken cancellationToken)
        {
            return new PermissionListVm
            {
                //Mapping UsersDto with Users entity; GetPermissionsAsync returns only active permissions
                PermissionList = _mapper.Map<List<PermissionDto>>(await _dataAccess.GetPermissionsAsync())
            };
        }
    }
}

