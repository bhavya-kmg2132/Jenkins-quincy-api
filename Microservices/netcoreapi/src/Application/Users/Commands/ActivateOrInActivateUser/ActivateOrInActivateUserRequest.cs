using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.ActivateOrInActivateUser
{
    /// <summary>
    /// class ActivateOrInActivateUserRequest extends the IRequest interface of MediatR
    /// </summary>
    public class ActivateOrInActivateUserRequest : IRequest<int>
    {
        public string userId { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request , created ActivateOrInActivateUserRequestHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class ActivateOrInActivateUserRequestHandler : IRequestHandler<ActivateOrInActivateUserRequest, int>
    {
        private readonly ILogger _logger;
        private readonly IUserDataAccess _userDataAccess;

        /// <summary>
        /// Instantiates the class ActivateOrInActivateUserRequestHandler
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public ActivateOrInActivateUserRequestHandler(ILogger logger, IUserDataAccess userDataAccess)
        {
            this._logger = logger;
            this._userDataAccess = userDataAccess;
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>UserId of created user</returns>
        public async Task<int> Handle(ActivateOrInActivateUserRequest request, CancellationToken cancellationToken)
        {
            //1. Logging Information : In Process
            _logger.LogInformation("ActivateOrInActivateUserRequest.Handle - In process");

            NetAuth.Contract.DataContract.Requests.ActivateOrInActivateUser activateOrInActivateUser = new NetAuth.Contract.DataContract.Requests.ActivateOrInActivateUser();
            activateOrInActivateUser.UserId = request.userId;
            activateOrInActivateUser.IsActive = request.IsActive;


            //2. Activate Or InActive User
            int status = await _userDataAccess.ActivateOrInActivateUser(activateOrInActivateUser);

            //3. Logging Information : Completed
            _logger.LogInformation("ActivateOrInActivateUserRequest.Handle - Completed");

            //4. Return id
            return status;
        }
    }
}



