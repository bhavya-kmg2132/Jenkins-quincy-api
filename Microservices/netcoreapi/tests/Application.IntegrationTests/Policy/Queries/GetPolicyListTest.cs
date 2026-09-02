using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Policy.Commands.CreatePolicy;
using Application.Policy.Queries.GetPolicyList;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.Policy.Queries
{
    using static Testing;

    [TestFixture]
    public class GetPolicyListTest : BaseTestFixture
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
        public async Task ShouldReturnPolicyList()
        {
            var query = new GetPolicyListQuery();
            var result = await SendAsync(query);

            result.Should().NotBeNull();
            result.PolicyList.Should().NotBeNull();
            result.PolicyList.Should().HaveCountGreaterThan(0);
        }

        [Test]
        public async Task ShouldIncludeNewlyCreatedPolicyInList()
        {
            // 1. Create
            var insuredName = GetUniqueName();
            policyId = await SendAsync(new CreatePolicyRequest
            {
                InsuredName = insuredName,
                PolicyType = "Marine",
                StatusCode = "Active",
                EffectiveDate = DateTime.UtcNow.Date,
                ExpirationDate = DateTime.UtcNow.Date.AddYears(1)
            });

            // 2. Query list
            var query = new GetPolicyListQuery();
            var result = await SendAsync(query);

            // 3. Assert created policy appears in list
            result.PolicyList.Should().NotBeNull();
            var found = result.PolicyList.FirstOrDefault(p => p.Id == policyId);
            found.Should().NotBeNull();
            found.InsuredName.Should().Be(insuredName);
            found.PolicyType.Should().Be("Marine");
            found.LineOfBusinessCode.Should().Be("MCA");
        }
    }
}
