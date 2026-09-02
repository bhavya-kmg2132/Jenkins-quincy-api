using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class FireForgetController : ApiControllerBase
    {
        private readonly ILogger<HeartbeatController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public FireForgetController(ILogger<HeartbeatController> logger, IConfiguration configuration, IWebHostEnvironment env)
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
        public void Execute([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                       [FromHeader(Name = "X-Request-Id")] string requestId,
                                       [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                       [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                       [FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            // Fire off the task, but don't await the result
            Task.Run(async () =>
            {
                // Exceptions must be caught
                try
                {
                    _logger.LogInformation("Fire Forget Invoked!");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    _logger.LogError("Fire Forget Error!");
                }
            });
        }
    }
}
