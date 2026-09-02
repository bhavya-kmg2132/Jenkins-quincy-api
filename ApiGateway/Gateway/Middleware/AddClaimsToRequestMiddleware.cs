using System.IdentityModel.Tokens.Jwt;

public class AddClaimsToRequestMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public AddClaimsToRequestMiddleware(IConfiguration configuration, RequestDelegate next)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext context)
    {
        // Extract the access token from the Authorization header
        var authorizationHeader = context.Request.Headers["Authorization"].ToString();
        var CorrelationId = Guid.NewGuid().ToString();
        var RequestId = Guid.NewGuid().ToString();
        string apiKey = this._configuration["Api:api-key"];



        // Check if the authorization header is present and starts with "Bearer "
        if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
        {
            // Extract the access token from the authorization header
            var accessToken = authorizationHeader.Substring("Bearer ".Length);

            // Check if the access token is not empty
            if (!string.IsNullOrEmpty(accessToken))
            {
                string token_decoded_uid = null;
                string oidClaim = null;

                try
                {
                    // Extract claims from the access token
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadToken(accessToken) as JwtSecurityToken;
                    var unique_name = jsonToken?.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
                    oidClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == "oid")?.Value;
                    var email = jsonToken?.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                    var preferred_username = jsonToken?.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
                    var userName = jsonToken?.Claims.FirstOrDefault(c => c.Type == "username")?.Value;

                    string uid = Convert.ToString(userName);
                    
                    if (string.IsNullOrEmpty(uid))
                        uid = email;

                    if (string.IsNullOrEmpty(uid))
                        uid = preferred_username;

                    if (string.IsNullOrEmpty(uid))
                        uid = unique_name;

                    token_decoded_uid = uid;
                }
                catch
                {
                    // Malformed token — skip claim extraction; downstream auth will reject it
                }

                // Add claims as headers to the outgoing request
                if (!context.Request.Headers.ContainsKey("X-Correlation-Id"))
                {
                    context.Request.Headers.Append("X-Correlation-Id", CorrelationId);
                }

                if (!context.Request.Headers.ContainsKey("X-Request-Id"))
                {
                    context.Request.Headers.Append("X-Request-Id", RequestId);
                }
                context.Request.Headers.Append("X-Request-Uid", token_decoded_uid ?? string.Empty);
                context.Request.Headers.Append("X-Request-Oid", oidClaim ?? string.Empty);
            }

        }
        // Add X-Api-Key in request header
        if (!string.IsNullOrWhiteSpace(apiKey) && !context.Request.Headers.ContainsKey("X-Api-Key"))
        {
            context.Request.Headers.Append("X-Api-Key", apiKey);
        }

        // Forward gateway start timestamp so netcoreapi can compute full round-trip elapsed
        if (context.Items.TryGetValue("GatewayStartMs", out var gatewayStartMs))
        {
            context.Request.Headers.Append("X-Gateway-Start-Ms", gatewayStartMs.ToString());
        }

        await _next(context);
    }

}

public static class AddClaimsToRequestMiddlewareExtensions
{
    public static IApplicationBuilder UseAddClaimsToRequestMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AddClaimsToRequestMiddleware>();
    }
}
