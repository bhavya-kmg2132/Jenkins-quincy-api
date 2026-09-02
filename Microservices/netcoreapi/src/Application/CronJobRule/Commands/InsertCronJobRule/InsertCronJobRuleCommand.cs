using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.CronJobRule.Commands.InsertCronJobRule
{
    /// <summary>
    /// InsertCronJobRuleCommand extends the IRequest interface of MediatR
    /// </summary>
    public class InsertCronJobRuleCommand : IRequest<string>
    {
        public string NotificationName { get; set; }
        public string Frequency { get; set; }
        public DateTime LastExecutionDate { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        public string ExecutionDay { get; set; }
        public string Role { get; set; }
        public bool IsNotificationPaused { get; set; }
        public int ExecutionMonth { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request , created InsertCronJobRuleCommand class
    /// That implements the IRequestHandler interface as shown below.
    /// </summary>
    public class InsertCronJobRuleCommandHandler : IRequestHandler<InsertCronJobRuleCommand, string>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ICronJobScheduler _cronJobScheduler;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Instantiates the InsertCronJobRuleCommandHandler Class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="notificationService"></param>
        public InsertCronJobRuleCommandHandler(IConfiguration configuration, ILogger logger, ICronJobScheduler cronJobScheduler, ICurrentUserService currentUserService)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._cronJobScheduler = cronJobScheduler;
            this._currentUserService = currentUserService;
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response.  
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>int</returns>
        public async Task<string> Handle(InsertCronJobRuleCommand request, CancellationToken cancellationToken)
        {
            //1. Logging Information : In Process
            _logger.LogInformation("InsertCronJobRuleCommand.Handle - In Process");

            //2. Create a new Assignment variable
            var notification = new Domain.Entities.CronJobRule();

            notification.Id = Guid.NewGuid().ToString();
            notification.NotificationName = request.NotificationName;
            notification.Frequency = request.Frequency;
            notification.ExecutionTime = request.ExecutionTime;
            notification.LastExecutionDate = request.LastExecutionDate;
            notification.ExecutionDay = request.ExecutionDay;
            notification.Role = request.Role;
            notification.IsNotificationPaused = request.IsNotificationPaused;
            notification.ExecutionMonth = request.ExecutionMonth;

            // Set correlation and audit-related properties
            notification.CorrelationId = _currentUserService.CorrelationId;
            notification.AuditableRequestId = _currentUserService.RequestId;
            notification.AuditableRequestName = nameof(InsertCronJobRuleCommand);
            notification.CreatedDateTime = System.DateTime.UtcNow;
            notification.CreatedBy = _currentUserService.UserName;
            notification.CreatedById = _currentUserService.UserId;

            //3. Add a domain event
            notification.DomainEvents.Add(new CronJobRuleCreatedEvent(notification));

            //4. Add the Assignment to the data access layer for persistence
            var response = await _cronJobScheduler.InsertCronJobRuleAsync(notification);

            //5. Logging Information : Completed
            _logger.LogInformation("InsertCronJobRuleCommand.Handle - Completed");

            //6. Return generated Notification id
            return response;
        }

    }
}


