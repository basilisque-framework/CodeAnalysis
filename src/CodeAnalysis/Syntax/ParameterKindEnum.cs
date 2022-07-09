namespace Basilisque.CodeAnalysis.Syntax
{
    /// <summary>
    /// Represents the kind of a parameter
    /// </summary>
    public enum ParameterKind
    {
        /// <summary>
        /// An ordinary parameter
        /// </summary>
        Ordinary,
        /// <summary>
        /// An out parameter
        /// </summary>
        Out,
        /// <summary>
        /// A parameter that is passed by reference
        /// </summary>
        Ref,
        /// <summary>
        /// A parameter that takes a variable number of arguments
        /// </summary>
        Params
    }
}
