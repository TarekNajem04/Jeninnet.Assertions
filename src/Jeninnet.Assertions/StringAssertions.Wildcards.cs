//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class StringAssertions {
    /// <summary>
    /// Asserts that the string matches the wildcard <paramref name="pattern"/>, where <c>*</c> matches zero or more
    /// characters and <c>?</c> matches exactly one character. The match is case-sensitive and must cover the whole string.
    /// </summary>
    /// <param name="pattern">The wildcard pattern to match against.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pattern"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string is <see langword="null"/> or does not match.</exception>
    public AndConstraint<StringAssertions> Match(string pattern, string? because = null, params object?[] becauseArgs) {
        if(pattern is null) {
            throw new ArgumentNullException(nameof(pattern));
        }

        if(_value is null || !TextMatching.WildcardMatch(_value, pattern, ignoreCase: false)) {
            var detail = _value is null ? NULL_DETAIL : $"but found {Format(_value)}";
            Fail(Strings.Format(StringKeys.STRING_MATCH, Expression, Format(pattern), detail), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string does not match the wildcard <paramref name="pattern"/>. A <see langword="null"/> subject passes.</summary>
    /// <param name="pattern">The wildcard pattern the string must not match.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pattern"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string matches <paramref name="pattern"/>.</exception>
    public AndConstraint<StringAssertions> NotMatch(string pattern, string? because = null, params object?[] becauseArgs) {
        if(pattern is null) {
            throw new ArgumentNullException(nameof(pattern));
        }

        if(_value is not null && TextMatching.WildcardMatch(_value, pattern, ignoreCase: false)) {
            Fail(Strings.Format(StringKeys.STRING_NOT_MATCH, Expression, Format(pattern), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string matches the wildcard <paramref name="pattern"/> ignoring casing.</summary>
    /// <param name="pattern">The wildcard pattern to match against.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pattern"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string is <see langword="null"/> or does not match.</exception>
    public AndConstraint<StringAssertions> MatchEquivalentOf(string pattern, string? because = null, params object?[] becauseArgs) {
        if(pattern is null) {
            throw new ArgumentNullException(nameof(pattern));
        }

        if(_value is null || !TextMatching.WildcardMatch(_value, pattern, ignoreCase: true)) {
            var detail = _value is null ? NULL_DETAIL : $"but found {Format(_value)}";
            Fail(Strings.Format(StringKeys.STRING_MATCH_EQUIVALENT, Expression, Format(pattern), detail), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string matches the wildcard <paramref name="pattern"/> honoring <paramref name="config"/>.</summary>
    /// <param name="pattern">The wildcard pattern to match against.</param>
    /// <param name="config">The equivalency options (a <see langword="null"/> value selects the defaults).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pattern"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string is <see langword="null"/> or does not match.</exception>
    public AndConstraint<StringAssertions> MatchEquivalentOf(string pattern, Func<StringEquivalencyOptions, StringEquivalencyOptions>? config, string? because = null, params object?[] becauseArgs) {
        if(pattern is null) {
            throw new ArgumentNullException(nameof(pattern));
        }

        var options = ResolveOptions(config);
        if(_value is null || !MatchesEquivalent(_value, pattern, options)) {
            var detail = _value is null ? NULL_DETAIL : $"but found {Format(_value)}";
            Fail(Strings.Format(StringKeys.STRING_MATCH_EQUIVALENT, Expression, Format(pattern), detail), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string does not match the wildcard <paramref name="pattern"/> ignoring casing. A <see langword="null"/> subject passes.</summary>
    /// <param name="pattern">The wildcard pattern the string must not match.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pattern"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string matches <paramref name="pattern"/>.</exception>
    public AndConstraint<StringAssertions> NotMatchEquivalentOf(string pattern, string? because = null, params object?[] becauseArgs) {
        if(pattern is null) {
            throw new ArgumentNullException(nameof(pattern));
        }

        if(_value is not null && TextMatching.WildcardMatch(_value, pattern, ignoreCase: true)) {
            Fail(Strings.Format(StringKeys.STRING_NOT_MATCH_EQUIVALENT, Expression, Format(pattern), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }
}
