using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace Application.CronJobRule.Queries.GetCronJobRuleById
{
    /// <summary>
    /// class GetCronJobRuleByIdQuery extends the IRequest interface of MediateR
    /// </summary>
    public class GetCronJobRuleByIdQuery : IRequest<CronJobRuleDto>
    {
        public string Id { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request , created GetAssignmentByIdQueryHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class GetCronJobRuleByIdQueryHandler : IRequestHandler<GetCronJobRuleByIdQuery, CronJobRuleDto>
    {
        private readonly ILogger<GetCronJobRuleByIdQuery> _logger;
        private readonly IConfiguration _configuration;
        private readonly ICronJobScheduler _cronJobScheduler;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates the GetCronJobRuleByIdQueryHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="_timetrackCronJobService"></param>
        public GetCronJobRuleByIdQueryHandler(IConfiguration configuration, ILogger<GetCronJobRuleByIdQuery> logger, ICronJobScheduler _cronJobScheduler, IMapper mapper)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._cronJobScheduler = _cronJobScheduler;
            this._mapper = mapper;
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>AssignmentVm</returns>
        public async Task<CronJobRuleDto> Handle(GetCronJobRuleByIdQuery request, CancellationToken cancellationToken)
        {
            //Logging Information : In Process.
            _logger.LogInformation("GetCronJobRuleByIdQuery.Handle - In process");

            var response = _mapper.Map<CronJobRuleDto>
                           (await _cronJobScheduler.GetCronJobRuleByIdAsync(request.Id));

            //Logging Information: Completed
            _logger.LogInformation("GetCronJobRuleByIdQuery.Handle - Completed");

            return response;
        }
    }
}
