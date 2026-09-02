using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Users.Commands.UpdateUserPasswordHash;
using Domain.Common;
using Domain.Dto;
using Domain.Entities;
using NetAuth.Contract.DataContract.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    /// <summary>
    /// Used For Fetching and Decoding Token
    /// </summary>

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ApiControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        private readonly IIdentityService _identityService;
        private readonly IJwtService _jwtService;
        private readonly IAutoLoginStore _autoLoginStore;

        public AuthController(IConfiguration configuration, ILogger<AuthController> logger, IIdentityService identityService, IJwtService jwtService, IAutoLoginStore autoLoginStore)
        {
            _configuration = configuration;
            _logger = logger;
            _identityService = identityService;
            _jwtService = jwtService;
            _autoLoginStore = autoLoginStore;
        }

        /// <summary>
        /// Register
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<bool>> Register([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                       [FromHeader(Name = "X-Request-Id")] string requestId,
                                                       [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                       [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                       [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                       LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Username))
                return BadRequest("Username is required.");

            try
            {
                var created = await _identityService.CreateUserAsync(request.Username, request.Password, request.FirstName, request.LastName, request.Mobile, request.oid, request.auth_type);
                return Ok(new
                {
                    UserId = created,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Login
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<TokenModel>> Login([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                    [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                    [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                    [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                    [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                    LoginRequest request)
        {
            var user = await _identityService.ValidateIdentityUser(request.Username, request.Password);

            if (user == null)
                return Unauthorized();

            if (!user.IsActive)
                return Unauthorized("User account is inactive. Please contact administrator.");


            var accessToken = _jwtService.GenerateToken(user);
            var refreshToken = await _identityService.CreateRefreshToken(user.UserId);

            // 3?? Return both
            return Ok(new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        /// <summary>
        /// Update UserPasswordHash
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("UpdateUserPasswordHash")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<int>> UpdateUserPasswordHash([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                         [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                         [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                         [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                         [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                         UpdateUserPasswordHashRequest request)
        {
            // mediator's send method will call the UpdateUserPasswordHashRequest
            await Mediator.Send(request);

            return (int)HttpStatusCode.OK;
        }

        [AllowAnonymous]
        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken(RefreshRevokeRequest request)
        {
            if (request is null)
                return BadRequest("Invalid client request");

            var token = request.RefreshToken;

            var result = await _identityService.RefreshTokenAsync(token);

            if (result == null)
                return Unauthorized();

            return Ok(result);

        }

        [AllowAnonymous]
        [HttpPost("revoke")]
        public async Task<IActionResult> RevokeToken(RefreshRevokeRequest request)
        {
            await _identityService.RevokeTokenAsync(request.RefreshToken);
            return Ok();
        }


        [HttpGet("serverIp")]
        [AllowAnonymous]
        public IActionResult GetServerIp([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                   [FromHeader(Name = "X-Request-Id")] string requestId,
                                                   [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                   [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                   [FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            var localIp = HttpContext.Connection.LocalIpAddress?.ToString();
            var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (localIp == "::1") localIp = "127.0.0.1";
            if (remoteIp == "::1") remoteIp = "127.0.0.1";

            return Ok(new { localIp, remoteIp });
        }



        /// <summary>
        /// AutoLogin
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("internalAutoLogin")]
        public async Task<IActionResult> InternalAutoLogin([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                   [FromHeader(Name = "X-Request-Id")] string requestId,
                                                   [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                   [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                   [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                   [FromBody] string mintKey)
        {

            // Step 1: Verify API key
            var configuredApiKey = _configuration["Api:api-key"];
            if (!string.IsNullOrEmpty(configuredApiKey) && apiKey != configuredApiKey)
                return Unauthorized();

            //Step 2 : Check if key is valid or expired.
            var record = _autoLoginStore.ValidateAndConsume(mintKey);
            if (record == null)
                return Unauthorized("Invalid or expired key.");

            // Step 3: verify the username in X-Request-Uid from db
            //3.1) not empty & must match the userId stored in the key
            if (string.IsNullOrWhiteSpace(requestUid) || !record.UserId.Equals(requestUid, StringComparison.OrdinalIgnoreCase))
                return Unauthorized();

            //3.2) should exist in db
            var user = await _identityService.GetIdentityUserAsync(requestUid);
            if (user == null)
                return Unauthorized();

            //3.3) should be active
            if (!user.IsActive)
                return Unauthorized("User account is inactive. Please contact administrator.");

            //3.4) should be db type only
            if (!user.auth_type.ToUpper().Equals("DB"))
                return Unauthorized("User should be db type only");

            // Step 4: Auto login
            var accessToken = _jwtService.GenerateToken(user);
            var refreshToken = await _identityService.CreateRefreshToken(user.UserId);

            _logger.LogInformation("JWT token issued for user {UserId}", record.UserId);

            return Ok(new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        /// <summary>
        /// KeyMint
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="redirectUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("keyMint")]
        [ProducesResponseType(302)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> KeyMint(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            [FromBody] string redirectUrl)
        {
            //Step 1 : RedirectUrl (mandatory)
            //1.1) not empty
            if (string.IsNullOrWhiteSpace(redirectUrl))
                return BadRequest("RedirectUrl is required.");

            //1.2) match redirectUrl with appsettings
            var allowedRedirectUrls = _configuration.GetSection("AutoLogin:AllowedRedirectUrls").Get<string[]>() ?? [];
            if (!allowedRedirectUrls.Any(url => redirectUrl.ToLower().Contains(url.ToLower())))
                return BadRequest("Invalid RedirectUrl.");

            //Step 2 : API Key (mandatory)
            var configuredApiKey = _configuration["Api:api-key"];
            if (!string.IsNullOrEmpty(configuredApiKey) && apiKey != configuredApiKey)
                return Unauthorized();

            //Step 3 : Request Uid (mandatory)
            //3.1) not empty
            if (string.IsNullOrWhiteSpace(requestUid))
                return BadRequest("X-Request-Uid is required.");

            //3.2) should exist in db
            var user = await _identityService.GetIdentityUserAsync(requestUid);
            if (user == null)
                return Unauthorized();

            //3.3) User should be active
            if (!user.IsActive)
                return Unauthorized("User account is inactive. Please contact administrator.");

            //3.3) should be db type only
            if (!user.auth_type.ToUpper().Equals("DB"))
                return Unauthorized("User should be db type only");

            // Step 4 : Check if IP is allowed
            //4.1) Extract the server's ip
            var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrWhiteSpace(remoteIp))
                return Unauthorized();
            if (remoteIp == "::1") remoteIp = "127.0.0.1";

            //4.2) Check if IP is allowed
            var allowedIps = _configuration.GetSection("AutoLogin:AllowedIPs").Get<string[]>() ?? Array.Empty<string>();
            if (!allowedIps.Any(ip => ip.Equals(remoteIp, StringComparison.OrdinalIgnoreCase)))
                return Unauthorized();

            //Step 5 : Return Key
            var loginKey = _autoLoginStore.MintKey(requestUid);
            var returnUrl = $"{redirectUrl.TrimEnd('/')}?key={Uri.EscapeDataString(loginKey)}";

            _logger.LogInformation("AutoLogin key minted for user {UserId} from IP {RemoteIp}", requestUid, remoteIp);

            return Redirect(returnUrl);
        }

        /// <summary>
        /// QOL2.0 AutoLogin — Step 1.
        /// Called by QOL1.0 (AS400) to mint a one-time login key and receive the redirect URL.
        /// Validates API key, caller IP, redirect URL whitelist, and user existence.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("autologin")]
        [ProducesResponseType(302)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> AutoLogin(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromHeader(Name = "X-Request-Uid")] string requestUid,
            [FromHeader(Name = "X-Api-Key")] string apiKey,
            [FromBody] string redirectUrl)
        {
            if (string.IsNullOrWhiteSpace(redirectUrl))
                return BadRequest("RedirectUrl is required.");
            if (string.IsNullOrWhiteSpace(requestUid))
                return BadRequest("X-Request-Uid is required.");

            // Validate API key
            var configuredApiKey = _configuration["Api:api-key"];
            if (!string.IsNullOrEmpty(configuredApiKey) && apiKey != configuredApiKey)
            {
                _logger.LogWarning("AutoLogin rejected: X-Api-Key did not match configured Api:api-key.");
                return Unauthorized("Invalid API key.");
            }

            // Validate caller (AS400) IP against whitelist
            var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrWhiteSpace(remoteIp))
            {
                _logger.LogWarning("AutoLogin rejected: could not determine caller RemoteIpAddress.");
                return Unauthorized("Unable to determine caller IP.");
            }
            if (remoteIp == "::1") remoteIp = "127.0.0.1";

            var allowedIps = _configuration.GetSection("AutoLogin:AllowedIPs").Get<string[]>() ?? Array.Empty<string>();
            if (!allowedIps.Any(ip => ip.Equals(remoteIp, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarning("AutoLogin rejected: caller IP {RemoteIp} is not in AutoLogin:AllowedIPs.", remoteIp);
                return Unauthorized($"IP {remoteIp} is not allowed.");
            }

            // Validate redirect URL against whitelist
            var allowedRedirects = _configuration.GetSection("AutoLogin:AllowedRedirectUrls").Get<string[]>() ?? Array.Empty<string>();
            if (allowedRedirects.Length > 0 &&
                !allowedRedirects.Any(url => redirectUrl.StartsWith(url, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarning("AutoLogin rejected: redirectUrl {RedirectUrl} did not match AutoLogin:AllowedRedirectUrls.", redirectUrl);
                return Unauthorized("RedirectUrl is not allowed.");
            }

            // Verify user exists and is active
            var user = await _identityService.GetIdentityUserAsync(requestUid);
            if (user == null)
            {
                _logger.LogWarning("AutoLogin rejected: no identity user found for {RequestUid}.", requestUid);
                return Unauthorized("User not found.");
            }
            if (!user.IsActive)
                return Unauthorized("User account is inactive. Please contact administrator.");

            // Mint one-time login key and build redirect URL
            var loginKey = _autoLoginStore.MintKey(user.UserId);
            var returnUrl = $"{redirectUrl.TrimEnd('/')}?key={Uri.EscapeDataString(loginKey)}";

            _logger.LogInformation("QOL2.0 AutoLogin key minted for user {UserId} from IP {RemoteIp}", user.UserId, remoteIp);

            return Redirect(returnUrl);
        }

        /// <summary>
        /// QOL2.0 AutoLogin — Step 2.
        /// Called by the React SPA to exchange a one-time login key for a JWT.
        /// The key is validated and immediately consumed (single-use).
        /// </summary>
        [AllowAnonymous]
        [HttpPost("authorize-token")]
        [ProducesResponseType(200, Type = typeof(TokenModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> AuthorizeToken(
            [FromHeader(Name = "X-Correlation-Id")] string correlationId,
            [FromHeader(Name = "X-Request-Id")] string requestId,
            [FromBody] AuthorizeTokenRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Key))
                return BadRequest("Key is required.");

            // Validate and atomically consume the one-time login key
            var record = _autoLoginStore.ValidateAndConsume(request.Key);
            if (record == null)
                return Unauthorized("Invalid or expired key.");

            var user = await _identityService.GetIdentityUserAsync(record.UserId);
            if (user == null || !user.IsActive)
                return Unauthorized();

            var accessToken = _jwtService.GenerateToken(user);
            var refreshToken = await _identityService.CreateRefreshToken(user.UserId);

            _logger.LogInformation("QOL2.0 JWT issued for user {UserId}", record.UserId);

            return Ok(new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }
    }
}

