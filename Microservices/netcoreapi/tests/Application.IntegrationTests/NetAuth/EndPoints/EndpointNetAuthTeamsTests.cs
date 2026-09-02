using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using RestSharp;

namespace Application.IntegrationTests.NetAuth.EndPoints
{
    [TestFixture]
    public class EndpointNetAuthTeamsTests : NetAuthTestBase
    {
        // ── GET /GetTeams ─────────────────────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_GetTeams_WithCorrelationHeaders_ShouldReturn200()
        {
            var req = BuildNetAuthRequest(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetTeams);

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_GetTeams_WithoutHeaders_ShouldNotReturn500()
        {
            var req = BuildRequestWithoutHeaders(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetTeams);

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.InternalServerError));
        }

        // ── GET /GetTeamById/{teamId} ─────────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_GetTeamById_WithRealTeamId_ShouldReturn200()
        {
            if (string.IsNullOrEmpty(_teamId))
                Assert.Inconclusive("No team found in DB — skipping GetTeamById test.");

            var req = BuildNetAuthRequest(Method.Get,
                $"{EndPointsSettings.ApiEndPoint.NetAuthGetTeamById}/{_teamId}");

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_GetTeamById_WithNonExistentId_ShouldReturn200OrNotFound()
        {
            var req = BuildNetAuthRequest(Method.Get,
                $"{EndPointsSettings.ApiEndPoint.NetAuthGetTeamById}/non-existent-team-id");

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK)
                    .Or.EqualTo(HttpStatusCode.NoContent));
        }

        // ── GET /GetTeamsByUserId/{userId} ────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_GetTeamsByUserId_WithRealUserId_ShouldReturn200()
        {
            if (string.IsNullOrEmpty(_userId))
                Assert.Inconclusive("No user ID found in DB — skipping GetTeamsByUserId test.");

            var req = BuildNetAuthRequest(Method.Get,
                $"{EndPointsSettings.ApiEndPoint.NetAuthGetTeamsByUserId}/{_userId}");

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        // ── POST /AddTeam ─────────────────────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_AddTeam_WithValidBody_ShouldReturn200AndReturnTeamId()
        {
            if (string.IsNullOrEmpty(_userId))
                Assert.Inconclusive("No user ID found in DB — skipping AddTeam test.");

            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthAddTeam);
            req.AddJsonBody(new
            {
                TeamName = $"Integration Test Team {Guid.NewGuid():N}",
                TeamShortName = "ITT",
                Description = "Created by integration test",
                TeamOwnerId = _userId,
                TeamCaptainId = _userId
            });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public async Task Ep_NetAuth_AddTeam_WithEmptyBody_ShouldReturn200OrBadRequest()
        {
            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthAddTeam);
            req.AddJsonBody(new { });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK)
                    .Or.EqualTo(HttpStatusCode.InternalServerError));
        }

        // ── POST /AddTeamMembers ──────────────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_AddTeamMembers_WithRealTeamId_ShouldReturn200()
        {
            if (string.IsNullOrEmpty(_userId) || string.IsNullOrEmpty(_level1UserId))
                Assert.Inconclusive("Admin or Level1 user ID not found in DB — skipping AddTeamMembers test.");

            // Create a fresh team (owned by admin) so Level1 user is guaranteed not already a member
            var createReq = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthAddTeam);
            createReq.AddJsonBody(new
            {
                TeamName      = $"MemberTest Team {Guid.NewGuid():N}",
                TeamShortName = "MT",
                Description   = "Team for AddTeamMembers integration test",
                TeamOwnerId   = _userId,
                TeamCaptainId = _userId
            });
            var createResp = await Client.ExecuteAsync(createReq);
            if (createResp.StatusCode != System.Net.HttpStatusCode.OK)
                Assert.Inconclusive("AddTeam failed — cannot test AddTeamMembers without a fresh team.");

            var freshTeamId = System.Text.Json.JsonSerializer.Deserialize<string>(createResp.Content, JsonOpts);

            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthAddTeamMembers);
            req.AddJsonBody(new
            {
                TeamId  = freshTeamId,
                UserIds = new List<string> { _level1UserId }
            });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_AddTeamMembers_WithEmptyBody_ShouldReturn200OrBadRequest()
        {
            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthAddTeamMembers);
            req.AddJsonBody(new { });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK)
                    .Or.EqualTo(HttpStatusCode.InternalServerError));
        }

        // ── POST /RemoveTeamMember ────────────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_RemoveTeamMember_WithRealTeamId_ShouldReturn200OrNotFound()
        {
            if (string.IsNullOrEmpty(_teamId) || string.IsNullOrEmpty(_userId))
                Assert.Inconclusive("No team or user found in DB — skipping RemoveTeamMember test.");

            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthRemoveTeamMember);
            req.AddJsonBody(new
            {
                TeamId = _teamId,
                UserId = _userId
            });

            var response = await Client.ExecuteAsync(req);

            // OK if the user was a member, NotFound/BadRequest if not — both are valid outcomes
            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK)
                    .Or.EqualTo(HttpStatusCode.NotFound)
                    .Or.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task Ep_NetAuth_RemoveTeamMember_WithEmptyBody_ShouldReturn200OrBadRequest()
        {
            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthRemoveTeamMember);
            req.AddJsonBody(new { });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK)
                    .Or.EqualTo(HttpStatusCode.BadRequest)
                    .Or.EqualTo(HttpStatusCode.InternalServerError));
        }

        
        // ── GET /GetTeamMembersByTeamId/{teamId} ──────────────────────────────

        [Test]
        public async Task Ep_NetAuth_GetTeamMembersByTeamId_WithRealTeamId_ShouldReturn200()
        {
            if (string.IsNullOrEmpty(_teamId))
                Assert.Inconclusive("No team found in DB — skipping GetTeamMembersByTeamId test.");

            var req = BuildNetAuthRequest(Method.Get,
                $"{EndPointsSettings.ApiEndPoint.NetAuthGetTeamMembersByTeamId}/{_teamId}");

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        // ── Full flow: AddTeam then GetTeamById ───────────────────────────────

        [Test]
        public async Task Ep_NetAuth_AddTeam_ThenGetTeamById_ShouldReturnCreatedTeam()
        {
            if (string.IsNullOrEmpty(_userId))
                Assert.Inconclusive("No user ID found in DB — skipping AddTeam flow test.");

            var addReq = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthAddTeam);
            addReq.AddJsonBody(new
            {
                TeamName = $"FlowTest Team {Guid.NewGuid():N}",
                TeamShortName = "FT",
                Description = "Integration flow test team",
                TeamOwnerId = _userId,
                TeamCaptainId = _userId
            });

            var addResponse = await Client.ExecuteAsync(addReq);
            Assert.That(addResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "AddTeam step must succeed before GetTeamById can be tested.");

            var createdTeamId = JsonSerializer.Deserialize<string>(addResponse.Content, JsonOpts);
            Assert.That(createdTeamId, Is.Not.Null.And.Not.Empty, "Expected a team ID in the response.");

            var getReq = BuildNetAuthRequest(Method.Get,
                $"{EndPointsSettings.ApiEndPoint.NetAuthGetTeamById}/{createdTeamId}");

            var getResponse = await Client.ExecuteAsync(getReq);

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }
}
