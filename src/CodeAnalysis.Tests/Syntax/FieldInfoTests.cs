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

public class FieldInfoTests
{
    [Test]
    [Arguments(null, DisplayName = "Type = null")]
    [Arguments("", DisplayName = "Type = \"\"")]
    [Arguments("   ", DisplayName = "Type = \"   \"")]
    public void EmptyType_ThrowsException(string? type)
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new FieldInfo(type!, "name"));
    }

    [Test]
    [Arguments(null, DisplayName = "Name = null")]
    [Arguments("", DisplayName = "Name = \"\"")]
    [Arguments("   ", DisplayName = "Name = \"   \"")]
    public void EmptyName_ThrowsException(string? name)
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new FieldInfo("type", name!));
    }

    [Test]
    [Arguments(null, DisplayName = "Type = null")]
    [Arguments("", DisplayName = "Type = \"\"")]
    [Arguments("   ", DisplayName = "Type = \"   \"")]
    public void EmptyTypeProperty_ThrowsException(string? type)
    {
        var fieldInfo = new FieldInfo("type", "name");

        Assert.ThrowsExactly<ArgumentNullException>(() => fieldInfo.Type = type!);
    }

    [Test]
    [Arguments(null, DisplayName = "Type = null")]
    [Arguments("", DisplayName = "Type = \"\"")]
    [Arguments("   ", DisplayName = "Type = \"   \"")]
    public void EmptyNameProperty_ThrowsException(string? name)
    {
        var fieldInfo = new FieldInfo("type", "name");

        Assert.ThrowsExactly<ArgumentNullException>(() => fieldInfo.Name = name!);
    }

    [Test]
    public async Task CanChangeType()
    {
        var fieldInfo = new FieldInfo("type", "name");

        await Assert.That(fieldInfo.Type).IsEqualTo("type");

        fieldInfo.Type = "type2";

        await Assert.That(fieldInfo.Type).IsEqualTo("type2");
    }

    [Test]
    public async Task CanChangeName()
    {
        var fieldInfo = new FieldInfo("type", "name");

        await Assert.That(fieldInfo.Name).IsEqualTo("name");

        fieldInfo.Name = "name2";

        await Assert.That(fieldInfo.Name).IsEqualTo("name2");
    }

    [Test]
    public async Task WithoutInitialValue()
    {
        var fieldInfo = new FieldInfo("int", "_myField");

        var src = fieldInfo.ToString();

        await Assert.That(src).IsEqualTo(@"private int _myField;");
    }

    [Test]
    public async Task WithInitialValue()
    {
        var fieldInfo = new FieldInfo("int", "_myField", "8");

        var src = fieldInfo.ToString();

        await Assert.That(src).IsEqualTo(@"private int _myField = 8;");
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
        var fieldInfo = new FieldInfo("string", "_myField", initialValue);

        var src = fieldInfo.ToString();

        await Assert.That(src).IsEqualTo($@"private string _myField = ""{expected}"";");
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
        var fieldInfo = new FieldInfo("string", "_myField", initialValue);

        var src = fieldInfo.ToString();

        await Assert.That(src).IsEqualTo($@"private string _myField = {initialValue};");
    }

    [Test]
    public async Task WithAccessModifierInternal()
    {
        var fieldInfo = new FieldInfo("int", "_myField", "8")
        {
            AccessModifier = AccessModifier.Internal
        };

        var src = fieldInfo.ToString();

        await Assert.That(src).IsEqualTo(@"internal int _myField = 8;");
    }

    [Test]
    public async Task WithXmlDoc_Summary()
    {
        var fieldInfo = new FieldInfo("int", "_myField", "8")
        {
            XmlDocSummary = "This is my special summary"
        };

        var src = fieldInfo.ToString();

        await Assert.That(src).IsEqualTo(@"/// <summary>
/// This is my special summary
/// </summary>
private int _myField = 8;");
    }

    [Test]
    public async Task WithXmlDoc_AdditionalLines()
    {
        var fieldInfo = new FieldInfo("int", "_myField", "8");

        fieldInfo.XmlDocAdditionalLines.Add("<myTag>This is my tag</myTag>");

        var src = fieldInfo.ToString();

        await Assert.That(src).IsEqualTo(@"/// <summary>
/// _myField
/// </summary>
/// <myTag>This is my tag</myTag>
private int _myField = 8;");
    }
}