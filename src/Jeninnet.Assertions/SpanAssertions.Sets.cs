//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class SpanAssertions<T> {
    /// <summary>Asserts that every item in the span also appears in <paramref name="expected"/>.</summary>
    /// <param name="expected">The superset that must contain every item.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">An item is missing from the superset.</exception>
    public AndConstraint<SpanAssertions<T>> BeSubsetOf(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) {
        _inner.BeSubsetOf(expected, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span is not a subset of the specified items.</summary>
    /// <param name="unexpected">The items that must not be a superset.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="unexpected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span is a subset of the specified items.</exception>
    public AndConstraint<SpanAssertions<T>> NotBeSubsetOf(IEnumerable<T> unexpected, string? because = null, params object?[] becauseArgs) {
        _inner.NotBeSubsetOf(unexpected, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span contains every item of <paramref name="expected"/>.</summary>
    /// <param name="expected">The subset that must be contained.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span misses a subset item.</exception>
    public AndConstraint<SpanAssertions<T>> BeSupersetOf(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) {
        _inner.BeSupersetOf(expected, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that every item in the span is in <paramref name="expected"/>, which holds at least one item the span lacks.</summary>
    /// <param name="expected">The strict superset.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span has an outsider or matches the superset exactly.</exception>
    public AndConstraint<SpanAssertions<T>> BeProperSubsetOf(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) {
        _inner.BeProperSubsetOf(expected, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span contains every item of <paramref name="expected"/> plus at least one item it lacks.</summary>
    /// <param name="expected">The strict subset that must be contained.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span misses a subset item or matches the subset exactly.</exception>
    public AndConstraint<SpanAssertions<T>> BeProperSupersetOf(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) {
        _inner.BeProperSupersetOf(expected, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span shares at least one item with <paramref name="expected"/>.</summary>
    /// <param name="expected">The collection to intersect with.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span shares no items.</exception>
    public AndConstraint<SpanAssertions<T>> IntersectWith(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) {
        _inner.IntersectWith(expected, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span shares no items with <paramref name="expected"/>.</summary>
    /// <param name="expected">The collection that must stay disjoint.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span shares items.</exception>
    public AndConstraint<SpanAssertions<T>> NotIntersectWith(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) {
        _inner.NotIntersectWith(expected, because, becauseArgs);
        return new(this);
    }
}
