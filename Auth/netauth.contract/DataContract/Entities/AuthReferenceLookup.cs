using NetAuth.Contract.DataContract.Common;

namespace NetAuth.Contract.DataContract.Entities
{
    public class AuthReferenceLookup : AuditableEntity
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

    }
}
