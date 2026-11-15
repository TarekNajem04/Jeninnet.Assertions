//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Xml;

/// <summary>Provides assertion methods for verifying classic <see cref="XmlElement"/> values with element-specific names and structural diagnostics.</summary>
/// <param name="value">The element to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public sealed class XmlElementAssertions(XmlElement? value, string? expression = null) : AssertionsBase(expression, "XML element") {
    private readonly XmlElement? _value = value;

    /// <summary>Asserts that the element is <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The element is not <see langword="null"/>.</exception>
    public AndConstraint<XmlElementAssertions> BeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is not null) {
            Fail(Strings.Format(StringKeys.XML_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the element is not <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The element is <see langword="null"/>.</exception>
    public AndConstraint<XmlElementAssertions> NotBeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.XML_NOT_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the element has the qualified name <paramref name="expected"/> (for example <c>ns:order</c>).</summary>
    /// <param name="expected">The expected qualified element name.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The element has another name or is <see langword="null"/>.</exception>
    public AndConstraint<XmlElementAssertions> HaveName(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(!string.Equals(_value?.Name, expected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.XML_HAVE_NAME, Expression, expected, _value?.Name ?? "<null>"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the element namespace URI equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected namespace URI (an empty string for no namespace).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The namespace differs or the element is <see langword="null"/>.</exception>
    public AndConstraint<XmlElementAssertions> HaveNamespace(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(!string.Equals(_value?.NamespaceURI, expected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.XML_HAVE_NAMESPACE, Expression, expected, _value?.NamespaceURI ?? "<null>"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the element has an attribute named <paramref name="name"/> and exposes it for further assertions.</summary>
    /// <param name="name">The qualified attribute name.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndWhichConstraint{TParent, T}"/> wrapping the attribute.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The element is <see langword="null"/> or lacks the attribute.</exception>
    public AndWhichConstraint<XmlElementAssertions, XmlAttribute> HaveAttribute(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        var attribute = _value?.GetAttributeNode(name);
        if(attribute is null) {
            Fail(Strings.Format(StringKeys.XML_HAVE_ATTRIBUTE, Expression, name), because, becauseArgs);
            return new(this, null!);
        }

        return new(this, attribute);
    }

    /// <summary>Asserts that the element has an attribute named <paramref name="name"/> with value <paramref name="expected"/>.</summary>
    /// <param name="name">The qualified attribute name.</param>
    /// <param name="expected">The expected attribute value (ordinal comparison).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The attribute is missing or its value differs.</exception>
    public AndConstraint<XmlElementAssertions> HaveAttributeWithValue(string name, string? expected, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        var attribute = _value?.GetAttributeNode(name);
        if(attribute is null) {
            Fail(Strings.Format(StringKeys.XML_HAVE_ATTRIBUTE, Expression, name), because, becauseArgs);
        } else if(!string.Equals(attribute.Value, expected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.XML_HAVE_ATTRIBUTE_WITH_VALUE, Expression, name, expected, attribute.Value), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the element does not have an attribute named <paramref name="name"/>.</summary>
    /// <param name="name">The qualified attribute name that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The attribute is present.</exception>
    public AndConstraint<XmlElementAssertions> NotHaveAttribute(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        if(_value?.GetAttributeNode(name) is not null) {
            Fail(Strings.Format(StringKeys.XML_NOT_HAVE_ATTRIBUTE, Expression, name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the element has a direct child element named <paramref name="name"/> and exposes it.</summary>
    /// <param name="name">The qualified child element name.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndWhichConstraint{TParent, T}"/> wrapping the first matching child.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The element is <see langword="null"/> or has no such child.</exception>
    public AndWhichConstraint<XmlElementAssertions, XmlElement> HaveElement(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        var element = FindChild(_value, candidate => string.Equals(candidate.Name, name, StringComparison.Ordinal));
        if(element is null) {
            Fail(Strings.Format(StringKeys.XML_HAVE_ELEMENT, Expression, name), because, becauseArgs);
            return new(this, null!);
        }

        return new(this, element);
    }

    /// <summary>Asserts that direct child elements named <paramref name="name"/> occur as often as <paramref name="occurrence"/> requires.</summary>
    /// <param name="name">The qualified child element name.</param>
    /// <param name="occurrence">The expected occurrence constraint (e.g. <c>Exactly.Once()</c>).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping every matching child.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The matching children do not satisfy <paramref name="occurrence"/>.</exception>
    public WhichConstraint<IReadOnlyList<XmlElement>> HaveElement(string name, OccurrenceConstraint occurrence, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        var matches = FindChildren(_value, candidate => string.Equals(candidate.Name, name, StringComparison.Ordinal));
        if(!occurrence.Matches(matches.Count)) {
            Fail(XmlOccurrence.FailureMessage(Expression, name, occurrence, matches.Count, because, becauseArgs));
        }

        return new(matches);
    }

    /// <summary>Asserts that the element has a direct child named <paramref name="name"/> with inner text <paramref name="expected"/>.</summary>
    /// <param name="name">The qualified child element name.</param>
    /// <param name="expected">The expected child inner text.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndWhichConstraint{TParent, T}"/> wrapping the matching child.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">No such child has the expected text.</exception>
    public AndWhichConstraint<XmlElementAssertions, XmlElement> HaveElementWithValue(string name, string? expected, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        var matches = FindChildren(_value, candidate => string.Equals(candidate.Name, name, StringComparison.Ordinal));
        var match = matches.Find(candidate => string.Equals(candidate.InnerText, expected, StringComparison.Ordinal));
        if(match is null) {
            var actual = matches.Count > 0 ? matches[0].InnerText : null;
            Fail(Strings.Format(StringKeys.XML_HAVE_ELEMENT_WITH_VALUE, Expression, name, expected, actual), because, becauseArgs);
            return new(this, null!);
        }

        return new(this, match);
    }

    /// <summary>Asserts that the element inner text equals <paramref name="expected"/> using ordinal comparison.</summary>
    /// <param name="expected">The expected inner text.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The element is <see langword="null"/> or its inner text differs.</exception>
    public AndConstraint<XmlElementAssertions> HaveInnerText(string? expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || !string.Equals(_value.InnerText, expected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.XML_HAVE_INNER_TEXT, Expression, expected, _value?.InnerText), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the element is structurally equivalent to <paramref name="expected"/> and reports the first differing paths.</summary>
    /// <param name="expected">The expected element.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The trees differ.</exception>
    public AndConstraint<XmlElementAssertions> BeEquivalentTo(XmlElement? expected, string? because = null, params object?[] becauseArgs) =>
        AssertEquivalent(expected, new XmlEquivalencyOptions(), because, becauseArgs);

    /// <summary>Asserts that the element is structurally equivalent to <paramref name="expected"/> under <paramref name="config"/>.</summary>
    /// <param name="expected">The expected element.</param>
    /// <param name="config">The equivalency configuration (element order, whitespace, comments).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="config"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The trees differ.</exception>
    public AndConstraint<XmlElementAssertions> BeEquivalentTo(XmlElement? expected, Func<XmlEquivalencyOptions, XmlEquivalencyOptions> config, string? because = null, params object?[] becauseArgs) {
        if(config is null) {
            throw new ArgumentNullException(nameof(config));
        }

        return AssertEquivalent(expected, config(new XmlEquivalencyOptions()), because, becauseArgs);
    }

    private AndConstraint<XmlElementAssertions> AssertEquivalent(XmlElement? expected, XmlEquivalencyOptions options, string? because, object?[] becauseArgs) {
        var differences = XmlEquivalency.Compare(ToXElement(_value), ToXElement(expected), options);
        if(differences.Count > 0) {
            Fail(Strings.Format(StringKeys.XML_BE_EQUIVALENT_DIFF, Expression, AssertionScope.FormatInvariant(differences.Count), XmlEquivalency.Format(differences)), because, becauseArgs);
        }

        return new(this);
    }

    private static XElement? ToXElement(XmlElement? element) =>
        element is null ? null : XElement.Parse(element.OuterXml, LoadOptions.PreserveWhitespace);

    private static XmlElement? FindChild(XmlElement? parent, Func<XmlElement, bool> predicate) {
        if(parent is null) {
            return null;
        }

        foreach(XmlNode child in parent.ChildNodes) {
            if(child is XmlElement element && predicate(element)) {
                return element;
            }
        }

        return null;
    }

    private static List<XmlElement> FindChildren(XmlElement? parent, Func<XmlElement, bool> predicate) {
        var matches = new List<XmlElement>();
        if(parent is null) {
            return matches;
        }

        foreach(XmlNode child in parent.ChildNodes) {
            if(child is XmlElement element && predicate(element)) {
                matches.Add(element);
            }
        }

        return matches;
    }
}
