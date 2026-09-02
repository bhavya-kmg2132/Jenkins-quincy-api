using System;
using System.Net.Http;
using System.Threading.Tasks;
using Api;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

[SetUpFixture]
public class Testing
{
    private static WebApplicationFactory<Program> _factory = null!;

    //1. Test Users
    //1.1) Admin User
    public const string AdminTestUsername = "test_Admin_User@systemdesign.com";
    public const string AdminTestPassword = "Admin@Test123!";

    //1.2) Level 1 User
    public const string Level1TestUsername = "test_Level_1_User@systemdesign.com";
    public const string Level1TestPassword = "Level1@Test123!";

    //1.3) Level 3 User
    public const string Level2TestUsername = "test_Level_2_User@systemdesign.com";
    public const string Level2TestPassword = "Level2@Test123!";

    //2. Http Headers - mandatory fields for auth
    public const string ApiKey = "68a8a0a5-f22f-4ef8-ae1c-81fc23f40faf";
    public const string RequestUid = "test_Admin_User@systemdesign.com";

    private static IConfiguration _configuration = null!;
    public static IConfiguration Configuration => _configuration;
    private static IServiceScopeFactory _scopeFactory;
    private static string _currentUserId = null;
    public static ICurrentUserService _currentUserService = null;
    public static IUserDataAccess _userDataAccess;
    private static int IncrementalInt = 0;

    static IServiceScope _scope;

    [OneTimeSetUp]
    public async Task RunBeforeAnyTests()
    {
        _factory = new Application.IntegrationTests.CustomWebApplicationFactory();
        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
        _configuration = _factory.Services.GetRequiredService<IConfiguration>();
        _scope = _scopeFactory.CreateScope();
        _userDataAccess = _scope.ServiceProvider.GetService<IUserDataAccess>();

        SetupMockHttpContext();

        EnsureDatabase();

        // Grant Policy field permissions to Admin if they exist in the Permission table.
        // Permissions added to the global Permission table (e.g. via the UI) require an
        // explicit PermissionGranted row per user; without it AuthHasRequestPermissionAsync
        // returns false even for Admin.  This INSERT is a no-op on a fresh DB where those
        // permissions have not yet been registered globally.
        await EnsureAdminHasPolicyFieldPermissionsAsync();

        _currentUserService = _scope.ServiceProvider.GetRequiredService<ICurrentUserService>();

        await RunAsDefaultUserAsync();

        #region code not used .
        //_currentUserService.AccessLevel = "";

        //var builder = new ConfigurationBuilder()
        //    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
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

        #endregion
    }

    public static string GetCurrentUserId()
    {

        //_currentUserId = TestRunnerUserId;
        return _currentUserId;
    }

    public static string GetCurrentUserAccessLevel()
    {
        string result = null;
        if (_currentUserId.Equals("d320862d-6863-42f1-94a8-107728e4f20a"))// Admin User
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

        return result;
    }

    public static async Task<ICurrentUserService> RunAsDefaultUserAsync()
        => await RunAsAdminUserAsync();

    public static async Task<ICurrentUserService> RunAsAdminUserAsync()
    {
        try
        {
            var dbUser = await _userDataAccess.GetUserFromNetAuthLibAsync(AdminTestUsername);
            if (dbUser?.Id != null)
                _currentUserService = await RunAsUserAsync(dbUser.Id);
        }
        catch (Exception)
        {
            // Auth tables may not exist in this environment; tests run with minimal user context.
        }
        return _currentUserService;
    }

    public static async Task<ICurrentUserService> RunAsLevel1UserAsync()
    {
        try
        {
            var dbUser = await _userDataAccess.GetUserFromNetAuthLibAsync(Level1TestUsername);
            if (dbUser?.Id != null)
                _currentUserService = await RunAsUserAsync(dbUser.Id);
        }
        catch (Exception)
        {
            // Auth tables may not exist in this environment; tests run with minimal user context.
        }
        return _currentUserService;
    }

    public static async Task<ICurrentUserService> RunAsLevel2UserAsync()
    {
        try
        {
            var dbUser = await _userDataAccess.GetUserFromNetAuthLibAsync(Level2TestUsername);
            if (dbUser?.Id != null)
                _currentUserService = await RunAsUserAsync(dbUser.Id);
        }
        catch (Exception)
        {
            // Auth tables may not exist in this environment; tests run with minimal user context.
        }
        return _currentUserService;
    }

    public static ICurrentUserService GetICurrentUserService()
    {

        return _currentUserService;
    }

    // Re-applied before every test so that endpoint tests (which run through the real
    // ASP.NET Core middleware) cannot wipe out the mock by setting HttpContext = null
    // when their HTTP request ends.
    private static void SetupMockHttpContext(string requestUid = RequestUid)
    {
        var httpContextAccessor = _scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
        var mockHttpContext = Substitute.For<HttpContext>();
        var mockHttpRequest = Substitute.For<HttpRequest>();
        var requestHeaders = new HeaderDictionary();
        requestHeaders.Add("X-Correlation-Id", Guid.NewGuid().ToString());
        requestHeaders.Add("X-Request-Id", Guid.NewGuid().ToString());
        requestHeaders.Add("X-Api-Key", ApiKey);
        requestHeaders.Add("X-Request-Uid", requestUid);
        mockHttpRequest.Headers.Returns(requestHeaders);
        mockHttpContext.Request.Returns(mockHttpRequest);
        httpContextAccessor.HttpContext = mockHttpContext;
    }

