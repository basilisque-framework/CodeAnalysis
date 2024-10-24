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
    /// Represents a property for source generation
    /// </summary>
    public class PropertyInfo : SyntaxNode
    {
        private string _name;
        private string? _type;
        private bool _hasAutoSetter = true;
        private bool _hasAutoGetter = true;
        private CodeLines? _getterBody = null;
        private CodeLines? _setterBody = null;
        private string? _fieldName = null;
        private bool _hasAutoFieldName = true;
        private List<string>? _xmlDocAdditionalLines;
        private List<AttributeInfo>? _attributes = null;

        /// <summary>
        /// The access modifier that specifies the accessibility of the property
        /// </summary>
        /// <example>public</example>
        public AccessModifier AccessModifier { get; set; } = AccessModifier.Public;

        /// <summary>
        /// The type of the property
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
        /// The name of the property
        /// </summary>
        /// <exception cref="ArgumentNullException">Setting the <see cref="Name"/> to null or to an empty string throws an <see cref="ArgumentNullException"/></exception>
        /// <example>MyProperty</example>
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

                if (_hasAutoFieldName)
                    _fieldName = $"_{_name}_{Guid.NewGuid():N}";
            }
        }

        /// <summary>
        /// The name of the field that is used as backing field
        /// </summary>
        public string? FieldName
        {
            get
            {
                return _fieldName;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _hasAutoFieldName = true;
                    _fieldName = $"_{_name}_{Guid.NewGuid():N}";
                }
                else
                {
                    _hasAutoFieldName = false;
                    _fieldName = value;
                }
            }
        }

        /// <summary>
        /// The initial value of the parameter
        /// </summary>
        public string? InitialValue { get; set; }

        /// <summary>
        /// Defines if the property has a getter
        /// </summary>
        public bool HasGetter
        {
            get
            {
                return _hasAutoGetter || _getterBody?.Count > 0;
            }
            set
            {
                if (value != _hasAutoGetter)
                    _hasAutoGetter = value;

                if (!value && _getterBody != null)
                    _getterBody = null;
            }
        }

        /// <summary>
        /// Defines if the property has a setter
        /// </summary>
        public bool HasSetter
        {
            get
            {
                return _hasAutoSetter || _setterBody?.Count > 0;
            }
            set
            {
                if (value != _hasAutoSetter)
                    _hasAutoSetter = value;

                if (!value && _setterBody != null)
                    _setterBody = null;
            }
        }

        /// <summary>
        /// Represents the body of the get method
        /// </summary>
        public CodeLines GetterBody
        {
            get
            {
                if (_getterBody == null)
                    _getterBody = new CodeLines();

                return _getterBody;
            }
        }

        /// <summary>
        /// Represents the body of the set method
        /// </summary>
        public CodeLines SetterBody
        {
            get
            {
                if (_setterBody == null)
                    _setterBody = new CodeLines();

                return _setterBody;
            }
        }

        /// <summary>
        /// When this property is set, and '<inheritdoc />'-tag is generated.
        /// </summary>
        public bool InheritXmlDoc { get; set; } = false;

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
        /// Determines if the property is required or not
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Determines if the property has <see cref="Attributes"/>
        /// </summary>
        public bool HasAttributes { get { return _attributes?.Any() == true; } }

        /// <summary>
        /// Defines the attributes of the property
        /// </summary>
        public List<AttributeInfo> Attributes
        {
            get
            {
                if (_attributes is null)
                    _attributes = new List<AttributeInfo>();

                return _attributes;
            }
        }

        /// <summary>
        /// Creates a new <see cref="PropertyInfo"/>
        /// </summary>
        /// <param name="type">The type of the property as string</param>
        /// <param name="name">The name of the property</param>
        /// <exception cref="ArgumentNullException">Throws an <see cref="ArgumentNullException"/> when the name or the type parameter is null or an empty string</exception>
        public PropertyInfo(string type, string name)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentNullException(nameof(type));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            _name = name;
            _type = type;
            _fieldName = $"_{_name}_{Guid.NewGuid():N}";
        }

        /// <summary>
        /// Converts the current <see cref="SyntaxNode"/> and its children to C# code
        /// </summary>
        /// <param name="includeBackingFieldIfNecessary">Defines, if the backing field of the property is part of the resulting string or not</param>
        /// <returns>Returns a code representation of the current <see cref="SyntaxNode"/> and its children as string</returns>
        public string ToString(bool includeBackingFieldIfNecessary)
        {
            return ToString(Language.CSharp, includeBackingFieldIfNecessary);
        }

        /// <summary>
        /// Converts the current <see cref="SyntaxNode"/> and its children to code in the specified <see cref="Language"/>
        /// </summary>
        /// <param name="language">The <see cref="Language"/> that is used to generate the code</param>
        /// <param name="includeBackingFieldIfNecessary">Defines, if the backing field of the property is part of the resulting string or not</param>
        /// <returns>Returns a code representation of the current <see cref="SyntaxNode"/> and its children as string</returns>
        public string ToString(Language language, bool includeBackingFieldIfNecessary)
        {
            StringBuilder sb = new StringBuilder();

            ToString(sb, 0, language, includeBackingFieldIfNecessary);

            return sb.ToString();
        }

        /// <summary>
        /// Appends the current <see cref="SyntaxNode"/> and its children as code in the specified <see cref="Language"/> to the given <see cref="StringBuilder"/>
        /// </summary>
        /// <param name="stringBuilder">The <see cref="StringBuilder"/> that the <see cref="SyntaxNode"/> is added to</param>
        /// <param name="indentationLevel" example="0">The count of indentation levels for this <see cref="SyntaxNode"/></param>
        /// <param name="language">The <see cref="Language"/> that is used to generate the code</param>
        /// <param name="includeBackingFieldIfNecessary">Defines, if the backing field of the property is part of the resulting string or not</param>
        /// <exception cref="ArgumentNullException">Thrown when the given <paramref name="stringBuilder"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the given <paramref name="indentationLevel"/> is less or equal to 0</exception>
        public void ToString(StringBuilder stringBuilder, int indentationLevel, Language language, bool includeBackingFieldIfNecessary)
        {
            Action<StringBuilder, int, int, int, int> langSpecificToString = (sb, iLvl, cILvl, iCharCnt, cICharCnt) =>
            {
                switch (language)
                {
                    case Language.VisualBasic:
                        throw new NotSupportedException("Visual Basic is not supported by this generator.");
                    case Language.CSharp:
                    default:
                        ToCSharpString(sb, iLvl, cILvl, iCharCnt, cICharCnt, includeBackingFieldIfNecessary);
                        break;
                }
            };

            InnerToString(stringBuilder, indentationLevel, language, langSpecificToString);
        }

        /// <summary>
        /// Appends the current <see cref="PropertyInfo"/> as C# code to the given <see cref="StringBuilder"/>
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> that the <see cref="PropertyInfo"/> is added to</param>
        /// <param name="indentLvl">The count of indentation levels the <see cref="PropertyInfo"/> should be indented by</param>
        /// <param name="childIndentLvl">The count of indentation levels the direct children of this <see cref="PropertyInfo"/> should be indented by</param>
        /// <param name="indentCharCnt">The count of indentation characters for the current <see cref="PropertyInfo"/> (how many times should the <see cref="SyntaxNode.IndentationCharacter"/> be repeated for the current level)</param>
        /// <param name="childIndentCharCnt">The count of indentation characters for the direct childre of this <see cref="PropertyInfo"/> (how many times should the <see cref="SyntaxNode.IndentationCharacter"/> be repeated for the direct child level)</param>
        protected override void ToCSharpString(StringBuilder sb, int indentLvl, int childIndentLvl, int indentCharCnt, int childIndentCharCnt)
        {
            ToCSharpString(sb, indentLvl, childIndentLvl, indentCharCnt, childIndentCharCnt, true);
        }

        /// <summary>
        /// Appends the current <see cref="PropertyInfo"/> as C# code to the given <see cref="StringBuilder"/>
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> that the <see cref="PropertyInfo"/> is added to</param>
        /// <param name="indentLvl">The count of indentation levels the <see cref="PropertyInfo"/> should be indented by</param>
        /// <param name="childIndentLvl">The count of indentation levels the direct children of this <see cref="PropertyInfo"/> should be indented by</param>
        /// <param name="indentCharCnt">The count of indentation characters for the current <see cref="PropertyInfo"/> (how many times should the <see cref="SyntaxNode.IndentationCharacter"/> be repeated for the current level)</param>
        /// <param name="childIndentCharCnt">The count of indentation characters for the direct childre of this <see cref="PropertyInfo"/> (how many times should the <see cref="SyntaxNode.IndentationCharacter"/> be repeated for the direct child level)</param>
        /// <param name="includeBackingFieldIfNecessary">Defines, if the backing field of the property is part of the resulting string when necessary or not</param>
        protected void ToCSharpString(StringBuilder sb, int indentLvl, int childIndentLvl, int indentCharCnt, int childIndentCharCnt, bool includeBackingFieldIfNecessary)
        {
            if (includeBackingFieldIfNecessary)
            {
                if (AppendFieldIfNecessary(sb, indentLvl, Language.CSharp, null!))
                    sb.AppendLine();
            }

            appendXmlDoc(sb, indentCharCnt);

            if (HasAttributes)
            {
                foreach (var a in Attributes)
                {
                    a.ToString(sb, indentLvl, Language.CSharp);
                    sb.AppendLine();
                }
            }

            AppendIntentation(sb, indentCharCnt);

            sb.Append(AccessModifier.ToKeywordString());

            sb.Append(' ');

            if (IsRequired)
                sb.Append("required ");

            sb.Append(_type);

            sb.Append(' ');

            sb.Append(_name);

            bool hasGetterBody = _getterBody?.Count > 0;
            bool hasSetterBody = _setterBody?.Count > 0;

            if (hasGetterBody || hasSetterBody || !_hasAutoFieldName)
            {
                sb.AppendLine();
                AppendIntentation(sb, indentCharCnt);

                sb.AppendLine("{");
                AppendIntentation(sb, childIndentCharCnt);

                writeExtendedProperty(sb, childIndentLvl, childIndentCharCnt, hasGetterBody, hasSetterBody);

                AppendIntentation(sb, indentCharCnt);
                sb.Append("}");
            }
            else
            {
                writeAutoProperty(sb);
            }
        }

        private void writeExtendedProperty(StringBuilder sb, int childIndentLvl, int childIndentCharCnt, bool hasGetterBody, bool hasSetterBody)
        {
            var bodyIndentCharCnt = childIndentCharCnt + IndentationCharacterCountPerLevel;
            var bodyIndentLvl = childIndentLvl + 1;

            var writeGetter = HasGetter;
            var writeSetter = HasSetter;

            if (!writeGetter && !writeSetter)
                writeGetter = writeSetter = true;

            if (writeGetter)
            {
                sb.AppendLine("get");
                AppendIntentation(sb, childIndentCharCnt);
                sb.AppendLine("{");

                if (hasGetterBody)
                {
                    _getterBody!.ToString(sb, bodyIndentLvl, Language.CSharp);
                }
                else
                {
                    AppendIntentation(sb, bodyIndentCharCnt);
                    sb.Append("return this.");
                    sb.Append(_fieldName);
                    sb.Append(";");
                }

                sb.AppendLine();
                AppendIntentation(sb, childIndentCharCnt);
                sb.AppendLine("}");
            }

            if (writeSetter)
            {
                if (writeGetter)
                    AppendIntentation(sb, childIndentCharCnt);

                sb.AppendLine("set");
                AppendIntentation(sb, childIndentCharCnt);
                sb.AppendLine("{");

                if (hasSetterBody)
                {
                    _setterBody!.ToString(sb, bodyIndentLvl, Language.CSharp);
                }
                else
                {
                    AppendIntentation(sb, bodyIndentCharCnt);
                    sb.Append("if (value != this.");
                    sb.Append(_fieldName);
                    sb.AppendLine(")");

                    AppendIntentation(sb, bodyIndentCharCnt);
                    sb.AppendLine("{");

                    var ifBodyIndentCharCnt = bodyIndentCharCnt + IndentationCharacterCountPerLevel;
                    AppendIntentation(sb, ifBodyIndentCharCnt);
                    sb.Append("this.");
                    sb.Append(_fieldName);
                    sb.AppendLine(" = value;");

                    AppendIntentation(sb, bodyIndentCharCnt);
                    sb.Append("}");
                }

                sb.AppendLine();
                AppendIntentation(sb, childIndentCharCnt);
                sb.AppendLine("}");
            }
        }

        private void writeAutoProperty(StringBuilder sb)
        {
            sb.Append(" {");

            bool hasGetter = HasGetter;
            bool hasSetter = HasSetter;

            if (hasGetter || !hasSetter)
                sb.Append(" get;");

            if (hasSetter || !hasGetter)
                sb.Append(" set;");

            sb.Append(" }");

            writeInitialValue(sb);
        }

        private bool writeInitialValue(StringBuilder sb)
        {
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
                    sb.Append(';');

                    return true;
                }
                else if (!string.Empty.Equals(initialValue))
                {
                    sb.Append(" = ");
                    sb.Append(initialValue);
                    sb.Append(';');

                    return true;
                }
            }

            return false;
        }

        private void appendXmlDoc(StringBuilder sb, int indentCharCnt)
        {
            if (InheritXmlDoc)
            {
                AppendIntentation(sb, indentCharCnt);
                sb.AppendLine("/// <inheritdoc />");
                return;
            }

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

        /// <summary>
        /// Appends the backing field for the property to the given <see cref="StringBuilder"/> when there is not already a field with the same name in the given <see cref="List{FieldInfo}"/>
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> that the field is appended to</param>
        /// <param name="indentLvl">The indentation level that the field is indeted by</param>
        /// <param name="language">The <see cref="Language"/> that the field is generated in</param>
        /// <param name="fields">The list of fields that is used to check if the needed field already exists or not</param>
        /// <returns>A <see cref="bool"/> value that states if a field was added or not</returns>
        public bool AppendFieldIfNecessary(StringBuilder sb, int indentLvl, Language language, List<FieldInfo>? fields)
        {
            if (sb == null)
                return false;

            if (language != Language.CSharp)
                throw new NotImplementedException("Currently only C# is supported");

            bool hasGetterBody = _getterBody?.Count > 0;
            bool hasSetterBody = _setterBody?.Count > 0;

            if (hasGetterBody || hasSetterBody || !_hasAutoFieldName)
            {
                var writeGetter = HasGetter;
                var writeSetter = HasSetter;

                if (!writeGetter && !writeSetter)
                    writeGetter = writeSetter = true;

                if ((writeGetter && !hasGetterBody) || (writeSetter && !hasSetterBody))
                {
                    if (fields?.Any(fi => fi.Name == _fieldName) != true)
                    {
                        AppendIntentation(sb, indentLvl * IndentationCharacterCountPerLevel);

                        sb.Append("private ");

                        sb.Append(_type);

                        sb.Append(' ');

                        sb.Append(_fieldName);

                        if (!writeInitialValue(sb))
                            sb.Append(";");

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
