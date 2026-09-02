using System;
using System.Threading.Tasks;
using Application.Policy.Commands.CreatePolicy;
using Application.Policy.Queries.GetPolicyById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.Policy.Queries
{
    using static Testing;

    [TestFixture]
    public class GetPolicyByIdTest : BaseTestFixture
    {
        string policyId = null;

        [SetUp]
        public async Task DerivedSetUp()
        {
            await Task.CompletedTask;
        }

        [TearDown]
        public async Task DerivedTearDown()
        {
            if (policyId != null)
            {
                await GetPolicyDataAccess().PermanentDelete(policyId);
                policyId = null;
            }
        }

        [Test]
        public async Task ShouldReturnPolicyById()
        {
            // 1. Create
            var createRequest = new CreatePolicyRequest
            {
                InsuredName = GetUniqueName(),
                InsuredAddress = "789 Query Lane, Chicago, IL",
                PolicyType = "Marine",
                StatusCode = "Active",
                EffectiveDate = DateTime.UtcNow.Date,
                ExpirationDate = DateTime.UtcNow.Date.AddYears(1),
                TotalPremium = 7500m,
                SumInsured = 500000m,
                Currency = "USD",
                ProducerCode = "PROD-001",
                VesselName = "MV Test Vessel",
                VesselType = "Bulk Carrier"
            };
            policyId = await SendAsync(createRequest);

            // 2. Query via MediatR
            var query = new GetPolicyByIdQuery { Id = policyId };
            var result = await SendAsync(query);

            // 3. Assert DTO fields
            result.Should().NotBeNull();
            result.Id.Should().Be(policyId);
            result.InsuredName.Should().Be(createRequest.InsuredName);
            result.InsuredAddress.Should().Be(createRequest.InsuredAddress);
            result.PolicyType.Should().Be(createRequest.PolicyType);
            result.StatusCode.Should().Be(createRequest.StatusCode);
            result.EffectiveDate.Should().Be(createRequest.EffectiveDate);
            result.ExpirationDate.Should().Be(createRequest.ExpirationDate);
            result.TotalPremium.Should().Be(createRequest.TotalPremium);
            result.SumInsured.Should().Be(createRequest.SumInsured);
            result.Currency.Should().Be(createRequest.Currency);
            result.ProducerCode.Should().Be(createRequest.ProducerCode);
            result.VesselName.Should().Be(createRequest.VesselName);
            result.VesselType.Should().Be(createRequest.VesselType);
            result.LineOfBusinessCode.Should().Be("MCA");
            result.PolicyNumber.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task ShouldReturnNull_WhenPolicyDoesNotExist()
        {
            var query = new GetPolicyByIdQuery { Id = "00000000-0000-0000-0000-000000000000" };
            var result = await SendAsync(query);

            result.Should().BeNull();
        }
    }
}
