
namespace NetAuth.Domain.Dto
{
    internal class TeamDto
    {
        public string Id { get; set; }
        public string TeamName { get; set; }
        public string TeamShortName { get; set; }
        public string Description { get; set; }
        public string TeamOwnerId { get; set; }
        public string TeamCaptainId { get; set; }
        public List<string> MemberIds { get; set; } = new();
    }
}
