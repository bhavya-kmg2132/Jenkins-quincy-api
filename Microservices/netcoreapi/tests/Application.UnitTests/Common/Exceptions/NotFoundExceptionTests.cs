using System;
using Application.Common.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.Common.Exceptions
{
    public class NotFoundExceptionTests
    {
        [Test]
        public void DefaultConstructor_CreatesException()
        {
            var ex = new NotFoundException();
            ex.Should().NotBeNull();
        }

        [Test]
        public void MessageConstructor_SetsMessage()
        {
            var ex = new NotFoundException("item not found");
            ex.Message.Should().Be("item not found");
        }

        [Test]
        public void MessageAndInnerExceptionConstructor_SetsBoth()
        {
            var inner = new InvalidOperationException("inner");
            var ex = new NotFoundException("outer", inner);
            ex.Message.Should().Be("outer");
            ex.InnerException.Should().Be(inner);
        }

        [Test]
        public void NameAndKeyConstructor_FormatsMessage()
        {
            var ex = new NotFoundException("TodoItem", "abc-123");
            ex.Message.Should().Be("Entity \"TodoItem\" (abc-123) was not found.");
        }

        [Test]
        public void NameAndKeyConstructor_WithIntKey_FormatsMessage()
        {
            var ex = new NotFoundException("Order", 42);
            ex.Message.Should().Be("Entity \"Order\" (42) was not found.");
        }

        [Test]
        public void IsAssignableFrom_Exception()
        {
            var ex = new NotFoundException();
            ex.Should().BeAssignableTo<Exception>();
        }
    }
}
