using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NetAuth.Contract.DataContract.Entities;

namespace Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public string preferred_username { get; set; }
        public string oid { get; set; }
        public List<string> UserRoles { get; set; }
        public string Scope { get; set; }
        public string name { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public string BusinessUnit { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string AccessLevel { get; set; }
        public string display_name { get; set; }
        public string token_decoded_uid { get; set; }

        private readonly ILogger<CurrentUserService> _logger;
        private readonly IIdentityService _identityService;
        private readonly IConfiguration _configuration;
        public string CorrelationId => string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext?.Request.Headers["X-Correlation-Id"]) ?
                                            Guid.NewGuid().ToString() : _httpContextAccessor.HttpContext?.Request.Headers["X-Correlation-Id"];
        public string RequestId => string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext?.Request.Headers["X-Request-Id"]) ?
                                            Guid.NewGuid().ToString() : _httpContextAccessor.HttpContext?.Request.Headers["X-Request-Id"];
        public string RequestOid => $"{_httpContextAccessor.HttpContext?.Request.Headers["X-Request-Oid"] ?? string.Empty}";

        public string RequestUid => $"{_httpContextAccessor.HttpContext?.Request.Headers["X-Request-Uid"] ?? string.Empty}";
        public string ApiKey => $"{_httpContextAccessor.HttpContext?.Request.Headers["X-Api-Key"] ?? string.Empty}";

        public string AuthorizationToken => _httpContextAccessor.HttpContext?.Request.Headers.Authorization;
        IdentityUser identityUserfromDb = null;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor, ILogger<CurrentUserService> logger, IIdentityService identityService, IConfiguration configuration)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _identityService = identityService;
            _configuration = configuration;
            this.oid = RequestOid;
            ValidateApiKey();
        }

        public void ValidateApiKey()
        {
            string apiKey = this._configuration["Api:api-key"];

            if (!string.IsNullOrEmpty(apiKey))
            {
                //string Request_X_Api_Key = $"{_httpContextAccessor.HttpContext?.Request.Headers["X-Api-CacheKey"] ?? string.Empty}";

                if (!_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("X-Api-Key", out var extractedApiKey))
                {
                    _logger.LogError("Error CurrentUserService-ValidateApiKey: Valid API key is missing in request!");
                    throw new UnauthorizedAccessException("Error CurrentUserService-ValidateApiKey: Valid API key is missing in request!");
                }

                //var appSettings = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

                if (!apiKey.Equals(extractedApiKey))
                {
                    _logger.LogError("Error CurrentUserService-ValidateApiKey: Unauthorized client! Invalid Api Key!");
                    throw new UnauthorizedAccessException("Error CurrentUserService-ValidateApiKey: Unauthorized client! Invalid Api Key!");
                }
            }
        }

        //This method will always be called as all microservice need to have a valid user.
        public async Task<bool> ValidateRequestUser()
        {
            if (string.IsNullOrEmpty(CorrelationId) && string.IsNullOrEmpty(RequestId))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(this.RequestOid) || !string.IsNullOrEmpty(RequestUid))
            {
                await GetIdentityUser();
                await ValidateCurrentUserRoles();
                return true;
            }

            // This part of code will be executed if both RequestOid and RequestUid are empty.
            // So in this case, API will try to directly validate through the token and Gateway is not performing AUTH.
            if (_configuration.GetSection("Api:SelfAuthentication").Get<bool>())
            {
                await DecodeToken(AuthorizationToken);
                if (!String.IsNullOrEmpty(this.oid))
                {
                    this.identityUserfromDb = await _identityService.GetIdentityUserAsync(this.oid);

                }
                else if (!String.IsNullOrEmpty(this.token_decoded_uid))
                {
                    this.identityUserfromDb = await _identityService.GetIdentityUserAsync(this.token_decoded_uid);
                }

                await LoadCurrentIdentityUserAttributes(identityUserfromDb);
                await ValidateCurrentUserRoles();
                return true;
            }

            throw new UnauthorizedAccessException();

            //return false;
        }

        private async Task<bool> GetIdentityUser()
        {
            try
            {
                if (!String.IsNullOrEmpty(this.RequestUid))
                {
                    this.identityUserfromDb = await _identityService.GetIdentityUserAsync(this.RequestUid);

                    if (this.identityUserfromDb == null)
                    {
                        _logger.LogError("Error CurrentUserService.GetIdentityUser:" + "GetIdentityUserAsync: identityUserfromDb is NULL for X-Request-Uid: " + this.RequestUid);
                        throw new UnauthorizedAccessException("User not found in database for X-Request-Uid: " + this.RequestUid);
                    }

                    await LoadCurrentIdentityUserAttributes(identityUserfromDb);
                }
                else if (!String.IsNullOrEmpty(this.RequestOid))
                {
                    this.identityUserfromDb = await _identityService.GetIdentityUserAsync(this.RequestOid);

                    if (this.identityUserfromDb == null)
                    {
                        _logger.LogError("Error CurrentUserService.GetIdentityUser:" + "GetIdentityUserAsync: identityUserfromDb is NULL for X-Request-Oid: " + this.RequestOid);
                        throw new UnauthorizedAccessException("User not found in database for X-Request-Oid: " + this.RequestOid);
                    }

                    await LoadCurrentIdentityUserAttributes(identityUserfromDb);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error CurrentUserService.GetIdentityUser:" + ex.Message + " || X-Request-Oid: " + this.RequestOid + "|| X-Request-Uid: " + this.RequestUid);
                throw;
            }
        }

        private async Task<bool> LoadCurrentIdentityUserAttributes(IdentityUser localUserfromDb)
        {
            try
            {
                if (localUserfromDb != null)
                {
                    this.UserRoles = new List<string>();
                    this.display_name = localUserfromDb.display_name;
                    foreach (var role in localUserfromDb.UserRoles)
                    {
                        this.UserRoles.Add(role.RoleName);
                    }
                    this.UserName = localUserfromDb.UserName;
                    this.UserId = localUserfromDb.UserId;
                    this.oid = localUserfromDb.oid;
                    this.Mobile = localUserfromDb.Mobile;
                    this.display_name = localUserfromDb.display_name;
                    this.name = localUserfromDb.given_name;
                    this.family_name = localUserfromDb.family_name;
                    this.BusinessUnit = localUserfromDb.BusinessUnit;
                    this.Email = localUserfromDb.Email;
                    this.given_name = localUserfromDb.given_name;
                    this.AccessLevel = localUserfromDb.AccessLevel;
                    this.Position = localUserfromDb.Position;
                    this.preferred_username = localUserfromDb.preferred_username;
                    this.IsDeleted = localUserfromDb.IsDeleted;
                    this.IsActive = localUserfromDb.IsActive;
                    return true;
                }
                await Task.CompletedTask;
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<bool> DecodeToken(string tokenString)
        {
            try
            {
                if (string.IsNullOrEmpty(tokenString))
                {
                    return false;
                }

                var jwtEncodedString = tokenString;
                if (tokenString.Contains("Bearer"))
                {
                    jwtEncodedString = tokenString.Substring(7); // trim 'Bearer ' from the start since its just a prefix for the token string
                }

                var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
                UserRoles = new List<string>();


                this.oid = token.Claims.FirstOrDefault(c => c.Type == "oid")?.Value;
                this.preferred_username = token.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
                this.name = token.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
                this.Scope = token.Claims.FirstOrDefault(c => c.Type == "scp")?.Value;
                this.Email = token?.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                var unique_name = token?.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
                this.UserName = token.Claims.FirstOrDefault(c => c.Type == "username")?.Value;

                string uid = Convert.ToString(this.UserName);

                if (string.IsNullOrEmpty(uid))
                    uid = this.Email;

                if (string.IsNullOrEmpty(uid))
                {
                    uid = preferred_username;
                }

                //Again check if uid is still empty. It can be the case if preferred_username is empty
                if (string.IsNullOrEmpty(uid))
                {
                    uid = unique_name;
                }

                //Assign the final uid to token_decoded_uid
                token_decoded_uid = uid;

                foreach (var item in token.Claims.Where(c => c.Type == "roles"))
                {
                    this.UserRoles.Add(item.Value);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error CurrentUserService-DecodeToken:" + ex.Message);
                throw;
            }
        }

        public async Task<bool> ValidateUserToken()
        {
            if (!string.IsNullOrEmpty(AuthorizationToken))
            {
                await DecodeToken(AuthorizationToken);
                return true;
            }
            return false;
        }

        private async Task<bool> ValidateCurrentUserRoles()
        {
            if (!this.UserRoles.Any())
            {
                _logger.LogError("Error CurrentUserService.ValidateCurrentUserRoles:- " + "No roles assigned for UserName:" + this.identityUserfromDb.UserName);
                throw new UnauthorizedAccessException("Error CurrentUserService.ValidateCurrentUserRoles:- " + "No roles assigned for UserName:" + this.identityUserfromDb.UserName);
            }
            await Task.CompletedTask;
            return true;
        }

        public async Task<bool> HasPermissionAsync(string permissionName)
        {
            if (identityUserfromDb == null)
            {
                return false;
            }

            this.display_name = identityUserfromDb.display_name;
            bool hasRequestPermission = await _identityService.AuthHasRequestPermissionAsync(this.UserName, permissionName);
            if (!hasRequestPermission)
            {
                return false;
            }

            return true;
        }
    }
}
