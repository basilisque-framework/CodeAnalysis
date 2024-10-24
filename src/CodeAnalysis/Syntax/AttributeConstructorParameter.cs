namespace Basilisque.CodeAnalysis.Syntax;

/// <summary>
/// Represents a constructor parameter of an attribute
/// </summary>
public class AttributeConstructorParameter
{
    /// <summary>
    /// The name of the parameter
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The value of the parameter
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Creates a new <see cref="AttributeConstructorParameter"/>
    /// </summary>
    /// <param name="value">The value of the parameter</param>
    /// <exception cref="ArgumentNullException">Throws an <see cref="ArgumentNullException"/> when the value parameter is null or an empty string</exception>
    public AttributeConstructorParameter(string value)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentNullException(nameof(value));

        Value = value;
    }

    /// <summary>
    /// Creates a new <see cref="AttributeConstructorParameter"/>
    /// </summary>
    /// <param name="name">The name of the parameter</param>
    /// <param name="value">The value of the parameter</param>
    /// <exception cref="ArgumentNullException">Throws an <see cref="ArgumentNullException"/> when the name or the value parameter is null or an empty string</exception>
    public AttributeConstructorParameter(string? name, string value)
        : this(value)
    {
        Name = name;
    }
}
