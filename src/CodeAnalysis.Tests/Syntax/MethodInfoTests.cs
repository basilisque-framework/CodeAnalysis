using Basilisque.CodeAnalysis.Syntax;

namespace Basilisque.CodeAnalysis.Tests.Syntax
{
    [TestClass]
    public class MethodInfoTests
    {
        [TestMethod]
        public void ConstructorInitializesParameters()
        {
            var methodInfo = new MethodInfo(AccessModifier.Public, "string", "MyMethod1");

            var src = methodInfo.ToString();

            Assert.AreEqual(@"public string MyMethod1()
{
}", src);
        }

        [DataTestMethod]
        [DataRow(null, "MyMethod1Async", "async ", DisplayName = "returnType = null, method = async")]
        [DataRow("", "MyMethod1Async", "async ", DisplayName = "returnType = \"\", method = async")]
        [DataRow("  ", "MyMethod1Async", "async ", DisplayName = "returnType = \"  \", method = async")]
        [DataRow("void", "MyMethod1Async", "async ", DisplayName = "returnType = void method = async")]
        [DataRow("Task", "MyMethod1Async", "async ", DisplayName = "returnType = Task, method = async")]
        [DataRow("Task<string>", "MyMethod1Async", "async ", DisplayName = "returnType = Task<string>, method = async")]
        [DataRow("System.Threading.Tasks.Task", "MyMethod1Async", "async ", DisplayName = "returnType = System.Threading.Tasks.Task, method = async")]
        [DataRow("System.Threading.Tasks.Task<string>", "MyMethod1Async", "async ", DisplayName = "returnType = System.Threading.Tasks.Task<string>, method = async")]
        [DataRow("string", "MyMethod1Async", "", DisplayName = "returnType = string, method = async")]
        [DataRow("Task", "MyMethod1", "", DisplayName = "returnType = Task, method = sync")]
        public void Constructor_DoesInitializeAsync(string returnType, string methodName, string isAsyncResult)
        {
            var methodInfo = new MethodInfo(AccessModifier.Public, returnType, methodName);

            var src = methodInfo.ToString();

            if (string.IsNullOrWhiteSpace(returnType))
                returnType = "void";

            Assert.AreEqual(@$"public {isAsyncResult}{returnType} {methodName}()
{{
}}", src);
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "method name = null")]
        [DataRow("", DisplayName = "method name = empty")]
        [DataRow("  ", DisplayName = "method name = whitespace")]
        public void Constructor_EmptyMethodNameThrows(string methodName)
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                var methodInfo = new MethodInfo(AccessModifier.Public, "void", methodName);
            });
        }

        [TestMethod]
        public void MethodName_CanBeChanged()
        {
            var methodInfo = new MethodInfo(AccessModifier.PrivateProtected, "string", "ConstructorMethodName");

            Assert.AreEqual("ConstructorMethodName", methodInfo.Name);

            var src = methodInfo.ToString();
            Assert.AreEqual(@"private protected string ConstructorMethodName()
{
}", src);

            methodInfo.Name = "ChangedMethodName";

            Assert.AreEqual("ChangedMethodName", methodInfo.Name);

            src = methodInfo.ToString();
            Assert.AreEqual(@"private protected string ChangedMethodName()
{
}", src);
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "method name = null")]
        [DataRow("", DisplayName = "method name = empty")]
        [DataRow("  ", DisplayName = "method name = whitespace")]
        public void MethodName_ThrowsWhenSetToNull(string methodName)
        {
            var methodInfo = new MethodInfo(AccessModifier.Public, "void", "OriginalMethodName");

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                methodInfo.Name = methodName;
            });
        }

        [TestMethod]
        public void MethodsCanBeStatic()
        {
            var methodInfo = new MethodInfo(AccessModifier.Private, "int", "MyMethod");

            methodInfo.IsStatic = true;

            var src = methodInfo.ToString();

            Assert.AreEqual(@"private static int MyMethod()
{
}", src);
        }

        [TestMethod]
        public void BodyGetsIndented()
        {
            var methodInfo = new MethodInfo(AccessModifier.Private, "int", "MyMethod");

            methodInfo.Body.Append(@"
if (true)
    doSomething();
");

            var src = methodInfo.ToString();

            Assert.AreEqual(@"private int MyMethod()
{
    if (true)
        doSomething();
}", src);
        }

        [TestMethod]
        public void WithXmlDoc_Summary()
        {
            var methodInfo = new MethodInfo(AccessModifier.Public, "string", "MyMethod1");

            methodInfo.XmlDocSummary = @"This is the summary for the method
Line 2";

            var src = methodInfo.ToString();

            Assert.AreEqual(@"/// <summary>
/// This is the summary for the method
/// Line 2
/// </summary>
public string MyMethod1()
{
}", src);
        }

        [TestMethod]
        public void WithXmlDoc_AdditionalLines()
        {
            var methodInfo = new MethodInfo(AccessModifier.Public, "string", "MyMethod1");

            methodInfo.XmlDocAdditionalLines.Add("<example>This is the first line</example>");
            methodInfo.XmlDocAdditionalLines.Add("<someTag>This is the second line</someTag>");

            var src = methodInfo.ToString();

            Assert.AreEqual(@"/// <summary>
/// MyMethod1
/// </summary>
/// <example>This is the first line</example>
/// <someTag>This is the second line</someTag>
public string MyMethod1()
{
}", src);
        }


        [TestMethod]
        public void WithXmlDoc_Generic()
        {
            var methodInfo = new MethodInfo(AccessModifier.Private, "int", "MyMethod");

            methodInfo.GenericTypes.Add("T1", (null, "This is my generic parameter"));

            var src = methodInfo.ToString();

            Assert.AreEqual(@"/// <summary>
/// MyMethod
/// </summary>
/// <typeparam name=""T1"">This is my generic parameter</typeparam>
private int MyMethod<T1>()
{
}", src);
        }


        [TestMethod]
        public void WithXmlDoc_FullDoc()
        {
            var methodInfo = new MethodInfo(AccessModifier.Public, "string", "MyMethod1");

            methodInfo.XmlDocAdditionalLines.Add("<example>This is the first line</example>");
            methodInfo.XmlDocAdditionalLines.Add("<someTag>This is the second line</someTag>");

            methodInfo.XmlDocSummary = @"This is the summary for the method
Line 2";


            methodInfo.GenericTypes.Add("T1", (null, "This is my generic parameter"));

            var src = methodInfo.ToString();

            Assert.AreEqual(@"/// <summary>
/// This is the summary for the method
/// Line 2
/// </summary>
/// <typeparam name=""T1"">This is my generic parameter</typeparam>
/// <example>This is the first line</example>
/// <someTag>This is the second line</someTag>
public string MyMethod1<T1>()
{
}", src);
        }

        [TestMethod]
        public void With1GenericType_NoConstraints()
        {
            var methodInfo = new MethodInfo(AccessModifier.Private, "int", "MyMethod");

            methodInfo.GenericTypes.Add("T1", (null, null));

            var src = methodInfo.ToString();

            Assert.AreEqual(@"private int MyMethod<T1>()
{
}", src);
        }

        [TestMethod]
        public void With1GenericType_WithConstraints()
        {
            var methodInfo = new MethodInfo(AccessModifier.Private, "int", "MyMethod");

            methodInfo.GenericTypes.Add("T1", (new List<string>() { "ISomeInterface" }, null));

            var src = methodInfo.ToString();

            Assert.AreEqual(@"private int MyMethod<T1>()
    where T1 : ISomeInterface
{
}", src);
        }

        [TestMethod]
        public void With3GenericType_NoConstraints()
        {
            var methodInfo = new MethodInfo(AccessModifier.Private, "int", "MyMethod");

            methodInfo.GenericTypes.Add("T1", (null, null));
            methodInfo.GenericTypes.Add("T2", (null, null));
            methodInfo.GenericTypes.Add("T3", (null, null));

            var src = methodInfo.ToString();

            Assert.AreEqual(@"private int MyMethod<T1, T2, T3>()
{
}", src);
        }

        [TestMethod]
        public void With3GenericType_WithConstraints()
        {
            var methodInfo = new MethodInfo(AccessModifier.Private, "int", "MyMethod");

            methodInfo.GenericTypes.Add("A1", (new List<string>() { "ISomeInterface" }, null));
            methodInfo.GenericTypes.Add("A2", (new List<string>() { "ISomeOtherInterface" }, null));
            methodInfo.GenericTypes.Add("A3", (new List<string>() { "ISomeInterface", "ISomeOtherInterface" }, null));

            var src = methodInfo.ToString();

            Assert.AreEqual(@"private int MyMethod<A1, A2, A3>()
    where A1 : ISomeInterface
    where A2 : ISomeOtherInterface
    where A3 : ISomeInterface, ISomeOtherInterface
{
}", src);
        }

        [TestMethod]
        public void Partial_IsInitializedByConstructor_IsPartial()
        {
            var methodInfo = new MethodInfo(true, "MyMethod");

            Assert.AreEqual(true, methodInfo.IsPartial);
            Assert.AreEqual("void", methodInfo.ReturnType);
            Assert.AreEqual(AccessModifier.Private, methodInfo.AccessModifier);

            var src = methodInfo.ToString();

            Assert.AreEqual(@"partial void MyMethod();", src);
        }

        [TestMethod]
        public void Partial_IsInitializedByConstructor_IsNotPartial()
        {
            var methodInfo = new MethodInfo(false, "MyMethod");

            Assert.AreEqual(false, methodInfo.IsPartial);
            Assert.AreEqual("void", methodInfo.ReturnType);
            Assert.AreEqual(AccessModifier.Private, methodInfo.AccessModifier);

            var src = methodInfo.ToString();

            Assert.AreEqual(@"private void MyMethod()
{
}", src);
        }

        [TestMethod]
        public void Partial_CannotChangeReturnType()
        {
            var methodInfo = new MethodInfo(true, "MyMethod");

            Assert.ThrowsException<ArgumentException>(() => methodInfo.ReturnType = "string");
        }

        [TestMethod]
        public void Partial_CannotChangeAccessModifier()
        {
            var methodInfo = new MethodInfo(true, "MyMethod");

            Assert.ThrowsException<ArgumentException>(() => methodInfo.AccessModifier = AccessModifier.Public);
        }

        [TestMethod]
        public void Partial_GenericWithoutBody()
        {
            var methodInfo = new MethodInfo(true, "MyMethod");

            methodInfo.GenericTypes.Add("T1", (new List<string>() { "ISomeInterface" }, "T1 is cool"));

            var src = methodInfo.ToString();

            Assert.AreEqual(@"/// <summary>
/// MyMethod
/// </summary>
/// <typeparam name=""T1"">T1 is cool</typeparam>
partial void MyMethod<T1>()
    where T1 : ISomeInterface;", src);
        }

        [TestMethod]
        public void Partial_GenericWithBody()
        {
            var methodInfo = new MethodInfo(true, "MyMethod");

            methodInfo.GenericTypes.Add("T1", (new List<string>() { "ISomeInterface" }, "T1 is cool"));

            methodInfo.Body.Append(@"
if (true == false)
    ;
throw new NotImplementedException();
");

            var src = methodInfo.ToString();

            Assert.AreEqual(@"/// <summary>
/// MyMethod
/// </summary>
/// <typeparam name=""T1"">T1 is cool</typeparam>
partial void MyMethod<T1>()
    where T1 : ISomeInterface
{
    if (true == false)
        ;
    throw new NotImplementedException();
}", src);
        }

        [TestMethod]
        public void With1Parameter()
        {
            var methodInfo = new MethodInfo(true, "MyMethod");

            methodInfo.Parameters.Add(new ParameterInfo(ParameterKind.Ordinary, "int", "myPar"));

            var src = methodInfo.ToString();

            Assert.AreEqual(@"partial void MyMethod(int myPar);", src);
        }

        [TestMethod]
        public void With3Parameters()
        {
            var methodInfo = new MethodInfo(AccessModifier.Public, "void", "MyMethod");

            methodInfo.Parameters.Add(new ParameterInfo(ParameterKind.Ordinary, "int", "myPar"));
            methodInfo.Parameters.Add(new ParameterInfo(ParameterKind.Ordinary, "string", "myPar2") { DefaultValue = "" });
            methodInfo.Parameters.Add(new ParameterInfo(ParameterKind.Ref, "string", "myPar3"));

            var src = methodInfo.ToString();

            Assert.AreEqual(@"public void MyMethod(int myPar, string myPar2 = """", ref string myPar3)
{
}", src);
        }

        [TestMethod]
        public void IsExtensionMethod()
        {
            var methodInfo = new MethodInfo(AccessModifier.Public, "void", "MyMethod");

            methodInfo.Parameters.Add(new ParameterInfo(ParameterKind.Ordinary, "int", "myPar"));

            methodInfo.IsExtensionMethod = true;

            var src = methodInfo.ToString();

            Assert.AreEqual(@"public static void MyMethod(this int myPar)
{
}", src);
        }
    }
}
