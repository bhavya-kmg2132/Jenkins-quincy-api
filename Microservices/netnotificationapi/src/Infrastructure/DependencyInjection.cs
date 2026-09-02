using System;
using Application.Common.Behaviours;
using Application.Common.Interfaces;
using Infrastructure.Integration;
using Infrastructure.Integration.EmailNotification;
using Infrastructure.Persistence;
//using Infrastructure.Persistence;
using Infrastructure.Services;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
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

            services.AddDbContext<ApplicationDbContext>(options =>
              options.UseSqlServer(configuration.GetConnectionString("SqlDBConnection")));

            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

            services.AddTransient<IDomainEventService, DomainEventService>();

            services.Configure<SqlTransportOptions>(options =>
            {
                options.ConnectionString =
                    configuration.GetConnectionString("AzureEventDBPostgreSqlDBConnection");

                options.Schema = "app"; // default for PostgreSQL

            });

            //services.AddPostgresMigrationHostedService(true, false);


            #region MassTransit using Batch Processing with PostgreSQL Transport (Not in use)
            //services.AddMassTransit(x =>
            //{
            //    // 🔹 Register both consumers
            //    x.AddConsumer<CreateNotificationBatchConsumer>();
            //    x.AddConsumer<MessageEnvelopeBatchConsumer<string>>();

            //    x.UsingPostgres((context, cfg) =>
            //    {
            //        cfg.ConnectConsumeObserver(
            //            context.GetRequiredService<FailurMessageObserver>());

            //        // 🔹 Endpoint 1 → CreateNotification
            //        cfg.ReceiveEndpoint("create-notification-queue", e =>
            //        {
            //            e.ConcurrentMessageLimit = 1;

            //            e.Batch<CreateNotificationMessage>(b =>
            //            {
            //                b.MessageLimit = 10;
            //                b.TimeLimit = TimeSpan.FromSeconds(5);
            //            });

            //            e.ConfigureConsumer<CreateNotificationBatchConsumer>(context);
            //        });

            //        // 🔹 Endpoint 2 → AnotherMessage
            //        cfg.ReceiveEndpoint("message-envelop-queue", e =>
            //        {
            //            e.ConcurrentMessageLimit = 1;

            //            e.Batch<MessageEnvelope<string>>(b =>
            //            {
            //                b.MessageLimit = 10;
            //                b.TimeLimit = TimeSpan.FromSeconds(5);
            //            });

            //            e.DiscardFaultedMessages();

            //            e.ConfigureConsumer<MessageEnvelopeBatchConsumer<string>>(context);
            //        });

            //        // ❗ Retry applies globally (both endpoints)
            //        cfg.UseMessageRetry(r =>
            //            r.Interval(2, TimeSpan.FromSeconds(10)));

            //    });
            //});
            #endregion


            services.AddMassTransit(x =>
            {
                // 🔹 Register both consumers
                x.AddConsumer<CreateNotificationConsumer>();
                x.AddConsumer<MessageEnvelopeConsumer<string>>();

                x.UsingPostgres((context, cfg) =>
                {
                    cfg.ConnectConsumeObserver(
                        context.GetRequiredService<FailurMessageObserver>());

                    // 🔹 Endpoint 1 → CreateNotification
                    cfg.ReceiveEndpoint("create-notification-queue", e =>
                    {
                        e.ConcurrentMessageLimit = 1;

                        e.DiscardFaultedMessages();

                        e.PrefetchCount = 10;

                        e.ConfigureConsumer<CreateNotificationConsumer>(context);
                    });

                    // 🔹 Endpoint 2 → AnotherMessage
                    cfg.ReceiveEndpoint("message-envelop-queue", e =>
                    {
                        e.ConcurrentMessageLimit = 1;

                        e.DiscardFaultedMessages();

                        e.PrefetchCount = 10;

                        e.ConfigureConsumer<MessageEnvelopeConsumer<string>>(context);
                    });

                    /// Refer README.md for required practices to perform before and after local testing 

                    #region For Local Testing, Uncomment this and comment above two endpoints

                    //// 🔹 Endpoint 1 → CreateNotification
                    //cfg.ReceiveEndpoint("create-notification-queue-local", e =>
                    //{
                    //    e.ConcurrentMessageLimit = 1;

                    //    e.DiscardFaultedMessages();

                    //    e.PrefetchCount = 10;

                    //    e.ConfigureConsumer<CreateNotificationConsumer>(context);
                    //});

                    //// 🔹 Endpoint 2 → AnotherMessage
                    //cfg.ReceiveEndpoint("message-envelop-queue-local", e =>
                    //{
                    //    e.ConcurrentMessageLimit = 1;

                    //    e.DiscardFaultedMessages();

                    //    e.PrefetchCount = 10;

                    //    e.ConfigureConsumer<MessageEnvelopeConsumer<string>>(context);
                    //});

                    #endregion

                    // ❗ Retry applies globally (both endpoints)
                    cfg.UseMessageRetry(r =>
                        r.Interval(2, TimeSpan.FromSeconds(10)));

                });
            });

            if (Convert.ToBoolean(configuration["CacheSettings:UseInMemoryCache"]))
            {
                services.AddMemoryCache();
                services.AddTransient<IMemoryCacheService, InMemoryCacheService>();
                services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehaviour<,>));
            }

            services.AddTransient<IDateTime, DateTimeService>();
            services.AddTransient<IWebClient, WebClient>();
            services.AddTransient<IEndpoint, Endpoint>();
            services.AddSingleton<IEmailNotificationService, EmailNotificationService>();
            services.AddSingleton<IZeptoMailService, ZeptoMailService>();
            services.AddSingleton<FailurMessageObserver>();

            return services;
        }
    }
}