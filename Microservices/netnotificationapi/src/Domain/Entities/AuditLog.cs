using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities
{
    public class AuditLog : AuditableEntity, IHasDomainEvent
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Entity { get; set; }
        public string FieldName { get; set; }
        public string PreviousValue { get; set; }
        public string CurrentValue { get; set; }
        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}
