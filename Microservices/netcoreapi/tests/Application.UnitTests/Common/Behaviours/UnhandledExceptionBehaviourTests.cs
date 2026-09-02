using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Behaviours;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Application.UnitTests.Common.Behaviours
{
    public class UnhandledTestRequest : IRequest<string>
    {
        public string Value { get; set; }
    }

    public class UnhandledExceptionBehaviourTests
    {
        private Mock<ILogger<UnhandledTestRequest>> _logger;
        private UnhandledExceptionBehaviour<UnhandledTestRequest, string> _sut;

        [SetUp]
        public void SetUp()
        {
            _logger = new Mock<ILogger<UnhandledTestRequest>>();
            _sut = new UnhandledExceptionBehaviour<UnhandledTestRequest, string>(_logger.Object);
        }

        [Test]
        public async Task Handle_NextSucceeds_ReturnsResponse()
        {
            var result = await _sut.Handle(
                new UnhandledTestRequest { Value = "hello" },
                ct => Task.FromResult("ok"),
                CancellationToken.None);

            result.Should().Be("ok");
        }

        [Test]
        public async Task Handle_NextThrows_RethrowsException()
        {
            var act = async () => await _sut.Handle(
                new UnhandledTestRequest { Value = "hello" },
                ct => throw new InvalidOperationException("boom"),
                CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("boom");
        }

        [Test]
        public async Task Handle_NextThrows_LogsError()
        {
            try
            {
                await _sut.Handle(
                    new UnhandledTestRequest { Value = "hello" },
                    ct => throw new InvalidOperationException("boom"),
                    CancellationToken.None);
            }
            catch { }

            _logger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
