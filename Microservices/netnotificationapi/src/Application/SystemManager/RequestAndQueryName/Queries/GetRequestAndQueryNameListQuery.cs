using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.RequestAndQueryName.Queries.GetRequestAndQueryNameListQuery
{
    /// <summary>
    /// class GetRequestAndQueryNameListQuery extends the IRequest interface of MediatR
    /// </summary>
    public class GetRequestAndQueryNameListQuery : IRequest<RequestAndQueryNameListVm>
    {

    }

    /// <summary>
    /// For Creating handler for the above request , created GetRequestAndQueryNameListQueryHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class GetRequestAndQueryNameListQueryHandler : IRequestHandler<GetRequestAndQueryNameListQuery, RequestAndQueryNameListVm>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates GetRequestAndQueryNameListQueryHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public GetRequestAndQueryNameListQueryHandler(IConfiguration configuration, ILogger logger, IMapper mapper)
        {
            this._configuration = configuration;
            this._logger = logger;
            // this._dataAccess = dataAccess;
            _mapper = mapper;
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<RequestAndQueryNameListVm> Handle(GetRequestAndQueryNameListQuery query, CancellationToken cancellationToken)
        {

            List<RequestAndQueryNameDto> RequestAndQueryNameDtoList = new List<RequestAndQueryNameDto>();
            RequestAndQueryNameListVm RequestAndQueryNameListVm = new RequestAndQueryNameListVm();
            foreach (Type type in System.Reflection.Assembly.GetExecutingAssembly()
           .GetTypes()
           .Where(mytype => mytype.GetInterfaces().Contains(typeof(IBaseRequest))))
            {
                if (!type.Name.Contains("TodoItem") && !type.Name.Contains("TodoLists") && !type.Name.Contains("Acme") && !type.Name.Contains("Todos"))
                {
                    RequestAndQueryNameDto RequestAndQueryNameDto = new RequestAndQueryNameDto
                    {
                        Description = type.Name,
                    };
                    RequestAndQueryNameDtoList.Add(RequestAndQueryNameDto);
                }
            }
            RequestAndQueryNameListVm.RequestAndQueryNameList = RequestAndQueryNameDtoList;
            await Task.CompletedTask;
            return RequestAndQueryNameListVm;
        }
    }
}

