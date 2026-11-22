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
    public class AccessModifierTests
    {
        [Test]
        [Arguments(AccessModifier.Public, "public")]
        [Arguments(AccessModifier.Internal, "internal")]
        [Arguments(AccessModifier.Protected, "protected")]
        [Arguments(AccessModifier.Private, "private")]
        [Arguments(AccessModifier.ProtectedInternal, "protected internal")]
        [Arguments(AccessModifier.PrivateProtected, "private protected")]
        public async Task ToKeywordString_ReturnsValidString(AccessModifier accessModifier, string expectedResult)
        {
            string keyword = accessModifier.ToKeywordString();

            await Assert.That(keyword).IsEqualTo(expectedResult);
        }

        [Test]
        public async Task ToKeywordString_CanBeCalledAsNormalFunction()
        {
            string keyword = AccessModifierExtensions.ToKeywordString(AccessModifier.Internal);

            await Assert.That(keyword).IsEqualTo("internal");
        }

        [Test]
        public void ToKeywordString_ThrowsForInvalidValue()
        {
            Assert.ThrowsExactly<NotSupportedException>(() =>
            {
                AccessModifierExtensions.ToKeywordString((AccessModifier)20);
            });
        }

        [Test]
        public async Task GetAccessModifier_ClassWithPublicModifier_ReturnsPublic()
        {
            var classDeclaration = SyntaxFactory.ClassDeclaration("TestClass")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            var result = classDeclaration.GetAccessModifier();

            await Assert.That(result).IsEqualTo(AccessModifier.Public);
        }

        [Test]
        public async Task GetAccessModifier_ClassWithInternalModifier_ReturnsInternal()
        {
            var classDeclaration = SyntaxFactory.ClassDeclaration("TestClass")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.InternalKeyword));

            var result = classDeclaration.GetAccessModifier();

            await Assert.That(result).IsEqualTo(AccessModifier.Internal);
        }

        [Test]
        public async Task GetAccessModifier_ClassWithProtectedInternalModifier_ReturnsProtectedInternal()
        {
            var classDeclaration = SyntaxFactory.ClassDeclaration("TestClass")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword), SyntaxFactory.Token(SyntaxKind.InternalKeyword));

            var result = classDeclaration.GetAccessModifier();

            await Assert.That(result).IsEqualTo(AccessModifier.ProtectedInternal);
        }

        [Test]
        public async Task GetAccessModifier_ClassWithPrivateProtectedModifier_ReturnsPrivateProtected()
        {
            var classDeclaration = SyntaxFactory.ClassDeclaration("TestClass")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.ProtectedKeyword));

            var result = classDeclaration.GetAccessModifier();

            await Assert.That(result).IsEqualTo(AccessModifier.PrivateProtected);
        }

        [Test]
        public async Task GetAccessModifier_ClassWithProtectedModifier_ReturnsProtected()
        {
            var classDeclaration = SyntaxFactory.ClassDeclaration("TestClass")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword));

            var result = classDeclaration.GetAccessModifier();

            await Assert.That(result).IsEqualTo(AccessModifier.Protected);
        }

        [Test]
        public async Task GetAccessModifier_ClassWithPrivateModifier_ReturnsPrivate()
        {
            var classDeclaration = SyntaxFactory.ClassDeclaration("TestClass")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));

            var result = classDeclaration.GetAccessModifier();

            await Assert.That(result).IsEqualTo(AccessModifier.Private);
        }

        [Test]
        public async Task GetAccessModifier_ClassWithNoModifier_ReturnsInternal()
        {
            var classDeclaration = SyntaxFactory.ClassDeclaration("TestClass");

            var result = classDeclaration.GetAccessModifier();

            await Assert.That(result).IsEqualTo(AccessModifier.Internal);
        }

        [Test]
        public async Task GetAccessModifier_PropertyWithPublicModifier_ReturnsPublic()
        {
            var propertyDeclaration = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("int"), "TestProperty")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            var result = propertyDeclaration.GetAccessModifier();

            await Assert.That(result).IsEqualTo(AccessModifier.Public);
        }

        [Test]
        public async Task GetAccessModifier_PropertyWithInternalModifier_ReturnsInternal()
        {
            var propertyDeclaration = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("int"), "TestProperty")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.InternalKeyword));

            var result = propertyDeclaration.GetAccessModifier();

            await Assert.That(result).IsEqualTo(AccessModifier.Internal);
        }

        [Test]
        public async Task GetAccessModifier_PropertyWithProtectedInternalModifier_ReturnsProtectedInternal()
        {
            var propertyDeclaration = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("int"), "TestProperty")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword), SyntaxFactory.Token(SyntaxKind.InternalKeyword));

            var result = propertyDeclaration.GetAccessModifier();

            await Assert.That(result).IsEqualTo(AccessModifier.ProtectedInternal);
        }

        [Test]
        public async Task GetAccessModifier_PropertyWithPrivateProtectedModifier_ReturnsPrivateProtected()
        {
            var propertyDeclaration = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("int"), "TestProperty")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.ProtectedKeyword));

            var result = propertyDeclaration.GetAccessModifier();

            await Assert.That(result).IsEqualTo(AccessModifier.PrivateProtected);
        }

        [Test]
        public async Task GetAccessModifier_PropertyWithProtectedModifier_ReturnsProtected()
        {
            var propertyDeclaration = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("int"), "TestProperty")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword));

            var result = propertyDeclaration.GetAccessModifier();

            await Assert.That(result).IsEqualTo(AccessModifier.Protected);
        }

        [Test]
        public async Task GetAccessModifier_PropertyWithPrivateModifier_ReturnsPrivate()
        {
            var propertyDeclaration = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("int"), "TestProperty")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));

            var result = propertyDeclaration.GetAccessModifier();

            await Assert.That(result).IsEqualTo(AccessModifier.Private);
        }

        [Test]
        public async Task GetAccessModifier_PropertyWithNoModifier_ReturnsPrivate()
        {
            var propertyDeclaration = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("int"), "TestProperty");

            var result = propertyDeclaration.GetAccessModifier();

            await Assert.That(result).IsEqualTo(AccessModifier.Private);
        }

        [Test]
        public async Task GetAccessModifier_EventWithPublicModifier_ReturnsPublic()
        {
            var eventDeclaration = SyntaxFactory.EventDeclaration(SyntaxFactory.ParseTypeName("EventHandler"), "TestEvent")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            var result = eventDeclaration.GetAccessModifier();

            await Assert.That(result).IsEqualTo(AccessModifier.Public);
        }

        [Test]
        public async Task GetAccessModifier_EventWithNoModifier_ReturnsPrivate()
        {
            var eventDeclaration = SyntaxFactory.EventDeclaration(SyntaxFactory.ParseTypeName("EventHandler"), "TestEvent");

            var result = eventDeclaration.GetAccessModifier();

            await Assert.That(result).IsEqualTo(AccessModifier.Private);
        }

        [Test]
        public async Task GetAccessModifier_FieldWithPublicModifier_ReturnsPublic()
        {
            var fieldDeclaration = SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName("int"))
                    .AddVariables(SyntaxFactory.VariableDeclarator("TestField")))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            var result = fieldDeclaration.GetAccessModifier();

            await Assert.That(result).IsEqualTo(AccessModifier.Public);
        }

        [Test]
        public async Task GetAccessModifier_FieldWithNoModifier_ReturnsPrivate()
        {
            var fieldDeclaration = SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName("int"))
                    .AddVariables(SyntaxFactory.VariableDeclarator("TestField")));

            var result = fieldDeclaration.GetAccessModifier();

            await Assert.That(result).IsEqualTo(AccessModifier.Private);
        }

        [Test]
        public async Task GetAccessModifier_MethodWithPublicModifier_ReturnsPublic()
        {
            var methodDeclaration = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("void"), "TestMethod")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            var result = methodDeclaration.GetAccessModifier();

            await Assert.That(result).IsEqualTo(AccessModifier.Public);
        }

        [Test]
        public async Task GetAccessModifier_MethodWithNoModifier_ReturnsPrivate()
        {
            var methodDeclaration = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("void"), "TestMethod");

            var result = methodDeclaration.GetAccessModifier();

            await Assert.That(result).IsEqualTo(AccessModifier.Private);
        }

        [Test]
        public async Task GetAccessModifier_ConstructorWithPublicModifier_ReturnsPublic()
        {
            var constructorDeclaration = SyntaxFactory.ConstructorDeclaration("TestConstructor")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            var result = constructorDeclaration.GetAccessModifier();

            await Assert.That(result).IsEqualTo(AccessModifier.Public);
        }

        [Test]
        public async Task GetAccessModifier_ConstructorWithNoModifier_ReturnsPublic()
        {
            var constructorDeclaration = SyntaxFactory.ConstructorDeclaration("TestConstructor");

            var result = constructorDeclaration.GetAccessModifier();

            await Assert.That(result).IsEqualTo(AccessModifier.Public);
        }

        [Test]
        [Arguments(AccessModifier.Private, Accessibility.Private)]
        [Arguments(AccessModifier.PrivateProtected, Accessibility.ProtectedAndInternal)]
        [Arguments(AccessModifier.Protected, Accessibility.Protected)]
        [Arguments(AccessModifier.Internal, Accessibility.Internal)]
        [Arguments(AccessModifier.ProtectedInternal, Accessibility.ProtectedOrInternal)]
        [Arguments(AccessModifier.Public, Accessibility.Public)]
        public async Task ToAccessibility_ReturnsCorrectValue(AccessModifier src, Accessibility target)
        {
            var result = src.ToAccessibility();

            await Assert.That(result).IsEqualTo(target);
        }

        [Test]
        [Arguments(Accessibility.Private, AccessModifier.Private)]
        [Arguments(Accessibility.ProtectedAndInternal, AccessModifier.PrivateProtected)]
        [Arguments(Accessibility.Protected, AccessModifier.Protected)]
        [Arguments(Accessibility.Internal, AccessModifier.Internal)]
        [Arguments(Accessibility.ProtectedOrInternal, AccessModifier.ProtectedInternal)]
        [Arguments(Accessibility.Public, AccessModifier.Public)]
        public async Task ToToAccessModifier_ReturnsCorrectValue(Accessibility src, AccessModifier target)
        {
            var result = src.ToAccessModifier();

            await Assert.That(result).IsEqualTo(target);
        }

        [Test]
        public void ToAccessModifier_WithNotApplicable_ThrowsException()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            {
                Accessibility.NotApplicable.ToAccessModifier();
            });
        }

        [Test]
        [Arguments(AccessModifier.Protected)]
        [Arguments(AccessModifier.PrivateProtected)]
        public async Task ToAccessibility_WithDefault_ReturnsCorrectValue(AccessModifier defaultValue)
        {
            var result = Accessibility.NotApplicable.ToAccessModifier(defaultValue);

            await Assert.That(result).IsEqualTo(defaultValue);
        }





        [Test]
        [Arguments(AccessModifier.Private, Accessibility.Private)]
        [Arguments(AccessModifier.PrivateProtected, Accessibility.ProtectedAndInternal)]
        [Arguments(AccessModifier.Protected, Accessibility.Protected)]
        [Arguments(AccessModifier.Internal, Accessibility.Internal)]
        [Arguments(AccessModifier.ProtectedInternal, Accessibility.ProtectedOrInternal)]
        [Arguments(AccessModifier.Public, Accessibility.Public)]
        public async Task Casting_AccessModifier_To_Accessibility_ReturnsCorrectValue(AccessModifier src, Accessibility target)
        {
            var result = (Accessibility)src;

            await Assert.That(result).IsEqualTo(target);
        }

        [Test]
        [Arguments(Accessibility.Private, AccessModifier.Private)]
        [Arguments(Accessibility.ProtectedAndInternal, AccessModifier.PrivateProtected)]
        [Arguments(Accessibility.Protected, AccessModifier.Protected)]
        [Arguments(Accessibility.Internal, AccessModifier.Internal)]
        [Arguments(Accessibility.ProtectedOrInternal, AccessModifier.ProtectedInternal)]
        [Arguments(Accessibility.Public, AccessModifier.Public)]
        public async Task Casting_Accessibility_To_AccessModifier_ReturnsCorrectValue(Accessibility src, AccessModifier target)
        {
            var result = (AccessModifier)src;

            await Assert.That(result).IsEqualTo(target);
        }

        [Test]
        public async Task Ensure_AccessModifier_SupportsAllValuesOf_Accessibility_AndViceVersa()
        {
            var accessibilityValues = Enum.GetValues<Accessibility>();
            var accessModifierValues = Enum.GetValues<AccessModifier>();

            var unmappedAV = accessibilityValues.Where(av => av != Accessibility.NotApplicable && !accessModifierValues.Contains((AccessModifier)av)).ToList();
            var unmappedAMV = accessModifierValues.Where(amv => !accessibilityValues.Contains((Accessibility)amv)).ToList();

            await Assert.That(unmappedAV.Count).IsEqualTo(0);
            await Assert.That(unmappedAMV.Count).IsEqualTo(0);
        }
    }
}
