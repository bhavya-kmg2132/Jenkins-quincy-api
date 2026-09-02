using System;
using Domain.Common;

namespace Domain.Entities
{
    public class ApiLog : AuditableEntity
    {
        public DateTime? LogDate { get; set; }
        public string Discription { get; set; }

    }
}
