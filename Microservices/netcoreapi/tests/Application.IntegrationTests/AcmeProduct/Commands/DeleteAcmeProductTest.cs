using System.Collections.Generic;
using System.Threading.Tasks;
using Application.AcmeProduct.Commands.CreateAcmeProduct;
using Application.AcmeProduct.Commands.DeleteAcmeProduct;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.AcmeProduct.Commands
{
    using static Testing;

    [TestFixture]
    public class DeleteAcmeProductTest : BaseTestFixture
    {
        string acmeProductId = null;


        [SetUp]
        public async Task DerivedSetUp()
        {
            //await RunAsDefaultUserAsync();
            await Task.CompletedTask;
        }


        [TearDown]
        public async Task DerivedTearDown()
        {
            if (acmeProductId != null)
            {
                var acmeProductDataAccess = GetAcmeDataAccess();
                await acmeProductDataAccess.DeletePermanentAcmeProduct(acmeProductId);
                acmeProductId = null;
            }
        }

        /// <summary>
        /// Test to Delete AcmeProductId
        /// </summary>

        [Test]
        public async Task ShouldDeleteAcmeProduct()
        {
            // 1 Create Acme
            var AcmeProduct = new CreateAcmeProductRequest
            {
                Name = GetUniqueName(),
                //Desc = "12345678901",
                //Image = "DummyAcme.Image",
                CustomFields = new Dictionary<string, string>
                 {
                      { "abc_code", "execuads" },
                      { "ved_code", "vc" },
                      { "extended_family_name", "cv" },
                      { "extended_code", "sd" },
                      { "version_value", "sd" },
                      { "version_data", "sd" },
                      { "decode_version", "sd" },
                      { "decode_value", "sd" },
                      { "extended_version", "sd" }
                     // Add more key-value pairs as needed
                 }
            };

            //2 Get CacheKey of created Acme 
            acmeProductId = await SendAsync(AcmeProduct);

            //3 Delete the CreateAcmeProduct
            DeleteAcmeProductRequest deleteAcmeProduct = new DeleteAcmeProductRequest
            {
                Id = acmeProductId,
                //CreatedBy = Testing.TestRunnerUserId
            };

            var deleteAcmeProductResponse = await SendAsync(deleteAcmeProduct);

            // Finding delete Acme
            var dataAccess = GetAcmeDataAccess();
            var acmeProduct = await dataAccess.GetAcmeProductById(acmeProductId);

            // Null should be returned.
            acmeProduct.Should().BeNull();
        }
    }
}
