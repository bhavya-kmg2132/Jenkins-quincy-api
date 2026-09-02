using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetAuth.Application.Common.Interfaces;
using NetAuth.DataAccess;
using NetAuth.DataAccess.Common;
using NetAuth.Interfaces;
using netauthlib;

public class NetAuthLibServices
{

    public NetAuthLibServices()
    {

    }

    public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        services.AddTransient<INetAuthProvider, netauthlib.NetAuthProvider>();
        services.AddTransient<IUserLoader, UserLoader>();
        services.AddTransient<IIdentityManager, IdentityManager>();
        services.AddTransient<IUserDataAccess, UserDataAccess>();
        services.AddTransient<IUiPermissionDataAccess, UiPermissionDataAccess>();
        services.AddTransient<IConnectionHelper, ConnectionHelper>();

        // Field-level permission service: reads [FieldPermission] from entity properties.
        services.AddScoped<IFieldPermissionService, FieldPermissionService>();

        // services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }
}