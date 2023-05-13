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
namespace Basilisque.CodeAnalysis.Tests.Syntax
{
    // Ensure, that the benchmark implementations also fulfill all requirements.
    // Otherwise the performance comparison wouldn't be fair/valid.

    [TestClass]
    public class StringExtensions_ToValidNamespaceTests_Basic_Implementation : StringExtensions_ToValidNamespaceTests
    {
        public StringExtensions_ToValidNamespaceTests_Basic_Implementation()
            : base(s => Basilisque.CodeAnalysis.Benchmarks.Syntax.StringExtensionTestMethods.ToValidIdentifier_BasicImplementationStringBuilder(s, Benchmarks.Syntax.ToValidIdentifierBenchmarks.GetIsIdentifierPartCharacterFunc()))
        { }
    }

    [TestClass]
    public class StringExtensions_ToValidToValidNamespaceTests_WithCharArray : StringExtensions_ToValidNamespaceTests
    {
        public StringExtensions_ToValidToValidNamespaceTests_WithCharArray()
            : base(s => Basilisque.CodeAnalysis.Benchmarks.Syntax.StringExtensionTestMethods.ToValidIdentifier_WithCharArray(s, Benchmarks.Syntax.ToValidIdentifierBenchmarks.GetIsIdentifierPartCharacterFunc()))
        { }
    }

    [TestClass]
    public class StringExtensions_ToValidToValidNamespaceTests_MinimizedAPICalls : StringExtensions_ToValidNamespaceTests
    {
        public StringExtensions_ToValidToValidNamespaceTests_MinimizedAPICalls()
            : base(s => Basilisque.CodeAnalysis.Benchmarks.Syntax.StringExtensionTestMethods.ToValidIdentifier_WithoutIsValidIdentifierCall(s, Benchmarks.Syntax.ToValidIdentifierBenchmarks.GetIsIdentifierPartCharacterFunc()))
        { }
    }
}
