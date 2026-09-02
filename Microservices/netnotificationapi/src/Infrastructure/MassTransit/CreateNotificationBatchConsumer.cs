using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Application.Common.Interfaces;
using Application.Notification.Commands.SendBatchEmailUsingMicrosoftGraph;
using Application.ZeptoMail.Commands.SendBatchTransactionalEmail;
using Dapper;
using Dapper.Extensions;
using Domain.Entities;
using MassTransit;
using Messaging.Contract.Notification;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npgsql;

public class CreateNotificationBatchConsumer : IConsumer<Batch<CreateNotificationMessage>>
{
    private readonly ILogger<CreateNotificationBatchConsumer> _logger;
    private readonly IConfiguration _config;
    private readonly IEndpoint _endpoint;

    private readonly IDapper _dapper;
    private readonly string _sqlFilePath;
    private readonly Dictionary<string, string> _sqlQueries;
    private IDbConnection _dapperDbConnection { get; set; }
    private readonly IEmailNotificationService _emailNotificationService;
    public CreateNotificationBatchConsumer(ILogger<CreateNotificationBatchConsumer> logger, IConfiguration config, IEndpoint endpoint, IDapper dapper, IEmailNotificationService emailNotificationService)
    {
        _logger = logger;
        _config = config;
        _endpoint = endpoint;
        _sqlFilePath = _config["SqlSeparationQueries:PostgreNotification"];
        _sqlQueries = LoadSqlQueries();
        _dapperDbConnection = new NpgsqlConnection(_config["ConnectionStrings:AzureEventDBPostgreSqlDBConnection"]);
        this._dapper = dapper;
        _emailNotificationService = emailNotificationService;
    }

