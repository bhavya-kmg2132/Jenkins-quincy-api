using Application.ExternalPolicy.Endorsement.Queries.GetPolicyEndorsement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// Proxies the DB2 (QOL Insurance) Endorsement APIs.
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [Route("api/v1/policy")]
    public class Db2EndorsementController : Db2ProxyControllerBase
    {
        /// <summary>Retrieves a policy's current data from DB2 for an endorsement.</summary>
        [HttpPost("endorse/get-policy")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> GetPolicyEndorsement(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            GetPolicyEndorsementQuery request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }
    }
}
