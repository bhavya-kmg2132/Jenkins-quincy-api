using Application.ExternalPolicy.PolicyCancellation.Commands.CancelPolicy;
using Application.ExternalPolicy.PolicyCancellation.Commands.DeleteTransPolicy;
using Application.ExternalPolicy.PolicyCancellation.Commands.HoldTransaction;
using Application.ExternalPolicy.PolicyCancellation.Queries.GetPolicyCancellation;
using Application.ExternalPolicy.PolicyCancellation.Queries.GetPolicyCancellationDetail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// Proxies the DB2 (QOL Insurance) Policy Cancellation APIs.
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [Route("api/v1/policy")]
    public class Db2PolicyCancellationController : Db2ProxyControllerBase
    {
        /// <summary>Retrieves policy cancellation data from DB2.</summary>
        [HttpPost("cancellation/get-policy")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> GetPolicyCancellation(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            GetPolicyCancellationQuery request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }

        /// <summary>Retrieves the premium/refund detail breakdown for a policy cancellation from DB2.</summary>
        [HttpPost("cancellation/details")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> GetPolicyCancellationDetail(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            GetPolicyCancellationDetailQuery request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }

        /// <summary>Cancels a policy in DB2.</summary>
        [HttpPost("cancellation/cancel-policy")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> CancelPolicy(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            CancelPolicyRequest request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }

        /// <summary>Deletes a pending cancellation transaction on a policy in DB2.</summary>
        [HttpDelete("cancellation/delete-policy")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> DeleteTransPolicy(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            DeleteTransPolicyRequest request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }

        /// <summary>Holds a pending transaction on a policy in DB2.</summary>
        [HttpPost("cancellation/hold-transaction")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> HoldTransaction(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            HoldTransactionRequest request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }
    }
}
