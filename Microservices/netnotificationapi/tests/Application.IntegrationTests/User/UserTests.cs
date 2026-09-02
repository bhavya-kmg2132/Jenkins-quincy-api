//using System;
//using Application.Common.Exceptions;
//using FluentAssertions;
//using NUnit.Framework;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using Application.Users.Commands.CreateUser;
//using Application.WeatherForecasts.Queries.GetWeatherForecasts;
//using Application.TodoLists.Queries.GetTodos;
//using Application.Users.Commands.AddRoles;
//using Application.Users.Commands.RolePermissions;
//using Application.Users.Commands.UserPermissions.AddPermissionsDeniedForUser;

//namespace Application.IntegrationTests.User
//{
//    using static Testing;

//    [TestFixture]
//    public class Test : BaseTestFixture
//    {
//        [SetUp]
//        public void DerivedSetUp() 
        //{
        //    RunAsDefaultUserAsync();
        //}

//        [TearDown]
//        public void DerivedTearDown() { }

//        /// <summary>
//        ///1. SALES DIRECTOR
//        /// Test for :
//        ///i)Check for access
//        ///ii)Check for permission granted but not in role
//        ///iii)Check for permission in role but not in granted
//        ///iv)Check for Role as 'User' by default
//        ///v)Check for permission denied 
//        /// </summary>
//        [Test]
//        //[SetUp]
//        //[TearDown]
//        public async Task ShouldCheckUserForSalesDirector()
//        {
//            //1. Get UserId
//            var UserId = "0f14d87f-b8fb-4d8c-a17f-cf12a90dc76a";

//            //2.Add Role for User
//            //2.1 Create Role (Sales Director)
//            var role = new Domain.Entities.Role()
//            {
//                Id = "084cf24e-092b-4b9f-bebd-4eb2a7e59424"
//            };

//            //2.2 Add role
//            var addRole = new AddRolesForUserRequest
//            {
//                Roles = new List<Domain.Entities.Role> { role },
//                UserId = UserId
//            };
//            await SendAsync(addRole);

//            //3.Add permissions 
//            //3.1 Add permissions for Role
//            var rolePermission1 = new Domain.Entities.Permission
//            {
//                PermissionId = "540D0009-2EA8-4818-B3C5-C11A92C78005" //RolePermission1 : GetTodosQuery
//            };

//            var rolePermission2 = new Domain.Entities.Permission
//            {
//                PermissionId = "B06A8230-1980-4839-BCA6-C88E68E592B9"  //RolePermission2 : GetWeatherForecastsQuery
//            };

//            var addPermissionsForRole = new AddPermissionsForRoleRequest
//            {
//                Id = role.Id,
//                RolePermissions = new List<Domain.Entities.Permission> { rolePermission1, rolePermission2 },
//            };
//            await SendAsync(addPermissionsForRole);

//            //3.2 Add permissions granted for User
//            string permissionGranted1 = "B06A8230-1980-4839-BCA6-C88E68E592B9"; //PermissionGranted1 : GetWeatherForecastsQuery
//            string permissionGranted2 = "C1B232A7-B882-47EB-B8BD-A3791919BE5C"; //PermissionGranted2 : GetResultQuery (NOT IN RolePermission)

//            var addPermissionGrantedForUser = new AddPermissionGrantedForUserRequest
//            {
//                UserId = UserId,
//                PermissionId = new List<string> { permissionGranted1, permissionGranted2 },
//            };
//            await SendAsync(addPermissionGrantedForUser);

//            //3.3 Add permission denied for User
//            string permissionDenied1 = "B06A8230-1980-4839-BCA6-C88E68E592B9"; //PermissionDenied : GetWeatherForecastsQuery

//            var addPermissionDeniedForUser = new AddPermissionDeniedForUserRequest
//            {
//                UserId = UserId,
//                PermissionId = new List<string> { permissionDenied1 },
//            };
//            await SendAsync(addPermissionDeniedForUser);

//            //4. Find created User in the data access.
//            var dataAccess = GetUserDataAccess();
//            var user = await dataAccess.GetUserFromDb(UserId);

//            //5. Assertion
//            //5.1 Test that created user should not be null.
//            user.Should().NotBeNull();

//            //5.2 Check for Access
//            var query1 = new GetWeatherForecastsQuery();  //check Access for PermissionGranted but not in RolePermission
//            query1.Should().NotBeNull();

//            var query2 = new GetTodosQuery();   //check Access for RolePermission but not in PermissionGranted
//            query2.Should().NotBeNull();

