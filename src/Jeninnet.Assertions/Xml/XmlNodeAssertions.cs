//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Xml;

/// <summary>Provides assertion methods for verifying <see cref="XmlNode"/> values (classic System.Xml API; also serves <see cref="XmlDocument"/> and <see cref="XmlElement"/>).</summary>
/// <param name="value">The node to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public class XmlNodeAssertions(XmlNode? value, string? expression = null) : AssertionsBase(expression, "XML") {
    private readonly XmlNode? _value = value;

    /// <summary>Asserts that the node is <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The node is not <see langword="null"/>.</exception>
    public AndConstraint<XmlNodeAssertions> BeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is not null) {
            Fail(Strings.Format(StringKeys.XML_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the node is not <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The node is <see langword="null"/>.</exception>
    public AndConstraint<XmlNodeAssertions> NotBeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.XML_NOT_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the node has a descendant element named <paramref name="name"/> and exposes the first match for further assertions.</summary>
    /// <param name="name">The element name to search for.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the first matching element.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The node is <see langword="null"/> or has no such descendant.</exception>
    public WhichConstraint<XmlNode> HaveElement(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        var element = FindDescendant(_value, name) ?? throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.XML_HAVE_ELEMENT, Expression, name), because, becauseArgs));
        return new WhichConstraint<XmlNode>(element);
    }

    /// <summary>Asserts that the node has no descendant element named <paramref name="name"/>.</summary>
    /// <param name="name">The element name that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The node has such a descendant.</exception>
    public AndConstraint<XmlNodeAssertions> NotHaveElement(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        if(FindDescendant(_value, name) is not null) {
            Fail(Strings.Format(StringKeys.XML_NOT_HAVE_ELEMENT, Expression, name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the node has a descendant named <paramref name="name"/> with inner text <paramref name="expected"/> and exposes the first match.</summary>
    /// <param name="name">The element name to search for.</param>
    /// <param name="expected">The expected inner text.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the first matching element.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">No matching descendant has the expected text.</exception>
    public WhichConstraint<XmlNode> HaveElementWithValue(string name, string? expected, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        var matches = FindDescendants(_value, candidate => string.Equals(candidate.Name, name, StringComparison.Ordinal));
        var match = matches.Find(candidate => string.Equals(candidate.InnerText, expected, StringComparison.Ordinal));
        if(match is null) {
            throw new AssertionFailedException(
                AssertionScope.AppendBecause(
                        Strings.Format(StringKeys.XML_HAVE_ELEMENT_WITH_VALUE, Expression, name, expected, matches.FirstOrDefault()?.InnerText),
                    because,
                    becauseArgs));
        }

        return new WhichConstraint<XmlNode>(match);
    }

    /// <summary>Asserts that the node has no descendant named <paramref name="name"/> with inner text <paramref name="unexpected"/>.</summary>
    /// <param name="name">The element name that must be absent with that text.</param>
    /// <param name="unexpected">The inner text that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">A descendant has the name and text.</exception>
    public AndConstraint<XmlNodeAssertions> NotHaveElementWithValue(string name, string? unexpected, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        if(FindDescendants(_value, named => string.Equals(named.Name, name, StringComparison.Ordinal))
            .Exists(candidate => string.Equals(candidate.InnerText, unexpected, StringComparison.Ordinal))) {
            Fail(Strings.Format(StringKeys.XML_NOT_HAVE_ELEMENT_WITH_VALUE, Expression, name, unexpected), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the node has a descendant named <paramref name="name"/> in namespace <paramref name="namespaceName"/>.</summary>
    /// <param name="name">The local element name.</param>
    /// <param name="namespaceName">The element namespace.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the first matching element.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="namespaceName"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The node has no such descendant.</exception>
    public WhichConstraint<XmlNode> HaveElementWithNamespace(string name, string namespaceName, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        if(namespaceName is null) {
            throw new ArgumentNullException(nameof(namespaceName));
        }

        var matches = FindDescendants(
            _value,
            named => string.Equals(named.LocalName, name, StringComparison.Ordinal)
                && string.Equals(named.NamespaceURI, namespaceName, StringComparison.Ordinal));
        var element = matches.FirstOrDefault();
        if(element is null) {
            throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.XML_HAVE_ELEMENT_WITH_NAMESPACE, Expression, name, namespaceName), because, becauseArgs));
        }

        return new WhichConstraint<XmlNode>(element);
    }

    /// <summary>Asserts that the node carries an attribute named <paramref name="name"/> and exposes it for further assertions.</summary>
    /// <param name="name">The attribute name.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the attribute.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The node is <see langword="null"/> or lacks the attribute.</exception>
    public WhichConstraint<XmlAttribute> HaveAttribute(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        var attribute = (_value?.Attributes?[name]) ?? throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.XML_HAVE_ATTRIBUTE, Expression, name), because, becauseArgs));
        return new WhichConstraint<XmlAttribute>(attribute);
    }

    /// <summary>Asserts that the node does not carry an attribute named <paramref name="name"/>.</summary>
    /// <param name="name">The attribute name that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The node carries the attribute.</exception>
    public AndConstraint<XmlNodeAssertions> NotHaveAttribute(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        if(_value?.Attributes?[name] is not null) {
            Fail(Strings.Format(StringKeys.XML_NOT_HAVE_ATTRIBUTE, Expression, name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the node carries an attribute named <paramref name="name"/> with value <paramref name="expected"/>.</summary>
    /// <param name="name">The attribute name.</param>
    /// <param name="expected">The expected attribute value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The node is <see langword="null"/> or the attribute value differs.</exception>
    public AndConstraint<XmlNodeAssertions> HaveAttributeWithValue(string name, string? expected, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        var actual = _value?.Attributes?[name]?.Value;
        if(!string.Equals(actual, expected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.XML_HAVE_ATTRIBUTE_WITH_VALUE, Expression, name, expected, actual), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the node does not carry an attribute named <paramref name="name"/> with value <paramref name="unexpected"/>.</summary>
    /// <param name="name">The attribute name.</param>
    /// <param name="unexpected">The attribute value that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The node carries the attribute with that value.</exception>
    public AndConstraint<XmlNodeAssertions> NotHaveAttributeWithValue(string name, string? unexpected, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        var actual = _value?.Attributes?[name]?.Value;
        if(string.Equals(actual, unexpected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.XML_NOT_HAVE_ATTRIBUTE_WITH_VALUE, Expression, name, unexpected), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the node carries an attribute named <paramref name="name"/> in namespace <paramref name="namespaceName"/>.</summary>
    /// <param name="name">The local attribute name.</param>
    /// <param name="namespaceName">The attribute namespace.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the attribute.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="namespaceName"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The node is <see langword="null"/> or lacks the attribute.</exception>
    public WhichConstraint<XmlAttribute> HaveAttributeWithNamespace(string name, string namespaceName, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        if(namespaceName is null) {
            throw new ArgumentNullException(nameof(namespaceName));
        }

        var attribute = FindAttribute(_value?.Attributes, name, namespaceName) ?? throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.XML_HAVE_ATTRIBUTE_WITH_NAMESPACE, Expression, name, namespaceName), because, becauseArgs));
        return new WhichConstraint<XmlAttribute>(attribute);
    }

    /// <summary>Asserts that the node inner text equals <paramref name="expected"/> using ordinal comparison.</summary>
    /// <param name="expected">The expected inner text.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The node is <see langword="null"/> or its inner text differs.</exception>
    public AndConstraint<XmlNodeAssertions> HaveValue(string? expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || !string.Equals(_value.InnerText, expected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.XML_HAVE_VALUE, Expression, expected, _value?.InnerText), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the node inner text equals <paramref name="expected"/> using ordinal comparison.</summary>
    /// <remarks>This names the behavior already checked by <see cref="HaveValue"/> explicitly.</remarks>
    /// <param name="expected">The expected inner text.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The node is <see langword="null"/> or its inner text differs.</exception>
    public AndConstraint<XmlNodeAssertions> HaveInnerText(string? expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || !string.Equals(_value.InnerText, expected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.XML_HAVE_INNER_TEXT, Expression, expected, _value?.InnerText), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the node inner text does not equal <paramref name="unexpected"/> using ordinal comparison.</summary>
    /// <param name="unexpected">The inner text that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The node has that inner text.</exception>
    public AndConstraint<XmlNodeAssertions> NotHaveInnerText(string? unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value is not null && string.Equals(_value.InnerText, unexpected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.XML_NOT_HAVE_INNER_TEXT, Expression, unexpected), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the node markup equals <paramref name="expected"/> markup using ordinal comparison.</summary>
    /// <param name="expected">The expected node.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The markup differs (comparison is whitespace-sensitive).</exception>
    public AndConstraint<XmlNodeAssertions> Be(XmlNode? expected, string? because = null, params object?[] becauseArgs) {
        if(!string.Equals(_value?.OuterXml, expected?.OuterXml, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.XML_BE, Expression, FormatNode(expected), FormatNode(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the node markup does not equal <paramref name="unexpected"/> markup using ordinal comparison.</summary>
    /// <param name="unexpected">The unexpected node.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The markup is equal.</exception>
    public AndConstraint<XmlNodeAssertions> NotBe(XmlNode? unexpected, string? because = null, params object?[] becauseArgs) {
        if(string.Equals(_value?.OuterXml, unexpected?.OuterXml, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.XML_NOT_BE, Expression, FormatNode(unexpected), FormatNode(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the node markup equals <paramref name="expected"/> markup using ordinal comparison.</summary>
    /// <param name="expected">The expected node.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The markup differs (comparison is whitespace-sensitive).</exception>
    public AndConstraint<XmlNodeAssertions> BeEquivalentTo(XmlNode? expected, string? because = null, params object?[] becauseArgs) {
        if(!string.Equals(_value?.OuterXml, expected?.OuterXml, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.XML_BE_EQUIVALENT, Expression, _value?.OuterXml, expected?.OuterXml), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the node markup does not equal <paramref name="unexpected"/> markup using ordinal comparison.</summary>
    /// <param name="unexpected">The unexpected node.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The markup is equal.</exception>
    public AndConstraint<XmlNodeAssertions> NotBeEquivalentTo(XmlNode? unexpected, string? because = null, params object?[] becauseArgs) {
        if(string.Equals(_value?.OuterXml, unexpected?.OuterXml, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.XML_NOT_BE_EQUIVALENT, Expression, _value?.OuterXml, unexpected?.OuterXml), because, becauseArgs);
        }

        return new(this);
    }

    private static string FormatNode(XmlNode? node) => node?.OuterXml ?? "<null>";

    /// <summary>Finds an attribute by local name and namespace using ordinal comparison.</summary>
    /// <param name="attributes">The attribute collection (may be <see langword="null"/>).</param>
    /// <param name="name">The attribute local name.</param>
    /// <param name="namespaceName">The attribute namespace URI.</param>
    /// <returns>The matching attribute, or <see langword="null"/>.</returns>
    private static XmlAttribute? FindAttribute(XmlAttributeCollection? attributes, string name, string namespaceName) {
        if(attributes is null) {
            return null;
        }

        foreach(XmlAttribute attribute in attributes) {
            if(string.Equals(attribute.LocalName, name, StringComparison.Ordinal)
                && string.Equals(attribute.NamespaceURI, namespaceName, StringComparison.Ordinal)) {
                return attribute;
            }
        }

        return null;
    }

    /// <summary>Collects every descendant element that matches the predicate.</summary>
    /// <param name="node">The node to walk.</param>
    /// <param name="isMatch">The predicate deciding which elements to collect.</param>
    /// <returns>The matching elements in document order.</returns>
    private static List<XmlNode> FindDescendants(XmlNode? node, Func<XmlElement, bool> isMatch) {
        var matches = new List<XmlNode>();
        CollectDescendants(node, isMatch, matches);
        return matches;
    }

    /// <summary>Recursively collects every descendant element that matches the predicate.</summary>
    /// <param name="node">The node to walk.</param>
    /// <param name="isMatch">The predicate deciding which elements to collect.</param>
    /// <param name="matches">The destination list, appended in document order.</param>
    private static void CollectDescendants(XmlNode? node, Func<XmlElement, bool> isMatch, List<XmlNode> matches) {
        if(node is null) {
            return;
        }

        foreach(XmlNode child in node.ChildNodes) {
            if(child is XmlElement element) {
                if(isMatch(element)) {
                    matches.Add(element);
                }

                CollectDescendants(element, isMatch, matches);
            } else {
                CollectDescendants(child, isMatch, matches);
            }
        }
    }

    /// <summary>Finds the first descendant element with the given name (ordinal, depth-first).</summary>
    /// <param name="node">The node to walk.</param>
    /// <param name="name">The element name to match.</param>
    /// <returns>The first matching element, or <see langword="null"/>.</returns>
    private static XmlNode? FindDescendant(XmlNode? node, string name) {
        if(node is null) {
            return null;
        }

        foreach(XmlNode child in node.ChildNodes) {
            if(child is XmlElement element && string.Equals(element.Name, name, StringComparison.Ordinal)) {
                return element;
            }

            var nested = FindDescendant(child, name);
            if(nested is not null) {
                return nested;
            }
        }

        return null;
    }
}
