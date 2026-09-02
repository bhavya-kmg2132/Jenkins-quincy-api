using System;
using System.Data.Common;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Application.IntegrationTests.EndPoints;
using Dapper;
using Domain.Entities;
using Microsoft.Data.SqlClient;
using NetAuth.Contract.DataContract.Dto;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using RestSharp;

namespace Application.IntegrationTests.Auth.EndPoints
{
    using static Testing;

    /// <summary>
    /// Shared infrastructure for all Auth endpoint test fixtures.
    /// Extend this class instead of EndpointTestBase directly to get:
    ///   — DB-aware teardown via IUserDataAccess
    ///   — Register / Login / BuildRequest helpers
    ///   — Cached JsonSerializerOptions
    /// </summary>
    public abstract class AuthTestBase : EndpointTestBase
    {
        protected IConfiguration _configuration;

        // ── Cached serializer options (avoids CA1869) ────────────────────────
        protected static readonly JsonSerializerOptions JsonOpts =
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // ── DB provider — mirrors "Api:SqlDatabaseServer" in appsettings.json ─
        // Values: "MsSqlServer" (default) | "PostgreSql"
        protected static string DatabaseProvider =>
            Testing.Configuration?["Api:SqlDatabaseServer"] ?? "MsSqlServer";

        // ─────────────────────────────────────────────────────────────────────
        //  REST helpers
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Builds a RestRequest pre-wired with ServerUrl, JSON format, and
        /// optionally the standard correlation headers all Auth endpoints expect.
        /// </summary>
        protected RestRequest BuildRequest(Method method, string endpointPath, bool withCorrelationHeaders = false)
        {
            var req = new RestRequest
            {
                Method = method,
                Resource = ServerUrl + endpointPath,
                RequestFormat = DataFormat.Json
            };

            if (withCorrelationHeaders)
            {
                req.AddHeader("X-Correlation-Id", Guid.NewGuid().ToString());
                req.AddHeader("X-Request-Id", Guid.NewGuid().ToString());
                req.AddHeader("X-Request-Uid", RequestUid);
                req.AddHeader("X-Api-Key", ApiKey);
            }

            return req;
        }

        /// <summary>
        /// Calls POST /Auth/register and returns true on HTTP 200.
        /// </summary>
        protected async Task<bool> RegisterUserViaEndpointAsync(
            string username,
            string password,
            string firstName = "Integration",
            string lastName = "Test",
            string authType = "db")
        {
            var req = BuildRequest(Method.Post, EndPointsSettings.ApiEndPoint.AuthRegister, withCorrelationHeaders: true);
            req.AddJsonBody(new LoginRequest
            {
                Username = username,
                Password = password,
                FirstName = firstName,
                LastName = lastName,
                Mobile = "0000000000",
                auth_type = authType
            });

            var response = await Client.ExecuteAsync(req);
            return response.StatusCode == HttpStatusCode.OK;
        }

        /// <summary>
        /// Calls POST /Auth/login and returns the TokenModel, or null on failure.
        /// </summary>
        protected async Task<TokenModel> LoginAsync(string username, string password)
        {
            var req = BuildRequest(Method.Post, EndPointsSettings.ApiEndPoint.AuthLogin, withCorrelationHeaders: true);
            req.AddJsonBody(new LoginRequest { Username = username, Password = password });

            var response = await Client.ExecuteAsync(req);
            if (response.StatusCode != HttpStatusCode.OK) return null;

            return JsonSerializer.Deserialize<TokenModel>(response.Content, JsonOpts);
        }


        // ─────────────────────────────────────────────────────────────────────
        //  DB teardown helpers
        // ─────────────────────────────────────────────────────────────────────

        // Opens a connection to the NetAuth database using the provider that
        // matches Api:SqlDatabaseServer in appsettings.json.
        private DbConnection OpenNetAuthConnection()
        {
            _configuration ??= new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionKey = DatabaseProvider == "PostgreSql"
                ? "PostgreSqlDBConnection"
                : "SqlDBConnection";

            var cs = _configuration[$"NetAuth.ConnectionStrings:{connectionKey}"];

            if (DatabaseProvider == "PostgreSql")
                return new Npgsql.NpgsqlConnection(cs);

            return new SqlConnection(cs);
        }

