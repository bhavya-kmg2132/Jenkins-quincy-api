using System;
using Application.Common.Rules.Engine.Execution;
using Application.Common.Rules.Engine.Interfaces.Execution;
using Application.Common.Rules.Engine.Interfaces.Specification;
using Application.Common.Rules.Execution;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Application.UnitTests.Common.Rules
{
    public class ExecutorTests
    {
        // Concrete subclass to expose the protected Add method for testing
        private class TestExecutor<TEntity> : Executor<TEntity> where TEntity : class
        {
            public void AddRule(string name, IRule<TEntity> rule) => Add(name, rule);
        }

        [Test]
        public void Execute_NoRules_ReturnsValidResult()
        {
            var executor = new TestExecutor<string>();

            var result = executor.Execute("any");

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void Execute_AllRulesPass_ReturnsValidResult()
        {
            var executor = new TestExecutor<string>();
            var passingSpec = new Mock<ISpecification<string>>();
            passingSpec.Setup(s => s.IsSatisfiedBy(It.IsAny<string>())).Returns(true);

            executor.AddRule("rule1", new Rule<string>(passingSpec.Object, "error1"));

            var result = executor.Execute("value");

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void Execute_FailingRule_ReturnsInvalidResult()
        {
            var executor = new TestExecutor<string>();
            var failingSpec = new Mock<ISpecification<string>>();
            failingSpec.Setup(s => s.IsSatisfiedBy(It.IsAny<string>())).Returns(false);

            executor.AddRule("rule1", new Rule<string>(failingSpec.Object, "field is invalid"));

            var result = executor.Execute("value");

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.Message == "field is invalid");
        }

        [Test]
        public void Execute_MultipleFailingRules_ReturnsAllErrors()
        {
            var executor = new TestExecutor<string>();
            var failingSpec = new Mock<ISpecification<string>>();
            failingSpec.Setup(s => s.IsSatisfiedBy(It.IsAny<string>())).Returns(false);

            executor.AddRule("rule1", new Rule<string>(failingSpec.Object, "error A"));
            executor.AddRule("rule2", new Rule<string>(failingSpec.Object, "error B"));

            var result = executor.Execute("value");

            result.Errors.Should().HaveCount(2);
        }

        [Test]
        public void Execute_WithThrowError_ThrowsApplicationException_WhenInvalid()
        {
            var executor = new TestExecutor<string>();
            var failingSpec = new Mock<ISpecification<string>>();
            failingSpec.Setup(s => s.IsSatisfiedBy(It.IsAny<string>())).Returns(false);

            executor.AddRule("rule1", new Rule<string>(failingSpec.Object, "must not be empty"));

            Action act = () => executor.Execute("value", throwError: true);

            act.Should().Throw<ApplicationException>().WithMessage("*must not be empty*");
        }

        [Test]
        public void Execute_WithThrowError_DoesNotThrow_WhenValid()
        {
            var executor = new TestExecutor<string>();
            var passingSpec = new Mock<ISpecification<string>>();
            passingSpec.Setup(s => s.IsSatisfiedBy(It.IsAny<string>())).Returns(true);

            executor.AddRule("rule1", new Rule<string>(passingSpec.Object, "error"));

            Action act = () => executor.Execute("value", throwError: true);

            act.Should().NotThrow();
        }
    }
}
