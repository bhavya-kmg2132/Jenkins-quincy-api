using System.Net;
using System.Threading.Tasks;
using Application.AcmeImportExport.Queries.Get;
using Application.AcmeOrders.Commands.DeletePermanentAcmeProduct;
using Application.AcmeProduct.Commands.CreateAcmeProduct;
using Application.AcmeProduct.Commands.DeleteAcmeProduct;
using Application.AcmeProduct.Commands.UpdateAcmeProduct;
using Application.AcmeProduct.Queries;
using Application.AcmeProduct.Queries.GetAcmeProductById;
using Application.AcmeProduct.Queries.GetAcmeProductList;
using Application.AcmeProductDetailExport.Commands.Create;
using Application.AcmeProductDetailExport.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Api.Controllers
{
    /// <summary>
    /// Controller class handles incoming HTTP requests and send response back to the caller.
    /// </summary>
    //AllowAnonymous :negates the Authorize Attribute and allows anonymous access.
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AcmeProductController : ApiControllerBase
    {
        /// <summary>
        /// CreateAcmeProduct
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="request"></param>
        /// <returns>string</returns>
        [HttpPost()]
        [Route("Create")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<string>> Create([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                       [FromHeader(Name = "X-Request-Id")] string requestId,
                                                       [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                       [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                       [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                       CreateAcmeProductRequest request)
        {
            // mediator's send method will call the CreateAcmeProductRequest to create a Acme
            return await Mediator.Send(request);
        }

        /// <summary>
        /// Update AcmeProduct 
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns>int</returns>
        [HttpPost("Update")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<int>> Update([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                               [FromHeader(Name = "X-Request-Id")] string requestId,
                                               [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                               [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                               [FromHeader(Name = "X-Api-Key")] string apiKey,
                                               UpdateAcmeProductRequest request)
        {
            // mediator's send method will call the UpdateAcmeProductRequest .
            await Mediator.Send(request);

            // return HttpStatusCode OK 
            return (int)HttpStatusCode.OK;
        }

        /// <summary>
        /// Delete AcmeProduct
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="id"></param>
        /// <returns>int</returns>
        [HttpPost()]
        [Route("Delete")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<int>> Delete([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                               [FromHeader(Name = "X-Request-Id")] string requestId,
                                               [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                               [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                               [FromHeader(Name = "X-Api-Key")] string apiKey,
                                               DeleteAcmeProductRequest request)
        {
            // mediator's send method will call the DeleteAcmeProductRequest
            await Mediator.Send(request);

            // return HttpStatusCode OK 
            return (int)HttpStatusCode.OK;
        }

        /// <summary>
        /// Get AcmeProduct List
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <returns>AcmeProductListVm</returns>
        [HttpGet("GetList")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(AcmeProductListVm))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<AcmeProductListVm>> GetAcmeProductList([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                              [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                              [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                              [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                              [FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            // mediator's send method will call the GetAcmeProductByIdQuery for reading the Acme
            return await Mediator.Send(new GetAcmeProductListQuery { });
        }

        /// <summary>
        /// Get AcmeProduct By Id
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="id"></param>
        /// <returns>AcmeProductDto</returns>

        [HttpGet("GetById")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(AcmeProductDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<AcmeProductDto>> GetAcmeProductById([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                           [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                           [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                           [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                           [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                           string id)
        {
            // mediator's send method will call the GetAcmeProductByIdQuery for reading the Acme by AcmeProductId
            return await Mediator.Send(new GetAcmeProductByIdQuery { Id = id });
        }

        /// <summary>
        /// ExportAcmeProductDetailList
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <returns>FileResult</returns>

        [HttpGet("ExportAcmeProductDetailList")]
        [ProducesResponseType(200, Type = typeof(FileResult))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<FileResult> ExportAcmeProductDetailList([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                  [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                  [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                  [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                  [FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            //method will call the GetAcmeProductDetailQuery
            var fileContent = await Mediator.Send(new GetAcmeProductDetailQuery { });
            return File(fileContent.Content, fileContent.ContentType, fileContent.FileName);
        }

        /// <summary>
        /// Upload CSV and Excel file
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="csvFile"></param>
        /// <returns>ImportFileMetaDataDto</returns>
        [HttpPost()]
        [Route("ImportAcmeProductDetailListFromCsv")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(200, Type = typeof(AcmeImportFileMetaDataDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<AcmeImportFileMetaDataDto> ImportAcmeProductDetailListFromCsv([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                                        [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                                        [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                                        [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                                        [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                                        IFormFile csvFile)
        {
            //Check file contain data 
            if (csvFile == null)
            {
                return null;
            }

            // mediator's send method will call the AcmeProductImportFromCsvRequest
            return await Mediator.Send(new AcmeProductImportFromCsvRequest { formFile = csvFile });
        }

        /// <summary>
        /// Delete Acme Product
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="id"></param>
        /// <returns>int</returns>
        [HttpPost("DeletePermanent")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<int>> DeletePermanent([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                    [FromHeader(Name = "X-Request-Id")] string requestId,
                                                    [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                    [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                    [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                    DeletePermanentAcmeProductCommand command)
        {
            // mediator's send method will call the DeletePermanentAcmeProductCommand
            await Mediator.Send(command);

            // return HttpStatusCode OK 
            return (int)HttpStatusCode.OK;
        }
    }
}
