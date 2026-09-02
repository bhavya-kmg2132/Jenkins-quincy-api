using System.Linq;
using Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
//using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            var integrationConfig = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            configurationBuilder.AddConfiguration(integrationConfig);
        });

        builder.ConfigureServices((builder, services) =>
        {
            services.AddLogging();

            // Replace AsyncLocal-based HttpContextAccessor with a plain field-based one
            // so the mocked HttpContext flows to all NUnit test threads.
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IHttpContextAccessor));
            if (descriptor != null) services.Remove(descriptor);
            services.AddSingleton<IHttpContextAccessor, TestHttpContextAccessor>();


            //services
            //.Remove<ICurrentUserService>()
            //.AddScoped(provider => Mock.Of<ICurrentUserService>(s =>
            //    s.UserId == GetCurrentUserId()));
            //        && s.AccessLevel == GetCurrentUserAccessLevel()));

            //services
            //    .Remove<DbContextOptions<ApplicationDbContext>>()
            //    .AddDbContext<ApplicationDbContext>((sp, options) =>
            //        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
            //            builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        });
    }
}