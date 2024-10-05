/*
   Copyright 2023-2024 Alexander Stärk

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
using Microsoft.CodeAnalysis.CSharp;

namespace Basilisque.CodeAnalysis.Tests.Syntax
{
    [TestClass]
    public class AccessModifierTests
    {
        [DataTestMethod]
        [DataRow(AccessModifier.Public, "public")]
        [DataRow(AccessModifier.Internal, "internal")]
        [DataRow(AccessModifier.Protected, "protected")]
        [DataRow(AccessModifier.Private, "private")]
        [DataRow(AccessModifier.ProtectedInternal, "protected internal")]
        [DataRow(AccessModifier.PrivateProtected, "private protected")]
        public void ToKeywordString_ReturnsValidString(AccessModifier accessModifier, string expectedResult)
        {
            string keyword = accessModifier.ToKeywordString();

            Assert.AreEqual(expectedResult, keyword);
        }

        [TestMethod]
        public void ToKeywordString_CanBeCalledAsNormalFunction()
        {
            string keyword = AccessModifierExtensions.ToKeywordString(AccessModifier.Internal);

            Assert.AreEqual("internal", keyword);
        }

        [TestMethod]
        public void ToKeywordString_ThrowsForInvalidValue()
        {
            Assert.ThrowsException<NotSupportedException>(() =>
            {
                AccessModifierExtensions.ToKeywordString((AccessModifier)20);
            });
        }

        [TestMethod]
        public void GetAccessModifier_ClassWithPublicModifier_ReturnsPublic()
        {
            var classDeclaration = SyntaxFactory.ClassDeclaration("TestClass")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            var result = classDeclaration.GetAccessModifier();

            Assert.AreEqual(AccessModifier.Public, result);
        }

        [TestMethod]
        public void GetAccessModifier_ClassWithInternalModifier_ReturnsInternal()
        {
            var classDeclaration = SyntaxFactory.ClassDeclaration("TestClass")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.InternalKeyword));

            var result = classDeclaration.GetAccessModifier();

            Assert.AreEqual(AccessModifier.Internal, result);
        }

        [TestMethod]
        public void GetAccessModifier_ClassWithProtectedInternalModifier_ReturnsProtectedInternal()
        {
            var classDeclaration = SyntaxFactory.ClassDeclaration("TestClass")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword), SyntaxFactory.Token(SyntaxKind.InternalKeyword));

            var result = classDeclaration.GetAccessModifier();

            Assert.AreEqual(AccessModifier.ProtectedInternal, result);
        }

        [TestMethod]
        public void GetAccessModifier_ClassWithPrivateProtectedModifier_ReturnsPrivateProtected()
        {
            var classDeclaration = SyntaxFactory.ClassDeclaration("TestClass")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.ProtectedKeyword));

            var result = classDeclaration.GetAccessModifier();

            Assert.AreEqual(AccessModifier.PrivateProtected, result);
        }

        [TestMethod]
        public void GetAccessModifier_ClassWithProtectedModifier_ReturnsProtected()
        {
            var classDeclaration = SyntaxFactory.ClassDeclaration("TestClass")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword));

            var result = classDeclaration.GetAccessModifier();

            Assert.AreEqual(AccessModifier.Protected, result);
        }

        [TestMethod]
        public void GetAccessModifier_ClassWithPrivateModifier_ReturnsPrivate()
        {
            var classDeclaration = SyntaxFactory.ClassDeclaration("TestClass")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));

            var result = classDeclaration.GetAccessModifier();

            Assert.AreEqual(AccessModifier.Private, result);
        }

        [TestMethod]
        public void GetAccessModifier_ClassWithNoModifier_ReturnsInternal()
        {
            var classDeclaration = SyntaxFactory.ClassDeclaration("TestClass");

            var result = classDeclaration.GetAccessModifier();

            Assert.AreEqual(AccessModifier.Internal, result);
        }

        [TestMethod]
        public void GetAccessModifier_PropertyWithPublicModifier_ReturnsPublic()
        {
            var propertyDeclaration = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("int"), "TestProperty")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            var result = propertyDeclaration.GetAccessModifier();

            Assert.AreEqual(AccessModifier.Public, result);
        }

        [TestMethod]
        public void GetAccessModifier_PropertyWithInternalModifier_ReturnsInternal()
        {
            var propertyDeclaration = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("int"), "TestProperty")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.InternalKeyword));

            var result = propertyDeclaration.GetAccessModifier();

            Assert.AreEqual(AccessModifier.Internal, result);
        }

        [TestMethod]
        public void GetAccessModifier_PropertyWithProtectedInternalModifier_ReturnsProtectedInternal()
        {
            var propertyDeclaration = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("int"), "TestProperty")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword), SyntaxFactory.Token(SyntaxKind.InternalKeyword));

            var result = propertyDeclaration.GetAccessModifier();

            Assert.AreEqual(AccessModifier.ProtectedInternal, result);
        }

        [TestMethod]
        public void GetAccessModifier_PropertyWithPrivateProtectedModifier_ReturnsPrivateProtected()
        {
            var propertyDeclaration = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("int"), "TestProperty")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.ProtectedKeyword));

            var result = propertyDeclaration.GetAccessModifier();

            Assert.AreEqual(AccessModifier.PrivateProtected, result);
        }

        [TestMethod]
        public void GetAccessModifier_PropertyWithProtectedModifier_ReturnsProtected()
        {
            var propertyDeclaration = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("int"), "TestProperty")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword));

            var result = propertyDeclaration.GetAccessModifier();

            Assert.AreEqual(AccessModifier.Protected, result);
        }

        [TestMethod]
        public void GetAccessModifier_PropertyWithPrivateModifier_ReturnsPrivate()
        {
            var propertyDeclaration = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("int"), "TestProperty")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));

            var result = propertyDeclaration.GetAccessModifier();

            Assert.AreEqual(AccessModifier.Private, result);
        }

        [TestMethod]
        public void GetAccessModifier_PropertyWithNoModifier_ReturnsPrivate()
        {
            var propertyDeclaration = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("int"), "TestProperty");

            var result = propertyDeclaration.GetAccessModifier();

            Assert.AreEqual(AccessModifier.Private, result);
        }

        [TestMethod]
        public void GetAccessModifier_EventWithPublicModifier_ReturnsPublic()
        {
            var eventDeclaration = SyntaxFactory.EventDeclaration(SyntaxFactory.ParseTypeName("EventHandler"), "TestEvent")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            var result = eventDeclaration.GetAccessModifier();

            Assert.AreEqual(AccessModifier.Public, result);
        }

        [TestMethod]
        public void GetAccessModifier_EventWithNoModifier_ReturnsPrivate()
        {
            var eventDeclaration = SyntaxFactory.EventDeclaration(SyntaxFactory.ParseTypeName("EventHandler"), "TestEvent");

            var result = eventDeclaration.GetAccessModifier();

            Assert.AreEqual(AccessModifier.Private, result);
        }

        [TestMethod]
        public void GetAccessModifier_FieldWithPublicModifier_ReturnsPublic()
        {
            var fieldDeclaration = SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName("int"))
                    .AddVariables(SyntaxFactory.VariableDeclarator("TestField")))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            var result = fieldDeclaration.GetAccessModifier();

            Assert.AreEqual(AccessModifier.Public, result);
        }

        [TestMethod]
        public void GetAccessModifier_FieldWithNoModifier_ReturnsPrivate()
        {
            var fieldDeclaration = SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName("int"))
                    .AddVariables(SyntaxFactory.VariableDeclarator("TestField")));

            var result = fieldDeclaration.GetAccessModifier();

            Assert.AreEqual(AccessModifier.Private, result);
        }

        [TestMethod]
        public void GetAccessModifier_MethodWithPublicModifier_ReturnsPublic()
        {
            var methodDeclaration = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("void"), "TestMethod")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            var result = methodDeclaration.GetAccessModifier();

            Assert.AreEqual(AccessModifier.Public, result);
        }

        [TestMethod]
        public void GetAccessModifier_MethodWithNoModifier_ReturnsPrivate()
        {
            var methodDeclaration = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("void"), "TestMethod");

            var result = methodDeclaration.GetAccessModifier();

            Assert.AreEqual(AccessModifier.Private, result);
        }

        [TestMethod]
        public void GetAccessModifier_ConstructorWithPublicModifier_ReturnsPublic()
        {
            var constructorDeclaration = SyntaxFactory.ConstructorDeclaration("TestConstructor")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            var result = constructorDeclaration.GetAccessModifier();

            Assert.AreEqual(AccessModifier.Public, result);
        }

        [TestMethod]
        public void GetAccessModifier_ConstructorWithNoModifier_ReturnsPublic()
        {
            var constructorDeclaration = SyntaxFactory.ConstructorDeclaration("TestConstructor");

            var result = constructorDeclaration.GetAccessModifier();

            Assert.AreEqual(AccessModifier.Public, result);
        }

        [DataTestMethod]
        [DataRow(AccessModifier.Private, Accessibility.Private)]
        [DataRow(AccessModifier.PrivateProtected, Accessibility.ProtectedAndInternal)]
        [DataRow(AccessModifier.Protected, Accessibility.Protected)]
        [DataRow(AccessModifier.Internal, Accessibility.Internal)]
        [DataRow(AccessModifier.ProtectedInternal, Accessibility.ProtectedOrInternal)]
        [DataRow(AccessModifier.Public, Accessibility.Public)]
        public void ToAccessibility_ReturnsCorrectValue(AccessModifier src, Accessibility target)
        {
            var result = src.ToAccessibility();

            Assert.AreEqual(target, result);
        }

        [DataTestMethod]
        [DataRow(Accessibility.Private, AccessModifier.Private)]
        [DataRow(Accessibility.ProtectedAndInternal, AccessModifier.PrivateProtected)]
        [DataRow(Accessibility.Protected, AccessModifier.Protected)]
        [DataRow(Accessibility.Internal, AccessModifier.Internal)]
        [DataRow(Accessibility.ProtectedOrInternal, AccessModifier.ProtectedInternal)]
        [DataRow(Accessibility.Public, AccessModifier.Public)]
        public void ToToAccessModifier_ReturnsCorrectValue(Accessibility src, AccessModifier target)
        {
            var result = src.ToAccessModifier();

            Assert.AreEqual(target, result);
        }

        [TestMethod]
        public void ToAccessModifier_WithNotApplicable_ThrowsException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                Accessibility.NotApplicable.ToAccessModifier();
            });
        }

        [DataTestMethod]
        [DataRow(AccessModifier.Protected)]
        [DataRow(AccessModifier.PrivateProtected)]
        public void ToAccessibility_WithDefault_ReturnsCorrectValue(AccessModifier defaultValue)
        {
            var result = Accessibility.NotApplicable.ToAccessModifier(defaultValue);

            Assert.AreEqual(defaultValue, result);
        }





        [DataTestMethod]
        [DataRow(AccessModifier.Private, Accessibility.Private)]
        [DataRow(AccessModifier.PrivateProtected, Accessibility.ProtectedAndInternal)]
        [DataRow(AccessModifier.Protected, Accessibility.Protected)]
        [DataRow(AccessModifier.Internal, Accessibility.Internal)]
        [DataRow(AccessModifier.ProtectedInternal, Accessibility.ProtectedOrInternal)]
        [DataRow(AccessModifier.Public, Accessibility.Public)]
        public void Casting_AccessModifier_To_Accessibility_ReturnsCorrectValue(AccessModifier src, Accessibility target)
        {
            var result = (Accessibility)src;

            Assert.AreEqual(target, result);
        }

        [DataTestMethod]
        [DataRow(Accessibility.Private, AccessModifier.Private)]
        [DataRow(Accessibility.ProtectedAndInternal, AccessModifier.PrivateProtected)]
        [DataRow(Accessibility.Protected, AccessModifier.Protected)]
        [DataRow(Accessibility.Internal, AccessModifier.Internal)]
        [DataRow(Accessibility.ProtectedOrInternal, AccessModifier.ProtectedInternal)]
        [DataRow(Accessibility.Public, AccessModifier.Public)]
        public void Casting_Accessibility_To_AccessModifier_ReturnsCorrectValue(Accessibility src, AccessModifier target)
        {
            var result = (AccessModifier)src;

            Assert.AreEqual(target, result);
        }

        [TestMethod]
        public void Ensure_AccessModifier_SupportsAllValuesOf_Accessibility_AndViceVersa()
        {
            var accessibilityValues = Enum.GetValues(typeof(Accessibility)).Cast<Accessibility>().ToList();
            var accessModifierValues = Enum.GetValues(typeof(AccessModifier)).Cast<AccessModifier>().ToList();

            var unmappedAV = accessibilityValues.Where(av => av != Accessibility.NotApplicable && !accessModifierValues.Contains((AccessModifier)av)).ToList();
            var unmappedAMV = accessModifierValues.Where(amv => !accessibilityValues.Contains((Accessibility)amv)).ToList();

            Assert.AreEqual(0, unmappedAV.Count);
            Assert.AreEqual(0, unmappedAMV.Count);
        }
    }
}
