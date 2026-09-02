using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.ApiLog.Queries.GetAppInformationQuery
{
    /// <summary>
    /// class GetAppInformationQuery extends the IRequest interface of MediatR
    /// </summary>
    public class GetAppInformationQuery : IRequest<ApiLogListVm>
    {

    }

    /// <summary>
    /// For Creating handler for the above request , created GetAcitivityQueryHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class GetAppInformationQueryHandler : IRequestHandler<GetAppInformationQuery, ApiLogListVm>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates GetAcitivityQueryHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public GetAppInformationQueryHandler(IConfiguration configuration, ILogger logger, IMapper mapper)
        {
            this._configuration = configuration;
            this._logger = logger;
            // this._dataAccessAcmeDetail = dataAccess;
            _mapper = mapper;
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiLogListVm> Handle(GetAppInformationQuery query, CancellationToken cancellationToken)
        {

            ApiLogListVm apiList = new ApiLogListVm();

            string[] lines;
            var list = new List<string>();

            var dllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var dllParentPath = Path.GetDirectoryName(dllPath);
            var dllGrandParentPath = Path.GetDirectoryName(dllParentPath);
            var path = Path.Combine(dllParentPath + "\\Logs", "App-Information.log");

            using (var streamReader = new StreamReader(path, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    list.Add(line);
                }
            }
            lines = list.ToArray();
            Array.Reverse(lines);

            List<ApiLogDto> ApiLogListData = new List<ApiLogDto>();
            foreach (var line in lines)
            {
                ApiLogDto apiLog = new ApiLogDto
                {
                    Discription = line.ToString()
                };

                ApiLogListData.Add(apiLog);
            }

            apiList.ApiLogList = ApiLogListData;
            await Task.CompletedTask;
            return apiList;

        }

    }
}
