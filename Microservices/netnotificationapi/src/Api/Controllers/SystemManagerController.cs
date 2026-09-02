using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Application.ApiLog.Queries;
using Application.ApiLog.Queries.GetApiLogQuery;
using Application.RequestAndQueryName.Queries;
using Application.RequestAndQueryName.Queries.GetRequestAndQueryNameListQuery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SystemManagerController : ApiControllerBase
    {
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
    }


}



