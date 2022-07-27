using Basilisque.CodeAnalysis.Syntax;

namespace Basilisque.CodeAnalysis.Tests.Syntax
{
    [TestClass]
    public class ClassInfoTests
    {
        private const string C_CODEGENERATION_ATTRIBUTE_STRING = @"[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""<GeneratorName>"", ""<GeneratorVersion>"")]
[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
";

        private string _codeGenerationAttributeString;

        public ClassInfoTests()
        {
            var execAssemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();

            _codeGenerationAttributeString = C_CODEGENERATION_ATTRIBUTE_STRING
                .Replace("<GeneratorName>", execAssemblyName.Name)
                .Replace("<GeneratorVersion>", execAssemblyName.Version?.ToString() ?? "1.0.0.0");
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "null")]
        [DataRow("", DisplayName = "empty string")]
        [DataRow("   ", DisplayName = "whitespace")]
        public void EmptyClassName_Throws(string className)
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ClassInfo(className, AccessModifier.ProtectedInternal));

            var classInfo = new ClassInfo("MyClass", AccessModifier.Public);
            Assert.ThrowsException<ArgumentNullException>(() => classInfo.ClassName = className);
        }

        [TestMethod]
        [DataRow(null, DisplayName = "codeGenAttr: <default>")]
        [DataRow(true, DisplayName = "codeGenAttr: true")]
        [DataRow(false, DisplayName = "codeGenAttr: false")]
        public void EmptyClass_CorrectString(bool? codeGenAttrEnabled)
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.ProtectedInternal);
            if (codeGenAttrEnabled.HasValue)
                classInfo.AddGeneratedCodeAttributes = codeGenAttrEnabled.Value;

            var classStr = classInfo.ToString();

            string codeGenAttrStr = (!codeGenAttrEnabled.HasValue || codeGenAttrEnabled.HasValue && codeGenAttrEnabled.Value) ? _codeGenerationAttributeString : string.Empty;

            Assert.AreEqual(codeGenAttrStr + @"protected internal class TestClass1
{
}", classStr);
        }

        [DataTestMethod]
        [DataRow(false, 1, "I1")]
        [DataRow(false, 2, "I1, I2")]
        [DataRow(true, 1, "MyBaseClass1, I1")]
        [DataRow(true, 3, "MyBaseClass1, I1, I2, I3")]
        public void BaseClassAndInterfaces_CorrectString(bool withBaseClass, int interfaceCnt, string resultPart)
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
            classInfo.AddGeneratedCodeAttributes = false;

            if (withBaseClass)
                classInfo.BaseClass = "MyBaseClass1";

            for (int i = 1; i <= interfaceCnt; i++)
                classInfo.ImplementedInterfaces.Add("I" + i.ToString());

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"private class TestClass1 : " + resultPart + @"
{
}", classStr);
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "null")]
        [DataRow("", DisplayName = "empty string")]
        [DataRow("   ", DisplayName = "whitespace")]
        public void EmptyInterfaceName_IsIgnored(string interfaceName)
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
            classInfo.AddGeneratedCodeAttributes = false;
            classInfo.ImplementedInterfaces.Add("I1");
            classInfo.ImplementedInterfaces.Add(interfaceName);
            classInfo.ImplementedInterfaces.Add("I2");

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"private class TestClass1 : I1, I2
{
}", classStr);
        }

        [TestMethod]
        public void With1GenericType_NoConstraints_NoBaseClass()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
            classInfo.AddGeneratedCodeAttributes = false;
            classInfo.GenericTypes.Add("T1", (new List<string>() { }, null));

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"private class TestClass1<T1>
{
}", classStr);
        }

        [TestMethod]
        public void With4GenericTypes_NoConstraints_NoBaseClass()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
            classInfo.AddGeneratedCodeAttributes = false;
            classInfo.GenericTypes.Add("T1", null);
            classInfo.GenericTypes.Add("T2", null);
            classInfo.GenericTypes.Add("T3", (new List<string>() { }, null));
            classInfo.GenericTypes.Add("T4", (null, null));

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"private class TestClass1<T1, T2, T3, T4>
{
}", classStr);
        }

        [TestMethod]
        public void With1GenericType_NoConstraints_WithBaseClass()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
            classInfo.AddGeneratedCodeAttributes = false;
            classInfo.BaseClass = "MyBaseClass1";
            classInfo.GenericTypes.Add("T1", null);

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"private class TestClass1<T1> : MyBaseClass1
{
}", classStr);
        }

        [TestMethod]
        public void With3GenericTypes_NoConstraints_WithBaseClass()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
            classInfo.AddGeneratedCodeAttributes = false;
            classInfo.BaseClass = "MyBaseClass1";
            classInfo.GenericTypes.Add("T1", null);
            classInfo.GenericTypes.Add("T2", null);
            classInfo.GenericTypes.Add("T3", null);

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"private class TestClass1<T1, T2, T3> : MyBaseClass1
{
}", classStr);
        }

        [TestMethod]
        public void With1GenericType_WithConstraints_NoBaseClass()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
            classInfo.AddGeneratedCodeAttributes = false;
            classInfo.GenericTypes.Add("T1", (new List<string>() { "class", "new()" }, null));

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"private class TestClass1<T1>
    where T1 : class, new()
{
}", classStr);
        }

        [TestMethod]
        public void With3GenericTypes_WithConstraints_NoBaseClass()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
            classInfo.AddGeneratedCodeAttributes = false;
            classInfo.GenericTypes.Add("T1", null);
            classInfo.GenericTypes.Add("T2", (new List<string>() { "class" }, null));
            classInfo.GenericTypes.Add("T3", (new List<string>() { }, null));

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"private class TestClass1<T1, T2, T3>
    where T2 : class
{
}", classStr);
        }

        [TestMethod]
        public void With1GenericType_WithConstraints_WithBaseClass()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
            classInfo.AddGeneratedCodeAttributes = false;
            classInfo.BaseClass = "MyBaseClass1";
            classInfo.GenericTypes.Add("T1", (new List<string>() { "class", "new()" }, null));

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"private class TestClass1<T1> : MyBaseClass1
    where T1 : class, new()
{
}", classStr);
        }

        [TestMethod]
        public void With5GenericTypes_WithConstraints_WithBaseClass()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
            classInfo.AddGeneratedCodeAttributes = false;
            classInfo.BaseClass = "MyBaseClass1";
            classInfo.GenericTypes.Add("T1", null);
            classInfo.GenericTypes.Add("T2", (new List<string>() { "class", "new()" }, null));
            classInfo.GenericTypes.Add("T3", (new List<string>() { "enum" }, null));
            classInfo.GenericTypes.Add("T4", (new List<string>(), null));
            classInfo.GenericTypes.Add("T5", (null, null));

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"private class TestClass1<T1, T2, T3, T4, T5> : MyBaseClass1
    where T2 : class, new()
    where T3 : enum
{
}", classStr);
        }

        [TestMethod]
        public void BaseClass_CanBeChanged()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Internal);
            classInfo.AddGeneratedCodeAttributes = false;
            classInfo.BaseClass = "MyBaseClass1";
            classInfo.BaseClass = "MyBaseClass2";

            Assert.AreEqual("MyBaseClass2", classInfo.BaseClass);

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"internal class TestClass1 : MyBaseClass2
{
}", classStr);
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "null")]
        [DataRow("", DisplayName = "empty string")]
        [DataRow("   ", DisplayName = "whitespace")]
        public void BaseClass_ResetToNull(string baseClassValue)
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Internal);
            classInfo.AddGeneratedCodeAttributes = false;
            classInfo.BaseClass = "MyBaseClass1";
            classInfo.BaseClass = baseClassValue;

            Assert.AreEqual(null, classInfo.BaseClass);

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"internal class TestClass1
{
}", classStr);
        }

        [TestMethod]
        public void ClassName_CanBeChanged()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Internal);
            classInfo.AddGeneratedCodeAttributes = false;
            classInfo.ClassName = "ChangedTestClassName";

            Assert.AreEqual("ChangedTestClassName", classInfo.ClassName);

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"internal class ChangedTestClassName
{
}", classStr);
        }

        [TestMethod]
        public void GeneratedCodeToolName_Property_CorrectValue()
        {
            var classInfo = new ClassInfo("MyClass1", AccessModifier.Public, "MyToolName1", "MyToolVersion1");

            Assert.AreEqual("MyToolName1", classInfo.GeneratedCodeToolName);

            classInfo.GeneratedCodeToolName = "MyToolName2";
            Assert.AreEqual("MyToolName2", classInfo.GeneratedCodeToolName);
        }

        [TestMethod]
        public void GeneratedCodeToolVersion_Property_CorrectValue()
        {
            var classInfo = new ClassInfo("MyClass1", AccessModifier.Public, "MyToolName1", "MyToolVersion1");

            Assert.AreEqual("MyToolVersion1", classInfo.GeneratedCodeToolVersion);

            classInfo.GeneratedCodeToolVersion = "MyToolVersion2";
            Assert.AreEqual("MyToolVersion2", classInfo.GeneratedCodeToolVersion);
        }

        [TestMethod]
        public void PartialClass_CorrectString()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.PrivateProtected);
            classInfo.AddGeneratedCodeAttributes = false;
            classInfo.IsPartial = true;

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"private protected partial class TestClass1
{
}", classStr);
        }

        [TestMethod]
        public void SealedClass_CorrectString()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.PrivateProtected);
            classInfo.AddGeneratedCodeAttributes = false;
            classInfo.IsSealed = true;

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"private protected sealed class TestClass1
{
}", classStr);
        }

        [TestMethod]
        public void StaticClass_CorrectString()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.PrivateProtected);
            classInfo.AddGeneratedCodeAttributes = false;
            classInfo.IsStatic = true;

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"private protected static class TestClass1
{
}", classStr);
        }

        [TestMethod]
        public void WithXmlDoc_Summary()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
            classInfo.AddGeneratedCodeAttributes = false;

            classInfo.XmlDocSummary = @"This is the summary
