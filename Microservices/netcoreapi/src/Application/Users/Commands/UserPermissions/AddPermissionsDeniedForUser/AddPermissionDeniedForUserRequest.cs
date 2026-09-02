using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.UserPermissions.AddPermissionsDeniedForUser
{
    /// <summary>
    /// class AddPermissionDeniedForUserRequest extends the IRequest interface of MediatR
    /// </summary>
    public class AddPermissionDeniedForUserRequest : IRequest<Unit>
    {
        public string UserId { get; set; }
        public List<string> PermissionIds { get; set; }

    }

    /// <summary>
    /// For Creating handler for the above request , created CreatePermissionGrantedForUserRequestHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class CreatePermissionDeniedForUserRequestHandler : IRequestHandler<AddPermissionDeniedForUserRequest, Unit>
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
        public CreatePermissionDeniedForUserRequestHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUserDataAccess userDataAccess, ICurrentUserService currentUserService)
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
        public async Task<Unit> Handle(AddPermissionDeniedForUserRequest request, CancellationToken cancellationToken)
        {
            //1. Logging Information : In Process
            _logger.LogInformation("AddPermissionGrantedForUserRequest.Handle - In process");

            string createdBy = _currentUserService.oid;

            //2. Add permissions denied for user
            await _userDataAccess.AddPermissionsDeniedForUser(request.UserId, request.PermissionIds, createdBy);

            //3. Logging Information : Completed
            _logger.LogInformation("AddPermissionGrantedForUserRequest.Handle - Completed");

            //4. Return Unit
            return Unit.Value;
        }
    }
}


