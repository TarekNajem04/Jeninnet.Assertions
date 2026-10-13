//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class CollectionAssertions<T> {
    /// <summary>Asserts that the collection starts with the items in <paramref name="expected"/> (in order).</summary>
    /// <param name="expected">The expected leading items.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/>, too short, or differs at the start.</exception>
    public AndConstraint<CollectionAssertions<T>> StartWith(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var expectedList = expected.ToList();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_START_WITH_NULL, Expression, JoinItems(expectedList)), because, becauseArgs);
            return new(this);
        }

        var actualList = _value.ToList();
        if(actualList.Count < expectedList.Count) {
            Fail(Strings.Format(StringKeys.COLLECTION_START_WITH_SHORT, Expression, JoinItems(expectedList), Format(actualList.Count)), because, becauseArgs);
            return new(this);
        }

        for(var index = 0; index < expectedList.Count; index++) {
            if(!EqualityComparer<T>.Default.Equals(actualList[index], expectedList[index])) {
                Fail(Strings.Format(StringKeys.COLLECTION_START_WITH_ORDER, Expression, JoinItems(expectedList), Format(index), Format(actualList[index])), because, becauseArgs);
                return new(this);
            }
        }

        return new(this);
    }

    /// <summary>Asserts that the collection starts with the items in <paramref name="expected"/> (in order).</summary>
    /// <param name="expected">The expected leading items.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/>, too short, or differs at the start.</exception>
    public AndConstraint<CollectionAssertions<T>> StartWith(params T[] expected) => StartWith((IEnumerable<T>)expected);

    /// <summary>Asserts that the collection starts with <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected first item.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/>, empty, or starts with another item.</exception>
    public AndConstraint<CollectionAssertions<T>> StartWith(T expected, string? because = null, params object?[] becauseArgs) =>
        StartWith([expected], because, becauseArgs);

    /// <summary>
    /// Asserts that the collection starts with the items in <paramref name="expected"/> (in order),
    /// comparing element-wise with <paramref name="equality"/>.
    /// </summary>
    /// <typeparam name="TExpectation">The expected element type.</typeparam>
    /// <param name="expected">The expected leading items.</param>
    /// <param name="equality">The element comparer.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> or <paramref name="equality"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/>, too short, or differs at the start.</exception>
    public AndConstraint<CollectionAssertions<T>> StartWith<TExpectation>(IEnumerable<TExpectation> expected, Func<T, TExpectation, bool> equality, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(equality is null) {
            throw new ArgumentNullException(nameof(equality));
        }

        var expectedList = expected.ToList();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_START_WITH_NULL, Expression, JoinExpectations(expectedList)), because, becauseArgs);
            return new(this);
        }

        var actualList = _value.ToList();
        if(actualList.Count < expectedList.Count) {
            Fail(Strings.Format(StringKeys.COLLECTION_START_WITH_SHORT, Expression, JoinExpectations(expectedList), Format(actualList.Count)), because, becauseArgs);
            return new(this);
        }

        for(var index = 0; index < expectedList.Count; index++) {
            if(!equality(actualList[index], expectedList[index])) {
                Fail(Strings.Format(StringKeys.COLLECTION_START_WITH_ORDER, Expression, JoinExpectations(expectedList), Format(index), Format(actualList[index])), because, becauseArgs);
                return new(this);
            }
        }

        return new(this);
    }

    /// <summary>Asserts that the collection ends with the items in <paramref name="expected"/> (in order).</summary>
    /// <param name="expected">The expected trailing items.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/>, too short, or differs at the end.</exception>
    public AndConstraint<CollectionAssertions<T>> EndWith(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var expectedList = expected.ToList();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_END_WITH_NULL, Expression, JoinItems(expectedList)), because, becauseArgs);
            return new(this);
        }

        var actualList = _value.ToList();
        if(actualList.Count < expectedList.Count) {
            Fail(Strings.Format(StringKeys.COLLECTION_END_WITH_SHORT, Expression, JoinItems(expectedList), Format(actualList.Count)), because, becauseArgs);
            return new(this);
        }

        var offset = actualList.Count - expectedList.Count;
        for(var index = 0; index < expectedList.Count; index++) {
            if(!EqualityComparer<T>.Default.Equals(actualList[offset + index], expectedList[index])) {
                Fail(Strings.Format(StringKeys.COLLECTION_END_WITH_ORDER, Expression, JoinItems(expectedList), Format(offset + index), Format(actualList[offset + index])), because, becauseArgs);
                return new(this);
            }
        }

        return new(this);
    }

    /// <summary>Asserts that the collection ends with the items in <paramref name="expected"/> (in order).</summary>
    /// <param name="expected">The expected trailing items.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/>, too short, or differs at the end.</exception>
    public AndConstraint<CollectionAssertions<T>> EndWith(params T[] expected) => EndWith((IEnumerable<T>)expected);

    /// <summary>Asserts that the collection ends with <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected last item.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/>, empty, or ends with another item.</exception>
    public AndConstraint<CollectionAssertions<T>> EndWith(T expected, string? because = null, params object?[] becauseArgs) =>
        EndWith([expected], because, becauseArgs);

    /// <summary>
    /// Asserts that the collection ends with the items in <paramref name="expected"/> (in order),
    /// comparing element-wise with <paramref name="equality"/>.
    /// </summary>
    /// <typeparam name="TExpectation">The expected element type.</typeparam>
    /// <param name="expected">The expected trailing items.</param>
    /// <param name="equality">The element comparer.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> or <paramref name="equality"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/>, too short, or differs at the end.</exception>
    public AndConstraint<CollectionAssertions<T>> EndWith<TExpectation>(IEnumerable<TExpectation> expected, Func<T, TExpectation, bool> equality, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(equality is null) {
            throw new ArgumentNullException(nameof(equality));
        }

        var expectedList = expected.ToList();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_END_WITH_NULL, Expression, JoinExpectations(expectedList)), because, becauseArgs);
            return new(this);
        }

        var actualList = _value.ToList();
        if(actualList.Count < expectedList.Count) {
            Fail(Strings.Format(StringKeys.COLLECTION_END_WITH_SHORT, Expression, JoinExpectations(expectedList), Format(actualList.Count)), because, becauseArgs);
            return new(this);
        }

        var offset = actualList.Count - expectedList.Count;
        for(var index = 0; index < expectedList.Count; index++) {
            if(!equality(actualList[offset + index], expectedList[index])) {
                Fail(Strings.Format(StringKeys.COLLECTION_END_WITH_ORDER, Expression, JoinExpectations(expectedList), Format(offset + index), Format(actualList[offset + index])), because, becauseArgs);
                return new(this);
            }
        }

        return new(this);
    }
}
