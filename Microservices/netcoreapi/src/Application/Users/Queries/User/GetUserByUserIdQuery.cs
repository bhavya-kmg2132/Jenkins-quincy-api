using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.User
{
    /// <summary>
    /// class GetUserByUserIdQuery extends the IRequest interface of MediatR
    /// </summary>
    public class GetUserByUserIdQuery : IRequest<UserDto>
    {
        public string UserId { get; set; }

    }

    /// <summary>
    /// For Creating handler for the above request, created GetUserByUserIdQueryHandler class
    /// that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class GetUserByUserIdQueryHandler : IRequestHandler<GetUserByUserIdQuery, UserDto>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserDataAccess _dataAccess;
        private readonly IMapper _mapper;

        /// <summary>
        ///  Instantiates GetUserByUserIdQueryHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        /// <param name="dataAccess"></param>
        public GetUserByUserIdQueryHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUserDataAccess dataAccess)
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
        /// <returns>UserDto</returns>
        public async Task<UserDto> Handle(GetUserByUserIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetUserByUserIdQuery - In process");

            return _mapper.Map<UserDto>(await _dataAccess.GetUserFromDbAsync(request.UserId));
        }
    }
}

