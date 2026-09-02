namespace NetAuth.Domain.Entities.CoreDto
{
    internal class TokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string user_id { get; set; }
        public DateTime issued { get; set; }
        public DateTime expires { get; set; }
        public object mfa_token { get; set; }
    }
}
