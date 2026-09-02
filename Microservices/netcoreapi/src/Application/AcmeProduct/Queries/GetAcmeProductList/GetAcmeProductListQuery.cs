using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.AcmeProduct.Queries.GetAcmeProductList
{
    /// <summary>
    /// class GetAcmeProductByIdQuery extends the IRequest interface of MediatR
    /// </summary>
    public class GetAcmeProductListQuery : IRequest<AcmeProductListVm>
    {
    }

    /// <summary>
    /// For Creating handler for the above request, created GetAcmeProductListQueryHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>

    public class GetAcmeProductListQueryHandler : IRequestHandler<GetAcmeProductListQuery, AcmeProductListVm>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IAcmeDataAccess _AcmeProductDataAccess;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Instantiates GetAcmeProductListQueryHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>

        public GetAcmeProductListQueryHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IAcmeDataAccess acmeProductDataAccess, ICurrentUserService currentUserService)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._AcmeProductDataAccess = acmeProductDataAccess;
            this._mapper = mapper;
            this._currentUserService = currentUserService;
        }

        /// <summary>
        /// Handler will recieve request, process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>NotesVm</returns>

        public async Task<AcmeProductListVm> Handle(GetAcmeProductListQuery request, CancellationToken cancellationToken)
        {
            var userEntity = new AcmeProductListVm();

            // Execute domain rules for user has access to Admin rights.
            //  var ruleExecutionResultForAdmin  = new Application.Rules.AcmeProduct.IsAdminValid(_currentUserService).Execute(userEntity, true);

            // Execute domain rules for user has access to User rights.
            //  var ruleExecutionResultForUser = new Application.Rules.AcmeProduct.IsUserValid(_currentUserService).Execute(userEntity, true);

            // Execute domain rules for user has access to Power rights.
            //  var ruleExecutionResultForPower = new Application.Rules.AcmeProduct.IsPowerValid(_currentUserService).Execute(userEntity, true);

            // Execute domain rules for user has access to System Manager rights.
            //  var ruleExecutionResultForSystemManager = new Application.Rules.AcmeProduct.IsSystemManagerValid(_currentUserService).Execute(userEntity, true);

            // return request Acme Product
            return new AcmeProductListVm
            {
                //Mapping AcmeProductDto with Acme entity
                AcmeProductList = _mapper.Map<List<AcmeProductDto>>(await _AcmeProductDataAccess.GetAcmeProductList())
            };
        }

    }
}
