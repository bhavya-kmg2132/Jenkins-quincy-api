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
    public class DeleteCronJobRuleTests : BaseTestFixture
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
        public async Task ShouldDeleteCronJobRule()
        {
            // Arrange
            var id = await SendAsync(new InsertCronJobRuleCommand
            {
                NotificationName = $"RuleToDel-{Guid.NewGuid():N}".Substring(0, 24),
                Frequency = "Daily",
                ExecutionTime = new TimeSpan(8, 0, 0),
                IsNotificationPaused = false
            });
            id.Should().NotBeNullOrEmpty("rule must be created before delete test");

            // Act
            await SendAsync(new DeleteCronJobRuleCommand { Id = id });

            // Assert
            var result = await SendAsync(new GetCronJobRuleByIdQuery { Id = id });
            result.Should().BeNull();
        }

        [Test]
        public async Task ShouldNotThrowWhenDeletingNonExistentRule()
        {
            Func<Task> act = async () =>
                await SendAsync(new DeleteCronJobRuleCommand { Id = Guid.NewGuid().ToString() });

            await act.Should().NotThrowAsync();
        }
    }
}
