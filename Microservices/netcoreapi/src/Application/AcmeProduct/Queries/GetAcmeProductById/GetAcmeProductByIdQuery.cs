using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.AcmeProduct.Queries.GetAcmeProductById
{
    public class GetAcmeProductByIdQuery : IRequest<AcmeProductDto>
    {
        public string Id { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request, created GetAcmeProductQueryHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class GetAcmeProductByIdHandler : IRequestHandler<GetAcmeProductByIdQuery, AcmeProductDto>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IAcmeDataAccess _dataAccess;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates GetAcmeProductQueryHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public GetAcmeProductByIdHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IAcmeDataAccess dataAccess)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._dataAccess = dataAccess;
            this._mapper = mapper;

            this._logger.LogInformation("GetAcmeProductByIdQuery.GetAcmeProductQueryHandler - constructor");
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<AcmeProductDto> Handle(GetAcmeProductByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetAcmeProductByIdQuery.Handle - In process");

            //Mapping AcmeProductDto with AcmeProduct entity
            return _mapper.Map<AcmeProductDto>(await _dataAccess.GetAcmeProductById(request.Id));
        }
    }
}