Line 2";

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"/// <summary>
/// This is the summary
/// Line 2
/// </summary>
private class TestClass1
{
}", classStr);
        }

        [TestMethod]
        public void WithXmlDoc_Generics()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
            classInfo.AddGeneratedCodeAttributes = false;

            classInfo.GenericTypes.Add("T1", (null, "T1 is cool"));
            classInfo.GenericTypes.Add("T2", null);

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"/// <summary>
/// TestClass1
/// </summary>
/// <typeparam name=""T1"">T1 is cool</typeparam>
/// <typeparam name=""T2""></typeparam>
private class TestClass1<T1, T2>
{
}", classStr);
        }

        [TestMethod]
        public void WithXmlDoc_AdditionalLines()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
            classInfo.AddGeneratedCodeAttributes = false;

            classInfo.XmlDocAdditionalLines.Add("<example>This is the first line</example>");
            classInfo.XmlDocAdditionalLines.Add("<someTag>This is the second line</someTag>");

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"/// <summary>
/// TestClass1
/// </summary>
/// <example>This is the first line</example>
/// <someTag>This is the second line</someTag>
private class TestClass1
{
}", classStr);
        }

        [TestMethod]
        public void WithXmlDoc_FullDoc()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
            classInfo.AddGeneratedCodeAttributes = false;

            classInfo.XmlDocAdditionalLines.Add("<example>This is the first line</example>");
            classInfo.XmlDocAdditionalLines.Add("<someTag>This is the second line</someTag>");

            classInfo.XmlDocSummary = @"This is the summary
