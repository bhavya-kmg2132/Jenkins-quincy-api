namespace Application.Common.Interfaces
{
    public class ExternalPolicyResponse
    {
        public int StatusCode { get; set; }
        public string Content { get; set; }
        public bool IsSuccessStatusCode { get; set; }
    }
}
