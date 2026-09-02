using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace Application.CronJobRule.Queries
{
    /// <summary>
    /// class GetNotificationUserSubscriptionsQuery extends the IRequest interface of MediateR
    /// </summary>
    public class GetNotificationUserSubscriptionsQuery : IRequest<NotificationUserSubscriptionVm>
    {
        public List<string> UserIds { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request , created GetAssignmentByIdQueryHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class GetNotificationUserSubscriptionsQueryHandler : IRequestHandler<GetNotificationUserSubscriptionsQuery, NotificationUserSubscriptionVm>
    {
        private readonly ILogger<GetNotificationUserSubscriptionsQuery> _logger;
        private readonly IConfiguration _configuration;
        private readonly ICronJobScheduler _timetrackNotificationService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates the GetNotificationUserSubscriptionsQueryHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="_cronJobScheduler"></param>
        public GetNotificationUserSubscriptionsQueryHandler(IConfiguration configuration, ILogger<GetNotificationUserSubscriptionsQuery> logger, ICronJobScheduler _cronJobScheduler, IMapper mapper)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._timetrackNotificationService = _cronJobScheduler;
            this._mapper = mapper;
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>AssignmentVm</returns>
        public async Task<NotificationUserSubscriptionVm> Handle(GetNotificationUserSubscriptionsQuery request, CancellationToken cancellationToken)
        {
            //Logging Information : In Process.
            _logger.LogInformation("GetNotificationUserSubscriptionsQuery.Handle - In process");

            var result = new NotificationUserSubscriptionVm
            {
                //2.1 Mapping NotificationUserSubscriptionDto with NotificationUserSubscription entity
                NotificationUserSubscriptions = _mapper.Map<List<NotificationUserSubscriptionDto>>(await _timetrackNotificationService.GetNotificationUserSubscriptionsAsync(request.UserIds))
            };

            //Logging Information: Completed
            _logger.LogInformation("GetNotificationUserSubscriptionsQuery.Handle - Completed");

            return result;
        }
    }
}
