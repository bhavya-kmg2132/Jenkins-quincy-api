using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Role.Queries.GetRole;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.Role.GetPermissionsByRoleIdQuery
{
    /// <summary>
    /// class GetRolePermissionsByIdQuery extends the IRequest interface of MediatR
    /// </summary>
    public class GetPermissionsByRoleId : IRequest<RoleVm>
    {
        public string RoleId { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request, created GetPermissionsByRoleIdQueryHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class GetPermissionsByRoleIdQueryHandler : IRequestHandler<GetPermissionsByRoleId, RoleVm>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserDataAccess _dataAccess;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates of GetPermissionsByRoleIdQueryHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public GetPermissionsByRoleIdQueryHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUserDataAccess dataAccess)
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
        /// <returns>RoleListVm</returns>
        public async Task<RoleVm> Handle(GetPermissionsByRoleId request, CancellationToken cancellationToken)
        {
            //Returns RoleListVm
            return new RoleVm
            {
                //Mapping RoleDto with RoleEntity
                Role = _mapper.Map<RoleDto>(await _dataAccess.GetPermissionsForRoleAsync(request.RoleId))
            };
        }
    }
}

