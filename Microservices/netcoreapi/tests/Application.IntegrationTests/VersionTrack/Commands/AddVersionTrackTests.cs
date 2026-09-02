using System;
using System.Threading.Tasks;
using Application.VersionTrack.Commands.AddVersionTrack;
using Application.VersionTrack.Queries.GetVersionTrack;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.VersionTrack.Commands
{
    using static Testing;

    [TestFixture]
    public class AddVersionTrackTests : BaseTestFixture
    {
        [SetUp]
        public async Task DerivedSetUp() => await Task.CompletedTask;

        [TearDown]
        public void DerivedTearDown() { }
        // VersionTrack is an append-only audit log — no delete endpoint exists.

        [Test, Order(1)]
        public async Task ShouldAddVersionTrack()
        {
            var command = new AddVersionTrackRequest
            {
                PlatformType = "API",
                VersionNumber = $"1.{rnd.Next(0, 999)}.{rnd.Next(0, 999)}",
                ReleaseDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                ReleaseNotes = "Integration test release",
                ReleasedBy = RequestUid,
                ReleasedTo = "All"
            };

            var id = await SendAsync(command);

            id.Should().NotBeNullOrEmpty();
        }

        [Test, Order(2)]
        public async Task ShouldAddVersionTrackAndAppearInList()
        {
            var version = $"2.{rnd.Next(0, 999)}.{rnd.Next(0, 999)}";
            await SendAsync(new AddVersionTrackRequest
            {
                PlatformType = "Mobile",
                VersionNumber = version,
                ReleaseDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                ReleaseNotes = "Integration test entry",
                ReleasedBy = RequestUid,
                ReleasedTo = "Beta"
            });

            var list = await SendAsync(new GetVersionTrackQuery());

            list.Should().NotBeNull();
            list.VersionTrackList.Should().Contain(v => v.VersionNumber == version);
        }
    }
}
