using System.Threading.Tasks;
using Application.Acme.Commands.Create;
using Application.AcmeProduct.Commands.CreateAcmeProduct;
using Application.Common.Interfaces;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;

namespace Application.UnitTests.Validators
{
    public class CreateAcmeProductValidatorTests
    {
        private Mock<IAcmeDataAccess> _dataAccess;
        private CreateAcmeProductValidator _sut;

        [SetUp]
        public void SetUp()
        {
            _dataAccess = new Mock<IAcmeDataAccess>();
            _sut = new CreateAcmeProductValidator(_dataAccess.Object);
        }

        [Test]
        public async Task Name_Empty_FailsValidation()
        {
            var result = await _sut.TestValidateAsync(new CreateAcmeProductRequest { Name = "" });
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Test]
        public async Task Name_Null_FailsValidation()
        {
            var result = await _sut.TestValidateAsync(new CreateAcmeProductRequest { Name = null });
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Test]
        public async Task Name_Valid_PassesValidation()
        {
            var result = await _sut.TestValidateAsync(new CreateAcmeProductRequest { Name = "Widget Pro" });
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Test]
        public async Task Name_Exactly150Chars_PassesValidation()
        {
            var result = await _sut.TestValidateAsync(new CreateAcmeProductRequest { Name = new string('a', 150) });
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }
    }
}
