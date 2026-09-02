using System.Threading;
using System.Threading.Tasks;
using Application.AcmeProduct.Commands.DeleteAcmeProduct;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using AcmeProductEntity = Domain.Entities.AcmeProduct;

namespace Application.UnitTests.Handlers
{
    public class DeleteAcmeOrderHandlerTests
    {
        private Mock<ILogger> _logger;
        private Mock<IConfiguration> _configuration;
        private Mock<ICurrentUserService> _currentUserService;


        [SetUp]
        public void SetUp()
        {
            _logger = new Mock<ILogger>();
            _configuration = new Mock<IConfiguration>();
            _currentUserService = new Mock<ICurrentUserService>();
            _currentUserService.Setup(x => x.UserId).Returns("user-1");
            _currentUserService.Setup(x => x.UserName).Returns("user-1");
            _currentUserService.Setup(x => x.CorrelationId).Returns("corr-1");
            _currentUserService.Setup(x => x.RequestId).Returns("req-1");
        }

        public class DeleteAcmeProductHandlerTests
        {
            private Mock<ILogger> _logger;
            private Mock<IConfiguration> _configuration;
            private Mock<IAcmeDataAccess> _dataAccess;
            private Mock<ICurrentUserService> _currentUserService;
            private DeleteAcmeProductRequestHandler _sut;

            [SetUp]
            public void SetUp()
            {
                _logger = new Mock<ILogger>();
                _configuration = new Mock<IConfiguration>();
                _dataAccess = new Mock<IAcmeDataAccess>();
                _currentUserService = new Mock<ICurrentUserService>();
                _currentUserService.Setup(x => x.UserId).Returns("user-1");
                _currentUserService.Setup(x => x.UserName).Returns("user-1");
                _currentUserService.Setup(x => x.CorrelationId).Returns("corr-1");
                _currentUserService.Setup(x => x.RequestId).Returns("req-1");

                _sut = new DeleteAcmeProductRequestHandler(
                    _configuration.Object, _logger.Object,
                    _dataAccess.Object, _currentUserService.Object);
            }

            [Test]
            public async Task Handle_ExistingProduct_DeletesAndReturnsUnit()
            {
                var product = new AcmeProductEntity { Id = "prod-1", Name = "Widget" };
                _dataAccess.Setup(x => x.GetAcmeProductById("prod-1")).ReturnsAsync(product);
                _dataAccess.Setup(x => x.Delete(product)).ReturnsAsync(1);

                var result = await _sut.Handle(new DeleteAcmeProductRequest { Id = "prod-1" }, CancellationToken.None);

                result.Should().Be(Unit.Value);
                _dataAccess.Verify(x => x.Delete(product), Times.Once);
            }

            [Test]
            public async Task Handle_ExistingProduct_SetsAuditFields()
            {
                var product = new AcmeProductEntity { Id = "prod-1" };
                _dataAccess.Setup(x => x.GetAcmeProductById("prod-1")).ReturnsAsync(product);
                _dataAccess.Setup(x => x.Delete(It.IsAny<AcmeProductEntity>())).ReturnsAsync(1);

                await _sut.Handle(new DeleteAcmeProductRequest { Id = "prod-1" }, CancellationToken.None);

                product.UpdatedBy.Should().Be("user-1");
                product.CorrelationId.Should().Be("corr-1");
                product.AuditableRequestId.Should().Be("req-1");
                product.AuditableRequestName.Should().Be(nameof(DeleteAcmeProductRequest));
            }

            [Test]
            public async Task Handle_NonExistentProduct_ThrowsNotFoundException()
            {
                _dataAccess.Setup(x => x.GetAcmeProductById("bad-id")).ReturnsAsync((AcmeProductEntity)null);

                var act = async () => await _sut.Handle(new DeleteAcmeProductRequest { Id = "bad-id" }, CancellationToken.None);

                await act.Should().ThrowAsync<NotFoundException>()
                    .WithMessage("*AcmeProduct*bad-id*");
            }
        }
    }
}
