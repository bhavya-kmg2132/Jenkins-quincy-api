namespace NetAuth.Contract.DataContract.Requests
{
    public class UpdateUserPasswordHash
    {
        public string UserId { get; set; }
        public string PasswordHash { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public string UpdateReason { get; set; }
    }

}
