using System;
using System.Threading.Tasks;
using Application.VersionTrack.Commands.AddVersionTrack;
using Application.VersionTrack.Queries.GetVersionTrack;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.VersionTrack.Queries
{
    using static Testing;

    [TestFixture]
    public class GetVersionTrackTests : BaseTestFixture
    {
        [SetUp]
        public async Task DerivedSetUp() => await Task.CompletedTask;

        [TearDown]
        public void DerivedTearDown() { }

        [Test, Order(1)]
        public async Task ShouldReturnVersionTrackList()
        {
            var result = await SendAsync(new GetVersionTrackQuery());

            result.Should().NotBeNull();
            result.VersionTrackList.Should().NotBeNull();
        }

        [Test, Order(2)]
        public async Task ShouldReturnListWithAtLeastOneEntryAfterAdd()
        {
            await SendAsync(new AddVersionTrackRequest
            {
                PlatformType = "API",
                VersionNumber = $"3.{rnd.Next(0, 999)}.0",
                ReleaseDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                ReleaseNotes = "Query test entry",
                ReleasedBy = RequestUid,
                ReleasedTo = "All"
            });

            var result = await SendAsync(new GetVersionTrackQuery());

            result.VersionTrackList.Should().HaveCountGreaterThan(0);
        }
    }
}
