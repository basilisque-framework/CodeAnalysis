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

namespace Basilisque.CodeAnalysis.TestSupport.Diagnostics;

/// <summary>
/// Represents a collection of config options
/// (Extends <see cref="AnalyzerConfigOptions"/> with additional options.)
/// </summary>
public class ConfigOptions : AnalyzerConfigOptions
{
    private readonly AnalyzerConfigOptions _configOptions;
    private readonly Dictionary<string, string> _globalOptions;

    /// <summary>
    /// Creates new <see cref="ConfigOptions"/>
    /// </summary>
    /// <param name="configOptions">The <see cref="AnalyzerConfigOptions"/> that are being extended</param>
    /// <param name="globalOptions">A <see cref="Dictionary{TKey, TValue}"/> containing the additional global options</param>
    public ConfigOptions(AnalyzerConfigOptions configOptions, Dictionary<string, string> globalOptions)
    {
        _configOptions = configOptions ?? throw new ArgumentNullException(nameof(configOptions));
        _globalOptions = globalOptions ?? throw new ArgumentNullException(nameof(globalOptions));
    }

    /// <summary>
    /// Get an analyzer config value for the given key, using the <see cref="AnalyzerConfigOptions.KeyComparer"/>.
    /// </summary>
    public override bool TryGetValue(string key, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? value)
    {
        return _configOptions.TryGetValue(key, out value) || _globalOptions.TryGetValue(key, out value);
    }
}