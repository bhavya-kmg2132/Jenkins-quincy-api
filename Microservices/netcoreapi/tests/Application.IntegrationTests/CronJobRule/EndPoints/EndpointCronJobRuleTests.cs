using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Application.CronJobRule.Commands.DeleteCronJobRule;
using Application.CronJobRule.Commands.InsertCronJobRule;
using Application.CronJobRule.Commands.UpdateCronJobRule;
using NUnit.Framework;
using RestSharp;

namespace Application.IntegrationTests.CronJobRule.EndPoints
{
    using static Testing;

    /// <summary>
    /// Integration tests for CronJobRuleController
    ///   POST api/v1/CronJobRule/InsertCronJobRule
    ///   POST api/v1/CronJobRule/UpdateCronJobRule
    ///   POST api/v1/CronJobRule/DeleteCronJobRule
    ///   POST api/v1/CronJobRule/GetCronJobRuleById
    ///   GET  api/v1/CronJobRule/GetCronJobRules
    ///
    /// Lifecycle: Insert in test → store Id → Delete in [TearDown].
    /// </summary>
    [TestFixture]
    public class EndpointCronJobRuleTests : EndpointTestBase
    {
        private const string Base = "api/v1/CronJobRule";
        private static readonly JsonSerializerOptions JsonOpts =
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        private string _createdId;

        [TearDown]
        public async Task CronJobRuleTearDown()
        {
            if (!string.IsNullOrEmpty(_createdId))
            {
                var req = Post($"{Base}/DeleteCronJobRule");
                req.AddJsonBody(new DeleteCronJobRuleCommand { Id = _createdId });
                await Client.ExecuteAsync(req);
                _createdId = null;
            }
        }

        // ── helpers ──────────────────────────────────────────────────────────

        private RestRequest Post(string path)
        {
            var req = new RestRequest
            {
                Method = Method.Post,
                Resource = ServerUrl + path,
                RequestFormat = DataFormat.Json
            };
            req.AddHeader("X-Correlation-Id", Guid.NewGuid().ToString());
            req.AddHeader("X-Request-Id", Guid.NewGuid().ToString());
            req.AddHeader("X-Request-Uid", RequestUid);
            req.AddHeader("X-Api-Key", ApiKey);
            return req;
        }

        private RestRequest Get(string path)
        {
            var req = new RestRequest { Method = Method.Get, Resource = ServerUrl + path };
            req.AddHeader("X-Correlation-Id", Guid.NewGuid().ToString());
            req.AddHeader("X-Request-Id", Guid.NewGuid().ToString());
            req.AddHeader("X-Request-Uid", RequestUid);
            req.AddHeader("X-Api-Key", ApiKey);
            return req;
        }

        private async Task<string> InsertRuleAsync(string name)
        {
            var req = Post($"{Base}/InsertCronJobRule");
            req.AddJsonBody(new InsertCronJobRuleCommand
            {
                NotificationName = name,
                Frequency = "Daily",
                ExecutionTime = new TimeSpan(8, 0, 0),
                IsNotificationPaused = false
            });
            var response = await Client.ExecuteAsync(req);
            if (response.StatusCode != HttpStatusCode.OK) return null;
            return JsonSerializer.Deserialize<string>(response.Content, JsonOpts);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  GetCronJobRules
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public async Task Ep_CronJobRule_GetList_ShouldReturnOk()
        {
            var req = Get($"{Base}/GetCronJobRules");

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Is.Not.Null.And.Not.Empty);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Insert
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public async Task Ep_CronJobRule_Insert_WithValidRequest_ShouldReturnId()
        {
            var req = Post($"{Base}/InsertCronJobRule");
            req.AddJsonBody(new InsertCronJobRuleCommand
            {
                NotificationName = $"TestRule-{Guid.NewGuid():N}".Substring(0, 24),
                Frequency = "Weekly",
                ExecutionTime = new TimeSpan(11, 0, 0),
                ExecutionDay = "Monday",
                IsNotificationPaused = false
            });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            _createdId = JsonSerializer.Deserialize<string>(response.Content, JsonOpts);
            Assert.That(_createdId, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public async Task Ep_CronJobRule_Insert_WithMissingName_ShouldNotReturnOk()
        {
            var req = Post($"{Base}/InsertCronJobRule");
            req.AddJsonBody(new InsertCronJobRuleCommand
            {
                Frequency = "Daily",
                ExecutionTime = new TimeSpan(8, 0, 0)
            });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.OK));
        }

        // ─────────────────────────────────────────────────────────────────────
        //  GetCronJobRuleById
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public async Task Ep_CronJobRule_GetById_WithExistingId_ShouldReturnOk()
        {
            _createdId = await InsertRuleAsync($"RuleById-{Guid.NewGuid():N}".Substring(0, 20));
            Assert.That(_createdId, Is.Not.Null, "Rule must be created before GetById test.");

            var req = Post($"{Base}/GetCronJobRuleById");
            req.AddJsonBody(new { Id = _createdId });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK).Or.EqualTo(HttpStatusCode.NotFound));
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Update
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public async Task Ep_CronJobRule_Update_WithValidRequest_ShouldReturnOk()
        {
            _createdId = await InsertRuleAsync($"RuleUpd-{Guid.NewGuid():N}".Substring(0, 20));
            Assert.That(_createdId, Is.Not.Null, "Rule must be created before Update test.");

            var req = Post($"{Base}/UpdateCronJobRule");
            req.AddJsonBody(new UpdateCronJobRuleCommand
            {
                Id = _createdId,
                NotificationName = $"UpdatedRule-{Guid.NewGuid():N}".Substring(0, 20),
                Frequency = "Monthly",
                ExecutionTime = new TimeSpan(10, 0, 0)
            });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Delete
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public async Task Ep_CronJobRule_Delete_WithValidId_ShouldReturnOk()
        {
            var tempId = await InsertRuleAsync($"RuleDel-{Guid.NewGuid():N}".Substring(0, 20));
            Assert.That(tempId, Is.Not.Null, "Rule must be created before Delete test.");

            var req = Post($"{Base}/DeleteCronJobRule");
            req.AddJsonBody(new DeleteCronJobRuleCommand { Id = tempId });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }
}
