//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Xml;

/// <summary>Provides assertion methods for verifying <see cref="XDocument"/> values.</summary>
/// <param name="value">The document to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public class XDocumentAssertions(XDocument? value, string? expression = null) : AssertionsBase(expression, "XML") {
    private readonly XDocument? _value = value;

    /// <summary>Asserts that the document is <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The document is not <see langword="null"/>.</exception>
    public AndConstraint<XDocumentAssertions> BeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is not null) {
            Fail(Strings.Format(StringKeys.XML_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the document is not <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The document is <see langword="null"/>.</exception>
    public AndConstraint<XDocumentAssertions> NotBeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.XML_NOT_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the document has a root element named <paramref name="name"/> and exposes it for further assertions.</summary>
    /// <param name="name">The expected root element name.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the root element.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The document is <see langword="null"/> or the root differs.</exception>
    public WhichConstraint<XElement> HaveRoot(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        if(_value?.Root is null || !string.Equals(_value.Root.Name.LocalName, name, StringComparison.Ordinal)) {
            throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.XML_HAVE_ROOT, Expression, name), because, becauseArgs));
        }

        return new WhichConstraint<XElement>(_value.Root);
    }

    /// <summary>Asserts that the document has a root element named <paramref name="name"/> and exposes it for further assertions.</summary>
    /// <param name="name">The qualified expected root element name.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the root element.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The document is <see langword="null"/> or the root differs.</exception>
    public WhichConstraint<XElement> HaveRoot(XName name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        var root = _value?.Root;
        if(root is null || root.Name != name) {
            throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.XML_HAVE_ROOT, Expression, name), because, becauseArgs));
        }

        return new WhichConstraint<XElement>(root);
    }

    /// <summary>Asserts that the document contains a descendant element named <paramref name="name"/> and exposes the first match for further assertions.</summary>
    /// <param name="name">The element name to search for (root included).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the first matching element.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The document is <see langword="null"/> or has no such element.</exception>
    public WhichConstraint<XElement> HaveElement(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        var element = FindFirst(XNamespace.None + name) ?? throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.XML_HAVE_ELEMENT, Expression, name), because, becauseArgs));

        return new WhichConstraint<XElement>(element);
    }

    /// <summary>Asserts that the document contains a descendant element named <paramref name="name"/> and exposes the first match for further assertions.</summary>
    /// <param name="name">The qualified element name to search for (root included).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the first matching element.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The document is <see langword="null"/> or has no such element.</exception>
    public WhichConstraint<XElement> HaveElement(XName name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        var element = FindFirst(name) ?? throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.XML_HAVE_ELEMENT, Expression, name), because, becauseArgs));

        return new WhichConstraint<XElement>(element);
    }

    /// <summary>Asserts that descendant elements named <paramref name="name"/> occur as often as <paramref name="occurrence"/> requires.</summary>
    /// <param name="name">The element name to count (root included).</param>
    /// <param name="occurrence">The expected occurrence constraint (e.g. <c>Exactly.Once()</c>).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping every matching element.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The matching elements do not satisfy <paramref name="occurrence"/>.</exception>
    public WhichConstraint<IReadOnlyList<XElement>> HaveElement(string name, OccurrenceConstraint occurrence, string? because = null, params object?[] becauseArgs) =>
        XmlElementAssertionsCore.RequireElementsByName(name, FindAll, Expression, occurrence, because, becauseArgs);

    /// <summary>Asserts that descendant elements named <paramref name="name"/> occur as often as <paramref name="occurrence"/> requires.</summary>
    /// <param name="name">The qualified element name to count (root included).</param>
    /// <param name="occurrence">The expected occurrence constraint (e.g. <c>Exactly.Once()</c>).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping every matching element.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The matching elements do not satisfy <paramref name="occurrence"/>.</exception>
    public WhichConstraint<IReadOnlyList<XElement>> HaveElement(XName name, OccurrenceConstraint occurrence, string? because = null, params object?[] becauseArgs) =>
        XmlElementAssertionsCore.RequireElementsByXName(name, FindAll, Expression, occurrence, because, becauseArgs);

    /// <summary>Asserts that the document contains no descendant element named <paramref name="name"/>.</summary>
    /// <param name="name">The element name that must be absent (root included).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The document contains such an element.</exception>
    public AndConstraint<XDocumentAssertions> NotHaveElement(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        return NotHaveElement(XNamespace.None + name, name, because, becauseArgs);
    }

    /// <summary>Asserts that the document contains no descendant element named <paramref name="name"/>.</summary>
    /// <param name="name">The qualified element name that must be absent (root included).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The document contains such an element.</exception>
    public AndConstraint<XDocumentAssertions> NotHaveElement(XName name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        return NotHaveElement(name, name.ToString(), because, becauseArgs);
    }

    /// <summary>Core check for <c>NotHaveElement</c>: fails when the root or any descendant matches.</summary>
    /// <param name="name">The qualified element name to search for.</param>
    /// <param name="displayName">The name as displayed in failure messages.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    private AndConstraint<XDocumentAssertions> NotHaveElement(XName name, string displayName, string? because, object?[]? becauseArgs) {
        if(FindFirst(name) is not null) {
            Fail(Strings.Format(StringKeys.XML_NOT_HAVE_ELEMENT, Expression, displayName), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that a descendant named <paramref name="name"/> has value <paramref name="expected"/> and exposes the first match.</summary>
    /// <param name="name">The element name to search for (root included).</param>
    /// <param name="expected">The expected element value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the first matching element.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">No matching descendant has the expected value.</exception>
    public WhichConstraint<XElement> HaveElementWithValue(string name, string? expected, string? because = null, params object?[] becauseArgs) =>
        XmlElementAssertionsCore.HaveElementWithValueByName(name, FindAll, Expression, expected, because, becauseArgs);

    /// <summary>Asserts that a descendant named <paramref name="name"/> has value <paramref name="expected"/> and exposes the first match.</summary>
    /// <param name="name">The qualified element name to search for (root included).</param>
    /// <param name="expected">The expected element value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the first matching element.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">No matching descendant has the expected value.</exception>
    public WhichConstraint<XElement> HaveElementWithValue(XName name, string? expected, string? because = null, params object?[] becauseArgs) =>
        XmlElementAssertionsCore.HaveElementWithValueByXName(name, FindAll, Expression, expected, because, becauseArgs);

    /// <summary>Asserts that no descendant named <paramref name="name"/> has value <paramref name="unexpected"/>.</summary>
    /// <param name="name">The element name that must be absent with that value (root included).</param>
    /// <param name="unexpected">The element value that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">A descendant has the name and value.</exception>
    public AndConstraint<XDocumentAssertions> NotHaveElementWithValue(string name, string? unexpected, string? because = null, params object?[] becauseArgs) {
        XmlElementAssertionsCore.RequireNotHaveElementWithValueByName(name, FindAll, Expression, unexpected, message => Fail(message, because, becauseArgs));
        return new(this);
    }

    /// <summary>Asserts that no descendant named <paramref name="name"/> has value <paramref name="unexpected"/>.</summary>
    /// <param name="name">The qualified element name that must be absent with that value (root included).</param>
    /// <param name="unexpected">The element value that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">A descendant has the name and value.</exception>
    public AndConstraint<XDocumentAssertions> NotHaveElementWithValue(XName name, string? unexpected, string? because = null, params object?[] becauseArgs) {
        XmlElementAssertionsCore.RequireNotHaveElementWithValueByXName(name, FindAll, Expression, unexpected, message => Fail(message, because, becauseArgs));
        return new(this);
    }

    /// <summary>Asserts that the document contains a descendant named <paramref name="name"/> in namespace <paramref name="namespaceName"/>.</summary>
    /// <param name="name">The local element name (root included).</param>
    /// <param name="namespaceName">The element namespace.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the first matching element.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="namespaceName"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The document has no such descendant.</exception>
    public WhichConstraint<XElement> HaveElementWithNamespace(string name, string namespaceName, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        if(namespaceName is null) {
            throw new ArgumentNullException(nameof(namespaceName));
        }

        var qualified = XNamespace.Get(namespaceName) + name;
        var element = FindFirst(qualified) ?? throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.XML_HAVE_ELEMENT_WITH_NAMESPACE, Expression, name, namespaceName), because, becauseArgs));
        return new WhichConstraint<XElement>(element);
    }

    /// <summary>Finds the root or first descendant with the given qualified name.</summary>
    /// <param name="name">The qualified element name.</param>
    /// <returns>The first matching element, or <see langword="null"/> when none exists.</returns>
    private XElement? FindFirst(XName name) {
        var root = _value?.Root;
        if(root is null) {
            return null;
        }

        return root.Name == name ? root : _value?.Descendants(name).FirstOrDefault();
    }

    /// <summary>Finds the root and every descendant with the given qualified name.</summary>
    /// <param name="name">The qualified element name.</param>
    /// <returns>The matching elements, root first.</returns>
    private List<XElement> FindAll(XName name) {
        var root = _value?.Root;
        if(root is null) {
            return [];
        }

        var matches = new List<XElement>();
        if(root.Name == name) {
            matches.Add(root);
        }

        matches.AddRange(_value?.Descendants(name) ?? []);
        return matches;
    }

    /// <summary>Asserts that the concatenated descendant text of the document root equals <paramref name="expected"/> using ordinal comparison.</summary>
    /// <param name="expected">The expected inner text.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The document has no root or its inner text differs.</exception>
    public AndConstraint<XDocumentAssertions> HaveInnerText(string? expected, string? because = null, params object?[] becauseArgs) {
        if(_value?.Root is null || !string.Equals(_value.Root.Value, expected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.XML_HAVE_INNER_TEXT, Expression, expected, _value?.Root?.Value), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the concatenated descendant text of the document root does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The inner text that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The root has that inner text.</exception>
    public AndConstraint<XDocumentAssertions> NotHaveInnerText(string? unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value?.Root is not null && string.Equals(_value.Root.Value, unexpected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.XML_NOT_HAVE_INNER_TEXT, Expression, unexpected), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the document equals <paramref name="expected"/> using <see cref="XNode.DeepEquals(XNode, XNode)"/>.</summary>
    /// <param name="expected">The expected document.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The documents are not deeply equal.</exception>
    public AndConstraint<XDocumentAssertions> Be(XDocument? expected, string? because = null, params object?[] becauseArgs) {
        if(!XNode.DeepEquals(_value, expected)) {
            Fail(Strings.Format(StringKeys.XML_BE, Expression, FormatNode(expected), FormatNode(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the document does not equal <paramref name="unexpected"/> using <see cref="XNode.DeepEquals(XNode, XNode)"/>.</summary>
    /// <param name="unexpected">The unexpected document.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The documents are deeply equal.</exception>
    public AndConstraint<XDocumentAssertions> NotBe(XDocument? unexpected, string? because = null, params object?[] becauseArgs) {
        if(XNode.DeepEquals(_value, unexpected)) {
            Fail(Strings.Format(StringKeys.XML_NOT_BE, Expression, FormatNode(unexpected), FormatNode(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the document is deeply equivalent to <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected document.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The documents are not deeply equivalent.</exception>
    public AndConstraint<XDocumentAssertions> BeEquivalentTo(XDocument? expected, string? because = null, params object?[] becauseArgs) {
        if(!XNode.DeepEquals(_value, expected)) {
            Fail(Strings.Format(StringKeys.XML_BE_EQUIVALENT, Expression, _value, expected), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the document is not deeply equivalent to <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The unexpected document.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The documents are deeply equivalent.</exception>
    public AndConstraint<XDocumentAssertions> NotBeEquivalentTo(XDocument? unexpected, string? because = null, params object?[] becauseArgs) {
        if(XNode.DeepEquals(_value, unexpected)) {
            Fail(Strings.Format(StringKeys.XML_NOT_BE_EQUIVALENT, Expression, _value, unexpected), because, becauseArgs);
        }

        return new(this);
    }

    private static string FormatNode(XNode? node) => node?.ToString() ?? "<null>";
}
