using Basilisque.CodeAnalysis.Syntax;

namespace Basilisque.CodeAnalysis.Tests.Syntax
{
    [TestClass]
    public class StringExtensions_ToValidIdentifierTests
    {
        [TestMethod]
        public void Ensure_CanBeCalledAsNormalMethod()
        {
            var result = StringExtensions.ToValidIdentifier("");

            Assert.IsNull(result);
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "null")]
        [DataRow("", DisplayName = "empty string")]
        public void EmptySource_ReturnsNull(string? s)
        {
            var result = s.ToValidIdentifier();

            Assert.IsNull(result);
        }

        [DataTestMethod]
        [DataRow("  ", "__", DisplayName = "2 spaces")]
        [DataRow("    ", "____", DisplayName = "4 spaces")]
        [DataRow("	 ", "__", DisplayName = "1 tab & 1 space")]
        public void Whitepace_ReturnsUnderscores(string? s, string expectedResult)
        {
            var result = s.ToValidIdentifier();

            Assert.AreEqual(expectedResult, result);
        }

        [DataTestMethod]
        [DataRow("Name.With.Dots", "Name_With_Dots", DisplayName = "Dots are replaced (in contrast to ToValidNamespace())")]
        [DataRow("1MyClass", "_1MyClass", DisplayName = "Letter at start should be prefixed")]
        [DataRow("%1MyClass", "_1MyClass", DisplayName = "Invalid start should replace first character")]
        [DataRow("@MyClass", "_MyClass", DisplayName = "Starting with @ should be replaced")]
        [DataRow("My@Class", "My_Class", DisplayName = "@ in the middle should be replaced")]
        [DataRow("My$Class", "My_Class", DisplayName = "$ in the middle should be replaced")]
        [DataRow("class", "@class", DisplayName = "keyword should be prefixed with @")]
        [DataRow("Name.With.Dots.And.class.keyword", "Name_With_Dots_And_class_keyword", DisplayName = "keyword as part of a longer string should be fine")]
        public void SourceString_ReturnsCorrectNamespace(string? s, string expectedResult)
        {
            var result = s.ToValidIdentifier();

            Assert.AreEqual(expectedResult, result);
        }
    }
}
