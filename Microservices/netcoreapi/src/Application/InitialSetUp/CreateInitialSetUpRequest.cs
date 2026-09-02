using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.InitialSetUp.CreateInitialSetUpRequest
{
    /// <summary>
    /// Create InitialSetUpRequest
    /// </summary>
    public class CreateInitialSetUpRequest : IRequest<Unit>
    {
    }

    /// <summary>
    /// CreateInitialSetUpRequestHandler : Handle to CreateInitialSetUpRequest and will return id of this created contact.
    /// </summary>
    public class CreateInitialSetUpRequestHandler : IRequestHandler<CreateInitialSetUpRequest, Unit>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IInitialSetUpDataAccess _initialSetUpDataAccess;
        private readonly IMainDbInitialSetUpDataAccess _mainDbInitialSetUpDataAccess;
        private readonly IEventDbInitialSetUpDataAccess _eventDbInitialSetUpDataAccess;
        private readonly ICurrentUserService _currentUserService;

        public CreateInitialSetUpRequestHandler(
            IConfiguration configuration,
            ILogger logger,
            IInitialSetUpDataAccess initialSetUpDataAccess,
            IMainDbInitialSetUpDataAccess mainDbInitialSetUpDataAccess,
            IEventDbInitialSetUpDataAccess eventDbInitialSetUpDataAccess,
            ICurrentUserService currentUserService)
        {
            _configuration = configuration;
            _logger = logger;
            _initialSetUpDataAccess = initialSetUpDataAccess;
            _mainDbInitialSetUpDataAccess = mainDbInitialSetUpDataAccess;
            _eventDbInitialSetUpDataAccess = eventDbInitialSetUpDataAccess;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(CreateInitialSetUpRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("CreateInitialSetUpRequest.Handle - In Process");

            await _initialSetUpDataAccess.Add();
            await _mainDbInitialSetUpDataAccess.Add();
            await _eventDbInitialSetUpDataAccess.Add();

            _logger.LogInformation("CreateInitialSetUpRequest.Handle - Completed");

            return Unit.Value;
        }
    }
}
