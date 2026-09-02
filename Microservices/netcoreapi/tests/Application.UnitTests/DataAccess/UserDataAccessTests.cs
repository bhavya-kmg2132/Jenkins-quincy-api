using System.Threading.Tasks;
using Application.Common.Interfaces;
using AutoMapper;
using FluentAssertions;
using Infrastructure.DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using netauthlib;
using NUnit.Framework;

namespace Application.UnitTests.DataAccess
{
    public class UserDataAccessTests
    {
        private Mock<IConfiguration> _configuration;
        private Mock<IMapper> _mapper;
        private Mock<ILogger<UserDataAccess>> _logger;
        private Mock<INetAuthProvider> _netAuthUser;
        private Mock<IConnectionHelper> _connectionHelper;
        private UserDataAccess _sut;

        [SetUp]
        public void SetUp()
        {
            _configuration = new Mock<IConfiguration>();
            _mapper = new Mock<IMapper>();
            _logger = new Mock<ILogger<UserDataAccess>>();
            _netAuthUser = new Mock<INetAuthProvider>();
            _connectionHelper = new Mock<IConnectionHelper>();

            _sut = new UserDataAccess(
                _configuration.Object,
                _mapper.Object,
                _logger.Object,
                _netAuthUser.Object,
                _connectionHelper.Object);
        }

        [Test]
        public async Task GetRoleById_Zero_ReturnsNull()
        {
            var result = await _sut.GetRoleById("0");

            result.Should().BeNull();
        }

        [Test]
        public async Task GetRoleById_EmptyString_ReturnsNull()
        {
            var result = await _sut.GetRoleById(string.Empty);

            result.Should().BeNull();
        }

        [Test]
        public async Task GetRoleById_Null_ReturnsNull()
        {
            var result = await _sut.GetRoleById(null);

            result.Should().BeNull();
        }

        [Test]
        public async Task GetRoleById_Whitespace_ReturnsNull()
        {
            var result = await _sut.GetRoleById("   ");

            result.Should().BeNull();
        }

        [Test]
        public async Task GetUserIdBasedOnOidFromDb_AnyOid_ReturnsEmptyString()
        {
            var result = await _sut.GetUserIdBasedOnOidFromDb("any-oid");

            result.Should().BeEmpty();
        }

        [Test]
        public async Task UpdateUserAccessLevel_AnyUser_ReturnsFalse()
        {
            var result = await _sut.UpdateUserAccessLevel(new NetAuth.Contract.DataContract.Dto.UserDto());

            result.Should().BeFalse();
        }

        [Test]
        public async Task GetUserProfileByUserId_AnyId_ReturnsEmptyList()
        {
            var result = await _sut.GetUserProfileByUserId("user-1");

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetUserProfileByProfileId_AnyId_ReturnsEmptyList()
        {
            var result = await _sut.GetUserProfileByProfileId(42);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetUserProfileList_ReturnsEmptyList()
        {
            var result = await _sut.GetUserProfileList();

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetUserAccessLevelList_ReturnsEmptyList()
        {
            var result = await _sut.GetUserAccessLevelList();

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Test]
        public void UserListReport_ReturnsNull()
        {
            var result = _sut.UserListReport();

            result.Should().BeNull();
        }
    }
}
