using Application.ExternalPolicy.Vehicle.Commands.AddCoverages;
using Application.ExternalPolicy.Vehicle.Commands.AddVehicle;
using Application.ExternalPolicy.Vehicle.Commands.DeleteCoverages;
using Application.ExternalPolicy.Vehicle.Commands.DeleteVehicle;
using Application.ExternalPolicy.Vehicle.Commands.PatchVehicle;
using Application.ExternalPolicy.Vehicle.Queries.GetVehicleDetail;
using Application.ExternalPolicy.Vehicle.Queries.GetVehicleLocationMaster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// Proxies the DB2 (QOL Insurance) Vehicle APIs.
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [Route("api/v1/policy")]
    public class Db2VehicleController : Db2ProxyControllerBase
    {
        /// <summary>Adds a vehicle (by VIN and/or registration) to a policy in DB2.</summary>
        [HttpPost("AddVehicle")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> AddVehicle(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            AddVehicleRequest request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }

        /// <summary>Deletes one or more vehicles from a policy in DB2.</summary>
        [HttpDelete("DeleteVehicle")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> DeleteVehicle(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            DeleteVehicleRequest request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }

        /// <summary>Queries vehicle detail on a policy in DB2.</summary>
        [HttpPost("GetVehicleDetail")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> GetVehicleDetail(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            GetVehicleDetailQuery request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }

        /// <summary>Gets the vehicle location master list from DB2.</summary>
        [HttpGet("vehicles/location-master")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> GetVehicleLocationMaster(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(new GetVehicleLocationMasterQuery(), cancellationToken);
            return FromExternalResponse(response);
        }

        /// <summary>Adds coverages for a vehicle location on a policy in DB2.</summary>
        [HttpPost("vehicles/coverages")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> AddCoverages(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            AddCoveragesRequest request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }

        /// <summary>Deletes coverages for a vehicle location on a policy in DB2.</summary>
        [HttpDelete("vehicles/coverages")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> DeleteCoverages(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            DeleteCoveragesRequest request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }

        /// <summary>Updates one or more vehicles on a policy in DB2.</summary>
        [HttpPost("PatchVehicle")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<IActionResult> PatchVehicle(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Oid")] string requestOid,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            PatchVehicleRequest request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            return FromExternalResponse(response);
        }
    }
}
