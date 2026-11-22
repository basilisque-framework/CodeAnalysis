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
using Basilisque.CodeAnalysis.Syntax;

namespace Basilisque.CodeAnalysis.Tests.Syntax
{
    public class CodeLinesTests
    {
        [Test]
        public async Task InitializedEmpty()
        {
            CodeLines codeLines = new CodeLines();

            var src = codeLines.ToString();

            await Assert.That(src).IsEqualTo(string.Empty);
        }

        [Test]
        public async Task Append_NullToEmptyCodeLinesIsANoOp()
        {
            CodeLines codeLines = new CodeLines();

            codeLines.Add(null!);

            var src = codeLines.ToString();

            await Assert.That(src).IsEqualTo(string.Empty);
        }

        [Test]
        public async Task Append_NullToFilledCodeLinesIsANoOp()
        {
            CodeLines codeLines = new CodeLines();

            codeLines.Add("Test");

            codeLines.Add(null!);

            var src = codeLines.ToString();

            await Assert.That(src).IsEqualTo("Test");
        }

        [Test]
        public async Task Append_SingleWhitespaceLine()
        {
            const string C_WHITESPACE = "   ";

            CodeLines codeLines = new CodeLines();

            codeLines.Add(C_WHITESPACE);

            var src = codeLines.ToString();

            await Assert.That(src).IsEqualTo(C_WHITESPACE);
        }

        [Test]
        public async Task Append_SingleLine()
        {
            CodeLines codeLines = new CodeLines();

            codeLines.Add("MyLine");

            var src = codeLines.ToString();

            await Assert.That(src).IsEqualTo("MyLine");
        }

