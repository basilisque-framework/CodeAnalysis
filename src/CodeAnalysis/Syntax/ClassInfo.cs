using System.Text;

namespace Basilisque.CodeAnalysis.Syntax
{
    /// <summary>
    /// Represents a class for source generation
    /// </summary>
    public class ClassInfo : SyntaxNode
    {
        private System.Reflection.AssemblyName _constructingAssemblyName;
        private string _className;
        private string? _baseClass;
        private string? _generatedCodeToolName;
        private string? _generatedCodeToolVersion;

        /// <summary>
        /// The name of the class
        /// </summary>
        /// <exception cref="ArgumentNullException">Settings the <see cref="ClassName"/> to null or to an empty string throws an <see cref="ArgumentNullException"/></exception>
        /// <example>MyClass</example>
        public string ClassName
        {
            get
            {
                return _className;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(nameof(ClassName));

                _className = value;
            }
        }

        /// <summary>
        /// The access modifier that specifies the accessibility of the class
        /// </summary>
        /// <example>public</example>
        public AccessModifier AccessModifier { get; set; }

        /// <summary>
        /// The name of class that this class inherits from
        /// </summary>
        /// <example>MyBaseClass</example>
        public string? BaseClass
        {
            get
            {
                return _baseClass;
            }
            set
            {
                _baseClass = string.IsNullOrWhiteSpace(value) ? null : value;
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
        /// A list of interfaces that this class implements
        /// </summary>
        public IList<string> ImplementedInterfaces { get; } = new List<string>();

        /// <summary>
        /// Creates a new instance of <see cref="ClassInfo"/>
        /// </summary>
        /// <param name="className" example="MyClass">The name of the class</param>
        /// <param name="accessModifier" example="AccessModifier.Public">The access modifier that specifies the accessibility of the class</param>
        /// <exception cref="ArgumentNullException">Throws an <see cref="ArgumentNullException"/> when the className parameter is null or an empty string</exception>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        public ClassInfo(string className, AccessModifier accessModifier)
            : this(className, accessModifier, null, null, System.Reflection.Assembly.GetCallingAssembly().GetName())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="ClassInfo"/>
        /// </summary>
        /// <param name="className" example="MyClass">The name of the class</param>
        /// <param name="accessModifier" example="AccessModifier.Public">The access modifier that specifies the accessibility of the class</param>
        /// <param name="generatedCodeToolName">The name of the tool that generated the source. (used for the <see cref="System.CodeDom.Compiler.GeneratedCodeAttribute"/>)</param>
        /// <param name="generatedCodeToolVersion">The version of the tool that generated the source. (used for the <see cref="System.CodeDom.Compiler.GeneratedCodeAttribute"/>)</param>
        /// <exception cref="ArgumentNullException">Throws an <see cref="ArgumentNullException"/> when the className parameter is null or an empty string</exception>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        public ClassInfo(
            string className,
            AccessModifier accessModifier,
            string generatedCodeToolName,
            string generatedCodeToolVersion
            )
            : this(className, accessModifier, generatedCodeToolName, generatedCodeToolVersion, System.Reflection.Assembly.GetCallingAssembly().GetName())
        { }

        internal ClassInfo(
            string className,
            AccessModifier accessModifier,
            string? generatedCodeToolName,
            string? generatedCodeToolVersion,
            System.Reflection.AssemblyName constructingAssemblyName
            )
        {
            if (string.IsNullOrWhiteSpace(className))
                throw new ArgumentNullException(nameof(className));

            _className = className;
            AccessModifier = accessModifier;
            _generatedCodeToolName = generatedCodeToolName;
            _generatedCodeToolVersion = generatedCodeToolVersion;
            _constructingAssemblyName = constructingAssemblyName;
        }

        /// <summary>
        /// A list of generic type arguments of this class and their constraints
        /// </summary>
        public Dictionary<string, List<string>?> GenericTypes { get; } = new Dictionary<string, List<string>?>();

        /// <summary>
        /// Appends the current class and its children as C# code to the given <see cref="StringBuilder"/>
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> that the class is added to</param>
        /// <param name="indentCnt">The count of indentation levels the class should be indented by</param>
        /// <param name="childIndentCnt">The count of indentation levels the direct children of this class should be indented by</param>
        /// <param name="indent">A string containing the indentation characters for the current class (a string containing the <see cref="SyntaxNode.IndentationCharacter"/> times the <see cref="SyntaxNode.IndentationCharacterCountPerLevel"/>)</param>
        protected override void ToCSharpString(StringBuilder sb, int indentCnt, int childIndentCnt, string indent)
        {
            if (AddGeneratedCodeAttributes)
            {
                var generatedCodeToolName = GeneratedCodeToolName ?? _constructingAssemblyName.Name;
                var generatedCodeToolVersion = GeneratedCodeToolVersion ?? _constructingAssemblyName.Version.ToString();

                sb.Append(indent);
                sb.AppendLine($@"[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""{generatedCodeToolName}"", ""{generatedCodeToolVersion}"")]");
                sb.Append(indent);
                sb.AppendLine(@"[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]");
                sb.Append(indent);
                sb.AppendLine(@"[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]");
            }

            appendClassWithName(sb, indent);

            bool hasGenericTypeConstraints = appendGenericTypes(sb);

            appendBaseClassAndInterfaces(sb);

            //append line break after class declaration
            sb.AppendLine();

            if (hasGenericTypeConstraints)
                appendGenericTypeConstraints(sb, childIndentCnt);

            sb.Append(indent);
            sb.AppendLine("{");

            //ToDo: add methods, properties, ...

            sb.Append(indent);
            sb.Append("}");
        }

        private void appendClassWithName(StringBuilder sb, string indent)
        {
            sb.Append(indent);
            sb.Append(AccessModifier.ToKeywordString());
            sb.Append(" class ");
            sb.Append(ClassName);
        }

        private bool appendGenericTypes(StringBuilder sb)
        {
            bool hasGenericTypeConstraints = false;

            if (GenericTypes.Count > 0)
            {
                sb.Append("<");

                bool addCommaBeforeNextGenericType = false;

                foreach (var genericType in GenericTypes)
                {
                    if (addCommaBeforeNextGenericType)
                        sb.Append(", ");
                    else
                        addCommaBeforeNextGenericType = true;

                    sb.Append(genericType.Key);

                    if (!hasGenericTypeConstraints && genericType.Value?.Count > 0)
                        hasGenericTypeConstraints = true;
                }

                sb.Append(">");
            }

            return hasGenericTypeConstraints;
        }

        private void appendBaseClassAndInterfaces(StringBuilder sb)
        {
            //append base class
            bool addCommaBeforeNextImplementedInterface = false;
            if (_baseClass is not null)
            {
                sb.Append(" : ");
                sb.Append(_baseClass);

                addCommaBeforeNextImplementedInterface = true;
            }

            //append implemented interfaces
            foreach (var interfaceName in ImplementedInterfaces)
            {
                if (string.IsNullOrWhiteSpace(interfaceName))
                    continue;

                if (addCommaBeforeNextImplementedInterface)
                    sb.Append(", ");
                else
                {
                    addCommaBeforeNextImplementedInterface = true;
                    sb.Append(" : ");
                }

                sb.Append(interfaceName);
            }
        }

        private void appendGenericTypeConstraints(StringBuilder sb, int childIndentCnt)
        {
            var childIndent = GetIndentation(childIndentCnt);

            foreach (var genericType in GenericTypes)
            {
                if (genericType.Value?.Count > 0)
                {
                    sb.Append(childIndent);
                    sb.Append("where ");
                    sb.Append(genericType.Key);
                    sb.Append(" : ");

                    bool addCommaBeforeNextConstraint = false;
                    foreach (var constraint in genericType.Value)
                    {
                        if (addCommaBeforeNextConstraint)
                            sb.Append(", ");
                        else
                            addCommaBeforeNextConstraint = true;

                        sb.Append(constraint);
                    }

                    sb.AppendLine();
                }
            }
        }

    }
}
