using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetAuth.Contract.DataContract.VM;

namespace Application.Users.Commands.UiPermission.RoleUiPermissions.AddUiPermissionsForRole
{
    /// <summary>
    /// class AddUiPermissionsForRoleRequest extends the IRequest interface of MediatR
    /// </summary>
    public class AddUiPermissionsForRoleRequest : IRequest<Unit>
    {
        public RoleUiPermissionsVm roleUiPermissionsVm { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request, created AddUiPermissionsForRoleRequestHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class AddUiPermissionsForRoleRequestHandler : IRequestHandler<AddUiPermissionsForRoleRequest, Unit>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUiPermissionDataAccess _dataAccess;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates the class AddUiPermissionsForRoleRequestHandler
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public AddUiPermissionsForRoleRequestHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUiPermissionDataAccess dataAccess, ICurrentUserService currentUserService)
        {
            _configuration = configuration;
            _logger = logger;
            _dataAccess = dataAccess;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Handler will recieve request, process it and will return the response.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Unit> Handle(AddUiPermissionsForRoleRequest request, CancellationToken cancellationToken)
        {
            //1. Logging Information : In Process
            _logger.LogInformation("AddUiPermissionsForRoleRequest.Handle - In process");

            //2. Add one UiPermission per role-permission pair
            foreach (var roleUiPermission in request.roleUiPermissionsVm.RoleAndUiPermissions)
            {
                var addUiPermissionsForRole = new NetAuth.Contract.DataContract.Requests.AddUiPermissionsForRole
                {
                    RoleUiPermission = new NetAuth.Contract.DataContract.Entities.RoleUiPermission
                    {
                        RoleId = roleUiPermission.RoleId,
                        UiPermissionId = roleUiPermission.UiPermissionId
                    },
                    CreatedBy = _currentUserService.UserId
                };

                await _dataAccess.AddUiPermissionsForRole(addUiPermissionsForRole);
            }

            //3. Logging Information : Completed
            _logger.LogInformation("AddUiPermissionsForRoleRequest.Handle - Completed");

            //4. Return Unit
            return Unit.Value;
        }
    }
}


