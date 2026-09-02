using System.Linq;
using Api.Filters;
using Api.Services;
using Api.Services.Notification;
using Application.Common.Interfaces;
using Confluent.Kafka;
using Dapper.Extensions.MSSQL;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

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
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationServices();
            services.AddInfrastructure(Configuration);

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

            services.AddScoped<ApiExceptionFilterAttribute>();

            //// Role based access
            ////This enforces global authentication, similar to using the [Authorize] attribute on controllers and / or operations
            services.AddControllers(options =>
            {
                // Use DI to add ApiExceptionFilterAttribute
                options.Filters.AddService<ApiExceptionFilterAttribute>();
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

            //services.AddSingleton<ICurrentUserService, Api.Services.CurrentUserService>();
            services.AddScoped<ICurrentUserService, Api.Services.CurrentUserService>();
            services.AddHttpClient();
            services.AddDapperForMSSQL();


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
            services.AddHostedService<Api.Services.ApiWorkerService>();

            //For Notification
            services.AddHostedService<Api.Services.Notification.NotificationWorkerServiceForZeptoMail>();
            services.AddHostedService<NotificationWorkerService>();


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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1"));
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "My API v2");
            });
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(AllowSpecificOrigins);

            app.UseEndpoints(endpoints =>
              {
                  endpoints.MapControllers();
              });

            app.UseStaticFiles();
        }
    }
}
