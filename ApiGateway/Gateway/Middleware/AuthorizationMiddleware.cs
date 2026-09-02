using Gateway.Interface;

namespace Middleware.Authorization
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAuthorizationService _authorizationService;

        public AuthorizationMiddleware(RequestDelegate next, IAuthorizationService authorizationService)
        {
            _next = next;
            _authorizationService = authorizationService;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            var route = context.Request.PathBase.Add(context.Request.Path).ToString();

            if (route.Contains("gateway",StringComparison.OrdinalIgnoreCase))
            {
                if (!context.Request.Headers.TryGetValue("X-Request-Uid", out var userId) || string.IsNullOrEmpty(userId))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized: No user ID found.");
                    return;
                }

                if (!await _authorizationService.HasPermissionAsync(userId, route))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Forbidden: You do not have permission to access this resource.");
                    return;
                }
            }

            await _next(context);
        }
    }

    public static class AuthorizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthorizationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthorizationMiddleware>();
        }
    }
}
