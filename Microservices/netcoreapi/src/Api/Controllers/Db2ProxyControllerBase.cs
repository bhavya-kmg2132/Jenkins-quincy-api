using Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public abstract class Db2ProxyControllerBase : ApiControllerBase
    {
        protected IActionResult FromExternalResponse(ExternalPolicyResponse response)
        {
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(response.StatusCode, response.Content);
            }

            return Content(response.Content, "application/json");
        }
    }
}
