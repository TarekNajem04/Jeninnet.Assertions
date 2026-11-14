//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class GenericDictionaryAssertions<TKey, TValue> {
    /// <summary>Asserts that the dictionary contains more than <paramref name="expected"/> entries.</summary>
    /// <param name="expected">The exclusive lower bound for the entry count.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or too small.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> HaveCountGreaterThan(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_HAVE_MORE_THAN_NULL, Expression, AssertionScope.FormatInvariant(expected)), because, becauseArgs);
            return new(this);
        }

        if(_value.Count <= expected) {
            Fail(Strings.Format(StringKeys.DICT_HAVE_MORE_THAN, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_value.Count)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the dictionary contains at least <paramref name="expected"/> entries.</summary>
    /// <param name="expected">The inclusive lower bound for the entry count.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or too small.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> HaveCountGreaterThanOrEqualTo(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_HAVE_AT_LEAST_NULL, Expression, AssertionScope.FormatInvariant(expected)), because, becauseArgs);
            return new(this);
        }

        if(_value.Count < expected) {
            Fail(Strings.Format(StringKeys.DICT_HAVE_AT_LEAST, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_value.Count)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the dictionary contains fewer than <paramref name="expected"/> entries.</summary>
    /// <param name="expected">The exclusive upper bound for the entry count.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or too large.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> HaveCountLessThan(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_HAVE_FEWER_THAN_NULL, Expression, AssertionScope.FormatInvariant(expected)), because, becauseArgs);
            return new(this);
        }

        if(_value.Count >= expected) {
            Fail(Strings.Format(StringKeys.DICT_HAVE_FEWER_THAN, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_value.Count)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the dictionary contains at most <paramref name="expected"/> entries.</summary>
    /// <param name="expected">The inclusive upper bound for the entry count.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or too large.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> HaveCountLessThanOrEqualTo(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_HAVE_AT_MOST_NULL, Expression, AssertionScope.FormatInvariant(expected)), because, becauseArgs);
            return new(this);
        }

        if(_value.Count > expected) {
            Fail(Strings.Format(StringKeys.DICT_HAVE_AT_MOST, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_value.Count)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the dictionary contains exactly one entry and exposes it for further assertions.</summary>
    /// <remarks>
    /// This assertion returns a value rather than an <see cref="AndConstraint{TParent}"/>, so it throws
    /// immediately even inside an <see cref="AssertionScope"/> instead of being batched.
    /// </remarks>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the single key/value pair.</returns>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or does not contain exactly one entry.</exception>
    public WhichConstraint<KeyValuePair<TKey, TValue>> ContainSingle(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.DICT_CONTAIN_SINGLE_NULL, Expression), because, becauseArgs));
        }

        if(_value.Count != 1) {
            throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.DICT_CONTAIN_SINGLE, Expression, AssertionScope.FormatInvariant(_value.Count)), because, becauseArgs));
        }

        return new WhichConstraint<KeyValuePair<TKey, TValue>>(_value.First());
    }
}
