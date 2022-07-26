using Basilisque.CodeAnalysis.Syntax;

namespace Basilisque.CodeAnalysis.Tests.Syntax
{
    [TestClass]
    public class FieldInfoTests
    {
        [DataTestMethod]
        [DataRow(null, DisplayName = "Type = null")]
        [DataRow("", DisplayName = "Type = \"\"")]
        [DataRow("   ", DisplayName = "Type = \"   \"")]
        public void EmptyType_ThrowsException(string type)
        {
            Assert.ThrowsException<ArgumentNullException>(() => new FieldInfo(type, "name"));
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "Name = null")]
        [DataRow("", DisplayName = "Name = \"\"")]
        [DataRow("   ", DisplayName = "Name = \"   \"")]
        public void EmptyName_ThrowsException(string name)
        {
            Assert.ThrowsException<ArgumentNullException>(() => new FieldInfo("type", name));
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "Type = null")]
        [DataRow("", DisplayName = "Type = \"\"")]
        [DataRow("   ", DisplayName = "Type = \"   \"")]
        public void EmptyTypeProperty_ThrowsException(string type)
        {
            var fieldInfo = new FieldInfo("type", "name");

            Assert.ThrowsException<ArgumentNullException>(() => fieldInfo.Type = type);
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "Type = null")]
        [DataRow("", DisplayName = "Type = \"\"")]
        [DataRow("   ", DisplayName = "Type = \"   \"")]
        public void EmptyNameProperty_ThrowsException(string name)
        {
            var fieldInfo = new FieldInfo("type", "name");

            Assert.ThrowsException<ArgumentNullException>(() => fieldInfo.Name = name);
        }

        [TestMethod]
        public void CanChangeType()
        {
            var fieldInfo = new FieldInfo("type", "name");

            Assert.AreEqual("type", fieldInfo.Type);

            fieldInfo.Type = "type2";

            Assert.AreEqual("type2", fieldInfo.Type);
        }

        [TestMethod]
        public void CanChangeName()
        {
            var fieldInfo = new FieldInfo("type", "name");

            Assert.AreEqual("name", fieldInfo.Name);

            fieldInfo.Name = "name2";

            Assert.AreEqual("name2", fieldInfo.Name);
        }

        [TestMethod]
        public void WithoutInitialValue()
        {
            var fieldInfo = new FieldInfo("int", "_myField");

            var src = fieldInfo.ToString();

            Assert.AreEqual(@"private int _myField;", src);
        }

        [TestMethod]
        public void WithInitialValue()
        {
            var fieldInfo = new FieldInfo("int", "_myField", "8");

            var src = fieldInfo.ToString();

            Assert.AreEqual(@"private int _myField = 8;", src);
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
            var fieldInfo = new FieldInfo("string", "_myField", initialValue);

            var src = fieldInfo.ToString();

            Assert.AreEqual($@"private string _myField = ""{expected}"";", src);
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
            var fieldInfo = new FieldInfo("string", "_myField", initialValue);

            var src = fieldInfo.ToString();

            Assert.AreEqual($@"private string _myField = {initialValue};", src);
        }

        [TestMethod]
        public void WithAccessModifierInternal()
        {
            var fieldInfo = new FieldInfo("int", "_myField", "8");

            fieldInfo.AccessModifier = AccessModifier.Internal;

            var src = fieldInfo.ToString();

            Assert.AreEqual(@"internal int _myField = 8;", src);
        }

        [TestMethod]
        public void WithXmlDoc_Summary()
        {
            var fieldInfo = new FieldInfo("int", "_myField", "8");

            fieldInfo.XmlDocSummary = "This is my special summary";

            var src = fieldInfo.ToString();

            Assert.AreEqual(@"/// <summary>
/// This is my special summary
/// </summary>
private int _myField = 8;", src);
        }

        [TestMethod]
        public void WithXmlDoc_AdditionalLines()
        {
            var fieldInfo = new FieldInfo("int", "_myField", "8");

            fieldInfo.XmlDocAdditionalLines.Add("<myTag>This is my tag</myTag>");

            var src = fieldInfo.ToString();

            Assert.AreEqual(@"/// <summary>
/// _myField
/// </summary>
/// <myTag>This is my tag</myTag>
private int _myField = 8;", src);
        }
    }
}
