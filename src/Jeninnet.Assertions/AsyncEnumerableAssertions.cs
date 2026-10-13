//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
#if !NETSTANDARD2_0
// CA1068: the cancellation token intentionally sits after the required inputs but before the trailing
// because/becauseArgs pair every assertion family shares. Moving it last would either displace the
// family's `params` array (impossible) or break the uniform call shape across the library; the token is
// therefore positional-optional by design.
#pragma warning disable CA1068
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for consuming asynchronous sequences deterministically, with cancellation and index diagnostics.</summary>
/// <remarks>
/// Every verb accepts a <see cref="CancellationToken"/> that is handed to the source's enumerator; a token already
/// canceled before the assertion throws <see cref="OperationCanceledException"/> immediately (it is never
/// swallowed). The verbs consume the sequence: they assume a finite source and drain it fully where a count or
/// equivalence verdict is required, so infinite sequences are out of scope. Element equality uses
/// <see cref="EqualityComparer{T}.Default"/>.
/// </remarks>
/// <param name="value">The asynchronous sequence to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
/// <typeparam name="T">The type of the elements yielded by the sequence.</typeparam>
public class AsyncEnumerableAssertions<T>(IAsyncEnumerable<T>? value, string? expression = null) : AssertionsBase(expression, "async sequence") {
    private readonly IAsyncEnumerable<T>? _value = value;

