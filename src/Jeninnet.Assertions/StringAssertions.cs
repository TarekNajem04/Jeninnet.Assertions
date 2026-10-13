//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying <see cref="string"/> values.</summary>
/// <remarks>
/// Null-subject rule: positive assertions (<c>Be</c>, <c>Contain</c>, <c>StartWith</c>, …) fail on a
/// <see langword="null"/> subject, while negative assertions (<c>NotBe</c>, <c>NotContain</c>, …) pass on
/// <see langword="null"/> — except <c>NotBeNull</c> (fails) and <c>NotBeNullOrWhiteSpace</c> (fails).
/// Null values render as <c>&lt;null&gt;</c> in failure messages.
/// </remarks>
/// <param name="value">The string value to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public partial class StringAssertions(string? value, string? expression = null) : AssertionsBase(expression, "string") {
    private const string NULL_DETAIL = "but it was null";
    private readonly string? _value = value;

    /// <summary>Asserts that the string exactly equals <paramref name="expected"/> using ordinal comparison.</summary>
    /// <param name="expected">The expected string value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The string does not match <paramref name="expected"/>.</exception>
    /// <remarks>Long or multiline values append an invariant unified diff to the failure message.</remarks>
    public AndConstraint<StringAssertions> Be(string? expected, string? because = null, params object?[] becauseArgs) {
        if(!string.Equals(_value, expected, StringComparison.Ordinal)) {
            Fail(WithDiff(StringKeys.STRING_BE, expected, _value, because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that the string does not equal <paramref name="unexpected"/> using ordinal comparison.</summary>
    /// <param name="unexpected">The string value the subject must not equal.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The string equals <paramref name="unexpected"/>.</exception>
    public AndConstraint<StringAssertions> NotBe(string? unexpected, string? because = null, params object?[] becauseArgs) {
        if(string.Equals(_value, unexpected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.STRING_NOT_BE, Expression, Format(unexpected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string is equivalent to <paramref name="expected"/> (case-insensitive by default).</summary>
    /// <param name="expected">The expected string value.</param>
    /// <param name="config">Optional configuration for case, whitespace, and newline handling.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The strings are not equivalent.</exception>
    /// <remarks>Long or multiline values append an invariant unified diff to the failure message.</remarks>
    public AndConstraint<StringAssertions> BeEquivalentTo(
        string? expected,
        Func<StringEquivalencyOptions, StringEquivalencyOptions>? config = null,
        string? because = null,
        params object?[] becauseArgs) {
        var options = config?.Invoke(new StringEquivalencyOptions()) ?? new StringEquivalencyOptions();
        if(!options.AreEquivalent(_value, expected)) {
            Fail(WithDiff(StringKeys.STRING_BE_EQUIVALENT, expected, _value, because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that the string is not equivalent to <paramref name="expected"/> (case-insensitive by default).</summary>
    /// <param name="expected">The string value the subject must not be equivalent to.</param>
    /// <param name="config">Optional configuration for case, whitespace, and newline handling.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The strings are equivalent.</exception>
    public AndConstraint<StringAssertions> NotBeEquivalentTo(
        string? expected,
        Func<StringEquivalencyOptions, StringEquivalencyOptions>? config = null,
        string? because = null,
        params object?[] becauseArgs) {
        var options = config?.Invoke(new StringEquivalencyOptions()) ?? new StringEquivalencyOptions();
        if(options.AreEquivalent(_value, expected)) {
            Fail(Strings.Format(StringKeys.STRING_NOT_BE_EQUIVALENT, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }
}
