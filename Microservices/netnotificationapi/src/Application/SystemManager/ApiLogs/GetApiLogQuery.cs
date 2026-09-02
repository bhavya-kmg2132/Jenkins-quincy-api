using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.ApiLog.Queries.GetApiLogQuery
{
    /// <summary>
    /// class GetApiLogQuery extends the IRequest interface of MediatR
    /// </summary>
    public class GetApiLogQuery : IRequest<ApiLogListVm>
    {

    }

    /// <summary>
    /// For Creating handler for the above request , created GetAcitivityQueryHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class GetApiLogQueryHandler : IRequestHandler<GetApiLogQuery, ApiLogListVm>
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
        public GetApiLogQueryHandler(IConfiguration configuration, ILogger logger, IMapper mapper)
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
        public async Task<ApiLogListVm> Handle(GetApiLogQuery query, CancellationToken cancellationToken)
        {
            ApiLogListVm apiList = new ApiLogListVm();
            var apiLogListData = new List<ApiLogDto>();

            string logFileName = "App-Information.log";
            var dllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var dllParentPath = Path.GetDirectoryName(dllPath);
            var logFilePath = Path.Combine(dllParentPath, "Logs", logFileName);

            // Using FileStream and MemoryStream to read the file efficiently
            using (FileStream fileStream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (MemoryStream memoryStream = new MemoryStream())
            using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                await streamReader.BaseStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0; // Reset the position for reading

                using (StreamReader memoryStreamReader = new StreamReader(memoryStream, Encoding.UTF8))
                {
                    string line;
                    while ((line = memoryStreamReader.ReadLine()) != null)
                    {
                        apiLogListData.Add(new ApiLogDto { Discription = line });
                    }
                }
            }

            apiLogListData.Reverse();
            apiList.ApiLogList = apiLogListData;
            return apiList;
        }
    }
}
