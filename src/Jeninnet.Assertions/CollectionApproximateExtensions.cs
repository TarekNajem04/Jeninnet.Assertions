//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
#if NET8_0_OR_GREATER
namespace Jeninnet.Assertions;

/// <summary>Provides element-wise approximate equality assertions for numeric collections.</summary>
/// <remarks>
/// Available on .NET 8 and later, where <see cref="INumber{TSelf}"/> exists. Comparisons use the
/// type-aware precision rules of <c>NumericComparison</c>: decimal and 64-bit integers stay exact,
/// 128-bit integers and <see cref="BigInteger"/> use arbitrary precision, and NaN never approximates
/// a value. The extension shape keeps the <c>INumber&lt;T&gt;</c> constraint off the non-generic
/// <see cref="CollectionAssertions{T}"/> type, so non-numeric collections never see the API.
/// </remarks>
public static class CollectionApproximateExtensions {
    /// <summary>Asserts that the collection approximately equals <paramref name="expected"/> element by element, in order.</summary>
    /// <typeparam name="T">The numeric element type.</typeparam>
    /// <param name="assertions">The collection assertions to continue from.</param>
    /// <param name="expected">The expected items in order.</param>
    /// <param name="precision">The non-negative maximum accepted absolute difference per element.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assertions"/> or <paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="precision"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/>, has a different length, or an element differs by more than <paramref name="precision"/>.</exception>
    public static AndConstraint<CollectionAssertions<T>> EqualApproximately<T>(
        this CollectionAssertions<T> assertions,
        IEnumerable<T> expected,
        T precision,
        string? because = null,
        params object?[] becauseArgs) where T : struct, INumber<T> {
        if(assertions is null) {
            throw new ArgumentNullException(nameof(assertions));
        }

        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(precision.CompareTo(T.Zero) < 0) {
            throw new ArgumentOutOfRangeException(nameof(precision), "Precision must not be negative.");
        }

        var expectedList = expected.ToList();
        if(assertions.Subject is null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.COLLECTION_EQUAL_APPROXIMATELY_NULL, assertions.Expression, JoinItems(expectedList)), because, becauseArgs));
            return new(assertions);
        }

        var actualList = assertions.Subject.ToList();
        if(actualList.Count != expectedList.Count) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.COLLECTION_EQUAL_APPROXIMATELY_COUNT, assertions.Expression, JoinItems(expectedList), AssertionScope.FormatInvariant(actualList.Count), AssertionScope.FormatInvariant(expectedList.Count)), because, becauseArgs));
            return new(assertions);
        }

        for(var index = 0; index < expectedList.Count; index++) {
            if(!NumericComparison.WithinPrecision(actualList[index], expectedList[index], precision)) {
                AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.COLLECTION_EQUAL_APPROXIMATELY_ORDER, assertions.Expression, JoinItems(expectedList), AssertionScope.FormatInvariant(index), AssertionScope.FormatInvariant(actualList[index]), AssertionScope.FormatInvariant(expectedList[index]), AssertionScope.FormatInvariant(precision)), because, becauseArgs));
                return new(assertions);
            }
        }

        return new(assertions);
    }

    /// <summary>Asserts that the collection does not approximately equal <paramref name="unexpected"/> element by element, in order.</summary>
    /// <typeparam name="T">The numeric element type.</typeparam>
    /// <param name="assertions">The collection assertions to continue from.</param>
    /// <param name="unexpected">The items the collection must not approximately match.</param>
    /// <param name="precision">The non-negative absolute difference that counts as approximately equal.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assertions"/> or <paramref name="unexpected"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="precision"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or approximately equals <paramref name="unexpected"/>.</exception>
    public static AndConstraint<CollectionAssertions<T>> NotEqualApproximately<T>(
        this CollectionAssertions<T> assertions,
        IEnumerable<T> unexpected,
        T precision,
        string? because = null,
        params object?[] becauseArgs) where T : struct, INumber<T> {
        if(assertions is null) {
            throw new ArgumentNullException(nameof(assertions));
        }

        if(unexpected is null) {
            throw new ArgumentNullException(nameof(unexpected));
        }

        if(precision.CompareTo(T.Zero) < 0) {
            throw new ArgumentOutOfRangeException(nameof(precision), "Precision must not be negative.");
        }

        var unexpectedList = unexpected.ToList();
        if(assertions.Subject is null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.COLLECTION_NOT_EQUAL_APPROXIMATELY_NULL, assertions.Expression, JoinItems(unexpectedList)), because, becauseArgs));
            return new(assertions);
        }

        var actualList = assertions.Subject.ToList();
        if(actualList.Count != unexpectedList.Count) {
            return new(assertions);
        }

        for(var index = 0; index < unexpectedList.Count; index++) {
            if(!NumericComparison.WithinPrecision(actualList[index], unexpectedList[index], precision)) {
                return new(assertions);
            }
        }

        AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.COLLECTION_NOT_EQUAL_APPROXIMATELY, assertions.Expression, JoinItems(unexpectedList), AssertionScope.FormatInvariant(precision)), because, becauseArgs));
        return new(assertions);
    }

    /// <summary>Formats expected items for failure messages.</summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="items">The items to format.</param>
    /// <returns>The bracketed, comma-separated, quoted items.</returns>
    private static string JoinItems<TItem>(IEnumerable<TItem> items) =>
        "[" + string.Join(", ", items.Select(static item => $"'{AssertionScope.FormatInvariant(item)}'")) + "]";
}
#endif
