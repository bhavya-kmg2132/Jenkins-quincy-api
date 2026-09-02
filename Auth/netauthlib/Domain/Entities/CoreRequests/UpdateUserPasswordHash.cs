namespace NetAuth.Domain.Entities.CoreRequests
{
    internal class UpdateUserPasswordHash
    {
        public string UserId { get; set; }
        public string PasswordHash { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public string UpdateReason { get; set; }
    }

}
