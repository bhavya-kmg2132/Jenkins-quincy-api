namespace NetAuth.Lib.Domain.Entities.CoreRequests
{
    public class UpdateUser
    {
        public string userId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string EmpId { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDateTime { get; set; }

    }
}
