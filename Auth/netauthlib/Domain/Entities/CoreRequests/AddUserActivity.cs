namespace NetAuth.Domain.Entities.CoreRequests
{
    internal class AddUserActivity
    {
        public string UserId { get; set; }
        public DateTime? LastLoginDateTime { get; set; }
        public DateTime? LastActivityDateTime { get; set; }
        public string LastActivityModule { get; set; }
        public string LastActionType { get; set; }
        public string LastActivityDetail { get; set; }
        public bool IsUserLogout { get; set; } = false;
        public string CreatedBy { get; set; }

    }

}
