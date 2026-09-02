using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.AcmeProduct.Commands.CreateAcmeProduct;
using Application.AcmeProduct.Queries.GetAcmeProductById;
using Domain.Common;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.AcmeProduct.Queries
{
    using static Testing;

    [TestFixture]
    public class GetAcmeProductByIdTest : BaseTestFixture
    {
        string AcmeProductId = null;

        [SetUp]
        public async Task DerivedSetUp()
        {
            //await RunAsDefaultUserAsync();
            await Task.CompletedTask;
        }

        [TearDown]
        public async Task DerivedTearDown()
        {
            if (AcmeProductId != null)
            {
                var acmeProductDataAccess = GetAcmeDataAccess();
                await acmeProductDataAccess.DeletePermanentAcmeProduct(AcmeProductId);
                AcmeProductId = null;
            }
        }
        /// <summary>
        /// Test to get AcmeProduct list based on CacheKey
        /// </summary>
        [Test]
        public async Task ShouldReturnAcmeProductById()
        {

            var dataAccess = GetAcmeDataAccess();
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
            AcmeProductId = await SendAsync(AcmeProduct);

            // Retrieve reference custom fields from the database using DataAccess
            var referenceCustomFields = await dataAccess.GetReferenceCustomFields("AcmeProduct");
            List<CustomField> referenceCustomFieldListFromDatabase = referenceCustomFields.CustomFields;

            var customFieldResult = Helper.ExtractCustomFieldForInsertOperation(AcmeProduct.CustomFields, referenceCustomFieldListFromDatabase);

            var query = new GetAcmeProductByIdQuery()
            {
                Id = AcmeProductId,
            };

            //3.Find it in Database with AcmeProductId
            var result = await SendAsync(query);

            //4.Assertion
            result.Id.Should().Be(AcmeProductId);
            result.Name.Should().Be(AcmeProduct.Name);
            //result.Desc.Should().Be(AcmeProduct.Desc);
            //result.Image.Should().Be(AcmeProduct.Image);

            // 5. Test CustomFields
            // field_value is dynamic; System.Text.Json deserializes it as JsonElement from DB.
            result.CustomFields.Should().NotBeNull();
            result.CustomFields
                .Select(f => new { f.field_name, value = f.field_value?.ToString() })
                .Should().BeEquivalentTo(
                    customFieldResult.Select(f => new { f.field_name, value = f.field_value?.ToString() }));
        }
    }
}
