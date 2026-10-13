//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying <see cref="Span{T}"/> and <see cref="ReadOnlySpan{T}"/> values.</summary>
/// <remarks>
/// The span is snapshotted when assertions begin, so later mutations of the backing memory
/// do not affect the outcome. Enter through <c>AsAssertions()</c> (a dedicated entry point is used
/// because a <c>Should()</c> overload would be ambiguous with <c>Should(IEnumerable&lt;T&gt;)</c> for arrays).
/// </remarks>
/// <typeparam name="T">The element type of the span.</typeparam>
public partial class SpanAssertions<T> : AssertionsBase {
    private readonly CollectionAssertions<T> _inner;

    private readonly List<T> _snapshot;

    /// <summary>Creates span assertions over a snapshot of <paramref name="value"/>.</summary>
    /// <param name="value">The span to assert on (snapshotted).</param>
    /// <param name="expression">The caller expression naming the subject under test.</param>
    public SpanAssertions(ReadOnlySpan<T> value, string? expression = null) : base(expression, "span") {
        _snapshot = [.. value];
        _inner = new CollectionAssertions<T>(_snapshot, Expression);
    }

    /// <summary>Gets the snapshotted elements (for extensions in this assembly).</summary>
    internal IReadOnlyList<T> Snapshot => _snapshot;

    /// <summary>Asserts that the span contains exactly <paramref name="expected"/> items.</summary>
    /// <param name="expected">The expected item count.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The item count does not match.</exception>
    public AndConstraint<SpanAssertions<T>> HaveCount(int expected, string? because = null, params object?[] becauseArgs) {
        _inner.HaveCount(expected, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span is empty.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The span contains at least one item.</exception>
    public AndConstraint<SpanAssertions<T>> BeEmpty(string? because = null, params object?[] becauseArgs) {
        _inner.BeEmpty(because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span is not empty.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The span is empty.</exception>
    public AndConstraint<SpanAssertions<T>> NotBeEmpty(string? because = null, params object?[] becauseArgs) {
        _inner.NotBeEmpty(because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span has the same number of items as <paramref name="expected"/>.</summary>
    /// <typeparam name="TOther">The expected element type.</typeparam>
    /// <param name="expected">The collection to compare counts with.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The counts differ.</exception>
    public AndConstraint<SpanAssertions<T>> HaveSameCount<TOther>(IEnumerable<TOther> expected, string? because = null, params object?[] becauseArgs) {
        _inner.HaveSameCount(expected, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span does not have the same number of items as <paramref name="other"/>.</summary>
    /// <typeparam name="TOther">The other element type.</typeparam>
    /// <param name="other">The collection to compare counts with.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The counts match.</exception>
    public AndConstraint<SpanAssertions<T>> NotHaveSameCount<TOther>(IEnumerable<TOther> other, string? because = null, params object?[] becauseArgs) {
        _inner.NotHaveSameCount(other, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span does not contain exactly the specified number of items.</summary>
    /// <param name="unexpected">The item count that must not match.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The span contains exactly the specified number of items.</exception>
    public AndConstraint<SpanAssertions<T>> NotHaveCount(int unexpected, string? because = null, params object?[] becauseArgs) {
        _inner.NotHaveCount(unexpected, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span contains more than <paramref name="expected"/> items.</summary>
    /// <param name="expected">The exclusive lower bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The span has too few items.</exception>
    public AndConstraint<SpanAssertions<T>> HaveCountGreaterThan(int expected, string? because = null, params object?[] becauseArgs) {
        _inner.HaveCountGreaterThan(expected, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span contains at least <paramref name="expected"/> items.</summary>
    /// <param name="expected">The inclusive lower bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The span has too few items.</exception>
    public AndConstraint<SpanAssertions<T>> HaveCountGreaterThanOrEqualTo(int expected, string? because = null, params object?[] becauseArgs) {
        _inner.HaveCountGreaterThanOrEqualTo(expected, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span contains fewer than <paramref name="expected"/> items.</summary>
    /// <param name="expected">The exclusive upper bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The span has too many items.</exception>
    public AndConstraint<SpanAssertions<T>> HaveCountLessThan(int expected, string? because = null, params object?[] becauseArgs) {
        _inner.HaveCountLessThan(expected, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span contains at most <paramref name="expected"/> items.</summary>
    /// <param name="expected">The inclusive upper bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The span has too many items.</exception>
    public AndConstraint<SpanAssertions<T>> HaveCountLessThanOrEqualTo(int expected, string? because = null, params object?[] becauseArgs) {
        _inner.HaveCountLessThanOrEqualTo(expected, because, becauseArgs);
        return new(this);
    }
}
