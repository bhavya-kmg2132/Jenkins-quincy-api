using NetAuth.Domain.Common;

namespace NetAuth.Domain.Entities
{
    internal class IdentityUserTeam : AuditableEntity
    {
        public string Id { get; set; }
        public string MemberId { get; set; }
        public string TeamName { get; set; }
        public string TeamShortName { get; set; }
        public string Description { get; set; }
        public string TeamOwnerId { get; set; }
        public string TeamCaptainId { get; set; }
    }
}
