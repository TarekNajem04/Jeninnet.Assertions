//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class SpanAssertions<T> {
    /// <summary>Asserts that the span contains the items in <paramref name="expected"/> in order.</summary>
    /// <param name="expected">The expected items in order.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The sequence is not found in order.</exception>
    public AndConstraint<SpanAssertions<T>> ContainInOrder(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) {
        _inner.ContainInOrder(expected, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span does not contain the items in <paramref name="unexpected"/> in order.</summary>
    /// <param name="unexpected">The items that must not be found in order.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="unexpected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The sequence is found in order.</exception>
    public AndConstraint<SpanAssertions<T>> NotContainInOrder(IEnumerable<T> unexpected, string? because = null, params object?[] becauseArgs) {
        _inner.NotContainInOrder(unexpected, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span contains the items in <paramref name="expected"/> as one consecutive run.</summary>
    /// <param name="expected">The expected consecutive sequence.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The consecutive sequence is not found.</exception>
    public AndConstraint<SpanAssertions<T>> ContainInConsecutiveOrder(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) {
        _inner.ContainInConsecutiveOrder(expected, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span does not contain the items in <paramref name="unexpected"/> as one consecutive run.</summary>
    /// <param name="unexpected">The sequence that must not appear consecutively.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="unexpected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The consecutive sequence is found.</exception>
    public AndConstraint<SpanAssertions<T>> NotContainInConsecutiveOrder(IEnumerable<T> unexpected, string? because = null, params object?[] becauseArgs) {
        _inner.NotContainInConsecutiveOrder(unexpected, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span is in ascending order using the default comparer.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="AssertionFailedException">The span is not in ascending order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<T>> BeInAscendingOrder(string? because = null, params object?[] becauseArgs) =>
        _inner.BeInAscendingOrder(because, becauseArgs);

    /// <summary>Asserts that the span is in ascending order using <paramref name="comparer"/>.</summary>
    /// <param name="comparer">The comparer defining the order.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span is not in ascending order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<T>> BeInAscendingOrder(IComparer<T> comparer, string? because = null, params object?[] becauseArgs) =>
        _inner.BeInAscendingOrder(comparer, because, becauseArgs);

    /// <summary>Asserts that the span is in ascending order of <paramref name="comparison"/>.</summary>
    /// <param name="comparison">The comparison function (negative when the first item precedes the second).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="comparison"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span is not in ascending order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<T>> BeInAscendingOrder(Func<T, T, int> comparison, string? because = null, params object?[] becauseArgs) =>
        _inner.BeInAscendingOrder(comparison, because, becauseArgs);

    /// <summary>Asserts that the span is in ascending order of the key selected by <paramref name="selector"/>.</summary>
    /// <typeparam name="TSelector">The selected key type.</typeparam>
    /// <param name="selector">The key selecting expression.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span is not in ascending key order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<T>> BeInAscendingOrder<TSelector>(Expression<Func<T, TSelector>> selector, string? because = null, params object?[] becauseArgs) =>
        _inner.BeInAscendingOrder(selector, because, becauseArgs);

    /// <summary>Asserts that the span is in ascending order of the key selected by <paramref name="selector"/> using <paramref name="comparer"/>.</summary>
    /// <typeparam name="TSelector">The selected key type.</typeparam>
    /// <param name="selector">The key selecting expression.</param>
    /// <param name="comparer">The comparer defining the key order, or <see langword="null"/> for the default comparer.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span is not in ascending key order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<T>> BeInAscendingOrder<TSelector>(Expression<Func<T, TSelector>> selector, IComparer<TSelector>? comparer, string? because = null, params object?[] becauseArgs) =>
        _inner.BeInAscendingOrder(selector, comparer, because, becauseArgs);

    /// <summary>Asserts that the span is in descending order using the default comparer.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="AssertionFailedException">The span is not in descending order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<T>> BeInDescendingOrder(string? because = null, params object?[] becauseArgs) =>
        _inner.BeInDescendingOrder(because, becauseArgs);

    /// <summary>Asserts that the span is in descending order using <paramref name="comparer"/>.</summary>
    /// <param name="comparer">The comparer defining the order.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span is not in descending order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<T>> BeInDescendingOrder(IComparer<T> comparer, string? because = null, params object?[] becauseArgs) =>
        _inner.BeInDescendingOrder(comparer, because, becauseArgs);

    /// <summary>Asserts that the span is in descending order of <paramref name="comparison"/>.</summary>
    /// <param name="comparison">The comparison function (negative when the first item precedes the second).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="comparison"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span is not in descending order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<T>> BeInDescendingOrder(Func<T, T, int> comparison, string? because = null, params object?[] becauseArgs) =>
        _inner.BeInDescendingOrder(comparison, because, becauseArgs);

    /// <summary>Asserts that the span is in descending order of the key selected by <paramref name="selector"/>.</summary>
    /// <typeparam name="TSelector">The selected key type.</typeparam>
    /// <param name="selector">The key selecting expression.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span is not in descending key order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<T>> BeInDescendingOrder<TSelector>(Expression<Func<T, TSelector>> selector, string? because = null, params object?[] becauseArgs) =>
        _inner.BeInDescendingOrder(selector, because, becauseArgs);

    /// <summary>Asserts that the span is in descending order of the key selected by <paramref name="selector"/> using <paramref name="comparer"/>.</summary>
    /// <typeparam name="TSelector">The selected key type.</typeparam>
    /// <param name="selector">The key selecting expression.</param>
    /// <param name="comparer">The comparer defining the key order, or <see langword="null"/> for the default comparer.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span is not in descending key order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<T>> BeInDescendingOrder<TSelector>(Expression<Func<T, TSelector>> selector, IComparer<TSelector>? comparer, string? because = null, params object?[] becauseArgs) =>
        _inner.BeInDescendingOrder(selector, comparer, because, becauseArgs);

    /// <summary>Asserts that the span is not in ascending order of <paramref name="comparison"/>.</summary>
    /// <param name="comparison">The comparison function (negative when the first item precedes the second).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="comparison"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span is in ascending order.</exception>
    public AndConstraint<SpanAssertions<T>> NotBeInAscendingOrder(Func<T, T, int> comparison, string? because = null, params object?[] becauseArgs) {
        _inner.NotBeInAscendingOrder(comparison, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span is not in ascending order of the key selected by <paramref name="selector"/>.</summary>
    /// <typeparam name="TSelector">The selected key type.</typeparam>
    /// <param name="selector">The key selecting expression.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span is in ascending key order.</exception>
    public AndConstraint<SpanAssertions<T>> NotBeInAscendingOrder<TSelector>(Expression<Func<T, TSelector>> selector, string? because = null, params object?[] becauseArgs) {
        _inner.NotBeInAscendingOrder(selector, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span is not in ascending order of the key selected by <paramref name="selector"/> using <paramref name="comparer"/>.</summary>
    /// <typeparam name="TSelector">The selected key type.</typeparam>
    /// <param name="selector">The key selecting expression.</param>
    /// <param name="comparer">The comparer defining the key order, or <see langword="null"/> for the default comparer.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span is in ascending key order.</exception>
    public AndConstraint<SpanAssertions<T>> NotBeInAscendingOrder<TSelector>(Expression<Func<T, TSelector>> selector, IComparer<TSelector>? comparer, string? because = null, params object?[] becauseArgs) {
        _inner.NotBeInAscendingOrder(selector, comparer, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span is not in descending order of <paramref name="comparison"/>.</summary>
    /// <param name="comparison">The comparison function (negative when the first item precedes the second).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="comparison"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span is in descending order.</exception>
    public AndConstraint<SpanAssertions<T>> NotBeInDescendingOrder(Func<T, T, int> comparison, string? because = null, params object?[] becauseArgs) {
        _inner.NotBeInDescendingOrder(comparison, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span is not in descending order of the key selected by <paramref name="selector"/>.</summary>
    /// <typeparam name="TSelector">The selected key type.</typeparam>
    /// <param name="selector">The key selecting expression.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span is in descending key order.</exception>
    public AndConstraint<SpanAssertions<T>> NotBeInDescendingOrder<TSelector>(Expression<Func<T, TSelector>> selector, string? because = null, params object?[] becauseArgs) {
        _inner.NotBeInDescendingOrder(selector, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span is not in descending order of the key selected by <paramref name="selector"/> using <paramref name="comparer"/>.</summary>
    /// <typeparam name="TSelector">The selected key type.</typeparam>
    /// <param name="selector">The key selecting expression.</param>
    /// <param name="comparer">The comparer defining the key order, or <see langword="null"/> for the default comparer.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span is in descending key order.</exception>
    public AndConstraint<SpanAssertions<T>> NotBeInDescendingOrder<TSelector>(Expression<Func<T, TSelector>> selector, IComparer<TSelector>? comparer, string? because = null, params object?[] becauseArgs) {
        _inner.NotBeInDescendingOrder(selector, comparer, because, becauseArgs);
        return new(this);
    }
}
