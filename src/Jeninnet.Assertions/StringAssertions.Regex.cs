//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class StringAssertions {
    /// <summary>Asserts that the string contains at least one match of the regular expression <paramref name="pattern"/>.</summary>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pattern"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string is <see langword="null"/> or contains no match.</exception>
    public AndConstraint<StringAssertions> MatchRegex(string pattern, string? because = null, params object?[] becauseArgs) {
        if(pattern is null) {
            throw new ArgumentNullException(nameof(pattern));
        }

        return MatchRegex(CreateRegex(pattern), because, becauseArgs);
    }

    /// <summary>Asserts that the regular expression <paramref name="pattern"/> matches as many times as <paramref name="occurrence"/> requires.</summary>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <param name="occurrence">The expected match-count constraint (e.g. <c>Exactly.Once()</c>).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pattern"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The match count does not satisfy <paramref name="occurrence"/>.</exception>
    public AndConstraint<StringAssertions> MatchRegex(string pattern, OccurrenceConstraint occurrence, string? because = null, params object?[] becauseArgs) {
        if(pattern is null) {
            throw new ArgumentNullException(nameof(pattern));
        }

        return MatchRegex(CreateRegex(pattern), occurrence, because, becauseArgs);
    }

    /// <summary>Asserts that the string contains at least one match of <paramref name="regex"/>.</summary>
    /// <param name="regex">The regular expression.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="regex"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string is <see langword="null"/> or contains no match.</exception>
    public AndConstraint<StringAssertions> MatchRegex(Regex regex, string? because = null, params object?[] becauseArgs) {
        if(regex is null) {
            throw new ArgumentNullException(nameof(regex));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.STRING_MATCH_REGEX_NULL, Expression, Format(regex.ToString())), because, becauseArgs);
            return new(this);
        }

        if(!regex.IsMatch(_value)) {
            Fail(Strings.Format(StringKeys.STRING_MATCH_REGEX, Expression, Format(regex.ToString()), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that <paramref name="regex"/> matches as many times as <paramref name="occurrence"/> requires.</summary>
    /// <param name="regex">The regular expression.</param>
    /// <param name="occurrence">The expected match-count constraint (e.g. <c>AtLeast.Twice()</c>).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="regex"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The match count does not satisfy <paramref name="occurrence"/>.</exception>
    public AndConstraint<StringAssertions> MatchRegex(Regex regex, OccurrenceConstraint occurrence, string? because = null, params object?[] becauseArgs) {
        if(regex is null) {
            throw new ArgumentNullException(nameof(regex));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.STRING_MATCH_REGEX_TIMES_NULL, Expression, Format(regex.ToString()), occurrence.Expectation), because, becauseArgs);
            return new(this);
        }

        var count = 0;
        // Regex.Count is unavailable on .NET Standard 2.0.
#pragma warning disable CA1875 // Build-verified: removing fails net8.0/net10.0.
        count = regex.Matches(_value).Count;
#pragma warning restore CA1875
        if(!occurrence.Matches(count)) {
            Fail(Strings.Format(StringKeys.STRING_MATCH_REGEX_TIMES, Expression, Format(regex.ToString()), occurrence.Expectation, CountWord(count, "match", "matches")), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string contains no match of the regular expression <paramref name="pattern"/>. A <see langword="null"/> subject passes.</summary>
    /// <param name="pattern">The regular expression pattern that must not match.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pattern"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string matches <paramref name="pattern"/>.</exception>
    public AndConstraint<StringAssertions> NotMatchRegex(string pattern, string? because = null, params object?[] becauseArgs) {
        if(pattern is null) {
            throw new ArgumentNullException(nameof(pattern));
        }

        return NotMatchRegex(CreateRegex(pattern), because, becauseArgs);
    }

    /// <summary>Asserts that the string contains no match of <paramref name="regex"/>. A <see langword="null"/> subject passes.</summary>
    /// <param name="regex">The regular expression that must not match.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="regex"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string matches <paramref name="regex"/>.</exception>
    public AndConstraint<StringAssertions> NotMatchRegex(Regex regex, string? because = null, params object?[] becauseArgs) {
        if(regex is null) {
            throw new ArgumentNullException(nameof(regex));
        }

        if(_value is not null && regex.IsMatch(_value)) {
            Fail(Strings.Format(StringKeys.STRING_NOT_MATCH_REGEX, Expression, Format(regex.ToString()), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    private static readonly TimeSpan _regexTimeout = TimeSpan.FromSeconds(1);

    private static Regex CreateRegex(string pattern) => new(pattern, RegexOptions.None, _regexTimeout);
}
