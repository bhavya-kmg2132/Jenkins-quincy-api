using Application.Users.Queries.AccessLevel;
using FluentAssertions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Application.IntegrationTests.User.Queries
{
    using static Testing;

    [TestFixture]
    public class GetUserAccessLevelQueryTest : BaseTestFixture
    {
        [SetUp]
        public void DerivedSetUp() { }

        [TearDown]
        public void DerivedTearDown() { }

        [Test]
        public async Task ShouldReturnUserAccessLevel()
        {
            var query = new GetUserAccessLevelQuery();

            var result = await SendAsync(query);

            result.UserAccessLevelList.Should().HaveCountGreaterThan(0);
        }
    }
}