    /// <summary>Asserts that the sequence yields no items.</summary>
    /// <param name="cancellationToken">The token handed to the source's enumerator.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="OperationCanceledException">The token was canceled before or during enumeration.</exception>
    /// <exception cref="AssertionFailedException">The sequence is <see langword="null"/> or yields at least one item.</exception>
    public async Task<AndConstraint<AsyncEnumerableAssertions<T>>> BeEmptyAsync(CancellationToken cancellationToken = default, string? because = null, params object?[] becauseArgs) {
        cancellationToken.ThrowIfCancellationRequested();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        var items = await DrainAsync(cancellationToken);
        if(items.Count != 0) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_BE_EMPTY, Expression, AssertionScope.FormatInvariant(items.Count)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the sequence yields at least one item.</summary>
    /// <param name="cancellationToken">The token handed to the source's enumerator.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="OperationCanceledException">The token was canceled before or during enumeration.</exception>
    /// <exception cref="AssertionFailedException">The sequence is <see langword="null"/> or empty.</exception>
    public async Task<AndConstraint<AsyncEnumerableAssertions<T>>> NotBeEmptyAsync(CancellationToken cancellationToken = default, string? because = null, params object?[] becauseArgs) {
        cancellationToken.ThrowIfCancellationRequested();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        var hasItems = false;
        await using(var enumerator = _value.GetAsyncEnumerator(cancellationToken)) {
            hasItems = await enumerator.MoveNextAsync();
        }

        if(!hasItems) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_NOT_BE_EMPTY, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the sequence yields exactly <paramref name="expected"/> items.</summary>
    /// <param name="expected">The expected item count.</param>
    /// <param name="cancellationToken">The token handed to the source's enumerator.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="OperationCanceledException">The token was canceled before or during enumeration.</exception>
    /// <exception cref="AssertionFailedException">The sequence is <see langword="null"/> or the count differs.</exception>
    public async Task<AndConstraint<AsyncEnumerableAssertions<T>>> HaveCountAsync(int expected, CancellationToken cancellationToken = default, string? because = null, params object?[] becauseArgs) {
        cancellationToken.ThrowIfCancellationRequested();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        var items = await DrainAsync(cancellationToken);
        if(items.Count != expected) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_HAVE_COUNT, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(items.Count)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the sequence yields an item equal to <paramref name="expected"/>.</summary>
    /// <param name="expected">The item that must appear.</param>
    /// <param name="cancellationToken">The token handed to the source's enumerator.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="OperationCanceledException">The token was canceled before or during enumeration.</exception>
    /// <exception cref="AssertionFailedException">The sequence is <see langword="null"/> or lacks <paramref name="expected"/>.</exception>
    public async Task<AndConstraint<AsyncEnumerableAssertions<T>>> ContainAsync(T expected, CancellationToken cancellationToken = default, string? because = null, params object?[] becauseArgs) {
        cancellationToken.ThrowIfCancellationRequested();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        await using(var enumerator = _value.GetAsyncEnumerator(cancellationToken)) {
            while(await enumerator.MoveNextAsync()) {
                if(EqualityComparer<T>.Default.Equals(enumerator.Current, expected)) {
                    return new(this);
                }
            }
        }

        Fail(Strings.Format(StringKeys.ASYNC_ENUM_CONTAIN, Expression, AssertionScope.FormatInvariant(expected)), because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the sequence yields no item equal to <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The item that must not appear.</param>
    /// <param name="cancellationToken">The token handed to the source's enumerator.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="OperationCanceledException">The token was canceled before or during enumeration.</exception>
    /// <exception cref="AssertionFailedException">The sequence is <see langword="null"/> or yields <paramref name="unexpected"/> (the message names its index).</exception>
    public async Task<AndConstraint<AsyncEnumerableAssertions<T>>> NotContainAsync(T unexpected, CancellationToken cancellationToken = default, string? because = null, params object?[] becauseArgs) {
        cancellationToken.ThrowIfCancellationRequested();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        var index = 0;
        await using(var enumerator = _value.GetAsyncEnumerator(cancellationToken)) {
            while(await enumerator.MoveNextAsync()) {
                if(EqualityComparer<T>.Default.Equals(enumerator.Current, unexpected)) {
                    Fail(Strings.Format(StringKeys.ASYNC_ENUM_NOT_CONTAIN, Expression, AssertionScope.FormatInvariant(unexpected), AssertionScope.FormatInvariant(index)), because, becauseArgs);
                    return new(this);
                }

                index++;
            }
        }

        return new(this);
    }

    /// <summary>Asserts that the sequence yields the items in <paramref name="expected"/> in order, not necessarily consecutively.</summary>
    /// <param name="expected">The expected items in order.</param>
    /// <param name="cancellationToken">The token handed to the source's enumerator.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">The token was canceled before or during enumeration.</exception>
    /// <exception cref="AssertionFailedException">The sequence is <see langword="null"/> or the ordered subsequence was not found (the message names where).</exception>
    public async Task<AndConstraint<AsyncEnumerableAssertions<T>>> ContainInOrderAsync(T[] expected, CancellationToken cancellationToken = default, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        cancellationToken.ThrowIfCancellationRequested();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        var expectedIndex = 0;
        var searchStart = 0;
        var index = 0;
        await using(var enumerator = _value.GetAsyncEnumerator(cancellationToken)) {
            while(expectedIndex < expected.Length && await enumerator.MoveNextAsync()) {
                if(EqualityComparer<T>.Default.Equals(enumerator.Current, expected[expectedIndex])) {
                    expectedIndex++;
                    searchStart = index + 1;
                }

                index++;
            }
        }

        if(expectedIndex < expected.Length) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_CONTAIN_IN_ORDER, Expression, AssertionScope.FormatInvariant(expected[expectedIndex]), AssertionScope.FormatInvariant(searchStart)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the sequence is an unordered multiset equivalent to <paramref name="expected"/> (order is ignored, duplicates are counted).</summary>
    /// <param name="expected">The expected items as a multiset.</param>
    /// <param name="cancellationToken">The token handed to the source's enumerator.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">The token was canceled before or during enumeration.</exception>
    /// <exception cref="AssertionFailedException">The sequence is <see langword="null"/>, the counts differ, or an expected item has no match.</exception>
    public async Task<AndConstraint<AsyncEnumerableAssertions<T>>> BeEquivalentToAsync(T[] expected, CancellationToken cancellationToken = default, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        cancellationToken.ThrowIfCancellationRequested();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        var actual = await DrainAsync(cancellationToken);
        if(actual.Count != expected.Length) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_HAVE_COUNT, Expression, AssertionScope.FormatInvariant(expected.Length), AssertionScope.FormatInvariant(actual.Count)), because, becauseArgs);
            return new(this);
        }

        var remaining = new List<T>(actual);
        foreach(var expectedItem in expected) {
            var found = remaining.FindIndex(item => EqualityComparer<T>.Default.Equals(item, expectedItem));
            if(found < 0) {
                Fail(Strings.Format(StringKeys.ASYNC_ENUM_BE_EQUIVALENT, Expression, AssertionScope.FormatInvariant(expectedItem)), because, becauseArgs);
                return new(this);
            }

            remaining.RemoveAt(found);
        }

        return new(this);
    }

    /// <summary>Drains the sequence completely and exposes the materialized items for further assertions.</summary>
    /// <param name="cancellationToken">The token handed to the source's enumerator.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the drained items in yield order.</returns>
    /// <exception cref="OperationCanceledException">The token was canceled before or during enumeration.</exception>
    /// <exception cref="AssertionFailedException">The sequence is <see langword="null"/>.</exception>
    public async Task<WhichConstraint<IReadOnlyList<T>>> CompleteAsync(CancellationToken cancellationToken = default, string? because = null, params object?[] becauseArgs) {
        cancellationToken.ThrowIfCancellationRequested();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_NULL, Expression), because, becauseArgs);
            return new([]);
        }

        var items = await DrainAsync(cancellationToken);
        return new(items);
    }

    /// <summary>Drains the whole async sequence into a list, honoring the cancellation token.</summary>
    /// <param name="cancellationToken">The token passed to the enumerator.</param>
    /// <returns>The items in yield order.</returns>
    /// <exception cref="OperationCanceledException">The token was canceled before or during enumeration.</exception>
    private async Task<List<T>> DrainAsync(CancellationToken cancellationToken) {
        var items = new List<T>();
        await using var enumerator = _value!.GetAsyncEnumerator(cancellationToken);
        while(await enumerator.MoveNextAsync()) {
            items.Add(enumerator.Current);
        }

        return items;
    }
}
#pragma warning restore CA1068
#endif