//            //5.3 Check for permission granted but not in role
//            foreach (var permissionGranted in user.PermissionsGranted)
//            {
//                permissionGranted.PermissionId.Should().BeOneOf(permissionGranted1, permissionGranted2);
//                permissionGranted.PermissionId.Should().NotBe(rolePermission1.PermissionId);
//            };

//            //5.4 Check for permission in role but not in granted
//            foreach (var permissionInRole in user.Roles[0].RolePermissions)
//            {
//                permissionInRole.PermissionId.Should().BeOneOf(rolePermission1.PermissionId, rolePermission2.PermissionId);
//                permissionInRole.PermissionId.Should().NotBe(permissionGranted2);
//            };

//            //5.5 Check for Role as 'User' by default
//            user.Roles.Should().Contain(s => s.RoleName.Equals("User"));

//            //5.6 Check for permission denied 
//            foreach (var permissionDenied in user.PermissionsDenied)
//            {
//                permissionDenied.PermissionId.Should().Be(permissionDenied1);
//            };
//        }

//        /// <summary>
//        ///2. MARKETING MANAGER
//        /// Test for :
//        ///i)Check for access
//        ///ii)Check for permission granted but not in role
//        ///iii)Check for permission in role but not in granted
//        ///iv)Check for Role as 'User' by default
//        ///v)Check for permission denied 
//        /// </summary>
//        [Test]
//        public async Task ShouldCheckUserForMarketingManager()
//        {
//            //1. Get UserId
//            var UserId = "0f14d87f-b8fb-4d8c-a17f-cf12a90dc76a";

//            //2.Add Role for User
//            //2.1 Create Role (Marketing Manager)
//            var role = new Domain.Entities.Role()
//            {
//                Id = "11cbe43f-73d0-431e-b384-52adb1eccf6f"
//            };

//            //2.2 Add role
//            var addRole = new AddRolesForUserRequest
//            {
//                Roles = new List<Domain.Entities.Role> { role },
//                UserId = UserId
//            };

//            //3.Add permissions 
//            //3.1 Add permissions for Role
//            var rolePermission1 = new Domain.Entities.Permission
//            {
//                PermissionId = "540D0009-2EA8-4818-B3C5-C11A92C78005" //RolePermission1 : GetTodosQuery
//            };

//            var rolePermission2 = new Domain.Entities.Permission
//            {
//                PermissionId = "B06A8230-1980-4839-BCA6-C88E68E592B9"  //RolePermission2 : GetWeatherForecastsQuery
//            };

//            var addPermissionsForRole = new AddPermissionsForRoleRequest
//            {
//                Id = role.Id,
//                RolePermissions = new List<Domain.Entities.Permission> { rolePermission1, rolePermission2 },
//            };

//            //3.2 Add permissions granted for User
//            string permissionGranted1 = "B06A8230-1980-4839-BCA6-C88E68E592B9"; //PermissionGranted1 : GetWeatherForecastsQuery
//            string permissionGranted2 = "C1B232A7-B882-47EB-B8BD-A3791919BE5C"; //PermissionGranted2 : GetResultQuery (NOT IN RolePermission)

//            var addPermissionGrantedForUser = new AddPermissionGrantedForUserRequest
//            {
//                UserId = UserId,
//                PermissionId = new List<string> { permissionGranted1, permissionGranted2 },
//            };

//            //3.3 Add permission denied for User
//            string permissionDenied1 = "B06A8230-1980-4839-BCA6-C88E68E592B9"; //PermissionDenied : GetWeatherForecastsQuery

//            var addPermissionDeniedForUser = new AddPermissionDeniedForUserRequest
//            {
//                UserId = UserId,
//                PermissionId = new List<string> { permissionDenied1 },
//            };

//            //4. Find created User in the data access.
//            var dataAccess = GetUserDataAccess();
//            var user = await dataAccess.GetUserFromDb(UserId);

//            //5. Assertion
//            //5.1 Test that created user should not be null.
//            user.Should().NotBeNull();

//            //5.2 Check for Access
//            var query1 = new GetWeatherForecastsQuery();  //check Access for PermissionGranted but not in RolePermission
//            query1.Should().NotBeNull();

//            var query2 = new GetTodosQuery();   //check Access for RolePermission but not in PermissionGranted
//            query2.Should().NotBeNull();

