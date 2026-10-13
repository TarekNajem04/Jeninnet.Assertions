//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Xml;

/// <summary>Provides assertion methods for verifying <see cref="XElement"/> values.</summary>
/// <param name="value">The element to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public partial class XElementAssertions(XElement? value, string? expression = null) : AssertionsBase(expression, "XML") {
    private readonly XElement? _value = value;

    /// <summary>Asserts that the element is <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The element is not <see langword="null"/>.</exception>
    public AndConstraint<XElementAssertions> BeNull(string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<XElementAssertions> NotBeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.XML_NOT_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the element equals <paramref name="expected"/> using <see cref="XNode.DeepEquals(XNode, XNode)"/>.</summary>
    /// <param name="expected">The expected element.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The elements are not deeply equal.</exception>
    public AndConstraint<XElementAssertions> Be(XElement? expected, string? because = null, params object?[] becauseArgs) {
        if(!XNode.DeepEquals(_value, expected)) {
            Fail(Strings.Format(StringKeys.XML_BE, Expression, FormatNode(expected), FormatNode(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the element does not equal <paramref name="unexpected"/> using <see cref="XNode.DeepEquals(XNode, XNode)"/>.</summary>
    /// <param name="unexpected">The unexpected element.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The elements are deeply equal.</exception>
    public AndConstraint<XElementAssertions> NotBe(XElement? unexpected, string? because = null, params object?[] becauseArgs) {
        if(XNode.DeepEquals(_value, unexpected)) {
            Fail(Strings.Format(StringKeys.XML_NOT_BE, Expression, FormatNode(unexpected), FormatNode(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the element is deeply equivalent to <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected element.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The elements are not deeply equivalent.</exception>
    public AndConstraint<XElementAssertions> BeEquivalentTo(XElement? expected, string? because = null, params object?[] becauseArgs) {
        if(!XNode.DeepEquals(_value, expected)) {
            Fail(Strings.Format(StringKeys.XML_BE_EQUIVALENT, Expression, _value, expected), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the element is not deeply equivalent to <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The unexpected element.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The elements are deeply equivalent.</exception>
    public AndConstraint<XElementAssertions> NotBeEquivalentTo(XElement? unexpected, string? because = null, params object?[] becauseArgs) {
        if(XNode.DeepEquals(_value, unexpected)) {
            Fail(Strings.Format(StringKeys.XML_NOT_BE_EQUIVALENT, Expression, _value, unexpected), because, becauseArgs);
        }

        return new(this);
    }

    private static string FormatNode(XNode? node) => node?.ToString() ?? "<null>";
}
