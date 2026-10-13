//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class StringAssertions {
    /// <summary>Asserts that the string contains the specified <paramref name="expected"/> substring using ordinal comparison.</summary>
    /// <param name="expected">The substring to search for.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The underlying string is <see langword="null"/> or does not contain <paramref name="expected"/>.</exception>
    public AndConstraint<StringAssertions> Contain(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.STRING_CONTAIN_NULL, Expression, Format(expected)), because, becauseArgs);
            return new(this);
        }

        if(_value.IndexOf(expected, StringComparison.Ordinal) < 0) {
            Fail(Strings.Format(StringKeys.STRING_CONTAIN, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string contains <paramref name="expected"/> exactly as many times as <paramref name="occurrence"/> requires.</summary>
    /// <param name="expected">The substring to count (non-overlapping occurrences).</param>
    /// <param name="occurrence">The expected occurrence constraint (e.g. <c>Exactly.Twice()</c>).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The occurrence count does not satisfy <paramref name="occurrence"/>.</exception>
    public AndConstraint<StringAssertions> Contain(string expected, OccurrenceConstraint occurrence, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.STRING_CONTAIN_TIMES_NULL, Expression, Format(expected), occurrence.Expectation), because, becauseArgs);
            return new(this);
        }

        var count = CountOccurrences(_value, expected, StringComparison.Ordinal);
        if(!occurrence.Matches(count)) {
            Fail(Strings.Format(StringKeys.STRING_CONTAIN_TIMES, Expression, Format(expected), occurrence.Expectation, CountWord(count, "occurrence", "occurrences")), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string does not contain <paramref name="expected"/> using ordinal comparison. A <see langword="null"/> subject passes.</summary>
    /// <param name="expected">The substring that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string contains <paramref name="expected"/>.</exception>
    public AndConstraint<StringAssertions> NotContain(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if((_value?.IndexOf(expected, StringComparison.Ordinal) ?? -1) >= 0) {
            Fail(Strings.Format(StringKeys.STRING_NOT_CONTAIN, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string contains every value in <paramref name="expected"/> using ordinal comparison.</summary>
    /// <param name="expected">The substrings that must all be present.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string is <see langword="null"/> or misses at least one value.</exception>
    public AndConstraint<StringAssertions> ContainAll(string[] expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.STRING_CONTAIN_ALL_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        var missing = expected.FirstOrDefault(candidate => _value.IndexOf(candidate, StringComparison.Ordinal) < 0);
        if(missing is not null) {
            Fail(Strings.Format(StringKeys.STRING_CONTAIN_MISSING, Expression, Format(missing)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string contains every value in <paramref name="expected"/> using ordinal comparison.</summary>
    /// <param name="expected">The substrings that must all be present.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The string is <see langword="null"/> or misses at least one value.</exception>
    public AndConstraint<StringAssertions> ContainAll(params string[] expected) => ContainAll(expected, null, []);

    /// <summary>Asserts that the string misses at least one value in <paramref name="expected"/>. A <see langword="null"/> subject passes.</summary>
    /// <param name="expected">The substrings that must not all be present.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string contains every value in <paramref name="expected"/>.</exception>
    public AndConstraint<StringAssertions> NotContainAll(string[] expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(_value is not null && expected.All(candidate => _value.IndexOf(candidate, StringComparison.Ordinal) >= 0)) {
            Fail(Strings.Format(StringKeys.STRING_NOT_CONTAIN_ALL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string misses at least one value in <paramref name="expected"/>. A <see langword="null"/> subject passes.</summary>
    /// <param name="expected">The substrings that must not all be present.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The string contains every value in <paramref name="expected"/>.</exception>
    public AndConstraint<StringAssertions> NotContainAll(params string[] expected) => NotContainAll(expected, null, []);

    /// <summary>Asserts that the string contains at least one value in <paramref name="expected"/> using ordinal comparison.</summary>
    /// <param name="expected">The candidate substrings.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string is <see langword="null"/> or contains none of the values.</exception>
    public AndConstraint<StringAssertions> ContainAny(string[] expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(_value is null || !expected.Any(candidate => _value.IndexOf(candidate, StringComparison.Ordinal) >= 0)) {
            var detail = _value is null ? NULL_DETAIL : "but none was found";
            Fail(Strings.Format(StringKeys.STRING_CONTAIN_ANY, Expression, Join(expected), detail), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string contains at least one value in <paramref name="expected"/> using ordinal comparison.</summary>
    /// <param name="expected">The candidate substrings.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The string is <see langword="null"/> or contains none of the values.</exception>
    public AndConstraint<StringAssertions> ContainAny(params string[] expected) => ContainAny(expected, null, []);

    /// <summary>Asserts that the string contains none of the values in <paramref name="expected"/>. A <see langword="null"/> subject passes.</summary>
    /// <param name="expected">The substrings that must all be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string contains at least one of the values.</exception>
    public AndConstraint<StringAssertions> NotContainAny(string[] expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var found = _value is not null ? expected.FirstOrDefault(candidate => _value.IndexOf(candidate, StringComparison.Ordinal) >= 0) : null;
        if(found is not null) {
            Fail(Strings.Format(StringKeys.STRING_NOT_CONTAIN_ANY, Expression, Join(expected), Format(found)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string contains none of the values in <paramref name="expected"/>. A <see langword="null"/> subject passes.</summary>
    /// <param name="expected">The substrings that must all be absent.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The string contains at least one of the values.</exception>
    public AndConstraint<StringAssertions> NotContainAny(params string[] expected) => NotContainAny(expected, null, []);

    /// <summary>Asserts that the string contains <paramref name="expected"/> ignoring casing.</summary>
    /// <param name="expected">The substring to search for.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string is <see langword="null"/> or does not contain <paramref name="expected"/>.</exception>
    public AndConstraint<StringAssertions> ContainEquivalentOf(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.STRING_CONTAIN_EQUIVALENT_NULL, Expression, Format(expected)), because, becauseArgs);
            return new(this);
        }

        if(_value.IndexOf(expected, StringComparison.OrdinalIgnoreCase) < 0) {
            Fail(Strings.Format(StringKeys.STRING_CONTAIN_EQUIVALENT, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string contains <paramref name="expected"/> honoring <paramref name="config"/>.</summary>
    /// <param name="expected">The substring to search for.</param>
    /// <param name="config">The equivalency options (a <see langword="null"/> value selects the defaults).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string is <see langword="null"/> or does not contain <paramref name="expected"/>.</exception>
    public AndConstraint<StringAssertions> ContainEquivalentOf(string expected, Func<StringEquivalencyOptions, StringEquivalencyOptions>? config, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var options = ResolveOptions(config);
        if(_value is null) {
            Fail(Strings.Format(StringKeys.STRING_CONTAIN_EQUIVALENT_NULL, Expression, Format(expected)), because, becauseArgs);
            return new(this);
        }

        if(!ContainsEquivalent(_value, expected, options)) {
            Fail(Strings.Format(StringKeys.STRING_CONTAIN_EQUIVALENT, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string contains <paramref name="expected"/> (ignoring casing) as many times as <paramref name="occurrence"/> requires.</summary>
    /// <param name="expected">The substring to count (non-overlapping occurrences).</param>
    /// <param name="occurrence">The expected occurrence constraint (e.g. <c>AtLeast.Twice()</c>).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The occurrence count does not satisfy <paramref name="occurrence"/>.</exception>
    public AndConstraint<StringAssertions> ContainEquivalentOf(string expected, OccurrenceConstraint occurrence, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.STRING_CONTAIN_EQUIVALENT_TIMES_NULL, Expression, Format(expected), occurrence.Expectation), because, becauseArgs);
            return new(this);
        }

        var count = CountOccurrences(_value, expected, StringComparison.OrdinalIgnoreCase);
        if(!occurrence.Matches(count)) {
            Fail(Strings.Format(StringKeys.STRING_CONTAIN_EQUIVALENT_TIMES, Expression, Format(expected), occurrence.Expectation, CountWord(count, "occurrence", "occurrences")), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string contains <paramref name="expected"/> (honoring <paramref name="config"/>) as many times as <paramref name="occurrence"/> requires.</summary>
    /// <param name="expected">The substring to count (non-overlapping occurrences).</param>
    /// <param name="occurrence">The expected occurrence constraint (e.g. <c>AtLeast.Twice()</c>).</param>
    /// <param name="config">The equivalency options (a <see langword="null"/> value selects the defaults).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The occurrence count does not satisfy <paramref name="occurrence"/>.</exception>
    public AndConstraint<StringAssertions> ContainEquivalentOf(string expected, OccurrenceConstraint occurrence, Func<StringEquivalencyOptions, StringEquivalencyOptions>? config, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var options = ResolveOptions(config);
        if(_value is null) {
            Fail(Strings.Format(StringKeys.STRING_CONTAIN_EQUIVALENT_TIMES_NULL, Expression, Format(expected), occurrence.Expectation), because, becauseArgs);
            return new(this);
        }

        var count = CountEquivalentOccurrences(_value, expected, options);
        if(!occurrence.Matches(count)) {
            Fail(Strings.Format(StringKeys.STRING_CONTAIN_EQUIVALENT_TIMES, Expression, Format(expected), occurrence.Expectation, CountWord(count, "occurrence", "occurrences")), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string does not contain <paramref name="expected"/> ignoring casing. A <see langword="null"/> subject passes.</summary>
    /// <param name="expected">The substring that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string contains <paramref name="expected"/>.</exception>
    public AndConstraint<StringAssertions> NotContainEquivalentOf(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if((_value?.IndexOf(expected, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0) {
            Fail(Strings.Format(StringKeys.STRING_NOT_CONTAIN_EQUIVALENT, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string does not contain <paramref name="expected"/> honoring <paramref name="config"/>. A <see langword="null"/> subject passes.</summary>
    /// <param name="expected">The substring that must be absent.</param>
    /// <param name="config">The equivalency options (a <see langword="null"/> value selects the defaults).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string contains <paramref name="expected"/>.</exception>
    public AndConstraint<StringAssertions> NotContainEquivalentOf(string expected, Func<StringEquivalencyOptions, StringEquivalencyOptions>? config, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var options = ResolveOptions(config);
        if(_value is not null && ContainsEquivalent(_value, expected, options)) {
            Fail(Strings.Format(StringKeys.STRING_NOT_CONTAIN_EQUIVALENT, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }
}