//            //5.3 Check for permission granted but not in role
//            foreach (var permissionGranted in user.PermissionsGranted)
//            {
//                permissionGranted.PermissionId.Should().BeOneOf(permissionGranted1, permissionGranted2);
//                permissionGranted.PermissionId.Should().NotBe(rolePermission1.PermissionId);
//            };

//            //5.4 Check for permission in role but not in granted
//            foreach (var permissionInRole in user.Roles[0].RolePermissions)
//            {
//                permissionInRole.PermissionId.Should().BeOneOf(rolePermission1.PermissionId, rolePermission2.PermissionId);
//                permissionInRole.PermissionId.Should().NotBe(permissionGranted2);
//            };

//            ////5.5 Check for Role as 'User' by default
//            user.Roles.Should().Contain(s => s.RoleName.Equals("User"));

//            //5.6 Check for permission denied 
//            foreach (var permissionDenied in user.PermissionsDenied)
//            {
//                permissionDenied.PermissionId.Should().Be(permissionDenied1);
//            };
//        }


//        /// <summary>
//        ///3. ACCOUNT MANAGER
//        /// Test for :
//        ///i)Check for access
//        ///ii)Check for permission granted but not in role
//        ///iii)Check for permission in role but not in granted
//        ///iv)Check for Role as 'User' by default
//        ///v)Check for permission denied 
//        /// </summary>
//        [Test]
//        public async Task ShouldCheckUserForAccountManager()
//        {
//            //1. Get UserId
//            var UserId = "0f14d87f-b8fb-4d8c-a17f-cf12a90dc76a";

//            //2.Add Role for User
//            //2.1 Create Role (Account Manager)
//            var role = new Domain.Entities.Role()
//            {
//                Id = "2b0aa9cf-0122-4da1-9b4e-0a22f036975b"
//            };

//            //2.2 Add role
//            var addRole = new AddRolesForUserRequest
//            {
//                Roles = new List<Domain.Entities.Role> { role },
//                UserId = UserId
//            };

//            //3.Add permissions 
//            //3.1 Add permissions for Role
//            var rolePermission1 = new Domain.Entities.Permission
//            {
//                PermissionId = "540D0009-2EA8-4818-B3C5-C11A92C78005" //RolePermission1 : GetTodosQuery
//            };

//            var rolePermission2 = new Domain.Entities.Permission
//            {
//                PermissionId = "B06A8230-1980-4839-BCA6-C88E68E592B9"  //RolePermission2 : GetWeatherForecastsQuery
//            };

//            var addPermissionsForRole = new AddPermissionsForRoleRequest
//            {
//                Id = role.Id,
//                RolePermissions = new List<Domain.Entities.Permission> { rolePermission1, rolePermission2 },
//            };

//            //3.2 Add permissions granted for User
//            string permissionGranted1 = "B06A8230-1980-4839-BCA6-C88E68E592B9"; //PermissionGranted1 : GetWeatherForecastsQuery
//            string permissionGranted2 = "C1B232A7-B882-47EB-B8BD-A3791919BE5C"; //PermissionGranted2 : GetResultQuery (NOT IN RolePermission)

//            var addPermissionGrantedForUser = new AddPermissionGrantedForUserRequest
//            {
//                UserId = UserId,
//                PermissionId = new List<string> { permissionGranted1, permissionGranted2 },
//            };

//            //3.3 Add permission denied for User
//            string permissionDenied1 = "B06A8230-1980-4839-BCA6-C88E68E592B9"; //PermissionDenied : GetWeatherForecastsQuery

//            var addPermissionDeniedForUser = new AddPermissionDeniedForUserRequest
//            {
//                UserId = UserId,
//                PermissionId = new List<string> { permissionDenied1 },
//            };

//            //4. Find created User in the data access.
//            var dataAccess = GetUserDataAccess();
//            var user = await dataAccess.GetUserFromDb(UserId);

//            //5. Assertion
//            //5.1 Test that created user should not be null.
//            user.Should().NotBeNull();

//            //5.2 Check for Access
//            var query1 = new GetWeatherForecastsQuery();  //check Access for PermissionGranted but not in RolePermission
//            query1.Should().NotBeNull();

//            var query2 = new GetTodosQuery();   //check Access for RolePermission but not in PermissionGranted
//            query2.Should().NotBeNull();

//            //5.3 Check for permission granted but not in role
//            foreach (var permissionGranted in user.PermissionsGranted)
//            {
//                permissionGranted.PermissionId.Should().BeOneOf(permissionGranted1, permissionGranted2);
//                permissionGranted.PermissionId.Should().NotBe(rolePermission1.PermissionId);
//            };

