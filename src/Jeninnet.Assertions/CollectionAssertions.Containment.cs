//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class CollectionAssertions<T> {
    /// <summary>Asserts that the collection contains at least one item matching the given <paramref name="predicate"/>.</summary>
    /// <param name="predicate">A function to test each element against.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or no item matches <paramref name="predicate"/>.</exception>
    public AndConstraint<CollectionAssertions<T>> Contain(Func<T, bool> predicate, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_CONTAIN_PREDICATE_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        if(!_value.Any(predicate)) {
            Fail(Strings.Format(StringKeys.COLLECTION_CONTAIN_PREDICATE_MISSING, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the collection contains the specified <paramref name="item"/>.</summary>
    /// <param name="item">The item to locate in the collection.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or does not contain <paramref name="item"/>.</exception>
    public AndConstraint<CollectionAssertions<T>> Contain(T item, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_CONTAIN_ITEM_NULL, Expression, Format(item)), because, becauseArgs);
            return new(this);
        }

        if(!_value.Contains(item)) {
            var message = Strings.Format(StringKeys.COLLECTION_CONTAIN_ITEM_MISSING, Expression, Format(item));
            if(item is string text) {
                message += NearestCandidate.Describe(text, _value.OfType<string>());
            }

            Fail(message, because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the collection contains an item structurally equivalent to <paramref name="expectation"/>.</summary>
    /// <param name="expectation">The expected object graph.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or holds no equivalent item.</exception>
    public AndConstraint<CollectionAssertions<T>> ContainEquivalentOf(object? expectation, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_CONTAIN_EQUIVALENT_NULL, Expression, Format(expectation)), because, becauseArgs);
            return new(this);
        }

        var options = new SelfReferenceEquivalencyOptions<object>();
        if(!_value.Any(item => EquivalencyEngine.FindDifferences((object?)item, expectation, options).Count == 0)) {
            Fail(Strings.Format(StringKeys.COLLECTION_CONTAIN_EQUIVALENT, Expression, Format(expectation)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the collection holds no item structurally equivalent to <paramref name="expectation"/>.</summary>
    /// <param name="expectation">The unexpected object graph.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or holds an equivalent item.</exception>
    public AndConstraint<CollectionAssertions<T>> NotContainEquivalentOf(object? expectation, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_CONTAIN_EQUIVALENT_NULL, Expression, Format(expectation)), because, becauseArgs);
            return new(this);
        }

        var options = new SelfReferenceEquivalencyOptions<object>();
        if(_value.Any(item => EquivalencyEngine.FindDifferences((object?)item, expectation, options).Count == 0)) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_CONTAIN_EQUIVALENT, Expression, Format(expectation)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that every item in the collection matches <paramref name="predicate"/>.</summary>
    /// <param name="predicate">A function every element must satisfy.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or an item does not match.</exception>
    public AndConstraint<CollectionAssertions<T>> OnlyContain(Func<T, bool> predicate, string? because = null, params object?[] becauseArgs) {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_CONTAIN_ONLY_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        var offender = _value.Where(item => !predicate(item)).ToList();
        if(offender.Count != 0) {
            Fail(Strings.Format(StringKeys.COLLECTION_CONTAIN_ONLY_OFFENDER, Expression, Format(offender[0])), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>
    /// Asserts that the collection contains exactly one item (optionally matching <paramref name="predicate"/>)
    /// and returns a <see cref="WhichConstraint{T}"/> that exposes the single item for further assertions.
    /// </summary>
    /// <remarks>
    /// This assertion returns a value rather than an <see cref="AndConstraint{TParent}"/>, so it throws
    /// immediately even inside an <see cref="AssertionScope"/> instead of being batched.
    /// </remarks>
    /// <param name="predicate">
    /// An optional filter; when provided, only items satisfying this predicate are considered.
    /// When <see langword="null"/>, all items in the collection are evaluated.
    /// </param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the single matched item.</returns>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or does not contain exactly one matching item.</exception>
    public WhichConstraint<T> ContainSingle(Func<T, bool>? predicate = null, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.COLLECTION_CONTAIN_SINGLE_NULL, Expression), because, becauseArgs));
        }

        List<T> items = predicate is not null ? [.. _value.Where(predicate)] : [.. _value];
        if(items.Count != 1) {
            throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.COLLECTION_CONTAIN_SINGLE, Expression, Format(items.Count)), because, becauseArgs));
        }

        return new WhichConstraint<T>(items[0]);
    }

    /// <summary>Asserts that no item in the collection matches the given <paramref name="predicate"/>.</summary>
    /// <param name="predicate">A function to test each element against.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or at least one item matches <paramref name="predicate"/>.</exception>
    public AndConstraint<CollectionAssertions<T>> NotContain(Func<T, bool> predicate, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_CONTAIN_PREDICATE_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        if(_value.Any(predicate)) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_CONTAIN_PREDICATE_FOUND, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the collection does not contain the specified <paramref name="item"/>.</summary>
    /// <param name="item">The item that must not appear in the collection.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or contains <paramref name="item"/>.</exception>
    public AndConstraint<CollectionAssertions<T>> NotContain(T item, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_CONTAIN_ITEM_NULL, Expression, Format(item)), because, becauseArgs);
            return new(this);
        }

        if(_value.Contains(item)) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_CONTAIN_ITEM_FOUND, Expression, Format(item)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the collection contains all items in <paramref name="expected"/> (cardinality need not match).</summary>
    /// <param name="expected">The subset of items that must all be present in the collection.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or one or more expected items are missing.</exception>
    public AndConstraint<CollectionAssertions<T>> ContainSubset(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_SUBSET_CHECK_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var list = _value.ToList();
        var missing = expected.Where(item => !list.Contains(item)).ToList();
        if(missing.Count != 0) {
            Fail(Strings.Format(StringKeys.COLLECTION_SUBSET_MISSING, Expression, Format(missing[0])), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the item at <paramref name="index"/> equals <paramref name="expected"/>.</summary>
    /// <param name="index">The zero-based position to inspect.</param>
    /// <param name="expected">The expected item at that position.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/>, too short, or holds a different item at <paramref name="index"/>.</exception>
    public AndConstraint<CollectionAssertions<T>> HaveElementAt(int index, T expected, string? because = null, params object?[] becauseArgs) {
        if(index < 0) {
            throw new ArgumentOutOfRangeException(nameof(index), "Index must not be negative.");
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_ELEMENT_AT_NULL, Expression, Format(expected), Format(index)), because, becauseArgs);
            return new(this);
        }

        var list = _value.ToList();
        if(index >= list.Count) {
            Fail(Strings.Format(StringKeys.COLLECTION_ELEMENT_AT_SHORT, Expression, Format(expected), Format(index), Format(list.Count)), because, becauseArgs);
            return new(this);
        }

        if(!EqualityComparer<T>.Default.Equals(list[index], expected)) {
            Fail(Strings.Format(StringKeys.COLLECTION_ELEMENT_AT_MISMATCH, Expression, Format(expected), Format(index), Format(list[index])), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that at least one element is assignable to type <typeparamref name="TExpectation"/>.</summary>`r`n    /// <typeparam name="TExpectation">The type at least one element must be assignable to.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">No element is assignable to <typeparamref name="TExpectation"/>.</exception>
    public AndConstraint<CollectionAssertions<T>> ContainItemsAssignableTo<TExpectation>(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_CONTAIN_ASSIGNABLE_NULL, Expression, typeof(TExpectation).Name), because, becauseArgs);
            return new(this);
        }

        if(_value.Any(static item => item is TExpectation)) {
            return new(this);
        }

        Fail(Strings.Format(StringKeys.COLLECTION_CONTAIN_ASSIGNABLE, Expression, typeof(TExpectation).Name), because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that no element is assignable to type <typeparamref name="TExpectation"/>.</summary>`r`n    /// <typeparam name="TExpectation">The type no element may be assignable to.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">An element is assignable to <typeparamref name="TExpectation"/>.</exception>
    public AndConstraint<CollectionAssertions<T>> NotContainItemsAssignableTo<TExpectation>(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_CONTAIN_ASSIGNABLE_NULL, Expression, typeof(TExpectation).Name), because, becauseArgs);
            return new(this);
        }

        var index = 0;
        foreach(var item in _value) {
            if(item is TExpectation) {
                Fail(Strings.Format(StringKeys.COLLECTION_NOT_CONTAIN_ASSIGNABLE, Expression, typeof(TExpectation).Name, Format(item), AssertionScope.FormatInvariant(index)), because, becauseArgs);
                return new(this);
            }

            index++;
        }

        return new(this);
    }

    /// <summary>Asserts that <paramref name="expectation"/> occurs immediately before <paramref name="successor"/>.</summary>
    /// <param name="successor">The element that must directly follow <paramref name="expectation"/>.</param>
    /// <param name="expectation">The element that must directly precede <paramref name="successor"/>.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">No adjacent <paramref name="expectation"/>/<paramref name="successor"/> pair exists.</exception>
    public AndConstraint<CollectionAssertions<T>> HaveElementPreceding(T successor, T expectation, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_HAVE_PRECEDING_NULL, Expression, Format(expectation), Format(successor)), because, becauseArgs);
            return new(this);
        }

        var list = _value.ToList();
        for(var index = 0; index + 1 < list.Count; index++) {
            if(EqualityComparer<T>.Default.Equals(list[index], expectation) && EqualityComparer<T>.Default.Equals(list[index + 1], successor)) {
                return new(this);
            }
        }

        Fail(Strings.Format(StringKeys.COLLECTION_HAVE_PRECEDING, Expression, Format(expectation), Format(successor)), because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that <paramref name="expectation"/> occurs immediately after <paramref name="predecessor"/>.</summary>
    /// <param name="predecessor">The element that must directly precede <paramref name="expectation"/>.</param>
    /// <param name="expectation">The element that must directly follow <paramref name="predecessor"/>.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">No adjacent <paramref name="predecessor"/>/<paramref name="expectation"/> pair exists.</exception>
    public AndConstraint<CollectionAssertions<T>> HaveElementSucceeding(T predecessor, T expectation, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_HAVE_SUCCEEDING_NULL, Expression, Format(expectation), Format(predecessor)), because, becauseArgs);
            return new(this);
        }

        var list = _value.ToList();
        for(var index = 0; index + 1 < list.Count; index++) {
            if(EqualityComparer<T>.Default.Equals(list[index], predecessor) && EqualityComparer<T>.Default.Equals(list[index + 1], expectation)) {
                return new(this);
            }
        }

        Fail(Strings.Format(StringKeys.COLLECTION_HAVE_SUCCEEDING, Expression, Format(expectation), Format(predecessor)), because, becauseArgs);
        return new(this);
    }
}
