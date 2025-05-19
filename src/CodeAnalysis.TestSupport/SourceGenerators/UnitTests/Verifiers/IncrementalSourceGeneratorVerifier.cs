/*
   Copyright 2023-2025 Alexander Stärk

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
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using System.Collections.Immutable;
using System.Reflection;

namespace Basilisque.CodeAnalysis.TestSupport.SourceGenerators.UnitTests.Verifiers
{
    //The issue that brought me in the right direction: https://github.com/dotnet/roslyn-sdk/issues/989

    /// <summary>
    /// Helper class to support verifying incremental source generators
    /// </summary>
    /// <typeparam name="TSourceGenerator">The incremental source generator to be verified</typeparam>
    public class IncrementalSourceGeneratorVerifier<TSourceGenerator> : IncrementalSourceGeneratorVerifier<TSourceGenerator, DefaultVerifier>
        where TSourceGenerator : IIncrementalGenerator, new()
    { }

    /// <summary>
    /// Helper class to support verifying incremental source generators
    /// </summary>
    /// <typeparam name="TSourceGenerator">The incremental source generator to be verified</typeparam>
    /// <typeparam name="TVerifier">The verifier that verifies the source generator</typeparam>
    public class IncrementalSourceGeneratorVerifier<TSourceGenerator, TVerifier> : Microsoft.CodeAnalysis.CSharp.Testing.CSharpSourceGeneratorTest<TSourceGenerator, TVerifier>
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
        /// Workaround for https://github.com/dotnet/roslyn-sdk/pull/1204 in V1.1.2 until the fix is released in an official version.
        /// After this is released, this code can be removed. The method GetAnalyzerOptions should do this job then.
        /// </summary>
        protected override Project ApplyCompilationOptions(Project project)
        {
            var result = base.ApplyCompilationOptions(project);

            if (_globalOptions is null)
                return result;

            var testStateProperty = typeof(Project).GetProperty("State", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (testStateProperty is null)
                return result;

            var testState = testStateProperty.GetValue(result);
            if (testState is null)
                return result;

            var analyzerOptionsField = testState.GetType().GetField("_lazyAnalyzerOptions", BindingFlags.Instance | BindingFlags.NonPublic);
            if (analyzerOptionsField is null)
                return result;

            var baseOptions = base.GetAnalyzerOptions(project);

            var analyzerOptions = new AnalyzerOptions(ImmutableArray<AdditionalText>.Empty, new Diagnostics.ConfigOptionsProvider(baseOptions.AnalyzerConfigOptionsProvider, _globalOptions));

            analyzerOptionsField.SetValue(testState, analyzerOptions);

            return result;
        }

        /// <summary>
        /// Gets the effective analyzer options for a project. Returns the default value extended with the provided global options.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>The effective <see cref="AnalyzerOptions"/> for the project.</returns>
        protected override AnalyzerOptions GetAnalyzerOptions(Project project)
        {
            var baseOptions = base.GetAnalyzerOptions(project);

            if (_globalOptions is null)
                return baseOptions;

            return new AnalyzerOptions(ImmutableArray<AdditionalText>.Empty, new Diagnostics.ConfigOptionsProvider(baseOptions.AnalyzerConfigOptionsProvider, _globalOptions));
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

            if (_diagnosticOptions?.Count > 0)
            {
                var opt = compilationOptions.SpecificDiagnosticOptions;
                foreach (var item in _diagnosticOptions)
                {
                    opt = opt.SetItem(item.Key, item.Value);
                }

                compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(opt);
            }

            return compilationOptions;
        }
    }
}