//            //5.4 Check for permission in role but not in granted
//            foreach (var permissionInRole in user.Roles[0].RolePermissions)
//            {
//                permissionInRole.PermissionId.Should().BeOneOf(rolePermission1.PermissionId, rolePermission2.PermissionId);
//                permissionInRole.PermissionId.Should().NotBe(permissionGranted2);
//            };

//            //5.5 Check for Role as 'User' by default
//            user.Roles.Should().Contain(s => s.RoleName.Equals("User"));

//            //5.6 Check for permission denied 
//            foreach (var permissionDenied in user.PermissionsDenied)
//            {
//                permissionDenied.PermissionId.Should().Be(permissionDenied1);
//            };
//        }


//        /// <summary>
//        /// 4. SYSTEM MANAGER
//        /// Test for :
//        ///i)Check for access
//        ///ii)Check for permission granted but not in role
//        ///iii)Check for permission in role but not in granted
//        ///iv)Check for Role as 'User' by default
//        ///v)Check for permission denied 
//        /// </summary>
//        [Test]
//        public async Task ShouldCheckUserForSystemManager()
//        {
//            //1. Get UserId
//            var UserId = "0f14d87f-b8fb-4d8c-a17f-cf12a90dc76a";

//            //2.Add Role for User
//            //2.1 Create Role (System Manager)
//            var role = new Domain.Entities.Role()
//            {
//                Id = "6c062ac5-1b91-4b9e-ab70-6d477c159880"
//            };

//            //2.2 Add role
//            var addRole = new AddRolesForUserRequest
//            {
//                Roles = new List<Domain.Entities.Role> { role },
//                UserId = UserId
//            };

//            //3.Add permissions 
//            //3.1 Add permissions for Role
//            var rolePermission1 = new Domain.Entities.Permission
//            {
//                PermissionId = "540D0009-2EA8-4818-B3C5-C11A92C78005" //RolePermission1 : GetTodosQuery
//            };

//            var rolePermission2 = new Domain.Entities.Permission
//            {
//                PermissionId = "B06A8230-1980-4839-BCA6-C88E68E592B9"  //RolePermission2 : GetWeatherForecastsQuery
//            };

//            var addPermissionsForRole = new AddPermissionsForRoleRequest
//            {
//                Id = role.Id,
//                RolePermissions = new List<Domain.Entities.Permission> { rolePermission1, rolePermission2 },
//            };

//            //3.2 Add permissions granted for User
//            string permissionGranted1 = "B06A8230-1980-4839-BCA6-C88E68E592B9"; //PermissionGranted1 : GetWeatherForecastsQuery
//            string permissionGranted2 = "C1B232A7-B882-47EB-B8BD-A3791919BE5C"; //PermissionGranted2 : GetResultQuery (NOT IN RolePermission)

//            var addPermissionGrantedForUser = new AddPermissionGrantedForUserRequest
//            {
//                UserId = UserId,
//                PermissionId = new List<string> { permissionGranted1, permissionGranted2 },
//            };

//            //3.3 Add permission denied for User
//            string permissionDenied1 = "B06A8230-1980-4839-BCA6-C88E68E592B9"; //PermissionDenied : GetWeatherForecastsQuery

//            var addPermissionDeniedForUser = new AddPermissionDeniedForUserRequest
//            {
//                UserId = UserId,
//                PermissionId = new List<string> { permissionDenied1 },
//            };

//            //4. Find created User in the data access.
//            var dataAccess = GetUserDataAccess();
//            var user = await dataAccess.GetUserFromDb(UserId);

//            //5. Assertion
//            //5.1 Test that created user should not be null.
//            user.Should().NotBeNull();

//            //5.2 Check for Access
//            var query1 = new GetWeatherForecastsQuery();  //check Access for PermissionGranted but not in RolePermission
//            query1.Should().NotBeNull();

//            var query2 = new GetTodosQuery();   //check Access for RolePermission but not in PermissionGranted
//            query2.Should().NotBeNull();

//            //5.3 Check for permission granted but not in role
//            foreach (var permissionGranted in user.PermissionsGranted)
//            {
//                permissionGranted.PermissionId.Should().BeOneOf(permissionGranted1, permissionGranted2);
//                permissionGranted.PermissionId.Should().NotBe(rolePermission1.PermissionId);
//            };

