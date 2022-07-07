﻿using System.Text;

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
        /// Appends the current method as C# code to the given <see cref="StringBuilder"/>
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> that the method is added to</param>
        /// <param name="indentCnt">The count of indentation levels the method should be indented by</param>
        /// <param name="childIndentCnt">The count of indentation levels the method body should be indented by</param>
        /// <param name="indent">A string containing the indentation characters for the current class (a string containing the <see cref="SyntaxNode.IndentationCharacter"/> times the <see cref="SyntaxNode.IndentationCharacterCountPerLevel"/>)</param>
        protected override void ToCSharpString(StringBuilder sb, int indentCnt, int childIndentCnt, string indent)
        {
            appendXmlDoc(sb, indent);

            sb.Append(indent);
            sb.Append(AccessModifier.ToKeywordString());
            sb.Append(' ');

            if (IsStatic)
                sb.Append("static ");

            if (IsAsync)
                sb.Append("async ");

            sb.Append(ReturnType);
            sb.Append(' ');

            sb.Append(_name);

            sb.Append('(');

            sb.AppendLine(")");

            sb.Append(indent);
            sb.AppendLine("{");

            var ci = childIndentCnt * IndentationCharacterCountPerLevel;
            if (_body != null)
            {
                foreach (var line in _body)
                {
                    sb.Append(IndentationCharacter, ci);
                    sb.AppendLine(line);
                }
            }

            sb.Append(indent);
            sb.Append("}");
        }

        private void appendXmlDoc(StringBuilder sb, string indent)
        {
            var hasXmlDoc = !string.IsNullOrWhiteSpace(XmlDocSummary)
                || _xmlDocAdditionalLines?.Count > 0;

            if (!hasXmlDoc)
                return;

            sb.Append(indent);
            sb.AppendLine("/// <summary>");

            if (string.IsNullOrWhiteSpace(XmlDocSummary))
            {
                sb.Append(indent);
                sb.Append("/// ");
                sb.AppendLine(Name);
            }
            else
            {
                var lines = XmlDocSummary!.Split(LineSeparators, StringSplitOptions.None);

                foreach (var line in lines)
                {
                    sb.Append(indent);
                    sb.Append("/// ");
                    sb.AppendLine(line);
                }
            }

            sb.Append(indent);
            sb.AppendLine("/// </summary>");

            if (_xmlDocAdditionalLines != null)
            {
                foreach (var line in _xmlDocAdditionalLines)
                {
                    sb.Append(indent);
                    sb.Append("/// ");
                    sb.AppendLine(line);
                }
            }
        }

    }
}
