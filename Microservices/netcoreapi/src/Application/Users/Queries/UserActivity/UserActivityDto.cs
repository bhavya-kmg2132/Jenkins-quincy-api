using System;
using Application.Common.Mappings;

namespace Application.Users.Queries.UserActivity
{
    /// <summary>
    /// Dto class is used to pass data from domain to ViewModel layer.
    /// It helps in:
    /// 1. Abstraction of Domain layer
    /// 2. Data Hiding
    /// 3. Serialization and Lazy load problems
    /// </summary>
    public class UserActivityDto : IMapFrom<NetAuth.Contract.DataContract.Dto.UserActivityDto>
    {
        public string Id { get; set; }
        public DateTime? LastLoginDateTime { get; set; }
        public DateTime? LastLogoutDateTime { get; set; }
        public DateTime? LastActivityDateTime { get; set; }
        public string LastActivityModule { get; set; }
        public string LastActionType { get; set; }
        public string LastActivityDetail { get; set; }
        public string Name { get; set; }

        //Mapping UserActivityDto with UserActivity entity
        public void Mapping(Profile profile)
        {
            profile.CreateMap<NetAuth.Contract.DataContract.Dto.UserActivityDto, UserActivityDto>()
            .ForMember(d => d.Name, opt => opt.Ignore());
        }
    }
}
