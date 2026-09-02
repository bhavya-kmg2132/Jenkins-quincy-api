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
    /// For Creating handler for the domain event notification,created PolicyCreatedEventHandler
    /// class that implements the INotificationHandler interface as shown below.
    /// </summary>
    public class PolicyCreatedEventHandler : INotificationHandler<DomainEventNotification<PolicyCreatedEvent>>
    {
        private readonly ILogger<PolicyCreatedEventHandler> _logger;
        private IConfiguration _configuration;
        private readonly IPublishEventDataAccess _publishEventDataAccess;
        private readonly IMassTransitPublisher _massTransitPublisher;
        private readonly IUserDataAccess _userDataAccess;
        private readonly ICurrentUserService _currentUserService;
        private readonly NotificationHelper _notificationHelper;

        /// <summary>
        /// Instantiation of PolicyCreatedEventHandler class
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        public PolicyCreatedEventHandler(IConfiguration configuration, ILogger<PolicyCreatedEventHandler> logger, IPublishEventDataAccess publishEventDataAccess, IMassTransitPublisher massTransitPublisher, IUserDataAccess userDataAccess, ICurrentUserService currentUserService, NotificationHelper notificationHelper)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._publishEventDataAccess = publishEventDataAccess;
            this._userDataAccess = userDataAccess;
            this._massTransitPublisher = massTransitPublisher;
            this._currentUserService = currentUserService;
            this._notificationHelper = notificationHelper;
        }

        /// <summary>
        /// Handler will recieve notification ,process it and will return the response.
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Task.CompletedTask</returns>
        public async Task Handle(DomainEventNotification<PolicyCreatedEvent> notification, CancellationToken cancellationToken)
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
        private async Task ProduceEvent(PolicyCreatedEvent domainEvent)
        {
            //2.1 Mass Transit - MassTransit events, Kafka
            await ProduceMassTransitEvent(domainEvent);
        }

        /// <summary>
        /// ProduceMassTransitEvent
        /// </summary>
        /// <returns></returns>
        private async Task ProduceMassTransitEvent(PolicyCreatedEvent domainEvent)
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
        private async Task PublishEventForNotification(PolicyCreatedEvent domainEvent)
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

        public NotificationData NotificationDataMapping(PolicyCreatedEvent domainEvent)
        {
            var policy = domainEvent.PolicyCreatedObject;
            return new NotificationData
            {
                EventType = nameof(PolicyCreatedEvent),
                Subject = $"Policy Created: {policy.PolicyNumber} - {policy.PolicyName}",
                ToEmails = new Dictionary<string, string>
                {
                    [_currentUserService.display_name ?? "Test user"] = _currentUserService.Email
                },
                htmlTemplate = $"<!DOCTYPE html>\r\n<html>\r\n<head>\r\n  <meta charset=\"UTF-8\">\r\n  <title>Policy Created</title>\r\n</head>\r\n<body style=\"margin:0;padding:0;font-family:Arial,sans-serif;background-color:#f4f6f8;\">\r\n<table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" style=\"background-color:#f4f6f8;padding:20px;\">\r\n  <tr><td align=\"center\">\r\n    <table width=\"600\" cellpadding=\"0\" cellspacing=\"0\" style=\"background:#ffffff;border-radius:8px;overflow:hidden;\">\r\n      <tr><td style=\"background:#1565C0;color:#ffffff;padding:20px;text-align:center;\">\r\n        <h2 style=\"margin:0;\">Policy Created</h2>\r\n      </td></tr>\r\n      <tr><td style=\"padding:30px;color:#333;\">\r\n        <h3 style=\"margin-top:0;\">Policy Successfully Created</h3>\r\n        <p>Hello <strong>{_currentUserService.display_name}</strong>,</p>\r\n        <p>A new policy has been created in the system. Here are the details:</p>\r\n        <table width=\"100%\" cellpadding=\"8\" cellspacing=\"0\" style=\"margin:20px 0;border-collapse:collapse;\">\r\n          <tr style=\"background:#f1f1f1;\"><td><strong>Policy Number</strong></td><td>{policy.PolicyNumber}</td></tr>\r\n          <tr><td><strong>Policy Name</strong></td><td>{policy.PolicyName}</td></tr>\r\n          <tr style=\"background:#f1f1f1;\"><td><strong>Policy Type</strong></td><td>{policy.PolicyType}</td></tr>\r\n          <tr><td><strong>Transaction Type</strong></td><td>{policy.TransactionType}</td></tr>\r\n          <tr style=\"background:#f1f1f1;\"><td><strong>Status</strong></td><td>{policy.StatusCode}</td></tr>\r\n          <tr><td><strong>Insured Name</strong></td><td>{policy.InsuredName}</td></tr>\r\n          <tr style=\"background:#f1f1f1;\"><td><strong>Effective Date</strong></td><td>{policy.EffectiveDate:yyyy-MM-dd}</td></tr>\r\n          <tr><td><strong>Expiration Date</strong></td><td>{policy.ExpirationDate:yyyy-MM-dd}</td></tr>\r\n          <tr style=\"background:#f1f1f1;\"><td><strong>Total Premium</strong></td><td>{policy.TotalPremium} {policy.Currency}</td></tr>\r\n          <tr><td><strong>Producer</strong></td><td>{policy.ProducerName}</td></tr>\r\n          <tr style=\"background:#f1f1f1;\"><td><strong>Underwriter</strong></td><td>{policy.UnderwriterName}</td></tr>\r\n        </table>\r\n        <p>If you have any questions, please contact the underwriting team.</p>\r\n        <p>Regards,<br><strong>Quincy Policy System</strong></p>\r\n      </td></tr>\r\n      <tr><td style=\"background:#f1f1f1;text-align:center;padding:15px;font-size:12px;color:#777;\">\r\n        &copy; 2026 KMG Inc. All rights reserved.\r\n      </td></tr>\r\n    </table>\r\n  </td></tr>\r\n</table>\r\n</body>\r\n</html>"
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
        private async void AddEventInEventStore_EventDB(PolicyCreatedEvent domainEvent)
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
        private PublishEventData PrepareEventDataForAuditLog(PolicyCreatedEvent domainEvent)
        {
            //prepare event data for Audit Log
            var newObject = domainEvent.PolicyCreatedObject;
            var eventData = new PublishEventData { EventData = new List<Domain.Common.Property>() };
            eventData.OperationType = Domain.Common.OperationType.INSERT;
            eventData.CreatedDateTime = DateTime.UtcNow;
            eventData.OperationSource = OperationSource.WEBPAGE;
            eventData.ApiName = this._configuration["Api:internal_name"];
            eventData.CollectionName = "Policy";

            #region Request Tracing
            eventData.CorrelationId = domainEvent.PolicyCreatedObject.CorrelationId;
            eventData.AuditableRequestId = domainEvent.PolicyCreatedObject.AuditableRequestId;
            eventData.AuditableRequestName = domainEvent.PolicyCreatedObject.AuditableRequestName;
            eventData.AuditableAssemblyQualifiedName = $"ApiDesign10_netcore_{Assembly.GetExecutingAssembly().GetName().Name}";
            eventData.AuditableSourceEventName = nameof(PolicyCreatedEvent);
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
