using Microsoft.AspNetCore.Http;

namespace Domain.Entities
{
    public class EmailNotification
    {
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string To { get; set; }
        public string messageId { get; set; }
        public IFormFile file { get; set; }

        public class EmailTemplate
        {
            public string Email { get; set; }
            public string Receiver { get; set; }
            public string Subject { get; set; }
            public string Trigger { get; set; }
            public string Template { get; set; }
        }
    }
}
