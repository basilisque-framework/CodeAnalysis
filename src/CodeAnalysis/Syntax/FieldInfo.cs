/*
   Copyright 2023 Alexander Stärk

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
using System.Text;

namespace Basilisque.CodeAnalysis.Syntax
{


    /// <summary>
    /// Represents a field for source generation
    /// </summary>
    public class FieldInfo : SyntaxNode
    {
        private string _name;
        private string? _type;
        private List<string>? _xmlDocAdditionalLines;

        /// <summary>
        /// The access modifier that specifies the accessibility of the field
        /// </summary>
        /// <example>private</example>
        public AccessModifier AccessModifier { get; set; } = AccessModifier.Private;

        /// <summary>
        /// The type of the parameter
        /// </summary>
        public string Type
        {
            get
            {
                return _type!;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(nameof(value));

                _type = value;
            }
        }

        /// <summary>
        /// The name of the field
        /// </summary>
        /// <exception cref="ArgumentNullException">Setting the <see cref="Name"/> to null or to an empty string throws an <see cref="ArgumentNullException"/></exception>
        /// <example>_myField</example>
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
        /// The initial value of the field
        /// </summary>
        public string? InitialValue { get; set; }

        /// <summary>
        /// Creates a new <see cref="FieldInfo"/>
        /// </summary>
        /// <param name="type">The type of the field as string</param>
        /// <param name="name">The name of the field</param>
        /// <exception cref="ArgumentNullException">Throws an <see cref="ArgumentNullException"/> when the name parameter is null or an empty string</exception>
        public FieldInfo(string type, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentNullException(nameof(type));

            _name = name;
            _type = type;
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
        /// Creates a new <see cref="FieldInfo"/>
        /// </summary>
        /// <param name="type">The type of the field as string</param>
        /// <param name="name">The name of the field</param>
        /// <param name="initialValue">The initial value of the field</param>
        /// <exception cref="ArgumentNullException">Throws an <see cref="ArgumentNullException"/> when the name parameter is null or an empty string</exception>
        public FieldInfo(string type, string name, string initialValue)
            : this(type, name)
        {
            InitialValue = initialValue;
        }


        /// <summary>
        /// Appends the current <see cref="FieldInfo"/> as C# code to the given <see cref="StringBuilder"/>
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> that the <see cref="FieldInfo"/> is added to</param>
        /// <param name="indentLvl">The count of indentation levels the <see cref="FieldInfo"/> should be indented by</param>
        /// <param name="childIndentLvl">The count of indentation levels the direct children of this <see cref="FieldInfo"/> should be indented by</param>
        /// <param name="indentCharCnt">The count of indentation characters for the current <see cref="FieldInfo"/> (how many times should the <see cref="SyntaxNode.IndentationCharacter"/> be repeated for the current level)</param>
        /// <param name="childIndentCharCnt">The count of indentation characters for the direct childre of this <see cref="FieldInfo"/> (how many times should the <see cref="SyntaxNode.IndentationCharacter"/> be repeated for the direct child level)</param>
        protected override void ToCSharpString(StringBuilder sb, int indentLvl, int childIndentLvl, int indentCharCnt, int childIndentCharCnt)
        {
            appendXmlDoc(sb, indentCharCnt);

            AppendIntentation(sb, indentCharCnt);

            sb.Append(AccessModifier.ToKeywordString());
            sb.Append(' ');

            sb.Append(_type);
            sb.Append(' ');

            sb.Append(_name);

            var initialValue = InitialValue;
            if (initialValue != null)
            {
                var isStringType = _type!.Equals("string", StringComparison.InvariantCultureIgnoreCase) || _type.Equals("system.string", StringComparison.InvariantCultureIgnoreCase);

                if (isStringType)
                {
                    if (!StringIsInParenthesisCSharp(initialValue))
                    {
                        initialValue = initialValue.Replace("\"", "\\\"");

                        initialValue = $"\"{initialValue}\"";
                    }

                    sb.Append(" = ");
                    sb.Append(initialValue);
                }
                else if (!string.Empty.Equals(initialValue))
                {
                    sb.Append(" = ");
                    sb.Append(initialValue);
                }
            }

            sb.Append(';');
        }

        private void appendXmlDoc(StringBuilder sb, int indentCharCnt)
        {
            var hasXmlDoc = !string.IsNullOrWhiteSpace(XmlDocSummary)
                || _xmlDocAdditionalLines?.Count > 0;

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
    }
}
