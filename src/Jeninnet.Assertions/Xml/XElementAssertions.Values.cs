//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Xml;

public partial class XElementAssertions {
    /// <summary>Asserts that the element value equals <paramref name="expected"/> using ordinal comparison.</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The element is <see langword="null"/> or its value differs.</exception>
    public AndConstraint<XElementAssertions> HaveValue(string? expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || !string.Equals(_value.Value, expected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.XML_HAVE_VALUE, Expression, expected, _value?.Value), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the concatenated descendant text of the element equals <paramref name="expected"/> using ordinal comparison.</summary>
    /// <remarks>LINQ to XML exposes this concatenated text as <see cref="XElement.Value"/>; this method names that behavior explicitly.</remarks>
    /// <param name="expected">The expected inner text.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The element is <see langword="null"/> or its inner text differs.</exception>
    public AndConstraint<XElementAssertions> HaveInnerText(string? expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || !string.Equals(_value.Value, expected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.XML_HAVE_INNER_TEXT, Expression, expected, _value?.Value), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the concatenated descendant text of the element does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The inner text that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The element has that inner text.</exception>
    public AndConstraint<XElementAssertions> NotHaveInnerText(string? unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value is not null && string.Equals(_value.Value, unexpected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.XML_NOT_HAVE_INNER_TEXT, Expression, unexpected), because, becauseArgs);
        }

        return new(this);
    }
}
