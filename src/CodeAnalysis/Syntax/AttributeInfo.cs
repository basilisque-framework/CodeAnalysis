/*
   Copyright 2024 Alexander Stärk

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

namespace Basilisque.CodeAnalysis.Syntax;

/// <summary>
/// Represents an attribute for source generation
/// </summary>
public class AttributeInfo : SyntaxNode
{
    private List<AttributeConstructorParameter>? _constructorParameters = null;
    private List<AttributeNamedParameter>? _namedParameters = null;

    /// <summary>
    /// The name of the attribute including the namespace if necessary
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Determines if the attribute has <see cref="ConstructorParameters"/>
    /// </summary>
    public bool HasConstructorParameters { get { return _constructorParameters?.Any() == true; } }

    /// <summary>
    /// Determines if the attribute has <see cref="NamedParameters"/>
    /// </summary>
    public bool HasNamedParameters { get { return _namedParameters?.Any() == true; } }

    /// <summary>
    /// Defines the constructor parameters of the attribute
    /// </summary>
    public List<AttributeConstructorParameter> ConstructorParameters
    {
        get
        {
            if (_constructorParameters is null)
                _constructorParameters = new List<AttributeConstructorParameter>();

            return _constructorParameters;
        }
    }

    /// <summary>
    /// Defines the named parameters of the attribute
    /// </summary>
    public List<AttributeNamedParameter> NamedParameters
    {
        get
        {
            if (_namedParameters is null)
                _namedParameters = new List<AttributeNamedParameter>();

            return _namedParameters;
        }
    }

    /// <summary>
    /// Creates a new <see cref="AttributeInfo"/>
    /// </summary>
    /// <param name="name">The name of the attribute including the namespace if necessary</param>
    /// <exception cref="ArgumentNullException">Throws an <see cref="ArgumentNullException"/> when the name parameter is null or an empty string</exception>
    public AttributeInfo(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        Name = name;
    }

    /// <inheritdoc />
    protected override void ToCSharpString(StringBuilder sb, int indentLvl, int childIndentLvl, int indentCharCnt, int childIndentCharCnt)
    {
        AppendIntentation(sb, indentCharCnt);

        sb.Append('[');

        sb.Append(Name);

        var hasConstructorParameters = HasConstructorParameters;
        var hasNamedParameters = HasNamedParameters;

        if (hasConstructorParameters || hasNamedParameters)
            sb.Append('(');

        var isFirstParameter = true;
        if (hasConstructorParameters)
            isFirstParameter = appendConstructorParameters(sb);

        if (hasNamedParameters)
            appendNamedParameters(sb, isFirstParameter);

        if (hasConstructorParameters || hasNamedParameters)
            sb.Append(")");

        sb.Append("]");
    }

    private bool appendConstructorParameters(StringBuilder sb)
    {
        var isFirstParameter = true;

        foreach (var parameter in ConstructorParameters)
        {
            if (isFirstParameter)
                isFirstParameter = false;
            else
                sb.Append(", ");

            if (!string.IsNullOrWhiteSpace(parameter.Name))
            {
                sb.Append(parameter.Name);
                sb.Append(": ");
            }

            sb.Append(parameter.Value);
        }

        return isFirstParameter;
    }

    private void appendNamedParameters(StringBuilder sb, bool isFirstParameter)
    {
        foreach (var parameter in NamedParameters)
        {
            if (isFirstParameter)
                isFirstParameter = false;
            else
                sb.Append(", ");

            sb.Append(parameter.Name);
            sb.Append(" = ");

            sb.Append(parameter.Value);
        }
    }
}
