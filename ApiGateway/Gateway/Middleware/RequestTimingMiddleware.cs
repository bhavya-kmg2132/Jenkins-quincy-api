using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Npgsql;

public class RequestTimingMiddleware
{
    private const string PostgreSql = "PostgreSql";
    private const string MsSqlServer = "MsSqlServer";

    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimingMiddleware> _logger;
    private readonly IConfiguration _configuration;

    public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();

        // Store start time for AddClaimsToRequestMiddleware to forward as X-Gateway-Start-Ms
        context.Items["GatewayStartMs"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        await _next(context);

        // sw covers: gateway middleware + network to netcoreapi + netcoreapi processing + network back
        // This is the true full round-trip time from the gateway's perspective.
        sw.Stop();

        // X-Correlation-Id is set by AddClaimsToRequestMiddleware (runs inside _next),
        // so it is available here after _next returns.
        var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault()
                            ?? Guid.NewGuid().ToString();

        _logger.LogInformation(
            "Gateway|{Method} {Path}|CorrelationId:{CorrelationId}|{StatusCode}|{ElapsedMs}ms",
            context.Request.Method, context.Request.Path, correlationId,
            context.Response.StatusCode, sw.ElapsedMilliseconds);

        if (_configuration.GetValue<bool>("Api:Behavior:EnablePerformanceLog"))
        {
            try
            {
                var dbServer = _configuration["Api:SqlDatabaseServer"] ?? MsSqlServer;

                if (dbServer == PostgreSql)
                    await WritePostgre(
                        _configuration.GetConnectionString("PostgreSqlDBConnection"),
                        correlationId, context, sw.ElapsedMilliseconds);
                else
                    await WriteSql(
                        _configuration.GetConnectionString("SqlDBConnection"),
                        correlationId, context, sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write gateway timing to ApiRequestLog");
            }
        }
    }

    private static async Task WritePostgre(string cs, string correlationId, HttpContext ctx, long elapsedMs)
    {
        await using var conn = new NpgsqlConnection(cs);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            "INSERT INTO app.\"ApiRequestLog\" (\"CorrelationId\", \"Method\", \"Path\", \"StatusCode\", \"ElapsedMs\", \"Source\") " +
            "VALUES (@c, @m, @p, @s, @e, @src)", conn);
        cmd.Parameters.AddWithValue("@c", correlationId);
        cmd.Parameters.AddWithValue("@m", ctx.Request.Method);
        cmd.Parameters.AddWithValue("@p", ctx.Request.Path.Value ?? string.Empty);
        cmd.Parameters.AddWithValue("@s", ctx.Response.StatusCode);
        cmd.Parameters.AddWithValue("@e", elapsedMs);
        cmd.Parameters.AddWithValue("@src", "Gateway");
        await cmd.ExecuteNonQueryAsync();
    }

    private static async Task WriteSql(string cs, string correlationId, HttpContext ctx, long elapsedMs)
    {
        await using var conn = new SqlConnection(cs);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(
            "INSERT INTO ApiRequestLog (CorrelationId, Method, Path, StatusCode, ElapsedMs, Source) " +
            "VALUES (@c, @m, @p, @s, @e, @src)", conn);
        cmd.Parameters.AddWithValue("@c", correlationId);
        cmd.Parameters.AddWithValue("@m", ctx.Request.Method);
        cmd.Parameters.AddWithValue("@p", ctx.Request.Path.Value ?? string.Empty);
        cmd.Parameters.AddWithValue("@s", ctx.Response.StatusCode);
        cmd.Parameters.AddWithValue("@e", elapsedMs);
        cmd.Parameters.AddWithValue("@src", "Gateway");
        await cmd.ExecuteNonQueryAsync();
    }
}

public static class RequestTimingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestTimingMiddleware(this IApplicationBuilder builder)
        => builder.UseMiddleware<RequestTimingMiddleware>();
}
