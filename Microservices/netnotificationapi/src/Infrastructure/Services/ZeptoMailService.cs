using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Application.Common.Interfaces;
using Dapper.Extensions;
using Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using Task = System.Threading.Tasks.Task;

namespace Infrastructure.Services
{
    public class ZeptoMailService : IZeptoMailService
    {
        private readonly IDomainEventService _domainEventService;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private IDbConnection _dapperDbConnection { get; set; }
        private IDapper dapper;
        private readonly string _sqlFilePath;
        private readonly Dictionary<string, string> _sqlQueries;
        Dictionary<string, (string SenderName, string SenderEmail, string AuthToken, string AgentName)> zeptoMailSettingsDict = new Dictionary<string, (string, string, string, string)>();
        public string defaultSenderName = string.Empty;
        public string defaultSenderEmail = string.Empty;
        public string authToken = string.Empty;
        public readonly int _maxParallelism = 20;
        public readonly int _retryCount = 3;
        public readonly int _delayBetweenBatchesMs = 100;

        public ZeptoMailService(IDomainEventService domainEventService, IConfiguration configuration, ILogger logger, IDapper dapper)
        {
            _domainEventService = domainEventService;
            _configuration = configuration;
            _logger = logger;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.zeptomail.in/v1.1/email")
            };
            this.dapper = dapper;
            _sqlFilePath = _configuration["SqlSeparationQueries:ZohoCampaign"];
            _sqlQueries = LoadSqlQueries();
            _dapperDbConnection = new NpgsqlConnection(_configuration["ConnectionStrings:PostgreSqlDBConnection"]);
            this.dapper = dapper;
            SqlConnectionStringBuilder sqlconnectionbuilder = new SqlConnectionStringBuilder(_configuration["ConnectionStrings:SqlDBConnection"]);

            var valuesSection = _configuration.GetSection("ZeptoMailSettings");
            foreach (IConfigurationSection section in valuesSection.GetChildren())
            {
                var key = section.GetValue<string>("ApiKey");
                zeptoMailSettingsDict.Add(key, (section.GetValue<string>("SenderName"), section.GetValue<string>("SenderEmail"), section.GetValue<string>("AuthToken"), section.GetValue<string>("AgentName")));
            }
        }

