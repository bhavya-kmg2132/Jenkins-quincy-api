using Application.Policy.Commands.CreatePolicy;
using Application.Policy.Commands.UpdatePolicy;
using Application.Policy.Queries.GetPolicyById;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Application.IntegrationTests.Policy.FieldPermissions
{
    using static Testing;

    [TestFixture]
    public class FieldPermissionBehaviourTest : BaseTestFixture
    {
        private string policyId = null;
        private const decimal InitialTotalPremium = 5000m;

        [SetUp]
        public async Task DerivedSetUp()
        {
            // BaseTestFixture.TestSetUp already called ResetState + RunAsAdminUserAsync,
            // so we are already running as admin here.
            policyId = await SendAsync(new CreatePolicyRequest
            {
                InsuredName = GetUniqueName(),
                PolicyType = "Marine",
                StatusCode = "Active",
                EffectiveDate = DateTime.UtcNow.Date,
                ExpirationDate = DateTime.UtcNow.Date.AddYears(1),
                TotalPremium = InitialTotalPremium,
                Currency = "USD"
            });
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

        // ── GET — View masking via FieldPermissionBehaviour pipeline ─────────────

        [Test]
        public async Task GetPolicy_AdminUser_TotalPremiumIsVisible()
        {
            // Already running as AdminTestUsername from BaseTestFixture setup.
            var result = await SendAsync(new GetPolicyByIdQuery { Id = policyId });

            result.Should().NotBeNull();
            result.TotalPremium.Should().Be(InitialTotalPremium);
        }

        [Test]
        public async Task GetPolicy_Level2User_TotalPremiumIsMaskedToNull()
        {
            // Switch to Level2TestUsername — lacks TotalPremium.View permission.
            await ResetState();
            await RunAsLevel2UserAsync();

            var result = await SendAsync(new GetPolicyByIdQuery { Id = policyId });

            result.Should().NotBeNull();
            result.TotalPremium.Should().BeNull("Level2 user lacks Core.Policy.TotalPremium.View");
        }

        [Test]
        public async Task GetPolicy_Level2User_NonRestrictedFieldsAreNotMasked()
        {
            await ResetState();
            await RunAsLevel2UserAsync();

            var result = await SendAsync(new GetPolicyByIdQuery { Id = policyId });

            result.Should().NotBeNull();
            result.Id.Should().Be(policyId);
            result.PolicyType.Should().Be("Marine");
            result.StatusCode.Should().Be("Active");
            result.Currency.Should().Be("USD");
            result.LineOfBusinessCode.Should().Be("MCA");
            result.PolicyNumber.Should().NotBeNullOrEmpty();
        }

        // ── UPDATE — Edit permission via FieldPermissionBehaviour + ApplyEditPermissionsAsync ──

        [Test]
        public async Task UpdatePolicy_AdminUser_TotalPremiumIsUpdated()
        {
            // Already running as AdminTestUsername.
            const decimal newPremium = 9000m;

            await SendAsync(BuildUpdateRequest(policyId, totalPremium: newPremium));

            var saved = await GetPolicyDataAccess().GetPolicyById(policyId);
            saved.TotalPremium.Should().Be(newPremium);
        }

        [Test]
        public async Task UpdatePolicy_Level2User_TotalPremiumIsSilentlyReverted()
        {
            await ResetState();
            await RunAsLevel2UserAsync();

            // Attempt to change TotalPremium — Level2 lacks edit permission.
            await SendAsync(BuildUpdateRequest(policyId, totalPremium: 9999m));

            var saved = await GetPolicyDataAccess().GetPolicyById(policyId);
            saved.TotalPremium.Should().Be(InitialTotalPremium,
                "TotalPremium must revert to the original DB value when user lacks edit permission");
        }

        [Test]
        public async Task UpdatePolicy_Level2User_NonRestrictedFieldsAreStillUpdated()
        {
            await ResetState();
            await RunAsLevel2UserAsync();

            var newInsuredName = GetUniqueName();

            await SendAsync(new UpdatePolicyRequest
            {
                Id = policyId,
                InsuredName = newInsuredName,
                PolicyType = "Cargo",
                StatusCode = "Active",
                EffectiveDate = DateTime.UtcNow.Date,
                ExpirationDate = DateTime.UtcNow.Date.AddYears(1),
                TotalPremium = 9999m,   // attempted change — will be silently reverted
                Currency = "USD"
            });

            var saved = await GetPolicyDataAccess().GetPolicyById(policyId);
            saved.InsuredName.Should().Be(newInsuredName, "unrestricted field must update normally");
            saved.PolicyType.Should().Be("Cargo", "unrestricted field must update normally");
            saved.TotalPremium.Should().Be(InitialTotalPremium, "TotalPremium must revert — Level2 lacks edit permission");
        }

        [Test]
        public async Task UpdatePolicy_AdminUser_SameTotalPremiumNoChange()
        {
            // Already running as AdminTestUsername.
            await SendAsync(BuildUpdateRequest(policyId, totalPremium: InitialTotalPremium));

            var saved = await GetPolicyDataAccess().GetPolicyById(policyId);
            saved.TotalPremium.Should().Be(InitialTotalPremium);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static UpdatePolicyRequest BuildUpdateRequest(string id, decimal totalPremium)
            => new UpdatePolicyRequest
            {
                Id = id,
                InsuredName = GetUniqueName(),
                PolicyType = "Marine",
                StatusCode = "Active",
                EffectiveDate = DateTime.UtcNow.Date,
                ExpirationDate = DateTime.UtcNow.Date.AddYears(1),
                TotalPremium = totalPremium,
                Currency = "USD"
            };
    }
}
