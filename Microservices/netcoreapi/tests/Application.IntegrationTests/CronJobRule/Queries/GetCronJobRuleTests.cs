using System;
using System.Threading.Tasks;
using Application.CronJobRule.Commands.DeleteCronJobRule;
using Application.CronJobRule.Commands.InsertCronJobRule;
using Application.CronJobRule.Queries.GetCronJobRuleById;
using Application.CronJobRule.Queries.GetCronJobRules;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.CronJobRule.Queries
{
    using static Testing;

    [TestFixture]
    public class GetCronJobRuleTests : BaseTestFixture
    {
        string Id = null;

        [SetUp]
        public async Task DerivedSetUp() => await Task.CompletedTask;

        [TearDown]
        public async Task DerivedTearDown()
        {
            if (Id != null)
            {
                await SendAsync(new DeleteCronJobRuleCommand { Id = Id });
                Id = null;
            }
        }

        [Test]
        public async Task ShouldReturnAllCronJobRules()
        {
            var result = await SendAsync(new GetCronJobRulesQuery());

            result.Should().NotBeNull();
        }

        [Test]
        public async Task ShouldReturnListContainingCreatedRule()
        {
            var ruleName = $"ListTest-{Guid.NewGuid():N}".Substring(0, 24);
            Id = await SendAsync(new InsertCronJobRuleCommand
            {
                NotificationName = ruleName,
                Frequency = "Daily",
                ExecutionTime = new TimeSpan(7, 0, 0),
                IsNotificationPaused = false
            });

            var result = await SendAsync(new GetCronJobRulesQuery());

            result.Should().Contain(r => r.NotificationName == ruleName);
        }

        [Test]
        public async Task ShouldReturnCronJobRuleById()
        {
            Id = await SendAsync(new InsertCronJobRuleCommand
            {
                NotificationName = $"ByIdTest-{Guid.NewGuid():N}".Substring(0, 24),
                Frequency = "Weekly",
                ExecutionTime = new TimeSpan(11, 0, 0),
                ExecutionDay = "Friday",
                IsNotificationPaused = false
            });

            var result = await SendAsync(new GetCronJobRuleByIdQuery { Id = Id });

            result.Should().NotBeNull();
            result.Id.Should().Be(Id);
        }

        [Test]
        public async Task ShouldReturnNullForNonExistentRuleById()
        {
            var result = await SendAsync(new GetCronJobRuleByIdQuery { Id = Guid.NewGuid().ToString() });

            result.Should().BeNull();
        }
    }
}
