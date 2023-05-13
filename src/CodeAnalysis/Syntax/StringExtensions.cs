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
using Microsoft.CodeAnalysis.CSharp;

#if BASILISQUE_BENCHMARK
namespace Basilisque.CodeAnalysis.Syntax.Benchmark
#else
namespace Basilisque.CodeAnalysis.Syntax
#endif
{
    /// <summary>
    /// Extension methods for <see cref="string"/> regarding CodeAnalysis.Syntax
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Checks if the given string is a reserved keyword
        /// </summary>
        /// <param name="s">The string to check</param>
        /// <returns>A bool stating if the given string is a keyword or not</returns>
        public static bool IsReservedKeyword(this string? s)
        {
            return SyntaxFacts.IsReservedKeyword(SyntaxFacts.GetKeywordKind(s!));
        }

        /// <summary>
        /// Converts a string to a valid C# namespace
        /// </summary>
        /// <param name="source">The string that should be converted</param>
        /// <returns>A string containing a valid namespace or null</returns>
        public static string? ToValidNamespace(this string? source)
        {
            var result = toValidIdentifier(source, c => c == '.' || SyntaxFacts.IsIdentifierPartCharacter(c));

            if (result == null)
                return result;

            var parts = result.Split('.');

            if (parts.Length > 1)
            {
                char[] resultArr = new char[result.Length + parts.Length];
                var targetIdx = 0;

                var lastIdx = parts.Length - 1;
                for (int i = 0; i < parts.Length; i++)
                {
                    var part = parts[i];

                    if (part.IsReservedKeyword())
                    {
                        resultArr[targetIdx] = '@';
                        targetIdx++;
                    }

                    foreach (var c in part)
                    {
                        resultArr[targetIdx] = c;
                        targetIdx++;
                    }

                    if (i < lastIdx)
                    {
                        resultArr[targetIdx] = '.';
                        targetIdx++;
                    }
                }

                return new string(resultArr, 0, targetIdx);
            }
            else
                return result.IsReservedKeyword() ? "@" + result : result;
        }

        /// <summary>
        /// Converts a string to a valid C# identifier
        /// </summary>
        /// <param name="source">The string that should be converted</param>
        /// <returns>A string containing a valid identifier or null</returns>
        public static string? ToValidIdentifier(this string? source)
        {
            var result = toValidIdentifier(source, SyntaxFacts.IsIdentifierPartCharacter);

            return result.IsReservedKeyword() ? "@" + result : result;
        }

        private static string? toValidIdentifier(string? source, Func<char, bool> isIdentifierPartCharacter)
        {
            if (string.IsNullOrEmpty(source))
                return null;

            int sourceLength = source!.Length;

            char[] resultArr;
            int targetIdx;
            var currentChar = source[0];
            var isStartChar = SyntaxFacts.IsIdentifierStartCharacter(currentChar);
            if (isStartChar || !isIdentifierPartCharacter(currentChar))
            {
                resultArr = new char[sourceLength];
                targetIdx = 0;

                if (!isStartChar)
                    currentChar = '_';
            }
            else
            {
                resultArr = new char[sourceLength + 1];
                resultArr[0] = '_';
                targetIdx = 1;
            }

            resultArr[targetIdx] = currentChar;

            for (int srcIdx = 1; srcIdx < sourceLength; srcIdx++)
            {
                targetIdx++;

                currentChar = source[srcIdx];
                if (isIdentifierPartCharacter(currentChar))
                    resultArr[targetIdx] = currentChar;
                else
                    resultArr[targetIdx] = '_';
            }

            return new string(resultArr);
        }

    }
}
