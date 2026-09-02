using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.CronJobRule.Commands.UpsertNotificationUserSubscription
{
    /// <summary>
    /// UpsertNotificationUserSubscriptionCommand extends the IRequest interface of MediatR
    /// </summary>
    public class UpsertNotificationUserSubscriptionCommand : IRequest<int>
    {
        public List<NotificationUserSubscription> notificationUserSubscriptions;
    }

    /// <summary>
    /// For Creating handler for the above request , created UpsertNotificationUserSubscriptionCommand class
    /// That implements the IRequestHandler interface as shown below.
    /// </summary>
    public class UpsertNotificationUserSubscriptionCommandHandler : IRequestHandler<UpsertNotificationUserSubscriptionCommand, int>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICronJobScheduler _cronJobRuleScheduler;

        /// <summary>
        /// Instantiates the UpsertNotificationUserSubscriptionCommandHandler Class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="notificationService"></param>
        public UpsertNotificationUserSubscriptionCommandHandler(IConfiguration configuration, ILogger logger, ICronJobScheduler cronJobRuleScheduler, ICurrentUserService currentUserService)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._cronJobRuleScheduler = cronJobRuleScheduler;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response.  
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>int</returns>
        public async Task<int> Handle(UpsertNotificationUserSubscriptionCommand request, CancellationToken cancellationToken)
        {
            //1. Logging Information : In Process
            _logger.LogInformation("UpsertNotificationUserSubscriptionCommand.Handle - In Process");

            //3. Add the Assignment to the data access layer for persistence
            var response = await _cronJobRuleScheduler.UpsertNotificationUserSubscriptionAsync(request.notificationUserSubscriptions, _currentUserService.Email);

            //4. Logging Information : Completed
            _logger.LogInformation("UpsertNotificationUserSubscriptionCommand.Handle - Completed");

            //5. Return generated Notification id
            return response;
        }
    }
}


