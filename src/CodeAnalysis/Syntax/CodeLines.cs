using System.Collections;
using System.Text;

namespace Basilisque.CodeAnalysis.Syntax
{
    /// <summary>
    /// Represents a list of code lines
    /// </summary>
    public class CodeLines : SyntaxNode, IEnumerable<string>
    {
        private static string[] _lineSeparators = new string[] { "\r\n", "\r", "\n" };
        private List<string> _lines = new List<string>();

        /// <summary>
        /// Gets or sets the line at a specified index
        /// </summary>
        /// <param name="index">The zero-based index of the line to get or set</param>
        /// <returns>The line at the specified index</returns>
        public string this[int index]
        {
            get
            {
                return _lines[index];
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                var lines = value.Split(_lineSeparators, StringSplitOptions.None);

                _lines[index] = lines[0];

                for (int i = 1; i < lines.Length; i++)
                {
                    _lines.Insert(index + i, lines[i]);
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the code lines
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> for the <see cref="CodeLines"/></returns>
        public IEnumerator<string> GetEnumerator()
        {
            return _lines.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the code lines
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> for the <see cref="CodeLines"/></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Appends the current <see cref="SyntaxNode"/> and its children as C# code to the given <see cref="StringBuilder"/>
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> that the <see cref="SyntaxNode"/> is added to</param>
        /// <param name="indentCnt">The count of indentation levels the <see cref="SyntaxNode"/> should be indented by</param>
        /// <param name="childIndentCnt">The count of indentation levels the direct children of this <see cref="SyntaxNode"/> should be indented by</param>
        /// <param name="indent">A string containing the indentation characters for the current <see cref="SyntaxNode"/> (a string containing the <see cref="SyntaxNode.IndentationCharacter"/> times the <see cref="SyntaxNode.IndentationCharacterCountPerLevel"/>)</param>
        protected override void ToCSharpString(StringBuilder sb, int indentCnt, int childIndentCnt, string indent)
        {
            var isFirstLine = true;

            foreach (var line in _lines)
            {
                if (isFirstLine)
                    isFirstLine = false;
                else
                    sb.AppendLine();

                sb.Append(indent);
                sb.Append(line);
            }
        }

        /// <summary>
        /// Appends source code to the current code lines
        /// </summary>
        /// <param name="sourceCode"></param>
        public void Append(string sourceCode)
        {
            if (sourceCode == null)
                return;

            var lines = sourceCode.Split(_lineSeparators, StringSplitOptions.None);

            var iMax = lines.Length - 1;
            var isMoreThanOneLine = lines.Length > 1;

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                //remove first/last line if emtpy
                if (isMoreThanOneLine && (i == 0 || i == iMax))
                {
                    if (string.IsNullOrEmpty(line))
                        continue;
                }

                _lines.Add(line);
            }
        }

    }
}
