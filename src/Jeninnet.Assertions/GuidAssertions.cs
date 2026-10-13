//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying <see cref="Guid"/> values.</summary>
/// <param name="value">The value to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public class GuidAssertions(Guid value, string? expression = null) : AssertionsBase(expression, "guid") {
    private readonly Guid _value = value;

    private static string Format(Guid value) => AssertionScope.FormatInvariant(value);

    /// <summary>Asserts that the value equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The values are not equal.</exception>
    public AndConstraint<GuidAssertions> Be(Guid expected, string? because = null, params object?[] becauseArgs) {
        if(_value != expected) {
            Fail(Strings.Format(StringKeys.GUID_BE, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value equals the parsed <paramref name="expected"/> text.</summary>
    /// <param name="expected">The expected GUID text (any format accepted by <see cref="Guid.Parse(string)"/>).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="FormatException"><paramref name="expected"/> is not a valid GUID text.</exception>
    /// <exception cref="AssertionFailedException">The values are not equal.</exception>
    public AndConstraint<GuidAssertions> Be(string? expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        return Be(Guid.Parse(expected), because, becauseArgs);
    }

    /// <summary>Asserts that the value does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The value the subject must not equal.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The values are equal.</exception>
    public AndConstraint<GuidAssertions> NotBe(Guid unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value == unexpected) {
            Fail(Strings.Format(StringKeys.GUID_NOT_BE, Expression, Format(unexpected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value does not equal the parsed <paramref name="unexpected"/> text.</summary>
    /// <param name="unexpected">The unexpected GUID text (any format accepted by <see cref="Guid.Parse(string)"/>).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="unexpected"/> is <see langword="null"/>.</exception>
    /// <exception cref="FormatException"><paramref name="unexpected"/> is not a valid GUID text.</exception>
    /// <exception cref="AssertionFailedException">The values are equal.</exception>
    public AndConstraint<GuidAssertions> NotBe(string? unexpected, string? because = null, params object?[] becauseArgs) {
        if(unexpected is null) {
            throw new ArgumentNullException(nameof(unexpected));
        }

        return NotBe(Guid.Parse(unexpected), because, becauseArgs);
    }

    /// <summary>Asserts that the value is <see cref="Guid.Empty"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is not empty.</exception>
    public AndConstraint<GuidAssertions> BeEmpty(string? because = null, params object?[] becauseArgs) {
        if(_value != Guid.Empty) {
            Fail(Strings.Format(StringKeys.GUID_BE_EMPTY, Expression, Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is not <see cref="Guid.Empty"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is empty.</exception>
    public AndConstraint<GuidAssertions> NotBeEmpty(string? because = null, params object?[] becauseArgs) {
        if(_value == Guid.Empty) {
            Fail(Strings.Format(StringKeys.GUID_NOT_BE_EMPTY, Expression, Format(_value)), because, becauseArgs);
        }

        return new(this);
    }
}
