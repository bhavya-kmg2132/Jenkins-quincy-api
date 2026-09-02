using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
namespace Application.Users.Commands.UserPermissions.AddPermissionRequest
{
    /// <summary>
    /// class AddPermissionRequest extends the IRequest interface of MediatR
    /// </summary>
    public class AddPermissionRequest : IRequest<Unit>
    {
        public string PermissionId { get; set; }
        public string PermissionValue { get; set; }
        public string PermissionDisplayName { get; set; }
        public string PermissionSetId { get; set; }
        public string ModuleId { get; set; }
        public string PermissionType { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request , created AddPermissionsRequestHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class AddPermissionRequestHandler : IRequestHandler<AddPermissionRequest, Unit>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserDataAccess _userDataAccess;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates the class CreateProspectRequestHandler
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public AddPermissionRequestHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUserDataAccess userDataAccess, ICurrentUserService currentUserService)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._userDataAccess = userDataAccess;
            this._mapper = mapper;
            this._currentUserService = currentUserService;
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Unit</returns>
        public async Task<Unit> Handle(AddPermissionRequest request, CancellationToken cancellationToken)
        {
            //1. Logging Information : In Process
            _logger.LogInformation("AddPermissionsRequest.Handle - In process");

            NetAuth.Contract.DataContract.Requests.AddPermission permission = new NetAuth.Contract.DataContract.Requests.AddPermission();
            permission.PermissionValue = request.PermissionValue;
            permission.PermissionDisplayName = request.PermissionDisplayName;
            permission.PermissionSetId = request.PermissionSetId;
            permission.ModuleId = request.ModuleId;
            permission.PermissionType = request.PermissionType;
            
            //2. Add permissions granted for user
            await _userDataAccess.AddPermission(permission, _currentUserService.UserName);

            //3. Logging Information : Completed
            _logger.LogInformation("AddPermissionsRequest.Handle - Completed");

            //4. Return Unit
            return Unit.Value;
        }
    }
}


