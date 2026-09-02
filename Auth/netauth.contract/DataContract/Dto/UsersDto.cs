namespace NetAuth.Contract.DataContract.Dto
{
    public class UsersDto
    {
        public string userId { get; set; }
        public string display_name { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; }

    }
}