    private Dictionary<string, string> LoadSqlQueries()
    {
        string absoluteSqlFilePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _sqlFilePath));
        var xml = XElement.Load(absoluteSqlFilePath);

        return xml.Elements("sql")
                  .ToDictionary(e => e.Attribute("name").Value, e => e.Value.Trim());
    }
    public async Task Consume(ConsumeContext<Batch<CreateNotificationMessage>> context)
    {
        //await Task.Delay(10000);

        for (int i = 0; i < context.Message.Length; i++)
        {
            ConsumeContext<CreateNotificationMessage> item = context.Message[i];
            var msg = item.Message;
            int sequenceNumber = i + 1; // 1-based index

            try
            {
                if (msg.Type == "ZeptoMail")
                {
                    List<ZeptoMail> rawList = new List<ZeptoMail>();

                    //Deserialize payload
                    var notification = JsonConvert.DeserializeObject<ZeptoMail>(
                        msg.PayloadJson);

                    if (notification == null)
                    {
                        _logger.LogError(
                            "BatchIndex={Index} | Deserialization failed for MessageId={MessageId}",
                            sequenceNumber,
                            item.MessageId);

                        continue;
                    }

                    rawList.Add(notification);

                    await GetNotificationRequestsForZeptoAsync(rawList);
                }

                else if (msg.Type == "MSGraph")
                {
                    List<PostgreNotification> rawList = new List<PostgreNotification>();

                    //Deserialize payload
                    var notification = JsonConvert.DeserializeObject<PostgreNotification>(
                        msg.PayloadJson);

                    if (notification == null)
                    {
                        _logger.LogError(
                            "BatchIndex={Index} | Deserialization failed for MessageId={MessageId}",
                            sequenceNumber,
                            item.MessageId);

                        continue;
                    }

                    rawList.Add(notification);

                    await GetNotificationRequestsForMicrosoftGraphAsync(rawList);
                }

                _logger.LogInformation(
                        "BatchIndex={Index} | CreateNotificationConsumer processed MessageId={MessageId}",
                        sequenceNumber,
                        item.MessageId);
            }

            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "BatchIndex={Index} | CreateNotificationConsumer failed MessageId={MessageId}",
                    sequenceNumber,
                    item.MessageId);
            }
        }

        //var json = JsonConvert.SerializeObject(rawList);
        //_logger.LogInformation($"ZeptoList For Consumer: {json}");
    }

    public async Task GetNotificationRequestsForZeptoAsync(List<ZeptoMail> rawList)
    {
        try
        {
            var insertNotificationResponseQuery = _sqlQueries["PostgreNotification.InsertZeptoMailResponse"];

            var requestObject = new SendBatchTransactionalEmailRequest
            {
                ZeptoMails = rawList
            };

            if (requestObject.ZeptoMails?.Any() == true)
            {
                _logger.LogInformation("Consumer reached HttpPostRequest");

                //Send Notification
                var response = await _endpoint.HttpPostRequest(_config["ServerURL:Notification"], _config["EndPoint:SendBatchTransactionalEmail"], requestObject);

                _logger.LogInformation("Consumer returned from HttpPostRequest");

                var repsonseJson = await response.Content.ReadAsStringAsync();
                var processedNotifications = JsonConvert.DeserializeObject<List<Domain.Entities.ZeptoMail>>(repsonseJson);

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
                            #endregion
                        }

                        catch (Exception ex)
                        {
                            _logger.LogError("CreateNotificationConsumer.GetNotificationRequestsForZeptoAsync - " + ex.Message);
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

    public async Task GetNotificationRequestsForMicrosoftGraphAsync(List<PostgreNotification> notificationList)
    {
        try
        {
            //var notificationRequestQuery = _sqlQueries["PostgreNotification.GetNotificationRequests"];
            var insertNotificationResponseQuery = _sqlQueries["PostgreNotification.InsertNotificationResponse"];

            var requestObject = new SendBatchEmailUsingMicrosoftGraphRequest
            {
                notificationList = notificationList
            };

            if (requestObject.notificationList?.Any() == true)
            {
                //Send Notification
                var response = await _endpoint.HttpPostRequest(_config["ServerURL:Notification"], _config["EndPoint:SendBatchEmailUsingMicrosoftGraph"], requestObject);
                var repsonseJson = await response.Content.ReadAsStringAsync();
                var processedNotifications = JsonConvert.DeserializeObject<List<Domain.Entities.PostgreNotification>>(repsonseJson);

                //using var stream = await response.Content.ReadAsStreamAsync();
                //using var reader = new StreamReader(stream);
                //using var jsonReader = new JsonTextReader(reader);

                //var serializer = new JsonSerializer();
                //var processedNotifications = serializer.Deserialize<List<PostgreNotification>>(jsonReader);

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
                            #endregion

                            #region SSE Code (Not used for now)
                            //    if (!string.IsNullOrWhiteSpace(notification.NotificationErrorMessage) &&
                            //        !notification.NotificationErrorMessage.Contains("Notification sent successfully 2.0.0 OK", StringComparison.OrdinalIgnoreCase))
                            //    {
                            //        var param = new InSystemNotification
                            //        {
                            //            Message = notification.NotificationErrorMessage,
                            //            CreatedDateTime = DateTime.UtcNow,
                            //            CreatedBy = notification.CreatedBy,
                            //            CreatedById = notification.CreatedById,
                            //            UpdatedBy = notification.UpdatedBy,
                            //            UpdatedById = notification.UpdatedById,
                            //            UpdatedDateTime = notification.UpdatedDateTime,
                            //            UpdateReason = notification.UpdateReason,
                            //            OwnerId = notification.OwnerId,
                            //            IsActive = notification.IsActive,
                            //            IsDeleted = notification.IsDeleted,
                            //            IsApproved = notification.IsApproved,
                            //            ApproverId = notification.ApproverId,
                            //            ApprovedDateTime = notification.ApprovedDateTime,
                            //            IsAuthorized = notification.IsAuthorized,
                            //            AuthorizedById = notification.AuthorizedById,
                            //            AuthorizedDateTime = notification.AuthorizedDateTime,
                            //            SysData = notification.SysData,
                            //            TenantId = notification.TenantId,
                            //            AssociatedUserId = notification.AssociatedUserId,
                            //            SubTenantId = notification.SubTenantId
                            //        };

                            //        // 🔥 Broadcast via SSE
                            //        //_emailNotificationService.Broadcast(JsonConvert.SerializeObject(param));
                            //        await _emailNotificationService.SaveAndBroadcastAsync(param);
                            //    }
                            //}
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "CreateNotificationConsumer.GetNotificationRequestsForMicrosoftGraphAsync failed");
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
