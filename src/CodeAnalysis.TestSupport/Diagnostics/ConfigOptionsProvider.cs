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
