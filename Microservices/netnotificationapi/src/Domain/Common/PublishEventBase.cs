using System;

namespace Domain.Common
{
    public abstract class PublishEventBase
    {
        public string EventName { get; set; }
        public string OperationType { get; set; }
        public DateTime OperationDateTimeUtc { get; set; }
        public string OperationSource { get; set; }
        public string ApiName { get; set; }
    }
}
