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
    public class ClassInfoTests
    {
        private const string C_CODEGENERATION_ATTRIBUTE_STRING = @"[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""<GeneratorName>"", ""<GeneratorVersion>"")]
[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
";

        private readonly string _codeGenerationAttributeString;

        public ClassInfoTests()
        {
            var execAssemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();

            _codeGenerationAttributeString = C_CODEGENERATION_ATTRIBUTE_STRING
                .Replace("<GeneratorName>", execAssemblyName.Name)
                .Replace("<GeneratorVersion>", execAssemblyName.Version?.ToString() ?? "1.0.0.0");
        }

        [Test]
        [Arguments(null, DisplayName = "null")]
        [Arguments("", DisplayName = "empty string")]
        [Arguments("   ", DisplayName = "whitespace")]
        public void EmptyClassName_Throws(string? className)
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new ClassInfo(className!, AccessModifier.ProtectedInternal));

            var classInfo = new ClassInfo("MyClass", AccessModifier.Public);
            Assert.ThrowsExactly<ArgumentNullException>(() => classInfo.ClassName = className!);
        }

        [Test]
        [Arguments(null, DisplayName = "codeGenAttr: <default>")]
        [Arguments(true, DisplayName = "codeGenAttr: true")]
        [Arguments(false, DisplayName = "codeGenAttr: false")]
        public async Task EmptyClass_CorrectString(bool? codeGenAttrEnabled)
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.ProtectedInternal);
            if (codeGenAttrEnabled.HasValue)
                classInfo.AddGeneratedCodeAttributes = codeGenAttrEnabled.Value;

            var classStr = classInfo.ToString();

            string codeGenAttrStr = codeGenAttrEnabled.GetValueOrDefault(true) ? _codeGenerationAttributeString : string.Empty;

            await Assert.That(classStr).IsEqualTo(codeGenAttrStr + @"protected internal class TestClass1
{
}");
        }

        [Test]
        [Arguments(false, 1, "I1")]
        [Arguments(false, 2, "I1, I2")]
        [Arguments(true, 1, "MyBaseClass1, I1")]
        [Arguments(true, 3, "MyBaseClass1, I1, I2, I3")]
        public async Task BaseClassAndInterfaces_CorrectString(bool withBaseClass, int interfaceCnt, string resultPart)
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
            classInfo.AddGeneratedCodeAttributes = false;

            if (withBaseClass)
                classInfo.BaseClass = "MyBaseClass1";

            for (int i = 1; i <= interfaceCnt; i++)
                classInfo.ImplementedInterfaces.Add("I" + i.ToString());

            var classStr = classInfo.ToString();

            await Assert.That(classStr).IsEqualTo(@"private class TestClass1 : " + resultPart + @"
{
}");
        }

        [Test]
        [Arguments(null, DisplayName = "null")]
        [Arguments("", DisplayName = "empty string")]
        [Arguments("   ", DisplayName = "whitespace")]
        public async Task EmptyInterfaceName_IsIgnored(string? interfaceName)
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
            classInfo.AddGeneratedCodeAttributes = false;
            classInfo.ImplementedInterfaces.Add("I1");
            classInfo.ImplementedInterfaces.Add(interfaceName!);
            classInfo.ImplementedInterfaces.Add("I2");

            var classStr = classInfo.ToString();

            await Assert.That(classStr).IsEqualTo(@"private class TestClass1 : I1, I2
{
}");
        }

        [Test]
        public async Task With1GenericType_NoConstraints_NoBaseClass()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
            classInfo.AddGeneratedCodeAttributes = false;
            classInfo.GenericTypes.Add("T1", (new List<string>() { }, null));

            var classStr = classInfo.ToString();

            await Assert.That(classStr).IsEqualTo(@"private class TestClass1<T1>
{
}");
        }

        [Test]
        public async Task With4GenericTypes_NoConstraints_NoBaseClass()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
            classInfo.AddGeneratedCodeAttributes = false;
            classInfo.GenericTypes.Add("T1", null);
            classInfo.GenericTypes.Add("T2", null);
            classInfo.GenericTypes.Add("T3", (new List<string>() { }, null));
            classInfo.GenericTypes.Add("T4", (null, null));

            var classStr = classInfo.ToString();

            await Assert.That(classStr).IsEqualTo(@"private class TestClass1<T1, T2, T3, T4>
{
}");
        }

        [Test]
        public async Task With1GenericType_NoConstraints_WithBaseClass()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
            classInfo.AddGeneratedCodeAttributes = false;
            classInfo.BaseClass = "MyBaseClass1";
            classInfo.GenericTypes.Add("T1", null);

            var classStr = classInfo.ToString();

            await Assert.That(classStr).IsEqualTo(@"private class TestClass1<T1> : MyBaseClass1
{
}");
        }

        [Test]
        public async Task With3GenericTypes_NoConstraints_WithBaseClass()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
            classInfo.AddGeneratedCodeAttributes = false;
            classInfo.BaseClass = "MyBaseClass1";
            classInfo.GenericTypes.Add("T1", null);
            classInfo.GenericTypes.Add("T2", null);
            classInfo.GenericTypes.Add("T3", null);

            var classStr = classInfo.ToString();

            await Assert.That(classStr).IsEqualTo(@"private class TestClass1<T1, T2, T3> : MyBaseClass1
{
}");
        }

        [Test]
        public async Task With1GenericType_WithConstraints_NoBaseClass()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
            classInfo.AddGeneratedCodeAttributes = false;
            classInfo.GenericTypes.Add("T1", (new List<string>() { "class", "new()" }, null));

            var classStr = classInfo.ToString();

            await Assert.That(classStr).IsEqualTo(@"private class TestClass1<T1>
    where T1 : class, new()
{
}");
        }

        [Test]
        public async Task With3GenericTypes_WithConstraints_NoBaseClass()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
            classInfo.AddGeneratedCodeAttributes = false;
            classInfo.GenericTypes.Add("T1", null);
            classInfo.GenericTypes.Add("T2", (new List<string>() { "class" }, null));
            classInfo.GenericTypes.Add("T3", (new List<string>() { }, null));

            var classStr = classInfo.ToString();

            await Assert.That(classStr).IsEqualTo(@"private class TestClass1<T1, T2, T3>
    where T2 : class
{
}");
        }

        [Test]
        public async Task With1GenericType_WithConstraints_WithBaseClass()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
            classInfo.AddGeneratedCodeAttributes = false;
            classInfo.BaseClass = "MyBaseClass1";
            classInfo.GenericTypes.Add("T1", (new List<string>() { "class", "new()" }, null));

            var classStr = classInfo.ToString();

            await Assert.That(classStr).IsEqualTo(@"private class TestClass1<T1> : MyBaseClass1
    where T1 : class, new()
{
}");
        }

        [Test]
        public async Task With5GenericTypes_WithConstraints_WithBaseClass()
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

            await Assert.That(classStr).IsEqualTo(@"private class TestClass1<T1, T2, T3, T4, T5> : MyBaseClass1
    where T2 : class, new()
    where T3 : enum
{
}");
        }

        [Test]
        public async Task BaseClass_CanBeChanged()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Internal);
            classInfo.AddGeneratedCodeAttributes = false;
            classInfo.BaseClass = "MyBaseClass1";
            classInfo.BaseClass = "MyBaseClass2";

            await Assert.That(classInfo.BaseClass).IsEqualTo("MyBaseClass2");

            var classStr = classInfo.ToString();

            await Assert.That(classStr).IsEqualTo(@"internal class TestClass1 : MyBaseClass2
{
}");
        }

        [Test]
        [Arguments(null, DisplayName = "null")]
        [Arguments("", DisplayName = "empty string")]
        [Arguments("   ", DisplayName = "whitespace")]
        public async Task BaseClass_ResetToNull(string? baseClassValue)
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Internal);
            classInfo.AddGeneratedCodeAttributes = false;
            classInfo.BaseClass = "MyBaseClass1";
            classInfo.BaseClass = baseClassValue;

            await Assert.That(classInfo.BaseClass).IsNull();

            var classStr = classInfo.ToString();

            await Assert.That(classStr).IsEqualTo(@"internal class TestClass1
{
}");
        }

        [Test]
        public async Task ClassName_CanBeChanged()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Internal);
            classInfo.AddGeneratedCodeAttributes = false;
            classInfo.ClassName = "ChangedTestClassName";

            await Assert.That(classInfo.ClassName).IsEqualTo("ChangedTestClassName");

            var classStr = classInfo.ToString();

            await Assert.That(classStr).IsEqualTo(@"internal class ChangedTestClassName
{
}");
        }

        [Test]
        public async Task GeneratedCodeToolName_Property_CorrectValue()
        {
            var classInfo = new ClassInfo("MyClass1", AccessModifier.Public, "MyToolName1", "MyToolVersion1");

            await Assert.That(classInfo.GeneratedCodeToolName).IsEqualTo("MyToolName1");

            classInfo.GeneratedCodeToolName = "MyToolName2";
            await Assert.That(classInfo.GeneratedCodeToolName).IsEqualTo("MyToolName2");
        }

        [Test]
        public async Task GeneratedCodeToolVersion_Property_CorrectValue()
        {
            var classInfo = new ClassInfo("MyClass1", AccessModifier.Public, "MyToolName1", "MyToolVersion1");

            await Assert.That(classInfo.GeneratedCodeToolVersion).IsEqualTo("MyToolVersion1");

            classInfo.GeneratedCodeToolVersion = "MyToolVersion2";
            await Assert.That(classInfo.GeneratedCodeToolVersion).IsEqualTo("MyToolVersion2");
        }

        [Test]
        public async Task PartialClass_CorrectString()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.PrivateProtected);
            classInfo.AddGeneratedCodeAttributes = false;
            classInfo.IsPartial = true;

            var classStr = classInfo.ToString();

            await Assert.That(classStr).IsEqualTo(@"private protected partial class TestClass1
{
}");
        }

        [Test]
        public async Task SealedClass_CorrectString()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.PrivateProtected);
            classInfo.AddGeneratedCodeAttributes = false;
            classInfo.IsSealed = true;

            var classStr = classInfo.ToString();

            await Assert.That(classStr).IsEqualTo(@"private protected sealed class TestClass1
{
}");
        }

        [Test]
        public async Task StaticClass_CorrectString()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.PrivateProtected);
            classInfo.AddGeneratedCodeAttributes = false;
            classInfo.IsStatic = true;

            var classStr = classInfo.ToString();

            await Assert.That(classStr).IsEqualTo(@"private protected static class TestClass1
{
}");
        }

        [Test]
        public async Task WithXmlDoc_Summary()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
            classInfo.AddGeneratedCodeAttributes = false;

            classInfo.XmlDocSummary = @"This is the summary
