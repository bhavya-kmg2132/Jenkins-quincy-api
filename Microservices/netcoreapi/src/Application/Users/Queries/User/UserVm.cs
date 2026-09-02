using System.Collections.Generic;


namespace Application.Users.Queries
{
    /// <summary>
    /// The ViewModel class is designed to store and manage UI-related data
    /// </summary>
    public class UserVm
    {
        public UserDto User { get; set; }
        public IList<UserDto> UserInfo { get; set; }
    }
}
