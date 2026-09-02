using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Community.Microsoft.Extensions.Caching.PostgreSql;
using Gateway.Interface;
using Gateway.Interfaces;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Middleware.Authorization;
using netauthlib;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Services.Authorization;
using Services.Backgroud;
using Services.Cache;

public class Startup
{
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public IConfiguration Configuration { get; }
    private readonly string AllowSpecificOrigins = "allowSpecificOrigin";
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        //Ensure token has scopes if the API is called on behalf of a user.
        //Ensure token has the app roles if the API can be called from a daemon app.

        var authBuilder = services.AddAuthentication(options =>
        {
            options.DefaultScheme = "MultiAuth";
        });

        // 🔀 Policy scheme
        authBuilder.AddPolicyScheme("MultiAuth", "Azure AD or DB JWT", options =>
        {
            options.ForwardDefaultSelector = context =>
            {
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader))
                    return "DbAuth";

                var token = authHeader.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);

                try
                {
                    var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

                    // Azure AD tokens have issuer https://sts.windows.net
                    if (jwt.Issuer.StartsWith("https://sts.windows.net", StringComparison.OrdinalIgnoreCase))
                        return "AzureAd";
                }
                catch
                {
                    // Malformed token — fall through to DbAuth for proper validation failure
                }

                return "DbAuth";
            };
        });

        // 🔹 Azure AD (Entra ID)
        authBuilder.AddMicrosoftIdentityWebApi(
            Configuration.GetSection("AzureAd"),
            jwtBearerScheme: "AzureAd"
        );

        // 🔹 Database JWT
        authBuilder.AddJwtBearer("DbAuth", options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Configuration["Jwt:Issuer"],

                ValidateAudience = true,
                ValidAudience = Configuration["Jwt:Audience"],

                ValidateLifetime = true, // you intentionally disabled it
                ClockSkew = TimeSpan.Zero,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])
                )
            };
        });


        //TODO: Team work on Policy
        // ================= AUTHORIZATION =================
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AzureUsersOnly",
                policy => policy.RequireClaim("iss"));

            options.AddPolicy("DbUsersOnly",
                policy => policy.RequireClaim("auth_type", "db"));
        });


        ////////////services
        ////////////    .AddDefaultIdentity<ApplicationUser>()
        ////////////    .AddRoles<IdentityRole>()
        ////////////    .AddEntityFrameworkStores<ApplicationDbContext>();

        ////////////services.AddIdentityServer()
        ////////////    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();
        // Enable cors to allow authentic sites
        string[] alloworiginapis = Configuration.GetSection("WebUrl:AllowOriginApis").Get<string[]>();

        services.AddCors(options =>
        {
            options.AddPolicy("allowSpecificOrigin",
                              builder =>
                              {
                                  builder.WithOrigins(alloworiginapis)
                                  .AllowAnyOrigin()
                                  .AllowAnyHeader()
                                  .AllowAnyMethod();

                              });
        });

        //Added Background Service
        services.AddHostedService<CacheSyncService>();

        // Configure HostOptions to control background service behavior.
        services.Configure<HostOptions>(options =>
        {
            options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
        });

        services.AddOcelot().AddCacheManager(x => x.WithDictionaryHandle());
        if (Convert.ToBoolean(Configuration["CacheSettings:UseInMemoryCache"]))
        {
            services.AddMemoryCache();
            services.AddTransient<IMemoryCacheService, InMemoryCacheService>();
        }
        // These are mutually exclusive on purpose: SqlServerCacheService and RedisCacheService
        // each take a plain IDistributedCache in their constructor, and .NET's DI container
        // resolves a single IDistributedCache to whichever implementation was registered LAST —
        // regardless of which typed wrapper is asking for it. If both blocks ran, e.g.
        // SqlServerCacheService could silently end up backed by the Redis client instead of SQL
        // Server. An if/else-if chain makes it impossible for more than one to be active at once,
        // instead of relying on nobody ever flipping both config flags on at the same time.
        // SQL Server is checked first because AuthorizationService's own _useSqlServerCache flag
        // is read independently from CacheSettings:UseSqlServerCache, so SQL Server must win
        // whenever that flag is true, or ISqlServerCacheService ends up null while
        // _useSqlServerCache is still true, and AuthorizationService would call a method on that
        // null reference.
        if (Convert.ToBoolean(Configuration["CacheSettings:UseSqlServerCache"]))
        {
            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = Configuration.GetConnectionString("SqlDBConnection");
                options.SchemaName = "dbo";
                options.TableName = "Cache";
            });
            services.AddTransient<ISqlServerCacheService, SqlServerCacheService>();
        }
        else if (Convert.ToBoolean(Configuration["CacheSettings:UseRedisCache"]))
        {
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = Configuration.GetConnectionString("RedisConnection");
            });
            services.AddTransient<IRedisCacheService, RedisCacheService>();
            services.AddTransient<ISqlServerCacheService>(_ => null!);
        }
        else
        {
            services.AddTransient<ISqlServerCacheService>(_ => null!);
        }
        if (!Convert.ToBoolean(Configuration["CacheSettings:UseInMemoryCache"]))
            services.AddTransient<IMemoryCacheService>(_ => null!);

        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<IEndpoint, Gateway.Integration.Endpoint>();

        if (Convert.ToBoolean(Configuration["UseNetAuthLib"]))
        {
            // Register netauthlib services 
            NetAuthLibServices netAuthLibServices = new NetAuthLibServices();
            netAuthLibServices.ConfigureServices(Configuration, services);
        }

        services.AddLogging();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRequestTimingMiddleware();

        app.UseCors(AllowSpecificOrigins);
        app.UseAuthentication();

        app.UseAddClaimsToRequestMiddleware();
        app.UseAuthorization();

        //If true then add authorization middldeware
        if (Configuration.GetValue<bool>("Api:Behavior:CheckApiPermission"))
        {
            app.UseAuthorizationMiddleware();
        }

        app.UseOcelot().Wait();
    }
}
