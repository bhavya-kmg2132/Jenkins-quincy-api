using System.Collections.Generic;
using Domain.Entities;

public class NotificationUserSubscription
{
    public string UserId { get; set; }
    public List<NotificationSubscriptionDetail> SubscriptionDetails { get; set; }

}
