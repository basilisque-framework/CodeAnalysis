namespace Basilisque.CodeAnalysis.Syntax
{
    /// <summary>
    /// Options that are provided with a registration callback.
    /// </summary>
    public struct RegistrationOptions
    {
        /// <summary>
        /// The language of the current compilation
        /// </summary>
        public Language Language { get; }

        /// <summary>
        /// The version of the <see cref="Language"/> that is used for the current compilation
        /// </summary>
        public Microsoft.CodeAnalysis.CSharp.LanguageVersion LanguageVersion { get; }

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
        /// <param name="language">The language of the current compilation</param>
        /// <param name="languageVersion">The version of the <paramref name="language"/> that is used for the current compilation</param>
        /// <param name="nullableContextOptions">The <see cref="Microsoft.CodeAnalysis.NullableContextOptions"/> of the current compilation</param>
        /// <param name="callingAssembly">The <see cref="System.Reflection.AssemblyName"/> of the assembly that is calling the register method</param>
        internal RegistrationOptions(Language language, Microsoft.CodeAnalysis.CSharp.LanguageVersion languageVersion, NullableContextOptions nullableContextOptions, System.Reflection.AssemblyName callingAssembly)
        {
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
        public CompilationInfo CreateCompilationInfo(string compilationName, string? targetNamespace)
        {
            var compilationInfo = new CompilationInfo(compilationName, targetNamespace, null, null, CallingAssembly);

            compilationInfo.EnableNullableContext = NullableContextOptions.AnnotationsEnabled();

            return compilationInfo;
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
