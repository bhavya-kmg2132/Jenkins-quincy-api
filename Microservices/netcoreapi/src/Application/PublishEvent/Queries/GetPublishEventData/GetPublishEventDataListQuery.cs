using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.PublishEvent.Queries.GetPublishEventDataList
{
    public class GetPublishEventDataListQuery : IRequest<PublishEventDataListVm>
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string ColumnName { get; set; }
        public string SearchText { get; set; }
        public string OrderType { get; set; }
        public string FilterJson { get; set; }
    }

    public class GetPublishEventDataListQueryHandler : IRequestHandler<GetPublishEventDataListQuery, PublishEventDataListVm>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IPublishEventDataAccess _dataAccess;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates GetPublishEventDatasListQueryHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        /// <param name="dataAccess"></param>
        public GetPublishEventDataListQueryHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IPublishEventDataAccess dataAccess)
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
        /// <returns>PublishEventDataListVm</returns>
        public async Task<PublishEventDataListVm> Handle(GetPublishEventDataListQuery request, CancellationToken cancellationToken)
        {

            var (PublishEventData, PublishEventDataCount) = await _dataAccess.GetList(request.PageNumber, request.PageSize, request.OrderType, request.ColumnName, request.FilterJson, request.SearchText);

            // Map the PublishEventDatas to PublishEventDataDto
            var result = new PublishEventDataListVm
            {
                PublishEventData = _mapper.Map<List<PublishEventDataDto>>(PublishEventData),
                TotalCount = PublishEventDataCount
            };
            return result;
        }
    }
}
