﻿using System.Text;

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
        private List<string>? _xmlDocAdditionalLines;
        private List<string>? _implementedInterfaces;
        private Dictionary<string, (List<string>? Constraints, string? XmlDoc)?>? _genericTypes;
        private List<MethodInfo>? _methods;

        /// <summary>
        /// The name of the class
        /// </summary>
        /// <exception cref="ArgumentNullException">Setting the <see cref="ClassName"/> to null or to an empty string throws an <see cref="ArgumentNullException"/></exception>
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
        /// Defines if the generated class is a partial class or not
        /// </summary>
        public bool IsPartial { get; set; } = false;

        /// <summary>
        /// The text that is used as summary for the XML documentation comment
        /// </summary>
        public string? XmlDocSummary { get; set; }

        /// <summary>
        /// Additional XML documentation lines.
        /// Full lines including XML tags.
        /// </summary>
        public List<string> XmlDocAdditionalLines
        {
            get
            {
                if (_xmlDocAdditionalLines == null)
                    _xmlDocAdditionalLines = new List<string>();

                return _xmlDocAdditionalLines;
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
        public IList<string> ImplementedInterfaces
        {
            get
            {
                if (_implementedInterfaces == null)
                    _implementedInterfaces = new List<string>();

                return _implementedInterfaces;
            }
        }

        /// <summary>
        /// A list of <see cref="MethodInfo"/> that this class contains
        /// </summary>
        public List<MethodInfo> Methods
        {
            get
            {
                if (_methods == null)
                    _methods = new List<MethodInfo>();

                return _methods;
            }
        }

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
        public Dictionary<string, (List<string>? Constraints, string? XmlDoc)?> GenericTypes
        {
            get
            {
                if (_genericTypes == null)
                    _genericTypes = new Dictionary<string, (List<string>? Constraints, string? XmlDoc)?>();

                return _genericTypes;
            }
        }

        /// <summary>
        /// Appends the current class and its children as C# code to the given <see cref="StringBuilder"/>
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> that the class is added to</param>
        /// <param name="indentLvl">The count of indentation levels the class should be indented by</param>
        /// <param name="childIndentLvl">The count of indentation levels the direct children of this class should be indented by</param>
        /// <param name="indentCharCnt">The count of indentation characters for the current class (how many times should the <see cref="SyntaxNode.IndentationCharacter"/> be repeated for the current level)</param>
        /// <param name="childIndentCharCnt">The count of indentation characters for the direct childre of this class (how many times should the <see cref="SyntaxNode.IndentationCharacter"/> be repeated for the direct child level)</param>
        protected override void ToCSharpString(StringBuilder sb, int indentLvl, int childIndentLvl, int indentCharCnt, int childIndentCharCnt)
        {
            appendXmlDoc(sb, indentCharCnt);

            if (AddGeneratedCodeAttributes)
            {
                var generatedCodeToolName = GeneratedCodeToolName ?? _constructingAssemblyName.Name;
                var generatedCodeToolVersion = GeneratedCodeToolVersion ?? _constructingAssemblyName.Version.ToString();

                AppendIntentation(sb, indentCharCnt);
                sb.AppendLine($@"[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""{generatedCodeToolName}"", ""{generatedCodeToolVersion}"")]");
                AppendIntentation(sb, indentCharCnt);
                sb.AppendLine(@"[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]");
                AppendIntentation(sb, indentCharCnt);
                sb.AppendLine(@"[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]");
            }

            appendClassWithName(sb, indentCharCnt);

            bool hasGenericTypeConstraints = appendGenericTypes(sb);

            appendBaseClassAndInterfaces(sb);

            //append line break after class declaration
            sb.AppendLine();

            if (hasGenericTypeConstraints)
                appendGenericTypeConstraints(sb, childIndentCharCnt);

            AppendIntentation(sb, indentCharCnt);
            sb.AppendLine("{");

            //ToDo: add properties, ...

            if (_methods != null)
            {
                var isFirst = true;

                foreach (var method in _methods)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        AppendIntentationLine(sb, childIndentCharCnt);

                    method.ToString(sb, childIndentLvl, Language.CSharp);
                    sb.AppendLine();
                }
            }

            AppendIntentation(sb, indentCharCnt);
            sb.Append("}");
        }

        private void appendXmlDoc(StringBuilder sb, int indentCharCnt)
        {
            var hasXmlDoc = !string.IsNullOrWhiteSpace(XmlDocSummary)
                || _xmlDocAdditionalLines?.Count > 0
                || _genericTypes?.Any(gt => !string.IsNullOrWhiteSpace(gt.Value?.XmlDoc)) == true;

            if (!hasXmlDoc)
                return;

            AppendIntentation(sb, indentCharCnt);
            sb.AppendLine("/// <summary>");

            if (string.IsNullOrWhiteSpace(XmlDocSummary))
            {
                AppendIntentation(sb, indentCharCnt);
                sb.Append("/// ");
                sb.AppendLine(ClassName);
            }
            else
            {
                var lines = XmlDocSummary!.Split(LineSeparators, StringSplitOptions.None);

                foreach (var line in lines)
                {
                    AppendIntentation(sb, indentCharCnt);
                    sb.Append("/// ");
                    sb.AppendLine(line);
                }
            }

            AppendIntentation(sb, indentCharCnt);
            sb.AppendLine("/// </summary>");

            if (_genericTypes != null)
            {
                foreach (var gt in _genericTypes)
                {
                    var gtXmlDoc = gt.Value?.XmlDoc;

                    AppendIntentation(sb, indentCharCnt);
                    sb.Append("/// <typeparam name=\"");
                    sb.Append(gt.Key);
                    sb.Append("\">");

                    if (!string.IsNullOrWhiteSpace(gtXmlDoc))
                        sb.Append(gtXmlDoc);

                    sb.AppendLine("</typeparam>");
                }
            }

            if (_xmlDocAdditionalLines != null)
            {
                foreach (var line in _xmlDocAdditionalLines)
                {
                    AppendIntentation(sb, indentCharCnt);
                    sb.Append("/// ");
                    sb.AppendLine(line);
                }
            }
        }

        private void appendClassWithName(StringBuilder sb, int indentCharCnt)
        {
            AppendIntentation(sb, indentCharCnt);
            sb.Append(AccessModifier.ToKeywordString());

            if (IsPartial)
                sb.Append(" partial");

            sb.Append(" class ");
            sb.Append(ClassName);
        }

        private bool appendGenericTypes(StringBuilder sb)
        {
            bool hasGenericTypeConstraints = false;

            if (_genericTypes?.Count > 0)
            {
                sb.Append("<");

                bool addCommaBeforeNextGenericType = false;

                foreach (var genericType in _genericTypes)
                {
                    if (addCommaBeforeNextGenericType)
                        sb.Append(", ");
                    else
                        addCommaBeforeNextGenericType = true;

                    sb.Append(genericType.Key);

                    if (!hasGenericTypeConstraints && genericType.Value?.Constraints?.Count > 0)
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
            if (_implementedInterfaces != null)
            {
                foreach (var interfaceName in _implementedInterfaces)
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
        }

        private void appendGenericTypeConstraints(StringBuilder sb, int childIndentCharCnt)
        {
            if (_genericTypes == null)
                return;

            foreach (var genericType in _genericTypes)
            {
                if (genericType.Value?.Constraints?.Count > 0)
                {
                    AppendIntentation(sb, childIndentCharCnt);
                    sb.Append("where ");
                    sb.Append(genericType.Key);
                    sb.Append(" : ");

                    bool addCommaBeforeNextConstraint = false;
                    foreach (var constraint in genericType.Value?.Constraints!)
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
