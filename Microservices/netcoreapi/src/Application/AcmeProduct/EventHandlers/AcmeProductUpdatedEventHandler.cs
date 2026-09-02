using System;
using System.Collections.Generic;
using System.Data;
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
    /// For Creating handler for the domain event notification,Updated AcmeProductUpdatedEventHandler 
    /// class that implements the INotificationHandler interface as shown below.
    /// </summary>
    public class AcmeProductUpdatedEventHandler : INotificationHandler<DomainEventNotification<AcmeProductUpdatedEvent>>
    {
        private readonly ILogger<AcmeProductUpdatedEventHandler> _logger;
        private IConfiguration _configuration;
        private readonly IPublishEventDataAccess _publishEventDataAccess;
        private readonly IMassTransitPublisher _massTransitPublisher;
        private readonly IUserDataAccess _userDataAccess;
        private readonly ICurrentUserService _currentUserService;
        private readonly NotificationHelper _notificationHelper;

        /// <summary>
        ///  Instantiation of AcmeProductUpdatedEventHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public AcmeProductUpdatedEventHandler(IConfiguration configuration, ILogger<AcmeProductUpdatedEventHandler> logger, IPublishEventDataAccess publishEventDataAccess, IUserDataAccess userDataAccess, IMassTransitPublisher massTransitPublisher, ICurrentUserService currentUserService, NotificationHelper notificationHelper)
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
        public async Task Handle(DomainEventNotification<AcmeProductUpdatedEvent> notification, CancellationToken cancellationToken)
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
        private async Task ProduceEvent(AcmeProductUpdatedEvent domainEvent)
        {
            //2.1 Mass Transit - MassTransit events, Kafka
            await ProduceMassTransitEvent(domainEvent);
        }

        /// <summary>
        /// ProduceMassTransitEvent
        /// </summary>
        /// <returns></returns>
        private async Task ProduceMassTransitEvent(AcmeProductUpdatedEvent domainEvent)
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
        private async Task PublishEventForNotification(AcmeProductUpdatedEvent domainEvent)
        {
            //Get notification data from event
            //var notification = await PrepareDataFor_notification(domainEvent);

            //if (notification.To != null && notification.To.Any())
            // This is common for all transport types
            //await _massTransitPublisher.PublishEventAsync(notification, "ZeptoMail");

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


        public NotificationData NotificationDataMapping(AcmeProductUpdatedEvent domainEvent)
        {
            return new NotificationData
            {
                EventType = nameof(AcmeProductUpdatedEvent),
                Subject = "Acme Product Updated!",
                ToEmails = new Dictionary<string, string>(),
                htmlTemplate = $""

                //Data = new Dictionary<string, object>
                //{
                //    ["ProductName"] = domainEvent.AcmeUpdatedObject.Name,
                //    ["ProductType"] = domainEvent.AcmeUpdatedObject.ProductType,
                //    ["Price"] = domainEvent.AcmeUpdatedObject.BasePrice,
                //    ["Vendor"] = domainEvent.AcmeUpdatedObject.VendorName
                //}
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
        /// Prepare the data for notification
        /// </summary>
        /// <param name="domainEvent"></param>
        /// <returns></returns>
        private async Task<Domain.Entities.ZeptoMail> PrepareDataFor_notification(AcmeProductUpdatedEvent domainEvent)
        {
            //_logger.LogInformation("AssignmentUpdatedEventHandler.PrepareDataFor_notification: Starting PrepareDataFor_notification method.");

            //domainEvent.AssignmentNewObject.AuditableSourceEventName = nameof(AssignmentCompletedEvent);
            //string assignmentName = domainEvent.AssignmentNewObject.Name;
            //string userName = _currentUserService.name ?? "Test User";
            //string updatedDateTime = DateTime.Now.ToString();
            //string assignmentlink = $"{this._configuration["TimetrackUrl"]}/assignment";

            //var newProject = await _projectDataAccess.GetProjectById(domainEvent.AssignmentNewObject.ProjectId);
            //var oldProject = await _projectDataAccess.GetProjectById(domainEvent.AssignmentOldObject.ProjectId);

            //List<ZeptoMailRecipient> assignmentEmailRecipients = new List<ZeptoMailRecipient>();

            //string newAssociatedUsersId = domainEvent.AssignmentNewObject.AssociatedUserId;
            //var newAssociatedUsers = JsonConvert.DeserializeObject<List<AssociatedUserDto>>(newAssociatedUsersId);

            //string oldAssociatedUsersId = domainEvent.AssignmentOldObject.AssociatedUserId;
            //var oldAssociatedUsers = JsonConvert.DeserializeObject<List<AssociatedUserDto>>(oldAssociatedUsersId);

            //var newUsers = newAssociatedUsers
            //    .Where(newUser => !oldAssociatedUsers.Any(oldUser => oldUser.Id == newUser.Id))
            //    .ToList();

            //_logger.LogInformation("AssignmentUpdatedEventHandler.PrepareDataFor_notification: Calculated new users to be notified: {NewUsers}", newUsers);

            //foreach (var associatedUserId in newUsers)
            //{
            //    var associatedUser = users.FirstOrDefault(u => u.Id == associatedUserId.Id);
            //    if (associatedUser != null)
            //    {
            //        assignmentEmailRecipients.Add(new ZeptoMailRecipient
            //        {
            //            email_address = new ZeptoMailAddress { address = associatedUser.Email, name = associatedUser.FullName }
            //        });
            //    }
            //    else
            //    {
            //        _logger.LogInformation("AssignmentUpdatedEventHandler.PrepareDataFor_notification: User with ID {UserId} not found in the user list.", associatedUserId.Id);
            //    }
            //}

            //var recipientEmails = String.Join(";", assignmentEmailRecipients.Select(b => b.email_address.address));

            //var filteredRecipients = string.Join(";", recipientEmails.Split(';').Except(blackList.Split(';')));
            //assignmentEmailRecipients.RemoveAll(recipient => !filteredRecipients.Contains(recipient.email_address.address));
            //_logger.LogInformation("AssignmentUpdatedEventHandler.PrepareDataFor_notification: Filtered recipients against blacklist.");

            //if (!assignmentEmailRecipients.Any())
            //{
            //    _logger.LogInformation("AssignmentUpdatedEventHandler.PrepareDataFor_notification: No recipients available after filtering. Notification will not be created.");
            //    return new Domain.Entities.ZeptoMail { };
            //}

            //var notification = new Domain.Entities.ZeptoMail
            //{
            //    Id = Guid.NewGuid().ToString(),
            //    ApiKey = _configuration["Api:ApiKey"],
            //    To = assignmentEmailRecipients,
            //    Cc = new List<ZeptoMailRecipient>
            //    {
            //        //new ZeptoMailRecipient
            //        //{
            //        //    email_address =
            //        //    {
            //        //        address = "sahil.malhotra@kmgus.com",
            //        //        name = "Sahil Malhotra"
            //        //    }
            //        //},

            //        //new ZeptoMailRecipient
            //        //{
            //        //    email_address =
            //        //    {
            //        //        address = "aayush.kapoor@kmgin.com",
            //        //        name = "Aayush Kapoor"
            //        //    }
            //        //},
            //        //new ZeptoMailRecipient
            //        //{
            //        //    email_address =
            //        //    {
            //        //        address = "leeladhar.kumawat@kmgin.co",
            //        //        name = "Leeladhar Kumawat"
            //        //    }
            //        //}
            //    },
            //    Subject = "Assignment Updated!",
            //    HtmlBody = $"<!DOCTYPE html>\r\n<html>\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n    <title>Assignment Update Notification</title>\r\n    <style>\r\n        body {{\r\n            font-family: Arial, sans-serif;\r\n            margin: 0;\r\n            padding: 0;\r\n            background-color: #f4f4f4;\r\n        }}\r\n        .container {{\r\n            max-width: 600px;\r\n            margin: 20px auto;\r\n            background: #ffffff;\r\n            border-radius: 8px;\r\n            overflow: hidden;\r\n            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);\r\n        }}\r\n        .header {{\r\n            background-color: #6CB4EE;\r\n            color: #ffffff;\r\n            text-align: center;\r\n            padding: 20px;\r\n        }}\r\n        .content {{\r\n            padding: 20px;\r\n            font-size: 16px;\r\n            line-height: 1.6;\r\n            color: #333333;\r\n        }}\r\n        .content a {{\r\n            color: #0000FF ;\r\n            text-decoration: none;\r\n        }}\r\n        .footer {{\r\n            background-color: #f4f4f4;\r\n            text-align: center;\r\n            padding: 10px;\r\n            font-size: 12px;\r\n            color: #666666;\r\n        }}\r\n    </style>\r\n</head>\r\n<body>\r\n    <div class=\"container\">\r\n        <div class=\"header\">\r\n            <h1>Assignment Update Notification</h1>\r\n        </div>\r\n        <div class=\"content\">\r\n            <p>Hello,</p>\r\n            <p>You have been added to a new assignment, or there has been an update to your assignment in <strong>{assignmentName}</strong>.</p>\r\n            <p>Details:</p>\r\n            <ul>\r\n                <li><strong>Task:</strong> <a href=\"{assignmentlink}\" target=\"_blank\">View Task Details</a></li>\r\n                <li><strong>Due Date:</strong>{domainEvent.AssignmentNewObject.DueDate}</li>\r\n            </ul>\r\n            <p>Please review the details at your earliest convenience.</p>\r\n            <p>Best regards,</p>\r\n            <p>Administrator</p>\r\n        </div>\r\n        <div class=\"footer\">\r\n            <p>This is an automated message. Please do not reply.</p>\r\n        </div>\r\n    </div>\r\n</body>\r\n</html>\r\n"
            //};

            //_logger.LogInformation("AssignmentUpdatedEventHandler.PrepareDataFor_notification: Notification object prepared with ID: {NotificationId}", notification.Id);
            var notification = new Domain.Entities.ZeptoMail();
            return notification;
        }

        /// <summary>
        /// Add event data for Audit Log 
        /// </summary>
        /// <param name="domainEvent"></param>
        private async void AddEventInEventStore_EventDB(AcmeProductUpdatedEvent domainEvent)
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
                _logger.LogError("Error occured while adding PublishEventData object to database - " + ex.Message);
            }
        }

        /// <summary>
        /// prepare event data for Audit Log
        /// </summary>
        /// <param name="domainEvent"></param>
        /// <returns></returns>
        private PublishEventData PrepareEventDataForAuditLog(AcmeProductUpdatedEvent domainEvent)
        {
            //prepare event data for Audit Log
            var newObject = domainEvent.AcmeNewObject;
            var oldObject = domainEvent.AcmeOldObject;
            var eventData = new PublishEventData { EventData = new List<Domain.Common.Property>() };
            eventData.OperationType = Domain.Common.OperationType.UPDATE;
            eventData.CreatedDateTime = DateTime.UtcNow;
            eventData.OperationSource = OperationSource.WEBPAGE;
            eventData.ApiName = this._configuration["Api:internal_name"];
            eventData.CollectionName = "AcmeProduct";

            #region Request Tracing
            eventData.CorrelationId = domainEvent.AcmeNewObject.CorrelationId;
            eventData.AuditableRequestId = domainEvent.AcmeNewObject.AuditableRequestId;
            eventData.AuditableRequestName = domainEvent.AcmeNewObject.AuditableRequestName;
            eventData.AuditableAssemblyQualifiedName = $"ApiDesign10_netcore_{Assembly.GetExecutingAssembly().GetName().Name}";
            eventData.AuditableSourceEventName = nameof(AcmeProductUpdatedEvent);
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

            //List of properties to always include, regardless of whether values are the same
            var requiredProperties = new HashSet<string>
                         {
                             "Id",
                             "CorrelationId",
                             "AuditableRequestId",
                             "AuditableRequestName",
                             "UpdatedBy",
                             "UpdatedDateTime"
                         };

            // Loop through all the members of the object and add them to eventData
            foreach (var item in filteredProperties)
            {
                // Get the value of the property from both newObject and oldObject
                var newValue = item.GetValue(newObject, null);
                var oldValue = item.GetValue(oldObject, null);

                // Check if the property should be included in eventData
                if (requiredProperties.Contains(item.Name) || !object.Equals(newValue, oldValue))
                {
                    eventData.EventData.Add(new Domain.Common.Property
                    {
                        PropertyName = item.Name,
                        NewValue = newValue,
                        OldValue = oldValue
                    });
                }
            }

            //returns eventData
            return eventData;
        }

    }
}
