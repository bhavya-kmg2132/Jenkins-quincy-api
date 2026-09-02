using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Common;
using FluentAssertions;
using Infrastructure.DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using AcmeProductEntity = Domain.Entities.AcmeProduct;

namespace Application.UnitTests.DataAccess
{
    internal class AcmeProductTestDomainEvent : DomainEvent { }

    public class AcmeDataAccessTests
    {
        private Mock<ILogger<AcmeDataAccess>> _logger;
        private Mock<IConfiguration> _configuration;
        private Mock<IDomainEventService> _domainEventService;
        private Mock<ICurrentUserService> _currentUserService;
        private Mock<IConnectionHelper> _connectionHelper;
        private AcmeDataAccess _sut;

        [SetUp]
        public void SetUp()
        {
            _logger = new Mock<ILogger<AcmeDataAccess>>();
            _configuration = new Mock<IConfiguration>();
            _domainEventService = new Mock<IDomainEventService>();
            _currentUserService = new Mock<ICurrentUserService>();
            _connectionHelper = new Mock<IConnectionHelper>();

            _connectionHelper
                .Setup(x => x.LoadSqlQueriesXml("AcmeProduct"))
                .Returns(new Dictionary<string, string>());

            _sut = new AcmeDataAccess(
                _configuration.Object,
                _logger.Object,
                _domainEventService.Object,
                _currentUserService.Object,
                _connectionHelper.Object);
        }

        [Test]
        public void Constructor_LoadsSqlQueriesForAcmeProduct()
        {
            _connectionHelper.Verify(x => x.LoadSqlQueriesXml("AcmeProduct"), Times.Once);
        }

        [Test]
        public async Task DispatchEvents_NoEvents_DoesNotPublish()
        {
            var entity = new AcmeProductEntity { DomainEvents = new List<DomainEvent>() };

            await _sut.DispatchEvents(entity);

            _domainEventService.Verify(x => x.Publish(It.IsAny<DomainEvent>()), Times.Never);
        }

        [Test]
        public async Task DispatchEvents_UnpublishedEvent_PublishesAndMarksPublished()
        {
            var evt = new AcmeProductTestDomainEvent { IsPublished = false };
            var entity = new AcmeProductEntity { DomainEvents = new List<DomainEvent> { evt } };

            await _sut.DispatchEvents(entity);

            _domainEventService.Verify(x => x.Publish(evt), Times.Once);
            evt.IsPublished.Should().BeTrue();
        }

        [Test]
        public async Task DispatchEvents_AlreadyPublishedEvent_DoesNotPublishAgain()
        {
            var evt = new AcmeProductTestDomainEvent { IsPublished = true };
            var entity = new AcmeProductEntity { DomainEvents = new List<DomainEvent> { evt } };

            await _sut.DispatchEvents(entity);

            _domainEventService.Verify(x => x.Publish(It.IsAny<DomainEvent>()), Times.Never);
        }

        [Test]
        public async Task DispatchEvents_MultipleUnpublishedEvents_PublishesAll()
        {
            var evt1 = new AcmeProductTestDomainEvent { IsPublished = false };
            var evt2 = new AcmeProductTestDomainEvent { IsPublished = false };
            var entity = new AcmeProductEntity { DomainEvents = new List<DomainEvent> { evt1, evt2 } };

            await _sut.DispatchEvents(entity);

            _domainEventService.Verify(x => x.Publish(It.IsAny<DomainEvent>()), Times.Exactly(2));
            evt1.IsPublished.Should().BeTrue();
            evt2.IsPublished.Should().BeTrue();
        }
    }
}