Line 2";

            classInfo.GenericTypes.Add("T1", (null, "T1 is cool"));
            classInfo.GenericTypes.Add("T2", (null, null));

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"/// <summary>
/// This is the summary
/// Line 2
/// </summary>
/// <typeparam name=""T1"">T1 is cool</typeparam>
/// <typeparam name=""T2""></typeparam>
/// <example>This is the first line</example>
/// <someTag>This is the second line</someTag>
private class TestClass1<T1, T2>
{
}", classStr);
        }

        [TestMethod]
        public void WithOneMethod()
        {
            var classInfo = new ClassInfo("MyClass", AccessModifier.Public);
            classInfo.AddGeneratedCodeAttributes = false;

            classInfo.Methods.Add(new MethodInfo(AccessModifier.Public, "void", "MyPublicMethod")
            {
                XmlDocSummary = "This is a public method"
            });

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"public class MyClass
{
    /// <summary>
    /// This is a public method
    /// </summary>
    public void MyPublicMethod()
    {
    }
}", classStr);
        }

        [TestMethod]
        public void WithTwoMethods()
        {
            var classInfo = new ClassInfo("MyClass", AccessModifier.Public);
            classInfo.AddGeneratedCodeAttributes = false;

            classInfo.Methods.Add(new MethodInfo(AccessModifier.Public, "void", "MyPublicMethod")
            {
                XmlDocSummary = "This is a public method"
            });

            var methodInfo = new MethodInfo(AccessModifier.Public, "Task<string>", "MySecondMethodAsync")
            {
                XmlDocSummary = "This is an async method"
            };

            methodInfo.Body.Append(@"
if (true == false)
    throw new Exception(""This should never occur"");

throw new NotImplementedException();
");

            classInfo.Methods.Add(methodInfo);

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"public class MyClass
{
    /// <summary>
    /// This is a public method
    /// </summary>
    public void MyPublicMethod()
    {
    }
    
    /// <summary>
    /// This is an async method
    /// </summary>
    public async Task<string> MySecondMethodAsync()
    {
        if (true == false)
            throw new Exception(""This should never occur"");
        
        throw new NotImplementedException();
    }
}", classStr);
        }

        [TestMethod]
        public void AccessAllChildProperties_ButLeaveThemEmpty()
        {
            var classInfo = new ClassInfo("MyClass", AccessModifier.Public);
            classInfo.AddGeneratedCodeAttributes = false;

            Assert.IsNotNull(classInfo.Fields);
            Assert.IsNotNull(classInfo.Properties);
            Assert.IsNotNull(classInfo.Methods);
            Assert.IsNotNull(classInfo.AdditionalCodeLines);
            Assert.IsNotNull(classInfo.XmlDocAdditionalLines);

            var src = classInfo.ToString();
            Assert.AreEqual(@"public class MyClass
{
}", src);
        }

        [TestMethod]
        public void With1AdditionalCodeLine()
        {
            var classInfo = new ClassInfo("MyClass", AccessModifier.Public);
            classInfo.AddGeneratedCodeAttributes = false;

            classInfo.Methods.Add(new MethodInfo(AccessModifier.Public, "void", "MyPublicMethod")
            {
                XmlDocSummary = "This is a public method"
            });

            classInfo.AdditionalCodeLines.Append(@"
public int TestProp { get; }
");

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"public class MyClass
{
    /// <summary>
    /// This is a public method
    /// </summary>
    public void MyPublicMethod()
    {
    }
    
    public int TestProp { get; }
}", classStr);
        }

        [TestMethod]
        public void With2AdditionalCodeLines()
        {
            var classInfo = new ClassInfo("MyClass", AccessModifier.Public);
            classInfo.AddGeneratedCodeAttributes = false;

            classInfo.Methods.Add(new MethodInfo(AccessModifier.Public, "void", "MyPublicMethod")
            {
                XmlDocSummary = "This is a public method"
            });

            classInfo.AdditionalCodeLines.Append(@"
public int TestProp { get; }

This is some line that doesn't compile but I don't care what is added in here...
");

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"public class MyClass
{
    /// <summary>
    /// This is a public method
    /// </summary>
    public void MyPublicMethod()
    {
    }
    
    public int TestProp { get; }
    
    This is some line that doesn't compile but I don't care what is added in here...
}", classStr);
        }

        [TestMethod]
        public void With2Fields()
        {
            var classInfo = new ClassInfo("MyClass", AccessModifier.Public);
            classInfo.AddGeneratedCodeAttributes = false;

            classInfo.Fields.Add(new FieldInfo("double", "_someField"));
            classInfo.Fields.Add(new FieldInfo("int", "_myField"));

            var src = classInfo.ToString();

            Assert.AreEqual(@"public class MyClass
{
    private double _someField;
    private int _myField;
}", src);
        }

        [TestMethod]
        public void With1Property()
        {
            var classInfo = new ClassInfo("MyClass", AccessModifier.Public);
            classInfo.AddGeneratedCodeAttributes = false;

            classInfo.Properties.Add(new PropertyInfo("int", "MyProp"));

            var src = classInfo.ToString();

            Assert.AreEqual(@"public class MyClass
{
    public int MyProp { get; set; }
}", src);
        }

        [TestMethod]
        public void With2Properties()
        {
            var classInfo = new ClassInfo("MyClass", AccessModifier.Public);
            classInfo.AddGeneratedCodeAttributes = false;

            classInfo.Properties.Add(new PropertyInfo("int", "MyProp"));
            classInfo.Properties.Add(new PropertyInfo("int", "MyProp2"));

            var src = classInfo.ToString();

            Assert.AreEqual(@"public class MyClass
{
    public int MyProp { get; set; }
    
    public int MyProp2 { get; set; }
}", src);
        }

        [TestMethod]
        public void With2Properties_WithAdditionalFieldsAndMethods()
        {
            var classInfo = new ClassInfo("MyClass", AccessModifier.Public);
            classInfo.AddGeneratedCodeAttributes = false;

            classInfo.Fields.Add(new FieldInfo("int", "_someOtherField"));

            classInfo.Properties.Add(new PropertyInfo("int", "MyProp") { FieldName = "_myField" });
            classInfo.Properties.Add(new PropertyInfo("int", "MyProp2") { FieldName = "_myField2" });

            classInfo.Methods.Add(new MethodInfo(AccessModifier.Public, "bool", "CanDoSomething"));

            var src = classInfo.ToString();

            Assert.AreEqual(@"public class MyClass
{
    private int _someOtherField;
    private int _myField;
    private int _myField2;
    
    public int MyProp
    {
        get
        {
            return this._myField;
        }
        set
        {
            if (value != this._myField)
            {
                this._myField = value;
            }
        }
    }
    
    public int MyProp2
    {
        get
        {
            return this._myField2;
        }
        set
        {
            if (value != this._myField2)
            {
                this._myField2 = value;
            }
        }
    }
    
    public bool CanDoSomething()
    {
    }
}", src);
        }
    }
}
