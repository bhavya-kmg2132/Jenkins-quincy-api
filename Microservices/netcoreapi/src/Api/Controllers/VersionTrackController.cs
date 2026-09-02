using System.Threading.Tasks;
using Application.VersionTrack.Commands.AddVersionTrack;
using Application.VersionTrack.Queries;
using Application.VersionTrack.Queries.GetVersionTrack;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.timetrack
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class VersionTrackController : ApiControllerBase
    {
        /// <summary>
        /// Create VersionTrack
        /// </summary>
        /// <param name="command">request</param>
        /// <returns>string</returns>
        [HttpPost("Create")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<string>> Create([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                               [FromHeader(Name = "X-Request-Id")] string requestId,
                                                               [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                 [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                       [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                              AddVersionTrackRequest command)
        {
            // mediator's send method will call the AddVersionTrackRequest to create a VersionTrack
            return await Mediator.Send(command);
        }

        /// <summary>
        /// Get VersionTrack
        /// </summary>
        /// <returns>VersionTrackListVm</returns>
        [HttpGet("GetVersionTrack")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(VersionTrackListVm))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<VersionTrackListVm>> GetVersionTrack([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                               [FromHeader(Name = "X-Request-Id")] string requestId,
                                                               [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                               [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                               [FromHeader(Name = "X-Api-Key")] string apiKey
                                                               )
        {
            // mediator's send method will call the GetVersionTrackQuery
            return await Mediator.Send(new GetVersionTrackQuery { });
        }

    }
}
