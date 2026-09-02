using System.Threading.Tasks;
using Application.PublishEvent.Queries;
using Application.PublishEvent.Queries.GetPublishEventDataList;
using Microsoft.AspNetCore.Mvc;


namespace Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class PublishEventDataController : ApiControllerBase
    {

        /// <summary>
        /// Get list of all the PublishEventData
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("Getlist")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(PublishEventDataListVm))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<PublishEventDataListVm>> GetPublishEventData([FromHeader(Name = "x-Correlation-Id")] string correlationid,
                                                               [FromHeader(Name = "x-Request-Id")] string requestid,
                                                               [FromHeader(Name = "x-Request-Oid")] string requestoid,
                                                               [FromHeader(Name = "x-Request-Uid")] string requestuid,
                                                               [FromHeader(Name = "x-Api-Key")] string apikey,
                                                               GetPublishEventDataListQuery request)

        {
            // mediator's send method will call the getPublishEventDataquery for reading the PublishEventData by PublishEventDataid
            return await Mediator.Send(request);
        }
    }
}