        [Test]
        public async Task Append_MultipleLinesWithOneCall()
        {
            CodeLines codeLines = new CodeLines();

            codeLines.Add(@"MyLine
MySecondLine");

            var src = codeLines.ToString();

            await Assert.That(src).IsEqualTo(@"MyLine
MySecondLine");
        }

        [Test]
        public async Task Append_MultipleLinesWithMultipleCalls()
        {
            CodeLines codeLines = new CodeLines();

            codeLines.Add(@"MyLine
MySecondLine");

            codeLines.Add("  MyThirdLine");

            codeLines.Add("");

            codeLines.Add(@" MyFourthLine 
MyFifthLine");

            var src = codeLines.ToString();

            await Assert.That(src).IsEqualTo(@"MyLine
MySecondLine
  MyThirdLine

 MyFourthLine 
MyFifthLine");
        }

        [Test]
        public async Task Append_RemovesEmptyFirstLine()
        {
            CodeLines codeLines = new CodeLines();

            codeLines.Add(@"
MyFirstLine
MySecondLine");

            codeLines.Add(@"
MyThirdLine");

            var src = codeLines.ToString();

            await Assert.That(src).IsEqualTo(@"MyFirstLine
MySecondLine
MyThirdLine");
        }

        [Test]
        public async Task Append_RemovesEmptyLastLine()
        {
            CodeLines codeLines = new CodeLines();

            codeLines.Add(@"MyFirstLine
MySecondLine
");

            codeLines.Add(@"MyThirdLine
");

            var src = codeLines.ToString();

            await Assert.That(src).IsEqualTo(@"MyFirstLine
MySecondLine
MyThirdLine");
        }

        [Test]
        public async Task Append_RemovesEmptyFirstAndLastLine()
        {
            CodeLines codeLines = new CodeLines();

            codeLines.Add(@"
    MyFirstLine
    MySecondLine
");

            codeLines.Add(@"
MyThirdLine
");

            var src = codeLines.ToString();

            await Assert.That(src).IsEqualTo(@"    MyFirstLine
    MySecondLine
MyThirdLine");
        }

        [Test]
        public async Task Append_PreservesFirstAndLastLineWithWhitespace()
        {
            CodeLines codeLines = new CodeLines();

            codeLines.Add(@" 
    MyFirstLine
    MySecondLine
 ");

            codeLines.Add(@" 
MyThirdLine
 ");

            var src = codeLines.ToString();

            var expected = string.Join(Environment.NewLine,
                " ", "    MyFirstLine", "    MySecondLine", " ", " ", "MyThirdLine", " ");
            await Assert.That(src).IsEqualTo(expected);
        }

        [Test]
        public async Task Append_RemovesEmptyFirstAndLastLineButPreservesMultipleEmptyLines()
        {
            CodeLines codeLines = new CodeLines();

            codeLines.Add(@"


    MyFirstLine
    MySecondLine

");

            codeLines.Add(@"
MyThirdLine


");

            var src = codeLines.ToString();

            await Assert.That(src).IsEqualTo(@"

    MyFirstLine
    MySecondLine

MyThirdLine

");
        }

        [Test]
        public async Task CanEnumerateAsCodeLinesObject()
        {
            CodeLines codeLines = new CodeLines();

            codeLines.Add(@"Line1
Line2");

            var cnt = 0;
            foreach (var line in codeLines)
            {
                cnt++;
            }

            await Assert.That(cnt).IsEqualTo(2);
        }

        [Test]
        public async Task CanEnumerateCorrectly()
        {
            CodeLines codeLines = new CodeLines();

            codeLines.Add(@"Line1
 Line2");

            var i = 0;
            foreach (var line in codeLines)
            {
                switch (i)
                {
                    case 0:
                        await Assert.That(line).IsEqualTo("Line1");
                        break;
                    case 1:
                        await Assert.That(line).IsEqualTo(" Line2");
                        break;
                    default:
                        Assert.Fail($"CodeLines should contain only 2 lines, but it contains at least '{i}' lines.");
                        break;
                }

                i++;
            }

            await Assert.That(i).IsEqualTo(2);
        }

        [Test]
        public async Task CanEnumerateAsIEnumerableInterface()
        {
            CodeLines codeLines = new CodeLines();

            codeLines.Add(@"Line1
Line2
Line3");

            System.Collections.IEnumerable codeLinesAsEnumerable = codeLines;

            var cnt = 0;
            foreach (var line in codeLinesAsEnumerable)
            {
                cnt++;
            }
            await Assert.That(cnt).IsEqualTo(3);
        }

        [Test]
        public async Task ToString_Indentation2Levels()
        {
            var codeLines = new CodeLines();

            codeLines.Add(@"
if (i > 0)
{
    i--;
}
");

            System.Text.StringBuilder sb = new();

            codeLines.ToString(sb, 2, Language.CSharp);

            var src = sb.ToString();

            await Assert.That(src).IsEqualTo(@"        if (i > 0)
        {
            i--;
        }");
        }

        [Test]
        public async Task Index_CanGetLinesByIndex()
        {
            CodeLines codeLines = new CodeLines();

            codeLines.Add(@"Line1
Line2
MyLine3");

            await Assert.That(codeLines[0]).IsEqualTo("Line1");
            await Assert.That(codeLines[1]).IsEqualTo("Line2");
            await Assert.That(codeLines[2]).IsEqualTo("MyLine3");
        }

        [Test]
        public async Task Index_CanSetSingleLineByIndex()
        {
            CodeLines codeLines = new CodeLines();

            codeLines.Add(@"
Line1
Line2
MyLine3
");

            codeLines[1] = "    AlternativeLine;";

            var src = codeLines.ToString();

            await Assert.That(src).IsEqualTo(@"Line1
    AlternativeLine;
MyLine3");
        }

        [Test]
        public async Task Index_CanSetMultipleLinesByIndex()
        {
            CodeLines codeLines = new CodeLines();

            codeLines.Add(@"
Line1
Line2
MyLine3
");

            codeLines[1] = @"    AlternativeLine;
     AltLin2
AltLin3";

            var src = codeLines.ToString();

            await Assert.That(src).IsEqualTo(@"Line1
    AlternativeLine;
     AltLin2
AltLin3
MyLine3");
        }

        [Test]
        public void Index_SetToNullThrowsException()
        {
            CodeLines codeLines = new CodeLines();

            codeLines.Add(@"
Line1
Line2
MyLine3
");
            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                codeLines[2] = null!;
            });
        }
    }
}
