using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Dapper;
using Domain.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.DataAccess
{
    public class RefreshTokenDataAccess : IRefreshTokenDataAccess
    {
        private readonly ILogger<RefreshTokenDataAccess> _logger;
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, string> _sqlQueries;
        private readonly IConnectionHelper _connectionHelper;

        public RefreshTokenDataAccess(IConfiguration configuration, ILogger<RefreshTokenDataAccess> logger, IConnectionHelper connectionHelper)
        {
            _logger = logger;
            _configuration = configuration;
            _connectionHelper = connectionHelper;
            _sqlQueries = _connectionHelper.LoadSqlQueriesXml("RefreshToken");
        }
        private string GenerateToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public async Task<RefreshToken> GenerateRefreshToken(string userId)
        {
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = GenerateToken(),
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                CreatedDate = DateTime.UtcNow,
                IsRevoked = false
            };

            var sql = _sqlQueries["RefreshToken.Create"];

            using (var connection = _connectionHelper.CreateNetAuthConnection())
            {
                await connection.ExecuteAsync(sql, refreshToken);
            }

            return refreshToken;
        }

        public async Task<RefreshToken> GetStoredTokenByRefreshToken(string token)
        {
            var sql = _sqlQueries["RefreshToken.GetByToken"];

            using (var connection = _connectionHelper.CreateNetAuthConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<RefreshToken>(
                    sql,
                    new { Token = token });
            }
        }

        public async Task RevokeAsync(string token)
        {
            var sql = _sqlQueries["RefreshToken.Revoke"];

            using (var connection = _connectionHelper.CreateNetAuthConnection())
            {
                await connection.ExecuteAsync(sql,
                    new { Token = token, Now = DateTime.UtcNow });
            }
        }

        public async Task RevokeAllAsync(string userId)
        {
            var sql = _sqlQueries["RefreshToken.RevokeAll"];

            using (var connection = _connectionHelper.CreateNetAuthConnection())
            {
                await connection.ExecuteAsync(sql,
                    new { UserId = userId, Now = DateTime.UtcNow });
            }
        }

        public async Task<RefreshToken> RotateAsync(RefreshToken oldToken)
        {
            using (var connection = _connectionHelper.CreateNetAuthConnection())
            {
                // Revoke old
                var revokeSql = _sqlQueries["RefreshToken.RotateRevoke"];

                await connection.ExecuteAsync(revokeSql,
                    new { Id = oldToken.Id, Now = DateTime.UtcNow });

                // Create new
                var newToken = new RefreshToken
                {
                    UserId = oldToken.UserId,
                    Token = GenerateToken(),
                    ExpiryDate = DateTime.UtcNow.AddDays(7),
                    CreatedDate = DateTime.UtcNow,
                    IsRevoked = false
                };

                var createSql = _sqlQueries["RefreshToken.Create"];
                await connection.ExecuteAsync(createSql, newToken);

                return newToken;
            }
        }
    }
}
