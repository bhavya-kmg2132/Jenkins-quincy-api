using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.AcmeProduct.Commands.CreateAcmeProduct;
using Domain.Common;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.AcmeProduct.Commands
{
    using static Testing;

    [TestFixture]
    public class CreateAcmeProductTest : BaseTestFixture
    {
        string Id = null;

        [SetUp]
        public async Task DerivedSetUp()
        {
            //await RunAsDefaultUserAsync();
            await Task.CompletedTask;
        }

        [TearDown]
        public async Task DerivedTearDown()
        {
            if (Id != null)
            {
                var acmeProductDataAccess = GetAcmeDataAccess();
                await acmeProductDataAccess.DeletePermanentAcmeProduct(Id);
                Id = null;
            }
        }

        [Test]

        public async Task ShouldCreateAcmeProduct()
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

            var dataAccess = GetAcmeDataAccess();

            // Retrieve reference custom fields from the database using DataAccess
            var referenceCustomFields = await dataAccess.GetReferenceCustomFields("AcmeProduct");
            List<CustomField> referenceCustomFieldListFromDatabase = referenceCustomFields.CustomFields;

            //Extract Custom Field For Insert Operation
            var customFieldResult = Helper.ExtractCustomFieldForInsertOperation(AcmeProduct.CustomFields, referenceCustomFieldListFromDatabase);

            // Get CacheKey of created Acme 
            Id = await SendAsync(AcmeProduct);

            // Find Created Acme in the database
            var acmeProduct = await dataAccess.GetAcmeProductById(Id);

            // Test that the acmeProduct in data access is same as the created one.
            //acmeProduct.CacheKey.Should().Be(Acme.CacheKey);
            acmeProduct.Name.Should().Be(AcmeProduct.Name);
            //acmeProduct.Desc.Should().Be(AcmeProduct.Desc);
            //acmeProduct.Image.Should().Be(AcmeProduct.Image);

            // 5. Test CustomFields
            // field_value is dynamic; System.Text.Json deserializes it as JsonElement from DB,
            // so compare via ToString() to avoid type-mismatch with the string from customFieldResult.
            acmeProduct.CustomFields.Should().NotBeNull();
            acmeProduct.CustomFields
                .Select(f => new { f.field_name, value = f.field_value?.ToString() })
                .Should().BeEquivalentTo(
                    customFieldResult.Select(f => new { f.field_name, value = f.field_value?.ToString() }));

        }
    }
}
