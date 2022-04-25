﻿using Basilisque.CodeAnalysis.Syntax;

namespace Basilisque.CodeAnalysis.Tests.Syntax
{
    [TestClass]
    public class ClassInfoTests
    {
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
        public void EmptyClass_CorrectString()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.ProtectedInternal);

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"protected internal class TestClass1
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
            classInfo.GenericTypes.Add("T1", new List<string>() { });

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"private class TestClass1<T1>
{
}", classStr);
        }

        [TestMethod]
        public void With3GenericTypes_NoConstraints_NoBaseClass()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
            classInfo.GenericTypes.Add("T1", null);
            classInfo.GenericTypes.Add("T2", null);
            classInfo.GenericTypes.Add("T3", new List<string>() { });

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"private class TestClass1<T1, T2, T3>
{
}", classStr);
        }

        [TestMethod]
        public void With1GenericType_NoConstraints_WithBaseClass()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
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
            classInfo.GenericTypes.Add("T1", new List<string>() { "class", "new()" });

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
            classInfo.GenericTypes.Add("T1", null);
            classInfo.GenericTypes.Add("T2", new List<string>() { "class" });
            classInfo.GenericTypes.Add("T3", new List<string>() { });

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
            classInfo.BaseClass = "MyBaseClass1";
            classInfo.GenericTypes.Add("T1", new List<string>() { "class", "new()" });

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"private class TestClass1<T1> : MyBaseClass1
    where T1 : class, new()
{
}", classStr);
        }

        [TestMethod]
        public void With3GenericTypes_WithConstraints_WithBaseClass()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Private);
            classInfo.BaseClass = "MyBaseClass1";
            classInfo.GenericTypes.Add("T1", null);
            classInfo.GenericTypes.Add("T2", new List<string>() { "class", "new()" });
            classInfo.GenericTypes.Add("T3", new List<string>() { "enum" });

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"private class TestClass1<T1, T2, T3> : MyBaseClass1
    where T2 : class, new()
    where T3 : enum
{
}", classStr);
        }

        [TestMethod]
        public void BaseClass_CanBeChanged()
        {
            var classInfo = new ClassInfo("TestClass1", AccessModifier.Internal);
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
            classInfo.ClassName = "ChangedTestClassName";

            Assert.AreEqual("ChangedTestClassName", classInfo.ClassName);

            var classStr = classInfo.ToString();

            Assert.AreEqual(@"internal class ChangedTestClassName
{
}", classStr);
        }
    }
}
