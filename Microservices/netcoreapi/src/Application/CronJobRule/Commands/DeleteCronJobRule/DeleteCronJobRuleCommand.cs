using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.CronJobRule.Commands.DeleteCronJobRule
{
    /// <summary>
    /// DeleteCronJobRuleCommand extends the IRequest interface of MediatR
    /// </summary>
    public class DeleteCronJobRuleCommand : IRequest<string>
    {
        public string Id { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request , created DeleteCronJobRuleCommand class
    /// That implements the IRequestHandler interface as shown below.
    /// </summary>
    public class DeleteCronJobRuleCommandHandler : IRequestHandler<DeleteCronJobRuleCommand, string>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ICronJobScheduler _cronJobScheduler;

        /// <summary>
        /// Instantiates the DeleteCronJobRuleCommandHandler Class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="notificationService"></param>
        public DeleteCronJobRuleCommandHandler(IConfiguration configuration, ILogger logger, ICronJobScheduler cronJobScheduler)
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
        /// <returns>int</returns>
        public async Task<string> Handle(DeleteCronJobRuleCommand request, CancellationToken cancellationToken)
        {
            //1. Logging Information : In Process
            _logger.LogInformation("DeleteCronJobRuleCommand.Handle - In Process");

            //2. Create a new Assignment variable
            var notification = new Domain.Entities.CronJobRule();
            notification = await _cronJobScheduler.GetCronJobRuleByIdAsync(request.Id);

            if (notification == null)
            {
                return "No CronJobRule found to delete.";
            }

            // Add a domain event
            notification.DomainEvents.Add(new CronJobRuleDeletedEvent(notification));

            //3. Add the Assignment to the data access layer for persistence
            var response = await _cronJobScheduler.DeleteCronJobRuleAsync(notification);

            //4. Logging Information : Completed
            _logger.LogInformation("DeleteCronJobRuleCommand.Handle - Completed");

            //5. Return affected rows number
            return response.ToString();
        }
    }
}