Line 2";

            var classStr = classInfo.ToString();

            await Assert.That(classStr).IsEqualTo(@"/// <summary>
/// This is the summary
/// Line 2
/// </summary>
private class TestClass1
{
}");
        }

        [Test]
        public async Task WithXmlDoc_Generics()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
            classInfo.AddGeneratedCodeAttributes = false;

            classInfo.GenericTypes.Add("T1", (null, "T1 is cool"));
            classInfo.GenericTypes.Add("T2", null);

            var classStr = classInfo.ToString();

            await Assert.That(classStr).IsEqualTo(@"/// <summary>
/// TestClass1
/// </summary>
/// <typeparam name=""T1"">T1 is cool</typeparam>
/// <typeparam name=""T2""></typeparam>
private class TestClass1<T1, T2>
{
}");
        }

        [Test]
        public async Task WithXmlDoc_AdditionalLines()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
            classInfo.AddGeneratedCodeAttributes = false;

            classInfo.XmlDocAdditionalLines.Add("<example>This is the first line</example>");
            classInfo.XmlDocAdditionalLines.Add("<someTag>This is the second line</someTag>");

            var classStr = classInfo.ToString();

            await Assert.That(classStr).IsEqualTo(@"/// <summary>
/// TestClass1
/// </summary>
/// <example>This is the first line</example>
/// <someTag>This is the second line</someTag>
private class TestClass1
{
}");
        }

        [Test]
        public async Task WithXmlDoc_FullDoc()
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

            await Assert.That(classStr).IsEqualTo(@"/// <summary>
/// This is the summary
/// Line 2
/// </summary>
/// <typeparam name=""T1"">T1 is cool</typeparam>
/// <typeparam name=""T2""></typeparam>
/// <example>This is the first line</example>
/// <someTag>This is the second line</someTag>
private class TestClass1<T1, T2>
{
}");
        }

        [Test]
        public async Task WithXmlDoc_InheritDoc()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
            classInfo.AddGeneratedCodeAttributes = false;

            classInfo.InheritXmlDoc = true;

            var classStr = classInfo.ToString();

            await Assert.That(classStr).IsEqualTo(@"/// <inheritdoc />
private class TestClass1
{
}");
        }

        [Test]
        public async Task WithOneMethod()
        {
            var classInfo = new ClassInfo("MyClass", AccessModifier.Public);
            classInfo.AddGeneratedCodeAttributes = false;

            classInfo.Methods.Add(new MethodInfo(AccessModifier.Public, "void", "MyPublicMethod")
            {
                XmlDocSummary = "This is a public method"
            });

            var classStr = classInfo.ToString();

            await Assert.That(classStr).IsEqualTo(@"public class MyClass
{
    /// <summary>
    /// This is a public method
    /// </summary>
    public void MyPublicMethod()
    {
    }
}");
        }

        [Test]
        public async Task WithTwoMethods()
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

            methodInfo.Body.Add(@"
if (true == false)
    throw new Exception(""This should never occur"");

throw new NotImplementedException();
");

            classInfo.Methods.Add(methodInfo);

            var classStr = classInfo.ToString();

            await Assert.That(classStr).IsEqualTo(@"public class MyClass
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
}");
        }

        [Test]
        public async Task AccessAllChildProperties_ButLeaveThemEmpty()
        {
            var classInfo = new ClassInfo("MyClass", AccessModifier.Public);
            classInfo.AddGeneratedCodeAttributes = false;

            await Assert.That(classInfo.Fields).IsNotNull();
            await Assert.That(classInfo.Properties).IsNotNull();
            await Assert.That(classInfo.Methods).IsNotNull();
            await Assert.That(classInfo.AdditionalCodeLines).IsNotNull();
            await Assert.That(classInfo.XmlDocAdditionalLines).IsNotNull();

            var src = classInfo.ToString();
            await Assert.That(src).IsEqualTo(@"public class MyClass
{
}");
        }

        [Test]
        public async Task With1AdditionalCodeLine()
        {
            var classInfo = new ClassInfo("MyClass", AccessModifier.Public);
            classInfo.AddGeneratedCodeAttributes = false;

            classInfo.Methods.Add(new MethodInfo(AccessModifier.Public, "void", "MyPublicMethod")
            {
                XmlDocSummary = "This is a public method"
            });

            classInfo.AdditionalCodeLines.Add(@"
public int TestProp { get; }
");

            var classStr = classInfo.ToString();

            await Assert.That(classStr).IsEqualTo(@"public class MyClass
{
    /// <summary>
    /// This is a public method
    /// </summary>
    public void MyPublicMethod()
    {
    }
    
    public int TestProp { get; }
}");
        }

        [Test]
        public async Task With2AdditionalCodeLines()
        {
            var classInfo = new ClassInfo("MyClass", AccessModifier.Public);
            classInfo.AddGeneratedCodeAttributes = false;

            classInfo.Methods.Add(new MethodInfo(AccessModifier.Public, "void", "MyPublicMethod")
            {
                XmlDocSummary = "This is a public method"
            });

            classInfo.AdditionalCodeLines.Add(@"
public int TestProp { get; }

This is some line that doesn't compile but I don't care what is added in here...
");

            var classStr = classInfo.ToString();

            await Assert.That(classStr).IsEqualTo(@"public class MyClass
{
    /// <summary>
    /// This is a public method
    /// </summary>
    public void MyPublicMethod()
    {
    }
    
    public int TestProp { get; }
    
    This is some line that doesn't compile but I don't care what is added in here...
}");
        }

        [Test]
        public async Task With2Fields()
        {
            var classInfo = new ClassInfo("MyClass", AccessModifier.Public);
            classInfo.AddGeneratedCodeAttributes = false;

            classInfo.Fields.Add(new FieldInfo("double", "_someField"));
            classInfo.Fields.Add(new FieldInfo("int", "_myField"));

            var src = classInfo.ToString();

            await Assert.That(src).IsEqualTo(@"public class MyClass
{
    private double _someField;
    private int _myField;
}");
        }

        [Test]
        public async Task With1Property()
        {
            var classInfo = new ClassInfo("MyClass", AccessModifier.Public);
            classInfo.AddGeneratedCodeAttributes = false;

            classInfo.Properties.Add(new PropertyInfo("int", "MyProp"));

            var src = classInfo.ToString();

            await Assert.That(src).IsEqualTo(@"public class MyClass
{
    public int MyProp { get; set; }
}");
        }

        [Test]
        public async Task With2Properties()
        {
            var classInfo = new ClassInfo("MyClass", AccessModifier.Public);
            classInfo.AddGeneratedCodeAttributes = false;

            classInfo.Properties.Add(new PropertyInfo("int", "MyProp"));
            classInfo.Properties.Add(new PropertyInfo("int", "MyProp2"));

            var src = classInfo.ToString();

            await Assert.That(src).IsEqualTo(@"public class MyClass
{
    public int MyProp { get; set; }
    
    public int MyProp2 { get; set; }
}");
        }

        [Test]
        public async Task With2Properties_WithAdditionalFieldsAndMethods()
        {
            var classInfo = new ClassInfo("MyClass", AccessModifier.Public);
            classInfo.AddGeneratedCodeAttributes = false;

            classInfo.Fields.Add(new FieldInfo("int", "_someOtherField"));

            classInfo.Properties.Add(new PropertyInfo("int", "MyProp") { FieldName = "_myField" });
            classInfo.Properties.Add(new PropertyInfo("int", "MyProp2") { FieldName = "_myField2" });

            classInfo.Methods.Add(new MethodInfo(AccessModifier.Public, "bool", "CanDoSomething"));

            var src = classInfo.ToString();

            await Assert.That(src).IsEqualTo(@"public class MyClass
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
}");
        }

        [Test]
        public async Task With1Property_With2Attributes()
        {
            var classInfo = new ClassInfo("MyClass", AccessModifier.Public);
            classInfo.AddGeneratedCodeAttributes = false;

            var propertyInfo = new PropertyInfo("int", "MyProp");

            propertyInfo.Attributes.Add(new AttributeInfo("MyAttribute1"));

            var ai = new AttributeInfo("MyAttribute2");
            ai.ConstructorParameters.Add(new AttributeConstructorParameter("typeof(string)"));
            ai.ConstructorParameters.Add(new AttributeConstructorParameter("param2", "typeof(int)"));
            propertyInfo.Attributes.Add(ai);

            classInfo.Properties.Add(propertyInfo);

            var src = classInfo.ToString();

            await Assert.That(src).IsEqualTo(@"public class MyClass
{
    [MyAttribute1]
    [MyAttribute2(typeof(string), param2: typeof(int))]
    public int MyProp { get; set; }
}");
        }
    }
}
