using System.Text;

namespace Basilisque.CodeAnalysis.Syntax
{
    /// <summary>
    /// Represents a method for source generation
    /// </summary>
    public class MethodInfo : SyntaxNode
    {
        private string _name;
        private string _returnType = "void";
        private CodeLines? _body;
        private List<string>? _xmlDocAdditionalLines;
        private Dictionary<string, (List<string>? Constraints, string? XmlDoc)?>? _genericTypes;

        /// <summary>
        /// The access modifier that specifies the accessibility of the method
        /// </summary>
        /// <example>public</example>
        public AccessModifier AccessModifier { get; set; }

        /// <summary>
        /// Defines if the method is static
        /// </summary>
        public bool IsStatic { get; set; } = false;

        /// <summary>
        /// Defines if the method is async
        /// </summary>
        public bool IsAsync { get; set; } = false;

        /// <summary>
        /// The return type of the method as string
        /// </summary>
        public string ReturnType
        {
            get
            {
                return _returnType;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    _returnType = "void";
                else
                    _returnType = value;
            }
        }

        /// <summary>
        /// The name of the method
        /// </summary>
        /// <exception cref="ArgumentNullException">Setting the <see cref="Name"/> to null or to an empty string throws an <see cref="ArgumentNullException"/></exception>
        /// <example>MyMethod</example>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(nameof(Name));

                _name = value;
            }
        }

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
        /// Contains the <see cref="CodeLines"/> of the body of the method
        /// </summary>
        public CodeLines Body
        {
            get
            {
                if (_body == null)
                    _body = new CodeLines();

                return _body;
            }
        }

        /// <summary>
        /// A list of generic type arguments of this method and their constraints
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
        /// Creates a new <see cref="MethodInfo"/>
        /// </summary>
        /// <param name="accessModifier">The access modifier that specifies the accessibility of the method</param>
        /// <param name="returnType">The return type of the method as string</param>
        /// <param name="name">The name of the method</param>
        public MethodInfo(AccessModifier accessModifier, string returnType, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            AccessModifier = accessModifier;
            ReturnType = returnType;
            _name = name;

            //read ReturnType again; could have been changed to 'void' by setter of the property
            returnType = ReturnType;
            if (name.EndsWith("Async") && (returnType == "void" || returnType.StartsWith("Task") || returnType.StartsWith("System.Threading.Tasks.Task")))
                IsAsync = true;

        }

        /// <summary>
        /// Appends the current method and its children as C# code to the given <see cref="StringBuilder"/>
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> that the method is added to</param>
        /// <param name="indentLvl">The count of indentation levels the method should be indented by</param>
        /// <param name="childIndentLvl">The count of indentation levels the direct children of this method should be indented by</param>
        /// <param name="indentCharCnt">The count of indentation characters for the current method (how many times should the <see cref="SyntaxNode.IndentationCharacter"/> be repeated for the current level)</param>
        /// <param name="childIndentCharCnt">The count of indentation characters for the direct childre of this method (how many times should the <see cref="SyntaxNode.IndentationCharacter"/> be repeated for the direct child level)</param>
        protected override void ToCSharpString(StringBuilder sb, int indentLvl, int childIndentLvl, int indentCharCnt, int childIndentCharCnt)
        {
            appendXmlDoc(sb, indentCharCnt);

            AppendIntentation(sb, indentCharCnt);
            sb.Append(AccessModifier.ToKeywordString());
            sb.Append(' ');

            if (IsStatic)
                sb.Append("static ");

            if (IsAsync)
                sb.Append("async ");

            sb.Append(ReturnType);
            sb.Append(' ');

            sb.Append(_name);

            bool hasGenericTypeConstraints = appendGenericTypes(sb);

            sb.Append('(');

            sb.AppendLine(")");

            if (hasGenericTypeConstraints)
                appendGenericTypeConstraints(sb, childIndentCharCnt);

            AppendIntentation(sb, indentCharCnt);
            sb.AppendLine("{");

            if (_body != null)
            {
                foreach (var line in _body)
                {
                    AppendIntentation(sb, childIndentCharCnt);
                    sb.AppendLine(line);
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
                sb.AppendLine(Name);
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
