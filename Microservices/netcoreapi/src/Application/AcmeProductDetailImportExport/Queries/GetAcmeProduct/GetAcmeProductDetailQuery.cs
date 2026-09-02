using System;
using System.Threading;
using System.Threading.Tasks;
using Application.AcmeProductDetailExport.Queries.GetAcmeProduct;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.AcmeProductDetailExport.Queries
{
    /// <summary>
    /// class GetAcmeProductDetailQuery extends the IRequest interface of MediatR 
    /// </summary>
    public class GetAcmeProductDetailQuery : IRequest<GetAcmeProductDetailVm>
    {

    }

    public class GetAcmeProductDetailQueryHandler : IRequestHandler<GetAcmeProductDetailQuery, GetAcmeProductDetailVm>
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
        public GetAcmeProductDetailQueryHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IAcmeDataAccess dataAccess)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._dataAccess = dataAccess;
            _mapper = mapper;
            _logger.LogInformation("GetAcmeProductDetailQuery.GetAcmeProductDetailQueryHandler - constructor");

        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        public async Task<GetAcmeProductDetailVm> Handle(GetAcmeProductDetailQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetAcmeProductDetailQuery.Handle - In process");
            //var list = _mapper.Map<List<GetAcmeProductDetailDto>>(await _dataAccess.GetAcmeProductExportList());
            var vm = new GetAcmeProductDetailVm();
            //vm.Content = _fileBuilder.BuildTodoItemsFile(list);
            vm.ContentType = "text/csv";
            vm.FileName = "ProductDeatilExportedFile" + DateTime.Now.ToString("MMddyy_HHMM") + ".csv";
            return await Task.FromResult(vm);
        }
    }
}
