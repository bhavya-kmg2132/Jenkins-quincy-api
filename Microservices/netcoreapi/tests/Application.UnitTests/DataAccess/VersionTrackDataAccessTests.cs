using System.Collections.Generic;
using Application.Common.Interfaces;
using Infrastructure.DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Application.UnitTests.DataAccess
{
    public class VersionTrackDataAccessTests
    {
        private Mock<ILogger<VersionTrackDataAccess>> _logger;
        private Mock<IConfiguration> _configuration;
        private Mock<IDomainEventService> _domainEventService;
        private Mock<IConnectionHelper> _connectionHelper;
        private Mock<IMasterDataAccess> _masterDataAccess;
        private VersionTrackDataAccess _sut;

        [SetUp]
        public void SetUp()
        {
            _logger = new Mock<ILogger<VersionTrackDataAccess>>();
            _configuration = new Mock<IConfiguration>();
            _domainEventService = new Mock<IDomainEventService>();
            _connectionHelper = new Mock<IConnectionHelper>();
            _masterDataAccess = new Mock<IMasterDataAccess>();

            _connectionHelper
                .Setup(x => x.LoadSqlQueriesXml("VersionTrack"))
                .Returns(new Dictionary<string, string>());

            _sut = new VersionTrackDataAccess(
                _configuration.Object,
                _logger.Object,
                _domainEventService.Object,
                _connectionHelper.Object,
                _masterDataAccess.Object);
        }

        [Test]
        public void Constructor_LoadsSqlQueriesForVersionTrack()
        {
            _connectionHelper.Verify(x => x.LoadSqlQueriesXml("VersionTrack"), Times.Once);
        }
    }
}
