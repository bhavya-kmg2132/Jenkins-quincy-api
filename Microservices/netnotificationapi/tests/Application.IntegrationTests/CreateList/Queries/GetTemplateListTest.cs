using Application.Contacts.Queries.GetContactListQuery; 
using Application.List.Queries;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.IntegrationTests.CreateList.Queries
{
    using static Testing;

    public class GetTemplateListTest : TestBase
    {
        /// <summary>
        /// Test to get Manage list.
        /// </summary>
        /// <returns>void</returns>
        [Test] 
        public async Task ShouldReturnCreateList()
        {
             var query = new GetTemplateListQuery();
                       
            var result = await SendAsync(query);
            result.List.Should().HaveCountGreaterThan(0);
        }
    }
}
