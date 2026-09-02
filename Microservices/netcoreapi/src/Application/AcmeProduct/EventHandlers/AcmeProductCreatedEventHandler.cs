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

namespace Application.AcmeProduct.EventHandlers
{
    /// <summary>
    /// For Creating handler for the domain event notification,created AcmeProductCreatedEventHandler 
    /// class that implements the INotificationHandler interface as shown below.
    /// </summary>
    public class AcmeProductCreatedEventHandler : INotificationHandler<DomainEventNotification<AcmeProductCreatedEvent>>
    {
        private readonly ILogger<AcmeProductCreatedEventHandler> _logger;
        private IConfiguration _configuration;
        private readonly IPublishEventDataAccess _publishEventDataAccess;
        private readonly IMassTransitPublisher _massTransitPublisher;
        private readonly IUserDataAccess _userDataAccess;
        //public bool isNotificationRequired = true;
        private readonly ICurrentUserService _currentUserService;
        private readonly NotificationHelper _notificationHelper;

        /// <summary>
        /// Instantiation of AcmeProductCreatedEventHandler class
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        public AcmeProductCreatedEventHandler(IConfiguration configuration, ILogger<AcmeProductCreatedEventHandler> logger, IPublishEventDataAccess publishEventDataAccess, IMassTransitPublisher massTransitPublisher, IUserDataAccess userDataAccess, ICurrentUserService currentUserService, NotificationHelper notificationHelper)
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
        public async Task Handle(DomainEventNotification<AcmeProductCreatedEvent> notification, CancellationToken cancellationToken)
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
        private async Task ProduceEvent(AcmeProductCreatedEvent domainEvent)
        {
            //2.1 Mass Transit - MassTransit events, Kafka
            await ProduceMassTransitEvent(domainEvent);
        }

