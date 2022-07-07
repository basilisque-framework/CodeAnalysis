using System.Text;

namespace Basilisque.CodeAnalysis.Syntax
{
    /// <summary>
    /// Creates a new <see cref="CompilationInfo"/>
    /// </summary>
    /// <param name="compilationName" example="CodeAnalysis_GeneratedSource">The name of the current compilation or rather the 'source output' (e.g. used as the hintName for the <see cref="SourceProductionContext"/>)</param>
    /// <param name="targetNamespace" example="Basilisque.CodeAnalysis">The containing namespace of all child syntax nodes (when the targetNamespace is null, empty or whitespace, all children are in the global namespace)</param>
    /// <returns>A new instance of <see cref="CompilationInfo"/></returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="compilationName"/> is null, empty or whitespace</exception>
    public delegate CompilationInfo CompilationInfoFactory(string compilationName, string? targetNamespace);


    /// <summary>
    /// Represents a single source output for source generation
    /// (e.g. to be used as source input for a <see cref="SourceProductionContext"/>)
    /// </summary>
    public class CompilationInfo : SyntaxNode
    {
        private System.Reflection.AssemblyName _constructingAssemblyName;
        private string _compilationName;
        private string? _targetNamespace;
        private string? _generatedCodeToolName;
        private string? _generatedCodeToolVersion;
        private List<ClassInfo>? _classes;

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
        /// Defines if the generated source contains attributes to mark it as generated code
        /// </summary>
        public bool AddGeneratedCodeAttributes { get; set; } = true;

        /// <summary>
        /// The name of the tool that generated the source.
        /// (used for the <see cref="System.CodeDom.Compiler.GeneratedCodeAttribute"/>)
        /// </summary>
        public string? GeneratedCodeToolName
        {
            get
            {
                return _generatedCodeToolName;
            }
            set
            {
                _generatedCodeToolName = value;
            }
        }

        /// <summary>
        /// The version of the tool that generated the source.
        /// (used for the <see cref="System.CodeDom.Compiler.GeneratedCodeAttribute"/>)
        /// </summary>
        public string? GeneratedCodeToolVersion
        {
            get
            {
                return _generatedCodeToolVersion;
            }
            set
            {
                _generatedCodeToolVersion = value;
            }
        }

        /// <summary>
        /// Defines if a nullable context is explicitly enabled for the generated code.
        /// (When set to true, the generated code is enclosed in #nullable enable/restore)
        /// </summary>
        public bool EnableNullableContext { get; set; } = true;

        /// <summary>
        /// A list of classes that are contained in the given <see cref="TargetNamespace"/>
        /// </summary>
        public List<ClassInfo> Classes
        {
            get
            {
                if (_classes == null)
                    _classes = new List<ClassInfo>();

                return _classes;
            }
        }

        /// <summary>
        /// Creates a new <see cref="CompilationInfo"/> for the specified language
        /// </summary>
        /// <param name="compilationName" example="CodeAnalysis_GeneratedSource">The name of the current compilation or rather the 'source output' (e.g. used as the hintName for the <see cref="SourceProductionContext"/>)</param>
        /// <param name="targetNamespace" example="Basilisque.CodeAnalysis">The containing namespace of all child syntax nodes (when the <see cref="TargetNamespace"/> is null, empty or whitespace, all children are in the global namespace)</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="compilationName"/> is null, empty or whitespace</exception>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        public CompilationInfo(
            string compilationName,
            string? targetNamespace
            )
            : this(compilationName, targetNamespace, null, null, System.Reflection.Assembly.GetCallingAssembly().GetName())
        { }

