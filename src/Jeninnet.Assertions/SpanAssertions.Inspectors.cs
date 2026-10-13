//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class SpanAssertions<T> {
    /// <summary>Asserts that span items satisfy <paramref name="inspectors"/> respectively, in order.</summary>
    /// <param name="inspectors">One inspector per leading item.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="inspectors"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">An inspection fails.</exception>
    public AndConstraint<SpanAssertions<T>> SatisfyRespectively(params Action<T>[] inspectors) {
        _inner.SatisfyRespectively(inspectors);
        return new(this);
    }

    /// <summary>Asserts that every span item satisfies <paramref name="action"/>.</summary>
    /// <param name="action">The assertion applied to every item.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">An item fails the assertion.</exception>
    public AndConstraint<SpanAssertions<T>> AllSatisfy(Action<T> action, string? because = null, params object?[] becauseArgs) {
        _inner.AllSatisfy(action, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that span items satisfy <paramref name="predicates"/> in any order.</summary>
    /// <param name="predicates">The predicates every item set must cover.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicates"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">A predicate matches no item.</exception>
    public AndConstraint<SpanAssertions<T>> Satisfy(params Expression<Func<T, bool>>[] predicates) {
        _inner.Satisfy(predicates);
        return new(this);
    }

    /// <summary>Asserts that every span item is of type <typeparamref name="TExpectation"/> and exposes the first one.</summary>
    /// <typeparam name="TExpectation">The expected item type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the first matching item.</returns>
    /// <exception cref="AssertionFailedException">An item has another type.</exception>
    public WhichConstraint<TExpectation> AllBeOfType<TExpectation>(string? because = null, params object?[] becauseArgs) =>
        _inner.AllBeOfType<TExpectation>(because, becauseArgs);

    /// <summary>Asserts that every span item is assignable to type <typeparamref name="TExpectation"/> and exposes the first one.</summary>
    /// <typeparam name="TExpectation">The type every item must be assignable to.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the first matching item.</returns>
    /// <exception cref="AssertionFailedException">An item is not assignable.</exception>
    public WhichConstraint<TExpectation> AllBeAssignableTo<TExpectation>(string? because = null, params object?[] becauseArgs) =>
        _inner.AllBeAssignableTo<TExpectation>(because, becauseArgs);

    /// <summary>Asserts that every span item appears exactly once.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The span contains a duplicate item.</exception>
    public AndConstraint<SpanAssertions<T>> OnlyHaveUniqueItems(string? because = null, params object?[] becauseArgs) {
        _inner.OnlyHaveUniqueItems(because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the projected keys every span item maps to via <paramref name="selector"/> are unique.</summary>
    /// <typeparam name="TKey">The projected key type.</typeparam>
    /// <param name="selector">The projection selecting the key of each item.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">Two items share a key.</exception>
    public AndConstraint<SpanAssertions<T>> OnlyHaveUniqueItems<TKey>(Expression<Func<T, TKey>> selector, string? because = null, params object?[] becauseArgs) {
        _inner.OnlyHaveUniqueItems(selector, because, becauseArgs);
        return new(this);
    }
}
