
using Api.Services;
using Confluent.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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


            ////Ensure token has scopes if the API is called on behalf of a user.
            ////Ensure token has the app roles if the API can be called from a daemon app.
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddMicrosoftIdentityWebApi(Configuration.GetSection("AzureAd"));

            //// For ACL: Daemon applications
            ////Update the AuthorizationPolicyBuilder, to check that the azp claim is part of the configured Access Control List
            ////From now on, calling the API will only succeed if the client application is allowed in the ACL in appsetting
            //services.AddControllers(options =>
            //{
            //    var policy = new AuthorizationPolicyBuilder()
            //                     .RequireAuthenticatedUser()
            //                     .RequireClaim("azp", Configuration.GetSection("AzureAd:AccessControlList").Get<string[]>())
            //                     .Build();
            //    options.Filters.Add(new AuthorizeFilter(policy));
            //});


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


            //// Role based access
            ////This enforces global authentication, similar to using the [Authorize] attribute on controllers and / or operations
            services.AddControllers(options =>
            {
                //var policy = new AuthorizationPolicyBuilder()
                //                 .RequireAuthenticatedUser()
                //                 .Build();
                //options.Filters.Add(new AuthorizeFilter(policy));

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
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "Api", Version = "v2" });
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
            });
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

            app.UseSwagger();
            //  app.UseSwaggerUI(c => c.SwaggerEndpoint("/networker/swagger/v2/swagger.json", "Api v2"));
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v2/swagger.json", "Api v2"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(AllowSpecificOrigins);

            //app.UseAuthentication();
            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseStaticFiles();
        }
    }
}
