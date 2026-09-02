namespace Domain.Entities
{
    public class NotificationSubscriptionDetail
    {
        public string NotificationId { get; set; }
        public bool OptOut { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
