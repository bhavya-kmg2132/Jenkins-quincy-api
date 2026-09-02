using System.Threading;
using System.Threading.Tasks;
using Application.Common.Behaviours;
using Application.Common.Interfaces;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Application.UnitTests.Common.Behaviours
{
    public class PerfTestRequest : IRequest<string>
    {
        public string Value { get; set; }
    }

    public class PerformanceBehaviourTests
    {
        private Mock<ILogger<PerfTestRequest>> _logger;
        private Mock<ICurrentUserService> _currentUserService;
        private Mock<IIdentityService> _identityService;
        private PerformanceBehaviour<PerfTestRequest, string> _sut;

        [SetUp]
        public void SetUp()
        {
            _logger = new Mock<ILogger<PerfTestRequest>>();
            _currentUserService = new Mock<ICurrentUserService>();
            _identityService = new Mock<IIdentityService>();
            _sut = new PerformanceBehaviour<PerfTestRequest, string>(
                _logger.Object, _currentUserService.Object, _identityService.Object);
        }

        [Test]
        public async Task Handle_FastRequest_ReturnsResponse()
        {
            var result = await _sut.Handle(
                new PerfTestRequest { Value = "hello" },
                ct => Task.FromResult("ok"),
                CancellationToken.None);

            result.Should().Be("ok");
        }

        [Test]
        public async Task Handle_FastRequest_DoesNotLogWarning()
        {
            await _sut.Handle(
                new PerfTestRequest { Value = "hello" },
                ct => Task.FromResult("ok"),
                CancellationToken.None);

            _logger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    null,
                    It.IsAny<System.Func<It.IsAnyType, System.Exception, string>>()),
                Times.Never);
        }

        [Test]
        public async Task Handle_WithNullUserId_DoesNotThrow()
        {
            _currentUserService.Setup(x => x.UserId).Returns((string)null);

            var act = async () => await _sut.Handle(
                new PerfTestRequest { Value = "hello" },
                ct => Task.FromResult("ok"),
                CancellationToken.None);

            await act.Should().NotThrowAsync();
        }
    }
}
