using System;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Policy.Commands.CreatePolicy;
using Application.Policy.Commands.DeletePolicy;
using FluentAssertions;
using NUnit.Framework;
using PolicyEntity = Domain.Entities.Policy;

namespace Application.IntegrationTests.Policy.Commands
{
    using static Testing;

    [TestFixture]
    public class DeletePolicyTest : BaseTestFixture
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
            // Only needed if the delete test itself failed; the success path already deletes
            if (policyId != null)
            {
                await GetPolicyDataAccess().PermanentDelete(policyId);
                policyId = null;
            }
        }

        [Test]
        public async Task ShouldDeletePolicy()
        {
            // 1. Create
            policyId = await SendAsync(new CreatePolicyRequest
            {
                InsuredName = GetUniqueName(),
                PolicyType = "Marine",
                StatusCode = "Active",
                EffectiveDate = DateTime.UtcNow.Date,
                ExpirationDate = DateTime.UtcNow.Date.AddYears(1)
            });

            // 2. Delete
            var deleteRequest = new DeletePolicyRequest
            {
                Policy = new PolicyEntity { Id = policyId }
            };
            await SendAsync(deleteRequest);

            // 3. Verify soft-deleted: GetPolicyById returns null for IsDeleted = true records
            var dataAccess = GetPolicyDataAccess();
            var policy = await dataAccess.GetPolicyById(deleteRequest.Policy.Id);

            policy.Should().BeNull();
        }

        [Test]
        public async Task ShouldThrowNotFoundException_WhenPolicyDoesNotExist()
        {
            var deleteRequest = new DeletePolicyRequest
            {
                Policy = new PolicyEntity { Id = "00000000-0000-0000-0000-000000000000" }
            };

            Func<Task> act = async () => await SendAsync(deleteRequest);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Test]
        public async Task ShouldThrowValidationException_WhenDeletingCancelledPolicy()
        {
            // 1. Create a Cancelled policy
            policyId = await SendAsync(new CreatePolicyRequest
            {
                InsuredName = GetUniqueName(),
                PolicyType = "Cargo",
                StatusCode = "Cancelled",
                EffectiveDate = DateTime.UtcNow.Date,
                ExpirationDate = DateTime.UtcNow.Date.AddYears(1)
            });

            // 2. Attempt to delete — domain rule IsPolicyDeletable should reject this
            var deleteRequest = new DeletePolicyRequest
            {
                Policy = new PolicyEntity { Id = policyId }
            };

            Func<Task> act = async () => await SendAsync(deleteRequest);

            await act.Should().ThrowAsync<ValidationException>();
        }
    }
}
