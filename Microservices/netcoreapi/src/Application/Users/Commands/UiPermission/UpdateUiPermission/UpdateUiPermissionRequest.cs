using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.UiPermission.UpdateUiPermission
{
    /// <summary>
    /// class UpdateUiPermissionRequest extends the IRequest interface of MediatR
    /// </summary>
    public class UpdateUiPermissionRequest : IRequest<Unit>
    {
        public string PermissionId { get; set; }
        public string PermissionDisplayName { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request, created UpdateUiPermissionRequestHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class UpdateUiPermissionRequestHandler : IRequestHandler<UpdateUiPermissionRequest, Unit>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUiPermissionDataAccess _dataAccess;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates the class UpdateUiPermissionRequestHandler
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public UpdateUiPermissionRequestHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUiPermissionDataAccess dataAccess, ICurrentUserService currentUserService)
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
        /// <returns>Unit</returns>
        public async Task<Unit> Handle(UpdateUiPermissionRequest request, CancellationToken cancellationToken)
        {
            //1. Logging Information : In Process
            _logger.LogInformation("UpdateUiPermissionRequest.Handle - In process");

            //2. Add UiPermission
            NetAuth.Contract.DataContract.Requests.UpdateUiPermission updateUiPermission = new NetAuth.Contract.DataContract.Requests.UpdateUiPermission();
            updateUiPermission.PermissionId = request.PermissionId;
            updateUiPermission.PermissionDisplayName = request.PermissionDisplayName;
            updateUiPermission.IsActive = request.IsActive;

            //3. Add Auditable fields to request
            updateUiPermission.UpdatedBy = _currentUserService.UserId;
            await _dataAccess.UpdateUiPermission(updateUiPermission);

            //4. Logging Information : Completed
            _logger.LogInformation("UpdateUiPermissionRequest.Handle - Completed");

            //5. Return Unit
            return Unit.Value;
        }
    }
}


