//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class SpanAssertions<T> {
    /// <summary>Asserts that the span contains exactly the items in <paramref name="expected"/>, ignoring order.</summary>
    /// <param name="expected">The expected items.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The contents do not match.</exception>
    public AndConstraint<SpanAssertions<T>> BeEquivalentTo(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) {
        _inner.BeEquivalentTo(expected, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span contains exactly the items in <paramref name="expected"/>, ignoring order.</summary>
    /// <param name="expected">The expected items.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The contents do not match.</exception>
    public AndConstraint<SpanAssertions<T>> BeEquivalentTo(params T[] expected) {
        _inner.BeEquivalentTo(expected);
        return new(this);
    }

    /// <summary>Asserts that the span contains exactly the items in <paramref name="expected"/> in the same order.</summary>
    /// <param name="expected">The expected items in order.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span differs in content or order.</exception>
    public AndConstraint<SpanAssertions<T>> Equal(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) {
        _inner.Equal(expected, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span contains exactly the items in <paramref name="expected"/> in the same order.</summary>
    /// <param name="expected">The expected items in order.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The span differs in content or order.</exception>
    public AndConstraint<SpanAssertions<T>> Equal(params T[] expected) {
        _inner.Equal(expected);
        return new(this);
    }

    /// <summary>Asserts that the span contains exactly the items in <paramref name="expected"/> in the same order (readability alias).</summary>
    /// <param name="expected">The expected items in order.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span differs in content or order.</exception>
    public AndConstraint<SpanAssertions<T>> BeEqualTo(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) {
        _inner.BeEqualTo(expected, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span contains exactly the items in <paramref name="expected"/> in the same order (readability alias).</summary>
    /// <param name="expected">The expected items in order.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span differs in content or order.</exception>
    public AndConstraint<SpanAssertions<T>> BeEqualTo(T[] expected, string? because = null, params object?[] becauseArgs) {
        _inner.BeEqualTo(expected, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span contains exactly the items in <paramref name="expected"/> in the same order, comparing element-wise.</summary>
    /// <typeparam name="TExpectation">The expected element type.</typeparam>
    /// <param name="expected">The expected items in order.</param>
    /// <param name="equality">The element comparer.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> or <paramref name="equality"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span differs in content or order.</exception>
    public AndConstraint<SpanAssertions<T>> BeEqualTo<TExpectation>(IEnumerable<TExpectation> expected, Func<T, TExpectation, bool> equality, string? because = null, params object?[] becauseArgs) {
        _inner.BeEqualTo(expected, equality, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span does not contain exactly the items in <paramref name="expected"/> in the same order.</summary>
    /// <param name="expected">The items that must differ in content or order.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span equals <paramref name="expected"/> in content and order.</exception>
    public AndConstraint<SpanAssertions<T>> NotBeEqualTo(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) {
        _inner.NotBeEqualTo(expected, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span does not contain exactly the items in <paramref name="expected"/> in the same order.</summary>
    /// <param name="expected">The items that must differ in content or order.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span equals <paramref name="expected"/> in content and order.</exception>
    public AndConstraint<SpanAssertions<T>> NotEqual(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) {
        _inner.NotEqual(expected, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that every span element is structurally equivalent to <paramref name="expectation"/>.</summary>
    /// <param name="expectation">The single expectation every element must match.</param>
    /// <param name="config">Optional equivalency configuration.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">Any element is not equivalent.</exception>
    public AndConstraint<SpanAssertions<T>> AllBeEquivalentTo(
        T expectation,
        Func<CollectionEquivalencyOptions<T>, CollectionEquivalencyOptions<T>>? config = null,
        string? because = null,
        params object?[] becauseArgs) {
        _inner.AllBeEquivalentTo(expectation, config, because, becauseArgs);
        return new(this);
    }
}
