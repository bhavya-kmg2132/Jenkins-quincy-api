using Application.Common.Rules.Generics;
using Domain.Common;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.Common.Rules
{
    public class IsCustomFieldDatatypeValidTests
    {
        private readonly IsCustomFieldDatatypeValid _sut = new();

        [Test]
        public void ValidBoolean_ReturnsTrue()
        {
            var field = new CustomField { field_type = "boolean", field_value = "true" };
            _sut.IsSatisfiedBy(field).Should().BeTrue();
        }

        [Test]
        public void InvalidBoolean_ReturnsFalse()
        {
            var field = new CustomField { field_type = "boolean", field_value = "notabool" };
            _sut.IsSatisfiedBy(field).Should().BeFalse();
        }

        [Test]
        public void ValidText_ReturnsTrue()
        {
            var field = new CustomField { field_type = "text", field_value = "hello" };
            _sut.IsSatisfiedBy(field).Should().BeTrue();
        }

        [Test]
        public void NullText_WhenNotRequired_ReturnsTrue()
        {
            var field = new CustomField { field_type = "text", field_value = null, field_is_required = false };
            _sut.IsSatisfiedBy(field).Should().BeTrue();
        }

        [Test]
        public void NullValue_WhenRequired_ReturnsFalse()
        {
            var field = new CustomField { field_type = "text", field_value = null, field_is_required = true };
            _sut.IsSatisfiedBy(field).Should().BeFalse();
        }

        [Test]
        public void ValidDatetime_ReturnsTrue()
        {
            var field = new CustomField { field_type = "datetime", field_value = "2024-01-15" };
            _sut.IsSatisfiedBy(field).Should().BeTrue();
        }

        [Test]
        public void InvalidDatetime_ReturnsFalse()
        {
            var field = new CustomField { field_type = "datetime", field_value = "not-a-date" };
            _sut.IsSatisfiedBy(field).Should().BeFalse();
        }

        [Test]
        public void ValidNumber_ReturnsTrue()
        {
            var field = new CustomField { field_type = "number", field_value = "42.5" };
            _sut.IsSatisfiedBy(field).Should().BeTrue();
        }

        [Test]
        public void InvalidNumber_ReturnsFalse()
        {
            var field = new CustomField { field_type = "number", field_value = "abc" };
            _sut.IsSatisfiedBy(field).Should().BeFalse();
        }

        [Test]
        public void UnknownFieldType_ReturnsFalse()
        {
            var field = new CustomField { field_type = "xml", field_value = "<root/>" };
            _sut.IsSatisfiedBy(field).Should().BeFalse();
        }
    }

    public class IsCustomFieldLengthValidTests
    {
        private readonly IsCustomFieldLengthValid _sut = new();

        [Test]
        public void TextWithinLimit_ReturnsTrue()
        {
            var field = new CustomField { field_type = "text", field_value = "hello", field_length = 10 };
            _sut.IsSatisfiedBy(field).Should().BeTrue();
        }

        [Test]
        public void TextExceedingLimit_ReturnsFalse()
        {
            var field = new CustomField { field_type = "text", field_value = "hello world", field_length = 5 };
            _sut.IsSatisfiedBy(field).Should().BeFalse();
        }

        [Test]
        public void TextAtExactLimit_ReturnsTrue()
        {
            var field = new CustomField { field_type = "text", field_value = "hello", field_length = 5 };
            _sut.IsSatisfiedBy(field).Should().BeTrue();
        }

        [Test]
        public void EmptyText_ReturnsTrue()
        {
            var field = new CustomField { field_type = "text", field_value = "", field_length = 5 };
            _sut.IsSatisfiedBy(field).Should().BeTrue();
        }

        [Test]
        public void NonTextField_IgnoresLength_ReturnsTrue()
        {
            var field = new CustomField { field_type = "number", field_value = "123456789", field_length = 3 };
            _sut.IsSatisfiedBy(field).Should().BeTrue();
        }
    }

    public class IsCustomFieldRegexValidTests
    {
        [Test]
        public void MatchingValue_ReturnsTrue()
        {
            var sut = new IsCustomFieldRegexValid(@"^\d{5}$");
            var field = new CustomField { field_value = "12345" };
            sut.IsSatisfiedBy(field).Should().BeTrue();
        }

        [Test]
        public void NonMatchingValue_ReturnsFalse()
        {
            var sut = new IsCustomFieldRegexValid(@"^\d{5}$");
            var field = new CustomField { field_value = "abc" };
            sut.IsSatisfiedBy(field).Should().BeFalse();
        }

        [Test]
        public void EmailPattern_ValidEmail_ReturnsTrue()
        {
            var sut = new IsCustomFieldRegexValid(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            var field = new CustomField { field_value = "user@example.com" };
            sut.IsSatisfiedBy(field).Should().BeTrue();
        }

        [Test]
        public void EmailPattern_InvalidEmail_ReturnsFalse()
        {
            var sut = new IsCustomFieldRegexValid(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            var field = new CustomField { field_value = "notanemail" };
            sut.IsSatisfiedBy(field).Should().BeFalse();
        }
    }
}
