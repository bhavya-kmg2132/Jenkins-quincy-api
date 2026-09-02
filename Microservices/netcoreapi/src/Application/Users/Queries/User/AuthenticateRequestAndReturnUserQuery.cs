using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.User
{
    /// <summary>
    /// class AuthenticateRequestAndReturnUserQuery extends the IRequest interface of MediatR
    /// </summary>
    public class AuthenticateRequestAndReturnUserQuery : IRequest<UserVm>
    {
    }

    /// <summary>
    /// For Creating handler for the above request, created AuthenticateRequestAndReturnUserQueryHandler class
    /// that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class AuthenticateRequestAndReturnUserQueryHandler : IRequestHandler<AuthenticateRequestAndReturnUserQuery, UserVm>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserDataAccess _dataAccess;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public string AuthorizationToken => _httpContextAccessor.HttpContext?.Request.Headers["Authorization"];
        private string Oid;


        /// <summary>
        ///  Instantiates AuthenticateRequestAndReturnUserQueryHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        /// <param name="dataAccess"></param>
        /// <param name="httpContextAccessor"></param>
        public AuthenticateRequestAndReturnUserQueryHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUserDataAccess dataAccess, IHttpContextAccessor httpContextAccessor)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._dataAccess = dataAccess;
            this._mapper = mapper;
            this._httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Handler will receive request, process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>UserVm</returns>
        public async Task<UserVm> Handle(AuthenticateRequestAndReturnUserQuery request, CancellationToken cancellationToken)
        {
            //1. Logging Information - In process
            _logger.LogInformation("AuthenticateRequestAndReturnUserQuery - In process");

            #region Get oid from Authentication Token and Return User
            try
            {
                //2. a sample jwt encoded token string which is supposed to be extracted from 'Authorization' HTTP header in your Web Api controller
                //var tokenString = "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSJ9.eyJhdWQiOiIwMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDAiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8wMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDAiLCJpYXQiOiIxNDI4MDM2NTM5IiwibmJmIjoiMTQyODAzNjUzOSIsImV4cCI6IjE0MjgwNDA0MzkiLCJ2ZXIiOiIxLjAiLCJ0aWQiOiIwMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDAiLCJhbXIiOiJwd2QiLCJvaWQiOiIwMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDAiLCJlbWFpbCI6Impkb2VAbGl2ZS5jb20iLCJwdWlkIjoiSm9obiBEb2UiLCJpZHAiOiJsaXZlLmNvbSIsImFsdHNlY2lkIjoiMTpsaXZlLmNvbTowMDAwMDAwMDAwMDAwMDAwIiwic3ViIjoieHh4eHh4eHh4eHh4eHh4eC15eXl5eSIsImdpdmVuX25hbWUiOiJKb2huIiwiZmFtaWx5X25hbWUiOiJEb2UiLCJuYW1lIjoiSm9obiBEb2UiLCJncm91cHMiOiIwMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDAiLCJ1bmlxdWVfbmFtZSI6ImxpdmUuY29tI2pkb2VAbGl2ZS5jb20iLCJhcHBpZCI6IjAwMDAwMDAwLTAwMDAtMDAwMC0wMDAwLTAwMDAwMDAwMDAwMCIsImFwcGlkYWNyIjoiMCIsInNjcCI6InVzZXJfaW1wZXJzb25hdGlvbiIsImFjciI6IjEifQ.K7BCa0NO-A5f9exFiWcIXFMGnLmmt3V2HVP0itMT-GsAxnQROWzJFDIQNFo4QhiW0NCCqJykVELeVBCy_7Dex2-szUPZ69rmmDVJhy_qkmAiHhS1mNZDvJ1sB-whb5wOJ_QPIlByVzubhTcNnuliTVjnTeuOurVJJcn0Vugx9UDkGgky0etHXzmKukWYp4nzA68Wf1xnzlMZBz7PfoPGhjgzQfceOkZJVXIBRMB_7tsyW7gYNbHB_aTiT47cEjkh-UdrZEdp2UaAKugC-es3m076kRHMJqx31x-zDLDBttKinRJVPctiqwb1jMOMV6cUAp2E6aMfEbNk_iqX_OKFJg";
                if (string.IsNullOrEmpty(AuthorizationToken))
                {
                    return null;
                }

                //3. get access token from bearer
                var jwtEncodedString = AuthorizationToken;
                if (AuthorizationToken.Contains("Bearer"))
                {
                    jwtEncodedString = AuthorizationToken.Substring(7); // trim 'Bearer ' from the start since its just a prefix for the token string
                }

                var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);

                //4. Get claims from jwt token
                Oid = token.Claims.FirstOrDefault(c => c.Type == "oid")?.Value;
                //string preferred_username = token.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
                string name = token.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
                string Scope = token.Claims.FirstOrDefault(c => c.Type == "scp")?.Value;

                //5. if oid is not in claim then return null 
                if (string.IsNullOrEmpty(Oid))
                    return null;

                //6. Return user based on oid
                else
                {
                    //6.1. Returns User
                    return new UserVm
                    {
                        User = _mapper.Map<UserDto>(await _dataAccess.GetUserFromDbAsync(Oid))
                    };
                }
            }

            catch (Exception ex)
            {
                //2. Logging LogError - Error
                _logger.LogError("AuthenticateRequestAndReturnUserQuery - Error: " + ex.Message);

                throw;
            }

            #endregion 
        }
    }
}

