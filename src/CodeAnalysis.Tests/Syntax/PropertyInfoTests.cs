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
    public class PropertyInfoTests
    {
        [DataTestMethod]
        [DataRow(null, DisplayName = "Name = null")]
        [DataRow("", DisplayName = "Name = \"\"")]
        [DataRow("  ", DisplayName = "Name = \"  \"")]
        public void Constructor_EmptyNameThrows(string name)
        {
            Assert.ThrowsException<ArgumentNullException>(() => new PropertyInfo("int", name));
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "Type = null")]
        [DataRow("", DisplayName = "Type = \"\"")]
        [DataRow("  ", DisplayName = "Type = \"  \"")]
        public void Constructor_EmptyTypeThrows(string type)
        {
            Assert.ThrowsException<ArgumentNullException>(() => new PropertyInfo(type, "name"));
        }

        [TestMethod]
        public void AutoImplemented()
        {
            var propertyInfo = new PropertyInfo("int", "MyProperty");

            var src = propertyInfo.ToString();

            Assert.AreEqual(@"public int MyProperty { get; set; }", src);
        }

        [TestMethod]
        public void ChangeAccessModifier()
        {
            var propertyInfo = new PropertyInfo("int", "MyProperty");

            propertyInfo.AccessModifier = AccessModifier.Private;

            var src = propertyInfo.ToString();

            Assert.AreEqual(@"private int MyProperty { get; set; }", src);
        }

        [TestMethod]
        public void ChangeName()
        {
            var propertyInfo = new PropertyInfo("int", "MyProperty");

            propertyInfo.Name = "MyOtherProperty";

            var src = propertyInfo.ToString();

            Assert.AreEqual(@"public int MyOtherProperty { get; set; }", src);
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "Name = null")]
        [DataRow("", DisplayName = "Name = \"\"")]
        [DataRow("  ", DisplayName = "Name = \"  \"")]
        public void EmptyNameThrows(string emptyName)
        {
            var propertyInfo = new PropertyInfo("int", "MyProperty");

            Assert.ThrowsException<ArgumentNullException>(() => propertyInfo.Name = emptyName);
        }

        [TestMethod]
        public void ChangeType()
        {
            var propertyInfo = new PropertyInfo("int", "MyProperty");

            Assert.AreEqual("int", propertyInfo.Type);

            propertyInfo.Type = "string";

            Assert.AreEqual("string", propertyInfo.Type);

            var src = propertyInfo.ToString();

            Assert.AreEqual(@"public string MyProperty { get; set; }", src);
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "Type = null")]
        [DataRow("", DisplayName = "Type = \"\"")]
        [DataRow("  ", DisplayName = "Type = \"  \"")]
        public void EmptyTypeThrows(string emptyType)
        {
            var propertyInfo = new PropertyInfo("int", "MyProperty");

            Assert.ThrowsException<ArgumentNullException>(() => propertyInfo.Type = emptyType);
        }

        [TestMethod]
        public void HasInitialValue()
        {
            var propertyInfo = new PropertyInfo("int", "MyProperty");

            propertyInfo.InitialValue = "8";

            var src = propertyInfo.ToString();

            Assert.AreEqual(@"public int MyProperty { get; set; } = 8;", src);
        }

        [TestMethod]
        public void HasInvalidInitialValue()
        {
            var propertyInfo = new PropertyInfo("int", "MyProperty");

            propertyInfo.InitialValue = "";

            var src = propertyInfo.ToString();

            Assert.AreEqual(@"public int MyProperty { get; set; }", src);
        }

        [DataTestMethod]
        [DataRow("test", "test", DisplayName = "InitialValue = no parenthesis")]
        [DataRow("te\"st", "te\\\"st", DisplayName = "InitialValue = contains parenthesis")]
        [DataRow("\"test", "\\\"test", DisplayName = "InitialValue = leading parenthesis")]
        [DataRow("test\"", "test\\\"", DisplayName = "InitialValue = trailing parenthesis")]
        [DataRow("@\"test", "@\\\"test", DisplayName = "InitialValue = starts with @")]
        [DataRow("$\"test", "$\\\"test", DisplayName = "InitialValue = starts with $")]
        [DataRow("$@\"test", "$@\\\"test", DisplayName = "InitialValue = starts with $@")]
        [DataRow("@$\"test", "@$\\\"test", DisplayName = "InitialValue = starts with @$")]
        public void WithInitialValue_StringAutoParenthesis(string initialValue, string expected)
        {
            var propertyInfo = new PropertyInfo("string", "MyProperty");

            propertyInfo.InitialValue = initialValue;

            var src = propertyInfo.ToString();

            Assert.AreEqual($@"public string MyProperty {{ get; set; }} = ""{expected}"";", src);
        }

        [DataTestMethod]
        [DataRow("\"test\"", DisplayName = "InitialValue = no additional parenthesis")]
        [DataRow("\"te\"\"st\"", DisplayName = "InitialValue = contains double parenthesis")]
        [DataRow("\"te\"st\"", DisplayName = "InitialValue = contains single parenthesis")]
        [DataRow("@\"test\"", DisplayName = "InitialValue = starts with @")]
        [DataRow("$\"test\"", DisplayName = "InitialValue = starts with $")]
        [DataRow("$@\"test\"", DisplayName = "InitialValue = starts with $@")]
        [DataRow("@$\"test\"", DisplayName = "InitialValue = starts with @$")]
        public void WithInitialValue_StringManualParenthesis(string initialValue)
        {
            var propertyInfo = new PropertyInfo("string", "MyProperty");

            propertyInfo.InitialValue = initialValue;

            var src = propertyInfo.ToString();

            Assert.AreEqual($@"public string MyProperty {{ get; set; }} = {initialValue};", src);
        }

        [TestMethod]
        public void AutoImplemented_GetOnly()
        {
            var propertyInfo = new PropertyInfo("int", "MyProperty");

            propertyInfo.HasSetter = false;

            var src = propertyInfo.ToString();

            Assert.AreEqual(@"public int MyProperty { get; }", src);
        }

        [TestMethod]
        public void AutoImplemented_SetOnly()
        {
            var propertyInfo = new PropertyInfo("int", "MyProperty");

            propertyInfo.HasGetter = false;

            var src = propertyInfo.ToString();

            Assert.AreEqual(@"public int MyProperty { set; }", src);
        }

        [TestMethod]
        public void AutoImplemented_GetAndSetFallback()
        {
            var propertyInfo = new PropertyInfo("int", "MyProperty");

            propertyInfo.HasGetter = false;
            propertyInfo.HasSetter = false;

            var src = propertyInfo.ToString();

            Assert.AreEqual(@"public int MyProperty { get; set; }", src);
        }

        [TestMethod]
        public void WithGetterBody_AutoSetter()
        {
            var propertyInfo = new PropertyInfo("int", "MyProperty");

            propertyInfo.GetterBody.Append(@"
return _myProperty;
");

            var src = propertyInfo.ToString(Language.CSharp, false);
            var fieldName = propertyInfo.FieldName;

            Assert.AreEqual(false, string.IsNullOrWhiteSpace(fieldName));

            Assert.AreEqual($@"public int MyProperty
{{
    get
    {{
        return _myProperty;
    }}
    set
    {{
        if (value != this.{fieldName})
        {{
            this.{fieldName} = value;
        }}
    }}
}}", src);
        }

        [TestMethod]
        public void WithSetterBody_AutoGetter()
        {
            var propertyInfo = new PropertyInfo("int", "MyProperty");

            propertyInfo.SetterBody.Append(@"
_myProperty = value;
");

            var src = propertyInfo.ToString(false);
            var fieldName = propertyInfo.FieldName;

            Assert.AreEqual(false, string.IsNullOrWhiteSpace(fieldName));

            Assert.AreEqual($@"public int MyProperty
{{
    get
    {{
        return this.{fieldName};
    }}
    set
    {{
        _myProperty = value;
    }}
}}", src);
        }

        [TestMethod]
        public void WithGetterAndSetterBody()
        {
            var propertyInfo = new PropertyInfo("int", "MyProperty");

            propertyInfo.GetterBody.Append(@"
return _myProperty;
");

            propertyInfo.SetterBody.Append(@"
_myProperty = value;
");

            var src = propertyInfo.ToString();

            Assert.AreEqual(@"public int MyProperty
{
    get
    {
        return _myProperty;
    }
    set
    {
        _myProperty = value;
    }
}", src);
        }

        [TestMethod]
        public void WithGetterBody_NoSetter()
        {
            var propertyInfo = new PropertyInfo("int", "MyProperty");

            propertyInfo.GetterBody.Append(@"
return _myProperty;
");

            propertyInfo.HasSetter = false;

            var src = propertyInfo.ToString();

            Assert.AreEqual(@"public int MyProperty
{
    get
    {
        return _myProperty;
    }
}", src);
        }

        [TestMethod]
        public void WithSetterBody_NoGetter()
        {
            var propertyInfo = new PropertyInfo("int", "MyProperty");

            propertyInfo.HasGetter = false;

            propertyInfo.SetterBody.Append(@"
_myProperty = value;
");

            var src = propertyInfo.ToString();

            Assert.AreEqual(@"public int MyProperty
{
    set
    {
        _myProperty = value;
    }
}", src);
        }

        [TestMethod]
        public void WithFieldname_GetAndSetFallback()
        {
            var propertyInfo = new PropertyInfo("int", "MyProperty");

            propertyInfo.FieldName = "_theField";

            propertyInfo.HasGetter = false;
            propertyInfo.HasSetter = false;

            var src = propertyInfo.ToString(false);

            Assert.AreEqual(@"public int MyProperty
{
    get
    {
        return this._theField;
    }
    set
    {
        if (value != this._theField)
        {
            this._theField = value;
        }
    }
}", src);
        }

        [TestMethod]
        public void AutoImplemented_GetAndSetFallback_AfterResettingFieldname()
        {
            var propertyInfo = new PropertyInfo("int", "MyProperty");

            propertyInfo.FieldName = "Test";
            propertyInfo.FieldName = null;

            propertyInfo.HasGetter = false;
            propertyInfo.HasSetter = false;

            var src = propertyInfo.ToString();

            Assert.AreEqual(@"public int MyProperty { get; set; }", src);
        }

        [TestMethod]
        public void WithBody_Indented()
        {
            var propertyInfo = new PropertyInfo("int", "MyProperty");

            propertyInfo.GetterBody.Append(@"
return _myProperty;
");

            var fieldName = propertyInfo.FieldName;
            Assert.AreEqual(false, string.IsNullOrWhiteSpace(fieldName));

            var sb = new System.Text.StringBuilder();
            propertyInfo.ToString(sb, 1, Language.CSharp, false);
            var src = sb.ToString();

            Assert.AreEqual($@"    public int MyProperty
    {{
        get
        {{
            return _myProperty;
        }}
        set
        {{
            if (value != this.{fieldName})
            {{
                this.{fieldName} = value;
            }}
        }}
    }}", src);
        }

        [TestMethod]
        public void WithoutBody_Indented()
        {
            var propertyInfo = new PropertyInfo("int", "MyProperty");

            var sb = new System.Text.StringBuilder();
            propertyInfo.ToString(sb, 2, Language.CSharp);
            var src = sb.ToString();

            Assert.AreEqual(@"        public int MyProperty { get; set; }", src);
        }

        [TestMethod]
        public void AutoImplemented_WithFieldName()
        {
            var propertyInfo = new PropertyInfo("int", "MyProperty");

            propertyInfo.FieldName = "_myField";

            var src = propertyInfo.ToString(false);

            Assert.AreEqual(@"public int MyProperty
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
}", src);
        }

        [TestMethod]
        public void WithXmlDoc_Summary()
        {
            var propertyInfo = new PropertyInfo("int", "MyProperty");

            propertyInfo.XmlDocSummary = "This is an important property";

            var src = propertyInfo.ToString();

            Assert.AreEqual(@"/// <summary>
/// This is an important property
/// </summary>
public int MyProperty { get; set; }", src);
        }

        [TestMethod]
        public void WithXmlDoc_AdditionalLines()
        {
            var propertyInfo = new PropertyInfo("int", "MyProperty");

            propertyInfo.XmlDocAdditionalLines.Add("<example>8</example>");

            var src = propertyInfo.ToString();

            Assert.AreEqual(@"/// <summary>
/// MyProperty
/// </summary>
/// <example>8</example>
public int MyProperty { get; set; }", src);
        }

        [TestMethod]
        public void WithXmlDoc_InheritDoc()
        {
            var propertyInfo = new PropertyInfo("int", "MyProperty");

            propertyInfo.InheritXmlDoc = true;

            var src = propertyInfo.ToString();

            Assert.AreEqual(@"/// <inheritdoc />
public int MyProperty { get; set; }", src);
        }

        [DataTestMethod]
        [DataRow(true, true, true, DisplayName = "Body = true, Auto = true, Expected = true")]
        [DataRow(true, false, true, DisplayName = "Body = true, Auto = false, Expected = true")]
        [DataRow(false, true, true, DisplayName = "Body = false, Auto = true, Expected = true")]
        [DataRow(false, false, false, DisplayName = "Body = false, Auto = false, Expected = false")]
        public void HasGetter_CorrectStatus(bool hasBody, bool hasAuto, bool expected)
        {
            var propertyInfo = new PropertyInfo("int", "Name");

            propertyInfo.HasGetter = hasAuto;

            if (hasBody)
                propertyInfo.GetterBody.Append("test");

            Assert.AreEqual(expected, propertyInfo.HasGetter);
        }

        [DataTestMethod]
        [DataRow(true, true, true, DisplayName = "Body = true, Auto = true, Expected = true")]
        [DataRow(true, false, true, DisplayName = "Body = true, Auto = false, Expected = true")]
        [DataRow(false, true, true, DisplayName = "Body = false, Auto = true, Expected = true")]
        [DataRow(false, false, false, DisplayName = "Body = false, Auto = false, Expected = false")]
        public void HasSetter_CorrectStatus(bool hasBody, bool hasAuto, bool expected)
        {
            var propertyInfo = new PropertyInfo("int", "Name");

            propertyInfo.HasSetter = hasAuto;

            if (hasBody)
                propertyInfo.SetterBody.Append("test");

            Assert.AreEqual(expected, propertyInfo.HasSetter);
        }

        [TestMethod]
        public void Reset_GetterBody()
        {
            var propertyInfo = new PropertyInfo("type", "name");

            propertyInfo.HasGetter = false;

            propertyInfo.GetterBody.Append("body");

            Assert.IsTrue(propertyInfo.HasGetter);

            propertyInfo.HasGetter = false;

            Assert.IsFalse(propertyInfo.HasGetter);
        }

        [TestMethod]
        public void Reset_SetterBody()
        {
            var propertyInfo = new PropertyInfo("type", "name");

            propertyInfo.HasSetter = false;

            propertyInfo.SetterBody.Append("body");

            Assert.IsTrue(propertyInfo.HasSetter);

            propertyInfo.HasSetter = false;

            Assert.IsFalse(propertyInfo.HasSetter);
        }

        [TestMethod]
        public void AdditionalField_StringBuilderNullReturnsFalse()
        {
            var propertyInfo = new PropertyInfo("int", "SomeProperty");

            var wasAppended = propertyInfo.AppendFieldIfNecessary(null!, 2, Language.CSharp, new List<FieldInfo>() { new FieldInfo("int", "_someField") });

            Assert.IsFalse(wasAppended);
        }

        [TestMethod]
        public void AdditionalField_VisualBasicNotImplementedYet()
        {
            var propertyInfo = new PropertyInfo("int", "SomeProperty");

            var sb = new System.Text.StringBuilder();

            Assert.ThrowsException<NotImplementedException>(() => propertyInfo.AppendFieldIfNecessary(sb, 2, Language.VisualBasic, new List<FieldInfo>() { new FieldInfo("int", "_someField") }));
        }

        [DataTestMethod]
        [DataRow(false, DisplayName = "AccessGetterAndSetterProperty = false")]
        [DataRow(true, DisplayName = "AccessGetterAndSetterProperty = true")]
        public void AdditionalField_NoFieldForAutoProp(bool accessGetterAndSetterProperty)
        {
            var propertyInfo = new PropertyInfo("int", "SomeProperty");

            if (accessGetterAndSetterProperty)
            {
                var gbdy = propertyInfo.GetterBody;
                var sbdy = propertyInfo.SetterBody;

                Assert.IsNotNull(gbdy);
                Assert.IsNotNull(sbdy);
            }

            var sb = new System.Text.StringBuilder();

            var wasAppended = propertyInfo.AppendFieldIfNecessary(sb, 2, Language.CSharp, new List<FieldInfo>() { new FieldInfo("int", "_someField") });

            var src = sb.ToString();

            Assert.IsFalse(wasAppended);
            Assert.AreEqual("", src);
        }

        [TestMethod]
        public void AdditionalField_NoGetterAndNoSetterFallback()
        {
            var propertyInfo = new PropertyInfo("int", "SomeProperty");

            propertyInfo.FieldName = "_myField";
            propertyInfo.HasGetter = false;
            propertyInfo.HasSetter = false;

            var sb = new System.Text.StringBuilder();

            var wasAppended = propertyInfo.AppendFieldIfNecessary(sb, 2, Language.CSharp, new List<FieldInfo>() { new FieldInfo("int", "_someField") });

            var src = sb.ToString();

            Assert.IsTrue(wasAppended);
            Assert.AreEqual("        private int _myField;", src);
        }

        [DataTestMethod]
        [DataRow(false, false, true, false, false, false, DisplayName = "AutoGetter = false, AutoSetter = false, GetterBody = true, SetterBody = false, FieldName = false, GeneratesProperty = false")]
        [DataRow(false, false, false, true, false, false, DisplayName = "AutoGetter = false, AutoSetter = false, GetterBody = false, SetterBody = true, FieldName = false, GeneratesProperty = false")]
        [DataRow(false, false, false, false, true, true, DisplayName = "AutoGetter = false, AutoSetter = false, GetterBody = false, SetterBody = false, FieldName = true, GeneratesProperty = true")]
        [DataRow(true, false, true, false, false, false, DisplayName = "AutoGetter = true, AutoSetter = false, GetterBody = true, SetterBody = false, FieldName = false, GeneratesProperty = false")]
        [DataRow(true, false, false, true, false, true, DisplayName = "AutoGetter = true, AutoSetter = false, GetterBody = false, SetterBody = true, FieldName = false, GeneratesProperty = true")]
        [DataRow(true, false, false, false, true, true, DisplayName = "AutoGetter = true, AutoSetter = false, GetterBody = false, SetterBody = false, FieldName = true, GeneratesProperty = true")]
        [DataRow(false, true, true, false, false, true, DisplayName = "AutoGetter = false, AutoSetter = true, GetterBody = true, SetterBody = false, FieldName = false, GeneratesProperty = true")]
        [DataRow(false, true, false, true, false, false, DisplayName = "AutoGetter = false, AutoSetter = true, GetterBody = false, SetterBody = true, FieldName = false, GeneratesProperty = false")]
        [DataRow(false, true, false, false, true, true, DisplayName = "AutoGetter = false, AutoSetter = true, GetterBody = false, SetterBody = false, FieldName = true, GeneratesProperty = true")]
        public void AdditionalField_ForExtendedProperty(bool withAutoGetter, bool withAutoSetter, bool withGetterBody, bool withSetterBody, bool withFieldName, bool generatesProperty)
        {
            var propertyInfo = new PropertyInfo("int", "SomeProperty");

            propertyInfo.HasGetter = withAutoGetter;
            propertyInfo.HasSetter = withAutoSetter;

            if (withGetterBody)
                propertyInfo.GetterBody.Append("gb");

            if (withSetterBody)
                propertyInfo.SetterBody.Append("sb");

            if (withFieldName)
                propertyInfo.FieldName = "_myField";

            var sb = new System.Text.StringBuilder();

            var wasAppended = propertyInfo.AppendFieldIfNecessary(sb, 2, Language.CSharp, new List<FieldInfo>() { new FieldInfo("int", "_someField") });

            var src = sb.ToString();

            Assert.AreEqual(generatesProperty, wasAppended);

            if (generatesProperty)
            {
                Assert.AreEqual($"        private int {propertyInfo.FieldName};", src);
            }
        }

        [TestMethod]
        public void AdditionalField_ListOfFieldsNull()
        {
            var propertyInfo = new PropertyInfo("int", "SomeProperty");

            propertyInfo.FieldName = "_someField";

            var sb = new System.Text.StringBuilder();

            var wasAppended = propertyInfo.AppendFieldIfNecessary(sb, 2, Language.CSharp, null!);

            var src = sb.ToString();

            Assert.IsTrue(wasAppended);
            Assert.AreEqual("        private int _someField;", src);
        }

        [TestMethod]
        public void AdditionalField_FieldNotInListOfFields()
        {
            var propertyInfo = new PropertyInfo("int", "SomeProperty");

            propertyInfo.FieldName = "_someField";

            var sb = new System.Text.StringBuilder();

            var wasAppended = propertyInfo.AppendFieldIfNecessary(sb, 2, Language.CSharp, new List<FieldInfo>() { new FieldInfo("int", "_someOtherField") });

            var src = sb.ToString();

            Assert.IsTrue(wasAppended);
            Assert.AreEqual("        private int _someField;", src);
        }

        [TestMethod]
        public void AdditionalField_FieldAlreadyInListOfFields()
        {
            var propertyInfo = new PropertyInfo("int", "SomeProperty");

            propertyInfo.FieldName = "_someField";

            var sb = new System.Text.StringBuilder();

            var wasAppended = propertyInfo.AppendFieldIfNecessary(sb, 2, Language.CSharp, new List<FieldInfo>() { new FieldInfo("int", "_someField") });

            var src = sb.ToString();

            Assert.IsFalse(wasAppended);
            Assert.AreEqual(string.Empty, src);
        }

        [TestMethod]
        public void AdditionalField_WithIntialValue()
        {
            var propertyInfo = new PropertyInfo("int", "SomeProperty");

            propertyInfo.FieldName = "_someField";
            propertyInfo.InitialValue = "9";

            var sb = new System.Text.StringBuilder();

            var wasAppended = propertyInfo.AppendFieldIfNecessary(sb, 2, Language.CSharp, new List<FieldInfo>() { new FieldInfo("int", "_someOtherField") });

            var src = sb.ToString();

            Assert.IsTrue(wasAppended);
            Assert.AreEqual(@"        private int _someField = 9;", src);
        }

        [TestMethod]
        public void IncludeBackingField()
        {
            var propertyInfo = new PropertyInfo("int", "PropertyName");
            propertyInfo.FieldName = "_fieldForPropertyName";

            var src = propertyInfo.ToString();

            Assert.AreEqual(@"private int _fieldForPropertyName;
public int PropertyName
{
    get
    {
        return this._fieldForPropertyName;
    }
    set
    {
        if (value != this._fieldForPropertyName)
        {
            this._fieldForPropertyName = value;
        }
    }
}", src);
        }
    }
}
