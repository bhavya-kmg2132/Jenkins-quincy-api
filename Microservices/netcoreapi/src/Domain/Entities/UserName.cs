using System.Collections.Generic;

namespace Domain.Entities
{
    public class UserName
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; }
    }
}
