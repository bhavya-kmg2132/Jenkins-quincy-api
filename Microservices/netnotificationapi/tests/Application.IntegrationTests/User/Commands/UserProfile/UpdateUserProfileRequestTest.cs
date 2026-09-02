using Application.Common.Exceptions;
using FluentAssertions;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using System;
using Application.User.Command;
using Application.Users.Command.UpdateUserProfile;
using Domain.Entities;

namespace Application.IntegrationTests.User.Commands
{
    using static Testing;

    [TestFixture]
    public class UpdateUserProfileRequestTest : BaseTestFixture
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
        public async Task ShouldUpdateUserProfile()
        {
            //1. Create UserAccessLevel
            var updateUserProfile = new UpdateUserProfileRequest
            {
                UserId = "0f14d87f-b8fb-4d8c-a17f-cf12a90dc76a",//Level3 User1 Id
                BranchId= 1,
                EPICLookupCode ="Marketing",
                LinkedInUrl ="www.marketing.com"
            };
            await SendAsync(updateUserProfile);

           
            //3. Find it in Database with userid
            var userDataAccess = GetUserDataAccess();
            var user = await userDataAccess.GetUserProfileByUserId("0f14d87f-b8fb-4d8c-a17f-cf12a90dc76a");

            //4. Assertion
            //4.1 Testing the updated user profile to be not null
            user.Should().NotBeNull();
            user.Should().HaveCountGreaterThan(0);
        }
    }
}
