//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Xml;

public partial class XElementAssertions {
    /// <summary>Asserts that the element has a descendant named <paramref name="name"/> and exposes the first match for further assertions.</summary>
    /// <param name="name">The element name to search for.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the first matching element.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The element is <see langword="null"/> or has no such descendant.</exception>
    public WhichConstraint<XElement> HaveElement(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        var element = (_value?.Descendants(name).FirstOrDefault()) ?? throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.XML_HAVE_ELEMENT, Expression, name), because, becauseArgs));
        return new WhichConstraint<XElement>(element);
    }

    /// <summary>Asserts that the element has a descendant named <paramref name="name"/> and exposes the first match for further assertions.</summary>
    /// <param name="name">The qualified element name to search for.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the first matching element.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The element is <see langword="null"/> or has no such descendant.</exception>
    public WhichConstraint<XElement> HaveElement(XName name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        var element = (_value?.Descendants(name).FirstOrDefault()) ?? throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.XML_HAVE_ELEMENT, Expression, name), because, becauseArgs));
        return new WhichConstraint<XElement>(element);
    }

    /// <summary>Asserts that the element has descendant elements named <paramref name="name"/> as often as <paramref name="occurrence"/> requires.</summary>
    /// <param name="name">The element name to count.</param>
    /// <param name="occurrence">The expected occurrence constraint (e.g. <c>Exactly.Once()</c>).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping every matching element.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The matching elements do not satisfy <paramref name="occurrence"/>.</exception>
    public WhichConstraint<IReadOnlyList<XElement>> HaveElement(string name, OccurrenceConstraint occurrence, string? because = null, params object?[] becauseArgs) =>
        XmlElementAssertionsCore.RequireElementsByName(name, Matches, Expression, occurrence, because, becauseArgs);

    /// <summary>Asserts that the element has descendant elements named <paramref name="name"/> as often as <paramref name="occurrence"/> requires.</summary>
    /// <param name="name">The qualified element name to count.</param>
    /// <param name="occurrence">The expected occurrence constraint (e.g. <c>Exactly.Once()</c>).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping every matching element.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The matching elements do not satisfy <paramref name="occurrence"/>.</exception>
    public WhichConstraint<IReadOnlyList<XElement>> HaveElement(XName name, OccurrenceConstraint occurrence, string? because = null, params object?[] becauseArgs) =>
        XmlElementAssertionsCore.RequireElementsByXName(name, Matches, Expression, occurrence, because, becauseArgs);

    /// <summary>Lists the descendants with the given qualified name for the shared element assertion core.</summary>
    /// <param name="name">The qualified element name.</param>
    /// <returns>The matching descendants, or an empty list when the element is <see langword="null"/>.</returns>
    private List<XElement> Matches(XName name) => _value?.Descendants(name).ToList() ?? [];

    /// <summary>Asserts that the element has no descendant named <paramref name="name"/>.</summary>
    /// <param name="name">The element name that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The element has such a descendant.</exception>
    public AndConstraint<XElementAssertions> NotHaveElement(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        if(_value?.Descendants(name).FirstOrDefault() is not null) {
            Fail(Strings.Format(StringKeys.XML_NOT_HAVE_ELEMENT, Expression, name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the element has no descendant named <paramref name="name"/>.</summary>
    /// <param name="name">The qualified element name that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The element has such a descendant.</exception>
    public AndConstraint<XElementAssertions> NotHaveElement(XName name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        if(_value?.Descendants(name).FirstOrDefault() is not null) {
            Fail(Strings.Format(StringKeys.XML_NOT_HAVE_ELEMENT, Expression, name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the element has a descendant named <paramref name="name"/> with value <paramref name="expected"/> and exposes the first match.</summary>
    /// <param name="name">The element name to search for.</param>
    /// <param name="expected">The expected element value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the first matching element.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The element is <see langword="null"/>, has no such descendant, or no match has the expected value.</exception>
    public WhichConstraint<XElement> HaveElementWithValue(string name, string? expected, string? because = null, params object?[] becauseArgs) =>
        XmlElementAssertionsCore.HaveElementWithValueByName(name, Matches, Expression, expected, because, becauseArgs);

    /// <summary>Asserts that the element has a descendant named <paramref name="name"/> with value <paramref name="expected"/> and exposes the first match.</summary>
    /// <param name="name">The qualified element name to search for.</param>
    /// <param name="expected">The expected element value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the first matching element.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The element is <see langword="null"/>, has no such descendant, or no match has the expected value.</exception>
    public WhichConstraint<XElement> HaveElementWithValue(XName name, string? expected, string? because = null, params object?[] becauseArgs) =>
        XmlElementAssertionsCore.HaveElementWithValueByXName(name, Matches, Expression, expected, because, becauseArgs);

    /// <summary>Asserts that the element has no descendant named <paramref name="name"/> with value <paramref name="unexpected"/>.</summary>
    /// <param name="name">The element name that must be absent with that value.</param>
    /// <param name="unexpected">The element value that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">A descendant has the name and value.</exception>
    public AndConstraint<XElementAssertions> NotHaveElementWithValue(string name, string? unexpected, string? because = null, params object?[] becauseArgs) {
        XmlElementAssertionsCore.RequireNotHaveElementWithValueByName(name, Matches, Expression, unexpected, message => Fail(message, because, becauseArgs));
        return new(this);
    }

    /// <summary>Asserts that the element has no descendant named <paramref name="name"/> with value <paramref name="unexpected"/>.</summary>
    /// <param name="name">The qualified element name that must be absent with that value.</param>
    /// <param name="unexpected">The element value that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">A descendant has the name and value.</exception>
    public AndConstraint<XElementAssertions> NotHaveElementWithValue(XName name, string? unexpected, string? because = null, params object?[] becauseArgs) {
        XmlElementAssertionsCore.RequireNotHaveElementWithValueByXName(name, Matches, Expression, unexpected, message => Fail(message, because, becauseArgs));
        return new(this);
    }

    /// <summary>Asserts that the element has a descendant named <paramref name="name"/> in namespace <paramref name="namespaceName"/>.</summary>
    /// <param name="name">The local element name.</param>
    /// <param name="namespaceName">The element namespace.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the first matching element.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="namespaceName"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The element is <see langword="null"/> or has no such descendant.</exception>
    public WhichConstraint<XElement> HaveElementWithNamespace(string name, string namespaceName, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        if(namespaceName is null) {
            throw new ArgumentNullException(nameof(namespaceName));
        }

        var qualified = XNamespace.Get(namespaceName) + name;
        var element = (_value?.Descendants(qualified).FirstOrDefault()) ?? throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.XML_HAVE_ELEMENT_WITH_NAMESPACE, Expression, name, namespaceName), because, becauseArgs));
        return new WhichConstraint<XElement>(element);
    }
}
