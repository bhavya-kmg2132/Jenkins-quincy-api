using Application.Common.Mappings;

namespace Application.Users.Queries.AccessLevel
{
    /// <summary>
    /// Dto class is used to pass data from domain to ViewModel layer.
    /// It helps in:
    /// 1. Abstraction of Domain layer
    /// 2. Data Hiding
    /// 3. Serialization and Lazy load problems
    /// </summary>
    public class UserAccessLevelDto : IMapFrom<NetAuth.Contract.DataContract.Entities.UserAccessLevel>
    {
        public string UserAccessLevelValue { get; set; }
        public string UserAccessLevelName { get; set; }

        //Mapping UserAccessLevelDto with UserAccessLevel entity
        public void Mapping(Profile profile)
        {
            profile.CreateMap<NetAuth.Contract.DataContract.Entities.UserAccessLevel, UserAccessLevelDto>();
        }
    }
}
