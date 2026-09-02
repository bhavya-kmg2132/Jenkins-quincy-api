using NetAuth.Contract.DataContract.Common;

namespace NetAuth.Contract.DataContract.Entities
{
    public class IdentityUserTeam : AuditableEntity
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