//            //5.4 Check for permission in role but not in granted
//            foreach (var permissionInRole in user.Roles[0].RolePermissions)
//            {
//                permissionInRole.PermissionId.Should().BeOneOf(rolePermission1.PermissionId, rolePermission2.PermissionId);
//                permissionInRole.PermissionId.Should().NotBe(permissionGranted2);
//            };

//            //5.5 Check for Role as 'User' by default
//            user.Roles.Should().Contain(s => s.RoleName.Equals("User"));

//            //5.6 Check for permission denied 
//            foreach (var permissionDenied in user.PermissionsDenied)
//            {
//                permissionDenied.PermissionId.Should().Be(permissionDenied1);
//            };
//        }

//        /// <summary>
//        ///5. SENIOR CLIENT EXECUTIVE
//        /// Test for :
//        ///i)Check for access
//        ///ii)Check for permission granted but not in role
//        ///iii)Check for permission in role but not in granted
//        ///iv)Check for Role as 'User' by default
//        ///v)Check for permission denied 
//        /// </summary>
//        [Test]
//        public async Task ShouldCheckUserForSeniorClientExecutive()
//        {
//            //1. Get UserId
//            var UserId = "0f14d87f-b8fb-4d8c-a17f-cf12a90dc76a";

//            //2.Add Role for User
//            //2.1 Create Role (Senior Client Executive)
//            var role = new Domain.Entities.Role()
//            {
//                Id = "8433d7bc-3fe2-41da-affe-b362da8534c6"
//            };

//            //2.2 Add role
//            var addRole = new AddRolesForUserRequest
//            {
//                Roles = new List<Domain.Entities.Role> { role },
//                UserId = UserId
//            };

//            //3.Add permissions 
//            //3.1 Add permissions for Role
//            var rolePermission1 = new Domain.Entities.Permission
//            {
//                PermissionId = "540D0009-2EA8-4818-B3C5-C11A92C78005" //RolePermission1 : GetTodosQuery
//            };

//            var rolePermission2 = new Domain.Entities.Permission
//            {
//                PermissionId = "B06A8230-1980-4839-BCA6-C88E68E592B9"  //RolePermission2 : GetWeatherForecastsQuery
//            };

//            var addPermissionsForRole = new AddPermissionsForRoleRequest
//            {
//                Id = role.Id,
//                RolePermissions = new List<Domain.Entities.Permission> { rolePermission1, rolePermission2 },
//            };

//            //3.2 Add permissions granted for User
//            string permissionGranted1 = "B06A8230-1980-4839-BCA6-C88E68E592B9"; //PermissionGranted1 : GetWeatherForecastsQuery
//            string permissionGranted2 = "C1B232A7-B882-47EB-B8BD-A3791919BE5C"; //PermissionGranted2 : GetResultQuery (NOT IN RolePermission)

//            var addPermissionGrantedForUser = new AddPermissionGrantedForUserRequest
//            {
//                UserId = UserId,
//                PermissionId = new List<string> { permissionGranted1, permissionGranted2 },
//            };

//            //3.3 Add permission denied for User
//            string permissionDenied1 = "B06A8230-1980-4839-BCA6-C88E68E592B9"; //PermissionDenied : GetWeatherForecastsQuery

//            var addPermissionDeniedForUser = new AddPermissionDeniedForUserRequest
//            {
//                UserId = UserId,
//                PermissionId = new List<string> { permissionDenied1 },
//            };

//            //4. Find created User in the data access.
//            var dataAccess = GetUserDataAccess();
//            var user = await dataAccess.GetUserFromDb(UserId);

//            //5. Assertion
//            //5.1 Test that created user should not be null.
//            user.Should().NotBeNull();

//            //5.2 Check for Access
//            var query1 = new GetWeatherForecastsQuery();  //check Access for PermissionGranted but not in RolePermission
//            query1.Should().NotBeNull();

//            var query2 = new GetTodosQuery();   //check Access for RolePermission but not in PermissionGranted
//            query2.Should().NotBeNull();

//            //5.3 Check for permission granted but not in role
//            foreach (var permissionGranted in user.PermissionsGranted)
//            {
//                permissionGranted.PermissionId.Should().BeOneOf(permissionGranted1, permissionGranted2);
//                permissionGranted.PermissionId.Should().NotBe(rolePermission1.PermissionId);
//            };

//            //5.4 Check for permission in role but not in granted
//            foreach (var permissionInRole in user.Roles[0].RolePermissions)
//            {
//                permissionInRole.PermissionId.Should().BeOneOf(rolePermission1.PermissionId, rolePermission2.PermissionId);
//                permissionInRole.PermissionId.Should().NotBe(permissionGranted2);
//            };

