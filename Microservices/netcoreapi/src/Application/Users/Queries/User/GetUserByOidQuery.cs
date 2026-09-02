using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.User
{
    /// <summary>
    /// class GetUserByOidQuery  extends the IRequest interface of MediatR
    /// </summary>
    public class GetUserByOidQuery : IRequest<NetAuth.Contract.DataContract.Entities.User>
    {
        public string Oid { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request, created GetUserByOidQueryHandler Handler class
    /// that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class GetUserByOidQueryHandler : IRequestHandler<GetUserByOidQuery, NetAuth.Contract.DataContract.Entities.User>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserDataAccess _dataAccess;
        private readonly IMapper _mapper;

        /// <summary>
        ///  Instantiates GetUserByOidQueryHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        /// <param name="dataAccess"></param>
        public GetUserByOidQueryHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUserDataAccess dataAccess)
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
        /// <returns>User</returns>
        public async Task<NetAuth.Contract.DataContract.Entities.User> Handle(GetUserByOidQuery request, CancellationToken cancellationToken)
        {
            //1. Logging Information - In process
            _logger.LogInformation("GetUserByOidQuery - In process");

            //2. Returns User
            return _mapper.Map<NetAuth.Contract.DataContract.Entities.User>(await _dataAccess.GetUserFromDbAsync(request.Oid));
        }
    }
}

