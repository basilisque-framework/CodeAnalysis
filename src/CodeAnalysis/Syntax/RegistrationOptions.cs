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
        /// <param name="nullableContextOptions">The <see cref="Microsoft.CodeAnalysis.NullableContextOptions"/> of the current compilation</param>
        /// <param name="callingAssembly">The <see cref="System.Reflection.AssemblyName"/> of the assembly that is calling the register method</param>
        internal RegistrationOptions(Language language, NullableContextOptions nullableContextOptions, System.Reflection.AssemblyName callingAssembly)
        {
            Language = language;
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
    }
}
