using Microsoft.CodeAnalysis.Diagnostics;

namespace Basilisque.CodeAnalysis.TestSupport.Diagnostics
{
    /// <summary>
    /// Provide options from an analyzer config file keyed on a source file.
    /// (Extends an <see cref="AnalyzerConfigOptionsProvider"/> with additional options.)
    /// </summary>
    public class ConfigOptionsProvider : AnalyzerConfigOptionsProvider
    {
        private readonly AnalyzerConfigOptionsProvider _analyzerConfigOptionsProvider;

        /// <summary>
        /// Creates a new <see cref="ConfigOptionsProvider"/>
        /// </summary>
        /// <param name="analyzerConfigOptionsProvider">The <see cref="AnalyzerConfigOptionsProvider"/> that is being extended</param>
        /// <param name="globalOptions">The additional global options</param>
        public ConfigOptionsProvider(AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider, Dictionary<string, string> globalOptions)
        {
            if (analyzerConfigOptionsProvider == null)
                throw new ArgumentNullException(nameof(analyzerConfigOptionsProvider));

            if (globalOptions == null)
                throw new ArgumentNullException(nameof(globalOptions));

            _analyzerConfigOptionsProvider = analyzerConfigOptionsProvider;
            this.GlobalOptions = new ConfigOptions(_analyzerConfigOptionsProvider.GlobalOptions, globalOptions);
        }

        /// <summary>
        /// Gets global options that do not apply to any specific file
        /// </summary>
        public override AnalyzerConfigOptions GlobalOptions { get; }

        /// <summary>
        /// Get options for a given <paramref name="tree"/>.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
        {
            return _analyzerConfigOptionsProvider.GetOptions(tree);
        }

        /// <summary>
        /// Get options for a given <see cref="AdditionalText"/>
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
        {
            return _analyzerConfigOptionsProvider.GetOptions(textFile);
        }
    }
}
