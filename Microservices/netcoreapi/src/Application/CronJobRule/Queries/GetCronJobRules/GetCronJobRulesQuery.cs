using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace Application.CronJobRule.Queries.GetCronJobRules
{
    /// <summary>
    /// class GetCronJobRulesQuery extends the IRequest interface of MediateR
    /// </summary>
    public class GetCronJobRulesQuery : IRequest<List<Domain.Entities.CronJobRule>>
    {

    }

    /// <summary>
    /// For Creating handler for the above request , created GetAssignmentByIdQueryHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class GetCronJobRulesQueryHandler : IRequestHandler<GetCronJobRulesQuery, List<Domain.Entities.CronJobRule>>
    {
        private readonly ILogger<GetCronJobRulesQuery> _logger;
        private readonly IConfiguration _configuration;
        private readonly ICronJobScheduler _cronJobScheduler;

        /// <summary>
        /// Instantiates the GetCronJobRulesQueryHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="_timetrackCronJobService"></param>
        public GetCronJobRulesQueryHandler(IConfiguration configuration, ILogger<GetCronJobRulesQuery> logger, ICronJobScheduler cronJobScheduler)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._cronJobScheduler = cronJobScheduler;
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>AssignmentVm</returns>
        public async Task<List<Domain.Entities.CronJobRule>> Handle(GetCronJobRulesQuery request, CancellationToken cancellationToken)
        {
            //Logging Information : In Process.
            _logger.LogInformation("GetCronJobRulesQuery.Handle - In process");

            var response = await _cronJobScheduler.GetCronJobRulesAsync();

            //Logging Information: Completed
            _logger.LogInformation("GetCronJobRulesQuery.Handle - Completed");

            return response;
        }
    }
}
