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
