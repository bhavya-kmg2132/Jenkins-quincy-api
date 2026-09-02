using Api;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Respawn;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Infrastructure.DataAccess;

[SetUpFixture]
public class Testing
{
    private static WebApplicationFactory<Program> _factory = null!;


    public const string TestRunnerUserId = "d320862d-6863-42f1-94a8-107728e4f20a"; //ChrisGreenUserId
    public const string TestRunnerUserName = "Chris.Green@lambinsurance.onmicrosoft.com";
    public const string TestUserName = "Caffeine Developer";

    //public const string TestRunnerUserId = "60c7521f-9316-4a5f-a478-cffbb0ea0367"; //ChrisGreenUserId
    //public const string TestRunnerUserName = "Chris.Green@caffeine09.onmicrosoft.com";
    //public const string TestUserName = "Chris Green";

    //public const string TestRunnerUserId = "cb672f8f-e085-4560-b4bd-fd3b693f2c3e"; //JohnSmithUserId
    //public const string TestRunnerUserName = "John.Smith@caffeine09.onmicrosoft.com";

    //public const string TestRunnerUserId = "9747b08a-2ffe-4927-be70-982a5088fb83"; // Level1User1@caffeine09.onmicrosoft.com
    //public const string TestRunnerUserName = "Level1User1@caffeine09.onmicrosoft.com";

    //public const string TestRunnerUserId = "fea63b39-f30d-463f-b0bb-c826924a6fcf"; // Level2User1@caffeine09.onmicrosoft.com
    //public const string TestRunnerUserName = "Level2User1@caffeine09.onmicrosoft.com";

    //public const string TestRunnerUserId = "8f131307-5589-4759-80b6-d4c930f8da9c"; // Level3User1@caffeine09.onmicrosoft.com
    //public const string TestRunnerUserName = "Level3User1@caffeine09.onmicrosoft.com";

    //private static IConfigurationRoot _configuration;
    private static IConfiguration _configuration = null!;
    private static IServiceScopeFactory _scopeFactory;
    private static Checkpoint _checkpoint;
    private static string? _currentUserId = null;
    private static string? _currentUserAccessLevel = null;
    public static ICurrentUserService _currentUserService = null;
    private static int IncrementalInt = 0;

    static IServiceScope _scope;

    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        _factory = new Application.IntegrationTests.CustomWebApplicationFactory();
        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
        _configuration = _factory.Services.GetRequiredService<IConfiguration>();
        _scope = _scopeFactory.CreateScope();

        _currentUserService = _scope.ServiceProvider.GetRequiredService<ICurrentUserService>();
        
        //_currentUserService.AccessLevel = "";




        //var builder = new ConfigurationBuilder()
        //    .SetBasePath(Directory.GetCurrentDirectory())
        //    .AddJsonFile("appsettings.json", true, true)
        //    .AddEnvironmentVariables();

        //_configuration = builder.Build();

        //var startup = new Startup(_configuration);

        //var services = new ServiceCollection();

        //services.AddSingleton(Mock.Of<IWebHostEnvironment>(w =>
        //    w.EnvironmentName == "Development" &&
        //    w.ApplicationName == "Api"));


        //services.AddLogging();
        //startup.ConfigureServices(services);
        //_scopeFactory = services.BuildServiceProvider().GetService<IServiceScopeFactory>();


        EnsureDatabase();

        //////Replace service registration for ICurrentUserService
        //////Remove existing registration
        //var currentUserServiceDescriptor = services.FirstOrDefault(d =>
        //   d.ServiceType == typeof(ICurrentUserService));
        //services.Remove(currentUserServiceDescriptor);

        ////// Register testing version
        //services.AddTransient(provider =>
        //    Mock.Of<ICurrentUserService>(s => s.UserId == _currentUserId));

