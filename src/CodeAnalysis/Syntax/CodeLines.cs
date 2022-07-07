using System.Collections;
using System.Text;

namespace Basilisque.CodeAnalysis.Syntax
{
    /// <summary>
    /// Represents a list of code lines
    /// </summary>
    public class CodeLines : SyntaxNode, IEnumerable<string>
    {
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

                var lines = value.Split(LineSeparators, StringSplitOptions.None);

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
        /// Appends the current lines of code and its children as C# code to the given <see cref="StringBuilder"/>
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> that the lines of code are added to</param>
        /// <param name="indentLvl">The count of indentation levels the lines of code should be indented by</param>
        /// <param name="childIndentLvl">The count of indentation levels the direct children of this <see cref="SyntaxNode"/> should be indented by</param>
        /// <param name="indentCharCnt">The count of indentation characters for the current lines of code (how many times should the <see cref="SyntaxNode.IndentationCharacter"/> be repeated for the current level)</param>
        /// <param name="childIndentCharCnt">The count of indentation characters for the direct childre of this <see cref="SyntaxNode"/> (how many times should the <see cref="SyntaxNode.IndentationCharacter"/> be repeated for the direct child level)</param>
        protected override void ToCSharpString(StringBuilder sb, int indentLvl, int childIndentLvl, int indentCharCnt, int childIndentCharCnt)
        {
            var isFirstLine = true;

            foreach (var line in _lines)
            {
                if (isFirstLine)
                    isFirstLine = false;
                else
                    sb.AppendLine();

                AppendIntentation(sb, indentCharCnt);
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

            var lines = sourceCode.Split(LineSeparators, StringSplitOptions.None);

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
