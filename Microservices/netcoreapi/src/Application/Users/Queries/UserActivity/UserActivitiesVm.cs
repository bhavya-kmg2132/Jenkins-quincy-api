using System.Collections.Generic;


namespace Application.Users.Queries.UserActivity
{
    /// <summary>
    /// The ViewModel class is designed to store and manage UI-related data
    /// </summary>
    public class UserActivitiesVm
    {
        public IList<UserActivityDto> UserActivities { get; internal set; }
    }
}
