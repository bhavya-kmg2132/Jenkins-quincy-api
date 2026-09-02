using Application.ExternalPolicy.TaskManager.Commands.CloseTask;
using Application.ExternalPolicy.TaskManager.Commands.CreateTask;
using Application.ExternalPolicy.TaskManager.Commands.ReferAllTasks;
using Application.ExternalPolicy.TaskManager.Commands.ReferTask;
using Application.ExternalPolicy.TaskManager.Commands.ReopenTask;
using Application.ExternalPolicy.TaskManager.Commands.UpdateTask;
using Application.ExternalPolicy.TaskManager.Queries.GetTaskDetail;
using Application.ExternalPolicy.TaskManager.Queries.GetTasks;
using Application.ExternalPolicy.TaskManager.Queries.GetTaskUsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// Proxies the DB2 (QOL Insurance) Task Manager APIs.
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [Route("api/v1/policy")]
    public class Db2TaskManagerController : Db2ProxyControllerBase
    {
        /// <summary>Queries tasks on a policy in DB2.</summary>
        [HttpPost("GetTasks")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> GetTasks(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            GetTasksQuery request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }

        /// <summary>Retrieves a single task by code from DB2.</summary>
        [HttpPost("GetTaskDetail")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> GetTaskDetail(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            GetTaskDetailQuery request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }

        /// <summary>Creates a task on a policy in DB2.</summary>
        [HttpPost("CreateTask")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> CreateTask(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            CreateTaskRequest request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }

        /// <summary>Updates a task in DB2.</summary>
        [HttpPut("UpdateTask")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> UpdateTask(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            UpdateTaskRequest request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }

        /// <summary>Queries eligible task users/assignees from DB2.</summary>
        [HttpPost("GetTaskUsers")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> GetTaskUsers(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            GetTaskUsersQuery request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }

        /// <summary>Refers a single task to another user in DB2.</summary>
        [HttpPost("ReferTask")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> ReferTask(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            ReferTaskRequest request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }

        /// <summary>Refers a batch of tasks to other users in DB2.</summary>
        [HttpPost("ReferAllTasks")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> ReferAllTasks(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            ReferAllTasksRequest request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }

        /// <summary>Closes a task in DB2.</summary>
        [HttpPost("CloseTask")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> CloseTask(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            CloseTaskRequest request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }

        /// <summary>Reopens a previously closed task in DB2.</summary>
        [HttpPost("ReopenTask")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> ReopenTask(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            ReopenTaskRequest request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }
    }
}
