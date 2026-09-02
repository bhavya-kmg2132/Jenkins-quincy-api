using System.Collections.Generic;
using Application.Common.Mappings;

namespace Application.Users.Queries.Team
{
    public class TeamListVm
    {
        public IList<TeamVm> Teams { get; internal set; }
    }

    public class TeamVm : IMapFrom<NetAuth.Contract.DataContract.Dto.TeamDto>
    {
        public string Id { get; set; }
        public string TeamName { get; set; }
        public string TeamShortName { get; set; }
        public string Description { get; set; }
        public string TeamOwnerId { get; set; }
        public string TeamCaptainId { get; set; }
        public List<string> MemberIds { get; set; }
    }
}
