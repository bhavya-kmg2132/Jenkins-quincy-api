using System.Collections.Generic;


namespace Application.Users.Queries.AccessLevel
{
    /// <summary>
    /// The ViewModel class is designed to store and manage UI-related data
    /// </summary>
    public class UserAccessLevelListVm
    {
        public IList<UserAccessLevelDto> UserAccessLevelList { get; internal set; }
    }
}