//            //5.5 Check for Role as 'User' by default
//            user.Roles.Should().Contain(s => s.RoleName.Equals("User"));

//            //5.6 Check for permission denied 
//            foreach (var permissionDenied in user.PermissionsDenied)
//            {
//                permissionDenied.PermissionId.Should().Be(permissionDenied1);
//            };
//        }

//        /// <summary>
//        ///6. ADMIN
//        /// Test for :
//        ///i)Check for access
//        ///ii)Check for permission granted but not in role
//        ///iii)Check for permission in role but not in granted
//        ///iv)Check for Role as 'User' by default
//        ///v)Check for permission denied 
//        /// </summary>
//        [Test]
//        public async Task ShouldCheckUserForAdmin()
//        {
//            //1. Get UserId
//            var UserId = "0f14d87f-b8fb-4d8c-a17f-cf12a90dc76a";

//            //2.Add Role for User
//            //2.1 Create Role (Admin)
//            var role = new Domain.Entities.Role()
//            {
//                Id = "86bf5045-8553-46d6-bf42-4111d91f3448"
//            };

//            //2.2 Add role
//            var addRole = new AddRolesForUserRequest
//            {
//                Roles = new List<Domain.Entities.Role> { role },
//                UserId = UserId
//            };

//            //3.Add permissions 
//            //3.1 Add permissions for Role
//            var rolePermission1 = new Domain.Entities.Permission
//            {
//                PermissionId = "540D0009-2EA8-4818-B3C5-C11A92C78005" //RolePermission1 : GetTodosQuery
//            };

//            var rolePermission2 = new Domain.Entities.Permission
//            {
//                PermissionId = "B06A8230-1980-4839-BCA6-C88E68E592B9"  //RolePermission2 : GetWeatherForecastsQuery
//            };

//            var addPermissionsForRole = new AddPermissionsForRoleRequest
//            {
//                Id = role.Id,
//                RolePermissions = new List<Domain.Entities.Permission> { rolePermission1, rolePermission2 },
//            };

//            //3.2 Add permissions granted for User
//            string permissionGranted1 = "B06A8230-1980-4839-BCA6-C88E68E592B9"; //PermissionGranted1 : GetWeatherForecastsQuery
//            string permissionGranted2 = "C1B232A7-B882-47EB-B8BD-A3791919BE5C"; //PermissionGranted2 : GetResultQuery (NOT IN RolePermission)

//            var addPermissionGrantedForUser = new AddPermissionGrantedForUserRequest
//            {
//                UserId = UserId,
//                PermissionId = new List<string> { permissionGranted1, permissionGranted2 },
//            };

//            //3.3 Add permission denied for User
//            string permissionDenied1 = "B06A8230-1980-4839-BCA6-C88E68E592B9"; //PermissionDenied : GetWeatherForecastsQuery

//            var addPermissionDeniedForUser = new AddPermissionDeniedForUserRequest
//            {
//                UserId = UserId,
//                PermissionId = new List<string> { permissionDenied1 },
//            };

//            //4. Find created User in the data access.
//            var dataAccess = GetUserDataAccess();
//            var user = await dataAccess.GetUserFromDb(UserId);

//            //5. Assertion
//            //5.1 Test that created user should not be null.
//            user.Should().NotBeNull();

//            //5.2 Check for Access
//            var query1 = new GetWeatherForecastsQuery();  //check Access for PermissionGranted but not in RolePermission
//            query1.Should().NotBeNull();

//            var query2 = new GetTodosQuery();   //check Access for RolePermission but not in PermissionGranted
//            query2.Should().NotBeNull();

//            //5.3 Check for permission granted but not in role
//            foreach (var permissionGranted in user.PermissionsGranted)
//            {
//                permissionGranted.PermissionId.Should().BeOneOf(permissionGranted1, permissionGranted2);
//                permissionGranted.PermissionId.Should().NotBe(rolePermission1.PermissionId);
//            };

//            //5.4 Check for permission in role but not in granted
//            foreach (var permissionInRole in user.Roles[0].RolePermissions)
//            {
//                permissionInRole.PermissionId.Should().BeOneOf(rolePermission1.PermissionId, rolePermission2.PermissionId);
//                permissionInRole.PermissionId.Should().NotBe(permissionGranted2);
//            };

