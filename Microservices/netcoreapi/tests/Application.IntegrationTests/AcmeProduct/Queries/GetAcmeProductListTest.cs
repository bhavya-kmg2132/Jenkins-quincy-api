using System.Threading.Tasks;
using Application.AcmeProduct.Queries.GetAcmeProductList;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.AcmeProduct.Queries
{
    using static Testing;

    [TestFixture]
    public class GetAcmeProductListTest : BaseTestFixture
    {
        [SetUp]
        public async Task DerivedSetUp()
        {
            // await RunAsDefaultUserAsync();
            await Task.CompletedTask;
        }

        [TearDown]
        public void DerivedTearDown() { }

        /// <summary>
        /// Test to get AcmeProduct list.
        /// </summary>
        /// <returns>void</returns>
        [Test]
        public async Task ShouldReturnAcmeProductList()
        {
            var query = new GetAcmeProductListQuery();

            //Find it in Database with AcmeProductId
            var result = await SendAsync(query);

            //Assertion
            result.AcmeProductList.Should().HaveCountGreaterThan(0);
        }
    }
}
