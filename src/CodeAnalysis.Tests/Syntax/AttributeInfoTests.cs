using Basilisque.CodeAnalysis.Syntax;

namespace Basilisque.CodeAnalysis.Tests.Syntax;

public class AttributeInfoTests
{
    [Test]
    [Arguments(null, DisplayName = "Name: null")]
    [Arguments("", DisplayName = "Name: ''")]
    [Arguments("  ", DisplayName = "Name: '  '")]
    public void InvalidName_ThrowsArgumentNullException(string? name)
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
        {
            _ = new AttributeInfo(name!);
        });
    }

    [Test]
    [Arguments("MyAttribute")]
    [Arguments("Name.Space.MyAttribute")]
    [Arguments("Name.Space.MyAttribute()")]
    [Arguments("Name.Space.MyAttribute(\"MyParam1\")")]
    public async Task NoParameters_GeneratesCorrectString(string name)
    {
        var attributeInfo = new AttributeInfo(name);

        var expected = $"[{name}]";

        await Assert.That(attributeInfo.ToString(Language.CSharp)).IsEqualTo(expected);
    }

    [Test]
    public async Task With_1_ConstructorParameter_And_0_NamedParameters()
    {
        var attributeInfo = new AttributeInfo("MyAttribute");

        attributeInfo.ConstructorParameters.Add(new AttributeConstructorParameter("typeof(string)"));

        var expected = $"[MyAttribute(typeof(string))]";

        await Assert.That(attributeInfo.ToString(Language.CSharp)).IsEqualTo(expected);
    }

    [Test]
    public async Task With_2_ConstructorParameters_And_0_NamedParameters()
    {
        var attributeInfo = new AttributeInfo("MyAttribute");

        attributeInfo.ConstructorParameters.Add(new AttributeConstructorParameter("typeof(string)"));
        attributeInfo.ConstructorParameters.Add(new AttributeConstructorParameter("\"MyString\""));

        var expected = $"[MyAttribute(typeof(string), \"MyString\")]";

        await Assert.That(attributeInfo.ToString(Language.CSharp)).IsEqualTo(expected);
    }

    [Test]
    public async Task With_2_Named_ConstructorParameters_And_0_NamedParameters()
    {
        var attributeInfo = new AttributeInfo("MyAttribute");

        attributeInfo.ConstructorParameters.Add(new AttributeConstructorParameter("typeof(string)"));
        attributeInfo.ConstructorParameters.Add(new AttributeConstructorParameter("myParam2", "\"MyString\""));

        var expected = $"[MyAttribute(typeof(string), myParam2: \"MyString\")]";

        await Assert.That(attributeInfo.ToString(Language.CSharp)).IsEqualTo(expected);
    }

    [Test]
    public async Task With_2_Named_ConstructorParameters_And_2_NamedParameters()
    {
        var attributeInfo = new AttributeInfo("MyAttribute");

        attributeInfo.ConstructorParameters.Add(new AttributeConstructorParameter("typeof(string)"));
        attributeInfo.ConstructorParameters.Add(new AttributeConstructorParameter("myParam2", "\"MyString\""));
        attributeInfo.NamedParameters.Add(new AttributeNamedParameter("MyNamed1", "\"Value1\""));
        attributeInfo.NamedParameters.Add(new AttributeNamedParameter("MyNamed2", "8"));

        var expected = $"[MyAttribute(typeof(string), myParam2: \"MyString\", MyNamed1 = \"Value1\", MyNamed2 = 8)]";

        await Assert.That(attributeInfo.ToString(Language.CSharp)).IsEqualTo(expected);
    }

    [Test]
    public async Task With_0_ConstructorParameters_And_2_NamedParameters()
    {
        var attributeInfo = new AttributeInfo("MyAttribute");

        attributeInfo.NamedParameters.Add(new AttributeNamedParameter("MyNamed1", "\"Value1\""));
        attributeInfo.NamedParameters.Add(new AttributeNamedParameter("MyNamed2", "8"));

        var expected = $"[MyAttribute(MyNamed1 = \"Value1\", MyNamed2 = 8)]";

        await Assert.That(attributeInfo.ToString(Language.CSharp)).IsEqualTo(expected);
    }
}
