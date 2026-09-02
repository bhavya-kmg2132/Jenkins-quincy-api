using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.CronJobRule.Commands.InsertCronJobRule;
using Domain.Common;
using Domain.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.CronJobRule.Commands.UpdateCronJobRule
{
    /// <summary>
    /// UpdateCronJobRuleCommand extends the IRequest interface of MediatR
    /// </summary>
    public class UpdateCronJobRuleCommand : IRequest<int>
    {
        public string Id { get; set; }
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
    /// For Creating handler for the above request , created UpdateCronJobRuleCommand class
    /// That implements the IRequestHandler interface as shown below.
    /// </summary>
    public class UpdateCronJobRuleCommandHandler : IRequestHandler<UpdateCronJobRuleCommand, int>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ICronJobScheduler _cronJobScheduler;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Instantiates the UpdateCronJobRuleCommandHandler Class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="notificationService"></param>
        public UpdateCronJobRuleCommandHandler(IConfiguration configuration, ILogger logger, ICronJobScheduler cronJobScheduler, ICurrentUserService currentUserService)
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
        public async Task<int> Handle(UpdateCronJobRuleCommand request, CancellationToken cancellationToken)
        {
            //1. Logging Information : In Process
            _logger.LogInformation("UpdateCronJobRuleCommand.Handle - In Process");

            //2. Find the request id for update in database
            var oldCronJobRule = await _cronJobScheduler.GetCronJobRuleByIdAsync(request.Id);

            //3. if the entity not found then throw NotFoundException
            if (oldCronJobRule == null)
            {
                throw new NotFoundException(nameof(Domain.Entities.CronJobRule), request.Id);
            }

            //4. Deep copy existing object before overriding it with new values
            var newCronJobRule = (Domain.Entities.CronJobRule)Helper.CloneObject(oldCronJobRule);

            newCronJobRule.Id = request.Id;
            newCronJobRule.NotificationName = request.NotificationName;
            newCronJobRule.Frequency = request.Frequency;
            newCronJobRule.ExecutionTime = request.ExecutionTime;
            newCronJobRule.LastExecutionDate = request.LastExecutionDate;
            newCronJobRule.ExecutionDay = request.ExecutionDay;
            newCronJobRule.Role = request.Role;
            newCronJobRule.IsNotificationPaused = request.IsNotificationPaused;
            newCronJobRule.ExecutionMonth = request.ExecutionMonth;

            // Set correlation and audit-related properties
            newCronJobRule.CorrelationId = _currentUserService.CorrelationId;
            newCronJobRule.AuditableRequestId = _currentUserService.RequestId;
            newCronJobRule.AuditableRequestName = nameof(InsertCronJobRuleCommand);
            newCronJobRule.CreatedDateTime = System.DateTime.UtcNow;
            newCronJobRule.CreatedBy = _currentUserService.UserName;
            newCronJobRule.CreatedById = _currentUserService.UserId;

            //5. Add a domain event
            newCronJobRule.DomainEvents.Add(new CronJobRuleUpdatedEvent(newCronJobRule, oldCronJobRule));

            //6. Add the Assignment to the data access layer for persistence
            var response = await _cronJobScheduler.UpdateCronJobRuleAsync(newCronJobRule, oldCronJobRule);

            //7. Logging Information : Completed
            _logger.LogInformation("UpdateCronJobRuleCommand.Handle - Completed");

            //8. Return generated Notification id
            return response;
        }
    }
}


