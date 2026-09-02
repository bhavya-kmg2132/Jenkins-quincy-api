using Application.Common.Exceptions;
using FluentAssertions;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using System;
using Application.User.Command.UpdateUserAccessLevel;

namespace Application.IntegrationTests.User.Commands
{
    using static Testing;

    [TestFixture]
    public class UpdateUserAccessLevelRequestTest : BaseTestFixture
    {
       

        [SetUp]
        public async Task DerivedSetUp()
        {
            await RunAsDefaultUserAsync();
        }

        [TearDown]
        public void DerivedTearDown()
        {
        }

        /// <summary>
        /// Test for Update Prospect
        /// </summary>
        [Test]
        public async Task ShouldUpdateUserAccessLevel()
        {
            //1. Create UserAccessLevel
            var createUserAccessLevel = new UpdateUserAccessLevelRequest
            {
                UserId = "8f131307-5589-4759-80b6-d4c930f8da9c",//Level3 User1 Id
                AccessLevelValue = "Level1"

            };
            await SendAsync(createUserAccessLevel);

           
            //3. Find it in Database with ProspectId
            //Gets Prospect and ProspectInformation
            var userDataAccess = GetUserDataAccess();
            var user = await userDataAccess.GetUserFromDb("8f131307-5589-4759-80b6-d4c930f8da9c");//Level3 User1 Id

            //4. Assertion
            //4.1 Testing the updated user to be not null
            user.Should().NotBeNull();

            //4.2 Testing values updated in DB
            //a. user
            user.Id.Should().Be(createUserAccessLevel.UserId);
            user.AccessLevel.Should().Be(createUserAccessLevel.AccessLevelValue);

            //-----------------------------------------------------------------------------
            // Change users access level back to correct one and check that in asserts
            //-----------------------------------------------------------------------------

            //1. Create UserAccessLevel
            var createUserAccessLevel3 = new UpdateUserAccessLevelRequest
            {
                UserId = "8f131307-5589-4759-80b6-d4c930f8da9c",//Level3 User1 Id
                AccessLevelValue = "Level3"

            };
            await SendAsync(createUserAccessLevel3);


            //3. Find it in Database with ProspectId
            //Gets Prospect and ProspectInformation
            var userDataAccess3 = GetUserDataAccess();
            var user3 = await userDataAccess3.GetUserFromDb("8f131307-5589-4759-80b6-d4c930f8da9c");//Level3 User1 Id

            //4. Assertion
            //4.1 Testing the updated user to be not null
            user3.Should().NotBeNull();

            //4.2 Testing values updated in DB
            //a. user
            user3.Id.Should().Be(createUserAccessLevel3.UserId);
            user3.AccessLevel.Should().Be(createUserAccessLevel3.AccessLevelValue);
        }
    }
}
