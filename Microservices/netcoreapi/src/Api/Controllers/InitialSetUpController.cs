using System.Net;
using System.Threading.Tasks;
using Application.InitialSetUp.CreateInitialSetUpRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    /// <summary>
    /// Controller class handles incoming HTTP requests and send response back to the caller.
    /// 1. Template for Dapper 
    /// </summary>
    //AllowAnonymous :negates the Authorize Attribute and allows anonymous access.
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class InitialSetUpController : ApiControllerBase
    {
        private readonly ILogger<InitialSetUpController> _logger;
        public InitialSetUpController(ILogger<InitialSetUpController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Create Initial Set up request
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="request"></param>
        /// <returns>ActionResult</returns>
        [HttpPost()]
        [Route("Create")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<int>> Create([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                    [FromHeader(Name = "X-Request-Id")] string requestId,
                                                    [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                    [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                    [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                    CreateInitialSetUpRequest request)
        {
            // mediator's send method will call the CreateInitialSetUpRequest
            await Mediator.Send(request);

            return (int)HttpStatusCode.OK;
        }
    }
}








