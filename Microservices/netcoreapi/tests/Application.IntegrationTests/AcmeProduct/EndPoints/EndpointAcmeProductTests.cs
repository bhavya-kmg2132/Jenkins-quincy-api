using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Application.AcmeOrders.Commands.DeletePermanentAcmeProduct;
using Application.AcmeProduct.Commands.CreateAcmeProduct;
using Application.AcmeProduct.Commands.DeleteAcmeProduct;
using Application.AcmeProduct.Commands.UpdateAcmeProduct;
using NUnit.Framework;
using RestSharp;

namespace Application.IntegrationTests.AcmeProduct.EndPoints
{
    using static Testing;

    /// <summary>
    /// Integration tests for AcmeProductController
    ///   POST api/v1/AcmeProduct/Create
    ///   POST api/v1/AcmeProduct/Update
    ///   POST api/v1/AcmeProduct/Delete
    ///   GET  api/v1/AcmeProduct/GetList
    ///   GET  api/v1/AcmeProduct/GetById?id=...
    ///
    /// Lifecycle: Create in test → store Id → Delete in [TearDown].
    /// </summary>
    [TestFixture]
    public class EndpointAcmeProductTests : EndpointTestBase
    {
        private const string Base = "api/v1/AcmeProduct";
        private static readonly JsonSerializerOptions JsonOpts =
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        private string _createdId;

        [TearDown]
        public async Task AcmeProductTearDown()
        {
            if (!string.IsNullOrEmpty(_createdId))
            {
                var req = Post(EndPointsSettings.ApiEndPoint.AcmeProductDeletePermanent);
                req.AddJsonBody(new DeletePermanentAcmeProductCommand { Id = _createdId });
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

        private async Task<string> CreateProductAsync(string name)
        {
            var req = Post($"{Base}/Create");
            req.AddJsonBody(new CreateAcmeProductRequest
            {
                Name = name,
                SKU = $"SKU-{Guid.NewGuid():N}".Substring(0, 20),
                CustomFields = new Dictionary<string, string> { { "test_field", "test_value" } }
            });
            var response = await Client.ExecuteAsync(req);
            if (response.StatusCode != HttpStatusCode.OK) return null;
            return JsonSerializer.Deserialize<string>(response.Content, JsonOpts);
        }

        private async Task DeleteProductAsync(string id)
        {
            var req = Post($"{Base}/Delete");
            req.AddJsonBody(new DeleteAcmeProductRequest { Id = id });
            await Client.ExecuteAsync(req);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Create
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        [Order(1)]
        public async Task Ep_AcmeProduct_Create_WithValidRequest_ShouldReturnId()
        {
            var req = Post($"{Base}/Create");
            req.AddJsonBody(new CreateAcmeProductRequest
            {
                Name = GetUniqueName(),
                SKU = $"SKU-{Guid.NewGuid():N}".Substring(0, 20),
                CustomFields = new Dictionary<string, string> { { "test_field", "value" } }
            });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            _createdId = JsonSerializer.Deserialize<string>(response.Content, JsonOpts);
            Assert.That(_createdId, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        [Order(2)]
        public async Task Ep_AcmeProduct_Create_WithEmptyName_ShouldNotReturnOk()
        {
            var req = Post($"{Base}/Create");
            req.AddJsonBody(new CreateAcmeProductRequest { Name = string.Empty });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.OK));
        }

        // ─────────────────────────────────────────────────────────────────────
        //  GetList
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        [Order(3)]
        public async Task Ep_AcmeProduct_GetList_ShouldReturnOk()
        {
            var req = Get($"{Base}/GetList");

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Is.Not.Null.And.Not.Empty);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  GetById
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        [Order(4)]
        public async Task Ep_AcmeProduct_GetById_WithExistingId_ShouldReturnOk()
        {
            _createdId = await CreateProductAsync(GetUniqueName());
            Assert.That(_createdId, Is.Not.Null, "Product must be created before GetById test.");

            var req = Get($"{Base}/GetById");
            req.AddQueryParameter("id", _createdId);

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Order(5)]
        public async Task Ep_AcmeProduct_GetById_WithNonExistentId_ShouldReturnNotFoundOrOkNull()
        {
            var req = Get($"{Base}/GetById");
            req.AddQueryParameter("id", Guid.NewGuid().ToString());

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK).Or.EqualTo(HttpStatusCode.NotFound).Or.EqualTo(HttpStatusCode.NoContent));
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Update
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        [Order(6)]
        public async Task Ep_AcmeProduct_Update_WithValidRequest_ShouldReturnOk()
        {
            _createdId = await CreateProductAsync(GetUniqueName());
            Assert.That(_createdId, Is.Not.Null, "Product must be created before Update test.");

            var req = Post($"{Base}/Update");
            req.AddJsonBody(new UpdateAcmeProductRequest
            {
                Id = _createdId,
                Name = GetUniqueName()
            });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Delete
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        [Order(7)]
        public async Task Ep_AcmeProduct_Delete_WithValidId_ShouldReturnOk()
        {
            // Create a dedicated product just to delete it (don't use _createdId so TearDown doesn't double-delete)
            var tempId = await CreateProductAsync(GetUniqueName());
            Assert.That(tempId, Is.Not.Null, "Product must be created before Delete test.");

            var req = Post($"{Base}/DeletePermanent");
            req.AddJsonBody(new DeletePermanentAcmeProductCommand { Id = tempId });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            // tempId already deleted — no TearDown needed


        }

        [Test]
        [Order(8)]
        public async Task Ep_AcmeProduct_Delete_WithNonExistentId_ShouldNotCrash()
        {
            var req = Post($"{Base}/Delete");
            req.AddJsonBody(new DeleteAcmeProductRequest { Id = Guid.NewGuid().ToString() });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.InternalServerError));
        }
    }
}
