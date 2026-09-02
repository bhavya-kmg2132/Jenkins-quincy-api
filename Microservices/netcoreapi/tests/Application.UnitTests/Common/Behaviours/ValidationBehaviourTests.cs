using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Behaviours;
using FluentAssertions;
using FluentValidation;
using MediatR;
using NUnit.Framework;
using ValidationException = Application.Common.Exceptions.ValidationException;

namespace Application.UnitTests.Common.Behaviours
{
    public class ValidationBehaviourTests
    {
        private record TestRequest(string Value) : IRequest<string>;

        private class AlwaysValidValidator : AbstractValidator<TestRequest>
        {
            public AlwaysValidValidator()
            {
                RuleFor(x => x.Value).NotEmpty();
            }
        }

        private class AlwaysFailValidator : AbstractValidator<TestRequest>
        {
            public AlwaysFailValidator()
            {
                RuleFor(x => x.Value).Must(_ => false).WithMessage("always fails");
            }
        }

        [Test]
        public async Task Handle_NoValidators_CallsNext()
        {
            var behaviour = new ValidationBehaviour<TestRequest, string>(new List<IValidator<TestRequest>>());
            var nextCalled = false;

            await behaviour.Handle(
                new TestRequest("hello"),
                ct => { nextCalled = true; return Task.FromResult("ok"); },
                CancellationToken.None);

            nextCalled.Should().BeTrue();
        }

        [Test]
        public async Task Handle_PassingValidator_CallsNext()
        {
            var behaviour = new ValidationBehaviour<TestRequest, string>(
                new List<IValidator<TestRequest>> { new AlwaysValidValidator() });
            var nextCalled = false;

            await behaviour.Handle(
                new TestRequest("hello"),
                ct => { nextCalled = true; return Task.FromResult("ok"); },
                CancellationToken.None);

            nextCalled.Should().BeTrue();
        }

        [Test]
        public async Task Handle_FailingValidator_ThrowsValidationException()
        {
            var behaviour = new ValidationBehaviour<TestRequest, string>(
                new List<IValidator<TestRequest>> { new AlwaysFailValidator() });

            var act = async () => await behaviour.Handle(
                new TestRequest("hello"),
                ct => Task.FromResult("ok"),
                CancellationToken.None);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Handle_FailingValidator_ExceptionContainsErrors()
        {
            var behaviour = new ValidationBehaviour<TestRequest, string>(
                new List<IValidator<TestRequest>> { new AlwaysFailValidator() });

            var exception = default(ValidationException);
            try
            {
                await behaviour.Handle(
                    new TestRequest("hello"),
                    ct => Task.FromResult("ok"),
                    CancellationToken.None);
            }
            catch (ValidationException ex)
            {
                exception = ex;
            }

            exception.Should().NotBeNull();
            exception!.Errors.Should().ContainKey("Value");
        }
    }
}
