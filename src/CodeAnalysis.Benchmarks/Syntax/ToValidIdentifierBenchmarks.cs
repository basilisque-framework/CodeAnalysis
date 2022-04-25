using BenchmarkDotNet.Attributes;

namespace Basilisque.CodeAnalysis.Benchmarks.Syntax
{
    [MemoryDiagnoser]
    public class ToValidIdentifierBenchmarks
    {
        [Params(
            "1Basilisque.CodeAnalysis.Benchmarks.Syntax"
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
