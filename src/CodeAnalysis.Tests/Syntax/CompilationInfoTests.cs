using Basilisque.CodeAnalysis.Syntax;

namespace Basilisque.CodeAnalysis.Tests.Syntax
{
    [TestClass]
    public class CompilationInfoTests
    {
        [DataTestMethod]
        [DataRow(null, DisplayName = "null")]
        [DataRow("", DisplayName = "empty string")]
        [DataRow("   ", DisplayName = "whitespace")]
        public void NoNamespace_NoClass_IsEmpty(string ns)
        {
            var compilationInfo = new CompilationInfo("MyCompilation1", targetNamespace: ns);

            var str = compilationInfo.ToString();

            Assert.AreEqual(string.Empty, str);
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "null")]
        [DataRow("", DisplayName = "empty string")]
        [DataRow("   ", DisplayName = "whitespace")]
        public void NoNamespace_1Class_CorrectString(string ns)
        {
            var compilationInfo = new CompilationInfo("MyCompilation1", targetNamespace: ns)
                .AddNewClassInfo("MyClass1", AccessModifier.Public, ci => { });

            var str = compilationInfo.ToString();

            Assert.AreEqual(@"public class MyClass1
{
}", str);
        }

        [TestMethod]
        public void NoNamespace_2Classes_CorrectString()
        {
            var compilationInfo = new CompilationInfo("MyCompilation1", null)
                .AddNewClassInfo("MyClass1", AccessModifier.Public, ci => { })
                .AddNewClassInfo("MyClass2", AccessModifier.Public, ci => { });

            var str = compilationInfo.ToString();

            Assert.AreEqual(@"public class MyClass1
{
}

public class MyClass2
{
}", str);
        }

        [TestMethod]
        public void WithNamespace_NoClass_CorrectString()
        {
            var compilationInfo = new CompilationInfo("MyCompilation1", "MyNamespace1");

            var str = compilationInfo.ToString();

            Assert.AreEqual(@"namespace MyNamespace1
{
}", str);
        }

        [TestMethod]
        public void WithNamespace_1Class_CorrectString()
        {
            var compilationInfo = new CompilationInfo("MyCompilation1", "MyNamespace1")
                .AddNewClassInfo("MyClass1", AccessModifier.Public, ci => { });

            var str = compilationInfo.ToString();

            Assert.AreEqual(@"namespace MyNamespace1
{
    public class MyClass1
    {
    }
}", str);
        }

        [TestMethod]
        public void WithNamespace_2Classes_CorrectString()
        {
            var compilationInfo = new CompilationInfo("MyCompilation1", "MyNamespace1")
                .AddNewClassInfo("MyClass1", AccessModifier.Public, ci => { })
                .AddNewClassInfo("MyClass2", ci => { });

            var str = compilationInfo.ToString();

            Assert.AreEqual(@"namespace MyNamespace1
{
    public class MyClass1
    {
    }

    public class MyClass2
    {
    }
}", str);
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "null")]
        [DataRow("", DisplayName = "empty string")]
        [DataRow("   ", DisplayName = "whitespace")]
        public void EmptyCompliationName_Throws(string compilationName)
        {
            Assert.ThrowsException<ArgumentNullException>(() => new CompilationInfo(compilationName, null));

            var compilationInfo = new CompilationInfo("Name1", "Namespace1");
            Assert.ThrowsException<ArgumentNullException>(() => compilationInfo.CompilationName = null!);
        }

        [TestMethod]
        public void CompilationName_Property_CorrectValue()
        {
            var compilationInfo = new CompilationInfo("MyInfo4711", null);

            Assert.AreEqual("MyInfo4711", compilationInfo.CompilationName);

            compilationInfo.CompilationName = "MyInfo0815";
            Assert.AreEqual("MyInfo0815", compilationInfo.CompilationName);
        }

        [TestMethod]
        public void TargetNamespace_Property_CorrectValue()
        {
            var compilationInfo = new CompilationInfo("MyInfo4711", "MyNS1");

            Assert.AreEqual("MyNS1", compilationInfo.TargetNamespace);

            compilationInfo.TargetNamespace = "MyNS2";
            Assert.AreEqual("MyNS2", compilationInfo.TargetNamespace);

            compilationInfo.TargetNamespace = null;
            Assert.AreEqual(null, compilationInfo.TargetNamespace);
        }

        [TestMethod]
        public void ToStringWithStringBuilder_ThrowsWhenStringBuilderIsNull()
        {
            var compilationInfo = new CompilationInfo("MyInfo4711", "MyNS1");

            Assert.ThrowsException<ArgumentNullException>(() => compilationInfo.ToString(null!, 0, Language.CSharp));
        }

        [TestMethod]
        public void ToStringWithStringBuilder_ThrowsWhenIndentationCountIsLessThanZero()
        {
            var compilationInfo = new CompilationInfo("MyInfo4711", "MyNS1");

            var sb = new System.Text.StringBuilder();

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => compilationInfo.ToString(sb, -1, Language.CSharp));
        }
    }
}
