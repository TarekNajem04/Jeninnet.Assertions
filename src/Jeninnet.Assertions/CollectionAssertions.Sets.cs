//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class CollectionAssertions<T> {
    /// <summary>Asserts that every item in the collection also appears in <paramref name="expected"/>.</summary>
    /// <param name="expected">The superset that must contain every item.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or an item is missing from the superset.</exception>
    public AndConstraint<CollectionAssertions<T>> BeSubsetOf(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var superset = expected.ToList();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_SUBSET_NULL, Expression, JoinItems(superset)), because, becauseArgs);
            return new(this);
        }

        var outsider = _value.Where(item => !superset.Contains(item)).ToList();
        if(outsider.Count != 0) {
            Fail(Strings.Format(StringKeys.COLLECTION_SUBSET_OUTSIDER, Expression, JoinItems(superset), Format(outsider[0])), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the collection shares at least one item with <paramref name="expected"/>.</summary>
    /// <param name="expected">The collection to intersect with.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or shares no items.</exception>
    public AndConstraint<CollectionAssertions<T>> IntersectWith(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var peers = expected.ToList();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_INTERSECT_NULL, Expression, JoinItems(peers)), because, becauseArgs);
            return new(this);
        }

        if(!_value.Any(peers.Contains)) {
            Fail(Strings.Format(StringKeys.COLLECTION_INTERSECT_EMPTY, Expression, JoinItems(peers)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the collection shares no items with <paramref name="expected"/>.</summary>
    /// <param name="expected">The collection that must be disjoint from the subject.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or shares at least one item.</exception>
    public AndConstraint<CollectionAssertions<T>> NotIntersectWith(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var peers = expected.ToList();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_INTERSECT_NULL, Expression, JoinItems(peers)), because, becauseArgs);
            return new(this);
        }

        var shared = _value.Where(peers.Contains).ToList();
        if(shared.Count != 0) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_INTERSECT_SHARED, Expression, JoinItems(peers), Format(shared[0])), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the collection is not a subset of the specified collection.</summary>
    /// <param name="unexpected">The collection that must not be a superset.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="unexpected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is a subset of the specified collection.</exception>
    public AndConstraint<CollectionAssertions<T>> NotBeSubsetOf(IEnumerable<T> unexpected, string? because = null, params object?[] becauseArgs) {
        if(unexpected is null) {
            throw new ArgumentNullException(nameof(unexpected));
        }

        var superset = unexpected.ToList();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_BE_SUBSET_NULL, Expression, JoinItems(superset)), because, becauseArgs);
            return new(this);
        }

        var outsider = _value.Where(item => !superset.Contains(item)).ToList();
        if(outsider.Count == 0) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_BE_SUBSET_OUTSIDER, Expression, JoinItems(superset), Format(_value.First())), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the collection contains every item of <paramref name="expected"/> (which may be empty).</summary>
    /// <param name="expected">The subset that must be contained.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or misses a subset item.</exception>
    public AndConstraint<CollectionAssertions<T>> BeSupersetOf(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var subset = expected.ToList();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_BE_SUPERSET_NULL, Expression, JoinItems(subset)), because, becauseArgs);
            return new(this);
        }

        var actualList = _value.ToList();
        var missing = subset.Where(item => !actualList.Contains(item)).ToList();
        if(missing.Count != 0) {
            Fail(Strings.Format(StringKeys.COLLECTION_BE_SUPERSET, Expression, JoinItems(subset), Format(missing[0])), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that every item of the collection is in <paramref name="expected"/>, which holds at least one item the collection lacks.</summary>
    /// <param name="expected">The strict superset.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/>, has an outsider, or matches the superset exactly.</exception>
    public AndConstraint<CollectionAssertions<T>> BeProperSubsetOf(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var superset = expected.ToList();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_BE_PROPER_SUBSET_NULL, Expression, JoinItems(superset)), because, becauseArgs);
            return new(this);
        }

        var actualList = _value.ToList();
        var outsider = actualList.Where(item => !superset.Contains(item)).ToList();
        if(outsider.Count != 0) {
            Fail(Strings.Format(StringKeys.COLLECTION_SUBSET_OUTSIDER, Expression, JoinItems(superset), Format(outsider[0])), because, becauseArgs);
            return new(this);
        }

        if(superset.All(actualList.Contains)) {
            Fail(Strings.Format(StringKeys.COLLECTION_BE_PROPER_SUBSET_SAME, Expression, JoinItems(superset)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the collection contains every item of <paramref name="expected"/> plus at least one item it lacks (an empty <paramref name="expected"/> passes only for a non-empty collection).</summary>
    /// <param name="expected">The strict subset that must be contained.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/>, misses a subset item, or matches the subset exactly.</exception>
    public AndConstraint<CollectionAssertions<T>> BeProperSupersetOf(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var subset = expected.ToList();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_BE_PROPER_SUPERSET_NULL, Expression, JoinItems(subset)), because, becauseArgs);
            return new(this);
        }

        var actualList = _value.ToList();
        var missing = subset.Where(item => !actualList.Contains(item)).ToList();
        if(missing.Count != 0) {
            Fail(Strings.Format(StringKeys.COLLECTION_BE_SUPERSET, Expression, JoinItems(subset), Format(missing[0])), because, becauseArgs);
            return new(this);
        }

        if(actualList.All(subset.Contains)) {
            Fail(Strings.Format(StringKeys.COLLECTION_BE_PROPER_SUPERSET_SAME, Expression, JoinItems(subset)), because, becauseArgs);
        }

        return new(this);
    }
}
