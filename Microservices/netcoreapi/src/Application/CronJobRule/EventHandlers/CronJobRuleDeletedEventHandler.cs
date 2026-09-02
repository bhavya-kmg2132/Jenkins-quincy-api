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

namespace Application.CronJobRule.EventHandlers
{
    /// <summary>
    /// For Creating handler for the domain event notification,Deleted CronJobRuleDeletedEventHandler 
    /// class that implements the INotificationHandler interface as shown below.
    /// </summary>
    public class CronJobRuleDeletedEventHandler : INotificationHandler<DomainEventNotification<CronJobRuleDeletedEvent>>
    {
        private readonly ILogger<CronJobRuleDeletedEventHandler> _logger;
        private IConfiguration _configuration;
        private readonly IPublishEventDataAccess _publishEventDataAccess;
        private readonly IMassTransitPublisher _massTransitPublisher;
        private readonly IUserDataAccess _userDataAccess;


        /// <summary>
        ///  Instantiation of CronJobRuleDeletedEventHandler class
        /// </summary>
        /// <param name="logger"></param>
        public CronJobRuleDeletedEventHandler(IConfiguration configuration, ILogger<CronJobRuleDeletedEventHandler> logger, IPublishEventDataAccess publishEventDataAccess, IMassTransitPublisher massTransitPublisher, IUserDataAccess userDataAccess)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._publishEventDataAccess = publishEventDataAccess;
            this._userDataAccess = userDataAccess;
            this._massTransitPublisher = massTransitPublisher;
        }

        /// <summary>
        /// Handler will recieve notification ,process it and will return the response. 
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Task.CompletedTask</returns>
        public async Task Handle(DomainEventNotification<CronJobRuleDeletedEvent> notification, CancellationToken cancellationToken)
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
        private async Task ProduceEvent(CronJobRuleDeletedEvent domainEvent)
        {
            //2.1 Mass Transit - MassTransit events, Kafka
            await ProduceMassTransitEvent(domainEvent);
        }

        /// <summary>
        /// ProduceMassTransitEvent
        /// </summary>
        /// <returns></returns>
        private async Task ProduceMassTransitEvent(CronJobRuleDeletedEvent domainEvent)
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
        private async Task PublishEventForNotification(CronJobRuleDeletedEvent domainEvent)
        {
            //Get notification data from event
            var notification = await PrepareDataFor_notification(domainEvent);

            if (notification.To != null && notification.To.Any())
                // This is common for all transport types
                await _massTransitPublisher.PublishEventAsync(notification, "ZeptoMail");
        }

        /// <summary>
        /// Prepare the data for notification
        /// </summary>
        /// <param name="domainEvent"></param>
        /// <returns></returns>
        private async Task<Domain.Entities.ZeptoMail> PrepareDataFor_notification(CronJobRuleDeletedEvent domainEvent)
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
        private async void AddEventInEventStore_EventDB(CronJobRuleDeletedEvent domainEvent)
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
        /// Prepare event data for Audit Log
        /// </summary>
        /// <param name="domainEvent"></param>
        /// <returns></returns>
        private PublishEventData PrepareEventDataForAuditLog(CronJobRuleDeletedEvent domainEvent)
        {
            //prepare event data for Audit Log
            var newObject = domainEvent.CronJobRuleDeletedObject;
            var eventData = new PublishEventData { EventData = new List<Domain.Common.Property>() };
            eventData.AuditableSourceEventName = newObject.AuditableSourceEventName;
            eventData.OperationType = Domain.Common.OperationType.DELETE;
            eventData.CreatedDateTime = DateTime.UtcNow;
            eventData.OperationSource = OperationSource.WEBPAGE;
            eventData.ApiName = this._configuration["Api:internal_name"];
            eventData.CollectionName = "CronJobRule";

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
