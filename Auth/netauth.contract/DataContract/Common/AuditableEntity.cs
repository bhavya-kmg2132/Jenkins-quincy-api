namespace NetAuth.Contract.DataContract.Common
{
    public abstract class AuditableEntity
    {
        #region Request Tracing
        public string CorrelationId { get; set; }
        public string AuditableRequestId { get; set; }
        public string AuditableRequestName { get; set; }
        public string AuditableSourceEventName { get; set; }

        #endregion
        public string CreatedBy { get; set; }
        public DateTime? CreatedDateTime { get; set; } = DateTime.UtcNow;
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDateTime { get; set; } = DateTime.UtcNow;

        public string UpdateReason { get; set; }
        public string OwnerId { get; set; }
       public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsApproved { get; set; }
        public string ApproverId { get; set; }
        public DateTime? ApprovedDateTime { get; set; }
        public bool? IsAuthorized { get; set; }
        public string AuthorizedById { get; set; }
        public DateTime? AuthorizedDateTime { get; set; }
        public string SysData { get; set; }
        public string TenantId { get; set; }
        public string SubTenantId { get; set; }
    }
}
