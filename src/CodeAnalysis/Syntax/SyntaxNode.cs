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
    /// A common base class for all syntax nodes that will be used for source generation
    /// </summary>
    public abstract class SyntaxNode
    {
        /// <summary>
        /// A list of line separators that can be used to split strings by lines
        /// </summary>
        internal static readonly string[] LineSeparators = new string[] { "\r\n", "\r", "\n" };

        /// <summary>
        /// The <see cref="char"/> that is used for indentation
        /// </summary>
        protected static char IndentationCharacter { get { return ' '; } }

        /// <summary>
        /// The count how many times the <see cref="IndentationCharacter"/> is repeated for each indentation level
        /// </summary>
        protected static int IndentationCharacterCountPerLevel { get { return 4; } }

        /// <summary>
        /// Converts the current <see cref="SyntaxNode"/> and its children to C# code
        /// </summary>
        /// <returns>Returns a code representation of the current <see cref="SyntaxNode"/> and its children as string</returns>
        public override string ToString()
        {
            return ToString(Language.CSharp);
        }

        /// <summary>
        /// Converts the current <see cref="SyntaxNode"/> and its children to code in the specified <see cref="Language"/>
        /// </summary>
        /// <param name="language">The <see cref="Language"/> that is used to generate the code</param>
        /// <returns>Returns a code representation of the current <see cref="SyntaxNode"/> and its children as string</returns>
        public string ToString(Language language)
        {
            StringBuilder sb = new StringBuilder();

            ToString(sb, 0, language);

            return sb.ToString();
        }

        /// <summary>
        /// Appends the current <see cref="SyntaxNode"/> and its children as code in the specified <see cref="Language"/> to the given <see cref="StringBuilder"/>
        /// </summary>
        /// <param name="stringBuilder">The <see cref="StringBuilder"/> that the <see cref="SyntaxNode"/> is added to</param>
        /// <param name="indentationLevel" example="0">The count of indentation levels for this <see cref="SyntaxNode"/></param>
        /// <param name="language">The <see cref="Language"/> that is used to generate the code</param>
        /// <exception cref="ArgumentNullException">Thrown when the given <paramref name="stringBuilder"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the given <paramref name="indentationLevel"/> is less or equal to 0</exception>
        public void ToString(StringBuilder stringBuilder, int indentationLevel, Language language)
        {
            Action<StringBuilder, int, int, int, int> langSpecificToString;
            switch (language)
            {
                case Language.VisualBasic:
                    throw new NotSupportedException("Visual Basic is not supported by this generator.");
                case Language.CSharp:
                default:
                    langSpecificToString = ToCSharpString;
                    break;
            }

            InnerToString(stringBuilder, indentationLevel, language, langSpecificToString);
        }

        /// <summary>
        /// Appends the current <see cref="SyntaxNode"/> and its children as code in the specified <see cref="Language"/> to the given <see cref="StringBuilder"/>
        /// </summary>
        /// <param name="stringBuilder">The <see cref="StringBuilder"/> that the <see cref="SyntaxNode"/> is added to</param>
        /// <param name="indentationLevel" example="0">The count of indentation levels for this <see cref="SyntaxNode"/></param>
        /// <param name="language">The <see cref="Language"/> that is used to generate the code</param>
        /// <param name="langSpecificToString">The language specific action that is called to generate the result string</param>
        /// <exception cref="ArgumentNullException">Thrown when the given <paramref name="stringBuilder"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the given <paramref name="indentationLevel"/> is less or equal to 0</exception>
        protected void InnerToString(StringBuilder stringBuilder, int indentationLevel, Language language, Action<StringBuilder, int, int, int, int> langSpecificToString)
        {
            if (stringBuilder == null)
                throw new ArgumentNullException(nameof(stringBuilder));

            if (indentationLevel < 0)
                throw new ArgumentOutOfRangeException(nameof(indentationLevel));

            var childindentationLevel = indentationLevel + 1;
            var indentCharCnt = indentationLevel * IndentationCharacterCountPerLevel;
            var childIndentCharCnt = childindentationLevel * IndentationCharacterCountPerLevel;

            langSpecificToString(stringBuilder, indentationLevel, childindentationLevel, indentCharCnt, childIndentCharCnt);
        }

        /// <summary>
        /// Checks if a given string is enclosed in parenthesis valid for C#
        /// </summary>
        /// <param name="str">The string that should be checked</param>
        /// <returns>A boolean value if the string is enclosed in parenthesis or not</returns>
        protected static bool StringIsInParenthesisCSharp(string str)
        {
            if (!str.EndsWith("\""))
                return false;

            if (str.StartsWith("\""))
                return true;

            if (str.StartsWith("@\""))
                return true;

            if (str.StartsWith("$\""))
                return true;

            if (str.StartsWith("@$\""))
                return true;

            if (str.StartsWith("$@\""))
                return true;

            return false;
        }

        /// <summary>
        /// Gets the indentation string for a given number of indentation levels
        /// </summary>
        /// <param name="indentationCount">The number of indentation levels</param>
        /// <returns>Returns a string containing the indentation characters for the current <see cref="SyntaxNode"/> (a string containing the <see cref="SyntaxNode.IndentationCharacter"/> times the <see cref="SyntaxNode.IndentationCharacterCountPerLevel"/>)</returns>
        protected string GetIndentation(int indentationCount)
        {
            return new string(IndentationCharacter, indentationCount * IndentationCharacterCountPerLevel);
        }

        /// <summary>
        /// Appends the specified number of copies of the <see cref="IndentationCharacter"/> to the provided <see cref="StringBuilder"/>
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> that the characters are added to</param>
        /// <param name="indentCharCnt">The number of times to append the <see cref="IndentationCharacter"/></param>
        protected void AppendIntentation(StringBuilder sb, int indentCharCnt)
        {
            sb.Append(IndentationCharacter, indentCharCnt);
        }

        /// <summary>
        /// Appends the specified number of copies of the <see cref="IndentationCharacter"/> to the provided <see cref="StringBuilder"/> and adds a linebreak at the end
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> that the characters and the linebreak are added to</param>
        /// <param name="indentCharCnt">The number of times to append the <see cref="IndentationCharacter"/> before the linebreak</param>
        protected void AppendIntentationLine(StringBuilder sb, int indentCharCnt)
        {
            AppendIntentation(sb, indentCharCnt);

            sb.AppendLine();
        }

        /// <summary>
        /// Appends the current <see cref="SyntaxNode"/> and its children as C# code to the given <see cref="StringBuilder"/>
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> that the <see cref="SyntaxNode"/> is added to</param>
        /// <param name="indentLvl">The count of indentation levels the <see cref="SyntaxNode"/> should be indented by</param>
        /// <param name="childIndentLvl">The count of indentation levels the direct children of this <see cref="SyntaxNode"/> should be indented by</param>
        /// <param name="indentCharCnt">The count of indentation characters for the current <see cref="SyntaxNode"/> (how many times should the <see cref="SyntaxNode.IndentationCharacter"/> be repeated for the current level)</param>
        /// <param name="childIndentCharCnt">The count of indentation characters for the direct childre of this <see cref="SyntaxNode"/> (how many times should the <see cref="SyntaxNode.IndentationCharacter"/> be repeated for the direct child level)</param>
        protected abstract void ToCSharpString(StringBuilder sb, int indentLvl, int childIndentLvl, int indentCharCnt, int childIndentCharCnt);
    }
}
