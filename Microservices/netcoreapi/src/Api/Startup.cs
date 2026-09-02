using System;
using System.IO.Compression;
using System.Linq;
using Api.Filters;
using Api.Middleware;
using Api.Services;
using Api.Services.HealthCheck;
using Application.Common.Interfaces;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Confluent.Kafka;
using Dapper;
using Dapper.Extensions.Caching.Memory;
using Dapper.Extensions.MSSQL;
using Domain.Entities;
using HealthChecks.UI.Client;
using Infrastructure;
using Infrastructure.Persistence.BuildScripts;
//using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using Npgsql;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private readonly string AllowSpecificOrigins = "allowSpecificOrigin";

        // This method gets called by the runtime. Use this method to add services to the container.
        [Obsolete]
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("Db");
            //Configuring Health Ckeck
            services.ConfigureHealthChecks(Configuration);

            services.AddApplicationServices();
            services.AddInfrastructure(Configuration);



            if (Configuration.GetSection("Api:SelfAuthentication").Get<bool>())
            {
                //Ensure token has scopes if the API is called on behalf of a user.
                //Ensure token has the app roles if the API can be called from a daemon app.
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddMicrosoftIdentityWebApi(Configuration.GetSection("AzureAd"));
            }


            // Enable cors to allow authentic sites
            string[] alloworiginapis = Configuration.GetSection("WebUrl:AllowOriginApis").Get<string[]>();
            services.AddCors(options =>
            {
                options.AddPolicy("allowSpecificOrigin",
                                  builder =>
                                  {
                                      builder.WithOrigins(alloworiginapis)
                                      .AllowAnyHeader()
                                      .AllowAnyMethod();
                                  });
            });


            //// Role based access
            ////This enforces global authentication, similar to using the [Authorize] attribute on controllers and / or operations
            services.AddControllers(options =>
            {

                if (Configuration.GetSection("Api:SelfAuthentication").Get<bool>())
                {
                    var policy = new AuthorizationPolicyBuilder()
                                     .RequireAuthenticatedUser()
                                     .Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                }
                //Api exception filter
                options.Filters.Add(new ApiExceptionFilterAttribute());
            });

            // Setup Logger 
            #region Setup Logger
            services.AddSingleton<ILogger>(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                return loggerFactory.CreateLogger("NLog");
            });
            #endregion



            services.AddSingleton<IConfiguration>(Configuration);
            services.AddHttpContextAccessor();
            services.AddHostedService<CacheSyncService>();
            services.AddScoped<ICurrentUserService, Api.Services.CurrentUserService>();
            #region KAFKA
            var kafkaProducerConfig = new ProducerConfig
            {
                //BootstrapServers = "localhost:9092"\
                BootstrapServers = Configuration["Kafka:BootstrapServers"],
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = Configuration["Kafka:SaslUsername"],
                SaslPassword = Configuration["Kafka:SaslPassword"]
            };
            services.AddSingleton<IProducer<Null, string>>(x => new ProducerBuilder<Null, string>(kafkaProducerConfig).Build());
            services.AddSingleton<IWeatherDataPublisher, WeatherDataPublisher>();
            #endregion

            // Background Service
            services.AddHostedService<ApiWorkerService>();

            // Configure HostOptions to control background service behavior.
            services.Configure<HostOptions>(options =>
            {
                options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
            });

            services.AddSwaggerGen(c =>
            {

                c.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = "My API v1" });
                c.SwaggerDoc("v2", new OpenApiInfo { Version = "v2", Title = "My API v2" });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                if (Configuration.GetSection("Api:SelfAuthentication").Get<bool>())
                {
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Scheme = "bearer",
                        Description = "Please insert JWT token into field"
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    } });
                }

                c.CustomSchemaIds(type => type.FullName);
            });

            // Configure API versioning
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    //new QueryStringApiVersionReader("version"),
                    new UrlSegmentApiVersionReader()
                //new HeaderApiVersionReader("X-API-Version"),
                //new MediaTypeApiVersionReader("version")
                );
            });

            //register dbup in services
            services.AddSingleton(_ => new DatabaseInitializer(Configuration, _.GetService<ILogger<DatabaseInitializer>>()));

            services.AddMemoryCache();

            // Add Dapper in-memory caching
            services.AddDapperCachingInMemory(new MemoryConfiguration
            {
                AllMethodsEnableCache = false,

            });

            services.AddDapperForMSSQL();

            // Response compression (Brotli preferred, Gzip fallback)
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });
            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            //Add hybrid cache
            services.AddHybridCache(options =>
            {
                options.MaximumPayloadBytes = 1024 * 1024;
                options.MaximumKeyLength = 1024;
                options.DefaultEntryOptions = new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromHours(24),
                    LocalCacheExpiration = TimeSpan.FromHours(18),

                };
            });

            var builder = new ContainerBuilder();
            builder.Populate(services);

            string SqlDBConnection = Configuration["ConnectionStrings:SqlDBConnection"];
            builder.AddDapperForMSSQL("SqlDBConnection", SqlDBConnection);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v2/swagger.json", "Api v2"));
            }

            app.UseHealthChecksUI(delegate (HealthChecks.UI.Configuration.Options options)
            {
                options.UIPath = "/health-ui";
                //options.AddCustomStylesheet("./HealthCheck/Custom.css");

            });

            app.UseSwagger();
             app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "My API v1");
                c.SwaggerEndpoint("v2/swagger.json", "My API v2");
            });


            app.UseRequestTimingMiddleware();

            app.UseResponseCompression();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(AllowSpecificOrigins);


            if (Configuration.GetSection("Api:SelfAuthentication").Get<bool>())
            {
                app.UseAuthentication();
                app.UseAuthorization();
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/api/health", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                    //Predicate = (check) => check.Tags.Contains("all")
                });
            });

            app.UseStaticFiles();

            //Initialize dbup
            var dbinitializer = app.ApplicationServices.GetRequiredService<DatabaseInitializer>();
            dbinitializer.Initialize();

            // Fetching the NotifcationRules and Scheduling jobs in Hangfire
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var scheduler = scope.ServiceProvider.GetRequiredService<ICronJobScheduler>();
                //var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var cronScheduler = (CronJobScheduler)scheduler;

                using var connection = new NpgsqlConnection(Configuration["Hangfire:HangfireTransportConnection:PostGreSQL"]);

                var rules = connection.Query<CronJobRule>(
                            cronScheduler.SqlQueries["CronJobRule.GetCronJobRules"]);

                foreach (var rule in rules)
                {
                    // ScheduleCronJobAsync is synchronous (returns Task.CompletedTask); no blocking needed.
                    _ = scheduler.ScheduleCronJobAsync(rule);
                }
            }
        }
    }
}
