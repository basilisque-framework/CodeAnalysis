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
using BenchmarkDotNet.Attributes;

namespace Basilisque.CodeAnalysis.Benchmarks.Syntax
{
    [MemoryDiagnoser]
    public class ToValidIdentifierBenchmarks
    {
        [Params(
            "Basilisque.CodeAnalysis.Benchmarks.Syntax"
            )]
        public string? Source { get; set; }

        [Benchmark]
        public void ToValidNamespace_WithStringBuilder() => StringExtensionTestMethods.ToValidIdentifier_BasicImplementationStringBuilder(Source, GetIsIdentifierPartCharacterFunc());

        [Benchmark]
        public void ToValidNamespace_WithCharArray() => StringExtensionTestMethods.ToValidIdentifier_WithCharArray(Source, GetIsIdentifierPartCharacterFunc());

        [Benchmark]
        public void ToValidNamespace_NoIsValidIdentifierCall() => StringExtensionTestMethods.ToValidIdentifier_WithoutIsValidIdentifierCall(Source, GetIsIdentifierPartCharacterFunc());

        [Benchmark]
        public void ToValidNamespace_RealImplementation() => CodeAnalysis.Syntax.Benchmark.StringExtensions.ToValidNamespace(Source);

        public static Func<char, bool> GetIsIdentifierPartCharacterFunc()
        {
            return c => c == '.' || Microsoft.CodeAnalysis.CSharp.SyntaxFacts.IsIdentifierPartCharacter(c);
        }
    }
}
