//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class CollectionAssertions<T> {
    /// <summary>
    /// Asserts that the collection contains exactly the items in <paramref name="expected"/> in the same order.
    /// </summary>
    /// <param name="expected">The expected items in order.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or differs in content or order.</exception>
    public AndConstraint<CollectionAssertions<T>> Equal(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var expectedList = expected.ToList();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_EQUAL_NULL, Expression, JoinItems(expectedList)), because, becauseArgs);
            return new(this);
        }

        var actualList = _value.ToList();
        if(actualList.Count != expectedList.Count) {
            Fail(Strings.Format(StringKeys.COLLECTION_EQUAL_COUNT, Expression, JoinItems(expectedList), Format(actualList.Count), Format(expectedList.Count)), because, becauseArgs);
            return new(this);
        }

        for(var index = 0; index < expectedList.Count; index++) {
            if(!EqualityComparer<T>.Default.Equals(actualList[index], expectedList[index])) {
                Fail(Strings.Format(StringKeys.COLLECTION_EQUAL_ORDER, Expression, JoinItems(expectedList), Format(index), Format(actualList[index]), Format(expectedList[index])), because, becauseArgs);
                return new(this);
            }
        }

        return new(this);
    }

    /// <summary>
    /// Asserts that the collection contains exactly the items in <paramref name="expected"/> in the same order.
    /// </summary>
    /// <param name="expected">The expected items in order.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or differs in content or order.</exception>
    public AndConstraint<CollectionAssertions<T>> Equal(params T[] expected) => Equal((IEnumerable<T>)expected);

    /// <summary>
    /// Asserts that the collection contains exactly the items in <paramref name="expected"/> in the same order,
    /// comparing element-wise with <paramref name="equality"/>.
    /// </summary>
    /// <typeparam name="TExpectation">The expected element type.</typeparam>
    /// <param name="expected">The expected items in order.</param>
    /// <param name="equality">The element comparer (both collections must have the same length).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> or <paramref name="equality"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or differs in content or order.</exception>
    public AndConstraint<CollectionAssertions<T>> Equal<TExpectation>(IEnumerable<TExpectation> expected, Func<T, TExpectation, bool> equality, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(equality is null) {
            throw new ArgumentNullException(nameof(equality));
        }

        var expectedList = expected.ToList();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_EQUAL_NULL, Expression, JoinExpectations(expectedList)), because, becauseArgs);
            return new(this);
        }

        var actualList = _value.ToList();
        if(actualList.Count != expectedList.Count) {
            Fail(Strings.Format(StringKeys.COLLECTION_EQUAL_COUNT, Expression, JoinExpectations(expectedList), Format(actualList.Count), Format(expectedList.Count)), because, becauseArgs);
            return new(this);
        }

        for(var index = 0; index < expectedList.Count; index++) {
            if(!equality(actualList[index], expectedList[index])) {
                Fail(Strings.Format(StringKeys.COLLECTION_EQUAL_ORDER, Expression, JoinExpectations(expectedList), Format(index), Format(actualList[index]), Format(expectedList[index])), because, becauseArgs);
                return new(this);
            }
        }

        return new(this);
    }

    /// <summary>Counts each distinct element using the supplied comparer.</summary>
    /// <param name="source">The sequence to count.</param>
    /// <param name="equality">The element comparer.</param>
    /// <returns>A map from element to occurrence count.</returns>
    private static Dictionary<object, int> CountByElement(IEnumerable<T> source, IEqualityComparer<T> equality) {
        var counts = new Dictionary<object, int>(new ComparerAdapter(equality));
        foreach(var item in source) {
            counts.TryGetValue(item!, out var current);
            counts[item!] = current + 1;
        }

        return counts;
    }

    /// <summary>Adapts a typed comparer to <see cref="IEqualityComparer{T}"/> of <see cref="object"/> for element counting.</summary>
    /// <param name="inner">The typed comparer to delegate to.</param>
    private sealed class ComparerAdapter(IEqualityComparer<T> inner) : IEqualityComparer<object> {
        public new bool Equals(object? x, object? y) {
            if(ReferenceEquals(x, y)) {
                return true;
            }

            if(x is null || y is null) {
                return false;
            }

            if(x is not T typedX || y is not T typedY) {
                return false;
            }

            return inner.Equals(typedX, typedY);
        }

        public int GetHashCode(object obj) => obj is T typed ? inner.GetHashCode(typed) : 0;
    }

    /// <summary>Asserts that the collection does not equal the specified collection (order-sensitive).</summary>
    /// <param name="unexpected">The collection that must not match.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="unexpected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collections are equal.</exception>
    public AndConstraint<CollectionAssertions<T>> NotEqual(IEnumerable<T> unexpected, string? because = null, params object?[] becauseArgs) {
        if(unexpected is null) {
            throw new ArgumentNullException(nameof(unexpected));
        }

        var unexpectedList = unexpected.ToList();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_EQUAL_NULL, Expression, JoinItems(unexpectedList)), because, becauseArgs);
            return new(this);
        }

        var actualList = _value.ToList();
        if(actualList.Count == unexpectedList.Count) {
            var allMatch = true;
            for(var index = 0; index < unexpectedList.Count; index++) {
                if(!EqualityComparer<T>.Default.Equals(actualList[index], unexpectedList[index])) {
                    allMatch = false;
                    break;
                }
            }

            if(allMatch) {
                Fail(Strings.Format(StringKeys.COLLECTION_NOT_EQUAL, Expression, JoinItems(unexpectedList)), because, becauseArgs);
            }
        }

        return new(this);
    }

    /// <summary>
    /// Asserts that the collection contains exactly the items in <paramref name="expected"/> in the same order
    /// (readability alias of <see cref="Equal(IEnumerable{T}, string, object[])"/>).
    /// </summary>
    /// <param name="expected">The expected items in order.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or differs in content or order.</exception>
    public AndConstraint<CollectionAssertions<T>> BeEqualTo(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) =>
        Equal(expected, because, becauseArgs);

    /// <summary>
    /// Asserts that the collection contains exactly the items in <paramref name="expected"/> in the same order
    /// (readability alias of the <c>Equal</c> array overload).
    /// </summary>
    /// <param name="expected">The expected items in order.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or differs in content or order.</exception>
    public AndConstraint<CollectionAssertions<T>> BeEqualTo(T[] expected, string? because = null, params object?[] becauseArgs) =>
        Equal(expected, because, becauseArgs);

    /// <summary>
    /// Asserts that the collection contains exactly the items in <paramref name="expected"/> in the same order,
    /// comparing element-wise with <paramref name="equality"/> (readability alias of the <c>Equal</c> predicate overload).
    /// </summary>
    /// <typeparam name="TExpectation">The expected element type.</typeparam>
    /// <param name="expected">The expected items in order.</param>
    /// <param name="equality">The element comparer (both collections must have the same length).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> or <paramref name="equality"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or differs in content or order.</exception>
    public AndConstraint<CollectionAssertions<T>> BeEqualTo<TExpectation>(IEnumerable<TExpectation> expected, Func<T, TExpectation, bool> equality, string? because = null, params object?[] becauseArgs) =>
        Equal(expected, equality, because, becauseArgs);

    /// <summary>
    /// Asserts that the collection does not contain exactly the items in <paramref name="unexpected"/> in the same order
    /// (readability alias of <see cref="NotEqual(IEnumerable{T}, string, object[])"/>).
    /// </summary>
    /// <param name="unexpected">The items that must differ in content or order.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="unexpected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection equals <paramref name="unexpected"/> in content and order.</exception>
    public AndConstraint<CollectionAssertions<T>> NotBeEqualTo(IEnumerable<T> unexpected, string? because = null, params object?[] becauseArgs) =>
        NotEqual(unexpected, because, becauseArgs);
}
