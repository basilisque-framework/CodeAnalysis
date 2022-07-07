using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using System.Collections.Immutable;

namespace Basilisque.CodeAnalysis.TestSupport.SourceGenerators.UnitTests.Verifiers
{
    //The issue that brought me in the right direction: https://github.com/dotnet/roslyn-sdk/issues/989

    /// <summary>
    /// Helper class to support verifying incremental source generators
    /// </summary>
    /// <typeparam name="TSourceGenerator">The incremental source generator to be verified</typeparam>
    /// <typeparam name="TVerifier">The verifier that verifies the source generator</typeparam>
    public class IncrementalSourceGeneratorVerifier<TSourceGenerator, TVerifier> : Microsoft.CodeAnalysis.CSharp.Testing.CSharpSourceGeneratorTest<EmptySourceGeneratorProvider, TVerifier>
        where TSourceGenerator : IIncrementalGenerator, new()
        where TVerifier : IVerifier, new()
    {
        private Dictionary<string, string>? _globalOptions;
        private Dictionary<string, ReportDiagnostic>? _diagnosticOptions;

        /// <summary>
        /// The target <see cref="LanguageVersion"/> that the source generator should generate code for
        /// </summary>
        public LanguageVersion LanguageVersion { get; set; } = LanguageVersion.Default;

        /// <summary>
        /// The global options that should be provided to the source generator
        /// </summary>
        public Dictionary<string, string> GlobalOptions
        {
            get
            {
                if (_globalOptions == null)
                    _globalOptions = new Dictionary<string, string>();

                return _globalOptions;
            }
        }

        /// <summary>
        /// The diagnostic options that should be added to the compilation
        /// </summary>
        public Dictionary<string, ReportDiagnostic> DiagnosticOptions
        {
            get
            {
                if (_diagnosticOptions == null)
                    _diagnosticOptions = new Dictionary<string, ReportDiagnostic>();

                return _diagnosticOptions;
            }
        }

        /// <summary>
        /// Creates a new <see cref="IncrementalSourceGeneratorVerifier{TSourceGenerator, TVerifier}"/>
        /// </summary>
        public IncrementalSourceGeneratorVerifier()
        {
            CompilerDiagnostics = CompilerDiagnostics.Warnings;
        }

        /// <summary>
        /// Creates and returns the source generator under test
        /// </summary>
        /// <returns>A new instance of <see cref="ISourceGenerator"/> as IEnumerable</returns>
        protected override IEnumerable<ISourceGenerator> GetSourceGenerators()
        {
            yield return new TSourceGenerator().AsSourceGenerator();
        }

        /// <summary>
        /// Creates and returns the generator driver
        /// </summary>
        /// <param name="project">The <see cref="Project"/> to be tested</param>
        /// <param name="sourceGenerators">The source generators to be tested</param>
        /// <returns></returns>
        protected override GeneratorDriver CreateGeneratorDriver(Project project, ImmutableArray<ISourceGenerator> sourceGenerators)
        {
            var driver = base.CreateGeneratorDriver(project, sourceGenerators);

            if (_globalOptions != null)
                driver = driver.WithUpdatedAnalyzerConfigOptions(new Diagnostics.ConfigOptionsProvider(project.AnalyzerOptions.AnalyzerConfigOptionsProvider, _globalOptions));

            return driver;
        }

        /// <summary>
        /// Creates and returns the parse options
        /// </summary>
        /// <returns>The <see cref="ParseOptions"/> that are provided to the source generators</returns>
        protected override ParseOptions CreateParseOptions()
        {
            var csharpParseOptions = (CSharpParseOptions)base.CreateParseOptions();

            csharpParseOptions = csharpParseOptions.WithLanguageVersion(LanguageVersion);

            return csharpParseOptions;
        }

        /// <summary>
        /// Creates and returns the compilation options
        /// </summary>
        /// <returns>The <see cref="CompilationOptions"/> that are used for the compilation</returns>
        protected override CompilationOptions CreateCompilationOptions()
        {
            var compilationOptions = (CSharpCompilationOptions)base.CreateCompilationOptions();

            if (DiagnosticOptions != null && DiagnosticOptions.Count > 0)
            {
                var opt = compilationOptions.SpecificDiagnosticOptions;
                foreach (var item in DiagnosticOptions)
                {
                    opt = opt.SetItem(item.Key, item.Value);
                }

                compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(opt);
            }

            return compilationOptions;
        }
    }
}
