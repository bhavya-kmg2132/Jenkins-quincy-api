using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Application.Common.Interfaces;
using Application.Notification.Commands.SendBatchEmailUsingMicrosoftGraph;
using Dapper;
using Dapper.Extensions;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npgsql;
namespace Api.Services.Notification
{
    public class NotificationWorkerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationWorkerService> _logger;
        private readonly IConfiguration _config;
        private readonly IEndpoint _endpoint;

        private readonly IDapper _dapper;
        private readonly string _sqlFilePath;
        private readonly Dictionary<string, string> _sqlQueries;
        private IDbConnection _dapperDbConnection { get; set; }
        private readonly IEmailNotificationService _emailNotificationService;

        // 300000 ms = 300 s = 5 min
        // 120000 ms = 120 s = 2 min
        // 60000 ms = 60 s = 1 min
        // 30000 ms = 30 s
        // 20000 ms = 20 s
        //private readonly System.Int32 BackgroungJobExecutionDelayInMilliSeconds = 300000;

        private readonly int BackgroungJobExecutionDelayInMilliSeconds = 300000;

        public NotificationWorkerService(IServiceProvider serviceProvider, ILogger<NotificationWorkerService> logger, IConfiguration config, IEndpoint endpoint, IDapper dapper, IEmailNotificationService emailNotificationService)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _config = config;
            _endpoint = endpoint;
            _sqlFilePath = _config["SqlSeparationQueries:PostgreNotification"];
            _sqlQueries = LoadSqlQueries();
            _dapperDbConnection = new NpgsqlConnection(_config["ConnectionStrings:NotificationPostgreSqlDBConnection"]);
            this._dapper = dapper;
            _emailNotificationService = emailNotificationService;
            //SqlConnectionStringBuilder sqlconnectionbuilder = new SqlConnectionStringBuilder(_config["ConnectionStrings:SqlDBConnection"]);
        }

        private Dictionary<string, string> LoadSqlQueries()
        {
            string absoluteSqlFilePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _sqlFilePath));
            var xml = XElement.Load(absoluteSqlFilePath);

            return xml.Elements("sql")
                      .ToDictionary(e => e.Attribute("name").Value, e => e.Value.Trim());
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (IServiceScope scope = _serviceProvider.CreateScope())
                    {
                        //Task 1
                        // await InsertApiWorkerServiceLog();
                    }

                    //Task 2
                    if (Convert.ToBoolean(_config["SendNotificationFromDb"]))
                    {
                        try
                        {
                            //Testing code
                            const int batchSize = 100;
                            var notificationRequestQuery = _sqlQueries["PostgreNotification.GetNotificationRequests"];

                            while (true)
                            {
                                // 1️ Read a small batch
                                var rawList = (await _dapperDbConnection.QueryAsync<PostgreNotificationCrud>(
                                    notificationRequestQuery,
                                    new { BatchSize = batchSize }
                                )).ToList();

                                if (!rawList.Any())
                                    break;

                                // 2️ Convert to your domain objects 
                                var notificationList = rawList.Select(x => new PostgreNotification
                                {
                                    Id = x.Id,
                                    ApiKey = x.ApiKey,
                                    EmailFrom = x.EmailFrom,
                                    EmailTo = x.EmailTo,
                                    EmailCc = x.EmailCc,
                                    EmailBcc = x.EmailBcc,
                                    EmailSubject = x.EmailSubject,
                                    EmailBody = x.EmailBody,
                                    NotificationErrorMessage = x.NotificationErrorMessage,
                                    ScheduledDateTime = x.ScheduledDateTime,
                                    NotificationResponseDateTime = x.NotificationResponseDateTime,

                                    EmailAttachments = string.IsNullOrEmpty(x.EmailAttachments)
                                        ? new List<EmailAttachment>()
                                        : JsonConvert.DeserializeObject<List<EmailAttachment>>(x.EmailAttachments),

                                    NotificationDelivery = string.IsNullOrEmpty(x.NotificationDelivery)
                                        ? new NotificationStatus()
                                        : JsonConvert.DeserializeObject<NotificationStatus>(x.NotificationDelivery),

                                    EntityJson = string.IsNullOrEmpty(x.EntityJson)
                                        ? new NotificationEntityJson()
                                        : JsonConvert.DeserializeObject<NotificationEntityJson>(x.EntityJson),

                                    CreatedBy = x.CreatedBy,
                                    CreatedById = x.CreatedById,
                                    CreatedDateTime = x.CreatedDateTime,
                                    UpdatedBy = x.UpdatedBy,
                                    UpdatedById = x.UpdatedById,
                                    UpdatedDateTime = x.UpdatedDateTime,
                                    UpdateReason = x.UpdateReason,
                                    OwnerId = x.OwnerId,
                                    IsActive = x.IsActive,
                                    IsDeleted = x.IsDeleted,
                                    IsApproved = x.IsApproved,
                                    ApproverId = x.ApproverId,
                                    ApprovedDateTime = x.ApprovedDateTime,
                                    IsAuthorized = x.IsAuthorized,
                                    AuthorizedById = x.AuthorizedById,
                                    AuthorizedDateTime = x.AuthorizedDateTime,
                                    SysData = x.SysData,
                                    TenantId = x.TenantId,
                                    AssociatedUserId = x.AssociatedUserId,
                                    SubTenantId = x.SubTenantId
                                }).ToList();

                                await GetNotificationRequestsAsync(notificationList);

                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("NotificationWorkerService Exception at: {time}: {exception}", DateTime.UtcNow.ToString(), ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("NotificationWorkerService Exception at: {time}: {exception}", DateTime.UtcNow.ToString(), ex.Message);
                }

                await Task.Delay(BackgroungJobExecutionDelayInMilliSeconds, stoppingToken);
            }
        }
        public async Task GetNotificationRequestsAsync(List<PostgreNotification> notificationList)
        {
            try
            {
                //var notificationRequestQuery = _sqlQueries["PostgreNotification.GetNotificationRequests"];
                var insertNotificationResponseQuery = _sqlQueries["PostgreNotification.InsertNotificationResponse"];
                var removeNotificationRequestByIdQuery = _sqlQueries["PostgreNotification.DeleteNotificationRequestById"];

                //var rawList = (await _dapperDbConnection.QueryAsync<PostgreNotificationCrud>(notificationRequestQuery, null)).ToList();

                //// Deserialize JSON fields manually
                //var notificationList = rawList.Select(x => new PostgreNotification
                //{
                //    Id = x.Id,
                //    ApiKey = x.ApiKey,
                //    EmailFrom = x.EmailFrom,
                //    EmailTo = x.EmailTo,
                //    EmailCc = x.EmailCc,
                //    EmailBcc = x.EmailBcc,
                //    EmailSubject = x.EmailSubject,
                //    EmailBody = x.EmailBody,
                //    NotificationErrorMessage = x.NotificationErrorMessage,
                //    ScheduledDateTime = x.ScheduledDateTime,
                //    NotificationResponseDateTime = x.NotificationResponseDateTime,

                //    EmailAttachments = string.IsNullOrEmpty(x.EmailAttachments)
                //        ? new List<EmailAttachment>()
                //        : JsonConvert.DeserializeObject<List<EmailAttachment>>(x.EmailAttachments),

                //    NotificationDelivery = string.IsNullOrEmpty(x.NotificationDelivery)
                //        ? new NotificationStatus()
                //        : JsonConvert.DeserializeObject<NotificationStatus>(x.NotificationDelivery),

                //    EntityJson = string.IsNullOrEmpty(x.EntityJson)
                //        ? new NotificationEntityJson()
                //        : JsonConvert.DeserializeObject<NotificationEntityJson>(x.EntityJson),
                //    CreatedBy = x.CreatedBy,
                //    CreatedById = x.CreatedById,
                //    CreatedDateTime = x.CreatedDateTime == default ? DateTime.UtcNow : x.CreatedDateTime,
                //    UpdatedBy = x.UpdatedBy,
                //    UpdatedById = x.UpdatedById,
                //    UpdatedDateTime = x.UpdatedDateTime == default ? DateTime.UtcNow : x.UpdatedDateTime,
                //    UpdateReason = x.UpdateReason,
                //    OwnerId = x.OwnerId,
                //    IsActive = x.IsActive,
                //    IsDeleted = x.IsDeleted,
                //    IsApproved = x.IsApproved,
                //    ApproverId = x.ApproverId,
                //    ApprovedDateTime = x.ApprovedDateTime,
                //    IsAuthorized = x.IsAuthorized,
                //    AuthorizedById = x.AuthorizedById,
                //    AuthorizedDateTime = x.AuthorizedDateTime,
                //    SysData = x.SysData,
                //    TenantId = x.TenantId,
                //    AssociatedUserId = x.AssociatedUserId,
                //    SubTenantId = x.SubTenantId
                //}).ToList();

                var now = DateTime.UtcNow;

                var filtered = notificationList
                    .Where(n => !n.ScheduledDateTime.HasValue
                             || (DateTime.SpecifyKind(n.ScheduledDateTime.Value, DateTimeKind.Utc) <= now.AddMinutes(35)
                                 && DateTime.SpecifyKind(n.ScheduledDateTime.Value, DateTimeKind.Utc) >= now))
                    .ToList();

                var requestObject = new SendBatchEmailUsingMicrosoftGraphRequest
                {
                    notificationList = filtered
                };

                if (requestObject.notificationList?.Any() == true)
                {
                    //Send Notification
                    var response = await _endpoint.HttpPostRequest(_config["ServerURL:Notification"], _config["EndPoint:SendBatchEmailUsingMicrosoftGraph"], requestObject);
                    //var repsonseJson = await response.Content.ReadAsStringAsync();
                    //var processedNotifications = JsonConvert.DeserializeObject<List<Domain.Entities.PostgreNotification>>(repsonseJson);

                    using var stream = await response.Content.ReadAsStreamAsync();
                    using var reader = new StreamReader(stream);
                    using var jsonReader = new JsonTextReader(reader);

                    var serializer = new JsonSerializer();
                    var processedNotifications = serializer.Deserialize<List<PostgreNotification>>(jsonReader);

                    if (processedNotifications != null)
                    {
                        foreach (var notification in processedNotifications)
                        {
                            try
                            {
                                #region Insert Notification To PostgreDb 
                                var parameters = new
                                {
                                    Id = notification.Id,
                                    ApiKey = notification.ApiKey,
                                    CreatedDateTime = notification.CreatedDateTime,
                                    UpdatedDateTime = notification.UpdatedDateTime,
                                    EmailFrom = notification.EmailFrom,
                                    EmailTo = notification.EmailTo,
                                    EmailCc = notification.EmailCc,
                                    EmailBcc = notification.EmailBcc,
                                    EmailSubject = notification.EmailSubject,
                                    EmailBody = notification.EmailBody,
                                    EmailAttachments = JsonConvert.SerializeObject(notification.EmailAttachments ?? new List<EmailAttachment>()),
                                    notificationErrorMessage = notification.NotificationErrorMessage,
                                    NotificationDelivery = JsonConvert.SerializeObject(notification.NotificationDelivery ?? new NotificationStatus()),
                                    ScheduledDateTime = notification.ScheduledDateTime,
                                    NotificationResponseDateTime = notification.NotificationResponseDateTime,
                                    EntityJson = JsonConvert.SerializeObject(notification.EntityJson ?? new NotificationEntityJson()),
                                    CreatedBy = notification.CreatedBy,
                                    CreatedById = notification.CreatedById,
                                    UpdatedBy = notification.UpdatedBy,
                                    UpdatedById = notification.UpdatedById,
                                    UpdateReason = notification.UpdateReason,
                                    OwnerId = notification.OwnerId,
                                    IsActive = notification.IsActive,
                                    IsDeleted = notification.IsDeleted,
                                    IsApproved = notification.IsApproved,
                                    ApproverId = notification.ApproverId,
                                    ApprovedDateTime = notification.ApprovedDateTime,
                                    IsAuthorized = notification.IsAuthorized,
                                    AuthorizedById = notification.AuthorizedById,
                                    AuthorizedDateTime = notification.AuthorizedDateTime,
                                    SysData = notification.SysData,
                                    TenantId = notification.TenantId,
                                    AssociatedUserId = notification.AssociatedUserId,
                                    SubTenantId = notification.SubTenantId
                                };
                                var rowsAffected = await _dapperDbConnection.ExecuteAsync(insertNotificationResponseQuery, parameters);
                                var removedNotificationRequest = await _dapperDbConnection.ExecuteAsync(removeNotificationRequestByIdQuery, new { Id = notification.Id });
                                #endregion

                                if (!string.IsNullOrWhiteSpace(notification.NotificationErrorMessage) &&
                                    !notification.NotificationErrorMessage.Contains("Notification sent successfully 2.0.0 OK", StringComparison.OrdinalIgnoreCase))
                                {
                                    var param = new InSystemNotification
                                    {
                                        Message = notification.NotificationErrorMessage,
                                        CreatedDateTime = DateTime.UtcNow,
                                        CreatedBy = notification.CreatedBy,
                                        CreatedById = notification.CreatedById,
                                        UpdatedBy = notification.UpdatedBy,
                                        UpdatedById = notification.UpdatedById,
                                        UpdatedDateTime = notification.UpdatedDateTime,
                                        UpdateReason = notification.UpdateReason,
                                        OwnerId = notification.OwnerId,
                                        IsActive = notification.IsActive,
                                        IsDeleted = notification.IsDeleted,
                                        IsApproved = notification.IsApproved,
                                        ApproverId = notification.ApproverId,
                                        ApprovedDateTime = notification.ApprovedDateTime,
                                        IsAuthorized = notification.IsAuthorized,
                                        AuthorizedById = notification.AuthorizedById,
                                        AuthorizedDateTime = notification.AuthorizedDateTime,
                                        SysData = notification.SysData,
                                        TenantId = notification.TenantId,
                                        AssociatedUserId = notification.AssociatedUserId,
                                        SubTenantId = notification.SubTenantId
                                    };

                                    // 🔥 Broadcast via SSE
                                    //_emailNotificationService.Broadcast(JsonConvert.SerializeObject(param));
                                    await _emailNotificationService.SaveAndBroadcastAsync(param);
                                }
                            }

                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "NotificationWorkerService.GetNotificationRequestsAsync failed");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddBackgroundJobLog for SMTP (NotificationFetch) failed");
            }
        }
    }
}
