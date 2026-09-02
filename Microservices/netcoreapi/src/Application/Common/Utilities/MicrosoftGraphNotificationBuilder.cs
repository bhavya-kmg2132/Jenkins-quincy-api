using System;
using Domain.Entities;
using Microsoft.Extensions.Configuration;

public class MicrosoftGraphNotificationBuilder : INotificationBuilder
{
    private readonly IConfiguration _config;

    public NotificationProvider Provider => NotificationProvider.MSGraph;

    public MicrosoftGraphNotificationBuilder(IConfiguration config)
    {
        _config = config;
    }

    public NotificationPayload BuildPayloadForSelectedNotificationProvider(NotificationData data)
    {

        return new PostgreNotification
        {
            Id = Guid.NewGuid().ToString(),
            ApiKey = _config["Api:api-key"],
            EmailTo = string.Join(";", data.ToEmails.Values),
            EmailSubject = data.Subject,
            EmailBody = data.htmlTemplate,
            EntityJson = new NotificationEntityJson
            {
                NotificationType = "email"
            }
        };
    }

    //private string BuildHtml(Dictionary<string, object> values)
    //{
    //    var html = "<h3>Notification</h3><table>";

    //    foreach (var item in values)
    //    {
    //        html += $"<tr><td><b>{item.Key}</b></td><td>{item.Value}</td></tr>";
    //    }

    //    html += "</table>";

    //    return html;
    //}
}