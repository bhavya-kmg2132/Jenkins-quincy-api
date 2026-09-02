using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Application.ApiLog.Queries;
using Application.ApiLog.Queries.GetApiLogQuery;
using Application.ApiLog.Queries.GetApiRequestLogQuery;
using Application.RequestAndQueryName.Queries;
using Application.RequestAndQueryName.Queries.GetRequestAndQueryNameListQuery;
using Application.SystemManager.UpdateActionPermissionEndPoint;
using Application.UnlistedRequestAndQueryName.Queries;
using Application.UnlistedRequestAndQueryName.Queries.GetUnlistedRequestAndQueryName;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;

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
    public class SystemManagerController : ApiControllerBase
    {
        [AllowAnonymous]
        [HttpGet("GetApiResponseTimeLogs")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(ApiRequestLogListVm))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<ApiRequestLogListVm>> GetApiResponseTimeLogs(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] string path,
            [FromQuery] long? minElapsedMs,
            [FromQuery] string source,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            return await Mediator.Send(new GetApiRequestLogQuery
            {
                From = from,
                To = to,
                Path = path,
                MinElapsedMs = minElapsedMs,
                Source = source,
                Page = page,
                PageSize = pageSize
            });
        }

        [AllowAnonymous]
        [HttpGet("GetAppInformationLogInJsonFormat")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<ApiLogListVm>> GetApiLogList()
        {
            //mediator's send method will call the GetApiLogQuery for reading the GetApiLog's list
            return await Mediator.Send(new GetApiLogQuery());
        }

        /// <summary>
        /// GetAppInformation
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [Route("GetAppInformationLogFile")]
        [AllowAnonymous]
        public ActionResult GetAppInformation()
        {
            try
            {
                string logFileName = "App-Information.log";
                var dllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                var dllParentPath = Path.GetDirectoryName(dllPath);
                var logFilePath = Path.Combine(dllParentPath, "Logs", logFileName);

                using (FileStream fileStream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (MemoryStream memoryStream = new MemoryStream())
                using (StreamWriter tw = new StreamWriter(memoryStream))
                using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string line = streamReader.ReadLine();
                        tw.WriteLine(line);
                    }

                    var length = memoryStream.Length;
                    var toWrite = new byte[length];

                    Array.Copy(memoryStream.ToArray(), 0, toWrite, 0, length);

                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd'T'HH':'mm':'ss");
                    string fileName = $"App-Information_{timestamp}.txt";

                    return File(toWrite, "text/plain", fileName);
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                return Content($"Error reading file: {ex.Message}");
            }
        }

        /// <summary>
        /// Get AppError Log File
        /// </summary>
        /// <returns>ActionResult</returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("GetAppErrorLogFile")]
        public ActionResult GetAppError()
        {
            try
            {
                string logsFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                string logFileName = "App-Error.log";
                string logFilePath = Path.Combine(logsFolderPath, logFileName);

                using (FileStream fileStream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (MemoryStream memoryStream = new MemoryStream())
                using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    streamReader.BaseStream.CopyTo(memoryStream);

                    byte[] fileContents = memoryStream.ToArray();

                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd'T'HH':'mm':'ss");
                    string fileName = $"App-Error_{timestamp}.txt";

                    return File(fileContents, "text/plain", fileName);
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                return Content($"Error reading file: {ex.Message}");
            }
        }

        /// <summary>
        /// GetLogFile
        /// </summary>
        /// <returns>ActionResult</returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("GetLogFile")]
        public ActionResult GetNLog()
        {
            try
            {
                string date = DateTime.Now.Date.ToString("yyyy-MM-dd");
                var filename = "nlog-AspNetCore-own-" + date + ".log";
                var dllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                var dllParentPath = Path.GetDirectoryName(dllPath);
                var logFilePath = Path.Combine(dllParentPath, "Logs", filename);

                using (FileStream fileStream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (MemoryStream memoryStream = new MemoryStream())
                using (StreamWriter tw = new StreamWriter(memoryStream))
                using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string line = streamReader.ReadLine();
                        tw.WriteLine(line);
                    }

                    var length = memoryStream.Length;
                    var toWrite = new byte[length];

                    Array.Copy(memoryStream.ToArray(), 0, toWrite, 0, length);

                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd'T'HH':'mm':'ss");
                    string fileName = $"nlog_{timestamp}.txt";

                    return File(toWrite, "text/plain", fileName);
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                return Content($"Error reading file: {ex.Message}");
            }
        }

        /// <summary>
        /// Get Request And Query Name List
        /// </summary>
        /// <returns>RequestAndQueryNameListVm</returns>

        [AllowAnonymous]
        [HttpGet("GetRequestAndQueryNameList")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(RequestAndQueryNameListVm))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<RequestAndQueryNameListVm>> GetRequestAndQueryNameList()
        {
            //mediator's send method will call the GetRequestAndQueryNameListQuery
            return await Mediator.Send(new GetRequestAndQueryNameListQuery());
        }

        [HttpGet("GetUnlistedRequestAndQueryNameList")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(UnlistedRequestAndQueryNameListVm))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<UnlistedRequestAndQueryNameListVm>> GetAllUnlistedPermissions([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                               [FromHeader(Name = "X-Request-Id")] string requestId,
                                               [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                               [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                               [FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            // mediator's send method will call the GetConfigListQuery for reading the CustomConfig.
            return await Mediator.Send(new GetUnlistedRequestAndQueryNameListQuery { });
        }

        /// <summary>
        /// Discovers every MediatR-backed action across all controllers and inserts any not already present
        /// in the Permission table (matched by PermissionValue).
        /// </summary>
        [HttpPost]
        [Route("UpdateActionPermissionEndPoint")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(UpdateActionPermissionEndPointVm))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<UpdateActionPermissionEndPointVm>> UpdateActionPermissionEndPoint([FromServices] IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
                                                       [FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                       [FromHeader(Name = "X-Request-Id")] string requestId,
                                                       [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                       [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                       [FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            var items = new List<UpdateActionPermissionEndPointDto>();
            var seenRequestTypes = new HashSet<string>();

            foreach (var descriptor in actionDescriptorCollectionProvider.ActionDescriptors.Items.OfType<ControllerActionDescriptor>())
            {
                if (descriptor.AttributeRouteInfo?.Template == null)
                {
                    continue;
                }

                // Some actions (e.g. parameter-less GET list queries) build their MediatR request inline
                // instead of binding it as a parameter - the resolver falls back to a naming convention in that case.
                var requestType = ActionPermissionEndPointRouteResolver.ResolveRequestType(descriptor);

                if (requestType == null || !seenRequestTypes.Add(requestType.Name))
                {
                    continue;
                }

                items.Add(new UpdateActionPermissionEndPointDto
                {
                    PermissionValue = requestType.Name,
                    PermissionDisplayName = ToPermissionDisplayName(requestType.Name),
                    ActionPermissionEndPoint = ActionPermissionEndPointRouteResolver.ToActionPermissionEndPoint(descriptor.AttributeRouteInfo.Template)
                });
            }

            //mediator's send method will call the UpdateActionPermissionEndPointRequest to persist the discovered permissions
            return await Mediator.Send(new UpdateActionPermissionEndPointRequest { Items = items });
        }

        private static string ToPermissionDisplayName(string requestTypeName)
        {
            var withoutSuffix = Regex.Replace(requestTypeName, "(Request|Command|Query)$", "");
            return Regex.Replace(withoutSuffix, "(?<=[a-z])(?=[A-Z])", " ");
        }
    }
}





