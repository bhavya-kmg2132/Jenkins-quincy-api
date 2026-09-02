namespace Domain.Common
{
    public abstract class PublishEventBase : AuditableEntity
    {
        //public string AuditableSourceEventName { get; set; }
        //public DateTime CreatedDateTime { get; set; }
        public string OperationType { get; set; }
        public string OperationSource { get; set; }
        public string ApiName { get; set; }
    }
}

