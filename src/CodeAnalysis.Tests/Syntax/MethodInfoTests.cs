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
    public class MethodInfoTests
    {
        [Test]
        public async Task ConstructorInitializesParameters()
        {
            var methodInfo = new MethodInfo(AccessModifier.Public, "string", "MyMethod1");

            var src = methodInfo.ToString();

            await Assert.That(src).IsEqualTo(@"public string MyMethod1()
{
}");
        }

        [Test]
        [Arguments(null, "MyMethod1Async", "async ", DisplayName = "returnType = null, method = async")]
        [Arguments("", "MyMethod1Async", "async ", DisplayName = "returnType = \"\", method = async")]
        [Arguments("  ", "MyMethod1Async", "async ", DisplayName = "returnType = \"  \", method = async")]
        [Arguments("void", "MyMethod1Async", "async ", DisplayName = "returnType = void method = async")]
        [Arguments("Task", "MyMethod1Async", "async ", DisplayName = "returnType = Task, method = async")]
        [Arguments("Task<string>", "MyMethod1Async", "async ", DisplayName = "returnType = Task<string>, method = async")]
        [Arguments("System.Threading.Tasks.Task", "MyMethod1Async", "async ", DisplayName = "returnType = System.Threading.Tasks.Task, method = async")]
        [Arguments("System.Threading.Tasks.Task<string>", "MyMethod1Async", "async ", DisplayName = "returnType = System.Threading.Tasks.Task<string>, method = async")]
        [Arguments("string", "MyMethod1Async", "", DisplayName = "returnType = string, method = async")]
        [Arguments("Task", "MyMethod1", "", DisplayName = "returnType = Task, method = sync")]
        public async Task Constructor_DoesInitializeAsync(string? returnType, string methodName, string isAsyncResult)
        {
            var methodInfo = new MethodInfo(AccessModifier.Public, returnType!, methodName);

            var src = methodInfo.ToString();

            if (string.IsNullOrWhiteSpace(returnType))
                returnType = "void";

            await Assert.That(src).IsEqualTo(@$"public {isAsyncResult}{returnType} {methodName}()
{{
}}");
        }

        [Test]
        [Arguments(null, DisplayName = "method name = null")]
        [Arguments("", DisplayName = "method name = empty")]
        [Arguments("  ", DisplayName = "method name = whitespace")]
        public void Constructor_EmptyMethodNameThrows(string? methodName)
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                var methodInfo = new MethodInfo(AccessModifier.Public, "void", methodName!);
            });
        }

        [Test]
        public async Task MethodName_CanBeChanged()
        {
            var methodInfo = new MethodInfo(AccessModifier.PrivateProtected, "string", "ConstructorMethodName");

            await Assert.That(methodInfo.Name).IsEqualTo("ConstructorMethodName");

            var src = methodInfo.ToString();
            await Assert.That(src).IsEqualTo(@"private protected string ConstructorMethodName()
{
}");

            methodInfo.Name = "ChangedMethodName";

            await Assert.That(methodInfo.Name).IsEqualTo("ChangedMethodName");

            src = methodInfo.ToString();
            await Assert.That(src).IsEqualTo(@"private protected string ChangedMethodName()
{
}");
        }

        [Test]
        [Arguments(null, DisplayName = "method name = null")]
        [Arguments("", DisplayName = "method name = empty")]
        [Arguments("  ", DisplayName = "method name = whitespace")]
        public void MethodName_ThrowsWhenSetToNull(string? methodName)
        {
            var methodInfo = new MethodInfo(AccessModifier.Public, "void", "OriginalMethodName");

            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                methodInfo.Name = methodName!;
            });
        }

        [Test]
        public async Task MethodsCanBeStatic()
        {
            var methodInfo = new MethodInfo(AccessModifier.Private, "int", "MyMethod");

            methodInfo.IsStatic = true;

            var src = methodInfo.ToString();

            await Assert.That(src).IsEqualTo(@"private static int MyMethod()
{
}");
        }

        [Test]
        public async Task MethodsCanOverrideBaseMethods()
        {
            var methodInfo = new MethodInfo(AccessModifier.Private, "int", "MyMethod");

            methodInfo.IsOverride = true;

            var src = methodInfo.ToString();

            await Assert.That(src).IsEqualTo(@"private override int MyMethod()
{
}");
        }

        [Test]
        public async Task BodyGetsIndented()
        {
            var methodInfo = new MethodInfo(AccessModifier.Private, "int", "MyMethod");

            methodInfo.Body.Add(@"
if (true)
    doSomething();
");

            var src = methodInfo.ToString();

            await Assert.That(src).IsEqualTo(@"private int MyMethod()
{
    if (true)
        doSomething();
}");
        }

        [Test]
        public async Task WithXmlDoc_Summary()
        {
            var methodInfo = new MethodInfo(AccessModifier.Public, "string", "MyMethod1");

            methodInfo.XmlDocSummary = @"This is the summary for the method
Line 2";

            var src = methodInfo.ToString();

            await Assert.That(src).IsEqualTo(@"/// <summary>
/// This is the summary for the method
/// Line 2
/// </summary>
public string MyMethod1()
{
}");
        }

        [Test]
        public async Task WithXmlDoc_AdditionalLines()
        {
            var methodInfo = new MethodInfo(AccessModifier.Public, "string", "MyMethod1");

            methodInfo.XmlDocAdditionalLines.Add("<example>This is the first line</example>");
            methodInfo.XmlDocAdditionalLines.Add("<someTag>This is the second line</someTag>");

            var src = methodInfo.ToString();

            await Assert.That(src).IsEqualTo(@"/// <summary>
/// MyMethod1
/// </summary>
/// <example>This is the first line</example>
/// <someTag>This is the second line</someTag>
public string MyMethod1()
{
}");
        }


        [Test]
        public async Task WithXmlDoc_Generic()
        {
            var methodInfo = new MethodInfo(AccessModifier.Private, "int", "MyMethod");

            methodInfo.GenericTypes.Add("T1", (null, "This is my generic parameter"));
            methodInfo.GenericTypes.Add("T2", null);

            var src = methodInfo.ToString();

            await Assert.That(src).IsEqualTo(@"/// <summary>
/// MyMethod
/// </summary>
/// <typeparam name=""T1"">This is my generic parameter</typeparam>
/// <typeparam name=""T2""></typeparam>
private int MyMethod<T1, T2>()
{
}");
        }


        [Test]
        public async Task WithXmlDoc_FullDoc()
        {
            var methodInfo = new MethodInfo(AccessModifier.Public, "string", "MyMethod1");

            methodInfo.XmlDocAdditionalLines.Add("<example>This is the first line</example>");
            methodInfo.XmlDocAdditionalLines.Add("<someTag>This is the second line</someTag>");

            methodInfo.XmlDocSummary = @"This is the summary for the method
Line 2";


            methodInfo.GenericTypes.Add("T1", (null, "This is my generic parameter"));

            var src = methodInfo.ToString();

            await Assert.That(src).IsEqualTo(@"/// <summary>
/// This is the summary for the method
/// Line 2
/// </summary>
/// <typeparam name=""T1"">This is my generic parameter</typeparam>
/// <example>This is the first line</example>
/// <someTag>This is the second line</someTag>
public string MyMethod1<T1>()
{
}");
        }

        [Test]
        public async Task WithXmlDoc_InheritDoc()
        {
            var methodInfo = new MethodInfo(AccessModifier.Public, "string", "MyMethod1");

            methodInfo.InheritXmlDoc = true;

            var src = methodInfo.ToString();

            await Assert.That(src).IsEqualTo(@"/// <inheritdoc />
public string MyMethod1()
{
}");
        }

        [Test]
        public async Task With1GenericType_NoConstraints()
        {
            var methodInfo = new MethodInfo(AccessModifier.Private, "int", "MyMethod");

            methodInfo.GenericTypes.Add("T1", (null, null));

            var src = methodInfo.ToString();

            await Assert.That(src).IsEqualTo(@"private int MyMethod<T1>()
{
}");
        }

        [Test]
        public async Task With1GenericType_WithConstraints()
        {
            var methodInfo = new MethodInfo(AccessModifier.Private, "int", "MyMethod");

            methodInfo.GenericTypes.Add("T1", (new List<string>() { "ISomeInterface" }, null));

            var src = methodInfo.ToString();

            await Assert.That(src).IsEqualTo(@"private int MyMethod<T1>()
    where T1 : ISomeInterface
{
}");
        }

        [Test]
        public async Task With3GenericType_NoConstraints()
        {
            var methodInfo = new MethodInfo(AccessModifier.Private, "int", "MyMethod");

            methodInfo.GenericTypes.Add("T1", null);
            methodInfo.GenericTypes.Add("T2", (null, null));
            methodInfo.GenericTypes.Add("T3", (new List<string>(), null));

            var src = methodInfo.ToString();

            await Assert.That(src).IsEqualTo(@"private int MyMethod<T1, T2, T3>()
{
}");
        }

        [Test]
        public async Task With5GenericType_With3Constraints()
        {
            var methodInfo = new MethodInfo(AccessModifier.Private, "int", "MyMethod");

            methodInfo.GenericTypes.Add("A1", (new List<string>() { "ISomeInterface" }, null));
            methodInfo.GenericTypes.Add("A2", (new List<string>() { "ISomeOtherInterface" }, null));
            methodInfo.GenericTypes.Add("A3", (new List<string>() { "ISomeInterface", "ISomeOtherInterface" }, null));
            methodInfo.GenericTypes.Add("A4", null);
            methodInfo.GenericTypes.Add("A5", (null, null));
            methodInfo.GenericTypes.Add("A6", (new List<string>(), null));

            var src = methodInfo.ToString();

            await Assert.That(src).IsEqualTo(@"private int MyMethod<A1, A2, A3, A4, A5, A6>()
    where A1 : ISomeInterface
    where A2 : ISomeOtherInterface
    where A3 : ISomeInterface, ISomeOtherInterface
{
}");
        }

        [Test]
        public async Task Partial_IsInitializedByConstructor_IsPartial()
        {
            var methodInfo = new MethodInfo(true, "MyMethod");

            await Assert.That(methodInfo.IsPartial).IsTrue();
            await Assert.That(methodInfo.ReturnType).IsEqualTo("void");
            await Assert.That(methodInfo.AccessModifier).IsEqualTo(AccessModifier.Private);

            var src = methodInfo.ToString();

            await Assert.That(src).IsEqualTo(@"partial void MyMethod();");
        }

        [Test]
        public async Task Partial_IsInitializedByConstructor_IsNotPartial()
        {
            var methodInfo = new MethodInfo(false, "MyMethod");

            await Assert.That(methodInfo.IsPartial).IsFalse();
            await Assert.That(methodInfo.ReturnType).IsEqualTo("void");
            await Assert.That(methodInfo.AccessModifier).IsEqualTo(AccessModifier.Private);

            var src = methodInfo.ToString();

            await Assert.That(src).IsEqualTo(@"private void MyMethod()
{
}");
        }

        [Test]
        public void Partial_CannotChangeReturnType()
        {
            var methodInfo = new MethodInfo(true, "MyMethod");

            Assert.ThrowsExactly<ArgumentException>(() => methodInfo.ReturnType = "string");
        }

        [Test]
        public void Partial_CannotChangeAccessModifier()
        {
            var methodInfo = new MethodInfo(true, "MyMethod");

            Assert.ThrowsExactly<ArgumentException>(() => methodInfo.AccessModifier = AccessModifier.Public);
        }

        [Test]
        public async Task Partial_GenericWithoutBody()
        {
            var methodInfo = new MethodInfo(true, "MyMethod");

            methodInfo.GenericTypes.Add("T1", (new List<string>() { "ISomeInterface" }, "T1 is cool"));

            var src = methodInfo.ToString();

            await Assert.That(src).IsEqualTo(@"/// <summary>
/// MyMethod
/// </summary>
/// <typeparam name=""T1"">T1 is cool</typeparam>
partial void MyMethod<T1>()
    where T1 : ISomeInterface;");
        }

        [Test]
        public async Task Partial_GenericWithBody()
        {
            var methodInfo = new MethodInfo(true, "MyMethod");

            methodInfo.GenericTypes.Add("T1", (new List<string>() { "ISomeInterface" }, "T1 is cool"));

            methodInfo.Body.Add(@"
if (true == false)
    ;
throw new NotImplementedException();
");

            var src = methodInfo.ToString();

            await Assert.That(src).IsEqualTo(@"/// <summary>
/// MyMethod
/// </summary>
/// <typeparam name=""T1"">T1 is cool</typeparam>
partial void MyMethod<T1>()
    where T1 : ISomeInterface
{
    if (true == false)
        ;
    throw new NotImplementedException();
}");
        }

        [Test]
        public async Task With1Parameter()
        {
            var methodInfo = new MethodInfo(true, "MyMethod");

            methodInfo.Parameters.Add(new ParameterInfo(ParameterKind.Ordinary, "int", "myPar"));

            var src = methodInfo.ToString();

            await Assert.That(src).IsEqualTo(@"partial void MyMethod(int myPar);");
        }

        [Test]
        public async Task With3Parameters()
        {
            var methodInfo = new MethodInfo(AccessModifier.Public, "void", "MyMethod");

            methodInfo.Parameters.Add(new ParameterInfo(ParameterKind.Ordinary, "int", "myPar"));
            methodInfo.Parameters.Add(new ParameterInfo(ParameterKind.Ordinary, "string", "myPar2") { DefaultValue = "" });
            methodInfo.Parameters.Add(new ParameterInfo(ParameterKind.Ref, "string", "myPar3"));

            var src = methodInfo.ToString();

            await Assert.That(src).IsEqualTo(@"public void MyMethod(int myPar, string myPar2 = """", ref string myPar3)
{
}");
        }

        [Test]
        public async Task IsExtensionMethod()
        {
            var methodInfo = new MethodInfo(AccessModifier.Public, "void", "MyMethod");

            methodInfo.Parameters.Add(new ParameterInfo(ParameterKind.Ordinary, "int", "myPar"));

            methodInfo.IsExtensionMethod = true;

            var src = methodInfo.ToString();

            await Assert.That(src).IsEqualTo(@"public static void MyMethod(this int myPar)
{
}");
        }
    }
}
