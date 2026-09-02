using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace Api.Controllers
{
    /// <summary>
    /// Controller class handles incoming HTTP requests and send response back to the caller.
    /// </summary>
    //AllowAnonymous :negates the Authorize Attribute and allows anonymous access.
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class HeartbeatController : ApiControllerBase
    {
        private readonly ILogger<HeartbeatController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public HeartbeatController(ILogger<HeartbeatController> logger, IConfiguration configuration, IWebHostEnvironment env)
        {
            _logger = logger;
            _configuration = configuration;
            _environment = env;
        }

        /// <summary>
        /// Get heartbeat for rater
        /// </summary>
        /// <returns>string</returns>
        [HttpGet]
        [MapToApiVersion("1.0")]
        public IEnumerable<string> Get([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                       [FromHeader(Name = "X-Request-Id")] string requestId,
                                       [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                       [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                       [FromHeader(Name = "X-Api-Key")] string requestApiKey)
        {
            SqlConnectionStringBuilder sqlconnectionbuilder = new SqlConnectionStringBuilder(this._configuration["ConnectionStrings:SqlDBConnection"]);
            var dbName = sqlconnectionbuilder.InitialCatalog;
            var dbServer = sqlconnectionbuilder.DataSource;
            string apiKey = this._configuration["Api:api-key"];
            string timeStampId = Convert.ToString(Guid.NewGuid());

            var api_information = new string[] {
                "Api name: " + this._configuration["Api:internal_name"],
                "Api Code: "+ this._configuration["Api:code"],
                "Api Key: " + apiKey,
                "TimeStampId: " + timeStampId,
                "Product: " + Convert.ToString(Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyProductAttribute>().Product),
                //"Description: " + Convert.ToString(Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyDescriptionAttribute>().Description),
                "Company: " + Convert.ToString(Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyCompanyAttribute>().Company),
                //"Copyright: " + Convert.ToString(Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright),
                "Product/Package Version (InformationalVersion): " + Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion,
                "File Version: " + Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version,
                "Environment: " + this._environment.EnvironmentName,
                "Database Server: " + dbServer,
                "Database Name: "+ dbName};

            _logger.LogInformation(string.Join(Environment.NewLine, api_information));

            return api_information;
        }


        /// <summary>
        /// Get not found exception
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet("NotFoundResult")]
        [MapToApiVersion("1.0")]
        public ActionResult NotFoundResult()
        {
            return new NotFoundResult();
        }

        /// <summary>
        /// Get no content result
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet("NoContentResult")]
        [MapToApiVersion("1.0")]
        public ActionResult NoContentResult()
        {
            return new NoContentResult();
        }

        /// <summary>
        /// Get bad request result
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet("BadRequestResult")]
        [MapToApiVersion("1.0")]
        public ActionResult BadRequestResult()
        {
            return new BadRequestResult();
        }

        /// <summary>
        /// Get internal server error result
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet("InternalServerErrorResult")]
        [MapToApiVersion("1.0")]
        public ActionResult InternalServerErrorResult()
        {
            return InternalServerErrorResult();
        }

        /// <summary>
        /// Get unprocessable entity result
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet("UnprocessableEntityResult")]
        [MapToApiVersion("1.0")]
        public ActionResult UnprocessableEntityResult()
        {
            return new UnprocessableEntityResult();
        }

        /// <summary>
        /// Get UnauthorizedResult result
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet("UnauthorizedResult")]
        [MapToApiVersion("1.0")]
        public ActionResult UnauthorizedResult()
        {
            return new UnauthorizedResult();
        }

        /// <summary>
        /// Get Ok Result
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet("OkResult")]
        [MapToApiVersion("1.0")]
        public ActionResult OkResult()
        {
            return new OkResult();
        }

        [HttpGet("TestMethod")]
        [MapToApiVersion("1.0")]
        public IActionResult TestMethodV1() => Ok("This is V1");

        [HttpGet("TestMethod")]
        [MapToApiVersion("2.0")]
        public IActionResult TestMethodV2() => Ok("This is V2");

    }
}






