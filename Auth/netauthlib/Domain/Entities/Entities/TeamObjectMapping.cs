using NetAuth.Domain.Common;

namespace NetAuth.Domain.Entities
{
    internal class TeamObjectMapping : AuditableEntity
    {
        public string Id { get; set; }
        public string TeamId { get; set; }
        public string ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string AssignedToTeamMemberId { get; set; }
    }
}
