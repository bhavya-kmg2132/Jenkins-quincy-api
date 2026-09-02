using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.ResetUserCache
{
    /// <summary>
    /// class GetAllUserQueryHandler extends the IRequest interface of MediatR
    /// </summary>
    public class ResetUserCacheRequest : IRequest<Unit>
    {

    }

    /// <summary>
    /// For Creating handler for the above request , created GetAllUserQueryHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class ResetUserCacheRequestHandler : IRequestHandler<ResetUserCacheRequest, Unit>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IIdentityManager _dataAccess;
        private readonly IUiPermissionDataAccess _uiPermissionDataAccess;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates GetAllUsersQueryHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public ResetUserCacheRequestHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IIdentityManager dataAccess, IUiPermissionDataAccess uiPermissionDataAccess)
        {
            _configuration = configuration;
            _logger = logger;
            _dataAccess = dataAccess;
            _mapper = mapper;
            _uiPermissionDataAccess = uiPermissionDataAccess;
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Unit> Handle(ResetUserCacheRequest request, CancellationToken cancellationToken)
        {
            await _dataAccess.ResetUserCache();
            await _uiPermissionDataAccess.ResetUiPermissionCache();

            return Unit.Value;
        }
    }
}