//            //5.5 Check for Role as 'User' by default
//            user.Roles.Should().Contain(s => s.RoleName.Equals("User"));

//            //5.6 Check for permission denied 
//            foreach (var permissionDenied in user.PermissionsDenied)
//            {
//                permissionDenied.PermissionId.Should().Be(permissionDenied1);
//            };
//        }

//        /// <summary>
//        ///7. CLIENT EXECUTIVE
//        /// Test for :
//        ///i)Check for access
//        ///ii)Check for permission granted but not in role
//        ///iii)Check for permission in role but not in granted
//        ///iv)Check for Role as 'User' by default
//        ///v)Check for permission denied 
//        /// </summary>
//        [Test]
//        public async Task ShouldCheckUserForClientExecutive()
//        {
//            //1. Get UserId
//            var UserId = "0f14d87f-b8fb-4d8c-a17f-cf12a90dc76a";

//            //2.Add Role for User
//            //2.1 Create Role (Client Executive)
//            var role = new Domain.Entities.Role()
//            {
//                Id = "c1f391f1-7b68-4a03-87c3-32f0b212e8ba"
//            };

//            //2.2 Add role
//            var addRole = new AddRolesForUserRequest
//            {
//                Roles = new List<Domain.Entities.Role> { role },
//                UserId = UserId
//            };

//            //3.Add permissions 
//            //3.1 Add permissions for Role
//            var rolePermission1 = new Domain.Entities.Permission
//            {
//                PermissionId = "540D0009-2EA8-4818-B3C5-C11A92C78005" //RolePermission1 : GetTodosQuery
//            };

//            var rolePermission2 = new Domain.Entities.Permission
//            {
//                PermissionId = "B06A8230-1980-4839-BCA6-C88E68E592B9"  //RolePermission2 : GetWeatherForecastsQuery
//            };

//            var addPermissionsForRole = new AddPermissionsForRoleRequest
//            {
//                Id = role.Id,
//                RolePermissions = new List<Domain.Entities.Permission> { rolePermission1, rolePermission2 },
//            };

//            //3.2 Add permissions granted for User
//            string permissionGranted1 = "B06A8230-1980-4839-BCA6-C88E68E592B9"; //PermissionGranted1 : GetWeatherForecastsQuery
//            string permissionGranted2 = "C1B232A7-B882-47EB-B8BD-A3791919BE5C"; //PermissionGranted2 : GetResultQuery (NOT IN RolePermission)

//            var addPermissionGrantedForUser = new AddPermissionGrantedForUserRequest
//            {
//                UserId = UserId,
//                PermissionId = new List<string> { permissionGranted1, permissionGranted2 },
//            };

//            //3.3 Add permission denied for User
//            string permissionDenied1 = "B06A8230-1980-4839-BCA6-C88E68E592B9"; //PermissionDenied : GetWeatherForecastsQuery

//            var addPermissionDeniedForUser = new AddPermissionDeniedForUserRequest
//            {
//                UserId = UserId,
//                PermissionId = new List<string> { permissionDenied1 },
//            };

//            //4. Find created User in the data access.
//            var dataAccess = GetUserDataAccess();
//            var user = await dataAccess.GetUserFromDb(UserId);

//            //5. Assertion
//            //5.1 Test that created user should not be null.
//            user.Should().NotBeNull();

//            //5.2 Check for Access
//            var query1 = new GetWeatherForecastsQuery();  //check Access for PermissionGranted but not in RolePermission
//            query1.Should().NotBeNull();

//            var query2 = new GetTodosQuery();   //check Access for RolePermission but not in PermissionGranted
//            query2.Should().NotBeNull();

//            //5.3 Check for permission granted but not in role
//            foreach (var permissionGranted in user.PermissionsGranted)
//            {
//                permissionGranted.PermissionId.Should().BeOneOf(permissionGranted1, permissionGranted2);
//                permissionGranted.PermissionId.Should().NotBe(rolePermission1.PermissionId);
//            };

//            //5.4 Check for permission in role but not in granted
//            foreach (var permissionInRole in user.Roles[0].RolePermissions)
//            {
//                permissionInRole.PermissionId.Should().BeOneOf(rolePermission1.PermissionId, rolePermission2.PermissionId);
//                permissionInRole.PermissionId.Should().NotBe(permissionGranted2);
//            };

//            //5.5 Check for Role as 'User' by default
//            user.Roles.Should().Contain(s => s.RoleName.Equals("User"));

