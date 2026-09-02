using System;
using Application.Common.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.Common.Exceptions
{
    public class ForbiddenAccessExceptionTests
    {
        [Test]
        public void DefaultConstructor_CreatesException()
        {
            var ex = new ForbiddenAccessException();
            ex.Should().NotBeNull();
        }

        [Test]
        public void IsAssignableFrom_Exception()
        {
            var ex = new ForbiddenAccessException();
            ex.Should().BeAssignableTo<Exception>();
        }
    }
}
