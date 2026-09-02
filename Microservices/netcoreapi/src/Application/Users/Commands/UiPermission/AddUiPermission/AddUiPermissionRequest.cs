using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.UiPermission.AddUiPermission
{
    /// <summary>
    /// class AddUiPermissionRequest extends the IRequest interface of MediatR
    /// </summary>
    public class AddUiPermissionRequest : IRequest<string>
    {
        public string PermissionId { get; set; }
        public string PermissionValue { get; set; }
        public string PermissionDisplayName { get; set; }
        public string PermissionTypeId { get; set; }
        public string PermissionParentId { get; set; }
        public string ModuleId { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request, created AddUiPermissionRequestHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class AddUiPermissionRequestHandler : IRequestHandler<AddUiPermissionRequest, string>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUiPermissionDataAccess _dataAccess;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates the class AddUiPermissionRequestHandler
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public AddUiPermissionRequestHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUiPermissionDataAccess dataAccess, ICurrentUserService currentUserService)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._dataAccess = dataAccess;
            this._mapper = mapper;
            this._currentUserService = currentUserService;
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Unit</returns>
        public async Task<string> Handle(AddUiPermissionRequest request, CancellationToken cancellationToken)
        {
            //1. Logging Information : In Process
            _logger.LogInformation("AddUiPermissionRequest.Handle - In process");

            //2. Add UiPermission
            NetAuth.Contract.DataContract.Requests.AddUiPermission addUiPermission = new NetAuth.Contract.DataContract.Requests.AddUiPermission();
            addUiPermission.PermissionId = request.PermissionId;
            addUiPermission.PermissionValue = request.PermissionValue;
            addUiPermission.PermissionDisplayName = request.PermissionDisplayName;
            addUiPermission.PermissionTypeId = request.PermissionTypeId;
            addUiPermission.PermissionParentId = request.PermissionParentId;
            addUiPermission.ModuleId = request.ModuleId;

            //3. Add Auditable fields to request
            addUiPermission.CreatedBy = _currentUserService.UserId;
            string permissionId = await _dataAccess.AddUiPermission(addUiPermission);

            //4. Logging Information : Completed
            _logger.LogInformation("AddUiPermissionsRequest.Handle - Completed");

            //5. Return Unit
            return permissionId;
        }
    }
}