        //// This may be reason for db cleanups
        //_checkpoint = new Checkpoint
        //{
        //    TablesToIgnore = new[] { "__EFMigrationsHistory" }
        //};
    }

    public static string? GetCurrentUserId()
    {

        //_currentUserId = TestRunnerUserId;
        return _currentUserId;
    }

    public static string? GetCurrentUserAccessLevel()
    {
        string result =null;
        if (_currentUserId.Equals("d320862d-6863-42f1-94a8-107728e4f20a"))// Chris green Lamb Insurance
        {
            result = null;
        }
        if (_currentUserId.Equals("60c7521f-9316-4a5f-a478-cffbb0ea0367"))// Chris green
        {
            result = null;
        }
        else if (_currentUserId.Equals("9747b08a-2ffe-4927-be70-982a5088fb83")) //Level1 User
        {
            result = "Level1";
        }
        else if (_currentUserId.Equals("fea63b39-f30d-463f-b0bb-c826924a6fcf")) //Level2 User
        {
            result = "Level2";
        }
        else if (_currentUserId.Equals("8f131307-5589-4759-80b6-d4c930f8da9c")) //Level3 User
        {
            result = "Level3";
        }

        return result;
    }

    public static async Task<ICurrentUserService> RunAsDefaultUserAsync()
    {
        _currentUserService = await RunAsUserAsync(TestRunnerUserId);

        return _currentUserService;
    }

    public static ICurrentUserService GetICurrentUserService()
    {
        
        return _currentUserService;
    }

    private static void EnsureDatabase()
    {
        //using var scope = _scopeFactory.CreateScope();
        var Logger = _scope.ServiceProvider.GetService<ILogger<Infrastructure.DataAccess.DataAccess>>();
        //var Logger = scope.ServiceProvider.GetRequiredService<ILogger<Infrastructure.DataAccess.DataAccess>>();

        var context = new Infrastructure.DataAccess.DataAccess(_configuration, Logger);
        context.TestSQLServerDBConnection();
    }

    public static string GetUniqueProspectName()
    {
        IncrementalInt++;

        return "PIT" + Testing.IncrementalInt + Guid.NewGuid();
    }

    public static string GetUniqueContactName()
    {
        IncrementalInt++;

        return "CIT" + Testing.IncrementalInt + Guid.NewGuid();
    }

    public static string GetUniqueName()
    {
        IncrementalInt++;

        return "UIT" + Testing.IncrementalInt + Guid.NewGuid();
    }

  
    //public static IAcmeOrderDataAccess GetAcmeOrderDataAccess()
    //{
    //    //using var scope = _scopeFactory.CreateScope();
    //    var Logger = _scope.ServiceProvider.GetService<ILogger>();
    //    var context = _scope.ServiceProvider.GetService<IAcmeOrderDataAccess>();

    //    return context;
    //}

    //public static IAcmeOrderDetailDataAccess GetAcmeOrderDetailDataAccess()
    //{
    //    //using var scope = _scopeFactory.CreateScope();
    //    var Logger = _scope.ServiceProvider.GetService<ILogger>();
    //    var context = _scope.ServiceProvider.GetService<IAcmeOrderDetailDataAccess>();

    //    return context;
    //}
    public static IUserDataAccess GetUserDataAccess()
    {
        //using var scope = _scopeFactory.CreateScope();
        var Logger = _scope.ServiceProvider.GetService<ILogger>();
        var context = _scope.ServiceProvider.GetService<IUserDataAccess>();

        return context;
    }

    public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {

        if (_scope ==null)
        {
            _scope = _scopeFactory.CreateScope();
        }

        //await RunAsDefaultUserAsync();

        var mediator = _scope.ServiceProvider.GetService<ISender>();

        return await mediator.Send(request);
    }

    public static async Task<ICurrentUserService> RunAsUserAsync(string userIdParam)
    {
        if (_scope == null)
        {
            _scope = _scopeFactory.CreateScope();
        }


        if (_currentUserService == null)
        {
            _currentUserService = _scope.ServiceProvider.GetRequiredService<ICurrentUserService>();
            //_currentUserService.UserId = _currentUserId;

        }

        if (string.IsNullOrEmpty(_currentUserService.AccessLevel))
        {
            var userDataAccess = _scope.ServiceProvider.GetRequiredService<IUserDataAccess>();
            var user = await userDataAccess.GetUserFromDb(userIdParam);
            _currentUserId = user.Id;
            _currentUserService.UserId = _currentUserId;

            _currentUserService.oid = user.oid;
            _currentUserService.name = user.display_name;
            _currentUserService.UserName = user.UserName;
            _currentUserService.AccessLevel = user.AccessLevel;
            _currentUserService.UserRoles = new System.Collections.Generic.List<string>();
            foreach (var role in user.Roles)
            {
                _currentUserService.UserRoles.Add(role.RoleName);
            }
        }

        return _currentUserService;
    }

    public static async Task ResetState()
    {
        //// This may be reason for db cleanups
        //await _checkpoint.Reset(_configuration.GetConnectionString("SqlDBConnection"));
        _currentUserService = null;
        _currentUserId = "";
    }


    [OneTimeTearDown]
    public void RunAfterAnyTests()
    {
    }
}
