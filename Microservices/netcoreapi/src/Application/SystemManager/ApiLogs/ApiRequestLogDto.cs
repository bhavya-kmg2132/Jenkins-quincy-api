using System;

namespace Application.ApiLog.Queries
{
    public class ApiRequestLogDto
    {
        public long Id { get; set; }
        public string CorrelationId { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public int StatusCode { get; set; }
        public long ElapsedMs { get; set; }
        public string Source { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
