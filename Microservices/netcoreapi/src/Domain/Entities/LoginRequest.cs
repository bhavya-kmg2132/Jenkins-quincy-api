namespace Domain.Entities
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public string oid { get; set; }
        public string auth_type { get; set; }
    }
}
