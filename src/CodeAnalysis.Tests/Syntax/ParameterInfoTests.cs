﻿using Basilisque.CodeAnalysis.Syntax;

namespace Basilisque.CodeAnalysis.Tests.Syntax
{
    [TestClass]
    public class ParameterInfoTests
    {
        [TestMethod]
        public void OrdinaryParameter()
        {
            var parameterInfo = new ParameterInfo(ParameterKind.Ordinary, "string", "myParameter");

            var src = parameterInfo.ToString();

            Assert.AreEqual(@"string myParameter", src);
        }

        [TestMethod]
        public void OutParameter()
        {
            var parameterInfo = new ParameterInfo(ParameterKind.Out, "string", "myParameter");

            var src = parameterInfo.ToString();

            Assert.AreEqual(@"out string myParameter", src);
        }

        [TestMethod]
        public void RefParameter()
        {
            var parameterInfo = new ParameterInfo(ParameterKind.Ref, "int", "myIntParameter");

            var src = parameterInfo.ToString();

            Assert.AreEqual(@"ref int myIntParameter", src);
        }

        [TestMethod]
        public void ParamsParameter()
        {
            var parameterInfo = new ParameterInfo(ParameterKind.Params, "int[]", "myIntParameters");

            var src = parameterInfo.ToString();

            Assert.AreEqual(@"params int[] myIntParameters", src);
        }

        [TestMethod]
        public void WithDefaultValue()
        {
            var parameterInfo = new ParameterInfo(ParameterKind.Params, "int", "myIntParameter");

            parameterInfo.DefaultValue = "8";

            var src = parameterInfo.ToString();

            Assert.AreEqual(@"params int myIntParameter = 8", src);
        }

        [DataTestMethod]
        [DataRow("test", "\"test\"", DisplayName = "no parenthesis")]
        [DataRow("\"test", "\"\\\"test\"", DisplayName = "no parenthesis end")]
        [DataRow("@\"test", "\"@\\\"test\"", DisplayName = "no parenthesis end 2")]
        [DataRow("test\"", "\"test\\\"\"", DisplayName = "no parenthesis start")]
        [DataRow("\"test\"", "\"test\"", DisplayName = "with parenthesis")]
        [DataRow("\"te\"st\"", "\"te\"st\"", DisplayName = "with parenthesis -> no replacement")]
        [DataRow("te\"st", "\"te\\\"st\"", DisplayName = "without parenthesis -> replacement")]
        public void WithDefaultValue_StringParenthesis(string defaultValue, string expectedResult)
        {
            var parameterInfo = new ParameterInfo(ParameterKind.Ordinary, "string", "myParameter");

            parameterInfo.DefaultValue = defaultValue;

            var src = parameterInfo.ToString();

            Assert.AreEqual($"string myParameter = {expectedResult}", src);
        }

        [TestMethod]
        public void WithDefaultValue_StringParamWithEmptyString()
        {
            var parameterInfo = new ParameterInfo(ParameterKind.Ordinary, "System.String", "myParameter");

            parameterInfo.DefaultValue = "";

            var src = parameterInfo.ToString();

            Assert.AreEqual($"System.String myParameter = \"\"", src);
        }

        [TestMethod]
        public void WithDefaultValue_NonStringParamWithEmptyString()
        {
            var parameterInfo = new ParameterInfo(ParameterKind.Ordinary, "bool", "myParameter");

            parameterInfo.DefaultValue = "";

            var src = parameterInfo.ToString();

            Assert.AreEqual($"bool myParameter", src);
        }
    }
}
