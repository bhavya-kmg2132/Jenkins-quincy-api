using NetAuth.Domain.Enums;

namespace NetAuth.Domain.Dto
{
    internal class UserActivityDto
    {
        public string UserActivityId { get; set; }
        public string UserId { get; set; }
        public DateTime? LastLoginDateTime { get; set; }
        public DateTime? LastLogoutDateTime { get; set; }
        public DateTime? LastActivityDateTime { get; set; }
        public string LastActivityModule { get; set; }
        public UserActionType? LastActionType { get; set; }
        public string LastActivityDetail { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDateTime { get; set; }

        public string PublishEventId { get; set; }
        public string EventName { get; set; }
        public DateTime? OperationDateTimeUtc { get; set; }
        public string Data { get; set; }
    }
}
