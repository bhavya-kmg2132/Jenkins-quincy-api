using System.Collections.Generic;


namespace Application.Users.Queries.UserProfile
{
    /// <summary>
    /// The ViewModel class is designed to store and manage UI-related data
    /// </summary>
    public class UserProfileListVm
    {
        public IList<UserProfileDto> UserProfileList { get; internal set; }

    }
}
