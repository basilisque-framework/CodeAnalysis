using System.Text;

namespace Basilisque.CodeAnalysis.Syntax
{
    /// <summary>
    /// A common base class for all syntax nodes that will be used for source generation
    /// </summary>
    public abstract class SyntaxNode
    {
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
        /// <param name="indentationCount" example="0">The count of indentation levels for this <see cref="SyntaxNode"/></param>
        /// <param name="language">The <see cref="Language"/> that is used to generate the code</param>
        /// <exception cref="ArgumentNullException">Thrown when the given <paramref name="stringBuilder"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the given <paramref name="indentationCount"/> is less or equal to 0</exception>
        public void ToString(StringBuilder stringBuilder, int indentationCount, Language language)
        {
            if (stringBuilder == null)
                throw new ArgumentNullException(nameof(stringBuilder));

            if (indentationCount < 0)
                throw new ArgumentOutOfRangeException(nameof(indentationCount));

            var childindentationCount = indentationCount + 1;
            var indentation = GetIndentation(indentationCount);

            switch (language)
            {
                case Language.CSharp:
                default:
                    ToCSharpString(stringBuilder, indentationCount, childindentationCount, indentation);
                    break;
            }
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
        /// Appends the current <see cref="SyntaxNode"/> and its children as C# code to the given <see cref="StringBuilder"/>
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> that the <see cref="SyntaxNode"/> is added to</param>
        /// <param name="indentCnt">The count of indentation levels the <see cref="SyntaxNode"/> should be indented by</param>
        /// <param name="childIndentCnt">The count of indentation levels the direct children of this <see cref="SyntaxNode"/> should be indented by</param>
        /// <param name="indent">A string containing the indentation characters for the current <see cref="SyntaxNode"/> (a string containing the <see cref="SyntaxNode.IndentationCharacter"/> times the <see cref="SyntaxNode.IndentationCharacterCountPerLevel"/>)</param>
        protected abstract void ToCSharpString(StringBuilder sb, int indentCnt, int childIndentCnt, string indent);
    }
}
