using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.EmailNotification.Queries.GetRecentNotifications;
using Application.Notification.Commands.SendBatchEmailUsingMicrosoftGraph;
using Application.Notification.Commands.SendEmailNotification;
using Application.Notification.Commands.SendEmailUsingMicrosoftGraph;
using Application.Notification.Queries;
using Application.ZeptoMail.Commands.SendBatchTransactionalEmail;
using Application.ZeptoMail.Commands.SendTransactionalEmail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class NotificationController : ApiControllerBase
    {
        private readonly IEmailNotificationService _emailNotificationService;
        private IConfiguration _configuration;
        private ILogger _logger;

        public NotificationController(IEmailNotificationService emailNotificationService, IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _emailNotificationService = emailNotificationService;
            _logger = logger;
        }

        [AllowAnonymous]

        #region Notification

        /// <summary>
        /// send an email notification
        /// </summary>
        /// <param name="request"></param>
        /// <returns>string</returns>
        [HttpPost("SendEmailNotification")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<string>> SendEmailNotification([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                      [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                      [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                      [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                      [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                      SendEmailNotificationRequest request)
        {
            //Mediator's send method will call the SendEmailNotificationRequest.
            return await Mediator.Send(request);
        }


        /// <summary>
        /// send an email notification
        /// </summary>
        /// <param name="request"></param>
        /// <returns>string</returns>
        [HttpPost("SendEmailUsingMicrosoftGraph")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<string>> SendEmailUsingMicrosoftGraph([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                      [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                      [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                      [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                      [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                      SendEmailUsingMicrosoftGraphRequest request)
        {
            //Mediator's send method will call the SendEmailNotificationRequest.
            return await Mediator.Send(request);
        }

        [HttpPost("SendBatchEmailUsingMicrosoftGraph")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<List<Domain.Entities.PostgreNotification>>> SendBatchEmailUsingMicrosoftGraph([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                  [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                  [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                  [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                  [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                  SendBatchEmailUsingMicrosoftGraphRequest request)
        {
            //Mediator's send method will call the SendEmailNotificationRequest.
            return await Mediator.Send(request);
        }


        #endregion

        #region Notification Broadcaster

        /// <summary>
        /// Stream live message to UI
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <returns></returns>
        [HttpGet("Stream")]
        public async Task Stream([FromQuery] string userId, CancellationToken cancellationToken)
        {
            Response.Headers.Append("Content-Type", "text/event-stream");
            Response.Headers.Append("Cache-Control", "no-cache");
            Response.Headers.Append("Connection", "keep-alive");
            Response.Headers.Append("X-Accel-Buffering", "no");

            // 👇 Register this tab’s unique channel for the given userId
            var channel = _emailNotificationService.RegisterMultipleChannelsPerUser(userId);

            _logger.LogInformation($"User {userId} connected to SSE stream.");

            try
            {
                var heartbeatTask = Task.Run(async () =>
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
                            if (!cancellationToken.IsCancellationRequested)
                            {
                                await Response.WriteAsync(": ping\n\n", cancellationToken);
                                await Response.Body.FlushAsync(cancellationToken);
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            break; // graceful exit
                        }
                        catch (Exception ex)
                        {
                            // don’t crash heartbeat
                            _logger.LogWarning(ex, $"Heartbeat failed for user {userId}");
                        }
                    }
                }, cancellationToken);

                await foreach (var message in channel.Reader.ReadAllAsync(cancellationToken))
                {
                    try
                    {
                        await Response.WriteAsync($"data: {message}\n\n", cancellationToken);
                        await Response.Body.FlushAsync(cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        break; // graceful disconnect
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Write failed for user {userId}");
                        break;
                    }
                }
                // Let the heartbeat finish gracefully
                await heartbeatTask;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Client disconnected: {userId}");
            }
            finally
            {
                // 🧹 Clean up on disconnect
                _emailNotificationService.UnregisterChannel(userId, channel);
                _logger.LogInformation($"Channel unregistered for user {userId}");
            }
        }

        /// <summary>
        /// Get the recent notification based on the defined criteria from PostgreDb
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <returns></returns>
        [HttpPost("GetRecentNotifications")]
        public async Task<InSystemNotificationVm> GetRecentNotifications([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                         [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                         [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                         [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                         [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                         GetRecentNotificationsQuery request)
        {
            var recentNotifications = await Mediator.Send(request);
            return recentNotifications;
        }

        /// <summary>
        /// Mark the notifications status to read
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("MarkAsRead")]
        public async Task<IActionResult> MarkAsRead([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                    [FromHeader(Name = "X-Request-Id")] string requestId,
                                                    [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                    [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                    [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                    MarkAsReadRequest request)
        {

            await Mediator.Send(request);

            return Ok(new { Status = "Marked as read" });
        }
        #endregion

        #region ZeptoMail

        /// <summary>
        /// Send a zepto transactional email
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("SendTransactionalEmail")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<string>> SendTransactionalEmail([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                     [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                     [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                     [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                     [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                     SendTransactionalEmailRequest request)
        {
            //Mediator's send method will call the SendEmailNotificationRequest.
            return await Mediator.Send(request);
        }

        /// <summary>
        /// Sends batch zepto emails 
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("SendBatchTransactionalEmail")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<List<Domain.Entities.ZeptoMail>>> SendBatchTransactionalEmail([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                     [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                     [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                     [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                     [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                     SendBatchTransactionalEmailRequest request)
        {
            //Mediator's send method will call the SendEmailNotificationRequest.
            return await Mediator.Send(request);
        }
        #endregion
    }
}