        /// <summary>
        /// Creates a new <see cref="CompilationInfo"/> for the specified language
        /// </summary>
        /// <param name="compilationName" example="CodeAnalysis_GeneratedSource">The name of the current compilation or rather the 'source output' (e.g. used as the hintName for the <see cref="SourceProductionContext"/>)</param>
        /// <param name="targetNamespace" example="Basilisque.CodeAnalysis">The containing namespace of all child syntax nodes (when the <see cref="TargetNamespace"/> is null, empty or whitespace, all children are in the global namespace)</param>
        /// <param name="generatedCodeToolName">The name of the tool that generated the source. (used for the <see cref="System.CodeDom.Compiler.GeneratedCodeAttribute"/>)</param>
        /// <param name="generatedCodeToolVersion">The version of the tool that generated the source. (used for the <see cref="System.CodeDom.Compiler.GeneratedCodeAttribute"/>)</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="compilationName"/> is null, empty or whitespace</exception>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        public CompilationInfo(
            string compilationName,
            string? targetNamespace,
            string generatedCodeToolName,
            string generatedCodeToolVersion
            )
            : this(compilationName, targetNamespace, generatedCodeToolName, generatedCodeToolVersion, System.Reflection.Assembly.GetCallingAssembly().GetName())
        { }

        internal CompilationInfo(
            string compilationName,
            string? targetNamespace,
            string? generatedCodeToolName,
            string? generatedCodeToolVersion,
            System.Reflection.AssemblyName constructingAssemblyName
            )
        {
            if (string.IsNullOrWhiteSpace(compilationName))
                throw new ArgumentNullException(nameof(compilationName));

            _compilationName = compilationName;
            _targetNamespace = targetNamespace;
            _generatedCodeToolName = generatedCodeToolName;
            _generatedCodeToolVersion = generatedCodeToolVersion;
            _constructingAssemblyName = constructingAssemblyName;
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
            var classInfo = new ClassInfo(className, accessModifier, GeneratedCodeToolName, GeneratedCodeToolVersion, _constructingAssemblyName);

            classInfo.AddGeneratedCodeAttributes = this.AddGeneratedCodeAttributes;

            classConfiguration(classInfo);

            Classes.Add(classInfo);

            return this;
        }

        /// <summary>
        /// Converts the current <see cref="CompilationInfo"/> to its string representation and adds it to the given <see cref="SourceProductionContext"/>
        /// </summary>
        /// <param name="context">The <see cref="SourceProductionContext"/> where the <see cref="SourceProductionContext.AddSource(string, string)"/> method is called on</param>
        /// <param name="language">The <see cref="Language"/> that is used to generate the source code</param>
        /// <returns>Returns the current <see cref="CompilationInfo"/> to enable use of fluent syntax</returns>
        public CompilationInfo AddToSourceProductionContext(SourceProductionContext context, Language language)
        {
            var compilationName = GetHintName(language);

            context.AddSource(compilationName, ToString(language));

            return this;
        }

        /// <summary>
        /// Returns the hint name that is used by <see cref="AddToSourceProductionContext"/> to add the <see cref="CompilationInfo"/> to a <see cref="SourceProductionContext"/>
        /// </summary>
        /// <param name="language">The <see cref="Language"/> that is used to generate the source code</param>
        /// <returns>The hint name as string</returns>
        public string GetHintName(Language language)
        {
            string fileExt;
            switch (language)
            {
                case Language.VisualBasic:
                    fileExt = ".vb";
                    break;
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

            return compilationName;
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
            var hasClasses = _classes?.Any() == true;

            var hasAnythingToGenerate = hasNamespace || hasClasses;

            if (!hasAnythingToGenerate)
                return;

            if (AddGeneratedCodeAttributes)
            {
                var generatedCodeToolName = GeneratedCodeToolName ?? _constructingAssemblyName.Name;
                var generatedCodeToolVersion = GeneratedCodeToolVersion ?? _constructingAssemblyName.Version.ToString();

                sb.AppendLine("//------------------------------------------------------------------------");
                sb.AppendLine("// <auto-generated>");
                sb.AppendLine("//   This code was generated by a tool.");
                sb.Append("//   ");
                sb.Append(generatedCodeToolName);
                sb.Append(", ");
                sb.AppendLine(generatedCodeToolVersion);
                sb.AppendLine("//   ");
                sb.AppendLine("//   Changes to this file may cause incorrect behavior and will be lost if");
                sb.AppendLine("//   the code is regenerated.");
                sb.AppendLine("// </auto-generated>");
                sb.AppendLine("//------------------------------------------------------------------------");
                sb.AppendLine();
            }

            if (EnableNullableContext)
            {
                sb.AppendLine("#nullable enable");
                sb.AppendLine();
            }

            if (hasNamespace)
            {
                sb.Append("namespace ");
                sb.AppendLine(_targetNamespace);
                sb.AppendLine("{");
            }

            bool addEmptyLineForNextClass = false;
            if (_classes != null)
            {
                foreach (var classInfo in _classes)
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
            }

            if (hasNamespace)
            {
                if (addEmptyLineForNextClass)
                    sb.AppendLine();
                sb.Append("}");
            }


            if (EnableNullableContext)
            {
                sb.AppendLine();
                sb.AppendLine();
                sb.Append("#nullable restore");
            }
        }
    }
}
