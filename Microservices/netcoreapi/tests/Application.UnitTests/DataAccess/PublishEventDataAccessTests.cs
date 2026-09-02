using System.Collections.Generic;
using Application.Common.Interfaces;
using Infrastructure.DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Application.UnitTests.DataAccess
{
    public class PublishEventDataAccessTests
    {
        private Mock<IConfiguration> _configuration;
        private Mock<IConnectionHelper> _connectionHelper;
        private Mock<ILogger<PublishEventDataAccess>> _logger;
        private Mock<ICrmMasterDataAccess> _crmMasterDataAccess;
        private Mock<ICurrentUserService> _currentUserService;
        private PublishEventDataAccess _sut;

        [SetUp]
        public void SetUp()
        {
            _configuration = new Mock<IConfiguration>();
            _connectionHelper = new Mock<IConnectionHelper>();
            _logger = new Mock<ILogger<PublishEventDataAccess>>();
            _crmMasterDataAccess = new Mock<ICrmMasterDataAccess>();
            _currentUserService = new Mock<ICurrentUserService>();

            _connectionHelper
                .Setup(x => x.LoadSqlQueriesXml("PublishEventData"))
                .Returns(new Dictionary<string, string>());

            _sut = new PublishEventDataAccess(
                _configuration.Object,
                _connectionHelper.Object,
                _logger.Object,
                _crmMasterDataAccess.Object,
                _currentUserService.Object);
        }

        [Test]
        public void Constructor_LoadsSqlQueriesForPublishEventData()
        {
            _connectionHelper.Verify(x => x.LoadSqlQueriesXml("PublishEventData",Domain.Enums.DbConfigKeys.EventDb));
        }
    }
}
