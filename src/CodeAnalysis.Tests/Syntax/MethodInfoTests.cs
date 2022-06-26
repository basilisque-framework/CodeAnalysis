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
    }
}
