//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class StringAssertions {
    /// <summary>Asserts that the string starts with <paramref name="expected"/> using ordinal comparison.</summary>
    /// <param name="expected">The prefix the string is expected to start with.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string is <see langword="null"/> or does not start with <paramref name="expected"/>.</exception>
    public AndConstraint<StringAssertions> StartWith(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.STRING_START_WITH_NULL, Expression, Format(expected)), because, becauseArgs);
            return new(this);
        }

        if(!_value.StartsWith(expected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.STRING_START_WITH, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string does not start with <paramref name="expected"/>. A <see langword="null"/> subject passes.</summary>
    /// <param name="expected">The prefix the string must not start with.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string starts with <paramref name="expected"/>.</exception>
    public AndConstraint<StringAssertions> NotStartWith(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(_value?.StartsWith(expected, StringComparison.Ordinal) == true) {
            Fail(Strings.Format(StringKeys.STRING_NOT_START_WITH, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string starts with <paramref name="expected"/> ignoring casing.</summary>
    /// <param name="expected">The prefix the string is expected to start with.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string is <see langword="null"/> or does not start with <paramref name="expected"/>.</exception>
    public AndConstraint<StringAssertions> StartWithEquivalentOf(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.STRING_START_WITH_EQUIVALENT_NULL, Expression, Format(expected)), because, becauseArgs);
            return new(this);
        }

        if(!_value.StartsWith(expected, StringComparison.OrdinalIgnoreCase)) {
            Fail(Strings.Format(StringKeys.STRING_START_WITH_EQUIVALENT, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string starts with <paramref name="expected"/> honoring <paramref name="config"/>.</summary>
    /// <param name="expected">The prefix the string is expected to start with.</param>
    /// <param name="config">The equivalency options (a <see langword="null"/> value selects the defaults).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string is <see langword="null"/> or does not start with <paramref name="expected"/>.</exception>
    public AndConstraint<StringAssertions> StartWithEquivalentOf(string expected, Func<StringEquivalencyOptions, StringEquivalencyOptions>? config, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var options = ResolveOptions(config);
        if(_value is null) {
            Fail(Strings.Format(StringKeys.STRING_START_WITH_EQUIVALENT_NULL, Expression, Format(expected)), because, becauseArgs);
            return new(this);
        }

        if(!StartsWithEquivalent(_value, expected, options)) {
            Fail(Strings.Format(StringKeys.STRING_START_WITH_EQUIVALENT, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string does not start with <paramref name="expected"/> ignoring casing. A <see langword="null"/> subject passes.</summary>
    /// <param name="expected">The prefix the string must not start with.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string starts with <paramref name="expected"/>.</exception>
    public AndConstraint<StringAssertions> NotStartWithEquivalentOf(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(_value?.StartsWith(expected, StringComparison.OrdinalIgnoreCase) == true) {
            Fail(Strings.Format(StringKeys.STRING_NOT_START_WITH_EQUIVALENT, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string does not start with <paramref name="expected"/> honoring <paramref name="config"/>. A <see langword="null"/> subject passes.</summary>
    /// <param name="expected">The prefix the string must not start with.</param>
    /// <param name="config">The equivalency options (a <see langword="null"/> value selects the defaults).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string starts with <paramref name="expected"/>.</exception>
    public AndConstraint<StringAssertions> NotStartWithEquivalentOf(string expected, Func<StringEquivalencyOptions, StringEquivalencyOptions>? config, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var options = ResolveOptions(config);
        if(_value is not null && StartsWithEquivalent(_value, expected, options)) {
            Fail(Strings.Format(StringKeys.STRING_NOT_START_WITH_EQUIVALENT, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string ends with the specified <paramref name="expected"/> substring using ordinal comparison.</summary>
    /// <param name="expected">The suffix that the string is expected to end with.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The underlying string is <see langword="null"/> or does not end with <paramref name="expected"/>.</exception>
    public AndConstraint<StringAssertions> EndWith(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.STRING_END_WITH_NULL, Expression, Format(expected)), because, becauseArgs);
            return new(this);
        }

        if(!_value.EndsWith(expected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.STRING_END_WITH, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string ends with the specified <paramref name="expected"/> substring. Alias of <see cref="EndWith"/>.</summary>
    /// <param name="expected">The suffix that the string is expected to end with.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The underlying string is <see langword="null"/> or does not end with <paramref name="expected"/>.</exception>
    public AndConstraint<StringAssertions> EndsWith(string expected, string? because = null, params object?[] becauseArgs) =>
        EndWith(expected, because, becauseArgs);

    /// <summary>Asserts that the string does not end with <paramref name="expected"/>. A <see langword="null"/> subject passes.</summary>
    /// <param name="expected">The suffix the string must not end with.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string ends with <paramref name="expected"/>.</exception>
    public AndConstraint<StringAssertions> NotEndWith(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(_value?.EndsWith(expected, StringComparison.Ordinal) == true) {
            Fail(Strings.Format(StringKeys.STRING_NOT_END_WITH, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string ends with <paramref name="expected"/> ignoring casing.</summary>
    /// <param name="expected">The suffix the string is expected to end with.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string is <see langword="null"/> or does not end with <paramref name="expected"/>.</exception>
    public AndConstraint<StringAssertions> EndWithEquivalentOf(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.STRING_END_WITH_EQUIVALENT_NULL, Expression, Format(expected)), because, becauseArgs);
            return new(this);
        }

        if(!_value.EndsWith(expected, StringComparison.OrdinalIgnoreCase)) {
            Fail(Strings.Format(StringKeys.STRING_END_WITH_EQUIVALENT, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string ends with <paramref name="expected"/> honoring <paramref name="config"/>.</summary>
    /// <param name="expected">The suffix the string is expected to end with.</param>
    /// <param name="config">The equivalency options (a <see langword="null"/> value selects the defaults).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string is <see langword="null"/> or does not end with <paramref name="expected"/>.</exception>
    public AndConstraint<StringAssertions> EndWithEquivalentOf(string expected, Func<StringEquivalencyOptions, StringEquivalencyOptions>? config, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var options = ResolveOptions(config);
        if(_value is null) {
            Fail(Strings.Format(StringKeys.STRING_END_WITH_EQUIVALENT_NULL, Expression, Format(expected)), because, becauseArgs);
            return new(this);
        }

        if(!EndsWithEquivalent(_value, expected, options)) {
            Fail(Strings.Format(StringKeys.STRING_END_WITH_EQUIVALENT, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string does not end with <paramref name="expected"/> ignoring casing. A <see langword="null"/> subject passes.</summary>
    /// <param name="expected">The suffix the string must not end with.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string ends with <paramref name="expected"/>.</exception>
    public AndConstraint<StringAssertions> NotEndWithEquivalentOf(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(_value?.EndsWith(expected, StringComparison.OrdinalIgnoreCase) == true) {
            Fail(Strings.Format(StringKeys.STRING_NOT_END_WITH_EQUIVALENT, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string does not end with <paramref name="expected"/> honoring <paramref name="config"/>. A <see langword="null"/> subject passes.</summary>
    /// <param name="expected">The suffix the string must not end with.</param>
    /// <param name="config">The equivalency options (a <see langword="null"/> value selects the defaults).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string ends with <paramref name="expected"/>.</exception>
    public AndConstraint<StringAssertions> NotEndWithEquivalentOf(string expected, Func<StringEquivalencyOptions, StringEquivalencyOptions>? config, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var options = ResolveOptions(config);
        if(_value is not null && EndsWithEquivalent(_value, expected, options)) {
            Fail(Strings.Format(StringKeys.STRING_NOT_END_WITH_EQUIVALENT, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }
}
