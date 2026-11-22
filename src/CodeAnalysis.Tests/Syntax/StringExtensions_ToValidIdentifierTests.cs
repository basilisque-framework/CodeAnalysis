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
    public class StringExtensions_ToValidIdentifierTests
    {
        [Test]
        public async Task Ensure_CanBeCalledAsNormalMethod()
        {
            var result = StringExtensions.ToValidIdentifier("");

            await Assert.That(result).IsNull();
        }

        [Test]
        [Arguments(null, DisplayName = "null")]
        [Arguments("", DisplayName = "empty string")]
        public async Task EmptySource_ReturnsNull(string? s)
        {
            var result = s.ToValidIdentifier();

            await Assert.That(result).IsNull();
        }

        [Test]
        [Arguments("  ", "__", DisplayName = "2 spaces")]
        [Arguments("    ", "____", DisplayName = "4 spaces")]
        [Arguments("	 ", "__", DisplayName = "1 tab & 1 space")]
        public async Task Whitepace_ReturnsUnderscores(string? s, string expectedResult)
        {
            var result = s.ToValidIdentifier();

            await Assert.That(result).IsEqualTo(expectedResult);
        }

        [Test]
        [Arguments("Name.With.Dots", "Name_With_Dots", DisplayName = "Dots are replaced (in contrast to ToValidNamespace())")]
        [Arguments("1MyClass", "_1MyClass", DisplayName = "Letter at start should be prefixed")]
        [Arguments("%1MyClass", "_1MyClass", DisplayName = "Invalid start should replace first character")]
        [Arguments("@MyClass", "_MyClass", DisplayName = "Starting with @ should be replaced")]
        [Arguments("My@Class", "My_Class", DisplayName = "@ in the middle should be replaced")]
        [Arguments("My$Class", "My_Class", DisplayName = "$ in the middle should be replaced")]
        [Arguments("class", "@class", DisplayName = "keyword should be prefixed with @")]
        [Arguments("Name.With.Dots.And.class.keyword", "Name_With_Dots_And_class_keyword", DisplayName = "keyword as part of a longer string should be fine")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S4144:Methods should not have identical implementations", Justification = "Name of method provides relevant test context")]
        public async Task SourceString_ReturnsCorrectNamespace(string? s, string expectedResult)
        {
            var result = s.ToValidIdentifier();

            await Assert.That(result).IsEqualTo(expectedResult);
        }
    }
}
