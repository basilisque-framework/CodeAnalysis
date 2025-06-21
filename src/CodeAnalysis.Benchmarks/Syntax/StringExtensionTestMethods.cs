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
using Basilisque.CodeAnalysis.Syntax.Benchmark;
using Microsoft.CodeAnalysis.CSharp;
using System.Text;

namespace Basilisque.CodeAnalysis.Benchmarks.Syntax
{
    /// <summary>
    /// Even before implementation it was pretty clear in what direction the implementation had to go.
    /// But I wanted to see the difference between a very naive implementation and a more thoughtful implementation in hard numbers.
    /// In addition to that I wanted to try out BenchmarkDotNet.
    /// I'm sure the real implementation could be further optimized. But at the moment I don't see the point why I would invest time in doing that.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S3776:Cognitive Complexity of methods should not be too high", Justification = "This is just benchmark code")]
    public static class StringExtensionTestMethods
    {
        /// <summary>
        /// A very naive implementation using a StringBuilder
        /// </summary>
        public static string? ToValidIdentifier_BasicImplementationStringBuilder(this string? source, Func<char, bool> isIdentifierPartCharacter)
        {
            if (string.IsNullOrEmpty(source))
                return null;

            if (SyntaxFacts.IsValidIdentifier(source))
                return source.IsReservedKeyword() ? "@" + source : source;

            StringBuilder stringBuilder = new StringBuilder();

            var currentChar = source[0];
            if (!SyntaxFacts.IsIdentifierStartCharacter(currentChar))
            {
                stringBuilder.Append('_');

                if (isIdentifierPartCharacter(currentChar))
                    stringBuilder.Append(currentChar);
            }
            else
                stringBuilder.Append(currentChar);

            int sourceLength = source.Length;
            for (int i = 1; i < sourceLength; i++)
            {
                currentChar = source[i];
                if (isIdentifierPartCharacter(currentChar))
                    stringBuilder.Append(currentChar);
                else
                    stringBuilder.Append('_');
            }

            var result = stringBuilder.ToString();



            var parts = result.Split('.');

            if (parts.Length > 1)
            {
                stringBuilder.Clear();

                var lastIdx = parts.Length - 1;
                for (int i = 0; i < parts.Length; i++)
                {
                    var part = parts[i];

                    if (part.IsReservedKeyword())
                        stringBuilder.Append('@');

                    stringBuilder.Append(part);

                    if (i < lastIdx)
                        stringBuilder.Append('.');
                }

                return stringBuilder.ToString();
            }
            else
                return result.IsReservedKeyword() ? "@" + result : result;
        }

        /// <summary>
        /// This implementation uses a char[] instead of a StringBuilder.
        /// Apart from that, it does the same.
        /// </summary>
        public static string? ToValidIdentifier_WithCharArray(this string? source, Func<char, bool> isIdentifierPartCharacter)
        {
            if (string.IsNullOrEmpty(source))
                return null;

            if (SyntaxFacts.IsValidIdentifier(source))
                return source.IsReservedKeyword() ? "@" + source : source;

            int sourceLength = source.Length;

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

            var result = new string(resultArr);



            var parts = result.Split('.');

            if (parts.Length > 1)
            {
                resultArr = new char[resultArr.Length + parts.Length];
                targetIdx = 0;

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

                return new string(resultArr.AsSpan(0, targetIdx));
            }
            else
                return result.IsReservedKeyword() ? "@" + result : result;
        }

        /// <summary>
        /// Initially the call to SyntaxFacts.IsValidIdentifier was there to save instantiating the StringBuilder and so on.
        /// But most of the time we're probably better off with saving that method call
        /// </summary>
        public static string? ToValidIdentifier_WithoutIsValidIdentifierCall(this string? source, Func<char, bool> isIdentifierPartCharacter)
        {
            if (string.IsNullOrEmpty(source))
                return null;

            int sourceLength = source.Length;

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

            var result = new string(resultArr);



            var parts = result.Split('.');

            if (parts.Length > 1)
            {
                resultArr = new char[resultArr.Length + parts.Length];
                targetIdx = 0;

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

                return new string(resultArr.AsSpan(0, targetIdx));
            }
            else
                return result.IsReservedKeyword() ? "@" + result : result;
        }
    }
}
