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
    public class ParameterInfoTests
    {
        [Test]
        public async Task OrdinaryParameter()
        {
            var parameterInfo = new ParameterInfo(ParameterKind.Ordinary, "string", "myParameter");

            var src = parameterInfo.ToString();

            await Assert.That(src).IsEqualTo(@"string myParameter");
        }

        [Test]
        public async Task OutParameter()
        {
            var parameterInfo = new ParameterInfo(ParameterKind.Out, "string", "myParameter");

            var src = parameterInfo.ToString();

            await Assert.That(src).IsEqualTo(@"out string myParameter");
        }

        [Test]
        public async Task RefParameter()
        {
            var parameterInfo = new ParameterInfo(ParameterKind.Ref, "int", "myIntParameter");

            var src = parameterInfo.ToString();

            await Assert.That(src).IsEqualTo(@"ref int myIntParameter");
        }

        [Test]
        public async Task ParamsParameter()
        {
            var parameterInfo = new ParameterInfo(ParameterKind.Params, "int[]", "myIntParameters");

            var src = parameterInfo.ToString();

            await Assert.That(src).IsEqualTo(@"params int[] myIntParameters");
        }

        [Test]
        public async Task WithDefaultValue()
        {
            var parameterInfo = new ParameterInfo(ParameterKind.Params, "int", "myIntParameter");

            parameterInfo.DefaultValue = "8";

            var src = parameterInfo.ToString();

            await Assert.That(src).IsEqualTo(@"params int myIntParameter = 8");
        }

        [Test]
        [Arguments("test", "\"test\"", DisplayName = "no parenthesis")]
        [Arguments("\"test", "\"\\\"test\"", DisplayName = "no parenthesis end")]
        [Arguments("@\"test", "\"@\\\"test\"", DisplayName = "no parenthesis end 2")]
        [Arguments("test\"", "\"test\\\"\"", DisplayName = "no parenthesis start")]
        [Arguments("\"test\"", "\"test\"", DisplayName = "with parenthesis")]
        [Arguments("\"te\"st\"", "\"te\"st\"", DisplayName = "with parenthesis -> no replacement")]
        [Arguments("te\"st", "\"te\\\"st\"", DisplayName = "without parenthesis -> replacement")]
        public async Task WithDefaultValue_StringParenthesis(string defaultValue, string expectedResult)
        {
            var parameterInfo = new ParameterInfo(ParameterKind.Ordinary, "string", "myParameter");

            parameterInfo.DefaultValue = defaultValue;

            var src = parameterInfo.ToString();

            await Assert.That(src).IsEqualTo($"string myParameter = {expectedResult}");
        }

        [Test]
        public async Task WithDefaultValue_StringParamWithEmptyString()
        {
            var parameterInfo = new ParameterInfo(ParameterKind.Ordinary, "System.String", "myParameter");

            parameterInfo.DefaultValue = "";

            var src = parameterInfo.ToString();

            await Assert.That(src).IsEqualTo($"System.String myParameter = \"\"");
        }

        [Test]
        public async Task WithDefaultValue_NonStringParamWithEmptyString()
        {
            var parameterInfo = new ParameterInfo(ParameterKind.Ordinary, "bool", "myParameter");

            parameterInfo.DefaultValue = "";

            var src = parameterInfo.ToString();

            await Assert.That(src).IsEqualTo($"bool myParameter");
        }

        [Test]
        [Arguments(null, DisplayName = "Name = null")]
        [Arguments("", DisplayName = "Name = \"\"")]
        [Arguments("   ", DisplayName = "Name = \"   \"")]
        public void EmptyName_ThrowsException(string? name)
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new ParameterInfo(ParameterKind.Ordinary, "type", name!));
        }

        [Test]
        [Arguments(null, DisplayName = "Type = null")]
        [Arguments("", DisplayName = "Type = \"\"")]
        [Arguments("   ", DisplayName = "Type = \"   \"")]
        public void EmptyType_ThrowsException(string? type)
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new ParameterInfo(ParameterKind.Ordinary, type!, "name"));
        }

        [Test]
        public async Task CanChangeName()
        {
            var parameterInfo = new ParameterInfo(ParameterKind.Ordinary, "bool", "myParameter");

            await Assert.That(parameterInfo.Name).IsEqualTo("myParameter");

            parameterInfo.Name = "myParameter2";

            await Assert.That(parameterInfo.Name).IsEqualTo("myParameter2");
        }

        [Test]
        public async Task CanChangeType()
        {
            var parameterInfo = new ParameterInfo(ParameterKind.Ordinary, "bool", "myParameter");

            await Assert.That(parameterInfo.Type).IsEqualTo("bool");

            parameterInfo.Type = "bool2";

            await Assert.That(parameterInfo.Type).IsEqualTo("bool2");
        }
    }
}