    private static void EnsureDatabase()
    {
        var Logger = _scope.ServiceProvider.GetService<ILogger<Infrastructure.DataAccess.DataAccess>>();

        var context = new Infrastructure.DataAccess.DataAccess(_configuration, Logger);
        context.TestSQLServerDBConnection();
    }

    public static string GetUniqueName()
    {
        IncrementalInt++;

        return "UIT" + Testing.IncrementalInt + Guid.NewGuid();
    }


    public static IAcmeDataAccess GetAcmeDataAccess()
    {
        //using var scope = _scopeFactory.CreateScope();
        var Logger = _scope.ServiceProvider.GetService<ILogger>();
        var context = _scope.ServiceProvider.GetService<IAcmeDataAccess>();

        return context;
    }

    public static IPolicyDataAccess GetPolicyDataAccess()
    {
        var context = _scope.ServiceProvider.GetService<IPolicyDataAccess>();
        return context;
    }
    public static HttpClient GetTestHttpClient() => _factory.CreateClient();

    public static T GetService<T>()
    {
        if (_scope == null) _scope = _scopeFactory.CreateScope();
        return _scope.ServiceProvider.GetRequiredService<T>();
    }

    public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {

        if (_scope == null)
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
            var userDataAccess = _scope.ServiceProvider.GetRequiredService<IIdentityManager>();
            var user = await userDataAccess.GetIdentityUserAsync(userIdParam);
            if (user != null)
            {
                _currentUserId = user.UserId;
                _currentUserService.UserId = _currentUserId;
                _currentUserService.oid = user.oid;
                _currentUserService.name = user.display_name;
                _currentUserService.UserName = user.UserName;
                _currentUserService.AccessLevel = user.AccessLevel;
                _currentUserService.IsActive = user.IsActive ? user.IsActive : true;
                _currentUserService.IsDeleted = user.IsDeleted;
                _currentUserService.UserRoles = new System.Collections.Generic.List<string>();
                if (user.UserRoles != null)
                {
                    foreach (var role in user.UserRoles)
                    {
                        _currentUserService.UserRoles.Add(role.RoleName);
                    }
                }

                // Sync the mock HTTP context so AuthorizationBehaviour.ValidateRequestUser()
                // reads X-Request-Uid as this user, not the previous user's header.
                SetupMockHttpContext(user.UserName);
            }
        }

        return _currentUserService;
    }

    public static async Task ResetState()
    {
        // Re-apply the mock HttpContext on every reset. Endpoint tests run through the
        // real ASP.NET Core test server, which sets HttpContext = null on the singleton
        // TestHttpContextAccessor when each HTTP request ends. Without this call,
        // ApiKeyAuthBehaviour throws NullReferenceException on the next MediatR dispatch.
        SetupMockHttpContext();

        // Clear singleton properties so RunAsUserAsync always re-initializes them.
        // Without this, a previous test that ran as a non-admin user leaves a non-null
        // AccessLevel on the singleton, causing the initialization block to be skipped.
        if (_currentUserService != null)
        {
            _currentUserService.UserId = null;
            _currentUserService.UserName = null;
            _currentUserService.AccessLevel = null;
            _currentUserService.UserRoles = null;
        }
        _currentUserService = null;
        _currentUserId = "";
        await Task.CompletedTask;
    }


    private static async Task EnsureAdminHasPolicyFieldPermissionsAsync()
    {
        var netAuthConnStr = _configuration["NetAuth.ConnectionStrings:SqlDBConnection"];
        if (string.IsNullOrEmpty(netAuthConnStr)) return;

        // MERGE so we handle both the "never inserted" and "soft-deleted" cases.
        const string sql = @"
            MERGE [dbo].[PermissionGranted] AS target
            USING (
                SELECT u.[Id] AS UserId, p.[Id] AS PermissionId
                FROM [dbo].[User] u
                JOIN [dbo].[Permission] p ON p.[PermissionValue] IN (
                    'Core.Policy.TotalPremium.View',
                    'Core.Policy.TotalPremium.Edit',
                    'Core.Policy.RenewalStatus.View',
                    'Core.Policy.RenewalStatus.Edit'
                )
                WHERE u.[UserName] = 'test_Admin_User@systemdesign.com'
                  AND (p.[IsDeleted] IS NULL OR p.[IsDeleted] = 0)
            ) AS src ON target.[UserId] = src.UserId AND target.[PermissionId] = src.PermissionId
            WHEN MATCHED AND (target.[IsDeleted] = 1 OR target.[IsActive] = 0) THEN
                UPDATE SET [IsDeleted] = 0, [IsActive] = 1, [UpdatedDateTime] = GETUTCDATE()
            WHEN NOT MATCHED THEN
                INSERT ([Id], [UserId], [PermissionId], [IsDeleted], [IsActive], [CreatedDateTime], [UpdatedDateTime])
                VALUES (CONVERT(VARCHAR(100), NEWID()), src.UserId, src.PermissionId, 0, 1, GETUTCDATE(), GETUTCDATE());";

        using var connection = new SqlConnection(netAuthConnStr);
        await connection.OpenAsync();
        using var command = new SqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();

        // Invalidate the HybridCache so the updated PermissionGranted rows are visible
        // to the first test that calls HasPermissionAsync.
        var identityManager = _scope.ServiceProvider.GetRequiredService<IIdentityManager>();
        await identityManager.ResetUserCache();
    }

    [OneTimeTearDown]
    public Task RunAfterAnyTests()
    {
        return Task.CompletedTask;
    }

}
