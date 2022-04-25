namespace Basilisque.CodeAnalysis.Syntax
{
    /// <summary>
    /// Access modifiers are keywords used to specify the declared accessibility of a member or a type.
    /// </summary>
    public enum AccessModifier
    {
        /// <summary>
        /// public - access is not restricted
        /// </summary>
        Public,
        /// <summary>
        /// internal - access is limited to the current assembly
        /// </summary>
        Internal,
        /// <summary>
        /// protected - access is limited to the containing class or types derived from the containing class
        /// </summary>
        Protected,
        /// <summary>
        /// private - access is limited to the containing type
        /// </summary>
        Private,
        /// <summary>
        /// protected internal - access is limited to the current assembly or types derived from the containing class
        /// </summary>
        ProtectedInternal,
        /// <summary>
        /// private protected - access is limited to the containing class or types derived from the containing class within the current assembly
        /// </summary>
        PrivateProtected
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
    }
}
