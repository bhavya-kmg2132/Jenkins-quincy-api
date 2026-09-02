using Domain.Exceptions;
using Domain.ValueObjects;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.DomainTests
{
    public class ColourTests
    {
        [Test]
        [TestCase("#FFFFFF")]
        [TestCase("#FF5733")]
        [TestCase("#FFC300")]
        [TestCase("#FFFF66")]
        [TestCase("#CCFF99 ")]
        [TestCase("#6666FF")]
        [TestCase("#9966CC")]
        [TestCase("#999999")]
        public void From_SupportedCode_ReturnsColour(string code)
        {
            var colour = Colour.From(code);
            colour.Code.Should().Be(code);
        }

        [Test]
        public void From_UnsupportedCode_ThrowsUnsupportedColourException()
        {
            var act = () => Colour.From("#000000");
            act.Should().Throw<UnsupportedColourException>();
        }

        [Test]
        public void White_HasCorrectCode()
        {
            Colour.White.Code.Should().Be("#FFFFFF");
        }

        [Test]
        public void Red_HasCorrectCode()
        {
            Colour.Red.Code.Should().Be("#FF5733");
        }

        [Test]
        public void Blue_HasCorrectCode()
        {
            Colour.Blue.Code.Should().Be("#6666FF");
        }

        [Test]
        public void ToString_ReturnsCode()
        {
            Colour.White.ToString().Should().Be("#FFFFFF");
        }

        [Test]
        public void ImplicitStringConversion_ReturnsCode()
        {
            string code = Colour.Red;
            code.Should().Be("#FF5733");
        }

        [Test]
        public void ExplicitColourConversion_FromValidCode_ReturnsColour()
        {
            var colour = (Colour)"#FFFFFF";
            colour.Code.Should().Be("#FFFFFF");
        }

        [Test]
        public void ExplicitColourConversion_FromInvalidCode_Throws()
        {
            var act = () => { var _ = (Colour)"#BADBAD"; };
            act.Should().Throw<UnsupportedColourException>();
        }

        [Test]
        public void TwoWhiteColours_AreEqual()
        {
            Colour.White.Should().Be(Colour.White);
        }

        [Test]
        public void WhiteAndRed_AreNotEqual()
        {
            Colour.White.Should().NotBe(Colour.Red);
        }

        [Test]
        public void DefaultConstructor_CreatesColourWithNullCode()
        {
            var colour = new Colour();
            colour.Code.Should().BeNull();
        }
    }
}
