using Basilisque.CodeAnalysis.Syntax;

namespace Basilisque.CodeAnalysis.Tests.Syntax
{
    [TestClass]
    public class AccessModifierTests
    {
        [DataTestMethod]
        [DataRow(AccessModifier.Public, "public")]
        [DataRow(AccessModifier.Internal, "internal")]
        [DataRow(AccessModifier.Protected, "protected")]
        [DataRow(AccessModifier.Private, "private")]
        [DataRow(AccessModifier.ProtectedInternal, "protected internal")]
        [DataRow(AccessModifier.PrivateProtected, "private protected")]
        public void ToKeywordString_ReturnsValidString(AccessModifier accessModifier, string expectedResult)
        {
            string keyword = accessModifier.ToKeywordString();

            Assert.AreEqual(expectedResult, keyword);
        }

        [TestMethod]
        public void ToKeywordString_CanBeCalledAsNormalFunction()
        {
            string keyword = AccessModifierExtensions.ToKeywordString(AccessModifier.Internal);

            Assert.AreEqual("internal", keyword);
        }

        [TestMethod]
        public void ToKeywordString_ThrowsForInvalidValue()
        {
            Assert.ThrowsException<NotSupportedException>(() =>
            {
                AccessModifierExtensions.ToKeywordString((AccessModifier)20);
            });
        }
    }
}
