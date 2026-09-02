using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.User.EventHandlers
{
    /// <summary>
    /// For Creating handler for the domain event notification,created UserCreatedEventHandler 
    /// class that implements the INotificationHandler interface as shown below.
    /// </summary>
    public class UserCreatedEventHandler : INotificationHandler<DomainEventNotification<UserCreatedEvent>>
    {
        private readonly ILogger<UserCreatedEventHandler> _logger;
        private IConfiguration _configuration;
        private readonly IPublishEventDataAccess _publishEventDataAccess;


        /// <summary>
        /// Instantiation of UserCreatedEventHandler class
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        public UserCreatedEventHandler(IConfiguration configuration, ILogger<UserCreatedEventHandler> logger, IPublishEventDataAccess publishEventDataAccess)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._publishEventDataAccess = publishEventDataAccess;
        }

        /// <summary>
        /// Handler will recieve notification ,process it and will return the response. 
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Task.CompletedTask</returns>
        public Task Handle(DomainEventNotification<UserCreatedEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            domainEvent.UsersDetails.AuditableSourceEventName = nameof(UserCreatedEvent);

            _logger.LogInformation("Domain Event: {DomainEvent}", domainEvent.GetType().Name);

            if (Convert.ToBoolean(this._configuration["AddEventDataForAuditLog"]))
            {
                AddEventDataForAuditLog(domainEvent);
            }

            return Task.CompletedTask;
        }

        //add event data for Audit Log
        private async void AddEventDataForAuditLog(UserCreatedEvent domainEvent)
        {
            try
            {
                //1. Prepare PublishEventData object for Audit Log
                var PublishEventData = PrepareEventDataForAuditLog(domainEvent);

                //2. Add PublishEventData to database
                await _publishEventDataAccess.Add(PublishEventData);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured while adding PublishEventData object to database - " + ex.Message);
            }
        }

        //prepare event data for Audit Log
        private PublishEventData PrepareEventDataForAuditLog(UserCreatedEvent domainEvent)
        {
            //prepare event data for Audit Log
            var newObject = domainEvent.UsersDetails;
            var eventData = new PublishEventData { EventData = new List<Domain.Common.Property>() };
            eventData.AuditableSourceEventName = newObject.AuditableSourceEventName;
            eventData.OperationType = Domain.Common.OperationType.INSERT;
            eventData.CreatedDateTime = DateTime.UtcNow;
            eventData.OperationSource = OperationSource.WEBPAGE;
            eventData.ApiName = this._configuration["Api:internal_name"];
            eventData.CollectionName = "User";

            //Get all the public and private members of the class/object
            var objectProperties = Helper.RetrievePropertiesWithFilter(newObject,
                         BindingFlags.Instance |
                         BindingFlags.Public |
                         BindingFlags.NonPublic);

            //Filter out the properties named 'DomainEvents' and 'Done'
            var filteredProperties = objectProperties
                .Where(prop => prop.Name != "DomainEvents" && prop.Name != "Done")
                .ToList();

            // Loop through all the members of the object and add them to eventData
            foreach (var item in filteredProperties)
            {
                // Get the value of the property from newObject
                var newValue = item.GetValue(newObject, null);

                // Check if the property should be included in eventData
                eventData.EventData.Add(new Domain.Common.Property
                {
                    PropertyName = item.Name,
                    NewValue = newValue,
                });
            }

            //returns eventData
            return eventData;
        }
    }
}
