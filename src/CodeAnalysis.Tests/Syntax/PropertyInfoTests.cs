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

namespace Basilisque.CodeAnalysis.Tests.Syntax;

public class PropertyInfoTests
{
    [Test]
    [Arguments(null, DisplayName = "Name = null")]
    [Arguments("", DisplayName = "Name = \"\"")]
    [Arguments("  ", DisplayName = "Name = \"  \"")]
    public void Constructor_EmptyNameThrows(string? name)
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new PropertyInfo("int", name!));
    }

    [Test]
    [Arguments(null, DisplayName = "Type = null")]
    [Arguments("", DisplayName = "Type = \"\"")]
    [Arguments("  ", DisplayName = "Type = \"  \"")]
    public void Constructor_EmptyTypeThrows(string? type)
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new PropertyInfo(type!, "name"));
    }

    [Test]
    public async Task AutoImplemented()
    {
        var propertyInfo = new PropertyInfo("int", "MyProperty");

        var src = propertyInfo.ToString();

        await Assert.That(src).IsEqualTo(@"public int MyProperty { get; set; }");
    }

    [Test]
    public async Task ChangeAccessModifier()
    {
        var propertyInfo = new PropertyInfo("int", "MyProperty")
        {
            AccessModifier = AccessModifier.Private
        };

        var src = propertyInfo.ToString();

        await Assert.That(src).IsEqualTo(@"private int MyProperty { get; set; }");
    }

    [Test]
    public async Task ChangeName()
    {
        var propertyInfo = new PropertyInfo("int", "MyProperty")
        {
            Name = "MyOtherProperty"
        };

        var src = propertyInfo.ToString();

        await Assert.That(src).IsEqualTo(@"public int MyOtherProperty { get; set; }");
    }

    [Test]
    [Arguments(null, DisplayName = "Name = null")]
    [Arguments("", DisplayName = "Name = \"\"")]
    [Arguments("  ", DisplayName = "Name = \"  \"")]
    public void EmptyNameThrows(string? emptyName)
    {
        var propertyInfo = new PropertyInfo("int", "MyProperty");

        Assert.ThrowsExactly<ArgumentNullException>(() => propertyInfo.Name = emptyName!);
    }

    [Test]
    public async Task ChangeType()
    {
        var propertyInfo = new PropertyInfo("int", "MyProperty");

        await Assert.That(propertyInfo.Type).IsEqualTo("int");

        propertyInfo.Type = "string";

        await Assert.That(propertyInfo.Type).IsEqualTo("string");

        var src = propertyInfo.ToString();

        await Assert.That(src).IsEqualTo(@"public string MyProperty { get; set; }");
    }

    [Test]
    [Arguments(null, DisplayName = "Type = null")]
    [Arguments("", DisplayName = "Type = \"\"")]
    [Arguments("  ", DisplayName = "Type = \"  \"")]
    public void EmptyTypeThrows(string? emptyType)
    {
        var propertyInfo = new PropertyInfo("int", "MyProperty");

        Assert.ThrowsExactly<ArgumentNullException>(() => propertyInfo.Type = emptyType!);
    }

    [Test]
    public async Task HasInitialValue()
    {
        var propertyInfo = new PropertyInfo("int", "MyProperty")
        {
            InitialValue = "8"
        };

        var src = propertyInfo.ToString();

        await Assert.That(src).IsEqualTo(@"public int MyProperty { get; set; } = 8;");
    }

    [Test]
    public async Task HasInvalidInitialValue()
    {
        var propertyInfo = new PropertyInfo("int", "MyProperty")
        {
            InitialValue = ""
        };

        var src = propertyInfo.ToString();

        await Assert.That(src).IsEqualTo(@"public int MyProperty { get; set; }");
    }

    [Test]
    [Arguments("test", "test", DisplayName = "InitialValue = no parenthesis")]
    [Arguments("te\"st", "te\\\"st", DisplayName = "InitialValue = contains parenthesis")]
    [Arguments("\"test", "\\\"test", DisplayName = "InitialValue = leading parenthesis")]
    [Arguments("test\"", "test\\\"", DisplayName = "InitialValue = trailing parenthesis")]
    [Arguments("@\"test", "@\\\"test", DisplayName = "InitialValue = starts with @")]
    [Arguments("$\"test", "$\\\"test", DisplayName = "InitialValue = starts with $")]
    [Arguments("$@\"test", "$@\\\"test", DisplayName = "InitialValue = starts with $@")]
    [Arguments("@$\"test", "@$\\\"test", DisplayName = "InitialValue = starts with @$")]
    public async Task WithInitialValue_StringAutoParenthesis(string initialValue, string expected)
    {
        var propertyInfo = new PropertyInfo("string", "MyProperty")
        {
            InitialValue = initialValue
        };

        var src = propertyInfo.ToString();

        await Assert.That(src).IsEqualTo($@"public string MyProperty {{ get; set; }} = ""{expected}"";");
    }

    [Test]
    [Arguments("\"test\"", DisplayName = "InitialValue = no additional parenthesis")]
    [Arguments("\"te\"\"st\"", DisplayName = "InitialValue = contains double parenthesis")]
    [Arguments("\"te\"st\"", DisplayName = "InitialValue = contains single parenthesis")]
    [Arguments("@\"test\"", DisplayName = "InitialValue = starts with @")]
    [Arguments("$\"test\"", DisplayName = "InitialValue = starts with $")]
    [Arguments("$@\"test\"", DisplayName = "InitialValue = starts with $@")]
    [Arguments("@$\"test\"", DisplayName = "InitialValue = starts with @$")]
    public async Task WithInitialValue_StringManualParenthesis(string initialValue)
    {
        var propertyInfo = new PropertyInfo("string", "MyProperty")
        {
            InitialValue = initialValue
        };

        var src = propertyInfo.ToString();

        await Assert.That(src).IsEqualTo($@"public string MyProperty {{ get; set; }} = {initialValue};");
    }

    [Test]
    public async Task AutoImplemented_GetOnly()
    {
        var propertyInfo = new PropertyInfo("int", "MyProperty")
        {
            HasSetter = false
        };

        var src = propertyInfo.ToString();

        await Assert.That(src).IsEqualTo(@"public int MyProperty { get; }");
    }

    [Test]
    public async Task AutoImplemented_SetOnly()
    {
        var propertyInfo = new PropertyInfo("int", "MyProperty")
        {
            HasGetter = false
        };

        var src = propertyInfo.ToString();

        await Assert.That(src).IsEqualTo(@"public int MyProperty { set; }");
    }

    [Test]
    public async Task AutoImplemented_GetAndSetFallback()
    {
        var propertyInfo = new PropertyInfo("int", "MyProperty")
        {
            HasGetter = false,
            HasSetter = false
        };

        var src = propertyInfo.ToString();

        await Assert.That(src).IsEqualTo(@"public int MyProperty { get; set; }");
    }

    [Test]
    public async Task WithGetterBody_AutoSetter()
    {
        var propertyInfo = new PropertyInfo("int", "MyProperty");

        propertyInfo.GetterBody.Add(@"
return _myProperty;
");

        var src = propertyInfo.ToString(Language.CSharp, false);
        var fieldName = propertyInfo.FieldName;

        await Assert.That(string.IsNullOrWhiteSpace(fieldName)).IsFalse();

        await Assert.That(src).IsEqualTo($@"public int MyProperty
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
}}");
    }

    [Test]
    public async Task WithSetterBody_AutoGetter()
    {
        var propertyInfo = new PropertyInfo("int", "MyProperty");

        propertyInfo.SetterBody.Add(@"
_myProperty = value;
");

        var src = propertyInfo.ToString(false);
        var fieldName = propertyInfo.FieldName;

        await Assert.That(string.IsNullOrWhiteSpace(fieldName)).IsFalse();

        await Assert.That(src).IsEqualTo($@"public int MyProperty
{{
    get
    {{
        return this.{fieldName};
    }}
    set
    {{
        _myProperty = value;
    }}
}}");
    }

    [Test]
    public async Task WithGetterAndSetterBody()
    {
        var propertyInfo = new PropertyInfo("int", "MyProperty");

        propertyInfo.GetterBody.Add(@"
return _myProperty;
");

        propertyInfo.SetterBody.Add(@"
_myProperty = value;
");

        var src = propertyInfo.ToString();

        await Assert.That(src).IsEqualTo(@"public int MyProperty
{
    get
    {
        return _myProperty;
    }
    set
    {
        _myProperty = value;
    }
}");
    }

    [Test]
    public async Task WithGetterBody_NoSetter()
    {
        var propertyInfo = new PropertyInfo("int", "MyProperty");

        propertyInfo.GetterBody.Add(@"
return _myProperty;
");

        propertyInfo.HasSetter = false;

        var src = propertyInfo.ToString();

        await Assert.That(src).IsEqualTo(@"public int MyProperty
{
    get
    {
        return _myProperty;
    }
}");
    }

    [Test]
    public async Task WithSetterBody_NoGetter()
    {
        var propertyInfo = new PropertyInfo("int", "MyProperty")
        {
            HasGetter = false
        };

        propertyInfo.SetterBody.Add(@"
_myProperty = value;
");

        var src = propertyInfo.ToString();

        await Assert.That(src).IsEqualTo(@"public int MyProperty
{
    set
    {
        _myProperty = value;
    }
}");
    }

    [Test]
    public async Task WithFieldname_GetAndSetFallback()
    {
        var propertyInfo = new PropertyInfo("int", "MyProperty")
        {
            FieldName = "_theField",

            HasGetter = false,
            HasSetter = false
        };

        var src = propertyInfo.ToString(false);

        await Assert.That(src).IsEqualTo(@"public int MyProperty
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
}");
    }

    [Test]
    public async Task AutoImplemented_GetAndSetFallback_AfterResettingFieldname()
    {
        var propertyInfo = new PropertyInfo("int", "MyProperty")
        {
            FieldName = "Test"
        };
        propertyInfo.FieldName = null;

        propertyInfo.HasGetter = false;
        propertyInfo.HasSetter = false;

        var src = propertyInfo.ToString();

        await Assert.That(src).IsEqualTo(@"public int MyProperty { get; set; }");
    }

    [Test]
    public async Task WithBody_Indented()
    {
        var propertyInfo = new PropertyInfo("int", "MyProperty");

        propertyInfo.GetterBody.Add(@"
return _myProperty;
");

        var fieldName = propertyInfo.FieldName;
        await Assert.That(string.IsNullOrWhiteSpace(fieldName)).IsFalse();

        var sb = new System.Text.StringBuilder();
        propertyInfo.ToString(sb, 1, Language.CSharp, false);
        var src = sb.ToString();

        await Assert.That(src).IsEqualTo($@"    public int MyProperty
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
    }}");
    }

    [Test]
    public async Task WithoutBody_Indented()
    {
        var propertyInfo = new PropertyInfo("int", "MyProperty");

        var sb = new System.Text.StringBuilder();
        propertyInfo.ToString(sb, 2, Language.CSharp);
        var src = sb.ToString();

        await Assert.That(src).IsEqualTo(@"        public int MyProperty { get; set; }");
    }

    [Test]
    public async Task AutoImplemented_WithFieldName()
    {
        var propertyInfo = new PropertyInfo("int", "MyProperty")
        {
            FieldName = "_myField"
        };

        var src = propertyInfo.ToString(false);

        await Assert.That(src).IsEqualTo(@"public int MyProperty
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
}");
    }

    [Test]
    public async Task WithXmlDoc_Summary()
    {
        var propertyInfo = new PropertyInfo("int", "MyProperty")
        {
            XmlDocSummary = "This is an important property"
        };

        var src = propertyInfo.ToString();

        await Assert.That(src).IsEqualTo(@"/// <summary>
/// This is an important property
/// </summary>
public int MyProperty { get; set; }");
    }

    [Test]
    public async Task WithXmlDoc_AdditionalLines()
    {
        var propertyInfo = new PropertyInfo("int", "MyProperty");

        propertyInfo.XmlDocAdditionalLines.Add("<example>8</example>");

        var src = propertyInfo.ToString();

        await Assert.That(src).IsEqualTo(@"/// <summary>
/// MyProperty
/// </summary>
/// <example>8</example>
public int MyProperty { get; set; }");
    }

    [Test]
    public async Task WithXmlDoc_InheritDoc()
    {
        var propertyInfo = new PropertyInfo("int", "MyProperty")
        {
            InheritXmlDoc = true
        };

        var src = propertyInfo.ToString();

        await Assert.That(src).IsEqualTo(@"/// <inheritdoc />
public int MyProperty { get; set; }");
    }

    [Test]
    [Arguments(true, true, true, DisplayName = "Body = true, Auto = true, Expected = true")]
    [Arguments(true, false, true, DisplayName = "Body = true, Auto = false, Expected = true")]
    [Arguments(false, true, true, DisplayName = "Body = false, Auto = true, Expected = true")]
    [Arguments(false, false, false, DisplayName = "Body = false, Auto = false, Expected = false")]
    public async Task HasGetter_CorrectStatus(bool hasBody, bool hasAuto, bool expected)
    {
        var propertyInfo = new PropertyInfo("int", "Name")
        {
            HasGetter = hasAuto
        };

        if (hasBody)
            propertyInfo.GetterBody.Add("test");

        await Assert.That(propertyInfo.HasGetter).IsEqualTo(expected);
    }

    [Test]
    [Arguments(true, true, true, DisplayName = "Body = true, Auto = true, Expected = true")]
    [Arguments(true, false, true, DisplayName = "Body = true, Auto = false, Expected = true")]
    [Arguments(false, true, true, DisplayName = "Body = false, Auto = true, Expected = true")]
    [Arguments(false, false, false, DisplayName = "Body = false, Auto = false, Expected = false")]
    public async Task HasSetter_CorrectStatus(bool hasBody, bool hasAuto, bool expected)
    {
        var propertyInfo = new PropertyInfo("int", "Name")
        {
            HasSetter = hasAuto
        };

        if (hasBody)
            propertyInfo.SetterBody.Add("test");

        await Assert.That(propertyInfo.HasSetter).IsEqualTo(expected);
    }

    [Test]
    public async Task Reset_GetterBody()
    {
        var propertyInfo = new PropertyInfo("type", "name")
        {
            HasGetter = false
        };

        propertyInfo.GetterBody.Add("body");

        await Assert.That(propertyInfo.HasGetter).IsTrue();

        propertyInfo.HasGetter = false;

        await Assert.That(propertyInfo.HasGetter).IsFalse();
    }

    [Test]
    public async Task Reset_SetterBody()
    {
        var propertyInfo = new PropertyInfo("type", "name")
        {
            HasSetter = false
        };

        propertyInfo.SetterBody.Add("body");

        await Assert.That(propertyInfo.HasSetter).IsTrue();

        propertyInfo.HasSetter = false;

        await Assert.That(propertyInfo.HasSetter).IsFalse();
    }

    [Test]
    public async Task AdditionalField_StringBuilderNullReturnsFalse()
    {
        var propertyInfo = new PropertyInfo("int", "SomeProperty");

        var wasAppended = propertyInfo.AppendFieldIfNecessary(null!, 2, Language.CSharp, [new FieldInfo("int", "_someField")]);

        await Assert.That(wasAppended).IsFalse();
    }

    [Test]
    public void AdditionalField_VisualBasicNotImplementedYet()
    {
        var propertyInfo = new PropertyInfo("int", "SomeProperty");

        var sb = new System.Text.StringBuilder();

        _ = Assert.ThrowsExactly<NotImplementedException>(() => propertyInfo.AppendFieldIfNecessary(sb, 2, Language.VisualBasic, [new FieldInfo("int", "_someField")]));
    }

    [Test]
    [Arguments(false, DisplayName = "AccessGetterAndSetterProperty = false")]
    [Arguments(true, DisplayName = "AccessGetterAndSetterProperty = true")]
    public async Task AdditionalField_NoFieldForAutoProp(bool accessGetterAndSetterProperty)
    {
        var propertyInfo = new PropertyInfo("int", "SomeProperty");

        if (accessGetterAndSetterProperty)
        {
            var gbdy = propertyInfo.GetterBody;
            var sbdy = propertyInfo.SetterBody;

            await Assert.That(gbdy).IsNotNull();
            await Assert.That(sbdy).IsNotNull();
        }

        var sb = new System.Text.StringBuilder();

        var wasAppended = propertyInfo.AppendFieldIfNecessary(sb, 2, Language.CSharp, [new FieldInfo("int", "_someField")]);

        var src = sb.ToString();

        await Assert.That(wasAppended).IsFalse();
        await Assert.That(src).IsEqualTo("");
    }

    [Test]
    public async Task AdditionalField_NoGetterAndNoSetterFallback()
    {
        var propertyInfo = new PropertyInfo("int", "SomeProperty")
        {
            FieldName = "_myField",
            HasGetter = false,
            HasSetter = false
        };

        var sb = new System.Text.StringBuilder();

        var wasAppended = propertyInfo.AppendFieldIfNecessary(sb, 2, Language.CSharp, [new FieldInfo("int", "_someField")]);

        var src = sb.ToString();

        await Assert.That(wasAppended).IsTrue();
        await Assert.That(src).IsEqualTo("        private int _myField;");
    }

    [Test]
    [Arguments(false, false, true, false, false, false, DisplayName = "AutoGetter = false, AutoSetter = false, GetterBody = true, SetterBody = false, FieldName = false, GeneratesProperty = false")]
    [Arguments(false, false, false, true, false, false, DisplayName = "AutoGetter = false, AutoSetter = false, GetterBody = false, SetterBody = true, FieldName = false, GeneratesProperty = false")]
    [Arguments(false, false, false, false, true, true, DisplayName = "AutoGetter = false, AutoSetter = false, GetterBody = false, SetterBody = false, FieldName = true, GeneratesProperty = true")]
    [Arguments(true, false, true, false, false, false, DisplayName = "AutoGetter = true, AutoSetter = false, GetterBody = true, SetterBody = false, FieldName = false, GeneratesProperty = false")]
    [Arguments(true, false, false, true, false, true, DisplayName = "AutoGetter = true, AutoSetter = false, GetterBody = false, SetterBody = true, FieldName = false, GeneratesProperty = true")]
    [Arguments(true, false, false, false, true, true, DisplayName = "AutoGetter = true, AutoSetter = false, GetterBody = false, SetterBody = false, FieldName = true, GeneratesProperty = true")]
    [Arguments(false, true, true, false, false, true, DisplayName = "AutoGetter = false, AutoSetter = true, GetterBody = true, SetterBody = false, FieldName = false, GeneratesProperty = true")]
    [Arguments(false, true, false, true, false, false, DisplayName = "AutoGetter = false, AutoSetter = true, GetterBody = false, SetterBody = true, FieldName = false, GeneratesProperty = false")]
    [Arguments(false, true, false, false, true, true, DisplayName = "AutoGetter = false, AutoSetter = true, GetterBody = false, SetterBody = false, FieldName = true, GeneratesProperty = true")]
    public async Task AdditionalField_ForExtendedProperty(bool withAutoGetter, bool withAutoSetter, bool withGetterBody, bool withSetterBody, bool withFieldName, bool generatesProperty)
    {
        var propertyInfo = new PropertyInfo("int", "SomeProperty")
        {
            HasGetter = withAutoGetter,
            HasSetter = withAutoSetter
        };

        if (withGetterBody)
            propertyInfo.GetterBody.Add("gb");

        if (withSetterBody)
            propertyInfo.SetterBody.Add("sb");

        if (withFieldName)
            propertyInfo.FieldName = "_myField";

        var sb = new System.Text.StringBuilder();

        var wasAppended = propertyInfo.AppendFieldIfNecessary(sb, 2, Language.CSharp, [new FieldInfo("int", "_someField")]);

        var src = sb.ToString();

        await Assert.That(wasAppended).IsEqualTo(generatesProperty);

        if (generatesProperty)
        {
            await Assert.That(src).IsEqualTo($"        private int {propertyInfo.FieldName};");
        }
    }

    [Test]
    public async Task AdditionalField_ListOfFieldsNull()
    {
        var propertyInfo = new PropertyInfo("int", "SomeProperty")
        {
            FieldName = "_someField"
        };

        var sb = new System.Text.StringBuilder();

        var wasAppended = propertyInfo.AppendFieldIfNecessary(sb, 2, Language.CSharp, null!);

        var src = sb.ToString();

        await Assert.That(wasAppended).IsTrue();
        await Assert.That(src).IsEqualTo("        private int _someField;");
    }

    [Test]
    public async Task AdditionalField_FieldNotInListOfFields()
    {
        var propertyInfo = new PropertyInfo("int", "SomeProperty")
        {
            FieldName = "_someField"
        };

        var sb = new System.Text.StringBuilder();

        var wasAppended = propertyInfo.AppendFieldIfNecessary(sb, 2, Language.CSharp, [new FieldInfo("int", "_someOtherField")]);

        var src = sb.ToString();

        await Assert.That(wasAppended).IsTrue();
        await Assert.That(src).IsEqualTo("        private int _someField;");
    }

    [Test]
    public async Task AdditionalField_FieldAlreadyInListOfFields()
    {
        var propertyInfo = new PropertyInfo("int", "SomeProperty")
        {
            FieldName = "_someField"
        };

        var sb = new System.Text.StringBuilder();

        var wasAppended = propertyInfo.AppendFieldIfNecessary(sb, 2, Language.CSharp, [new FieldInfo("int", "_someField")]);

        var src = sb.ToString();

        await Assert.That(wasAppended).IsFalse();
        await Assert.That(src).IsEqualTo(string.Empty);
    }

    [Test]
    public async Task AdditionalField_WithIntialValue()
    {
        var propertyInfo = new PropertyInfo("int", "SomeProperty")
        {
            FieldName = "_someField",
            InitialValue = "9"
        };

        var sb = new System.Text.StringBuilder();

        var wasAppended = propertyInfo.AppendFieldIfNecessary(sb, 2, Language.CSharp, [new FieldInfo("int", "_someOtherField")]);

        var src = sb.ToString();

        await Assert.That(wasAppended).IsTrue();
        await Assert.That(src).IsEqualTo(@"        private int _someField = 9;");
    }

    [Test]
    public async Task IncludeBackingField()
    {
        var propertyInfo = new PropertyInfo("int", "PropertyName")
        {
            FieldName = "_fieldForPropertyName"
        };

        var src = propertyInfo.ToString();

        await Assert.That(src).IsEqualTo(@"private int _fieldForPropertyName;
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
}");
    }

    [Test]
    public async Task IsRequired()
    {
        var propertyInfo = new PropertyInfo("int", "MyProperty")
        {
            IsRequired = true
        };

        var src = propertyInfo.ToString();

        await Assert.That(src).IsEqualTo(@"public required int MyProperty { get; set; }");
    }

    [Test]
    public async Task IncludeAttributes()
    {
        var propertyInfo = new PropertyInfo("int", "MyProperty");

        propertyInfo.Attributes.Add(new AttributeInfo("MyAttribute1"));

        var ai = new AttributeInfo("MyAttribute2");
        ai.ConstructorParameters.Add(new AttributeConstructorParameter("typeof(string)"));
        ai.ConstructorParameters.Add(new AttributeConstructorParameter("param2", "typeof(int)"));
        propertyInfo.Attributes.Add(ai);

        var src = propertyInfo.ToString();

        await Assert.That(src).IsEqualTo(@"[MyAttribute1]
[MyAttribute2(typeof(string), param2: typeof(int))]
public int MyProperty { get; set; }");
    }
}