using System;
using System.Threading.Tasks;
using Application.CronJobRule.Commands.DeleteCronJobRule;
using Application.CronJobRule.Commands.InsertCronJobRule;
using Application.CronJobRule.Commands.UpdateCronJobRule;
using Application.CronJobRule.Queries.GetCronJobRuleById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.CronJobRule.Commands
{
    using static Testing;

    [TestFixture]
    public class UpdateCronJobRuleTests : BaseTestFixture
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
        public async Task ShouldUpdateCronJobRule()
        {
            // Arrange
            Id = await SendAsync(new InsertCronJobRuleCommand
            {
                NotificationName = $"OriginalRule-{Guid.NewGuid():N}".Substring(0, 24),
                Frequency = "Daily",
                ExecutionTime = new TimeSpan(8, 0, 0),
                IsNotificationPaused = false
            });
            Id.Should().NotBeNullOrEmpty("rule must be created before update test");

            // Act
            var updatedName = $"UpdatedRule-{Guid.NewGuid():N}".Substring(0, 24);
            await SendAsync(new UpdateCronJobRuleCommand
            {
                Id = Id,
                NotificationName = updatedName,
                Frequency = "Monthly",
                ExecutionTime = new TimeSpan(10, 0, 0)
            });

            // Assert
            var result = await SendAsync(new GetCronJobRuleByIdQuery { Id = Id });
            result.Should().NotBeNull();
            result.NotificationName.Should().Be(updatedName);
            result.Frequency.Should().Be("Monthly");
        }
    }
}
