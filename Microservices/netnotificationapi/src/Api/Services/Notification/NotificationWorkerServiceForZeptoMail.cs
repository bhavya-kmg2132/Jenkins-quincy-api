using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Application.Common.Interfaces;
using Application.ZeptoMail.Commands.SendBatchTransactionalEmail;
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
    public class NotificationWorkerServiceForZeptoMail : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationWorkerServiceForZeptoMail> _logger;
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

        public NotificationWorkerServiceForZeptoMail(IServiceProvider serviceProvider, ILogger<NotificationWorkerServiceForZeptoMail> logger, IConfiguration config, IEndpoint endpoint, IDapper dapper, IEmailNotificationService emailNotificationService)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _config = config;
            _endpoint = endpoint;
            _sqlFilePath = _config["SqlSeparationQueries:PostgreNotification"];
            _sqlQueries = LoadSqlQueries();
            //_dapperDbConnection = new NpgsqlConnection(_config["ConnectionStrings:NotificationPostgreSqlDBConnection"]);
            _dapperDbConnection = new NpgsqlConnection(_config["ConnectionStrings:AzureEventDBPostgreSqlDBConnection"]);
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
                    if (Convert.ToBoolean(_config["SendNotificationFromDbForZepto"]))
                    {
                        try
                        {
                            await GetNotificationRequestsAsync();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("NotificationWorkerServiceForZeptoMail Exception at: {time}: {exception}", DateTime.UtcNow.ToString(), ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("NotificationWorkerServiceForZeptoMail Exception at: {time}: {exception}", DateTime.UtcNow.ToString(), ex.Message);
                }

                await Task.Delay(BackgroungJobExecutionDelayInMilliSeconds, stoppingToken);
            }
        }
        public async Task GetNotificationRequestsAsync()
        {
            try
            {

                var notificationRequestQuery = _sqlQueries["PostgreNotification.GetZeptoMailRequests"];
                var insertNotificationResponseQuery = _sqlQueries["PostgreNotification.InsertZeptoMailResponse"];
                var removeNotificationRequestByIdQuery = _sqlQueries["PostgreNotification.DeleteZeptoMailRequestById"];

                var rawList = (await _dapperDbConnection.QueryAsync<ZeptoMailCrud>(notificationRequestQuery, null)).ToList();

                // Deserialize JSON fields manually
                var notificationList = rawList.Select(x => new ZeptoMail
                {
                    Id = x.Id,
                    ApiKey = x.ApiKey,
                    SenderEmail = x.SenderEmail,
                    SenderName = x.SenderName,

                    To = string.IsNullOrEmpty(x.To)
                         ? new List<ZeptoMailRecipient>()
                         : JsonConvert.DeserializeObject<List<ZeptoMailRecipient>>(x.To),

                    Cc = string.IsNullOrEmpty(x.Cc)
                         ? new List<ZeptoMailRecipient>()
                         : JsonConvert.DeserializeObject<List<ZeptoMailRecipient>>(x.Cc),

                    Bcc = string.IsNullOrEmpty(x.Bcc)
                          ? new List<ZeptoMailRecipient>()
                          : JsonConvert.DeserializeObject<List<ZeptoMailRecipient>>(x.Bcc),

                    Subject = x.Subject,
                    HtmlBody = x.HtmlBody,
                    TextBody = x.TextBody,

                    Attachments = string.IsNullOrEmpty(x.Attachments)
                        ? new List<ZeptoMailAttachment>()
                        : JsonConvert.DeserializeObject<List<ZeptoMailAttachment>>(x.Attachments),
                    NotificationErrorMessage = x.NotificationErrorMessage,
                    NotificationDelivery = string.IsNullOrEmpty(x.NotificationDelivery)
                        ? new NotificationStatus()
                        : JsonConvert.DeserializeObject<NotificationStatus>(x.NotificationDelivery),

                    NotificationResponseDateTime = x.NotificationResponseDateTime,
                    CreatedBy = x.CreatedBy,
                    CreatedById = x.CreatedById,
                    CreatedDateTime = x.CreatedDateTime == default ? DateTime.UtcNow : x.CreatedDateTime,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedById = x.UpdatedById,
                    UpdatedDateTime = x.UpdatedDateTime == default ? DateTime.UtcNow : x.UpdatedDateTime,
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

                var requestObject = new SendBatchTransactionalEmailRequest
                {
                    ZeptoMails = notificationList // Ensure batch is a List<ZeptoMail>
                };

                if (requestObject.ZeptoMails.Any())
                {
                    //Send Notification
                    var response = await _endpoint.HttpPostRequest(_config["ServerURL:Notification"], _config["EndPoint:SendBatchTransactionalEmail"], requestObject);

                    var repsonseJson = await response.Content.ReadAsStringAsync();
                    var processedNotifications = JsonConvert.DeserializeObject<List<Domain.Entities.ZeptoMail>>(repsonseJson);

                    var responseText = await response.Content.ReadAsStringAsync();
                    var responseContent = responseText.Split(',');

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
                                    SenderEmail = notification.SenderEmail,
                                    SenderName = notification.SenderName,
                                    To = JsonConvert.SerializeObject(notification.To ?? new List<ZeptoMailRecipient>()),
                                    Cc = JsonConvert.SerializeObject(notification.Cc ?? new List<ZeptoMailRecipient>()),
                                    Bcc = JsonConvert.SerializeObject(notification.Bcc ?? new List<ZeptoMailRecipient>()),
                                    Subject = notification.Subject,
                                    HtmlBody = notification.HtmlBody,
                                    TextBody = notification.TextBody,
                                    Attachments = JsonConvert.SerializeObject(notification.Attachments ?? new List<ZeptoMailAttachment>()),
                                    NotificationErrorMessage = notification.NotificationErrorMessage,
                                    NotificationDelivery = JsonConvert.SerializeObject(notification.NotificationDelivery ?? new NotificationStatus()),
                                    NotificationResponseDateTime = notification.NotificationResponseDateTime,
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
                            }

                            catch (Exception ex)
                            {
                                _logger.LogError("NotificationWorkerServiceForZeptoMail.GetNotificationRequestsAsync - " + ex.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("AddBackgroundJobLog for SMTP (NotificationFetch) - " + ex.Message);
            }
        }
    }
}
