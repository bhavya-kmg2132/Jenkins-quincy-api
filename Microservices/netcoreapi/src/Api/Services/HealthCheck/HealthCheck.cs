using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Api.Services.HealthCheck
{
    public static class HealthCheck
    {
        public static void ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddSqlServer(configuration["ConnectionStrings:SqlDBConnection"], healthQuery: "select 1", name: "SQL server", failureStatus: HealthStatus.Unhealthy, tags: new[] { "Feedback", "Database" })
                // Remote server health check temporarily disabled.
                // Reason: No remote endpoint currently available for validation.
                // This check should be re-enabled once a remote service or API is configured for health monitoring.
                //.AddCheck<RemoteHealthCheck>("Remote endpoints Health Check", failureStatus: HealthStatus.Unhealthy)
                .AddCheck<MemoryHealthCheck>($"Feedback Service Memory Check", failureStatus: HealthStatus.Unhealthy, tags: new[] { "Feedback Service" });
                // "base URL" AddUrlGroup check removed: HealthCheckEndpoints:heartbeatapi pointed at a
                // dev-machine-only address with no equivalent deployed service (UseNetAuthLib is true,
                // i.e. auth runs via the embedded NetAuth.Lib, not a separate netauthapi microservice).
                // It made /api/health permanently report Unhealthy outside that one dev machine.

            //services.AddHealthChecksUI();
            services.AddHealthChecksUI(setupSettings: opt =>
            {
                opt.SetEvaluationTimeInSeconds(10); //time in seconds between check    
                opt.MaximumHistoryEntriesPerEndpoint(60); //maximum history of checks    
                opt.SetApiMaxActiveRequests(1); //api requests concurrency    
                opt.AddHealthCheckEndpoint("feedback api", configuration["HealthCheckEndpoints:healthCheck"]); //map health check api    

            })
                .AddInMemoryStorage();
        }
    }
}
