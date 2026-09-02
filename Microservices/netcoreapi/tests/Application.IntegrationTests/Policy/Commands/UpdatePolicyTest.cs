using System;
using System.Threading.Tasks;
using Application.Policy.Commands.CreatePolicy;
using Application.Policy.Commands.UpdatePolicy;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.Policy.Commands
{
    using static Testing;

    [TestFixture]
    public class UpdatePolicyTest : BaseTestFixture
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
        public async Task ShouldUpdatePolicy()
        {
            // 1. Create
            policyId = await SendAsync(new CreatePolicyRequest
            {
                InsuredName = GetUniqueName(),
                PolicyType = "Marine",
                StatusCode = "Active",
                EffectiveDate = DateTime.UtcNow.Date,
                ExpirationDate = DateTime.UtcNow.Date.AddYears(1),
                TotalPremium = 5000m,
                Currency = "USD"
            });

            // Capture PolicyNumber — must not change after update
            var dataAccess = GetPolicyDataAccess();
            var created = await dataAccess.GetPolicyById(policyId);
            var originalPolicyNumber = created.PolicyNumber;

            // 2. Update
            var updateRequest = new UpdatePolicyRequest
            {
                Id = policyId,
                InsuredName = GetUniqueName(),
                InsuredAddress = "456 Updated Ave, New York, NY",
                PolicyType = "Cargo",
                StatusCode = "Active",
                EffectiveDate = DateTime.UtcNow.Date,
                ExpirationDate = DateTime.UtcNow.Date.AddYears(2),
                TotalPremium = 9500m,
                Currency = "USD",
                CargoType = "Perishable Goods"
            };
            await SendAsync(updateRequest);

            // 3. Verify
            var updated = await dataAccess.GetPolicyById(policyId);

            updated.Should().NotBeNull();
            updated.Id.Should().Be(policyId);
            updated.InsuredName.Should().Be(updateRequest.InsuredName);
            updated.InsuredAddress.Should().Be(updateRequest.InsuredAddress);
            updated.PolicyType.Should().Be(updateRequest.PolicyType);
            updated.ExpirationDate.Should().Be(updateRequest.ExpirationDate);
            updated.TotalPremium.Should().Be(updateRequest.TotalPremium);
            updated.CargoType.Should().Be(updateRequest.CargoType);

            // PolicyNumber is immutable — must not change
            updated.PolicyNumber.Should().Be(originalPolicyNumber);

            // LineOfBusinessCode is always MCA
            updated.LineOfBusinessCode.Should().Be("MCA");
        }

        [Test]
        public async Task ShouldUpdatePolicy_PreservingPolicyNumber()
        {
            policyId = await SendAsync(new CreatePolicyRequest
            {
                InsuredName = GetUniqueName(),
                PolicyType = "Aviation",
                StatusCode = "Active",
                EffectiveDate = DateTime.UtcNow.Date,
                ExpirationDate = DateTime.UtcNow.Date.AddYears(1)
            });

            var dataAccess = GetPolicyDataAccess();
            var created = await dataAccess.GetPolicyById(policyId);
            var originalPolicyNumber = created.PolicyNumber;

            await SendAsync(new UpdatePolicyRequest
            {
                Id = policyId,
                InsuredName = GetUniqueName(),
                PolicyType = "Aviation",
                StatusCode = "Active",
                EffectiveDate = DateTime.UtcNow.Date,
                ExpirationDate = DateTime.UtcNow.Date.AddYears(1),
                AircraftRegistration = "N99999",
                RiskDescription = "Updated risk description"
            });

            var updated = await dataAccess.GetPolicyById(policyId);
            updated.PolicyNumber.Should().Be(originalPolicyNumber);
            updated.AircraftRegistration.Should().Be("N99999");
            updated.RiskDescription.Should().Be("Updated risk description");
        }
    }
}
