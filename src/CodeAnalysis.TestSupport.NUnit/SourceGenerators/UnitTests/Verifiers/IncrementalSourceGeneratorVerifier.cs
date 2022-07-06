using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Basilisque.CodeAnalysis.TestSupport.NUnit.SourceGenerators.UnitTests.Verifiers
{
    /// <summary>
    /// Helper class to support verifying incremental source generators with MSTest
    /// </summary>
    /// <typeparam name="TSourceGenerator">The incremental source generator to be verified</typeparam>
    public class IncrementalSourceGeneratorVerifier<TSourceGenerator> : Basilisque.CodeAnalysis.TestSupport.SourceGenerators.UnitTests.Verifiers.IncrementalSourceGeneratorVerifier<TSourceGenerator, NUnitVerifier>
        where TSourceGenerator : IIncrementalGenerator, new()
    { }
}
