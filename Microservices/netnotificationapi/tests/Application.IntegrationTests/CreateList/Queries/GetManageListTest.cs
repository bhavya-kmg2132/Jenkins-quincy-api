using Application.Contacts.Queries.GetContactListQuery;
 

using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.IntegrationTests.CreateList.Queries
{
    using static Testing;

    public class GetManageListTest : TestBase
    {
        /// <summary>
        /// Test to get Manage list.
        /// </summary>
        /// <returns>void</returns>
        [Test] 
        public async Task ShouldReturnManageList()
        {
             var query = new GetListQuery();
                       
            var result = await SendAsync(query);
            result.CreateListFilters.Primary.Should().HaveCountGreaterThan(0);
        }
    }
}