        /// <summary>
        /// ProduceMassTransitEvent
        /// </summary>
        /// <returns></returns>
        private async Task ProduceMassTransitEvent(AcmeProductCreatedEvent domainEvent)
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
        private async Task PublishEventForNotification(AcmeProductCreatedEvent domainEvent)
        {
            ////Get notification data from event
            //var notification = await PrepareDataFor_notification(domainEvent);

            //if (notification.To != null && notification.To.Any())
            //    // This is common for all transport types
            //    await _massTransitPublisher.PublishEventAsync(notification, "ZeptoMail");

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

        /// <summary>
        /// Prepare the data for notification
        /// </summary>
        /// <param name="domainEvent"></param>
        /// <returns></returns>
        private async Task<Domain.Entities.ZeptoMail> PrepareDataFor_notification(AcmeProductCreatedEvent domainEvent)
        {
            _logger.LogInformation("AcmeProductCreatedEventHandler.PrepareDataFor_notification: Starting PrepareDataFor_notification method.");

            domainEvent.AcmeCreatedObject.AuditableSourceEventName = nameof(AcmeProductCreatedEvent);
            string userName = _currentUserService.display_name ?? "Test User";
            string updatedDateTime = DateTime.Now.ToString();

            List<ZeptoMailRecipient> emailRecipients = new List<ZeptoMailRecipient>();

            //_logger.LogInformation("AcmeProductCreatedEventHandler.PrepareDataFor_notification: Calculated new users to be notified: {NewUsers}", newUsers);

            emailRecipients.Add(new ZeptoMailRecipient
            {
                email_address = new ZeptoMailAddress { address = _currentUserService.Email, name = _currentUserService.name }
            });


            _logger.LogInformation("AcmeProductCreatedEventHandler.PrepareDataFor_notification: Filtered recipients against blacklist.");

            if (!emailRecipients.Any())
            {
                _logger.LogInformation("AcmeProductCreatedEventHandler.PrepareDataFor_notification: No recipients available after filtering. Notification will not be created.");
                return new Domain.Entities.ZeptoMail { };
            }

            var notification = new Domain.Entities.ZeptoMail
            {
                Id = Guid.NewGuid().ToString(),
                ApiKey = _configuration["Api:api-key"],
                To = emailRecipients,
                Cc = new List<ZeptoMailRecipient>
                {
                    //new ZeptoMailRecipient
                    //{
                    //    email_address =
                    //    {
                    //        address = "sahil.malhotra@kmgus.com",
                    //        name = "Sahil Malhotra"
                    //    }
                    //},

                    //new ZeptoMailRecipient
                    //{
                    //    email_address =
                    //    {
                    //        address = "aayush.kapoor@kmgin.com",
                    //        name = "Aayush Kapoor"
                    //    }
                    //},
                    //new ZeptoMailRecipient
                    //{
                    //    email_address =
                    //    {
                    //        address = "leeladhar.kumawat@kmgin.co",
                    //        name = "Leeladhar Kumawat"
                    //    }
                    //}
                },
                Subject = "Acme Product Created!",
                HtmlBody = $"<!DOCTYPE html>\r\n\r\n<html>\r\n<head>\r\n  <meta charset=\"UTF-8\">\r\n  <title>Product Created</title>\r\n</head>\r\n<body style=\"margin:0; padding:0; font-family: Arial, sans-serif; background-color:#f4f6f8;\">\r\n\r\n  <table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" style=\"background-color:#f4f6f8; padding:20px;\">\r\n    <tr>\r\n      <td align=\"center\">\r\n\r\n    <table width=\"600\" cellpadding=\"0\" cellspacing=\"0\" style=\"background:#ffffff; border-radius:8px; overflow:hidden;\">\r\n      \r\n      <!-- Header -->\r\n      <tr>\r\n        <td style=\"background:#4CAF50; color:#ffffff; padding:20px; text-align:center;\">\r\n          <h2 style=\"margin:0;\">Product Created\r\n      </tr>\r\n\r\n      <!-- Body -->\r\n      <tr>\r\n        <td style=\"padding:30px; color:#333;\">\r\n          <h3 style=\"margin-top:0;\">🎉 Product Created Successfully</h3>\r\n          \r\n          <p>Hello <strong>{userName}</strong>,</p>\r\n          \r\n          <p>Your product has been successfully created in the system. Here are the details:</p>\r\n\r\n          <table width=\"100%\" cellpadding=\"8\" cellspacing=\"0\" style=\"margin:20px 0; border-collapse:collapse;\">\r\n            <tr style=\"background:#f1f1f1;\">\r\n              <td><strong>Product Name</strong></td>\r\n              <td>{domainEvent.AcmeCreatedObject.Name}</td>\r\n            </tr>\r\n            <tr>\r\n              <td><strong>Product Type</strong></td>\r\n              <td>{domainEvent.AcmeCreatedObject.ProductType}</td>\r\n            </tr>\r\n            <tr style=\"background:#f1f1f1;\">\r\n              <td><strong>Base Price</strong></td>\r\n              <td>{domainEvent.AcmeCreatedObject.BasePrice}</td>\r\n            </tr>\r\n            <tr>\r\n              <td><strong>Vendor Name</strong></td>\r\n              <td><strong>{domainEvent.AcmeCreatedObject.VendorName}</strong></td>\r\n            </tr>\r\n          </table>\r\n\r\n         \r\n          <p>If you have any questions, feel free to reach out to our support team.</p>\r\n\r\n          <p>Regards,<br><strong>System Design Team</strong></p>\r\n        </td>\r\n      </tr>\r\n\r\n      <!-- Footer -->\r\n      <tr>\r\n        <td style=\"background:#f1f1f1; text-align:center; padding:15px; font-size:12px; color:#777;\">\r\n          © 2026 KMG Inc. All rights reserved.\r\n        </td>\r\n      </tr>\r\n\r\n    </table>\r\n\r\n  </td>\r\n</tr>\r\n```\r\n\r\n  </table>\r\n\r\n</body>\r\n</html>\r\n"
            };

            //_logger.LogInformation("AcmeProductCreatedEventHandler.PrepareDataFor_notification: Notification object prepared with ID: {NotificationId}", notification.Id);
            //var notification = new Domain.Entities.ZeptoMail();
            return notification;
        }

        /// <summary>
        /// Add event data for Audit Log 
        /// </summary>
        /// <param name="domainEvent"></param>
        private async void AddEventInEventStore_EventDB(AcmeProductCreatedEvent domainEvent)
        {
            try
            {
                //1. Prepare PublishEventData object for Audit Log
                var PublishEventData = PrepareEventDataForAuditLog(domainEvent);

                //2. Add PublishEventData to database
                var eventStoreId = await _publishEventDataAccess.Add(PublishEventData);

                //return eventStoreId;
            }
            catch (Exception ex)
            {
                // Do NOT rethrow in async void — it escapes to the thread pool unhandled
                // exception handler and terminates the process.
                _logger.LogError("Error occured while adding PublishEventData object to database - " + ex.Message);
            }
        }

        /// <summary>
        /// Prepare event data for Audit Log
        /// </summary>
        /// <param name="domainEvent"></param>
        /// <returns></returns>
        private PublishEventData PrepareEventDataForAuditLog(AcmeProductCreatedEvent domainEvent)
        {
            //prepare event data for Audit Log
            var newObject = domainEvent.AcmeCreatedObject;
            var eventData = new PublishEventData { EventData = new List<Domain.Common.Property>() };
            eventData.OperationType = Domain.Common.OperationType.INSERT;
            eventData.CreatedDateTime = DateTime.UtcNow;
            eventData.OperationSource = OperationSource.WEBPAGE;
            eventData.ApiName = this._configuration["Api:internal_name"];
            eventData.CollectionName = "AcmeProduct";

            #region Request Tracing
            eventData.CorrelationId = domainEvent.AcmeCreatedObject.CorrelationId;
            eventData.AuditableRequestId = domainEvent.AcmeCreatedObject.AuditableRequestId;
            eventData.AuditableRequestName = domainEvent.AcmeCreatedObject.AuditableRequestName;
            eventData.AuditableAssemblyQualifiedName = $"ApiDesign10_netcore_{Assembly.GetExecutingAssembly().GetName().Name}";
            eventData.AuditableSourceEventName = nameof(AcmeProductCreatedEvent);
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

        public async Task<NotificationPayload> PreparePayloadForNotification(NotificationData data)
        {
            var provider = Enum.Parse<NotificationProvider>(
                _configuration["NotificationSettings:Provider"]);

            return _notificationHelper.BuildNotification(provider, data);
        }

        public NotificationData NotificationDataMapping(AcmeProductCreatedEvent domainEvent)
        {
            return new NotificationData
            {
                EventType = nameof(AcmeProductCreatedEvent),
                Subject = "Acme Product Created!",
                ToEmails = new Dictionary<string, string>
                {
                    [_currentUserService.display_name ?? "Test user"] = _currentUserService.Email
                },
                htmlTemplate = $"<!DOCTYPE html>\r\n\r\n<html>\r\n<head>\r\n  <meta charset=\"UTF-8\">\r\n  <title>Product Created</title>\r\n</head>\r\n<body style=\"margin:0; padding:0; font-family: Arial, sans-serif; background-color:#f4f6f8;\">\r\n\r\n  <table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" style=\"background-color:#f4f6f8; padding:20px;\">\r\n    <tr>\r\n      <td align=\"center\">\r\n\r\n    <table width=\"600\" cellpadding=\"0\" cellspacing=\"0\" style=\"background:#ffffff; border-radius:8px; overflow:hidden;\">\r\n      \r\n      <!-- Header -->\r\n      <tr>\r\n        <td style=\"background:#4CAF50; color:#ffffff; padding:20px; text-align:center;\">\r\n          <h2 style=\"margin:0;\">Product Created\r\n      </tr>\r\n\r\n      <!-- Body -->\r\n      <tr>\r\n        <td style=\"padding:30px; color:#333;\">\r\n          <h3 style=\"margin-top:0;\">🎉 Product Created Successfully</h3>\r\n          \r\n          <p>Hello <strong>{_currentUserService.display_name}</strong>,</p>\r\n          \r\n          <p>Your product has been successfully created in the system. Here are the details:</p>\r\n\r\n          <table width=\"100%\" cellpadding=\"8\" cellspacing=\"0\" style=\"margin:20px 0; border-collapse:collapse;\">\r\n            <tr style=\"background:#f1f1f1;\">\r\n              <td><strong>Product Name</strong></td>\r\n              <td>{domainEvent.AcmeCreatedObject.Name}</td>\r\n            </tr>\r\n            <tr>\r\n              <td><strong>Product Type</strong></td>\r\n              <td>{domainEvent.AcmeCreatedObject.ProductType}</td>\r\n            </tr>\r\n            <tr style=\"background:#f1f1f1;\">\r\n              <td><strong>Base Price</strong></td>\r\n              <td>{domainEvent.AcmeCreatedObject.BasePrice}</td>\r\n            </tr>\r\n            <tr>\r\n              <td><strong>Vendor Name</strong></td>\r\n              <td><strong>{domainEvent.AcmeCreatedObject.VendorName}</strong></td>\r\n            </tr>\r\n          </table>\r\n\r\n         \r\n          <p>If you have any questions, feel free to reach out to our support team.</p>\r\n\r\n          <p>Regards,<br><strong>System Design Team</strong></p>\r\n        </td>\r\n      </tr>\r\n\r\n      <!-- Footer -->\r\n      <tr>\r\n        <td style=\"background:#f1f1f1; text-align:center; padding:15px; font-size:12px; color:#777;\">\r\n          © 2026 KMG Inc. All rights reserved.\r\n        </td>\r\n      </tr>\r\n\r\n    </table>\r\n\r\n  </td>\r\n</tr>\r\n```\r\n\r\n  </table>\r\n\r\n</body>\r\n</html>\r\n"

                //Data = new Dictionary<string, object>
                //{
                //    ["ProductName"] = domainEvent.AcmeCreatedObject.Name,
                //    ["ProductType"] = domainEvent.AcmeCreatedObject.ProductType,
                //    ["Price"] = domainEvent.AcmeCreatedObject.BasePrice,
                //    ["Vendor"] = domainEvent.AcmeCreatedObject.VendorName
                //}
            };
        }

    }
}
