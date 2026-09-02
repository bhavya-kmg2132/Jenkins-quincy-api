using Application.ExternalPolicy.Models;
using Application.ExternalPolicy.Policy.Commands.ChangePolicy;
using Application.ExternalPolicy.Policy.Commands.ChangeTransaction;
using Application.ExternalPolicy.Policy.Commands.PatchPolicy;
using Application.ExternalPolicy.Policy.Commands.RateMcaData;
using Application.ExternalPolicy.Policy.Commands.SavePolicyInfo;
using Application.ExternalPolicy.Policy.Commands.UpdatePolicyInfo;
using Application.ExternalPolicy.Policy.Commands.UpdateUnderwriterQuestions;
using Application.ExternalPolicy.Policy.Queries.GetPolicyDetail;
using Application.ExternalPolicy.Policy.Queries.GetPolicyHistory;
using Application.ExternalPolicy.Policy.Queries.GetQuoteNumber;
using Application.ExternalPolicy.Policy.Queries.GetQuotes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// Proxies the DB2 (QOL Insurance) Policy APIs for quote numbers, quote search and transaction changes.
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [Route("api/v1/policy")]
    public class Db2PolicyController : Db2ProxyControllerBase
    {
        /// <summary>Requests a new quote number from DB2.</summary>
        [HttpPost("GetQuoteNumber")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> GetQuoteNumber(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            GetQuoteNumberQuery request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }

        /// <summary>Searches DB2 quotes by policy number, insured name or agent code.</summary>
        [HttpPost("GetQuotes")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> GetQuotes(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            GetQuotesQuery request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }

        /// <summary>Applies a WINS transaction change to a policy in DB2.</summary>
        [HttpPost("ChangeTransaction")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> ChangeTransaction(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            ChangeTransactionRequest request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }

        /// <summary>Retrieves a policy's current data from DB2 for a change transaction.</summary>
        // Routed off the controller's api/v1/policy prefix to match the documented api/v1/ChangePolicy path.
        [HttpPost("~/api/v1/ChangePolicy")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> ChangePolicy(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            ChangePolicyRequest request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }

        /// <summary>Updates policy-level data (coverages, endorsements, etc.) in DB2.</summary>
        [HttpPut("UpdatePolicyInfo")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> UpdatePolicyInfo(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            UpdatePolicyInfoRequest request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }

        /// <summary>Rates MCA (Massachusetts Commercial Auto) policy data against DB2.</summary>
        [HttpPost("RateMCAData")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> RateMcaData(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            [FromBody] List<PolicyDataTable> requestData,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(new RateMcaDataRequest { PolicyData = requestData }, cancellationToken);
            return FromExternalResponse(response);
        }

        /// <summary>Saves policy-level info (insured, addresses, coverage/underwriter indicators) to DB2.</summary>
        [HttpPost("SavePolicyInfo")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> SavePolicyInfo(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            SavePolicyInfoRequest request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }

        /// <summary>Updates underwriter questions on a policy in DB2.</summary>
        [HttpPut("UpdateUnderwriterQuestions")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> UpdateUnderwriterQuestions(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            UpdateUnderwriterQuestionsRequest request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }

        /// <summary>Retrieves a policy's full detail from DB2.</summary>
        [HttpPost("GetPolicyDetail")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> GetPolicyDetail(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            GetPolicyDetailQuery request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }

        /// <summary>Retrieves a paginated transaction history for a policy from DB2.</summary>
        [HttpPost("history")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> GetPolicyHistory(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            GetPolicyHistoryQuery request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }

        /// <summary>Partially updates policy/insured info in DB2.</summary>
        [HttpPost("PatchPolicy")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> PatchPolicy(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            PatchPolicyRequest request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }
    }
}
