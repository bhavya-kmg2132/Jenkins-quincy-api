namespace Domain.Dto
{
    public class PublishEventDataDto
    {
        public string Id { get; set; }
        public string CorrelationId { get; set; }
        public string AuditableRequestId { get; set; }
        public string AuditableRequestName { get; set; }
        public string AuditableAssemblyQualifiedName { get; set; }
        public string AuditableSourceEventName { get; set; }
        public string CreatedDateTime { get; set; }
        public string ApiName { get; set; }
        public string CollectionName { get; set; }
        public string EventData { get; set; }
        public string UserId { get; set; }
        public string OperationType { get; set; }
    }
}
