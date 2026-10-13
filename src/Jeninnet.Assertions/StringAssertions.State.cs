//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class StringAssertions {
    /// <summary>Asserts that the string equals one of the <paramref name="expected"/> values using ordinal comparison.</summary>
    /// <param name="expected">The candidate values, one of which must equal the subject.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string equals none of the candidates.</exception>
    public AndConstraint<StringAssertions> BeOneOf(string?[] expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(!expected.Any(candidate => string.Equals(_value, candidate, StringComparison.Ordinal))) {
            Fail(Strings.Format(StringKeys.STRING_BE_ONE_OF, Expression, Join(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string equals one of the <paramref name="expected"/> values using ordinal comparison.</summary>
    /// <param name="expected">The candidate values, one of which must equal the subject.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The string equals none of the candidates.</exception>
    public AndConstraint<StringAssertions> BeOneOf(params string?[] expected) => BeOneOf(expected, null, []);

    /// <summary>Asserts that the string value is <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The string is not <see langword="null"/>.</exception>
    public AndConstraint<StringAssertions> BeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is not null) {
            Fail(Strings.Format(StringKeys.STRING_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string value is not <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The string is <see langword="null"/>.</exception>
    public AndConstraint<StringAssertions> NotBeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.STRING_NOT_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string is empty.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The string is <see langword="null"/> or not empty.</exception>
    public AndConstraint<StringAssertions> BeEmpty(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.STRING_BE_EMPTY_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        if(_value.Length != 0) {
            Fail(Strings.Format(StringKeys.STRING_BE_EMPTY, Expression, Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string is not empty. A <see langword="null"/> subject passes.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The string is empty.</exception>
    public AndConstraint<StringAssertions> NotBeEmpty(string? because = null, params object?[] becauseArgs) {
        if(_value?.Length == 0) {
            Fail(Strings.Format(StringKeys.STRING_NOT_BE_EMPTY, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string is <see langword="null"/>, empty, or whitespace only.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The string contains a non-whitespace character.</exception>
    public AndConstraint<StringAssertions> BeNullOrWhiteSpace(string? because = null, params object?[] becauseArgs) {
        if(!string.IsNullOrWhiteSpace(_value)) {
            Fail(Strings.Format(StringKeys.STRING_BE_NULL_OR_WHITESPACE, Expression, Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string contains at least one non-whitespace character.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The string is <see langword="null"/>, empty, or whitespace only.</exception>
    public AndConstraint<StringAssertions> NotBeNullOrWhiteSpace(string? because = null, params object?[] becauseArgs) {
        if(string.IsNullOrWhiteSpace(_value)) {
            Fail(Strings.Format(StringKeys.STRING_NOT_BE_NULL_OR_WHITESPACE, Expression, Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string has exactly <paramref name="expected"/> characters.</summary>
    /// <param name="expected">The expected length.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The string is <see langword="null"/> or its length differs.</exception>
    public AndConstraint<StringAssertions> HaveLength(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.STRING_HAVE_LENGTH_NULL, Expression, AssertionScope.FormatInvariant(expected)), because, becauseArgs);
            return new(this);
        }

        if(_value.Length != expected) {
            Fail(Strings.Format(StringKeys.STRING_HAVE_LENGTH, Expression, AssertionScope.FormatInvariant(expected), Format(_value), AssertionScope.FormatInvariant(_value.Length)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string does not have exactly <paramref name="unexpected"/> characters. A <see langword="null"/> subject passes.</summary>
    /// <param name="unexpected">The length the string must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The string has length <paramref name="unexpected"/>.</exception>
    public AndConstraint<StringAssertions> NotHaveLength(int unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value is not null && _value.Length == unexpected) {
            Fail(Strings.Format(StringKeys.STRING_NOT_HAVE_LENGTH, Expression, AssertionScope.FormatInvariant(unexpected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that every cased character in the string is upper-case (caseless characters are ignored).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The string is <see langword="null"/> or contains a lower-case cased character.</exception>
    public AndConstraint<StringAssertions> BeUpperCased(string? because = null, params object?[] becauseArgs) {
        if(_value?.Any(static c => IsCased(c) && c != char.ToUpperInvariant(c)) != false) {
            Fail(Strings.Format(StringKeys.STRING_BE_UPPER, Expression, Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string is not entirely upper-case. Passes for <see langword="null"/> or caseless strings.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The string contains cased characters and all of them are upper-case.</exception>
    public AndConstraint<StringAssertions> NotBeUpperCased(string? because = null, params object?[] becauseArgs) {
        if(_value?.Any(IsCased) == true && _value.All(static c => !IsCased(c) || c == char.ToUpperInvariant(c))) {
            Fail(Strings.Format(StringKeys.STRING_NOT_BE_UPPER, Expression, Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that every cased character in the string is lower-case (caseless characters are ignored).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The string is <see langword="null"/> or contains an upper-case cased character.</exception>
    public AndConstraint<StringAssertions> BeLowerCased(string? because = null, params object?[] becauseArgs) {
        if(_value?.Any(static c => IsCased(c) && c != char.ToLowerInvariant(c)) != false) {
            Fail(Strings.Format(StringKeys.STRING_BE_LOWER, Expression, Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string is not entirely lower-case. Passes for <see langword="null"/> or caseless strings.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The string contains cased characters and all of them are lower-case.</exception>
    public AndConstraint<StringAssertions> NotBeLowerCased(string? because = null, params object?[] becauseArgs) {
        if(_value?.Any(IsCased) == true && _value.All(static c => !IsCased(c) || c == char.ToLowerInvariant(c))) {
            Fail(Strings.Format(StringKeys.STRING_NOT_BE_LOWER, Expression, Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string is <see langword="null"/> or empty.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The string is neither null nor empty.</exception>
    public AndConstraint<StringAssertions> BeNullOrEmpty(string? because = null, params object?[] becauseArgs) {
        if(_value is not null && _value.Length != 0) {
            Fail(Strings.Format(StringKeys.STRING_BE_NULL_OR_EMPTY, Expression, Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string is neither <see langword="null"/> nor empty.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The string is null or empty.</exception>
    public AndConstraint<StringAssertions> NotBeNullOrEmpty(string? because = null, params object?[] becauseArgs) {
        if(string.IsNullOrEmpty(_value)) {
            Fail(Strings.Format(StringKeys.STRING_NOT_BE_NULL_OR_EMPTY, Expression), because, becauseArgs);
        }

        return new(this);
    }
}
