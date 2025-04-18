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
    /// Extension methods for <see cref="IncrementalGeneratorInitializationContext"/>
    /// </summary>
    public static class IncrementalGeneratorInitializationContextExtensions
    {
        /// <summary>
        /// Creates and returns an <see cref="IncrementalValueProvider{TValue}"/> with the options needed for a <see cref="CompilationInfo"/>
        /// </summary>
        /// <param name="context">The <see cref="IncrementalGeneratorInitializationContext"/> that is used to create the <see cref="IncrementalValueProvider{TValue}"/></param>
        /// <returns>A <see cref="IncrementalValueProvider{TValue}"/> containing the compilation information</returns>
        /// <exception cref="System.NotSupportedException">Throws <see cref="System.NotSupportedException"/> when the compilation uses a language that is not supported by <see cref="Language"/></exception>
        private static IncrementalValueProvider<((NullableContextOptions NullableContextOptions, Language Language) CompilationOptions, int LanguageVersion)> getCompilationInfoValuesProvider(this IncrementalGeneratorInitializationContext context)
        {
            var compilationOptionsProvider = context.CompilationProvider.Select(static (cmp, ct) => (cmp.Options.Language, cmp.Options.NullableContextOptions));

            var selectedOptionsProvider = compilationOptionsProvider.Select(static (cmp, ct) =>
            {
                Language l;
                if (cmp.Language == "C#")
                    l = Language.CSharp;
                //else if (cmp.Language == "Visual Basic")
                //    l = Language.VisualBasic;
                else
                    throw new System.NotSupportedException($"The language '{cmp.Language}' is not supported by this generator.");

                return (cmp.NullableContextOptions, l);
            });

            var languageVersionProvider = context.ParseOptionsProvider.Select((po, ct) =>
            {
                int langVersion;
                if (po is Microsoft.CodeAnalysis.CSharp.CSharpParseOptions cspo)
                    langVersion = (int)cspo.LanguageVersion;
                //else if (po is Microsoft.CodeAnalysis.VisualBasic.VisualBasicParseOptions vbpo)
                //    return vbpo.LanguageVersion;
                else
                    throw new System.NotSupportedException($"The parse options of type '{po.GetType().FullName}' are not supported by this generator.");

                return langVersion;
            });

            var combinedProvider = selectedOptionsProvider.Combine(languageVersionProvider);

            return combinedProvider;
        }

        /// <summary>
        /// Allows to produce source files and diagnostics that will be included in the users compilation
        /// </summary>
        /// <typeparam name="TSource">The type of the <see cref="IncrementalValueProvider{TSource}"/> that provides the input values</typeparam>
        /// <param name="incrementalGeneratorInitializationContext">The <see cref="IncrementalGeneratorInitializationContext"/> where the output is registered on</param>
        /// <param name="source">The <see cref="IncrementalValueProvider{TSource}"/> that provides the input values</param>
        /// <param name="action">The <see cref="Action{SourceProductionContext, TSource, RegistrationOptions}"/> that is invoked for every value in the value provider</param>
        public static void RegisterCompilationInfoOutput<TSource>(this IncrementalGeneratorInitializationContext incrementalGeneratorInitializationContext,
            IncrementalValueProvider<TSource> source,
            Action<SourceProductionContext, TSource, RegistrationOptions> action)
        {
            var callingAssembly = System.Reflection.Assembly.GetCallingAssembly().GetName();

            var compilationInfoValuesProvider = getCompilationInfoValuesProvider(incrementalGeneratorInitializationContext);

            var combinedSource = source.Combine(compilationInfoValuesProvider);

            incrementalGeneratorInitializationContext.RegisterSourceOutput(combinedSource, (spc, opt) =>
            {
                TSource src = opt.Left;
                ((NullableContextOptions nullableContextOptions, Language language), int languageVersion) = opt.Right;

                var registrationOptions = new RegistrationOptions(spc, language, languageVersion, nullableContextOptions, callingAssembly);

                action(spc, src, registrationOptions);
            });
        }

        /// <summary>
        /// Allows to produce source files and diagnostics that will be included in the users compilation
        /// </summary>
        /// <typeparam name="TSource">The type of the <see cref="IncrementalValuesProvider{TSource}"/> that provides the input values</typeparam>
        /// <param name="incrementalGeneratorInitializationContext">The <see cref="IncrementalGeneratorInitializationContext"/> where the output is registered on</param>
        /// <param name="source">The <see cref="IncrementalValuesProvider{TSource}"/> that provides the input values</param>
        /// <param name="action">The <see cref="Action{SourceProductionContext, TSource, RegistrationOptions}"/> that is invoked for every value in the value provider</param>
        public static void RegisterCompilationInfoOutput<TSource>(this IncrementalGeneratorInitializationContext incrementalGeneratorInitializationContext,
            IncrementalValuesProvider<TSource> source,
            Action<SourceProductionContext, TSource, RegistrationOptions> action)
        {
            var callingAssembly = System.Reflection.Assembly.GetCallingAssembly().GetName();

            var compilationInfoValuesProvider = getCompilationInfoValuesProvider(incrementalGeneratorInitializationContext);

            var combinedSource = source.Combine(compilationInfoValuesProvider);

            incrementalGeneratorInitializationContext.RegisterSourceOutput(combinedSource, (spc, opt) =>
            {
                TSource src = opt.Left;
                ((NullableContextOptions nullableContextOptions, Language language), int languageVersion) = opt.Right;

                var registrationOptions = new RegistrationOptions(spc, language, languageVersion, nullableContextOptions, callingAssembly);

                action(spc, src, registrationOptions);
            });
        }

        /// <summary>
        /// Allows to produce source files that will be included in the users compilation that have no semantic impact on user code from the point of view of code analysis.
        /// This allows a host such as the IDE, to chose not to run these outputs as a performance optimization. A host that produces executable code will always run these outputs.
        /// </summary>
        /// <typeparam name="TSource">The type of the <see cref="IncrementalValueProvider{TSource}"/> that provides the input values</typeparam>
        /// <param name="incrementalGeneratorInitializationContext">The <see cref="IncrementalGeneratorInitializationContext"/> where the output is registered on</param>
        /// <param name="source">The <see cref="IncrementalValueProvider{TSource}"/> that provides the input values</param>
        /// <param name="action">The <see cref="Action{SourceProductionContext, TSource, RegistrationOptions}"/> that is invoked for every value in the value provider</param>
        public static void RegisterImplementationCompilationInfoOutput<TSource>(this IncrementalGeneratorInitializationContext incrementalGeneratorInitializationContext,
            IncrementalValueProvider<TSource> source,
            Action<SourceProductionContext, TSource, RegistrationOptions> action)
        {
            var callingAssembly = System.Reflection.Assembly.GetCallingAssembly().GetName();

            var compilationInfoValuesProvider = getCompilationInfoValuesProvider(incrementalGeneratorInitializationContext);

            var combinedSource = source.Combine(compilationInfoValuesProvider);

            incrementalGeneratorInitializationContext.RegisterImplementationSourceOutput(combinedSource, (spc, opt) =>
            {
                TSource src = opt.Left;
                ((NullableContextOptions nullableContextOptions, Language language), int languageVersion) = opt.Right;

                var registrationOptions = new RegistrationOptions(spc, language, languageVersion, nullableContextOptions, callingAssembly);

                action(spc, src, registrationOptions);
            });
        }

        /// <summary>
        /// Allows to produce source files that will be included in the users compilation that have no semantic impact on user code from the point of view of code analysis.
        /// This allows a host such as the IDE, to chose not to run these outputs as a performance optimization. A host that produces executable code will always run these outputs.
        /// </summary>
        /// <typeparam name="TSource">The type of the <see cref="IncrementalValuesProvider{TSource}"/> that provides the input values</typeparam>
        /// <param name="incrementalGeneratorInitializationContext">The <see cref="IncrementalGeneratorInitializationContext"/> where the output is registered on</param>
        /// <param name="source">The <see cref="IncrementalValuesProvider{TSource}"/> that provides the input values</param>
        /// <param name="action">The <see cref="Action{SourceProductionContext, TSource, RegistrationOptions}"/> that is invoked for every value in the value provider</param>
        public static void RegisterImplementationCompilationInfoOutput<TSource>(this IncrementalGeneratorInitializationContext incrementalGeneratorInitializationContext,
            IncrementalValuesProvider<TSource> source,
            Action<SourceProductionContext, TSource, RegistrationOptions> action)
        {
            var callingAssembly = System.Reflection.Assembly.GetCallingAssembly().GetName();

            var compilationInfoValuesProvider = getCompilationInfoValuesProvider(incrementalGeneratorInitializationContext);

            var combinedSource = source.Combine(compilationInfoValuesProvider);

            incrementalGeneratorInitializationContext.RegisterImplementationSourceOutput(combinedSource, (spc, opt) =>
            {
                TSource src = opt.Left;
                ((NullableContextOptions nullableContextOptions, Language language), int languageVersion) = opt.Right;

                var registrationOptions = new RegistrationOptions(spc, language, languageVersion, nullableContextOptions, callingAssembly);

                action(spc, src, registrationOptions);
            });
        }

        /// <summary>
        /// Allows to provide source code immediately after initialization has run. It takes no inputs, and so cannot refer to any source code written by the user, or any other compiler inputs.
        /// </summary>
        /// <param name="incrementalGeneratorInitializationContext">The <see cref="IncrementalGeneratorInitializationContext"/> where the output is registered on</param>
        /// <param name="callback">The <see cref="Action{IncrementalGeneratorPostInitializationContext, CompilationInfoFactory}"/> that is invoked to provide the source code.</param>
        public static void RegisterPostInitializationCompilationInfoOutput(this IncrementalGeneratorInitializationContext incrementalGeneratorInitializationContext,
            Action<IncrementalGeneratorPostInitializationContext, CompilationInfoFactory> callback)
        {
            var callingAssembly = System.Reflection.Assembly.GetCallingAssembly().GetName();

            incrementalGeneratorInitializationContext.RegisterPostInitializationOutput(ctx =>
            {
                CompilationInfoFactory compilationInfoFactoryFunc = (string compilationName, string? targetNamespace) =>
                {
                    return new CompilationInfo(compilationName, targetNamespace, null, null, callingAssembly);
                };

                callback(ctx, compilationInfoFactoryFunc);
            });
        }
    }
}
