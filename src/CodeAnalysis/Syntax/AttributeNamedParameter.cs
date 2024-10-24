namespace Basilisque.CodeAnalysis.Syntax;

/// <summary>
/// Represents a named parameter of an attribute
/// </summary>
public class AttributeNamedParameter
{
    /// <summary>
    /// The name of the parameter
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The value of the parameter
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Creates a new <see cref="AttributeNamedParameter"/>
    /// </summary>
    /// <param name="name">The name of the parameter</param>
    /// <param name="value">The value of the parameter</param>
    /// <exception cref="ArgumentNullException">Throws an <see cref="ArgumentNullException"/> when the name or the value parameter is null or an empty string</exception>
    public AttributeNamedParameter(string name, string value)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));

        if (string.IsNullOrEmpty(value))
            throw new ArgumentNullException(nameof(value));

        Name = name;
        Value = value;
    }
}
