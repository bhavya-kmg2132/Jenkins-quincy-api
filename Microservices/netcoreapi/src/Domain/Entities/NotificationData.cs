using System.Collections.Generic;

public class NotificationData
{
    public string EventType { get; set; }

    // flexible key-value
    public Dictionary<string, object> Data { get; set; } = new();

    // optional common fields
    public string Subject { get; set; }
    public Dictionary<string, string> ToEmails { get; set; } = new();

    public string htmlTemplate { get; set; }
}