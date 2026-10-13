//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Xml;

public partial class XElementAssertions {
    /// <summary>Asserts that the element carries an attribute named <paramref name="name"/> and exposes it for further assertions.</summary>
    /// <param name="name">The attribute name.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the attribute.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The element is <see langword="null"/> or lacks the attribute.</exception>
    public WhichConstraint<XAttribute> HaveAttribute(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        var attribute = (_value?.Attribute(name)) ?? throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.XML_HAVE_ATTRIBUTE, Expression, name), because, becauseArgs));
        return new WhichConstraint<XAttribute>(attribute);
    }

    /// <summary>Asserts that the element carries an attribute named <paramref name="name"/> and exposes it for further assertions.</summary>
    /// <param name="name">The qualified attribute name.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the attribute.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The element is <see langword="null"/> or lacks the attribute.</exception>
    public WhichConstraint<XAttribute> HaveAttribute(XName name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        var attribute = (_value?.Attribute(name)) ?? throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.XML_HAVE_ATTRIBUTE, Expression, name), because, becauseArgs));
        return new WhichConstraint<XAttribute>(attribute);
    }

    /// <summary>Asserts that the element does not carry an attribute named <paramref name="name"/>.</summary>
    /// <param name="name">The attribute name that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The element carries the attribute.</exception>
    public AndConstraint<XElementAssertions> NotHaveAttribute(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        if(_value?.Attribute(name) is not null) {
            Fail(Strings.Format(StringKeys.XML_NOT_HAVE_ATTRIBUTE, Expression, name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the element does not carry an attribute named <paramref name="name"/>.</summary>
    /// <param name="name">The qualified attribute name that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The element carries the attribute.</exception>
    public AndConstraint<XElementAssertions> NotHaveAttribute(XName name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        if(_value?.Attribute(name) is not null) {
            Fail(Strings.Format(StringKeys.XML_NOT_HAVE_ATTRIBUTE, Expression, name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the element carries an attribute named <paramref name="name"/> with value <paramref name="expected"/>.</summary>
    /// <param name="name">The attribute name.</param>
    /// <param name="expected">The expected attribute value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The element is <see langword="null"/> or the attribute value differs.</exception>
    public AndConstraint<XElementAssertions> HaveAttributeWithValue(string name, string? expected, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        return HaveAttributeWithValue(XNamespace.None + name, name, expected, because, becauseArgs);
    }

    /// <summary>Asserts that the element carries an attribute named <paramref name="name"/> with value <paramref name="expected"/>.</summary>
    /// <param name="name">The qualified attribute name.</param>
    /// <param name="expected">The expected attribute value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The element is <see langword="null"/> or the attribute value differs.</exception>
    public AndConstraint<XElementAssertions> HaveAttributeWithValue(XName name, string? expected, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        return HaveAttributeWithValue(name, name.ToString(), expected, because, becauseArgs);
    }

    /// <summary>Core check for <c>HaveAttributeWithValue</c>: compares the attribute value ordinally.</summary>
    /// <param name="name">The qualified attribute name.</param>
    /// <param name="displayName">The name as displayed in failure messages.</param>
    /// <param name="expected">The expected attribute value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    private AndConstraint<XElementAssertions> HaveAttributeWithValue(XName name, string displayName, string? expected, string? because, object?[]? becauseArgs) {
        var actual = _value?.Attribute(name)?.Value;
        if(!string.Equals(actual, expected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.XML_HAVE_ATTRIBUTE_WITH_VALUE, Expression, displayName, expected, actual), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the element does not carry an attribute named <paramref name="name"/> with value <paramref name="unexpected"/>.</summary>
    /// <param name="name">The attribute name.</param>
    /// <param name="unexpected">The attribute value that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The element carries the attribute with that value.</exception>
    public AndConstraint<XElementAssertions> NotHaveAttributeWithValue(string name, string? unexpected, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        return NotHaveAttributeWithValue(XNamespace.None + name, name, unexpected, because, becauseArgs);
    }

    /// <summary>Asserts that the element does not carry an attribute named <paramref name="name"/> with value <paramref name="unexpected"/>.</summary>
    /// <param name="name">The qualified attribute name.</param>
    /// <param name="unexpected">The attribute value that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The element carries the attribute with that value.</exception>
    public AndConstraint<XElementAssertions> NotHaveAttributeWithValue(XName name, string? unexpected, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        return NotHaveAttributeWithValue(name, name.ToString(), unexpected, because, becauseArgs);
    }

    /// <summary>Core check for <c>NotHaveAttributeWithValue</c>: fails when the attribute carries the value.</summary>
    /// <param name="name">The qualified attribute name.</param>
    /// <param name="displayName">The name as displayed in failure messages.</param>
    /// <param name="unexpected">The attribute value that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    private AndConstraint<XElementAssertions> NotHaveAttributeWithValue(XName name, string displayName, string? unexpected, string? because, object?[]? becauseArgs) {
        var actual = _value?.Attribute(name)?.Value;
        if(string.Equals(actual, unexpected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.XML_NOT_HAVE_ATTRIBUTE_WITH_VALUE, Expression, displayName, unexpected), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the element carries an attribute named <paramref name="name"/> in namespace <paramref name="namespaceName"/>.</summary>
    /// <param name="name">The local attribute name.</param>
    /// <param name="namespaceName">The attribute namespace.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the attribute.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="namespaceName"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The element is <see langword="null"/> or lacks the attribute.</exception>
    public WhichConstraint<XAttribute> HaveAttributeWithNamespace(string name, string namespaceName, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        if(namespaceName is null) {
            throw new ArgumentNullException(nameof(namespaceName));
        }

        var qualified = XNamespace.Get(namespaceName) + name;
        var attribute = (_value?.Attribute(qualified)) ?? throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.XML_HAVE_ATTRIBUTE_WITH_NAMESPACE, Expression, name, namespaceName), because, becauseArgs));
        return new WhichConstraint<XAttribute>(attribute);
    }
}
