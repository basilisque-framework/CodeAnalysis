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
    public class StringExtensions_ToValidNamespaceTests
    {
        private readonly Func<string?, string?> _toValidNamespace;

        public StringExtensions_ToValidNamespaceTests()
            : this(StringExtensions.ToValidNamespace)
        { }

        public StringExtensions_ToValidNamespaceTests(Func<string?, string?> testFunc)
        {
            _toValidNamespace = testFunc;
        }

        [TestMethod]
        public void Ensure_CanBeCalledAsExtensionMethod()
        {
            var result = "".ToValidNamespace();

            Assert.IsNull(result);
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "null")]
        [DataRow("", DisplayName = "empty string")]
        public void EmptySource_ReturnsNull(string? s)
        {
            var result = _toValidNamespace(s);

            Assert.IsNull(result);
        }

        [DataTestMethod]
        [DataRow("  ", "__", DisplayName = "2 spaces")]
        [DataRow("    ", "____", DisplayName = "4 spaces")]
        [DataRow("	 ", "__", DisplayName = "1 tab & 1 space")]
        public void Whitepace_ReturnsUnderscores(string? s, string expectedResult)
        {
            var result = _toValidNamespace(s);

            Assert.AreEqual(expectedResult, result);
        }

        [DataTestMethod]
        [DataRow("Valid.Name.Space", "Valid.Name.Space", DisplayName = "Valid namespace is returned unchanged")]
        [DataRow("1Basilisque.CodeAnalysis.Benchmarks.Syntax", "_1Basilisque.CodeAnalysis.Benchmarks.Syntax", DisplayName = "Letter at start should be prefixed")]
        [DataRow("%1Basilisque.CodeAnalysis.Benchmarks.Syntax", "_1Basilisque.CodeAnalysis.Benchmarks.Syntax", DisplayName = "Invalid start should replace first character")]
        [DataRow("nsContaining.namespace.keyword", "nsContaining.@namespace.keyword", DisplayName = "Keyword in the middle is prefixed with @")]
        [DataRow("namespace.key.word", "@namespace.key.word", DisplayName = "Keyword at the start is prefixed with @")]
        [DataRow("namespace.class.whatever", "@namespace.@class.whatever", DisplayName = "Multiple keywords are prefixed with @")]
        [DataRow("_starting.with.underscore", "_starting.with.underscore", DisplayName = "starting with underscore is valid")]
        [DataRow("-starting.with.dash", "_starting.with.dash", DisplayName = "starting with dash gets replaced")]
        [DataRow("NoDots", "NoDots", DisplayName = "namespace without dots")]
        [DataRow("1NoDots", "_1NoDots", DisplayName = "invalid namespace without dots")]
        [DataRow("class", "@class", DisplayName = "single reserved keyword 'class'")]
        [DataRow("namespace", "@namespace", DisplayName = "single reserved keyword 'namespace'")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S4144:Methods should not have identical implementations", Justification = "Name of method provides relevant test context")]
        public void SourceString_ReturnsCorrectNamespace(string? s, string expectedResult)
        {
            var result = _toValidNamespace(s);

            Assert.AreEqual(expectedResult, result);
        }
    }
}
