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
    [TestClass]
    public class CodeLinesTests
    {
        [TestMethod]
        public void InitializedEmpty()
        {
            CodeLines codeLines = new CodeLines();

            var src = codeLines.ToString();

            Assert.AreEqual(string.Empty, src);
        }

        [TestMethod]
        public void Append_NullToEmptyCodeLinesIsANoOp()
        {
            CodeLines codeLines = new CodeLines();

            codeLines.Add(null!);

            var src = codeLines.ToString();

            Assert.AreEqual(string.Empty, src);
        }

        [TestMethod]
        public void Append_NullToFilledCodeLinesIsANoOp()
        {
            CodeLines codeLines = new CodeLines();

            codeLines.Add("Test");

            codeLines.Add(null!);

            var src = codeLines.ToString();

            Assert.AreEqual("Test", src);
        }

        [TestMethod]
        public void Append_SingleWhitespaceLine()
        {
            const string C_WHITESPACE = "   ";

            CodeLines codeLines = new CodeLines();

            codeLines.Add(C_WHITESPACE);

            var src = codeLines.ToString();

            Assert.AreEqual(C_WHITESPACE, src);
        }

        [TestMethod]
        public void Append_SingleLine()
        {
            CodeLines codeLines = new CodeLines();

            codeLines.Add("MyLine");

            var src = codeLines.ToString();

            Assert.AreEqual("MyLine", src);
        }

        [TestMethod]
        public void Append_MultipleLinesWithOneCall()
        {
            CodeLines codeLines = new CodeLines();

            codeLines.Add(@"MyLine
MySecondLine");

            var src = codeLines.ToString();

            Assert.AreEqual(@"MyLine
MySecondLine", src);
        }

        [TestMethod]
        public void Append_MultipleLinesWithMultipleCalls()
        {
            CodeLines codeLines = new CodeLines();

            codeLines.Add(@"MyLine
MySecondLine");

            codeLines.Add("  MyThirdLine");

            codeLines.Add("");

            codeLines.Add(@" MyFourthLine 
MyFifthLine");

            var src = codeLines.ToString();

            Assert.AreEqual(@"MyLine
MySecondLine
  MyThirdLine

 MyFourthLine 
MyFifthLine", src);
        }

        [TestMethod]
        public void Append_RemovesEmptyFirstLine()
        {
            CodeLines codeLines = new CodeLines();

            codeLines.Add(@"
MyFirstLine
MySecondLine");

            codeLines.Add(@"
MyThirdLine");

            var src = codeLines.ToString();

            Assert.AreEqual(@"MyFirstLine
MySecondLine
MyThirdLine", src);
        }

        [TestMethod]
        public void Append_RemovesEmptyLastLine()
        {
            CodeLines codeLines = new CodeLines();

            codeLines.Add(@"MyFirstLine
MySecondLine
");

            codeLines.Add(@"MyThirdLine
");

            var src = codeLines.ToString();

            Assert.AreEqual(@"MyFirstLine
MySecondLine
MyThirdLine", src);
        }

        [TestMethod]
        public void Append_RemovesEmptyFirstAndLastLine()
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

            Assert.AreEqual(@"    MyFirstLine
    MySecondLine
MyThirdLine", src);
        }

        [TestMethod]
        public void Append_PreservesFirstAndLastLineWithWhitespace()
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

            Assert.AreEqual(@" 
    MyFirstLine
    MySecondLine
 
 
MyThirdLine
 ", src);
        }

        [TestMethod]
        public void Append_RemovesEmptyFirstAndLastLineButPreservesMultipleEmptyLines()
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

            Assert.AreEqual(@"

    MyFirstLine
    MySecondLine

MyThirdLine

", src);
        }

        [TestMethod]
        public void CanEnumerateAsCodeLinesObject()
        {
            CodeLines codeLines = new CodeLines();

            codeLines.Add(@"Line1
Line2");

            var cnt = 0;
            foreach (var line in codeLines)
            {
                cnt++;
            }

            Assert.AreEqual(2, cnt);
        }

        [TestMethod]
        public void CanEnumerateCorrectly()
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
                        Assert.AreEqual("Line1", line);
                        break;
                    case 1:
                        Assert.AreEqual(" Line2", line);
                        break;
                    default:
                        Assert.Fail($"CodeLines should contain only 2 lines, but it contains at least '{i}' lines.");
                        break;
                }

                i++;
            }

            Assert.AreEqual(2, i);
        }

        [TestMethod]
        public void CanEnumerateAsIEnumerableInterface()
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
            Assert.AreEqual(3, cnt);
        }

        [TestMethod]
        public void ToString_Indentation2Levels()
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

            Assert.AreEqual(@"        if (i > 0)
        {
            i--;
        }", src);
        }

        [TestMethod]
        public void Index_CanGetLinesByIndex()
        {
            CodeLines codeLines = new CodeLines();

            codeLines.Add(@"Line1
Line2
MyLine3");

            Assert.AreEqual("Line1", codeLines[0]);
            Assert.AreEqual("Line2", codeLines[1]);
            Assert.AreEqual("MyLine3", codeLines[2]);
        }

        [TestMethod]
        public void Index_CanSetSingleLineByIndex()
        {
            CodeLines codeLines = new CodeLines();

            codeLines.Add(@"
Line1
Line2
MyLine3
");

            codeLines[1] = "    AlternativeLine;";

            var src = codeLines.ToString();

            Assert.AreEqual(@"Line1
    AlternativeLine;
MyLine3", src);
        }

        [TestMethod]
        public void Index_CanSetMultipleLinesByIndex()
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

            Assert.AreEqual(@"Line1
    AlternativeLine;
     AltLin2
AltLin3
MyLine3", src);
        }

        [TestMethod]
        public void Index_SetToNullThrowsException()
        {
            CodeLines codeLines = new CodeLines();

            codeLines.Add(@"
Line1
Line2
MyLine3
");
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                codeLines[2] = null!;
            });
        }
    }
}
