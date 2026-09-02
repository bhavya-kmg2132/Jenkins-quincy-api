using Application.Common.Rules.Engine.Execution;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.Common.Rules
{
    public class RuleExecutionResultTests
    {
        [Test]
        public void NewResult_IsValid_WhenNoErrors()
        {
            var result = new RuleExecutionResult();

            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Test]
        public void Add_SingleError_IsInvalid()
        {
            var result = new RuleExecutionResult();
            result.Add(new Error("field1", "some error"));

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
        }

        [Test]
        public void Add_SingleError_SetsMessageToFirstError()
        {
            var result = new RuleExecutionResult();
            result.Add(new Error("field1", "first error"));

            result.Message.Should().Be("first error");
        }

        [Test]
        public void Add_MultipleErrors_MessageIsFirstError()
        {
            var result = new RuleExecutionResult();
            result.Add(new Error("field1", "first error"));
            result.Add(new Error("field2", "second error"));

            result.Message.Should().Be("first error");
            result.Errors.Should().HaveCount(2);
        }

        [Test]
        public void Add_OtherResults_MergesErrors()
        {
            var result1 = new RuleExecutionResult();
            result1.Add(new Error("field1", "error A"));

            var result2 = new RuleExecutionResult();
            result2.Add(new Error("field2", "error B"));

            var combined = new RuleExecutionResult();
            combined.Add(result1, result2);

            combined.IsValid.Should().BeFalse();
            combined.Errors.Should().HaveCount(2);
        }

        [Test]
        public void Remove_Error_LeavesResultValid_WhenNoErrorsRemain()
        {
            var result = new RuleExecutionResult();
            var error = new Error("field1", "some error");
            result.Add(error);
            result.Remove(error);

            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Test]
        public void Add_EmptyOtherResults_RemainsValid()
        {
            var result = new RuleExecutionResult();
            result.Add(new RuleExecutionResult());

            result.IsValid.Should().BeTrue();
        }
    }
}
