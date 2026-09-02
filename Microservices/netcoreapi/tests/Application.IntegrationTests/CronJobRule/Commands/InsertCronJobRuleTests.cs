using System;
using System.Threading.Tasks;
using Application.CronJobRule.Commands.DeleteCronJobRule;
using Application.CronJobRule.Commands.InsertCronJobRule;
using Application.CronJobRule.Queries.GetCronJobRuleById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.CronJobRule.Commands
{
    using static Testing;

    [TestFixture]
    public class InsertCronJobRuleTests : BaseTestFixture
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

        [Test, Order(0)]
        public async Task ShouldInsertCronJobRule()
        {
            var command = new InsertCronJobRuleCommand
            {
                NotificationName = $"TestRule-{Guid.NewGuid():N}".Substring(0, 24),
                Frequency = "Daily",
                ExecutionTime = new TimeSpan(8, 0, 0),
                IsNotificationPaused = false
            };

            Id = await SendAsync(command);

            Id.Should().NotBeNullOrEmpty();

            var result = await SendAsync(new GetCronJobRuleByIdQuery { Id = Id });
            result.Should().NotBeNull();
            result.NotificationName.Should().Be(command.NotificationName);
            result.Frequency.Should().Be(command.Frequency);
        }

        [Test, Order(1)]
        public async Task ShouldInsertCronJobRuleWithAllFields()
        {
            var command = new InsertCronJobRuleCommand
            {
                NotificationName = $"WeeklyRule-{Guid.NewGuid():N}".Substring(0, 24),
                Frequency = "Weekly",
                ExecutionTime = new TimeSpan(9, 30, 0),
                ExecutionDay = "Monday",
                ExecutionMonth = 0, // only pass value in Yearly Job (1 to 12)
                IsNotificationPaused = false
            };

            Id = await SendAsync(command);

            Id.Should().NotBeNullOrEmpty();
        }
    }
}
