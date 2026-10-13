//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class SpanAssertions<T> {
    /// <summary>Asserts that the span starts with the items in <paramref name="expected"/> (in order).</summary>
    /// <param name="expected">The expected leading items.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span is too short or differs at the start.</exception>
    public AndConstraint<SpanAssertions<T>> StartWith(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) {
        _inner.StartWith(expected, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span starts with the items in <paramref name="expected"/> (in order).</summary>
    /// <param name="expected">The expected leading items.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The span is too short or differs at the start.</exception>
    public AndConstraint<SpanAssertions<T>> StartWith(params T[] expected) {
        _inner.StartWith(expected);
        return new(this);
    }

    /// <summary>Asserts that the span ends with the items in <paramref name="expected"/> (in order).</summary>
    /// <param name="expected">The expected trailing items.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span is too short or differs at the end.</exception>
    public AndConstraint<SpanAssertions<T>> EndWith(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) {
        _inner.EndWith(expected, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span ends with the items in <paramref name="expected"/> (in order).</summary>
    /// <param name="expected">The expected trailing items.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The span is too short or differs at the end.</exception>
    public AndConstraint<SpanAssertions<T>> EndWith(params T[] expected) {
        _inner.EndWith(expected);
        return new(this);
    }

    /// <summary>Asserts that the span starts with <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected first item.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The span is empty or starts with another item.</exception>
    public AndConstraint<SpanAssertions<T>> StartWith(T expected, string? because = null, params object?[] becauseArgs) {
        _inner.StartWith(expected, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span ends with <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected last item.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The span is empty or ends with another item.</exception>
    public AndConstraint<SpanAssertions<T>> EndWith(T expected, string? because = null, params object?[] becauseArgs) {
        _inner.EndWith(expected, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span starts with the items in <paramref name="expected"/>, comparing element-wise.</summary>
    /// <typeparam name="TExpectation">The expected element type.</typeparam>
    /// <param name="expected">The expected leading items.</param>
    /// <param name="equality">The element comparer.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> or <paramref name="equality"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span is too short or differs at the start.</exception>
    public AndConstraint<SpanAssertions<T>> StartWith<TExpectation>(IEnumerable<TExpectation> expected, Func<T, TExpectation, bool> equality, string? because = null, params object?[] becauseArgs) {
        _inner.StartWith(expected, equality, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span ends with the items in <paramref name="expected"/>, comparing element-wise.</summary>
    /// <typeparam name="TExpectation">The expected element type.</typeparam>
    /// <param name="expected">The expected trailing items.</param>
    /// <param name="equality">The element comparer.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> or <paramref name="equality"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span is too short or differs at the end.</exception>
    public AndConstraint<SpanAssertions<T>> EndWith<TExpectation>(IEnumerable<TExpectation> expected, Func<T, TExpectation, bool> equality, string? because = null, params object?[] becauseArgs) {
        _inner.EndWith(expected, equality, because, becauseArgs);
        return new(this);
    }
}
