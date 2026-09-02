using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.RolePermissions
{
    /// <summary>
    /// class AddPermissionsForRoleRequest extends the IRequest interface of MediatR
    /// </summary>
    public class AddPermissionsForRoleRequest : IRequest<Unit>
    {
        public RoleAndPermissionMapping RoleAndPermissionMapping { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request, created CreatePermissionsForRoleRequestHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class CreatePermissionsForRoleRequestHandler : IRequestHandler<AddPermissionsForRoleRequest, Unit>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserDataAccess _userDataAccess;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly IIdentityManager _identityManager;



        /// <summary>
        /// Instantiates the class CreateProspectRequestHandler
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public CreatePermissionsForRoleRequestHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUserDataAccess userDataAccess, ICurrentUserService currentUserService, IIdentityManager identityManager)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._userDataAccess = userDataAccess;
            this._mapper = mapper;
            this._currentUserService = currentUserService;
            this._identityManager = identityManager;
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>UserId of created prospect</returns>
        public async Task<Unit> Handle(AddPermissionsForRoleRequest request, CancellationToken cancellationToken)
        {
            //1. Logging Information : In Process
            _logger.LogInformation("AddPermissionsForRoleRequest.Handle - In process");

            //2. Assign requested roles permission values 
            var entity = new Domain.Entities.RoleAndPermissionMapping();
            entity.RoleId = request.RoleAndPermissionMapping.RoleId;
            entity.PermissionIds = request.RoleAndPermissionMapping.PermissionIds;
            entity.CreatedBy = _currentUserService.UserId;

            //3. Add the Permissions for requested role
            await _userDataAccess.AddPermissionsForRole(entity.RoleId, entity.PermissionIds, entity.CreatedBy);

            //3.1 Only the users who actually hold this role are affected - reset their cache
            //    specifically (plus the global permissions list) rather than every cached user.
            var affectedUsers = await _identityManager.GetUserByRoleIdAsync(entity.RoleId);
            await _identityManager.ResetUserCache(affectedUsers?.Select(u => u.UserName).ToList());


            //4. Logging Information : Completed
            _logger.LogInformation("AddPermissionsForRoleRequest.Handle - Completed");

            //5. Return Unit
            return Unit.Value;
        }
    }
}


