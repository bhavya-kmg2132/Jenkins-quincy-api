using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.AcmeProduct.Commands.CreateAcmeProduct;
using Application.AcmeProduct.Commands.UpdateAcmeProduct;
using Domain.Common;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.AcmeProduct.Commands
{
    using static Testing;

    [TestFixture]
    public class UpdateAcmeProductTest : BaseTestFixture
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
        /// Test for Update acmeProduct
        /// </summary>

        [Test]

        public async Task shouldUpdatAcmeProduct()
        {
            var request = new CreateAcmeProductRequest
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
            var customFieldResult = Helper.ExtractCustomFieldForInsertOperation(request.CustomFields, referenceCustomFieldListFromDatabase);


            // 2 get id of created acmeProduct
            acmeProductId = await SendAsync(request);

            // 3 Update acmeProduct
            var acmeProductUpdate = new UpdateAcmeProductRequest
            {
                Id = acmeProductId,
                Name = GetUniqueName(),
                //Desc = "12345678901",
                //Image = "Capture.Image",
                CustomFields = new Dictionary<string, string>
                 {
                      { "abc_code", "execuads" },
                      { "ved_code", "vc" },
                      { "extended_family_name", "cv" },
                      { "extended_code", "sd" },
                      { "version_value", "sd" },
                      { "version_data", "fd" },
                      { "decode_version", "sd" },
                      { "decode_value", "ad" },
                      { "extended_version", "od" }
                     // Add more key-value pairs as needed
                 }
            };
            // 4 Wait for to update acmeProduct
            await SendAsync(acmeProductUpdate);

            // 5 Find id In database
            dataAccess = GetAcmeDataAccess();
            var acmeProduct = await dataAccess.GetAcmeProductById(acmeProductId);

            //Extract Custom Field For Update Operation 
            var updatecustomFieldResult = Helper.ExtractCustomFieldForUpdateOperation(request.CustomFields, acmeProduct.CustomFields);

            // 6 Assertion
            // 6.1 Testing update id not be null
            acmeProduct.Id.Should().Be(acmeProductUpdate.Id);
            acmeProduct.Name.Should().Be(acmeProductUpdate.Name);
            //acmeProduct.Desc.Should().Be(acmeProductUpdate.Desc);
            //acmeProduct.Image.Should().Be(acmeProductUpdate.Image);

            // 5. Test CustomFields
            // field_value is dynamic; System.Text.Json deserializes it as JsonElement from DB.
            acmeProduct.CustomFields.Should().NotBeNull();
            acmeProduct.CustomFields
                .Select(f => new { f.field_name, value = f.field_value?.ToString() })
                .Should().BeEquivalentTo(
                    customFieldResult.Select(f => new { f.field_name, value = f.field_value?.ToString() }));
        }
    }
}
