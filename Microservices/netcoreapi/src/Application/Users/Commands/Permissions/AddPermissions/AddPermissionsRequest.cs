using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.UserPermissions.AddPermissionsRequest
{
    /// <summary>
    /// class AddPermissionsRequest extends the IRequest interface of MediatR
    /// </summary>
    public class AddPermissionsRequest : IRequest<Unit>
    {
        public string PermissionValue { get; set; }
        public string PermissionDisplayName { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request , created AddPermissionsRequestHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class AddPermissionsRequestHandler : IRequestHandler<AddPermissionsRequest, Unit>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserDataAccess _userDataAccess;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates the class AddPermissionsRequestHandler
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public AddPermissionsRequestHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUserDataAccess userDataAccess, ICurrentUserService currentUserService)
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
        public async Task<Unit> Handle(AddPermissionsRequest request, CancellationToken cancellationToken)
        {
            //1. Logging Information : In Process
            _logger.LogInformation("AddPermissionsRequest.Handle - In process");

            //2. Add permissions granted for user
            await _userDataAccess.AddPermissions(request.PermissionValue, request.PermissionDisplayName, _currentUserService.UserName);

            //3. Logging Information : Completed
            _logger.LogInformation("AddPermissionsRequest.Handle - Completed");

            //4. Return Unit
            return Unit.Value;
        }
    }
}


