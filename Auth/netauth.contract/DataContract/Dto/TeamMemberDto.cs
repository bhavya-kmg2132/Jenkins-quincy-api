namespace NetAuth.Contract.DataContract.Dto
{
    public class TeamMemberDto
    {
        public string MemberId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string preferred_username { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string AccessLevel { get; set; }
    }
}
