using System.Collections.Generic;
using System.Threading.Tasks;
using Application.CronJobRule.Commands.DeleteCronJobRule;
using Application.CronJobRule.Commands.InsertCronJobRule;
using Application.CronJobRule.Commands.UpdateCronJobRule;
using Application.CronJobRule.Commands.UpsertNotificationUserSubscription;
using Application.CronJobRule.Queries;
using Application.CronJobRule.Queries.GetCronJobRuleById;
using Application.CronJobRule.Queries.GetCronJobRules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.CronJobRule.GetNotificationUserSubscriptions
{
    /// <summary>
    /// Controller class handles incoming HTTP requests and send response back to the caller.
    /// </summary>
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CronJobRuleController : ApiControllerBase
    {
        /// <summary>
        /// Insert the cronJobRule document to Postgre
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("InsertCronJobRule")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<string>> InsertCronJobRuleToDb([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                InsertCronJobRuleCommand command)
        {
            // mediator's send method will call the CreateAssignmentRequest to create a Acme
            return await Mediator.Send(command);
        }

        /// <summary>
        /// Update the status of CronJob Read Status in Db
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("UpdateCronJobRule")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<int>> UpdateCronJobRuleInDb([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                UpdateCronJobRuleCommand command)
        {
            // mediator's send method will call the CreateAssignmentRequest to create a Acme
            return await Mediator.Send(command);
        }

        /// <summary>
        /// Delete the CronJob Rule from PostgreDb
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("DeleteCronJobRule")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<string>> DeleteCronJobRuleFromDb([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                DeleteCronJobRuleCommand command)
        {
            // mediator's send method will call the CreateAssignmentRequest to create a Acme
            return await Mediator.Send(command);
        }

        /// <summary>
        /// Get any CronJobRule by id
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("GetCronJobRuleById")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(CronJobRuleDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<CronJobRuleDto> GetCronJobRuleById([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                               [FromHeader(Name = "X-Request-Id")] string requestId,
                                                               [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                               [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                               [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                               GetCronJobRuleByIdQuery query)
        {
            // mediator's send method will call the CreateAssignmentRequest to create a Acme
            return await Mediator.Send(query);
        }

        /// <summary>
        /// Controller to get all the CronJob documents
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        [HttpGet("GetCronJobRules")]
        public async Task<ActionResult<List<Domain.Entities.CronJobRule>>> GetAllCronJobRules([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                [FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            // mediator's send method will call the CreateAssignmentRequest to create a Acme
            return await Mediator.Send(new GetCronJobRulesQuery { });
        }

        /// <summary>
        /// Upsert NotificationUserSubscription
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("UpsertNotificationUserSubscription")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<int>> UpsertNotificationUserSubscription([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                List<NotificationUserSubscription> notificationUserSubscriptions)
        {
            // mediator's send method will call the UpdateNotificationUserSubscriptionInDbRequest to create a Acme
            return await Mediator.Send(new UpsertNotificationUserSubscriptionCommand { notificationUserSubscriptions = notificationUserSubscriptions });
        }

        /// <summary>
        /// Controller to get all the notification documents
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        [HttpPost("GetNotificationUserSubscriptions")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(NotificationUserSubscriptionVm))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<NotificationUserSubscriptionVm>> GetNotificationUserSubscriptions([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                 List<string> userIds)
        {
            // mediator's send method will call the CreateAssignmentRequest to create a Acme
            return await Mediator.Send(new GetNotificationUserSubscriptionsQuery { UserIds = userIds });
        }

    }

}