        // Returns table/column tokens appropriate for the active DB provider.
        // PostgreSQL: schema."TableName" — explicit schema + case-preserving quotes.
        // MsSqlServer: [TableName] — tables live in dbo (no schema prefix needed).
        private string Table(string schema, string name) =>
            DatabaseProvider == "PostgreSql"
                ? $@"{schema}.""{name}"""
                : $"[{name}]";

        private string Col(string name) =>
            DatabaseProvider == "PostgreSql" ? $@"""{name}""" : $"[{name}]";

        /// <summary>
        /// Deletes only <c>UserActivity</c> rows for the given user without
        /// touching the user itself.  Safe to call for shared test users.
        /// Swallows exceptions so teardown never masks a test failure.
        /// </summary>
        protected async Task CleanUpUserActivityByUsernameAsync(string username)
        {
            if (string.IsNullOrEmpty(username)) return;

            TestContext.WriteLine(
                $"[TearDown] Cleaning activity for '{username}' (provider={DatabaseProvider})");

            try
            {
                var user = await _userDataAccess.GetUserFromNetAuthLibAsync(username);
                if (user?.Id == null)
                {
                    TestContext.WriteLine($"[TearDown] User '{username}' not found — skipping.");
                    return;
                }

                var p = new { UserId = user.Id };

                await using var connection = OpenNetAuthConnection();
                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    $"DELETE FROM {Table("app", "UserActivity")} WHERE {Col("UserId")} = @UserId", p);

                await connection.ExecuteAsync(
                    $"DELETE FROM {Table("app", "RefreshToken")} WHERE {Col("UserId")} = @UserId", p);

                TestContext.WriteLine($"[TearDown] Cleaned activity for '{username}' (Id={user.Id}).");
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"[TearDown] Warning: could not clean activity for '{username}': {ex.Message}");
            }
        }

        /// <summary>
        /// Fully removes a test user and all dependent rows from the NetAuth DB
        /// inside a transaction.  Use only for users created by the test itself —
        /// never call this on the shared admin/test users.
        /// Swallows exceptions so teardown never masks a test failure.
        /// </summary>
        protected async Task DeactivateUserByUsernameAsync(string username)
        {
            if (string.IsNullOrEmpty(username)) return;

            TestContext.WriteLine(
                $"[TearDown] Deactivating '{username}' (provider={DatabaseProvider})");

            try
            {
                var user = await _userDataAccess.GetUserFromNetAuthLibAsync(username);
                if (user?.Id == null)
                {
                    TestContext.WriteLine($"[TearDown] User '{username}' not found — skipping.");
                    return;
                }

                await using var connection = OpenNetAuthConnection();
                await connection.OpenAsync();

                await using var transaction = await connection.BeginTransactionAsync();
                try
                {
                    var p = new { UserId = user.Id };

                    await connection.ExecuteAsync(
                        $"DELETE FROM {Table("app", "UserActivity")} WHERE {Col("UserId")} = @UserId", p, transaction);

                    await connection.ExecuteAsync(
                        $"DELETE FROM {Table("app", "UserPasswordHash")} WHERE {Col("UserId")} = @UserId", p, transaction);

                    await connection.ExecuteAsync(
                        $"DELETE FROM {Table("app", "UserProfile")} WHERE {Col("UserId")} = @UserId", p, transaction);

                    await connection.ExecuteAsync(
                        $"DELETE FROM {Table("app", "PermissionDenied")} WHERE {Col("UserId")} = @UserId", p, transaction);

                    await connection.ExecuteAsync(
                        $"DELETE FROM {Table("app", "PermissionGranted")} WHERE {Col("UserId")} = @UserId", p, transaction);

                    await connection.ExecuteAsync(
                        $"DELETE FROM {Table("app", "UserRole")} WHERE {Col("UserId")} = @UserId", p, transaction);

                    await connection.ExecuteAsync(
                        $"DELETE FROM {Table("app", "RefreshToken")} WHERE {Col("UserId")} = @UserId", p, transaction);

                    await connection.ExecuteAsync(
                        $"DELETE FROM {Table("app", "User")} WHERE {Col("Id")} = @UserId", p, transaction);

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }

                TestContext.WriteLine($"[TearDown] Deactivated '{username}' (Id={user.Id}).");
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"[TearDown] Warning: could not deactivate '{username}': {ex.Message}");
            }
        }
    }
}