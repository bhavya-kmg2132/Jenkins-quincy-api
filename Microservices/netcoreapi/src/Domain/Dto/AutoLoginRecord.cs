using System;

namespace Domain.Dto
{
    public class AutoLoginRecord
    {
        public string UserId { get; set; }
        public DateTime Expiry { get; set; }
        public bool IsUsed { get; set; }
    }
}
