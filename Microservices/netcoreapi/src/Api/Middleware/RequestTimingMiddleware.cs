using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Api.Middleware
{
    public class RequestTimingMiddleware
    {
        private const string MsSqlServer = "MsSqlServer";
        private const string PostgreSql = "PostgreSql";

        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RequestTimingMiddleware> _logger;

        public RequestTimingMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<RequestTimingMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();

            // Capture raw path now — PathBase+Path covers reverse-proxy sub-app scenarios
            var rawPath = (context.Request.PathBase + context.Request.Path).Value ?? string.Empty;

            await _next(context);

            sw.Stop();

            // UseRouting() runs inside _next(), so route values are populated here
            var routeValues = context.Request.RouteValues;
            var controller = routeValues?["controller"]?.ToString();
            var action = routeValues?["action"]?.ToString();
            var resolvedPath = (controller != null && action != null)
                               ? $"{controller}/{action}"
                               : rawPath;

            var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault()
                                ?? Guid.NewGuid().ToString();

            if (_configuration.GetValue<bool>("Api:Behavior:EnablePerformanceLog"))
            {
                try
                {
                    var dbServer = _configuration["Api:SqlDatabaseServer"] ?? MsSqlServer;

                    // Gateway writes its own row (full round-trip) directly from Gateway's RequestTimingMiddleware.
                    // netcoreapi only writes the netcoreapi row.
                    if (dbServer == PostgreSql)
                        await WritePostgre(_configuration.GetConnectionString("PostgreSqlDBConnection"),
                                           correlationId, context, resolvedPath, sw.ElapsedMilliseconds, "netcoreapi");
                    else
                        await WriteSql(_configuration.GetConnectionString("SqlDBConnection"),
                                       correlationId, context, resolvedPath, sw.ElapsedMilliseconds, "netcoreapi");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to write request timing to ApiRequestLog");
                }
            }
        }

        private static async Task WritePostgre(string cs, string correlationId, HttpContext ctx, string path, long elapsedMs, string source)
        {
            await using var conn = new NpgsqlConnection(cs);
            await conn.OpenAsync();
            await using var cmd = new NpgsqlCommand(
                "INSERT INTO app.\"ApiRequestLog\" (\"CorrelationId\", \"Method\", \"Path\", \"StatusCode\", \"ElapsedMs\", \"Source\") " +
                "VALUES (@c, @m, @p, @s, @e, @src)", conn);
            cmd.Parameters.AddWithValue("@c", correlationId);
            cmd.Parameters.AddWithValue("@m", ctx.Request.Method);
            cmd.Parameters.AddWithValue("@p", path);
            cmd.Parameters.AddWithValue("@s", ctx.Response.StatusCode);
            cmd.Parameters.AddWithValue("@e", elapsedMs);
            cmd.Parameters.AddWithValue("@src", source);
            await cmd.ExecuteNonQueryAsync();
        }

        private static async Task WriteSql(string cs, string correlationId, HttpContext ctx, string path, long elapsedMs, string source)
        {
            await using var conn = new SqlConnection(cs);
            await conn.OpenAsync();
            await using var cmd = new SqlCommand(
                "INSERT INTO ApiRequestLog (CorrelationId, Method, Path, StatusCode, ElapsedMs, Source) " +
                "VALUES (@c, @m, @p, @s, @e, @src)", conn);
            cmd.Parameters.AddWithValue("@c", correlationId);
            cmd.Parameters.AddWithValue("@m", ctx.Request.Method);
            cmd.Parameters.AddWithValue("@p", path);
            cmd.Parameters.AddWithValue("@s", ctx.Response.StatusCode);
            cmd.Parameters.AddWithValue("@e", elapsedMs);
            cmd.Parameters.AddWithValue("@src", source);
            await cmd.ExecuteNonQueryAsync();
        }
    }

    public static class RequestTimingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestTimingMiddleware(this IApplicationBuilder builder)
            => builder.UseMiddleware<RequestTimingMiddleware>();
    }
}
