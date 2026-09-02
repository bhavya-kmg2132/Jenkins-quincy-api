using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Account.EventHandlers
{
    public class UserActivityEventHandler : INotificationHandler<DomainEventNotification<UserActivityEvent>>
    {
        private readonly ILogger<UserActivityEventHandler> _logger;
        private IConfiguration _configuration;
        private readonly IUserDataAccess _dataAccess;

        public UserActivityEventHandler(IConfiguration configuration, ILogger<UserActivityEventHandler> logger, IUserDataAccess dataAccess)
        {
            this._configuration = configuration;
            _logger = logger;
            _dataAccess = dataAccess;
        }

        public Task Handle(DomainEventNotification<UserActivityEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            _logger.LogInformation("Domain Event: {User Activity DomainEvent}", domainEvent.GetType().Name);
            _dataAccess.AddUserActivity(domainEvent.UserActivityCompletedObject);
            return Task.CompletedTask;
        }
    }
}