        private Dictionary<string, string> LoadSqlQueries()
        {
            string absoluteSqlFilePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _sqlFilePath));
            var xml = XElement.Load(absoluteSqlFilePath);

            return xml.Elements("sql")
                      .ToDictionary(e => e.Attribute("name").Value, e => e.Value.Trim());
        }

        public async Task<ZeptoMail> SendEmailAsync(ZeptoMail zeptoMail)
        {
            try
            {
                if (zeptoMailSettingsDict.TryGetValue(zeptoMail.ApiKey, out var dictValue))
                {
                    // SenderName found and assigned
                    zeptoMail.SenderName = dictValue.SenderName;
                    zeptoMail.SenderEmail = dictValue.SenderEmail;
                    authToken = dictValue.AuthToken;
                }
                else
                {
                    _logger.LogError("ZeptoMailService.SendEmailAsync:ZeptoMail Credentials Missing.");
                }

                var attachmentList = new List<object>();
                if (zeptoMail.Attachments != null)
                {
                    foreach (var attachment in zeptoMail.Attachments)
                    {
                        //var fileBytes = attachment.Content;
                        //string mimeType = attachment.ContentType;

                        attachmentList.Add(new
                        {
                            name = attachment.FileName,
                            mime_type = attachment.ContentType,
                            content = attachment.Content
                        });
                    }
                }

                var payload = new
                {
                    from = new { address = zeptoMail.SenderEmail, name = zeptoMail.SenderName },
                    to = zeptoMail.To,
                    cc = zeptoMail.Cc != null && zeptoMail.Cc.Count > 0 ? zeptoMail.Cc : null,
                    bcc = zeptoMail.Bcc != null && zeptoMail.Bcc.Count > 0 ? zeptoMail.Bcc : null,
                    subject = zeptoMail.Subject,
                    htmlbody = zeptoMail.HtmlBody,
                    attachments = attachmentList.Count > 0 ? attachmentList : null
                };

                string json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Zoho-enczapikey {authToken}");
                var response = await _httpClient.PostAsync("email", content);
                string responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"[ZeptoMail] Status: {response.StatusCode}");
                _logger.LogInformation($"[ZeptoMail] Response: {responseBody}");

                var jObj = JObject.Parse(responseBody);

                string message = jObj["message"]?.ToString();  // "OK"
                string innerMessage = jObj["data"]?[0]?["message"]?.ToString(); // "Email request received"

                #region check status and update status 
                zeptoMail.NotificationDelivery = new NotificationStatus();
                if (message.Contains("OK"))
                {
                    zeptoMail.NotificationResponseDateTime = DateTime.UtcNow;
                    zeptoMail.NotificationDelivery.isDelivered = true;
                    zeptoMail.NotificationDelivery.DeliveryReport = responseBody;
                    zeptoMail.NotificationErrorMessage = "Notification sent successfully 2.0.0 OK";
                    return zeptoMail;
                }
                else
                {
                    zeptoMail.NotificationResponseDateTime = DateTime.UtcNow;
                    zeptoMail.NotificationDelivery.isDelivered = false;
                    zeptoMail.NotificationDelivery.DeliveryReport = responseBody;
                    zeptoMail.NotificationErrorMessage = $"{responseBody.ToString()}";
                    return zeptoMail;
                    // throw new ApplicationException($"Error:mail not sent.");
                }
                #endregion
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ZeptoMail] Error: {ex.Message}");
                return new ZeptoMail { NotificationErrorMessage = ex.Message };
            }
        }

        public async Task<List<ZeptoMail>> SendBatchEmailAsync(List<ZeptoMail> zeptoMails)
        {
            #region Last Code
            //    int nextBatch = 0;
            //    var groupedNotifications = zeptoMails
            //      .GroupBy(n => n.ApiKey) // Group by SenderEmail, SenderName and UpdateStatus
            //      .ToDictionary(
            //          group => group.Key,   // Key: Tuple (SenderEmail, SenderName, UpdateStatus)
            //          group => group.ToList() // Value: List of BrevoNotifications with the same SenderEmail & SenderName
            //      );

            //    using var semaphore = new SemaphoreSlim(_maxParallelism);
            //    var tasks = new List<Task>();

            //    foreach (var recipient in groupedNotifications.Values)
            //    {
            //        if (zeptoMailSettingsDict.TryGetValue(recipient.Select(x => x.ApiKey).ToString(), out var dictValue))
            //        {
            //            // SenderName found and assigned
            //            defaultSenderName = dictValue.SenderName;
            //            defaultSenderEmail = dictValue.SenderEmail;
            //            authToken = dictValue.AuthToken;
            //        }
            //        else
            //        {
            //            _logger.LogError("ZeptoMailService.SendEmailAsync:ZeptoMail Credentials Missing.");
            //            continue;
            //        }

            //        await semaphore.WaitAsync();

            //        tasks.Add(Task.Run(async () =>
            //        {
            //            try
            //            {
            //                var zeptoMail = await SendSingleEmailWithRetryAsync(recipient);
            //            }
            //            finally
            //            {
            //                semaphore.Release();
            //                await Task.Delay(_delayBetweenBatchesMs);
            //            }
            //        }));

            //        nextBatch++;
            //    }

            //    await Task.WhenAll(tasks);
            #endregion

            var groupedByApiKey = zeptoMails
                                 .GroupBy(z => z.ApiKey)
                                 .ToDictionary(g => g.Key, g => g.ToList());

            // Loop 1: process groups one by one
            foreach (var (apiKey, mailList) in groupedByApiKey)
            {
                if (!zeptoMailSettingsDict.TryGetValue(apiKey, out var settings))
                {
                    _logger.LogError($"ZeptoMail credentials missing for API Key: {apiKey}");
                    continue;
                }

                // Capture credentials once per group
                var authToken = settings.AuthToken;
                //var senderName = settings.SenderName;
                //var senderEmail = settings.SenderEmail;

                _logger.LogInformation($"Starting mails for Agent: {settings.AgentName}, ({mailList.Count} mails)");

                //Loop 2: process mails inside this group (optionally parallel)
                //var mailTasks = mailList.Select(mail =>
                //    SendSingleEmailWithRetryAsync(mail, authToken, senderName, senderEmail)
                //);

                var mailTasks = mailList.Select(mail =>
                {
                    mail.SenderEmail = settings.SenderEmail;
                    mail.SenderName = settings.SenderName;

                    return SendSingleEmailWithRetryAsync(mail, authToken);
                });

                await Task.WhenAll(mailTasks);

                _logger.LogInformation($"Completed mails for Agent: {settings.AgentName}, Sender: {settings.SenderEmail}");
            }

            return zeptoMails;
        }

        private async Task<ZeptoMail> SendSingleEmailWithRetryAsync(ZeptoMail zeptoMail, string authToken)
        {
            int attempt = 0;
            bool success = false;
            zeptoMail.NotificationDelivery = new NotificationStatus();

            while (!success && attempt < _retryCount)
            {
                attempt++;
                try
                {
                    var attachmentList = new List<object>();
                    if (zeptoMail.Attachments != null)
                    {
                        foreach (var attachment in zeptoMail.Attachments)
                        {
                            //var fileBytes = attachment.Content;
                            //string mimeType = attachment.ContentType;

                            attachmentList.Add(new
                            {
                                name = attachment.FileName,
                                mime_type = attachment.ContentType,
                                content = attachment.Content
                            });
                        }
                    }

                    var payload = new
                    {
                        from = new { address = zeptoMail.SenderEmail, name = zeptoMail.SenderName },
                        to = zeptoMail.To,
                        cc = zeptoMail.Cc != null && zeptoMail.Cc.Count > 0 ? zeptoMail.Cc : null,
                        bcc = zeptoMail.Bcc != null && zeptoMail.Bcc.Count > 0 ? zeptoMail.Bcc : null,
                        subject = zeptoMail.Subject,
                        htmlbody = zeptoMail.HtmlBody,
                        attachments = attachmentList.Count > 0 ? attachmentList : null
                    };

                    string json = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    _httpClient.DefaultRequestHeaders.Clear();
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Zoho-enczapikey {authToken}");
                    var response = await _httpClient.PostAsync("email", content);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    var jObj = JObject.Parse(responseBody);
                    string message = jObj["message"]?.ToString();  // "OK"
                    string innerMessage = jObj["data"]?[0]?["message"]?.ToString(); // "Email request received"

                    string recipientsEmailList = string.Join(",",
                        zeptoMail.To.Select(x => JsonConvert.SerializeObject(x.email_address))
                    );

                    _logger.LogInformation($"[{recipientsEmailList}] StatusCode ({response.StatusCode})");

                    if (response.IsSuccessStatusCode)
                    {
                        success = true;

                        if (message.Contains("OK"))
                        {
                            zeptoMail.NotificationResponseDateTime = DateTime.UtcNow;
                            zeptoMail.NotificationDelivery.isDelivered = true;
                            zeptoMail.NotificationDelivery.DeliveryReport = responseBody;
                            zeptoMail.NotificationErrorMessage = "Notification sent successfully 2.0.0 OK";
                        }
                        else
                        {
                            zeptoMail.NotificationResponseDateTime = DateTime.UtcNow;
                            zeptoMail.NotificationDelivery.isDelivered = false;
                            zeptoMail.NotificationDelivery.DeliveryReport = responseBody;
                            zeptoMail.NotificationErrorMessage = $"{responseBody.ToString()}";
                        }
                    }
                    else
                    {
                        _logger.LogError($"⚠️ Attempt {attempt} failed ({response.StatusCode}) → {responseBody.ToString()}");
                        zeptoMail.NotificationResponseDateTime = DateTime.UtcNow;
                        zeptoMail.NotificationDelivery.isDelivered = false;
                        zeptoMail.NotificationDelivery.DeliveryReport = responseBody;
                        zeptoMail.NotificationErrorMessage = $" Attempt {attempt} failed ({response.StatusCode}) → {responseBody.ToString()}";

                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Exception on attempt {attempt}: {ex.Message}");
                    zeptoMail.NotificationErrorMessage = $"Exception on attempt {attempt}: {ex.Message}";
                    zeptoMail.NotificationResponseDateTime = DateTime.UtcNow;
                    zeptoMail.NotificationDelivery.isDelivered = false;
                }

                if (!success && attempt < _retryCount)
                {
                    await Task.Delay(500 * attempt); // exponential backoff
                }
            }

            if (!success)
            {
                _logger.LogError($"🚫 Failed after {_retryCount} attempts");
                zeptoMail.NotificationErrorMessage = $" Failed after {_retryCount} attempts. Exception occured:" +
                                                       (zeptoMail.NotificationErrorMessage ?? string.Empty);
                zeptoMail.NotificationResponseDateTime = DateTime.UtcNow;
                zeptoMail.NotificationDelivery.isDelivered = false;
            }

            return zeptoMail;
        }

        public async System.Threading.Tasks.Task DispatchEvents(ZeptoMail entity)
        {
            while (true)
            {
                var domainEventEntity = entity.DomainEvents
                    .Where(domainEvent => !domainEvent.IsPublished)
                    .FirstOrDefault();
                if (domainEventEntity == null) break;

                domainEventEntity.IsPublished = true;
                await _domainEventService.Publish(domainEventEntity);
            }
        }
    }

}

