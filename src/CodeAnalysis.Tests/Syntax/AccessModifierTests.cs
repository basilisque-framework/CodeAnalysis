/*
   Copyright 2023 Alexander Stärk

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/
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
