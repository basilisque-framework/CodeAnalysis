using Basilisque.CodeAnalysis.Syntax;

namespace Basilisque.CodeAnalysis.Tests.Syntax;

[TestClass]
public class AttributeInfoTests
{
    [DataTestMethod]
    [DataRow(null, DisplayName = "Name: null")]
    [DataRow("", DisplayName = "Name: ''")]
    [DataRow("  ", DisplayName = "Name: '  '")]
    public void InvalidName_ThrowsArgumentNullException(string name)
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
        {
            _ = new AttributeInfo(name);
        });
    }

    [DataTestMethod]
    [DataRow("MyAttribute")]
    [DataRow("Name.Space.MyAttribute")]
    [DataRow("Name.Space.MyAttribute()")]
    [DataRow("Name.Space.MyAttribute(\"MyParam1\")")]
    public void NoParameters_GeneratesCorrectString(string name)
    {
        var attributeInfo = new AttributeInfo(name);

        var expected = $"[{name}]";

        Assert.AreEqual(expected, attributeInfo.ToString(Language.CSharp));
    }

    [TestMethod]
    public void With_1_ConstructorParameter_And_0_NamedParameters()
    {
        var attributeInfo = new AttributeInfo("MyAttribute");

        attributeInfo.ConstructorParameters.Add(new AttributeConstructorParameter("typeof(string)"));

        var expected = $"[MyAttribute(typeof(string))]";

        Assert.AreEqual(expected, attributeInfo.ToString(Language.CSharp));
    }

    [TestMethod]
    public void With_2_ConstructorParameters_And_0_NamedParameters()
    {
        var attributeInfo = new AttributeInfo("MyAttribute");

        attributeInfo.ConstructorParameters.Add(new AttributeConstructorParameter("typeof(string)"));
        attributeInfo.ConstructorParameters.Add(new AttributeConstructorParameter("\"MyString\""));

        var expected = $"[MyAttribute(typeof(string), \"MyString\")]";

        Assert.AreEqual(expected, attributeInfo.ToString(Language.CSharp));
    }

    [TestMethod]
    public void With_2_Named_ConstructorParameters_And_0_NamedParameters()
    {
        var attributeInfo = new AttributeInfo("MyAttribute");

        attributeInfo.ConstructorParameters.Add(new AttributeConstructorParameter("typeof(string)"));
        attributeInfo.ConstructorParameters.Add(new AttributeConstructorParameter("myParam2", "\"MyString\""));

        var expected = $"[MyAttribute(typeof(string), myParam2: \"MyString\")]";

        Assert.AreEqual(expected, attributeInfo.ToString(Language.CSharp));
    }

    [TestMethod]
    public void With_2_Named_ConstructorParameters_And_2_NamedParameters()
    {
        var attributeInfo = new AttributeInfo("MyAttribute");

        attributeInfo.ConstructorParameters.Add(new AttributeConstructorParameter("typeof(string)"));
        attributeInfo.ConstructorParameters.Add(new AttributeConstructorParameter("myParam2", "\"MyString\""));
        attributeInfo.NamedParameters.Add(new AttributeNamedParameter("MyNamed1", "\"Value1\""));
        attributeInfo.NamedParameters.Add(new AttributeNamedParameter("MyNamed2", "8"));

        var expected = $"[MyAttribute(typeof(string), myParam2: \"MyString\", MyNamed1 = \"Value1\", MyNamed2 = 8)]";

        Assert.AreEqual(expected, attributeInfo.ToString(Language.CSharp));
    }

    [TestMethod]
    public void With_0_ConstructorParameters_And_2_NamedParameters()
    {
        var attributeInfo = new AttributeInfo("MyAttribute");

        attributeInfo.NamedParameters.Add(new AttributeNamedParameter("MyNamed1", "\"Value1\""));
        attributeInfo.NamedParameters.Add(new AttributeNamedParameter("MyNamed2", "8"));

        var expected = $"[MyAttribute(MyNamed1 = \"Value1\", MyNamed2 = 8)]";

        Assert.AreEqual(expected, attributeInfo.ToString(Language.CSharp));
    }
}
