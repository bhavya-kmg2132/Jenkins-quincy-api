using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.AuthReferenceLookup.GetAuthReferenceLookupQuery
{
    /// <summary>
    /// class GetAuthReferenceLookupQuery extends the IRequest interface of MediatR
    /// </summary>
    public class GetAuthReferenceLookupQuery : IRequest<AuthReferenceLookupVm>
    {
        public string Type { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request , created GetAuthReferenceLookupQueryHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class GetAuthReferenceLookupQueryHandler : IRequestHandler<GetAuthReferenceLookupQuery, AuthReferenceLookupVm>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserDataAccess _dataAccess;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates of GetAuthReferenceLookupQueryHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public GetAuthReferenceLookupQueryHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUserDataAccess dataAccess)
        {
            _configuration = configuration;
            _logger = logger;
            _dataAccess = dataAccess;
            _mapper = mapper;
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<AuthReferenceLookupVm> Handle(GetAuthReferenceLookupQuery query, CancellationToken cancellationToken)
        {
            //Return AuthReferenceLookup List Vm 
            return new AuthReferenceLookupVm
            {
                AuthReferenceLookups = _mapper.Map<List<AuthReferenceLookupDto>>(await _dataAccess.GetAuthReferenceLookupList(query.Type))
            };
        }
    }
}

