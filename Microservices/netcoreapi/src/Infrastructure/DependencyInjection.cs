using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Application.Common.Behaviours;
using Application.Common.Interfaces;
using Application.Common.Utilities;
using Community.Microsoft.Extensions.Caching.PostgreSql;
using DataAccess.Common;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure.DataAccess;
using Infrastructure.Files;
using Infrastructure.Identity;
using Infrastructure.Integration;
using Infrastructure.Services;
using Infrastructure.Services.Cache;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using netauthlib;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        [Obsolete]
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            //{
            //    services.AddDbContext<ApplicationDbContext>(options =>
            //        options.UseInMemoryDatabase("CleanArchitectureDb"));
            //}
            //else
            //{
            //    services.AddDbContext<ApplicationDbContext>(options =>
            //        options.UseSqlServer(
            //            configuration.GetConnectionString("DefaultConnection"),
            //            b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            //}

            // services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());



            services.AddScoped<IDomainEventService, DomainEventService>();

            //Ensure token has scopes if the API is called on behalf of a user.
            //Ensure token has the app roles if the API can be called from a daemon app.
            if (configuration.GetSection("Api:SelfAuthentication").Get<bool>())
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "MultiAuth";
                })
                    .AddPolicyScheme("MultiAuth", "Azure AD or DB JWT", options =>
                    {
                        options.ForwardDefaultSelector = context =>
                        {
                            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                            if (string.IsNullOrEmpty(authHeader))
                                return "DbAuth";

                            var token = authHeader.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);

                            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

                            // Azure AD tokens have issuer https://sts.windows.net
                            if (jwt.Issuer.StartsWith("https://sts.windows.net"))
                                return "AzureAd";

                            return "DbAuth";
                        };
                    });

                // -------- Azure AD (Entra ID) --------
                services.AddAuthentication().AddMicrosoftIdentityWebApi(
                    configuration.GetSection("AzureAd"), jwtBearerScheme: "AzureAd"
                    );

                // -------- Database JWT --------
                services.AddAuthentication()
                .AddJwtBearer("DbAuth", options =>
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            context.HandleResponse(); // prevent default 302/500
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";

                            return context.Response.WriteAsync(
                                "{\"message\":\"Token is required\"}"
                            );
                        }
                    };

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["Jwt:Key"])
                        ),

                        ValidateIssuer = true,
                        ValidIssuer = configuration["Jwt:Issuer"],

                        ValidateAudience = true,
                        ValidAudience = configuration["Jwt:Audience"],

                        ValidateLifetime = true
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

            }

            //1.MASS TRANSIT CONFIGURATION : PostGreSQL
            if (Convert.ToBoolean(configuration["MassTransit:MassTransitTransportType:PostGreSQL"]))
            {
                //Step 1 :  Transport Configuration (used to provide config values like
                //                                   Db,Table,Queue where the messages is to be publised)
                services.Configure<SqlTransportOptions>(options =>
                {
                    options.ConnectionString = configuration["MassTransit:MassTransitTransportConnection:PostGreSQL"];

                    options.Schema = "app"; // default for PostgreSQL
                });

                //Step 2 : Table's migration (Usually not required in Producer side,
                //                            but if the servie is running first-time, it is useful)
                //services.AddPostgresMigrationHostedService(true, false);

                //Step 3 : Add Mass Transit (Register MassTransit, its methods like IPublishEndpoint, starts the bus)
                services.AddMassTransit(x =>
                {
                    // This code is used in Consumer only.

                    ////Consumer is added in NetNotification (Api-Design10)
                    //x.AddConsumer<CreateOrderConsumer>();

                    x.UsingPostgres((context, cfg) =>
                    {
                        cfg.ConfigureEndpoints(context);

                        cfg.UseConcurrencyLimit(1);

                        cfg.UseMessageRetry(r =>
                            r.Interval(3, TimeSpan.FromSeconds(10)));
                    });
                });
            }

            #region Hangfire

            //1.HANGFIRE(CRON JOB) CONFIGURATION: POSTGRE SQL
            if (Convert.ToBoolean(configuration["Hangfire:HangfireTransportType:PostGreSQL"]))
            {
                services.AddHangfire(config =>
            {
                config.UsePostgreSqlStorage(
                    configuration["Hangfire:HangfireTransportConnection:PostGreSQL"],
                   new PostgreSqlStorageOptions
                   {
                       SchemaName = "app"
                   });
            });

                // Start Hangfire background processing
                services.AddHangfireServer();
            }
            #endregion

            if (Convert.ToBoolean(configuration["CacheSettings:UsePostgreSqlCache"]))
            {
                services.AddDistributedPostgreSqlCache(options =>
                {
                    options.ConnectionString = configuration.GetConnectionString("PostgreSqlDBConnection");
                    options.SchemaName = "app";
                    options.TableName = "Cache";
                });
                services.AddTransient<IPostGreCacheService, PostGreCacheService>();
            }

            if (Convert.ToBoolean(configuration["CacheSettings:UseInMemoryCache"]))
            {
                services.AddMemoryCache();
                services.AddTransient<IMemoryCacheService, InMemoryCacheService>();
                services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehaviour<,>));
            }
            if (Convert.ToBoolean(configuration["CacheSettings:UseRedisCache"]))
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = configuration.GetConnectionString("RedisConnection");
                });
                services.AddTransient<IRedisCacheService, RedisCacheService>();
            }
            if (Convert.ToBoolean(configuration["CacheSettings:UseSqlServerCache"]))
            {
                services.AddDistributedSqlServerCache(options =>
                 {
                     options.ConnectionString = configuration.GetConnectionString("SqlDBConnection");
                     options.SchemaName = "dbo";
                     options.TableName = "Cache";
                 });
                services.AddTransient<ISqlServerCacheService, SqlServerCacheService>();
            }

            services.AddTransient<IDateTime, DateTimeService>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddTransient<IMasterDataAccess, MasterDataAccess>();
            services.AddTransient<IPublishEventDataAccess, PublishEventDataAccess>();


            services.AddScoped<CronJob>();
            services.AddScoped<ICronJobScheduler, CronJobScheduler>();

            services.AddHttpClient<IDb2PolicyService, Db2PolicyService>();

            services.AddTransient<IWebClient, WebClient>();
            services.AddTransient<IFileReaderWriter, FileReaderWriter>();
            services.AddTransient<IAcmeDataAccess, AcmeDataAccess>();
            services.AddTransient<ICrmMasterDataAccess, CrmMasterDataAccess>();
            services.AddTransient<ITransactionActionDataAccess, TransactionActionDataAccess>();

            services.AddTransient<IAcmeProductFileReaderWriter, AcmeProductFileLocation>();
            services.AddTransient<IInitialSetUpDataAccess, InitialSetUpDataAccess>();
            services.AddTransient<IMainDbInitialSetUpDataAccess, MainDbInitialSetUpDataAccess>();
            services.AddTransient<IEventDbInitialSetUpDataAccess, EventDbInitialSetUpDataAccess>();
            services.AddTransient<IUserDataAccess, UserDataAccess>();
            services.AddTransient<IUiPermissionDataAccess, UiPermissionDataAccess>();


            services.AddTransient<IPolicyDataAccess, PolicyDataAccess>();
            services.AddTransient<IConnectionHelper, ConnectionHelper>();
            services.AddTransient<IVersionTrackDataAccess, VersionTrackDataAccess>();
            services.AddTransient<IMassTransitPublisher, MassTransitPublisher>();
            services.AddTransient<IPostgreBulkInsertion, PostgreBulkInsertion>();

            services.AddTransient<ICustomFieldDataAccess, CustomFieldDataAccess>();
            services.AddTransient<IRefreshTokenDataAccess, RefreshTokenDataAccess>();
            services.AddSingleton<IAutoLoginStore, AutoLoginStore>();

            services.AddScoped<NotificationHelper>();
            services.AddScoped<INotificationBuilder, ZeptoNotificationBuilder>();
            services.AddScoped<INotificationBuilder, MicrosoftGraphNotificationBuilder>();

            //services.AddTransient<IDapperAcmeDataAccess, DapperAcmeDataAccess>();

            if (Convert.ToBoolean(configuration["UseNetAuthLib"]))
            {
                NetAuthLibServices netAuthLibServices = new NetAuthLibServices();
                netAuthLibServices.ConfigureServices(configuration, services);
            }

            //services.AddAuthentication()
            //    .AddIdentityServerJwt();

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("CanPurge", policy => policy.RequireRole("Administrator"));
            //});

            services.AddTransient<IIdentityManager, IdentityManager>();

            return services;
        }
    }
}