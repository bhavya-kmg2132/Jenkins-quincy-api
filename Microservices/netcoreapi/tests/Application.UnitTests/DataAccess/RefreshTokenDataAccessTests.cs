using System.Collections.Generic;
using Application.Common.Interfaces;
using Infrastructure.DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Application.UnitTests.DataAccess
{
    public class RefreshTokenDataAccessTests
    {
        private Mock<IConfiguration> _configuration;
        private Mock<ILogger<RefreshTokenDataAccess>> _logger;
        private Mock<IConnectionHelper> _connectionHelper;
        private RefreshTokenDataAccess _sut;

        [SetUp]
        public void SetUp()
        {
            _configuration = new Mock<IConfiguration>();
            _logger = new Mock<ILogger<RefreshTokenDataAccess>>();
            _connectionHelper = new Mock<IConnectionHelper>();

            _connectionHelper
                .Setup(x => x.LoadSqlQueriesXml("RefreshToken"))
                .Returns(new Dictionary<string, string>());

            _sut = new RefreshTokenDataAccess(
                _configuration.Object,
                _logger.Object,
                _connectionHelper.Object);
        }

        [Test]
        public void Constructor_LoadsSqlQueriesForRefreshToken()
        {
            _connectionHelper.Verify(x => x.LoadSqlQueriesXml("RefreshToken"), Times.Once);
        }
    }
}
