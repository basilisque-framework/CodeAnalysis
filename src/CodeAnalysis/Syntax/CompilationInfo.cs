using System.Text;

namespace Basilisque.CodeAnalysis.Syntax
{
    /// <summary>
    /// Represents a single source output for source generation
    /// (e.g. to be used as source input for a <see cref="SourceProductionContext"/>)
    /// </summary>
    public class CompilationInfo : SyntaxNode
    {
        private string _compilationName;
        private string? _targetNamespace;

        /// <summary>
        /// The name of the current compilation or rather the 'source output'
        /// (e.g. used as the hintName for the <see cref="SourceProductionContext"/>)
        /// </summary>
        public string CompilationName
        {
            get
            {
                return _compilationName;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(nameof(CompilationName));

                _compilationName = value;
            }
        }

        /// <summary>
        /// The containing namespace of all child syntax nodes
        /// (when the <see cref="TargetNamespace"/> is null, empty or whitespace, all children are in the global namespace)
        /// </summary>
        public string? TargetNamespace
        {
            get
            {
                return _targetNamespace;
            }
            set
            {
                _targetNamespace = value;
            }
        }

        /// <summary>
        /// A list of classes that are contained in the given <see cref="TargetNamespace"/>
        /// </summary>
        public List<ClassInfo> Classes { get; } = new List<ClassInfo>();

        /// <summary>
        /// Creates a new <see cref="CompilationInfo"/> for the specified language
        /// </summary>
        /// <param name="compilationName" example="CodeAnalysis_GeneratedSource">The name of the current compilation or rather the 'source output' (e.g. used as the hintName for the <see cref="SourceProductionContext"/>)</param>
        /// <param name="targetNamespace" example="Basilisque.CodeAnalysis">The containing namespace of all child syntax nodes (when the <see cref="TargetNamespace"/> is null, empty or whitespace, all children are in the global namespace)</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="compilationName"/> is null, empty or whitespace</exception>
        public CompilationInfo(
            string compilationName,
            string? targetNamespace
            )
        {
            if (string.IsNullOrWhiteSpace(compilationName))
                throw new ArgumentNullException(nameof(compilationName));

            _compilationName = compilationName;
            _targetNamespace = targetNamespace;
        }

        /// <summary>
        /// Creates a new <see cref="ClassInfo"/> with <see cref="ClassInfo.AccessModifier"/> set to <see cref="AccessModifier.Public"/> and adds it to the list of <see cref="Classes"/>
        /// </summary>
        /// <param name="className" example="MyClass">The name of the class</param>
        /// <param name="classConfiguration">A callback that is executed after creating the <see cref="ClassInfo"/> and that can be used to configure the created object</param>
        /// <returns>Returns the current <see cref="CompilationInfo"/> to enable use of fluent syntax</returns>
        public CompilationInfo AddNewClassInfo(string className, Action<ClassInfo> classConfiguration)
        {
            return AddNewClassInfo(className, AccessModifier.Public, classConfiguration);
        }

        /// <summary>
        /// Creates a new <see cref="ClassInfo"/> and adds it to the list of <see cref="Classes"/>
        /// </summary>
        /// <param name="className" example="MyClass">The name of the class</param>
        /// <param name="accessModifier" example="AccessModifier.Public">The access modifier that specifies the accessibility of the class</param>
        /// <param name="classConfiguration">A callback that is executed after creating the <see cref="ClassInfo"/> and that can be used to configure the created object</param>
        /// <returns>Returns the current <see cref="CompilationInfo"/> to enable use of fluent syntax</returns>
        public CompilationInfo AddNewClassInfo(string className, AccessModifier accessModifier, Action<ClassInfo> classConfiguration)
        {
            var classInfo = new ClassInfo(className, accessModifier);

            classConfiguration(classInfo);

            Classes.Add(classInfo);

            return this;
        }

        /// <summary>
        /// Converts the current <see cref="CompilationInfo"/> to its string representation and adds it to the given <see cref="SourceProductionContext"/>
        /// </summary>
        /// <param name="context">The <see cref="SourceProductionContext"/> where the <see cref="SourceProductionContext.AddSource(string, string)"/> method is called on</param>
        /// <returns>Returns the current <see cref="CompilationInfo"/> to enable use of fluent syntax</returns>
        public CompilationInfo AddToSourceProductionContext(SourceProductionContext context, Language language)
        {
            string fileExt;
            switch (language)
            {
                case Language.CSharp:
                default:
                    fileExt = ".cs";
                    break;
            }

            var compilationName = _compilationName;
            if (!compilationName.EndsWith(fileExt))
            {
                if (compilationName.EndsWith(".g"))
                    compilationName = compilationName + fileExt;
                else
                    compilationName = compilationName + ".g" + fileExt;
            }

            context.AddSource(_compilationName, ToString(language));

            return this;
        }

        /// <summary>
        /// Appends the current <see cref="CompilationInfo"/> and its children as C# code to the given <see cref="StringBuilder"/>
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> that the <see cref="CompilationInfo"/> is added to</param>
        /// <param name="indentCnt">The count of indentation levels the <see cref="CompilationInfo"/> should be indented by</param>
        /// <param name="childIndentCnt">The count of indentation levels the direct children of this <see cref="CompilationInfo"/> should be indented by</param>
        /// <param name="indent">A string containing the indentation characters for the current <see cref="CompilationInfo"/> (a string containing the <see cref="SyntaxNode.IndentationCharacter"/> times the <see cref="SyntaxNode.IndentationCharacterCountPerLevel"/>)</param>
        protected override void ToCSharpString(StringBuilder sb, int indentCnt, int childIndentCnt, string indent)
        {
            var hasNamespace = !string.IsNullOrWhiteSpace(_targetNamespace);

            if (hasNamespace)
            {
                sb.Append("namespace ");
                sb.AppendLine(_targetNamespace);
                sb.AppendLine("{");
            }

            bool addEmptyLineForNextClass = false;
            foreach (var classInfo in Classes)
            {
                if (addEmptyLineForNextClass)
                {
                    sb.AppendLine();
                    sb.AppendLine(indent);
                }
                else
                    addEmptyLineForNextClass = true;

                classInfo.ToString(sb, hasNamespace ? childIndentCnt : indentCnt, Language.CSharp);
            }

            if (hasNamespace)
            {
                if (addEmptyLineForNextClass)
                    sb.AppendLine();
                sb.Append("}");
            }
        }
    }
}
