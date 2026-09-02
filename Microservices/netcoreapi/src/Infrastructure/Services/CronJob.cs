using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Logging;

public class CronJob
{
    private readonly ILogger<CronJob> _logger;
    private readonly IUserDataAccess _userDataAccess;
    private readonly IPostgreBulkInsertion _postgreBulkInsertion;
    private readonly IMassTransitPublisher _massTransitPublisher;

    public CronJob(ILogger<CronJob> logger, IUserDataAccess userDataAccess, IPostgreBulkInsertion postgreBulkInsertion, IMassTransitPublisher massTransitPublisher)
    {
        _logger = logger;
        _userDataAccess = userDataAccess;
        _postgreBulkInsertion = postgreBulkInsertion;
        _massTransitPublisher = massTransitPublisher;
    }

    public async Task RunCronJob(string notificationRuleId, string notificationName, bool isNotificationPaused)
    {
        _logger.LogInformation($"Executing notification rule {notificationRuleId}");

        if (!isNotificationPaused)
        {
            #region Used in Timetrack
            // Fetching users and notification type-specific blacklist 
            //var users = await _userDataAccess.GetUserFullName();
            //var blacklist = await _timetrackCronService.GetCronUsersBlackList(notificationRuleId, users);
            //await _timetrackCronService.ExecuteCrons(DateTime.UtcNow, notificationName, blacklist);
            #endregion

            try
            {
                await ExecuteCronJobTasks();
            }
            catch
            {
                _logger.LogError($"CronJob.RunCronJob: CronJob failed for {notificationName}.");
            }

        }
    }

    public async Task ExecuteCronJobTasks()
    {
        //await AddNotificationToDB();
        await PublishEventForNotification();
    }

    private async Task<ZeptoMail> AddNotificationToDB()
    {
        var notifications = new List<ZeptoMail>();
        var notification = new Domain.Entities.ZeptoMail();

        notification.Id = Guid.NewGuid().ToString();
        notification.ApiKey = "68a8a0a5-f22f-4ef8-ae1c-81fc23f40faf";
        notification.To = new List<ZeptoMailRecipient>
        {
            new ZeptoMailRecipient
            {
                email_address = new ZeptoMailAddress
                {
                    address = "vibhuti@kmgus.com",
                    name = "Vibhuti Thapliyal"
                }
            },
            new ZeptoMailRecipient
            {
                email_address = new ZeptoMailAddress
                {
                    address = "aayush.kapoor@kmgin.com",
                    name = "Aayush Kapoor"
                }
            },

            new ZeptoMailRecipient
            {
                email_address = new ZeptoMailAddress
                {
                    address = "manish.bisht@kmgus.com",
                    name = "Manish Bisht"
                }
            }
        };

        if (notification.To.Count == 0)
        {
            _logger.LogInformation($".No user to notify.");
            return new ZeptoMail();
        }

        notification.Cc = new List<ZeptoMailRecipient>

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
        };
        notification.Subject = $"Yearly Cron Job For ApiDesign10!";
        notification.HtmlBody = $"<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <title>System Test Notification</title>\r\n    <style>\r\n        body {{\r\n            font-family: Arial, Helvetica, sans-serif;\r\n            background-color: #f5f7fa;\r\n            margin: 0;\r\n            padding: 0;\r\n        }}\r\n        .container {{\r\n            max-width: 600px;\r\n            margin: 40px auto;\r\n            background-color: #ffffff;\r\n            border: 1px solid #e5e7eb;\r\n            border-radius: 6px;\r\n        }}\r\n        .header {{\r\n            padding: 16px 20px;\r\n            background-color: #0f172a;\r\n            color: #ffffff;\r\n            font-size: 16px;\r\n            font-weight: bold;\r\n        }}\r\n        .content {{\r\n            padding: 20px;\r\n            color: #111827;\r\n            font-size: 14px;\r\n            line-height: 1.6;\r\n        }}\r\n        .footer {{\r\n            padding: 14px 20px;\r\n            background-color: #f9fafb;\r\n            color: #6b7280;\r\n            font-size: 12px;\r\n            border-top: 1px solid #e5e7eb;\r\n        }}\r\n    </style>\r\n</head>\r\n<body>\r\n    <div class=\"container\">\r\n        <div class=\"header\">\r\n            Yearly Cron Job For ApiDesign10_Core\r\n        </div>\r\n\r\n        <div class=\"content\">\r\n            <p>Hello,</p>\r\n\r\n            <p>\r\n                This is a <strong>scheduled yearly test notification</strong> to confirm that the\r\n                CronJob system is functioning as expected.\r\n            </p>\r\n\r\n            <p>\r\n                The cron scheduler executed successfully, and the associated application code\r\n                ran without errors.\r\n            </p>\r\n\r\n            <p>\r\n                <strong>Execution Timestamp (UTC):</strong><br>\r\n                {DateTime.UtcNow}\r\n            </p>\r\n\r\n            <p>\r\n                No action is required for this notification.\r\n            </p>\r\n\r\n            <p>\r\n                Regards,<br>\r\n                <strong>KMT Automation System</strong>\r\n            </p>\r\n        </div>\r\n\r\n        <div class=\"footer\">\r\n            This is an automated system-generated message.\r\n        </div>\r\n    </div>\r\n</body>\r\n</html>\r\n";

        //try
        //{
        //    notifications.Add(notification);
        //}
        //catch (Exception ex)
        //{
        //    _logger.LogError($"CronJob.AddNotificationToDB: {ex.Message}");
        //}

        //if (notifications.Any())
        //{
        //    await _postgreBulkInsertion.BulkInsertZeptoMailRequestsAsync(notifications);
        //}
        return notification;
    }

    private async Task PublishEventForNotification()
    {
        //Get notification data from event
        var notification = await AddNotificationToDB();

        if (notification.To != null && notification.To.Any())

            // This is common for all transport types
            await _massTransitPublisher.PublishEventAsync(notification, "ZeptoMail");
    }
}
