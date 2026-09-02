using System.Net;
using System.Threading.Tasks;
using Application.Policy.Commands.CreatePolicy;
using Application.Policy.Commands.DeletePolicy;
using Application.Policy.Commands.UpdatePolicy;
using Application.Policy.Queries;
using Application.Policy.Queries.GetPolicyById;
using Application.Policy.Queries.GetPolicyList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class PolicyController : ApiControllerBase
    {
        /// <summary>
        /// Create a new MCA Policy
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="request"></param>
        /// <returns>string</returns>
        [HttpPost("Create")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<string>> Create(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            CreatePolicyRequest request)
        {
            return await Mediator.Send(request);
        }

        /// <summary>Update an existing MCA Policy</summary>
        [HttpPost("Update")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<int>> Update(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            UpdatePolicyRequest request)
        {
            await Mediator.Send(request);
            return (int)HttpStatusCode.OK;
        }

        /// <summary>Soft-delete an MCA Policy</summary>
        [HttpPost("Delete")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<int>> Delete(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            DeletePolicyRequest request)
        {
            await Mediator.Send(request);
            return (int)HttpStatusCode.OK;
        }

        /// <summary>Get all MCA Policies</summary>
        [HttpGet("GetList")]
        [ProducesResponseType(200, Type = typeof(PolicyListVm))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<PolicyListVm>> GetList(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            return await Mediator.Send(new GetPolicyListQuery());
        }

        /// <summary>Get an MCA Policy by Id</summary>
        [HttpGet("GetById")]
        [ProducesResponseType(200, Type = typeof(PolicyDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<PolicyDto>> GetById(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            string id)
        {
            return await Mediator.Send(new GetPolicyByIdQuery { Id = id });
        }
    }
}
