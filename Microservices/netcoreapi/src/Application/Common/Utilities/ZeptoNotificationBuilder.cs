using System;
using System.Linq;
using Domain.Entities;
using Microsoft.Extensions.Configuration;

public class ZeptoNotificationBuilder : INotificationBuilder
{
    private readonly IConfiguration _config;

    public NotificationProvider Provider => NotificationProvider.Zepto;

    public ZeptoNotificationBuilder(IConfiguration config)
    {
        _config = config;
    }

    public NotificationPayload BuildPayloadForSelectedNotificationProvider(NotificationData data)
    {
        var recipients = data.ToEmails.Select(email => new ZeptoMailRecipient
        {
            email_address = new ZeptoMailAddress
            {
                address = email.Value,
                name = email.Key
            }
        }).ToList();

        return new ZeptoMail
        {
            Id = Guid.NewGuid().ToString(),
            ApiKey = _config["Api:api-key"],
            To = recipients,
            Subject = data.Subject,
            HtmlBody = data.htmlTemplate
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