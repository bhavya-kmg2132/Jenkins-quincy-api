using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries
{
    /// <summary>
    /// class GetUserQuery extends the IRequest interface of MediatR
    /// </summary>
    public class GetUserQuery : IRequest<UserVm>
    {
        public string UserId { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request, created GetUserQueryHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserVm>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserDataAccess _dataAccess;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates GetUserQueryHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public GetUserQueryHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUserDataAccess dataAccess)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._dataAccess = dataAccess;
            this._mapper = mapper;
        }

        /// <summary>
        /// Handler will receive request, process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>UserVm</returns>
        public async Task<UserVm> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            //1. Logging Information - In process
            _logger.LogInformation("GetUserQuery - In process");

            //2. Returns User
            return new UserVm
            {
                User = _mapper.Map<UserDto>(await _dataAccess.GetUserFromDbAsync(request.UserId))
            };
        }
    }
}

