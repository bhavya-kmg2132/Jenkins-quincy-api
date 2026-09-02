using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml.Linq;
using Application.Common.Interfaces;
using Azure.Identity;
using Dapper;
using Dapper.Extensions;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;
using MimeKit;
using MimeKit.Text;
using Newtonsoft.Json;
using Npgsql;
using Channel = System.Threading.Channels.Channel;
using MimeContent = MimeKit.MimeContent;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Infrastructure.Integration.EmailNotification
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private ILogger<EmailNotificationService> _logger;
        private readonly IConfiguration _configuration;
        private IWebHostEnvironment _ienvironment;
        private readonly IDomainEventService _domainEventService;
        string smtpServer = string.Empty;
        string username = string.Empty;
        string password = string.Empty;
        string senderName = string.Empty;
        private SmtpClient _smtpClient = new SmtpClient();
        Dictionary<string, string> senderNameDict = new Dictionary<string, string>();
        private readonly ConcurrentBag<Channel<string>> _channels = new();
        private readonly ConcurrentDictionary<string, List<Channel<string>>> _clientChannelDict = new();
        private IDbConnection _dapperDbConnection { get; set; }
        private IDbConnection _dapperEventDbConnection { get; set; }

        private IDapper _dapper;
        private readonly string _sqlFilePath;
        private readonly Dictionary<string, string> _sqlQueries;

        //Microsft Graph Credentials
        private readonly GraphServiceClient _graphClient;
        string senderEmail = string.Empty;
        string tenantId = string.Empty;
        string clientId = string.Empty;
        string clientSecret = string.Empty;
        string thumbprint = string.Empty;
        X509Certificate2 certificate;

        Dictionary<string, (string SenderName, string SenderEmail, string TenantId,
                            string ClientId, string ClientSecret, string Thumbprint)> microsoftGraphSettingsDict = new Dictionary<string, (string, string, string,
                                                                                                                        string, string, string)>();

        public EmailNotificationService(ILogger<EmailNotificationService> logger, IConfiguration configuration, IWebHostEnvironment ienvironment, IDomainEventService domainEventService, IDapper dapper)
        {
            this._logger = logger;
            this._configuration = configuration;
            this._ienvironment = ienvironment;
            this._domainEventService = domainEventService;

            _sqlFilePath = _configuration["SqlSeparationQueries:PostgreNotification"];
            _sqlQueries = LoadSqlQueries();
            _configuration = configuration;
            _dapperDbConnection = new NpgsqlConnection(_configuration["ConnectionStrings:PostgreSqlDBConnection"]);
            _dapperEventDbConnection = new NpgsqlConnection(_configuration["ConnectionStrings:NotificationPostgreSqlDBConnection"]);
            this._dapper = dapper;

            //Microsoft Graph Credentials
            var graphValuesSection = _configuration.GetSection("MicrosoftGraphSettings");
            foreach (IConfigurationSection section in graphValuesSection.GetChildren())
            {
                var key = section.GetValue<string>("ApiKey");
                tenantId = section.GetValue<string>("TenantId");
                clientId = section.GetValue<string>("ClientId");
                clientSecret = section.GetValue<string>("ClientSecret");
                senderEmail = section.GetValue<string>("SenderEmail");
                thumbprint = section.GetValue<string>("Thumbprint");
                senderNameDict.Add(key, section.GetValue<string>("SenderName"));
            }

            // Azure App Service
            using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);

            certificate = store.Certificates
                .Find(X509FindType.FindByThumbprint, thumbprint, false)
                .OfType<X509Certificate2>()
                .FirstOrDefault();

            if (certificate == null)
                _logger.LogError($"EmailNotificationServices - Failed to fetch certificate");

            var credential = new ClientCertificateCredential(
                  tenantId,
                  clientId,
                  certificate);

            _graphClient = new GraphServiceClient(credential);
        }

        private Dictionary<string, string> LoadSqlQueries()
        {
            string absoluteSqlFilePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _sqlFilePath));
            var xml = XElement.Load(absoluteSqlFilePath);

            return xml.Elements("sql")
                      .ToDictionary(e => e.Attribute("name").Value, e => e.Value.Trim());
        }

        #region Email Service

        /// <summary>
        ///  Sends an email with attachments using SMTP.
        /// </summary>
        /// <param name="notification"></param>
        /// <returns>True if the email is sent successfully; otherwise, false.</returns>
        public async Task<PostgreNotification> SendEmailNotification(PostgreNotification notification)
        {
            try
            {
                string EmailResponse = string.Empty;

                //Step 1: Get configuration values for smtp server 
                #region Get configuration values for smtp server

                if (senderNameDict.TryGetValue(notification.ApiKey, out string dictValue))
                {
                    // senderName found and assigned
                    senderName = dictValue;
                }
                else
                {
                    _logger.LogError("No Sender Name found.");
                }

                if (string.IsNullOrEmpty(smtpServer)) return notification;
                #endregion

                //Step 2: Assign values for email notification
                #region Assign values for email notification
                var message = new MimeMessage();

                //Step 2.1: Sender's email address 
                message.From.Add(new MailboxAddress(senderName, username));

                //Step 2.2: Recipient's email address
                if (!string.IsNullOrEmpty(notification.EmailTo))
                {
                    foreach (var emailto in notification.EmailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        message.To.Add(new MailboxAddress(emailto, emailto));
                    }
                }
                //Step 2.3: CC's email address
                if (!string.IsNullOrEmpty(notification.EmailCc))
                {
                    foreach (var emailcc in notification.EmailCc.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        message.Cc.Add(new MailboxAddress(emailcc, emailcc));
                    }
                }

                //Step 2.4: BCC's email address
                if (!string.IsNullOrEmpty(notification.EmailBcc))
                {
                    foreach (var emailbcc in notification.EmailBcc.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        message.Bcc.Add(new MailboxAddress(emailbcc, emailbcc));
                    }
                }

                //Step 2.5: Add email's subject to message
                message.Subject = notification.EmailSubject;

                //Step 2.6: Add email's body to message

                //Create a TextPart for the email body
                var Body = new TextPart(TextFormat.Html)
                {
                    Text = notification.EmailBody
                };
                //Create a Multipart to handle attachments
                var multipart = new Multipart("mixed");
                multipart.Add(Body);

                //Step 2.7: Add email's attachments to message
                if (notification.EmailAttachments != null)
                {
                    foreach (var attachmentFile in notification.EmailAttachments)
                    {
                        if (attachmentFile != null && attachmentFile.Content?.Length > 0)
                        {
                            // Create a stream from the byte array
                            var stream = new MemoryStream(attachmentFile.Content);

                            // Create a MimePart for the attachment
                            var attachment = new MimePart()
                            {
                                Content = new MimeContent(stream, ContentEncoding.Default),
                                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                                ContentTransferEncoding = ContentEncoding.Base64,
                                FileName = attachmentFile.FileName
                            };

                            // Add the attachment to the Multipart
                            multipart.Add(attachment);
                        }
                    }
                }

                //Set the Multipart as the email body
                message.Body = multipart;
                #endregion

                //Step 3: Send the email from SMTP
                try
                {
                    await _smtpClient.SendAsync(message);
                    EmailResponse = "2.0.0 OK";
                }
                catch (Exception ex)
                {
                    EmailResponse = ex.Message;
                }

                //Step 4: Check email's status and update it into db
                #region check status and update status in db
                notification.NotificationDelivery = new NotificationStatus();
                if (EmailResponse.Contains("2.0.0 OK"))
                {
                    notification.NotificationResponseDateTime = DateTime.UtcNow;
                    notification.NotificationDelivery.isDelivered = true;
                    notification.NotificationDelivery.DeliveryReport = EmailResponse;
                    notification.NotificationErrorMessage = "Notification sent successfully 2.0.0 OK";
                    return notification;
                }
                else
                {
                    notification.NotificationResponseDateTime = DateTime.UtcNow;
                    notification.NotificationDelivery.isDelivered = false;
                    notification.NotificationDelivery.DeliveryReport = EmailResponse;
                    notification.NotificationErrorMessage = $"{EmailResponse.ToString()}";
                    return notification;
                    // throw new ApplicationException($"Error:mail not sent.");
                }
                #endregion
            }
            catch (Exception ex)
            {
                // Throw the exception if an error occurs
                _logger.LogError("EmailNotificationService.SendEmailNotification - " + ex.Message);
                notification.NotificationErrorMessage = ex.Message.ToString();
                return notification;
            }
        }

        public async Task DispatchEvents(Domain.Entities.PostgreNotification entity)
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
        #endregion

        #region SSE (Broadcast) Service

        public Channel<string> Register()
        {
            var channel = Channel.CreateUnbounded<string>();
            _channels.Add(channel);
            return channel;
        }
        public void Broadcast(string message)
        {
            foreach (var ch in _channels)
            {
                ch.Writer.TryWrite(message);
            }
        }

        public Channel<string> RegisterMultipleChannelsPerUser(string userId)
        {
            if (!_clientChannelDict.TryGetValue(userId, out var channels))
            {
                channels = new List<Channel<string>>();
                _clientChannelDict[userId] = channels;
            }
            // Use UserId as the key
            var channel = Channel.CreateUnbounded<string>();
            channels.Add(channel);

            return channel;

        }
        public void BroadcastMultipleChannelsPerUser(string message, string? targetUserId = null)
        {
            // Prepare JSON once for logging clarity
            var payload = JsonConvert.DeserializeObject<dynamic>(message);
            string type = payload?.Type ?? "Unknown";

            if (targetUserId != null)
            {
                // 🎯 Send only to one user
                if (_clientChannelDict.TryGetValue(targetUserId, out var channels))
                {
                    foreach (var ch in channels.ToList()) // Copy for thread-safety
                    {
                        if (!ch.Writer.TryWrite(message))
                        {
                            // Clean up bad channels
                            _logger.LogWarning($"⚠️ Failed to write message to channel for user {targetUserId}. Removing channel.");
                            channels.Remove(ch);
                        }
                    }
                }

                _logger.LogInformation($"📨 Broadcasted targeted message to user {targetUserId}: {payload}");
                return;
            }

            // 🌍 Otherwise, send to all connected users
            foreach (var (userId, channels) in _clientChannelDict.ToList())
            {
                foreach (var ch in channels.ToList())
                {
                    if (!ch.Writer.TryWrite(message))
                    {
                        _logger.LogWarning($"⚠️ Failed to write message to channel for user {userId}. Removing channel.");
                        channels.Remove(ch);
                    }
                }
            }

            _logger.LogInformation($"🌍 Broadcasted global message to all users: {payload}");
        }

        public async Task<(int, List<InSystemNotification>)> GetRecentNotifications(string userId)
        {
            //var missedNotifications = (await _dapperEventDbConnection.QueryAsync<InSystemNotification>(_sqlQueries["PostgreNotification.GetRecentNotifications"])).ToList();
            //return missedNotifications;

            const string sql = "SELECT * FROM fn_get_recent_notifications_with_count(@UserId);";

            var notifications =
                (await _dapperEventDbConnection.QueryAsync<InSystemNotification>(sql, new { UserId = userId })).ToList();

            if (notifications?.Any() != true)
                return (0, new List<InSystemNotification>());

            var unreadCount = notifications.First().UnreadCount;

            return (unreadCount, notifications);
        }

        public async Task SaveAndBroadcastAsync(InSystemNotification notification)
        {
            var insertInSystemNotificationQuery = _sqlQueries["PostgreNotification.InsertInSystemNotification"];

            // Save the error notification into the PostgreDb
            var param = new
            {
                notification.Message,
                notification.CreatedDateTime,
                notification.CreatedBy,
                notification.CreatedById,
                notification.UpdatedBy,
                notification.UpdatedById,
                notification.UpdatedDateTime,
                notification.UpdateReason,
                notification.OwnerId,
                notification.IsActive,
                notification.IsDeleted,
                notification.IsApproved,
                notification.ApproverId,
                notification.ApprovedDateTime,
                notification.IsAuthorized,
                notification.AuthorizedById,
                notification.AuthorizedDateTime,
                notification.SysData,
                notification.TenantId,
                notification.AssociatedUserId,
                notification.SubTenantId
            };
            var insertedId = await _dapperEventDbConnection.ExecuteScalarAsync<string>(insertInSystemNotificationQuery, param);

            // If any active users in the list then exit the loop
            // (since no channel/user is present to consume)
            var activeUserIds = _clientChannelDict.Keys;
            if (!activeUserIds.Any())
            {
                _logger.LogInformation("No active users connected. Skipping live broadcast.");
                return;
            }

            // If active users exist, add the record in mapping table
            const string insertUserMapSql = @"
            INSERT INTO ""NotificationUserMapping"" (""UserId"", ""NotificationId"")
            VALUES (@UserId, @NotificationId)
            ON CONFLICT DO NOTHING;";
            var userMappings = activeUserIds.Select(userId => new
            {
                UserId = userId,
                NotificationId = insertedId
            });
            await _dapperEventDbConnection.ExecuteAsync(insertUserMapSql, userMappings);

            // 3️⃣ Broadcast to connected clients
            var payload = new
            {
                Id = insertedId,
                notification.Message,
                notification.CreatedDateTime,
                Type = "NewNotification"

            };
            BroadcastMultipleChannelsPerUser(JsonConvert.SerializeObject(payload));
            _logger.LogInformation($"Broadcasted notification to {activeUserIds.Count()} users via SSE: {notification.Message} ");
        }

        // 🔹 Unregister (on disconnect)
        public void UnregisterChannel(string userId, Channel<string> channel)
        {
            if (_clientChannelDict.TryGetValue(userId, out var channels))
            {
                channels.Remove(channel);
                if (channels.Count == 0)
                    _clientChannelDict.TryRemove(userId, out _);
            }

            _logger.LogInformation($"User {userId} disconnected. Channels left: {channels?.Count ?? 0}");
        }

        public async Task<string> MarkAsRead(string userId, string notificationId)
        {
            string sql = "";
            object param = new { };

            if (string.IsNullOrWhiteSpace(notificationId))
            {
                // 🟢 Mark ALL unread notifications for this user
                sql = @"
            UPDATE ""NotificationUserMapping""
            SET ""IsRead"" = TRUE,
                ""ReadAt"" = NOW()
            WHERE ""UserId"" = @UserId
              AND (""IsRead"" = FALSE OR ""IsRead"" IS NULL);";
                param = new { UserId = userId };
            }

            else
            {
                // 🔴 Mark a single notification
                sql = @"
            UPDATE ""NotificationUserMapping""
            SET ""IsRead"" = TRUE,
                ""ReadAt"" = NOW()
            WHERE ""UserId"" = @UserId
              AND ""NotificationId"" = @NotificationId;";
                param = new { UserId = userId, NotificationId = notificationId };
            }

            try
            {
                await _dapperEventDbConnection.ExecuteAsync(sql, param);

                // 🔹 Broadcast read update to all open channels for this user
                var payload = JsonConvert.SerializeObject(new
                {
                    Type = "ReadUpdate",
                    UserId = userId,
                    NotificationId = notificationId
                });

                BroadcastMultipleChannelsPerUser(payload, userId);

                return "Marked as Read";


            }
            catch
            {
                throw;
            }
        }


        #endregion

        #region Microsoft Graph 
        public async Task<List<PostgreNotification>> SendBatchEmailNotificationUsingMicrosoftGraph(List<PostgreNotification> notifications)
        {
            foreach (var notification in notifications)
            {
                try
                {
                    await SendEmailNotificationUsingMicrosoftGraph(notification);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        $"Error sending email to {string.Join(", ", notification.EmailTo)}");

                    notification.NotificationErrorMessage =
                        $"Failed to send email. {ex.Message}";
                }
            }

            return notifications;
        }

        public async Task<PostgreNotification> SendEmailNotificationUsingMicrosoftGraph(PostgreNotification notification)
        {
            try
            {
                string emailResponse = string.Empty;

                #region Step 1: Get configuration values

                if (senderNameDict.TryGetValue(notification.ApiKey, out string dictValue))
                {
                    senderName = dictValue;
                }
                else
                {
                    _logger.LogError("No Sender Name found.");
                }

                if (string.IsNullOrEmpty(senderEmail))
                    return notification;

                #endregion

                #region Step 2: Build Graph Message

                var toRecipients = new List<Recipient>();
                var ccRecipients = new List<Recipient>();
                var bccRecipients = new List<Recipient>();

                // TO
                if (!string.IsNullOrEmpty(notification.EmailTo))
                {
                    foreach (var email in notification.EmailTo
                                 .Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        toRecipients.Add(new Recipient
                        {
                            EmailAddress = new EmailAddress
                            {
                                Address = email.Trim()
                            }
                        });
                    }
                }

                // CC
                if (!string.IsNullOrEmpty(notification.EmailCc))
                {
                    foreach (var email in notification.EmailCc
                                 .Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        ccRecipients.Add(new Recipient
                        {
                            EmailAddress = new EmailAddress
                            {
                                Address = email.Trim()
                            }
                        });
                    }
                }

                // BCC
                if (!string.IsNullOrEmpty(notification.EmailBcc))
                {
                    foreach (var email in notification.EmailBcc
                                 .Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        bccRecipients.Add(new Recipient
                        {
                            EmailAddress = new EmailAddress
                            {
                                Address = email.Trim()
                            }
                        });
                    }
                }

                var message = new Microsoft.Graph.Models.Message
                {
                    Subject = notification.EmailSubject,
                    Body = new ItemBody
                    {
                        ContentType = BodyType.Html,
                        Content = notification.EmailBody
                    },
                    ToRecipients = toRecipients,
                    CcRecipients = ccRecipients,
                    BccRecipients = bccRecipients
                };

                // Attachments
                if (notification.EmailAttachments != null &&
                    notification.EmailAttachments.Any())
                {
                    message.Attachments = new List<Microsoft.Graph.Models.Attachment>();

                    foreach (var attachmentFile in notification.EmailAttachments)
                    {
                        if (attachmentFile?.Content?.Length > 0)
                        {
                            message.Attachments.Add(new FileAttachment
                            {
                                OdataType = "#microsoft.graph.fileAttachment",
                                Name = attachmentFile.FileName,
                                ContentType = "application/octet-stream",
                                ContentBytes = attachmentFile.Content
                            });
                        }
                    }
                }

                #endregion

                #region Step 3: Send via Microsoft Graph

                try
                {
                    await _graphClient
                        .Users[senderEmail]
                        .SendMail
                        .PostAsync(new SendMailPostRequestBody
                        {
                            Message = message,
                            SaveToSentItems = true
                        });

                    emailResponse = "2.0.0 OK";
                }
                catch (ServiceException ex)
                {
                    emailResponse = ex.Message;
                    _logger.LogError(ex, "Graph API send failed.");
                }
                catch (Exception ex)
                {
                    emailResponse = ex.Message;
                    _logger.LogError(ex, "Unexpected error while sending email.");
                }

                #endregion

                #region Step 4: Update Notification Status

                notification.NotificationDelivery = new NotificationStatus();

                notification.NotificationResponseDateTime = DateTime.UtcNow;

                if (emailResponse.Contains("2.0.0 OK"))
                {
                    notification.NotificationDelivery.isDelivered = true;
                    notification.NotificationDelivery.DeliveryReport = emailResponse;
                    notification.NotificationErrorMessage =
                        "Notification sent successfully 2.0.0 OK";
                }
                else
                {
                    notification.NotificationDelivery.isDelivered = false;
                    notification.NotificationDelivery.DeliveryReport = emailResponse;
                    notification.NotificationErrorMessage = emailResponse;
                }

                return notification;

                #endregion
            }
            catch (Exception ex)
            {
                _logger.LogError("EmailNotificationService.SendEmailNotification - " + ex.Message);

                notification.NotificationErrorMessage = ex.Message;
                notification.NotificationDelivery = new NotificationStatus
                {
                    isDelivered = false,
                    DeliveryReport = ex.Message
                };
                notification.NotificationResponseDateTime = DateTime.UtcNow;

                return notification;
            }
        }
        #endregion
    }
}