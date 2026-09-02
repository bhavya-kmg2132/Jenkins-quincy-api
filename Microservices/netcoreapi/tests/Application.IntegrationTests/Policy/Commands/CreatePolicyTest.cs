using System;
using System.Threading.Tasks;
using Application.Policy.Commands.CreatePolicy;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.Policy.Commands
{
    using static Testing;

    [TestFixture]
    public class CreatePolicyTest : BaseTestFixture
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
        public async Task ShouldCreatePolicy()
        {
            var request = new CreatePolicyRequest
            {
                InsuredName = GetUniqueName(),
                InsuredAddress = "123 Test Street, Miami, FL",
                PolicyType = "Marine",
                StatusCode = "Active",
                EffectiveDate = DateTime.UtcNow.Date,
                ExpirationDate = DateTime.UtcNow.Date.AddYears(1),
                TotalPremium = 5000m,
                Currency = "USD"
            };

            policyId = await SendAsync(request);

            policyId.Should().NotBeNullOrEmpty();

            var dataAccess = GetPolicyDataAccess();
            var policy = await dataAccess.GetPolicyById(policyId);

            policy.Should().NotBeNull();
            policy.Id.Should().Be(policyId);
            policy.InsuredName.Should().Be(request.InsuredName);
            policy.PolicyType.Should().Be(request.PolicyType);
            policy.StatusCode.Should().Be(request.StatusCode);
            policy.EffectiveDate.Should().Be(request.EffectiveDate);
            policy.ExpirationDate.Should().Be(request.ExpirationDate);
            policy.LineOfBusinessCode.Should().Be("MCA");
            policy.PolicyNumber.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task ShouldCreatePolicy_WithCargoType()
        {
            var request = new CreatePolicyRequest
            {
                InsuredName = GetUniqueName(),
                PolicyType = "Cargo",
                StatusCode = "Active",
                EffectiveDate = DateTime.UtcNow.Date,
                ExpirationDate = DateTime.UtcNow.Date.AddYears(1),
                CargoType = "General Cargo",
                RouteFrom = "Miami",
                RouteTo = "London"
            };

            policyId = await SendAsync(request);

            policyId.Should().NotBeNullOrEmpty();

            var dataAccess = GetPolicyDataAccess();
            var policy = await dataAccess.GetPolicyById(policyId);

            policy.Should().NotBeNull();
            policy.PolicyType.Should().Be("Cargo");
            policy.CargoType.Should().Be(request.CargoType);
            policy.RouteFrom.Should().Be(request.RouteFrom);
            policy.RouteTo.Should().Be(request.RouteTo);
        }

        [Test]
        public async Task ShouldCreatePolicy_WithAviationType()
        {
            var request = new CreatePolicyRequest
            {
                InsuredName = GetUniqueName(),
                PolicyType = "Aviation",
                StatusCode = "Active",
                EffectiveDate = DateTime.UtcNow.Date,
                ExpirationDate = DateTime.UtcNow.Date.AddYears(1),
                AircraftRegistration = "N12345",
                FlightNumber = "UA100"
            };

            policyId = await SendAsync(request);

            policyId.Should().NotBeNullOrEmpty();

            var dataAccess = GetPolicyDataAccess();
            var policy = await dataAccess.GetPolicyById(policyId);

            policy.Should().NotBeNull();
            policy.PolicyType.Should().Be("Aviation");
            policy.AircraftRegistration.Should().Be(request.AircraftRegistration);
        }
    }
}
