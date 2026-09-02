using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Events.Master;
//using Domain.Events.Note;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.GenericMaster.EventHandlers
{
    /// <summary>
    /// For Creating handler for the domain event notification,created GenericMasterCompletedEventHandler 
    /// class that implements the INotificationHandler interface as shown below.
    /// </summary>
    public class GenericMasterCompletedEventHandler : INotificationHandler<DomainEventNotification<GenericMasterCompletedEvent>>
    {
        private readonly ILogger<GenericMasterCompletedEventHandler> _logger;
        private IConfiguration _configuration;
        private readonly IPublishEventDataAccess _publishEventDataAccess;
        /// <summary>
        /// Instantiation of GenericMasterCompletedEventHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public GenericMasterCompletedEventHandler(IConfiguration configuration, ILogger<GenericMasterCompletedEventHandler> logger, IPublishEventDataAccess publishEventDataAccess)
        {
            this._configuration = configuration;
            _logger = logger;
            _publishEventDataAccess = publishEventDataAccess;
        }

        /// <summary>
        /// Handler will recieve notification ,process it and will return the response. 
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Task.CompletedTask</returns>
        public async Task Handle(DomainEventNotification<GenericMasterCompletedEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            string eventStoreId = string.Empty;

            _logger.LogInformation("Domain Event: {DomainEvent}", domainEvent.GetType().Name);

            if (Convert.ToBoolean(this._configuration["AddEventDataForAuditLog"]))
            {
                eventStoreId = await AddEventDataForAuditLog(domainEvent);
            }
            //return System.Threading.Tasks.Task.CompletedTask;
        }

        //prepare event data for Audit Log
        private PublishEventData PrepareEventDataForAuditLog(GenericMasterCompletedEvent domainEvent)
        {
            //prepare event data for Audit Log
            var newObject = domainEvent.GenericMasterCompletedObject;
            var eventData = new PublishEventData { EventData = new List<Domain.Common.Property>() };
            eventData.AuditableSourceEventName = newObject.AuditableSourceEventName;
            eventData.OperationType = Domain.Common.OperationType.INSERT;
            eventData.CreatedDateTime = DateTime.UtcNow;
            eventData.OperationSource = OperationSource.WEBPAGE;
            eventData.ApiName = this._configuration["Api:internal_name"];
            eventData.CollectionName = "GenericMaster";

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

        //add event data for Audit Log
        private async Task<string> AddEventDataForAuditLog(GenericMasterCompletedEvent domainEvent)
        {
            try
            {
                //1. Prepare PublishEventData object for Audit Log
                var PublishEventData = PrepareEventDataForAuditLog(domainEvent);

                //2. Add PublishEventData to database
                var eventStoreId = await _publishEventDataAccess.Add(PublishEventData);

                return eventStoreId;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured while adding PublishEventData object to database - " + ex.Message);
                return null;
            }
        }
    }
}
