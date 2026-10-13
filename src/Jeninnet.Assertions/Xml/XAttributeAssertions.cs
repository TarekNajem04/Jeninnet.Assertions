//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Xml;

/// <summary>Provides assertion methods for verifying <see cref="XAttribute"/> values.</summary>
/// <param name="value">The attribute to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public class XAttributeAssertions(XAttribute? value, string? expression = null) : AssertionsBase(expression, "XML") {
    private readonly XAttribute? _value = value;

    /// <summary>Asserts that the attribute is <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The attribute is not <see langword="null"/>.</exception>
    public AndConstraint<XAttributeAssertions> BeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is not null) {
            Fail(Strings.Format(StringKeys.XML_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the attribute is not <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The attribute is <see langword="null"/>.</exception>
    public AndConstraint<XAttributeAssertions> NotBeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.XML_NOT_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the attribute equals <paramref name="expected"/> by name and value.</summary>
    /// <param name="expected">The expected attribute.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The attribute name or value differs.</exception>
    public AndConstraint<XAttributeAssertions> Be(XAttribute? expected, string? because = null, params object?[] becauseArgs) {
        if(_value?.Name != expected?.Name || !string.Equals(_value?.Value, expected?.Value, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.XML_BE, Expression, FormatAttribute(expected), FormatAttribute(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the attribute does not equal <paramref name="unexpected"/> by name and value.</summary>
    /// <param name="unexpected">The unexpected attribute.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The attribute name and value match.</exception>
    public AndConstraint<XAttributeAssertions> NotBe(XAttribute? unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value?.Name == unexpected?.Name && string.Equals(_value?.Value, unexpected?.Value, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.XML_NOT_BE, Expression, FormatAttribute(unexpected), FormatAttribute(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the attribute value equals <paramref name="expected"/> using ordinal comparison.</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The attribute is <see langword="null"/> or its value differs.</exception>
    public AndConstraint<XAttributeAssertions> HaveValue(string? expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || !string.Equals(_value.Value, expected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.XML_HAVE_VALUE, Expression, expected, _value?.Value), because, becauseArgs);
        }

        return new(this);
    }

    private static string FormatAttribute(XAttribute? attribute) => attribute?.ToString() ?? "<null>";
}
