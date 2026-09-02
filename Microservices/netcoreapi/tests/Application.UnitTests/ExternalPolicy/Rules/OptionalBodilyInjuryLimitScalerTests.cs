using System.Text.Json;
using Application.ExternalPolicy.Rules;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.ExternalPolicy.Rules
{
    public class OptionalBodilyInjuryLimitScalerTests
    {
        [Test]
        public void ScaleUpFromDb2_MultipliesNumericDb2CodeByOneThousand()
        {
            var json = """[{"tableName":"DMBP130P","tableValue":[{"OBILMD":100,"BILLMD":"25/50"}]}]""";

            var result = OptionalBodilyInjuryLimitScaler.ScaleUpFromDb2(json);

            using var doc = JsonDocument.Parse(result);
            var row = doc.RootElement[0].GetProperty("tableValue")[0];
            row.GetProperty("OBILMD").GetDecimal().Should().Be(100000m);
            row.GetProperty("BILLMD").GetString().Should().Be("25/50");
        }

        [TestCase("100", "100000")]
        [TestCase("300", "300000")]
        [TestCase("500", "500000")]
        [TestCase("1000", "1000000")]
        public void ScaleUpFromDb2_MultipliesStringDb2Code_KeepsStringShape(string input, string expected)
        {
            var json = $$"""[{"tableName":"DMBP130P","tableValue":[{"OBILMD":"{{input}}"}]}]""";

            var result = OptionalBodilyInjuryLimitScaler.ScaleUpFromDb2(json);

            using var doc = JsonDocument.Parse(result);
            doc.RootElement[0].GetProperty("tableValue")[0].GetProperty("OBILMD").GetString().Should().Be(expected);
        }

        [Test]
        public void ScaleUpFromDb2_LeavesNonCodeValueUnchanged()
        {
            var json = """[{"tableName":"DMBP130P","tableValue":[{"OBILMD":250}]}]""";

            var result = OptionalBodilyInjuryLimitScaler.ScaleUpFromDb2(json);

            using var doc = JsonDocument.Parse(result);
            doc.RootElement[0].GetProperty("tableValue")[0].GetProperty("OBILMD").GetInt32().Should().Be(250);
        }

        [Test]
        public void ScaleUpFromDb2_HandlesFlatObjectNotWrappedInTableArray()
        {
            var json = """{"policyData":[{"tableName":"DMBP130P","tableValue":[{"OBILMD":1000}]}]}""";

            var result = OptionalBodilyInjuryLimitScaler.ScaleUpFromDb2(json);

            using var doc = JsonDocument.Parse(result);
            doc.RootElement.GetProperty("policyData")[0].GetProperty("tableValue")[0]
                .GetProperty("OBILMD").GetDecimal().Should().Be(1000000m);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("not valid json")]
        public void ScaleUpFromDb2_ReturnsInputUnchanged_WhenNullEmptyOrInvalid(string input)
        {
            var result = OptionalBodilyInjuryLimitScaler.ScaleUpFromDb2(input);

            result.Should().Be(input);
        }
    }
}
