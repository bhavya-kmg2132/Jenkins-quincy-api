using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.AddRoles
{
    /// <summary>
    /// class AddUserActivityRequest extends the IRequest interface of MediatR
    /// </summary>
    public class AddUserActivityRequest : IRequest<string>
    {
        public string UserId { get; set; }
        public DateTime? LastLoginDateTime { get; set; }
        public DateTime? LastActivityDateTime { get; set; }
        public string LastActivityModule { get; set; }
        public string LastActionType { get; set; }
        public string LastActivityDetail { get; set; }
        public bool IsUserLogout { get; set; } = false;
    }

    /// <summary>
    /// For Creating handler for the above request, created AddUserActivityRequestHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class AddUserActivityRequestHandler : IRequestHandler<AddUserActivityRequest, string>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserDataAccess _userDataAccess;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates the class AddUserActivityRequestHandler
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public AddUserActivityRequestHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUserDataAccess userDataAccess, ICurrentUserService currentUserService)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._userDataAccess = userDataAccess;
            this._mapper = mapper;
            this._currentUserService = currentUserService;
        }

        /// <summary>
        /// Handler will recieve request, process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>string</returns>
        public async Task<string> Handle(AddUserActivityRequest request, CancellationToken cancellationToken)
        {

            //1. Logging Information : In Process
            _logger.LogInformation("AddUserActivityRequest.Handle - In process");

            //2. Build contract request directly
            var addUserActivity = new NetAuth.Contract.DataContract.Requests.AddUserActivity
            {
                UserId               = request.UserId,
                LastLoginDateTime    = request.LastLoginDateTime,
                LastActivityDateTime = request.LastActivityDateTime,
                LastActivityModule   = request.LastActivityModule,
                LastActionType       = request.LastActionType,
                LastActivityDetail   = request.LastActivityDetail,
                IsUserLogout         = request.IsUserLogout,
                CreatedBy            = _currentUserService.UserId
            };

            //3. Persist user activity
            var userActivityId = await _userDataAccess.AddUserActivity(addUserActivity);

            //4. Logging Information : Completed
            _logger.LogInformation("AddUserActivityRequest.Handle - Completed");

            //5. Return userActivityId
            return userActivityId;
        }
    }
}


