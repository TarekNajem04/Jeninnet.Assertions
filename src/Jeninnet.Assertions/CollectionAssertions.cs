//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying <see cref="IEnumerable{T}"/> collections.</summary>
/// <typeparam name="T">The element type of the collection.</typeparam>
/// <param name="value">The collection to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public partial class CollectionAssertions<T>(IEnumerable<T>? value, string? expression = null) : AssertionsBase(expression, "collection") {
    private readonly IEnumerable<T>? _value = value;

    /// <summary>Gets the collection under assertion (for extensions in this assembly).</summary>
    internal IEnumerable<T>? Subject => _value;

    private static string Format<TValue>(TValue value) => AssertionScope.FormatInvariant(value);

    private static string JoinItems(IEnumerable<T> items) => "[" + string.Join(", ", items.Select(static x => $"'{AssertionScope.FormatInvariant(x)}'")) + "]";

    private static string JoinExpectations<TExpectation>(IEnumerable<TExpectation> items) => "[" + string.Join(", ", items.Select(static x => $"'{AssertionScope.FormatInvariant(x)}'")) + "]";

    /// <summary>Asserts that the collection contains exactly <paramref name="expected"/> items.</summary>
    /// <param name="expected">The expected item count.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or its count does not match.</exception>
    public AndConstraint<CollectionAssertions<T>> HaveCount(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_HAVE_EXACTLY_NULL, Expression, Format(expected)), because, becauseArgs);
            return new(this);
        }

        var list = _value.ToList();
        if(list.Count != expected) {
            Fail(Strings.Format(StringKeys.COLLECTION_HAVE_EXACTLY, Expression, Format(expected), Format(list.Count)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the item count satisfies <paramref name="predicate"/>.</summary>
    /// <param name="predicate">The condition the count must satisfy.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or its count does not satisfy the predicate.</exception>
    public AndConstraint<CollectionAssertions<T>> HaveCount(Expression<Func<int, bool>> predicate, string? because = null, params object?[] becauseArgs) {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_HAVE_COUNT_PREDICATE, Expression, predicate.Body, "<null>"), because, becauseArgs);
            return new(this);
        }

        var count = _value.ToList().Count;
        if(!predicate.Compile()(count)) {
            Fail(Strings.Format(StringKeys.COLLECTION_HAVE_COUNT_PREDICATE, Expression, predicate.Body, Format(count)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the collection contains more than <paramref name="expected"/> items.</summary>
    /// <param name="expected">The exclusive lower bound for the item count.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or too small.</exception>
    public AndConstraint<CollectionAssertions<T>> HaveCountGreaterThan(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_HAVE_MORE_THAN_NULL, Expression, Format(expected)), because, becauseArgs);
            return new(this);
        }

        var count = _value.Count();
        if(count <= expected) {
            Fail(Strings.Format(StringKeys.COLLECTION_HAVE_MORE_THAN, Expression, Format(expected), Format(count)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the collection contains at least <paramref name="expected"/> items.</summary>
    /// <param name="expected">The inclusive lower bound for the item count.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or too small.</exception>
    public AndConstraint<CollectionAssertions<T>> HaveCountGreaterThanOrEqualTo(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_HAVE_AT_LEAST_NULL, Expression, Format(expected)), because, becauseArgs);
            return new(this);
        }

        var count = _value.Count();
        if(count < expected) {
            Fail(Strings.Format(StringKeys.COLLECTION_HAVE_AT_LEAST, Expression, Format(expected), Format(count)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the collection contains fewer than <paramref name="expected"/> items.</summary>
    /// <param name="expected">The exclusive upper bound for the item count.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or too large.</exception>
    public AndConstraint<CollectionAssertions<T>> HaveCountLessThan(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_HAVE_FEWER_THAN_NULL, Expression, Format(expected)), because, becauseArgs);
            return new(this);
        }

        var count = _value.Count();
        if(count >= expected) {
            Fail(Strings.Format(StringKeys.COLLECTION_HAVE_FEWER_THAN, Expression, Format(expected), Format(count)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the collection contains at most <paramref name="expected"/> items.</summary>
    /// <param name="expected">The inclusive upper bound for the item count.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or too large.</exception>
    public AndConstraint<CollectionAssertions<T>> HaveCountLessThanOrEqualTo(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_HAVE_AT_MOST_NULL, Expression, Format(expected)), because, becauseArgs);
            return new(this);
        }

        var count = _value.Count();
        if(count > expected) {
            Fail(Strings.Format(StringKeys.COLLECTION_HAVE_AT_MOST, Expression, Format(expected), Format(count)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the collection has the same number of items as <paramref name="expected"/>.</summary>
    /// <typeparam name="TOther">The element type of the expected collection.</typeparam>
    /// <param name="expected">The collection to compare counts with.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or the counts differ.</exception>
    public AndConstraint<CollectionAssertions<T>> HaveSameCount<TOther>(IEnumerable<TOther> expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var expectedList = expected.ToList();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_HAVE_SAME_COUNT_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        var actualList = _value.ToList();
        if(actualList.Count != expectedList.Count) {
            Fail(Strings.Format(StringKeys.COLLECTION_HAVE_SAME_COUNT, Expression, Format(expectedList.Count), Format(actualList.Count)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the collection is <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection is not <see langword="null"/>.</exception>
    public AndConstraint<CollectionAssertions<T>> BeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is not null) {
            Fail(Strings.Format(StringKeys.COLLECTION_BE_NULL, Expression, Format(_value.Count())), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the collection is not <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/>.</exception>
    public AndConstraint<CollectionAssertions<T>> NotBeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the collection is empty.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or contains at least one item.</exception>
    public AndConstraint<CollectionAssertions<T>> BeEmpty(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_BE_EMPTY_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        var list = _value.ToList();
        if(list.Count != 0) {
            Fail(Strings.Format(StringKeys.COLLECTION_BE_EMPTY, Expression, Format(list.Count)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the collection is <see langword="null"/> or empty.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection contains at least one item.</exception>
    public AndConstraint<CollectionAssertions<T>> BeNullOrEmpty(string? because = null, params object?[] becauseArgs) {
        if(_value is not null && _value.Any()) {
            Fail(Strings.Format(StringKeys.COLLECTION_BE_NULL_OR_EMPTY, Expression, Format(_value.Count())), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the collection is neither <see langword="null"/> nor empty.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or empty.</exception>
    public AndConstraint<CollectionAssertions<T>> NotBeNullOrEmpty(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_BE_NULL_OR_EMPTY_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        if(!_value.Any()) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_BE_NULL_OR_EMPTY, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the collection is not empty.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or is empty.</exception>
    public AndConstraint<CollectionAssertions<T>> NotBeEmpty(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_BE_EMPTY_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        if(!_value.Any()) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_BE_EMPTY, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the collection does not contain exactly the specified number of items.</summary>
    /// <param name="unexpected">The item count that must not match.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection contains exactly the specified number of items.</exception>
    public AndConstraint<CollectionAssertions<T>> NotHaveCount(int unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_HAVE_EXACTLY_NULL, Expression, Format(unexpected)), because, becauseArgs);
            return new(this);
        }

        var list = _value.ToList();
        if(list.Count == unexpected) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_HAVE_EXACTLY, Expression, Format(unexpected), Format(list.Count)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the collection does not have the same number of items as the other collection.</summary>
    /// <typeparam name="TOther">The type of elements in the other collection.</typeparam>
    /// <param name="other">The other collection.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collections have the same number of items.</exception>
    public AndConstraint<CollectionAssertions<T>> NotHaveSameCount<TOther>(IEnumerable<TOther> other, string? because = null, params object?[] becauseArgs) {
        if(other is null) {
            throw new ArgumentNullException(nameof(other));
        }

        var otherList = other.ToList();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_HAVE_SAME_COUNT_NULL, Expression, Format(otherList.Count)), because, becauseArgs);
            return new(this);
        }

        var actualList = _value.ToList();
        if(actualList.Count == otherList.Count) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_HAVE_SAME_COUNT, Expression, Format(otherList.Count), Format(actualList.Count)), because, becauseArgs);
        }

        return new(this);
    }
}
