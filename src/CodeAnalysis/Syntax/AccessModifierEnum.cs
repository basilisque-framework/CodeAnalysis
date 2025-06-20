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
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Basilisque.CodeAnalysis.Syntax
{
    /// <summary>
    /// Access modifiers are keywords used to specify the declared accessibility of a member or a type.
    /// </summary>
    public enum AccessModifier
    {
        /// <summary>
        /// private - access is limited to the containing type
        /// </summary>
        Private = 1,
        /// <summary>
        /// private protected - access is limited to the containing class or types derived from the containing class within the current assembly
        /// </summary>
        PrivateProtected = 2,
        /// <summary>
        /// protected - access is limited to the containing class or types derived from the containing class
        /// </summary>
        Protected = 3,
        /// <summary>
        /// internal - access is limited to the current assembly
        /// </summary>
        Internal = 4,
        /// <summary>
        /// protected internal - access is limited to the current assembly or types derived from the containing class
        /// </summary>
        ProtectedInternal = 5,
        /// <summary>
        /// public - access is not restricted
        /// </summary>
        Public = 6
    }

    /// <summary>
    /// Extension methods for the <see cref="AccessModifier"/> enumeration
    /// </summary>
    public static class AccessModifierExtensions
    {
        /// <summary>
        /// Maps the <see cref="AccessModifier"/> enumeration to the corresponding C# keyword as string
        /// </summary>
        /// <param name="accessModifier" example="AccessModifier.Public">The <see cref="AccessModifier"/> enumeration that should be mapped</param>
        /// <returns>Returns a string containing the corresponding keyword</returns>
        /// <exception cref="NotSupportedException">Will be thrown for invalid values</exception>
        /// <example>public</example>
        public static string ToKeywordString(this AccessModifier accessModifier)
        {
            switch (accessModifier)
            {
                case AccessModifier.Public:
                case AccessModifier.Internal:
                case AccessModifier.Protected:
                case AccessModifier.Private:
                    return accessModifier.ToString().ToLower();
                case AccessModifier.ProtectedInternal:
                    return "protected internal";
                case AccessModifier.PrivateProtected:
                    return "private protected";
                default:
                    throw new NotSupportedException($"The access modifier '{accessModifier.ToString()}' is currently not supported.");
            }
        }

        /// <summary>
        /// Determines the <see cref="AccessModifier"/> from a <see cref="ClassDeclarationSyntax"/>
        /// </summary>
        /// <param name="classDeclaration">The <see cref="ClassDeclarationSyntax"/> to determine the <see cref="AccessModifier"/> from</param>
        /// <returns>The <see cref="AccessModifier"/> of the provided <see cref="ClassDeclarationSyntax"/></returns>
        public static AccessModifier GetAccessModifier(this ClassDeclarationSyntax classDeclaration)
        {
            var modifier = getAccessModifier(classDeclaration.Modifiers);

            if (modifier.HasValue)
                return modifier.Value;

            return AccessModifier.Internal;
        }

        /// <summary>
        /// Determines the <see cref="AccessModifier"/> from a <see cref="ConstructorDeclarationSyntax"/>
        /// </summary>
        /// <param name="constructorDeclaration">The <see cref="ConstructorDeclarationSyntax"/> to determine the <see cref="AccessModifier"/> from</param>
        /// <returns>The <see cref="AccessModifier"/> of the provided <see cref="ConstructorDeclarationSyntax"/></returns>
        public static AccessModifier GetAccessModifier(this ConstructorDeclarationSyntax constructorDeclaration)
        {
            var modifier = getAccessModifier(constructorDeclaration.Modifiers);

            if (modifier.HasValue)
                return modifier.Value;

            return AccessModifier.Public;
        }

        /// <summary>
        /// Determines the <see cref="AccessModifier"/> from a <see cref="MemberDeclarationSyntax"/>
        /// </summary>
        /// <param name="memberDeclarationSyntax">The <see cref="MemberDeclarationSyntax"/> to determine the <see cref="AccessModifier"/> from</param>
        /// <returns>The <see cref="AccessModifier"/> of the provided <see cref="MemberDeclarationSyntax"/></returns>
        public static AccessModifier GetAccessModifier(this MemberDeclarationSyntax memberDeclarationSyntax)
        {
            var modifier = getAccessModifier(memberDeclarationSyntax.Modifiers);

            if (modifier.HasValue)
                return modifier.Value;

            return AccessModifier.Private;
        }

        /// <summary>
        /// Casts a <see cref="AccessModifier"/> to the corresponding <see cref="Accessibility"/>.
        /// </summary>
        /// <param name="accessModifier">The <see cref="AccessModifier"/> to be castet.</param>
        /// <returns>The corresponding <see cref="Accessibility"/></returns>
        public static Accessibility ToAccessibility(this AccessModifier accessModifier)
        {
            return (Accessibility)accessModifier;
        }

        /// <summary>
        /// Casts a <see cref="Accessibility"/> to the corresponding <see cref="AccessModifier"/>.
        /// Providing <see cref="Accessibility.NotApplicable"/> raises a <see cref="ArgumentOutOfRangeException"/>.
        /// </summary>
        /// <param name="accessibility">The <see cref="Accessibility"/> to be castet.</param>
        /// <returns>The corresponding <see cref="AccessModifier"/></returns>
        /// <exception cref="ArgumentOutOfRangeException">Providing <see cref="Accessibility.NotApplicable"/> raises a <see cref="ArgumentOutOfRangeException"/></exception>
        public static AccessModifier ToAccessModifier(this Accessibility accessibility)
        {
            if ((int)accessibility < 1)
                throw new ArgumentOutOfRangeException(nameof(accessibility));

            return (AccessModifier)accessibility;
        }

        /// <summary>
        /// Casts a <see cref="Accessibility"/> to the corresponding <see cref="AccessModifier"/>.
        /// </summary>
        /// <param name="accessibility">The <see cref="Accessibility"/> to be castet.</param>
        /// <param name="defaultValue">The value that will be returned for <see cref="Accessibility.NotApplicable"/>.</param>
        /// <returns>The corresponding <see cref="AccessModifier"/></returns>
        public static AccessModifier ToAccessModifier(this Accessibility accessibility, AccessModifier defaultValue)
        {
            if ((int)accessibility < 1)
                return defaultValue;

            return (AccessModifier)accessibility;
        }

        private static AccessModifier? getAccessModifier(SyntaxTokenList modifiers)
        {
            if (modifiers.Any(SyntaxKind.PublicKeyword))
                return AccessModifier.Public;

            if (modifiers.Any(SyntaxKind.ProtectedKeyword))
            {
                if (modifiers.Any(SyntaxKind.InternalKeyword))
                    return AccessModifier.ProtectedInternal;

                if (modifiers.Any(SyntaxKind.PrivateKeyword))
                    return AccessModifier.PrivateProtected;

                return AccessModifier.Protected;
            }

            if (modifiers.Any(SyntaxKind.InternalKeyword))
                return AccessModifier.Internal;

            if (modifiers.Any(SyntaxKind.PrivateKeyword))
                return AccessModifier.Private;

            return null;
        }
    }
}
