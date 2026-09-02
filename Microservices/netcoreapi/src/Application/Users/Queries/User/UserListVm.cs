using System.Collections.Generic;


namespace Application.Users.Queries
{
    /// <summary>
    /// The ViewModel class is designed to store and manage UI-related data
    /// </summary>
    public class UserListVm
    {
        public IList<UserDto> UserList { get; internal set; }
    }
}