//            //5.6 Check for permission denied 
//            foreach (var permissionDenied in user.PermissionsDenied)
//            {
//                permissionDenied.PermissionId.Should().Be(permissionDenied1);
//            };
//        }

//        /// <summary>
//        ///8. POWER
//        /// Test for :
//        ///i)Check for access
//        ///ii)Check for permission granted but not in role
//        ///iii)Check for permission in role but not in granted
//        ///iv)Check for Role as 'User' by default
//        ///v)Check for permission denied 
//        /// </summary>
//        [Test]
//        public async Task ShouldCheckUserForPower()
//        {
//            //1. Get UserId
//            var UserId = "0f14d87f-b8fb-4d8c-a17f-cf12a90dc76a";

//            //2.Add Role for User
//            //2.1 Create Role (Power)
//            var role = new Domain.Entities.Role()
//            {
//                Id = "fc18c5fd-04a4-436b-9e66-21c2446f09c4"
//            };

//            //2.2 Add role
//            var addRole = new AddRolesForUserRequest
//            {
//                Roles = new List<Domain.Entities.Role> { role },
//                UserId = UserId
//            };

//            //3.Add permissions 
//            //3.1 Add permissions for Role
//            var rolePermission1 = new Domain.Entities.Permission
//            {
//                PermissionId = "540D0009-2EA8-4818-B3C5-C11A92C78005" //RolePermission1 : GetTodosQuery
//            };

//            var rolePermission2 = new Domain.Entities.Permission
//            {
//                PermissionId = "B06A8230-1980-4839-BCA6-C88E68E592B9"  //RolePermission2 : GetWeatherForecastsQuery
//            };

//            var addPermissionsForRole = new AddPermissionsForRoleRequest
//            {
//                Id = role.Id,
//                RolePermissions = new List<Domain.Entities.Permission> { rolePermission1, rolePermission2 },
//            };

//            //3.2 Add permissions granted for User
//            string permissionGranted1 = "B06A8230-1980-4839-BCA6-C88E68E592B9"; //PermissionGranted1 : GetWeatherForecastsQuery
//            string permissionGranted2 = "C1B232A7-B882-47EB-B8BD-A3791919BE5C"; //PermissionGranted2 : GetResultQuery (NOT IN RolePermission)

//            var addPermissionGrantedForUser = new AddPermissionGrantedForUserRequest
//            {
//                UserId = UserId,
//                PermissionId = new List<string> { permissionGranted1, permissionGranted2 },
//            };

//            //3.3 Add permission denied for User
//            string permissionDenied1 = "B06A8230-1980-4839-BCA6-C88E68E592B9"; //PermissionDenied : GetWeatherForecastsQuery

//            var addPermissionDeniedForUser = new AddPermissionDeniedForUserRequest
//            {
//                UserId = UserId,
//                PermissionId = new List<string> { permissionDenied1 },
//            };

//            //4. Find created User in the data access.
//            var dataAccess = GetUserDataAccess();
//            var user = await dataAccess.GetUserFromDb(UserId);

//            //5. Assertion
//            //5.1 Test that created user should not be null.
//            user.Should().NotBeNull();

//            //5.2 Check for Access
//            var query1 = new GetWeatherForecastsQuery();  //check Access for PermissionGranted but not in RolePermission
//            query1.Should().NotBeNull();

//            var query2 = new GetTodosQuery();   //check Access for RolePermission but not in PermissionGranted
//            query2.Should().NotBeNull();

//            //5.3 Check for permission granted but not in role
//            foreach (var permissionGranted in user.PermissionsGranted)
//            {
//                permissionGranted.PermissionId.Should().BeOneOf(permissionGranted1, permissionGranted2);
//                permissionGranted.PermissionId.Should().NotBe(rolePermission1.PermissionId);
//            };

//            //5.4 Check for permission in role but not in granted
//            foreach (var permissionInRole in user.Roles[0].RolePermissions)
//            {
//                permissionInRole.PermissionId.Should().BeOneOf(rolePermission1.PermissionId, rolePermission2.PermissionId);
//                permissionInRole.PermissionId.Should().NotBe(permissionGranted2);
//            };

//            //5.5 Check for Role as 'User' by default
//            user.Roles.Should().Contain(s => s.RoleName.Equals("User"));

//            //5.6 Check for permission denied 
//            foreach (var permissionDenied in user.PermissionsDenied)
//            {
//                permissionDenied.PermissionId.Should().Be(permissionDenied1);
//            };
//        }
//    }
//}
