//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class SpanAssertions<T> {
    /// <summary>Asserts that the span contains the specified <paramref name="item"/>.</summary>
    /// <param name="item">The item to locate in the span.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The span does not contain <paramref name="item"/>.</exception>
    public AndConstraint<SpanAssertions<T>> Contain(T item, string? because = null, params object?[] becauseArgs) {
        _inner.Contain(item, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span contains at least one item matching <paramref name="predicate"/>.</summary>
    /// <param name="predicate">A function to test each element against.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">No item matches <paramref name="predicate"/>.</exception>
    public AndConstraint<SpanAssertions<T>> Contain(Func<T, bool> predicate, string? because = null, params object?[] becauseArgs) {
        _inner.Contain(predicate, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span does not contain the specified <paramref name="item"/>.</summary>
    /// <param name="item">The item that must not appear in the span.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The span contains <paramref name="item"/>.</exception>
    public AndConstraint<SpanAssertions<T>> NotContain(T item, string? because = null, params object?[] becauseArgs) {
        _inner.NotContain(item, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that every item in the span matches <paramref name="predicate"/>.</summary>
    /// <param name="predicate">A function every element must satisfy.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">An item does not match.</exception>
    public AndConstraint<SpanAssertions<T>> OnlyContain(Func<T, bool> predicate, string? because = null, params object?[] becauseArgs) {
        _inner.OnlyContain(predicate, because, becauseArgs);
        return new(this);
    }

    /// <summary>
    /// Asserts that the span contains exactly one item (optionally matching <paramref name="predicate"/>)
    /// and returns a <see cref="WhichConstraint{T}"/> exposing it for further assertions.
    /// </summary>
    /// <param name="predicate">An optional filter over the items.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the single matched item.</returns>
    /// <exception cref="AssertionFailedException">The span does not contain exactly one matching item.</exception>
    public WhichConstraint<T> ContainSingle(Func<T, bool>? predicate = null, string? because = null, params object?[] becauseArgs) =>
        _inner.ContainSingle(predicate, because, becauseArgs);

    /// <summary>Asserts that the span contains an item structurally equivalent to <paramref name="expectation"/>.</summary>
    /// <param name="expectation">The expected object graph.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The span holds no equivalent item.</exception>
    public AndConstraint<SpanAssertions<T>> ContainEquivalentOf(object? expectation, string? because = null, params object?[] becauseArgs) {
        _inner.ContainEquivalentOf(expectation, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span holds no item structurally equivalent to <paramref name="expectation"/>.</summary>
    /// <param name="expectation">The unexpected object graph.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The span holds an equivalent item.</exception>
    public AndConstraint<SpanAssertions<T>> NotContainEquivalentOf(object? expectation, string? because = null, params object?[] becauseArgs) {
        _inner.NotContainEquivalentOf(expectation, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the item at <paramref name="index"/> equals <paramref name="expected"/>.</summary>
    /// <param name="index">The zero-based index.</param>
    /// <param name="expected">The expected item.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The span is too short or the item differs.</exception>
    public AndConstraint<SpanAssertions<T>> HaveElementAt(int index, T expected, string? because = null, params object?[] becauseArgs) {
        _inner.HaveElementAt(index, expected, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that <paramref name="successor"/> immediately follows <paramref name="predecessor"/> in the span.</summary>
    /// <param name="predecessor">The leading item.</param>
    /// <param name="successor">The item that must follow it.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">No such adjacent pair exists.</exception>
    public AndConstraint<SpanAssertions<T>> HaveElementSucceeding(T predecessor, T successor, string? because = null, params object?[] becauseArgs) {
        _inner.HaveElementSucceeding(predecessor, successor, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that <paramref name="expectation"/> immediately precedes <paramref name="successor"/> in the span.</summary>
    /// <param name="successor">The trailing item.</param>
    /// <param name="expectation">The item that must come first.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">No such adjacent pair exists.</exception>
    public AndConstraint<SpanAssertions<T>> HaveElementPreceding(T successor, T expectation, string? because = null, params object?[] becauseArgs) {
        _inner.HaveElementPreceding(successor, expectation, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span contains at least one item assignable to type <typeparamref name="TExpectation"/>.</summary>
    /// <typeparam name="TExpectation">The type at least one item must be assignable to.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">No item is assignable.</exception>
    public AndConstraint<SpanAssertions<T>> ContainItemsAssignableTo<TExpectation>(string? because = null, params object?[] becauseArgs) {
        _inner.ContainItemsAssignableTo<TExpectation>(because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the span contains no item assignable to type <typeparamref name="TExpectation"/>.</summary>
    /// <typeparam name="TExpectation">The type no item may be assignable to.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">An item is assignable.</exception>
    public AndConstraint<SpanAssertions<T>> NotContainItemsAssignableTo<TExpectation>(string? because = null, params object?[] becauseArgs) {
        _inner.NotContainItemsAssignableTo<TExpectation>(because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that no span item is <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The span contains a null item.</exception>
    public AndConstraint<SpanAssertions<T>> NotContainNulls(string? because = null, params object?[] becauseArgs) {
        _inner.NotContainNulls(because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that no projected key (via <paramref name="selector"/>) is <see langword="null"/>.</summary>
    /// <typeparam name="TKey">The projected key type.</typeparam>
    /// <param name="selector">The projection selecting the key of each item.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">A projected key is null.</exception>
    public AndConstraint<SpanAssertions<T>> NotContainNulls<TKey>(Expression<Func<T, TKey>> selector, string? because = null, params object?[] becauseArgs) {
        _inner.NotContainNulls(selector, because, becauseArgs);
        return new(this);
    }
}
