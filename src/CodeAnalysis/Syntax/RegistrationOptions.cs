﻿/*
   Copyright 2023-2024 Alexander Stärk

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

namespace Basilisque.CodeAnalysis.Syntax
{
    /// <summary>
    /// Options that are provided with a registration callback.
    /// </summary>
    public struct RegistrationOptions
    {
        /// <summary>
        /// The <see cref="SourceProductionContext"/> that the source code is added to
        /// </summary>
        public SourceProductionContext Context { get; }

        /// <summary>
        /// The language of the current compilation
        /// </summary>
        public Language Language { get; }

        /// <summary>
        /// The version of the <see cref="Language"/> that is used for the current compilation
        /// </summary>
        public int LanguageVersion { get; }

        /// <summary>
        /// The <see cref="Microsoft.CodeAnalysis.NullableContextOptions"/> of the current compilation
        /// </summary>
        public NullableContextOptions NullableContextOptions { get; }

        /// <summary>
        /// The <see cref="System.Reflection.AssemblyName"/> of the assembly that is calling the register method
        /// </summary>
        public System.Reflection.AssemblyName CallingAssembly { get; }

        /// <summary>
        /// Creates new <see cref="RegistrationOptions"/>
        /// </summary>
        /// <param name="context">The <see cref="SourceProductionContext"/> that the source generation is done for</param>
        /// <param name="language">The language of the current compilation</param>
        /// <param name="languageVersion">The version of the <paramref name="language"/> that is used for the current compilation</param>
        /// <param name="nullableContextOptions">The <see cref="Microsoft.CodeAnalysis.NullableContextOptions"/> of the current compilation</param>
        /// <param name="callingAssembly">The <see cref="System.Reflection.AssemblyName"/> of the assembly that is calling the register method</param>
        internal RegistrationOptions(
            SourceProductionContext context,
            Language language,
            int languageVersion,
            NullableContextOptions nullableContextOptions,
            System.Reflection.AssemblyName callingAssembly)
        {
            Context = context;
            Language = language;
            LanguageVersion = languageVersion;
            NullableContextOptions = nullableContextOptions;
            CallingAssembly = callingAssembly;
        }

        /// <summary>
        /// Creates a new <see cref="CompilationInfo"/> with the given options
        /// </summary>
        /// <param name="compilationName">The name of the current compilation or rather the 'source output' (e.g. used as the hintName for the <see cref="SourceProductionContext"/>)</param>
        /// <param name="targetNamespace">The containing namespace of all child syntax nodes (when null, empty or whitespace, all children are in the global namespace)</param>
        /// <returns></returns>
        public ContextAwareCompilationInfo CreateCompilationInfo(string compilationName, string? targetNamespace)
        {
            var compilationInfo = new ContextAwareCompilationInfo(Context, Language, compilationName, targetNamespace, null, null, CallingAssembly)
            {
                EnableNullableContext = NullableContextOptions.AnnotationsEnabled(),
                WriteFileScopedNamespace = getFileScopedNamespaceSupported()
            };

            return compilationInfo;
        }

        private bool getFileScopedNamespaceSupported()
        {
            switch (Language)
            {
                case Language.VisualBasic:
                    return false;
                case Language.CSharp:
                default:
                    return (Microsoft.CodeAnalysis.CSharp.LanguageVersion)LanguageVersion >= Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp10;
            }
        }

        /// <summary>
        /// Returns the hint name that can be used to add source text to a <see cref="SourceProductionContext"/>.
        /// Supplements the provided <paramref name="compilationName"/> with a matching file ending.
        /// </summary>
        /// <param name="compilationName">The name of the compilation that is supplemented with a matching file ending</param>
        /// <returns>The hint name as string</returns>
        public string GetHintName(string compilationName)
        {
            return CompilationInfo.GetHintName(compilationName, Language);
        }
    }
}
