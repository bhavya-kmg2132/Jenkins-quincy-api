using System.Collections.Generic;
using System.Threading.Tasks;
using Application.TransactionAction.Queries;
using Application.TransactionAction.Queries.GetTransactionActionMatrix;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    /// <summary>
    /// Serves the WINS transaction-code -> status -> available-actions matrix.
    /// </summary>
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class TransactionActionController : ApiControllerBase
    {
        /// <summary>Gets the full transaction action matrix.</summary>
        /// <returns>Dictionary keyed by transaction code (e.g. "10", "20", "55").</returns>
        [HttpGet("GetActionMatrix")]
        [ProducesResponseType(200, Type = typeof(Dictionary<string, TransactionActionStatusDto>))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<Dictionary<string, TransactionActionStatusDto>>> GetActionMatrix(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            return await Mediator.Send(new GetTransactionActionMatrixQuery());
        }
    }
}
