//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides string-specific assertion methods for string collections, on top of <see cref="CollectionAssertions{T}"/>.</summary>
/// <param name="value">The string collection to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public class StringCollectionAssertions(IEnumerable<string>? value, string? expression = null) : CollectionAssertions<string>(value, expression) {
    private static string FormatItem(string? value) => AssertionScope.FormatInvariant(value);

    /// <summary>Asserts that every item equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The value every item must equal.</param>
    /// <param name="config">Optional equivalency configuration.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or an item differs.</exception>
    public AndConstraint<StringCollectionAssertions> AllBe(
        string expected,
        Func<SelfReferenceEquivalencyOptions<string>, SelfReferenceEquivalencyOptions<string>>? config,
        string? because = null,
        params object?[] becauseArgs) {
        if(Subject is null) {
            Fail(Strings.Format(StringKeys.STRING_COLLECTION_ALL_BE_NULL, Expression, FormatItem(expected)), because, becauseArgs);
            return new(this);
        }

        var options = config?.Invoke(new SelfReferenceEquivalencyOptions<string>()) ?? new SelfReferenceEquivalencyOptions<string>();
        foreach(var item in Subject) {
            if(EquivalencyEngine.FindDifferences(item, expected, options).Count != 0) {
                Fail(Strings.Format(StringKeys.STRING_COLLECTION_ALL_BE, Expression, FormatItem(expected), FormatItem(item)), because, becauseArgs);
                return new(this);
            }
        }

        return new(this);
    }

    /// <summary>Asserts that every item equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The value every item must equal.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or an item differs.</exception>
    public AndConstraint<StringCollectionAssertions> AllBe(string expected, string? because = null, params object?[] becauseArgs) =>
        AllBe(expected, null, because, becauseArgs);

    /// <summary>Asserts that at least one item matches the wildcard <paramref name="pattern"/> (<c>*</c> and <c>?</c>) and exposes it.</summary>
    /// <param name="pattern">The wildcard pattern.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the first matching item.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pattern"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or nothing matches.</exception>
    public WhichConstraint<string> ContainMatch(string pattern, string? because = null, params object?[] becauseArgs) {
        if(pattern is null) {
            throw new ArgumentNullException(nameof(pattern));
        }

        if(Subject is null) {
            throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.STRING_COLLECTION_CONTAIN_MATCH_NULL, Expression, FormatItem(pattern)), because, becauseArgs));
        }

        var match = Subject.FirstOrDefault(item => item is not null && TextMatching.WildcardMatch(item, pattern, ignoreCase: false));
        if(match is null) {
            throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.STRING_COLLECTION_CONTAIN_MATCH, Expression, FormatItem(pattern)), because, becauseArgs));
        }

        return new WhichConstraint<string>(match);
    }

    /// <summary>Asserts that no item matches the wildcard <paramref name="pattern"/> (<c>*</c> and <c>?</c>).</summary>
    /// <param name="pattern">The wildcard pattern that must match nothing.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pattern"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or an item matches.</exception>
    public AndConstraint<StringCollectionAssertions> NotContainMatch(string pattern, string? because = null, params object?[] becauseArgs) {
        if(pattern is null) {
            throw new ArgumentNullException(nameof(pattern));
        }

        if(Subject is null) {
            Fail(Strings.Format(StringKeys.STRING_COLLECTION_NOT_CONTAIN_MATCH_NULL, Expression, FormatItem(pattern)), because, becauseArgs);
            return new(this);
        }

        var match = Subject.FirstOrDefault(item => item is not null && TextMatching.WildcardMatch(item, pattern, ignoreCase: false));
        if(match is not null) {
            Fail(Strings.Format(StringKeys.STRING_COLLECTION_NOT_CONTAIN_MATCH, Expression, FormatItem(pattern), FormatItem(match)), because, becauseArgs);
            return new(this);
        }

        return new(this);
    }
}
