using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities;
using Domain.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Policy.EventHandlers
{
    /// <summary>
    /// For Creating handler for the domain event notification,created PolicyCompletedEventHandler
    /// class that implements the INotificationHandler interface as shown below.
    /// </summary>
    public class PolicyCompletedEventHandler : INotificationHandler<DomainEventNotification<PolicyCompletedEvent>>
    {
        private readonly ILogger<PolicyCompletedEventHandler> _logger;
        private IConfiguration _configuration;
        private readonly IPublishEventDataAccess _publishEventDataAccess;
        private readonly IMassTransitPublisher _massTransitPublisher;
        private readonly IUserDataAccess _userDataAccess;
        private readonly ICurrentUserService _currentUserService;
        private readonly NotificationHelper _notificationHelper;

        /// <summary>
        /// Instantiation of PolicyCompletedEventHandler class
        /// </summary>
        /// <param name="logger"></param>
        public PolicyCompletedEventHandler(IConfiguration configuration, ILogger<PolicyCompletedEventHandler> logger, IPublishEventDataAccess publishEventDataAccess, IMassTransitPublisher massTransitPublisher, IUserDataAccess userDataAccess, ICurrentUserService currentUserService, NotificationHelper notificationHelper)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._publishEventDataAccess = publishEventDataAccess;
            this._massTransitPublisher = massTransitPublisher;
            this._userDataAccess = userDataAccess;
            this._currentUserService = currentUserService;
            this._notificationHelper = notificationHelper;
        }

        /// <summary>
        /// Handler will recieve notification ,process it and will return the response.
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Task.CompletedTask</returns>
        public async Task Handle(DomainEventNotification<PolicyCompletedEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            string eventStoreId = string.Empty;

            _logger.LogInformation("Domain Event: {DomainEvent}", domainEvent.GetType().Name);

            //Step 1: Event Store
            if (Convert.ToBoolean(this._configuration["EventConfiguration:AddEventInEventStore_EventDB"]))
            {
                AddEventInEventStore_EventDB(domainEvent);
            }

            //Step 2: Produce Event
            if (Convert.ToBoolean(this._configuration["EventConfiguration:AllowProduceEvent"]))
            {
                await ProduceEvent(domainEvent);
            }
        }

        /// <summary>
        /// Produce Event
        /// </summary>
        /// <param name="domainEvent"></param>
        /// <returns></returns>
        private async Task ProduceEvent(PolicyCompletedEvent domainEvent)
        {
            //2.1 Mass Transit - MassTransit events, Kafka
            await ProduceMassTransitEvent(domainEvent);
        }

        /// <summary>
        /// ProduceMassTransitEvent
        /// </summary>
        /// <returns></returns>
        private async Task ProduceMassTransitEvent(PolicyCompletedEvent domainEvent)
        {
            //Update the key to "true", when needed.
            if (Convert.ToBoolean(this._configuration["ZeptoMailConfigSettings:IsNotifictionRequired"]))
                await PublishEventForNotification(domainEvent);
        }

        /// <summary>
        /// Publish event for notification using Mass Transit
        /// </summary>
        /// <param name="domainEvent"></param>
        /// <returns></returns>
        private async Task PublishEventForNotification(PolicyCompletedEvent domainEvent)
        {
            var data = NotificationDataMapping(domainEvent);
            var notification = await PreparePayloadForNotification(data);

            if (notification is ZeptoMail zepto && zepto.To?.Any() == true)
            {
                await _massTransitPublisher.PublishEventAsync(zepto, "ZeptoMail");
            }

            else if (notification is PostgreNotification graph && graph.EmailTo?.Any() == true)
            {
                await _massTransitPublisher.PublishEventAsync(graph, "MSGraph");
            }
        }

        public NotificationData NotificationDataMapping(PolicyCompletedEvent domainEvent)
        {
            var policy = domainEvent.PolicyCompletedObject;
            return new NotificationData
            {
                EventType = nameof(PolicyCompletedEvent),
                Subject = $"Policy Completed: {policy.PolicyNumber} - {policy.PolicyName}",
                ToEmails = new Dictionary<string, string>
                {
                    [_currentUserService.display_name ?? "Test user"] = _currentUserService.Email
                },
                htmlTemplate = $"<!DOCTYPE html>\r\n<html>\r\n<head>\r\n  <meta charset=\"UTF-8\">\r\n  <title>Policy Completed</title>\r\n</head>\r\n<body style=\"margin:0;padding:0;font-family:Arial,sans-serif;background-color:#f4f6f8;\">\r\n<table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" style=\"background-color:#f4f6f8;padding:20px;\">\r\n  <tr><td align=\"center\">\r\n    <table width=\"600\" cellpadding=\"0\" cellspacing=\"0\" style=\"background:#ffffff;border-radius:8px;overflow:hidden;\">\r\n      <tr><td style=\"background:#2E7D32;color:#ffffff;padding:20px;text-align:center;\">\r\n        <h2 style=\"margin:0;\">Policy Completed</h2>\r\n      </td></tr>\r\n      <tr><td style=\"padding:30px;color:#333;\">\r\n        <h3 style=\"margin-top:0;\">Policy Successfully Completed</h3>\r\n        <p>Hello <strong>{_currentUserService.display_name}</strong>,</p>\r\n        <p>The following policy has been marked as completed:</p>\r\n        <table width=\"100%\" cellpadding=\"8\" cellspacing=\"0\" style=\"margin:20px 0;border-collapse:collapse;\">\r\n          <tr style=\"background:#f1f1f1;\"><td><strong>Policy Number</strong></td><td>{policy.PolicyNumber}</td></tr>\r\n          <tr><td><strong>Policy Name</strong></td><td>{policy.PolicyName}</td></tr>\r\n          <tr style=\"background:#f1f1f1;\"><td><strong>Policy Type</strong></td><td>{policy.PolicyType}</td></tr>\r\n          <tr><td><strong>Insured Name</strong></td><td>{policy.InsuredName}</td></tr>\r\n          <tr style=\"background:#f1f1f1;\"><td><strong>Effective Date</strong></td><td>{policy.EffectiveDate:yyyy-MM-dd}</td></tr>\r\n          <tr><td><strong>Expiration Date</strong></td><td>{policy.ExpirationDate:yyyy-MM-dd}</td></tr>\r\n          <tr style=\"background:#f1f1f1;\"><td><strong>Total Premium</strong></td><td>{policy.TotalPremium} {policy.Currency}</td></tr>\r\n          <tr><td><strong>Sum Insured</strong></td><td>{policy.SumInsured} {policy.Currency}</td></tr>\r\n          <tr style=\"background:#f1f1f1;\"><td><strong>Underwriter</strong></td><td>{policy.UnderwriterName}</td></tr>\r\n          <tr><td><strong>Producer</strong></td><td>{policy.ProducerName}</td></tr>\r\n        </table>\r\n        <p>No further action is required for this policy.</p>\r\n        <p>Regards,<br><strong>Quincy Policy System</strong></p>\r\n      </td></tr>\r\n      <tr><td style=\"background:#f1f1f1;text-align:center;padding:15px;font-size:12px;color:#777;\">\r\n        &copy; 2026 KMG Inc. All rights reserved.\r\n      </td></tr>\r\n    </table>\r\n  </td></tr>\r\n</table>\r\n</body>\r\n</html>"
            };
        }

        /// <summary>
        /// Prepare the payload for notification based on provider
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<NotificationPayload> PreparePayloadForNotification(NotificationData data)
        {
            var provider = Enum.Parse<NotificationProvider>(
                _configuration["NotificationSettings:Provider"]);

            return _notificationHelper.BuildNotification(provider, data);
        }

        /// <summary>
        /// Add event data for Audit Log
        /// </summary>
        /// <param name="domainEvent"></param>
        private async void AddEventInEventStore_EventDB(PolicyCompletedEvent domainEvent)
        {
            try
            {
                //1. Prepare PublishEventData object for Audit Log
                var PublishEventData = PrepareEventDataForAuditLog(domainEvent);

                //2. Add PublishEventData to database
                var eventStoreId = await _publishEventDataAccess.Add(PublishEventData);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured while adding PublishEventData object to database - " + ex.Message);
            }
        }

        /// <summary>
        /// Prepare event data for Audit Log
        /// </summary>
        /// <param name="domainEvent"></param>
        /// <returns></returns>
        private PublishEventData PrepareEventDataForAuditLog(PolicyCompletedEvent domainEvent)
        {
            //prepare event data for Audit Log
            var newObject = domainEvent.PolicyCompletedObject;
            var eventData = new PublishEventData { EventData = new List<Domain.Common.Property>() };
            eventData.OperationType = Domain.Common.OperationType.COMPLETE;
            eventData.CreatedDateTime = DateTime.UtcNow;
            eventData.OperationSource = OperationSource.WEBPAGE;
            eventData.ApiName = this._configuration["Api:internal_name"];
            eventData.CollectionName = "Policy";

            #region Request Tracing
            eventData.CorrelationId = domainEvent.PolicyCompletedObject.CorrelationId;
            eventData.AuditableRequestId = domainEvent.PolicyCompletedObject.AuditableRequestId;
            eventData.AuditableRequestName = domainEvent.PolicyCompletedObject.AuditableRequestName;
            eventData.AuditableAssemblyQualifiedName = $"ApiDesign10_netcore_{Assembly.GetExecutingAssembly().GetName().Name}";
            eventData.AuditableSourceEventName = nameof(PolicyCompletedEvent);
            #endregion

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
