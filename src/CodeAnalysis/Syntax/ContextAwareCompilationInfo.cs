namespace Basilisque.CodeAnalysis.Syntax
{
    /// <summary>
    /// Represents a single source output for source generation
    /// (e.g. to be used as source input for a <see cref="SourceProductionContext"/>)
    /// </summary>
    public class ContextAwareCompilationInfo : CompilationInfo<ContextAwareCompilationInfo>
    {
        /// <summary>
        /// The <see cref="SourceProductionContext"/> that the source code is added to
        /// </summary>
        public SourceProductionContext Context { get; set; }

        /// <summary>
        /// The language that is used to generate the source code
        /// </summary>
        public Language Language { get; set; }

        internal ContextAwareCompilationInfo(
            SourceProductionContext context,
            Language language,
            string compilationName,
            string? targetNamespace,
            string? generatedCodeToolName,
            string? generatedCodeToolVersion,
            System.Reflection.AssemblyName constructingAssemblyName
            )
            : base(compilationName,
                  targetNamespace,
                  generatedCodeToolName,
                  generatedCodeToolVersion,
                  constructingAssemblyName)
        {
            Context = context;
            Language = language;
        }

        /// <summary>
        /// Converts the current <see cref="CompilationInfo"/> to its string representation and adds it to the given <see cref="SourceProductionContext"/>
        /// </summary>
        /// <returns>Returns the current <see cref="CompilationInfo"/> to enable use of fluent syntax</returns>
        public ContextAwareCompilationInfo AddToSourceProductionContext()
        {
            var compilationName = GetHintName(Language);

            Context.AddSource(compilationName, ToString(Language));

            return this;
        }
    }
}