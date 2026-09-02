using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Users.Queries.UiPermission.RoleUiPermission;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.Role.RoleUiPermission.GetUiPermissionsByRoleIdQuery
{
    /// <summary>
    /// class GetUiPermissionsByRoleIdQuery extends the IRequest interface of MediatR
    /// </summary>
    public class GetUiPermissionsByRoleIdQuery : IRequest<RoleUiPermissionListVm>
    {
        public string RoleId { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request, created GetUiPermissionsByRoleIdQueryHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class GetUiPermissionsByRoleIdQueryHandler : IRequestHandler<GetUiPermissionsByRoleIdQuery, RoleUiPermissionListVm>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUiPermissionDataAccess _dataAccess;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates GetUiPermissionsByRoleIdQueryHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public GetUiPermissionsByRoleIdQueryHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUiPermissionDataAccess dataAccess)
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
        /// <returns>RoleUiPermissionListVm</returns>
        public async Task<RoleUiPermissionListVm> Handle(GetUiPermissionsByRoleIdQuery request, CancellationToken cancellationToken)
        {
            //Returns RoleUiPermissionListVm
            return new RoleUiPermissionListVm
            {
                //Mapping RoleUiPermissionDto with RoleUiPermission
                RoleUiPermissions = _mapper.Map<List<RoleUiPermissionDto>>(await _dataAccess.GetUiPermissionsForRole(request.RoleId))
            };
        }
    }
}

