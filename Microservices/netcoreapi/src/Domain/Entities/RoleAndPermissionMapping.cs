using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities
{
    public class RoleAndPermissionMapping : AuditableEntity
    {
        public string RoleId { get; set; }
        public List<string> PermissionIds { get; set; }
    }
}
