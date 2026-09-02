using System.Collections.Generic;

namespace Application.IntegrationTests
{
    public class UserFromToken
    {
        public List<string> Roles { get; set; }
        public string Username { get; set; }
        public string Scope { get; set; }
        public string Name { get; set; }
        public string Oid { get; set; }
        public string UserId { get; set; }


    }
}
