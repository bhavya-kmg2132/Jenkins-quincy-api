using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Common
{
    public abstract class AuditableEntity
    {
        #region Request Tracing
        public string CorrelationId { get; set; }
        public string AuditableRequestId { get; set; }
        public string AuditableRequestName { get; set; }
        public string AuditableSourceEventName { get; set; }
        public string AuditableAssemblyQualifiedName { get; set; }

        #endregion

        public string CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public DateTime? CreatedDateTime { get; set; } = DateTime.UtcNow;
        public string UpdatedBy { get; set; }
        public string UpdatedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public string UpdateReason { get; set; }

        [NotMapped]
        public string OwnerId { get; set; }
        [NotMapped]
        public bool IsActive { get; set; }
        [NotMapped]
        public bool IsDeleted { get; set; }
        [NotMapped]
        public bool? IsApproved { get; set; }
        [NotMapped]
        public string ApproverId { get; set; }
        [NotMapped]
        public DateTime? ApprovedDateTime { get; set; }
        [NotMapped]
        public bool? IsAuthorized { get; set; }
        [NotMapped]
        public string AuthorizedById { get; set; }
        [NotMapped]
        public DateTime? AuthorizedDateTime { get; set; }
        [NotMapped]
        public string SysData { get; set; }
        [NotMapped]
        public string TenantId { get; set; }
        [NotMapped]
        public string AssociatedUserId { get; set; }
        [NotMapped]
        public string SubTenantId { get; set; }
        public List<CustomField> CustomFields { get; set; }
    }
}
