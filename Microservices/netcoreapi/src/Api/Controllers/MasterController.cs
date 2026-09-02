using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Master.Master.Queries.GetFilteredGenericMasterTable;
using Application.Master.Queries.GetFilteredGenericMasterTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class MasterController : ApiControllerBase
    {
        [HttpPost("GetFilteredGenericMasterTable")]
        [Consumes("application/json")]
        public async Task<List<GetFilteredGenericMasterTableQueryDto>> GetFilteredGenericMasterTable([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                                                  [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                                                  [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                                                  [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                                                  [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                                                  GetFilteredGenericMasterTableQuery request)
        {
            var vm = await Mediator.Send(new GetFilteredGenericMasterTableQuery
            {
                Type = request.Type,
                Group = request.Group
            });

            return vm;
        }
    }
}
