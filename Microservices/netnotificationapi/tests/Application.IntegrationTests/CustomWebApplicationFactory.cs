using Api;
using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
//using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Application.IntegrationTests;

using static Testing;

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