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
    /// Represents a parameter for source generation
    /// </summary>
    public class ParameterInfo : SyntaxNode
    {
        private string? _name;
        private string? _type;

        /// <summary>
        /// Defines the kind of the parameter
        /// </summary>
        public ParameterKind ParameterKind { get; set; }

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
        /// Defines the name of the parameter
        /// </summary>
        public string Name
        {
            get
            {
                return _name!;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(nameof(value));

                _name = value;
            }
        }

        /// <summary>
        /// The default value of the parameter
        /// </summary>
        public string? DefaultValue { get; set; }

        /// <summary>
        /// Creates a new <see cref="ParameterInfo"/>
        /// </summary>
        /// <param name="parameterKind">The kind of the parameter</param>
        /// <param name="type">The type of the parameter</param>
        /// <param name="name">The name of the parameter</param>
        public ParameterInfo(ParameterKind parameterKind, string type, string name)
        {
            ParameterKind = parameterKind;
            Type = type;
            Name = name;
        }

        /// <summary>
        /// Appends the current <see cref="ParameterInfo"/> as C# code to the given <see cref="StringBuilder"/>
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> that the <see cref="ParameterInfo"/> is added to</param>
        /// <param name="indentLvl">The count of indentation levels the <see cref="SyntaxNode"/> should be indented by</param>
        /// <param name="childIndentLvl">The count of indentation levels the direct children of this <see cref="SyntaxNode"/> should be indented by</param>
        /// <param name="indentCharCnt">The count of indentation characters for the current <see cref="SyntaxNode"/> (how many times should the <see cref="SyntaxNode.IndentationCharacter"/> be repeated for the current level)</param>
        /// <param name="childIndentCharCnt">The count of indentation characters for the direct childre of this <see cref="SyntaxNode"/> (how many times should the <see cref="SyntaxNode.IndentationCharacter"/> be repeated for the direct child level)</param>

        protected override void ToCSharpString(StringBuilder sb, int indentLvl, int childIndentLvl, int indentCharCnt, int childIndentCharCnt)
        {
            AppendIntentation(sb, indentCharCnt);

            switch (ParameterKind)
            {
                case ParameterKind.Out:
                    sb.Append("out ");
                    break;
                case ParameterKind.Ref:
                    sb.Append("ref ");
                    break;
                case ParameterKind.Params:
                    sb.Append("params ");
                    break;
                case ParameterKind.Ordinary:
                default:
                    break;
            }

            sb.Append(_type);

            sb.Append(' ');

            sb.Append(_name);

            var defaultValue = DefaultValue;
            if (defaultValue != null)
            {
                var isStringType = _type!.Equals("string", StringComparison.InvariantCultureIgnoreCase) || _type.Equals("system.string", StringComparison.InvariantCultureIgnoreCase);

                if (isStringType)
                {
                    if (!StringIsInParenthesisCSharp(defaultValue))
                    {
                        defaultValue = defaultValue.Replace("\"", "\\\"");

                        defaultValue = $"\"{defaultValue}\"";
                    }

                    sb.Append(" = ");
                    sb.Append(defaultValue);
                }
                else if (!string.Empty.Equals(defaultValue))
                {
                    sb.Append(" = ");
                    sb.Append(defaultValue);
                }
            }
        }
    }
}