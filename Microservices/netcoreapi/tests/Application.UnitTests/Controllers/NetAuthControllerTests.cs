using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Api.Controllers;
using Application.AllUsers.Queries.AllUser.GetUsersQuery;
using Application.AllUsers.Queries.AllUser.GetUsersQueryAsync;
using Application.Users.Commands.ActivateOrInActivateUser;
using Application.Users.Commands.AddRoles;
using Application.Users.Commands.CreateUser;
using Application.Users.Commands.ResetUserCache;
using Application.Users.Commands.RolePermissions;
using Application.Users.Commands.UiPermission.AddUiPermission;
using Application.Users.Commands.UiPermission.RoleUiPermissions.AddUiPermissionsForRole;
using Application.Users.Commands.Teams.AddTeam;
using Application.Users.Commands.Teams.AddTeamMembers;
using Application.Users.Commands.Teams.RemoveTeamMember;
using Application.Users.Commands.UiPermission.UpdateUiPermission;
using Application.Users.Commands.UpdateUser;
using Application.Users.Commands.UserPermissions.AddPermissionRequest;
using Application.Users.Commands.UserPermissions.AddPermissionsDeniedForUser;
using Application.Users.Queries;
using Application.Users.Queries.AuthReferenceLookup;
using Application.Users.Queries.AuthReferenceLookup.GetAuthReferenceLookupQuery;
using Application.Users.Queries.GetUsersByStatusQuery;
using Application.Users.Queries.Permission;
using Application.Users.Queries.Permission.GetPermissionsQuery;
using Application.Users.Queries.Permission.GetPermissionsQueryAsync;
using Application.Users.Queries.Role.GetPermissionsByRoleIdQuery;
using Application.Users.Queries.Role.GetRoleQuery;
using Application.Users.Queries.Role.RoleUiPermission.GetUiPermissionsByRoleIdQuery;
using Application.Users.Queries.UiPermission;
using Application.Users.Queries.UiPermission.GetUiPermissionsQuery;
using Application.Users.Queries.UiPermission.RoleUiPermission;
using Application.Users.Queries.UiPermission.UserUiPermission;
using Application.Users.Queries.User;
using Application.Users.Queries.UserActivity;
using Application.Users.Queries.UserActivity.GetUserActivitiesByUserIdsQuery;
using Application.Users.Queries.UserActivity.GetUserActivityQuery;
using Application.Users.Queries.UserUiPermision.GetUserUiPermissionsByUserIdQuery;
using Application.Users.Commands.UserPermissions.UpdatePermissionRequest;
using Application.Users.Queries.Team;
using Application.Users.Queries.Team.GetTeamsQuery;
using Application.Users.Queries.Team.GetTeamByIdQuery;
using Application.Users.Queries.Team.GetTeamsByUserIdQuery;
using Application.Users.Queries.Team.GetTeamMembersByTeamIdQuery;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace Application.UnitTests.Controllers
{
    [TestFixture]
    public class NetAuthControllerTests
    {
        private Mock<ISender> _mediator;
        private NetAuthController _sut;

        private const string CorrelationId = "test-corr";
        private const string RequestId     = "test-req";
        private const string RequestOid    = "test-oid";
        private const string RequestUid    = "test-uid";
        private const string ApiKey        = "test-key";

        [SetUp]
        public void SetUp()
        {
            _mediator = new Mock<ISender>();

            var services = new ServiceCollection();
            services.AddSingleton(_mediator.Object);
            var sp = services.BuildServiceProvider();

            var httpContext = new DefaultHttpContext { RequestServices = sp };

            _sut = new NetAuthController();
            _sut.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        // ──────────────────────────────────────────────────────────────
        // ResetUserObjectCache  PUT
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task ResetUserObjectCache_ShouldReturnNoContent_WhenCalled()
        {
            _mediator.Setup(x => x.Send(It.IsAny<ResetUserCacheRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

            var result = await _sut.ResetUserObjectCache(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey);

            result.Should().BeOfType<NoContentResult>();
        }

        [Test]
        public async Task ResetUserObjectCache_ShouldCallMediatorSend_Once()
        {
            _mediator.Setup(x => x.Send(It.IsAny<ResetUserCacheRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

            await _sut.ResetUserObjectCache(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey);

            _mediator.Verify(x => x.Send(It.IsAny<ResetUserCacheRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // GetPermissions  GET
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task GetPermissions_ShouldReturnPermissionListVm_WhenCalled()
        {
            var vm = new PermissionListVm();
            _mediator.Setup(x => x.Send(It.IsAny<GetPermissionsQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(vm);

            var result = await _sut.GetPermissions(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey);

            result.Should().BeSameAs(vm);
        }

        [Test]
        public async Task GetPermissions_ShouldCallMediatorWithEmptyQuery_WhenEmptyJsonProvided()
        {
            var vm = new PermissionListVm();
            _mediator.Setup(x => x.Send(It.IsAny<GetPermissionsQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(vm);

            // Empty JSON {} → GetPermissionsQuery with no properties — still calls mediator
            await _sut.GetPermissions(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey);

            _mediator.Verify(x => x.Send(It.IsAny<GetPermissionsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // GetPermissionsAsync  GET
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task GetPermissionsAsync_ShouldReturnPermissionListVm_WhenCalled()
        {
            var vm = new PermissionListVm();
            _mediator.Setup(x => x.Send(It.IsAny<GetPermissionsQueryAsync>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(vm);

            var result = await _sut.GetPermissionsAsync(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey);

            result.Should().BeSameAs(vm);
        }

        [Test]
        public async Task GetPermissionsAsync_ShouldCallMediatorWithEmptyQuery_WhenEmptyJsonProvided()
        {
            var vm = new PermissionListVm();
            _mediator.Setup(x => x.Send(It.IsAny<GetPermissionsQueryAsync>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(vm);

            await _sut.GetPermissionsAsync(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey);

            _mediator.Verify(x => x.Send(It.IsAny<GetPermissionsQueryAsync>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // GetRoles  GET
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task GetRoles_ShouldReturnRoleListVm_WhenCalled()
        {
            var vm = new RoleListVm();
            _mediator.Setup(x => x.Send(It.IsAny<GetRolesQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(vm);

            var result = await _sut.GetRoles(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey);

            result.Value.Should().BeSameAs(vm);
        }

        [Test]
        public async Task GetRoles_ShouldCallMediatorWithEmptyQuery_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetRolesQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new RoleListVm());

            await _sut.GetRoles(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey);

            _mediator.Verify(x => x.Send(It.IsAny<GetRolesQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // GetUsers  GET
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task GetUsers_ShouldReturnUserListVm_WhenCalled()
        {
            var vm = new UserListVm();
            _mediator.Setup(x => x.Send(It.IsAny<GetUsersQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(vm);

            var result = await _sut.GetUsers(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey);

            result.Should().BeSameAs(vm);
        }

        [Test]
        public async Task GetUsers_ShouldCallMediatorOnce_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetUsersQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new UserListVm());

            await _sut.GetUsers(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey);

            _mediator.Verify(x => x.Send(It.IsAny<GetUsersQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // GetUsersAsync  GET
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task GetUsersAsync_ShouldReturnUserListVm_WhenCalled()
        {
            var vm = new UserListVm();
            _mediator.Setup(x => x.Send(It.IsAny<GetUsersQueryAsync>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(vm);

            var result = await _sut.GetUsersAsync(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey);

            result.Value.Should().BeSameAs(vm);
        }

        [Test]
        public async Task GetUsersAsync_ShouldCallMediatorOnce_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetUsersQueryAsync>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new UserListVm());

            await _sut.GetUsersAsync(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey);

            _mediator.Verify(x => x.Send(It.IsAny<GetUsersQueryAsync>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // GetUserVmByUserName  GET
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task GetUserVmByUserName_ShouldSendQueryWithCorrectUserName()
        {
            var vm = new UserVm();
            _mediator.Setup(x => x.Send(It.IsAny<GetUserVmByUserNameQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(vm);

            var result = await _sut.GetUserVmByUserName(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey, "john.doe");

            _mediator.Verify(x => x.Send(
                It.Is<GetUserVmByUserNameQuery>(q => q.UserName == "john.doe"),
                It.IsAny<CancellationToken>()), Times.Once);
            result.Value.Should().BeSameAs(vm);
        }

        [Test]
        public async Task GetUserVmByUserName_ShouldSendQueryWithNullUserName_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetUserVmByUserNameQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new UserVm());

            // Empty JSON = null userName query param
            await _sut.GetUserVmByUserName(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey, null);

            _mediator.Verify(x => x.Send(
                It.Is<GetUserVmByUserNameQuery>(q => q.UserName == null),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // GetUserByRoleId  GET
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task GetUserByRoleId_ShouldSendQueryWithCorrectRoleId()
        {
            var vm = new UserListVm();
            _mediator.Setup(x => x.Send(It.IsAny<GetUserByRoleIdQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(vm);

            var result = await _sut.GetUserByRoleId(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey, "role-001");

            _mediator.Verify(x => x.Send(
                It.Is<GetUserByRoleIdQuery>(q => q.RoleId == "role-001"),
                It.IsAny<CancellationToken>()), Times.Once);
            result.Value.Should().BeSameAs(vm);
        }

        [Test]
        public async Task GetUserByRoleId_ShouldSendQueryWithNullRoleId_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetUserByRoleIdQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new UserListVm());

            await _sut.GetUserByRoleId(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey, null);

            _mediator.Verify(x => x.Send(
                It.Is<GetUserByRoleIdQuery>(q => q.RoleId == null),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // GetPermissionsByRoleId  GET
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task GetPermissionsByRoleId_ShouldSendQueryWithCorrectRoleId()
        {
            var vm = new RoleVm();
            _mediator.Setup(x => x.Send(It.IsAny<GetPermissionsByRoleId>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(vm);

            var result = await _sut.GetPermissionsByRoleId(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey, "role-abc");

            _mediator.Verify(x => x.Send(
                It.Is<GetPermissionsByRoleId>(q => q.RoleId == "role-abc"),
                It.IsAny<CancellationToken>()), Times.Once);
            result.Value.Should().BeSameAs(vm);
        }

        [Test]
        public async Task GetPermissionsByRoleId_ShouldSendQueryWithNullRoleId_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetPermissionsByRoleId>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new RoleVm());

            await _sut.GetPermissionsByRoleId(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey, null);

            _mediator.Verify(x => x.Send(
                It.Is<GetPermissionsByRoleId>(q => q.RoleId == null),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // AddPermission  POST
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task AddPermission_ShouldReturn200_WhenValidRequestProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<AddPermissionRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

            var result = await _sut.AddPermission(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new AddPermissionRequest { PermissionValue = "read", PermissionDisplayName = "Read" });

            result.Value.Should().Be(200);
        }

        [Test]
        public async Task AddPermission_ShouldReturn200_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<AddPermissionRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

            // Empty JSON {} → all properties null, controller does no validation
            var result = await _sut.AddPermission(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new AddPermissionRequest());

            result.Value.Should().Be(200);
        }

        [Test]
        public async Task AddPermission_ShouldPassRequestDirectlyToMediator()
        {
            var request = new AddPermissionRequest { PermissionValue = "write", ModuleId = "m1" };
            _mediator.Setup(x => x.Send(It.IsAny<AddPermissionRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

            await _sut.AddPermission(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey, request);

            _mediator.Verify(x => x.Send(request, It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // UpdatePermission  POST
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task UpdatePermission_ShouldReturn200_WhenValidRequestProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<UpdatePermissionRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

            var result = await _sut.UpdatePermission(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new UpdatePermissionRequest { PermissionValue = "write" });

            result.Value.Should().Be(200);
        }

        [Test]
        public async Task UpdatePermission_ShouldReturn200_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<UpdatePermissionRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

            var result = await _sut.UpdatePermission(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new UpdatePermissionRequest());

            result.Value.Should().Be(200);
        }

        [Test]
        public async Task UpdatePermission_ShouldPassRequestDirectlyToMediator()
        {
            var request = new UpdatePermissionRequest { PermissionValue = "execute" };
            _mediator.Setup(x => x.Send(It.IsAny<UpdatePermissionRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

            await _sut.UpdatePermission(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey, request);

            _mediator.Verify(x => x.Send(request, It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // UpdateUser  POST
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task UpdateUser_ShouldReturn200_WhenValidRequestProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<UpdateUserRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(1);

            var result = await _sut.UpdateUser(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new UpdateUserRequest { userId = "user-001", Email = "user@example.com" });

            result.Value.Should().Be(200);
        }

        [Test]
        public async Task UpdateUser_ShouldReturn200_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<UpdateUserRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(1);

            var result = await _sut.UpdateUser(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new UpdateUserRequest());

            result.Value.Should().Be(200);
        }

        [Test]
        public async Task UpdateUser_ShouldPassRequestDirectlyToMediator()
        {
            var request = new UpdateUserRequest { userId = "user-002", Email = "jane@example.com" };
            _mediator.Setup(x => x.Send(It.IsAny<UpdateUserRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(1);

            await _sut.UpdateUser(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey, request);

            _mediator.Verify(x => x.Send(request, It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // GetUiPermissions  GET
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task GetUiPermissions_ShouldReturnUiPermissionListVm_WhenCalled()
        {
            var vm = new UiPermissionListVm();
            _mediator.Setup(x => x.Send(It.IsAny<GetUiPermissionsQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(vm);

            var result = await _sut.GetUiPermissions(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey);

            result.Value.Should().BeSameAs(vm);
        }

        [Test]
        public async Task GetUiPermissions_ShouldCallMediatorOnce_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetUiPermissionsQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new UiPermissionListVm());

            await _sut.GetUiPermissions(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey);

            _mediator.Verify(x => x.Send(It.IsAny<GetUiPermissionsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // GetUiPermissionsByUserId  GET
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task GetUiPermissionsByUserId_ShouldSendQueryWithCorrectUserId()
        {
            var vm = new UserUiPermissionListVm();
            _mediator.Setup(x => x.Send(It.IsAny<GetUserUiPermissionsByUserIdQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(vm);

            var result = await _sut.GetUiPermissionsByUserId(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey, "user-001");

            _mediator.Verify(x => x.Send(
                It.Is<GetUserUiPermissionsByUserIdQuery>(q => q.UserId == "user-001"),
                It.IsAny<CancellationToken>()), Times.Once);
            result.Value.Should().BeSameAs(vm);
        }

        [Test]
        public async Task GetUiPermissionsByUserId_ShouldSendQueryWithNullUserId_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetUserUiPermissionsByUserIdQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new UserUiPermissionListVm());

            await _sut.GetUiPermissionsByUserId(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey, null);

            _mediator.Verify(x => x.Send(
                It.Is<GetUserUiPermissionsByUserIdQuery>(q => q.UserId == null),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // GetUiPermissionsByRoleId  GET
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task GetUiPermissionsByRoleId_ShouldSendQueryWithCorrectRoleId()
        {
            var vm = new RoleUiPermissionListVm();
            _mediator.Setup(x => x.Send(It.IsAny<GetUiPermissionsByRoleIdQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(vm);

            var result = await _sut.GetUiPermissionsByRoleId(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey, "role-xyz");

            _mediator.Verify(x => x.Send(
                It.Is<GetUiPermissionsByRoleIdQuery>(q => q.RoleId == "role-xyz"),
                It.IsAny<CancellationToken>()), Times.Once);
            result.Value.Should().BeSameAs(vm);
        }

        [Test]
        public async Task GetUiPermissionsByRoleId_ShouldSendQueryWithNullRoleId_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetUiPermissionsByRoleIdQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new RoleUiPermissionListVm());

            await _sut.GetUiPermissionsByRoleId(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey, null);

            _mediator.Verify(x => x.Send(
                It.Is<GetUiPermissionsByRoleIdQuery>(q => q.RoleId == null),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // AddUiPermissionsForRole  POST
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task AddUiPermissionsForRole_ShouldReturn200_WhenValidRequestProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<AddUiPermissionsForRoleRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

            var result = await _sut.AddUiPermissionsForRole(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new AddUiPermissionsForRoleRequest());

            result.Value.Should().Be(200);
        }

        [Test]
        public async Task AddUiPermissionsForRole_ShouldReturn200_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<AddUiPermissionsForRoleRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

            var result = await _sut.AddUiPermissionsForRole(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new AddUiPermissionsForRoleRequest());

            result.Value.Should().Be(200);
            _mediator.Verify(x => x.Send(It.IsAny<AddUiPermissionsForRoleRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // AddUiPermission  POST  (returns string)
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task AddUiPermission_ShouldReturnPermissionId_WhenValidRequestProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<AddUiPermissionRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync("new-perm-id");

            var result = await _sut.AddUiPermission(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new AddUiPermissionRequest { PermissionValue = "view.dashboard" });

            result.Value.Should().Be("new-perm-id");
        }

        [Test]
        public async Task AddUiPermission_ShouldReturnNullId_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<AddUiPermissionRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((string)null);

            var result = await _sut.AddUiPermission(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new AddUiPermissionRequest());

            result.Value.Should().BeNull();
            _mediator.Verify(x => x.Send(It.IsAny<AddUiPermissionRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task AddUiPermission_ShouldPassRequestDirectlyToMediator()
        {
            var request = new AddUiPermissionRequest { PermissionValue = "edit.report" };
            _mediator.Setup(x => x.Send(It.IsAny<AddUiPermissionRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync("perm-99");

            await _sut.AddUiPermission(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey, request);

            _mediator.Verify(x => x.Send(request, It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // UpdateUiPermission  POST
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task UpdateUiPermission_ShouldReturn200_WhenValidRequestProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<UpdateUiPermissionRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

            var result = await _sut.UpdateUiPermission(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new UpdateUiPermissionRequest());

            result.Value.Should().Be(200);
        }

        [Test]
        public async Task UpdateUiPermission_ShouldReturn200_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<UpdateUiPermissionRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

            var result = await _sut.UpdateUiPermission(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new UpdateUiPermissionRequest());

            result.Value.Should().Be(200);
            _mediator.Verify(x => x.Send(It.IsAny<UpdateUiPermissionRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // GetUserActivities  GET
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task GetUserActivities_ShouldSendQueryWithCorrectParameters()
        {
            var vm = new UserActivitiesVm();
            _mediator.Setup(x => x.Send(It.IsAny<GetUserActivitiesQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(vm);

            var result = await _sut.GetUserActivities(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                "user-001", pageSize: 10, pageNumber: 2);

            _mediator.Verify(x => x.Send(
                It.Is<GetUserActivitiesQuery>(q => q.UserId == "user-001" && q.PageSize == 10 && q.PageNumber == 2),
                It.IsAny<CancellationToken>()), Times.Once);
            result.Value.Should().BeSameAs(vm);
        }

        [Test]
        public async Task GetUserActivities_ShouldSendQueryWithDefaultPageValues_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetUserActivitiesQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new UserActivitiesVm());

            // Empty JSON → null userId, 0 pageSize, 0 pageNumber
            await _sut.GetUserActivities(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                null, pageSize: 0, pageNumber: 0);

            _mediator.Verify(x => x.Send(
                It.Is<GetUserActivitiesQuery>(q => q.UserId == null && q.PageSize == 0 && q.PageNumber == 0),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // GetUserActivitiesByUserIds  POST
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task GetUserActivitiesByUserIds_ShouldReturnVm_WhenValidQueryProvided()
        {
            var vm = new UserActivitiesVm();
            var query = new GetUserActivitiesByUserIdsQuery { UserIds = new List<string> { "u1", "u2" } };
            _mediator.Setup(x => x.Send(query, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(vm);

            var result = await _sut.GetUserActivitiesByUserIds(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey, query);

            result.Value.Should().BeSameAs(vm);
        }

        [Test]
        public async Task GetUserActivitiesByUserIds_ShouldPassQueryDirectlyToMediator_WhenEmptyJsonProvided()
        {
            var emptyQuery = new GetUserActivitiesByUserIdsQuery();
            _mediator.Setup(x => x.Send(emptyQuery, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new UserActivitiesVm());

            await _sut.GetUserActivitiesByUserIds(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey, emptyQuery);

            _mediator.Verify(x => x.Send(emptyQuery, It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // AddUserActivity  POST  (returns string)
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task AddUserActivity_ShouldReturnActivityId_WhenValidRequestProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<AddUserActivityRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync("activity-123");

            var result = await _sut.AddUserActivity(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new AddUserActivityRequest { UserId = "u1", LastActivityModule = "Dashboard" });

            result.Value.Should().Be("activity-123");
        }

        [Test]
        public async Task AddUserActivity_ShouldReturnNullId_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<AddUserActivityRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((string)null);

            var result = await _sut.AddUserActivity(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new AddUserActivityRequest());

            result.Value.Should().BeNull();
            _mediator.Verify(x => x.Send(It.IsAny<AddUserActivityRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // AddPermissionsGrantedForUser  POST
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task AddPermissionsGrantedForUser_ShouldReturn200_WhenValidRequestProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<AddPermissionGrantedForUserRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

            var result = await _sut.AddPermissionsGrantedForUser(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new AddPermissionGrantedForUserRequest());

            result.Value.Should().Be(200);
        }

        [Test]
        public async Task AddPermissionsGrantedForUser_ShouldReturn200_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<AddPermissionGrantedForUserRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

            var result = await _sut.AddPermissionsGrantedForUser(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new AddPermissionGrantedForUserRequest());

            result.Value.Should().Be(200);
            _mediator.Verify(x => x.Send(It.IsAny<AddPermissionGrantedForUserRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // AddPermissionsDeniedForUser  POST
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task AddPermissionsDeniedForUser_ShouldReturn200_WhenValidRequestProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<AddPermissionDeniedForUserRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

            var result = await _sut.AddPermissionsDeniedForUser(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new AddPermissionDeniedForUserRequest());

            result.Value.Should().Be(200);
        }

        [Test]
        public async Task AddPermissionsDeniedForUser_ShouldReturn200_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<AddPermissionDeniedForUserRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

            var result = await _sut.AddPermissionsDeniedForUser(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new AddPermissionDeniedForUserRequest());

            result.Value.Should().Be(200);
            _mediator.Verify(x => x.Send(It.IsAny<AddPermissionDeniedForUserRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // AddPermissionsForRole  POST
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task AddPermissionsForRole_ShouldReturn200_WhenValidRequestProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<AddPermissionsForRoleRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

            var result = await _sut.AddPermissionsForRole(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new AddPermissionsForRoleRequest());

            result.Value.Should().Be(200);
        }

        [Test]
        public async Task AddPermissionsForRole_ShouldReturn200_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<AddPermissionsForRoleRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

            var result = await _sut.AddPermissionsForRole(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new AddPermissionsForRoleRequest());

            result.Value.Should().Be(200);
            _mediator.Verify(x => x.Send(It.IsAny<AddPermissionsForRoleRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // AddUser  POST  (returns string id)
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task AddUser_ShouldReturnUserId_WhenValidRequestProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<CreateUserRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync("user-new-001");

            var result = await _sut.AddUser(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new CreateUserRequest { UserName = "alice", Email = "alice@example.com" });

            result.Value.Should().Be("user-new-001");
        }

        [Test]
        public async Task AddUser_ShouldReturnNullId_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<CreateUserRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((string)null);

            // Empty JSON {} → all null properties, controller does no validation
            var result = await _sut.AddUser(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new CreateUserRequest());

            result.Value.Should().BeNull();
            _mediator.Verify(x => x.Send(It.IsAny<CreateUserRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task AddUser_ShouldPassRequestDirectlyToMediator()
        {
            var request = new CreateUserRequest { UserName = "bob", Email = "bob@example.com" };
            _mediator.Setup(x => x.Send(It.IsAny<CreateUserRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync("user-bob");

            await _sut.AddUser(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey, request);

            _mediator.Verify(x => x.Send(request, It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // AddRoles (multiple)  POST  — route AddRoles
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task AddRoles_ShouldReturn200_WhenValidMultipleRolesRequestProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<AddRolesForUserRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

            var result = await _sut.AddRoles(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new AddRolesForUserRequest());

            result.Value.Should().Be(200);
        }

        [Test]
        public async Task AddRoles_ShouldReturn200_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<AddRolesForUserRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

            var result = await _sut.AddRoles(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new AddRolesForUserRequest());

            result.Value.Should().Be(200);
            _mediator.Verify(x => x.Send(It.IsAny<AddRolesForUserRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // AddRole (single)  POST  — route AddRole
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task AddRole_ShouldReturn200_WhenValidSingleRoleRequestProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<AddRoleForUserRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

            var result = await _sut.AddRoles(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new AddRoleForUserRequest());

            result.Value.Should().Be(200);
        }

        [Test]
        public async Task AddRole_ShouldReturn200_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<AddRoleForUserRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

            var result = await _sut.AddRoles(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new AddRoleForUserRequest());

            result.Value.Should().Be(200);
            _mediator.Verify(x => x.Send(It.IsAny<AddRoleForUserRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // DeleteRole  POST
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task DeleteRole_ShouldReturn200_WhenValidRequestProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<DeleteRoleForUserRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

            var result = await _sut.DeleteRole(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new DeleteRoleForUserRequest());

            result.Value.Should().Be(200);
        }

        [Test]
        public async Task DeleteRole_ShouldReturn200_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<DeleteRoleForUserRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

            var result = await _sut.DeleteRole(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new DeleteRoleForUserRequest());

            result.Value.Should().Be(200);
            _mediator.Verify(x => x.Send(It.IsAny<DeleteRoleForUserRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // GetAuthReferenceLookupsByTypeName  GET
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task GetAuthReferenceLookupsByTypeName_ShouldSendQueryWithCorrectType()
        {
            var vm = new AuthReferenceLookupVm();
            _mediator.Setup(x => x.Send(It.IsAny<GetAuthReferenceLookupQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(vm);

            var result = await _sut.GetAuthReferenceLookupsByTypeName(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey, "auth_type");

            _mediator.Verify(x => x.Send(
                It.Is<GetAuthReferenceLookupQuery>(q => q.Type == "auth_type"),
                It.IsAny<CancellationToken>()), Times.Once);
            result.Value.Should().BeSameAs(vm);
        }

        [Test]
        public async Task GetAuthReferenceLookupsByTypeName_ShouldSendQueryWithNullType_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetAuthReferenceLookupQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new AuthReferenceLookupVm());

            await _sut.GetAuthReferenceLookupsByTypeName(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey, null);

            _mediator.Verify(x => x.Send(
                It.Is<GetAuthReferenceLookupQuery>(q => q.Type == null),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // ActivateOrInActivateUser  POST
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task ActivateOrInActivateUser_ShouldReturn200_WhenValidRequestProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<ActivateOrInActivateUserRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(1);

            var result = await _sut.ActivateOrInActivateUser(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new ActivateOrInActivateUserRequest { userId = "u1", IsActive = true });

            result.Value.Should().Be(200);
        }

        [Test]
        public async Task ActivateOrInActivateUser_ShouldReturn200_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<ActivateOrInActivateUserRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(1);

            var result = await _sut.ActivateOrInActivateUser(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new ActivateOrInActivateUserRequest());

            result.Value.Should().Be(200);
            _mediator.Verify(x => x.Send(It.IsAny<ActivateOrInActivateUserRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // GetUsersByStatus  GET
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task GetUsersByStatus_ShouldSendQueryWithCorrectStatus()
        {
            var vm = new List<UsersDto>();
            _mediator.Setup(x => x.Send(It.IsAny<GetUsersByStatusQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(vm);

            var result = await _sut.GetUsersByStatus(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey, "active");

            _mediator.Verify(x => x.Send(
                It.Is<GetUsersByStatusQuery>(q => q.Status == "active"),
                It.IsAny<CancellationToken>()), Times.Once);
            result.Value.Should().BeSameAs(vm);
        }

        [Test]
        public async Task GetUsersByStatus_ShouldSendQueryWithDefaultStatus_WhenNotProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetUsersByStatusQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new List<UsersDto>());

            // Default value is "all"
            await _sut.GetUsersByStatus(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey);

            _mediator.Verify(x => x.Send(
                It.Is<GetUsersByStatusQuery>(q => q.Status == "all"),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // GetTeams  GET
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task GetTeams_ShouldReturnTeamListVm_WhenCalled()
        {
            var vm = new TeamListVm();
            _mediator.Setup(x => x.Send(It.IsAny<GetTeamsQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(vm);

            var result = await _sut.GetTeams(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey);

            result.Value.Should().BeSameAs(vm);
        }

        [Test]
        public async Task GetTeams_ShouldCallMediatorOnce_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetTeamsQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new TeamListVm());

            await _sut.GetTeams(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey);

            _mediator.Verify(x => x.Send(It.IsAny<GetTeamsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // GetTeamById  GET
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task GetTeamById_ShouldSendQueryWithCorrectTeamId()
        {
            var vm = new TeamVm();
            _mediator.Setup(x => x.Send(It.IsAny<GetTeamByIdQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(vm);

            var result = await _sut.GetTeamById(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey, "team-001");

            _mediator.Verify(x => x.Send(
                It.Is<GetTeamByIdQuery>(q => q.TeamId == "team-001"),
                It.IsAny<CancellationToken>()), Times.Once);
            result.Value.Should().BeSameAs(vm);
        }

        [Test]
        public async Task GetTeamById_ShouldSendQueryWithNullTeamId_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetTeamByIdQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new TeamVm());

            await _sut.GetTeamById(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey, null);

            _mediator.Verify(x => x.Send(
                It.Is<GetTeamByIdQuery>(q => q.TeamId == null),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // GetTeamsByUserId  GET
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task GetTeamsByUserId_ShouldSendQueryWithCorrectUserId()
        {
            var vm = new TeamListVm();
            _mediator.Setup(x => x.Send(It.IsAny<GetTeamsByUserIdQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(vm);

            var result = await _sut.GetTeamsByUserId(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey, "user-001");

            _mediator.Verify(x => x.Send(
                It.Is<GetTeamsByUserIdQuery>(q => q.UserId == "user-001"),
                It.IsAny<CancellationToken>()), Times.Once);
            result.Value.Should().BeSameAs(vm);
        }

        [Test]
        public async Task GetTeamsByUserId_ShouldSendQueryWithNullUserId_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetTeamsByUserIdQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new TeamListVm());

            await _sut.GetTeamsByUserId(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey, null);

            _mediator.Verify(x => x.Send(
                It.Is<GetTeamsByUserIdQuery>(q => q.UserId == null),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // AddTeam  POST  (returns string)
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task AddTeam_ShouldReturnTeamId_WhenValidRequestProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<AddTeamRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync("team-new-001");

            var result = await _sut.AddTeam(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new AddTeamRequest { TeamName = "Alpha Squad" });

            result.Value.Should().Be("team-new-001");
        }

        [Test]
        public async Task AddTeam_ShouldReturnNullId_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<AddTeamRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((string)null);

            var result = await _sut.AddTeam(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new AddTeamRequest());

            result.Value.Should().BeNull();
            _mediator.Verify(x => x.Send(It.IsAny<AddTeamRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task AddTeam_ShouldPassRequestDirectlyToMediator()
        {
            var request = new AddTeamRequest { TeamName = "Beta Squad", TeamOwnerId = "u1" };
            _mediator.Setup(x => x.Send(It.IsAny<AddTeamRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync("team-beta");

            await _sut.AddTeam(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey, request);

            _mediator.Verify(x => x.Send(request, It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // AddTeamMembers  POST
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task AddTeamMembers_ShouldReturn200_WhenValidRequestProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<AddTeamMembersRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

            var result = await _sut.AddTeamMembers(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new AddTeamMembersRequest());

            result.Value.Should().Be(200);
        }

        [Test]
        public async Task AddTeamMembers_ShouldReturn200_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<AddTeamMembersRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

            var result = await _sut.AddTeamMembers(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new AddTeamMembersRequest());

            result.Value.Should().Be(200);
            _mediator.Verify(x => x.Send(It.IsAny<AddTeamMembersRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // RemoveTeamMember  POST
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task RemoveTeamMember_ShouldReturn200_WhenValidRequestProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<RemoveTeamMemberRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

            var result = await _sut.RemoveTeamMember(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new RemoveTeamMemberRequest());

            result.Value.Should().Be(200);
        }

        [Test]
        public async Task RemoveTeamMember_ShouldReturn200_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<RemoveTeamMemberRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

            var result = await _sut.RemoveTeamMember(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey,
                new RemoveTeamMemberRequest());

            result.Value.Should().Be(200);
            _mediator.Verify(x => x.Send(It.IsAny<RemoveTeamMemberRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // GetTeamMembersByTeamId  GET
        // ──────────────────────────────────────────────────────────────

        [Test]
        public async Task GetTeamMembersByTeamId_ShouldSendQueryWithCorrectTeamId()
        {
            var vm = new TeamMemberListVm();
            _mediator.Setup(x => x.Send(It.IsAny<GetTeamMembersByTeamIdQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(vm);

            var result = await _sut.GetTeamMembersByTeamId(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey, "team-001");

            _mediator.Verify(x => x.Send(
                It.Is<GetTeamMembersByTeamIdQuery>(q => q.TeamId == "team-001"),
                It.IsAny<CancellationToken>()), Times.Once);
            result.Value.Should().BeSameAs(vm);
        }

        [Test]
        public async Task GetTeamMembersByTeamId_ShouldSendQueryWithNullTeamId_WhenEmptyJsonProvided()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetTeamMembersByTeamIdQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new TeamMemberListVm());

            await _sut.GetTeamMembersByTeamId(CorrelationId, RequestId, RequestOid, RequestUid, ApiKey, null);

            _mediator.Verify(x => x.Send(
                It.Is<GetTeamMembersByTeamIdQuery>(q => q.TeamId == null),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
