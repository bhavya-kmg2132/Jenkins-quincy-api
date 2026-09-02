using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.Permission.GetActiveInactivePermissionQuery
{
    /// <summary>
    /// class GetActiveInactivePermissionQuery extends the IRequest interface of MediatR
    /// </summary>
    public class GetActiveInactivePermissionQuery : IRequest<PermissionListVm>
    {

    }

    /// <summary>
    /// For Creating handler for the above request, created GetActiveInactivePermissionQueryHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class GetActiveInactivePermissionQueryHandler : IRequestHandler<GetActiveInactivePermissionQuery, PermissionListVm>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IIdentityManager _dataAccess;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates GetActiveInactivePermissionQueryHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public GetActiveInactivePermissionQueryHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IIdentityManager dataAccess)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._dataAccess = dataAccess;
            this._mapper = mapper;
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response, including both active and inactive permissions.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<PermissionListVm> Handle(GetActiveInactivePermissionQuery request, CancellationToken cancellationToken)
        {
            return new PermissionListVm
            {
                //Mapping UsersDto with Users entity
                PermissionList = _mapper.Map<List<PermissionDto>>(await _dataAccess.GetAllPermissionsAsync())
            };
        }
    }
}
