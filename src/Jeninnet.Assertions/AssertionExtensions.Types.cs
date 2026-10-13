//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public static partial class AssertionExtensions {
    /// <summary>Creates a <see cref="TypeAssertions"/> for the given type.</summary>
    /// <param name="value">The type to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static TypeAssertions Should(this Type? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates an <see cref="AssemblyAssertions"/> for the given assembly.</summary>
    /// <param name="value">The assembly to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static AssemblyAssertions Should(this Assembly? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="MethodInfoAssertions"/> for the given method.</summary>
    /// <param name="value">The method to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static MethodInfoAssertions Should(this MethodInfo? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="PropertyInfoAssertions"/> for the given property.</summary>
    /// <param name="value">The property to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static PropertyInfoAssertions Should(this PropertyInfo? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="ConstructorInfoAssertions"/> for the given constructor.</summary>
    /// <param name="value">The constructor to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static ConstructorInfoAssertions Should(this ConstructorInfo? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="StreamAssertions"/> for the given stream.</summary>
    /// <param name="value">The stream to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static StreamAssertions Should(this Stream? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="BufferedStreamAssertions"/> for the given buffered stream.</summary>
    /// <param name="value">The buffered stream to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static BufferedStreamAssertions Should(this BufferedStream? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates an <see cref="XDocumentAssertions"/> for the given XML document.</summary>
    /// <param name="value">The document to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static XDocumentAssertions Should(this XDocument? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates an <see cref="XElementAssertions"/> for the given XML element.</summary>
    /// <param name="value">The element to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static XElementAssertions Should(this XElement? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates an <see cref="XAttributeAssertions"/> for the given XML attribute.</summary>
    /// <param name="value">The attribute to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static XAttributeAssertions Should(this XAttribute? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates an <see cref="XmlNodeAssertions"/> for the given XML node.</summary>
    /// <param name="value">The node to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static XmlNodeAssertions Should(this XmlNode? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);
}
