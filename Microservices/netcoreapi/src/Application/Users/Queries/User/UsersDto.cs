using System.Collections.Generic;
using Application.Common.Mappings;

namespace Application.Users.Queries
{
    public class UsersDto : IMapFrom<NetAuth.Contract.DataContract.Dto.UsersDto>
    {
        public string userId { get; set; }
        public string display_name { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<NetAuth.Contract.DataContract.Dto.UsersDto, UsersDto>()
                .ForMember(d => d.Roles, opt => opt.Ignore());
        }
    }
}
