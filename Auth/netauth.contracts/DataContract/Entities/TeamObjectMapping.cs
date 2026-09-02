using NetAuth.Contract.DataContract.Common;
namespace NetAuth.Contract.DataContract.Entities
{
    public class TeamObjectMapping : AuditableEntity
    {
        public string Id { get; set; }
        public string TeamId { get; set; }
        public string ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string AssignedToTeamMemberId { get; set; }
    }
}
