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

    /// <summary>Asserts that every item satisfies <paramref name="predicate"/> (stops at the first failure).</summary>
    /// <param name="predicate">The predicate every item must satisfy.</param>
    /// <param name="cancellationToken">The token handed to the source's enumerator.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">The token was canceled before or during enumeration.</exception>
    /// <exception cref="AssertionFailedException">The sequence is <see langword="null"/> or an item fails the predicate (its index is reported).</exception>
    public async Task<AndConstraint<AsyncEnumerableAssertions<T>>> AllAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default, string? because = null, params object?[] becauseArgs) {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        cancellationToken.ThrowIfCancellationRequested();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        var index = 0;
        await using(var enumerator = _value.GetAsyncEnumerator(cancellationToken)) {
            while(await enumerator.MoveNextAsync()) {
                if(!predicate(enumerator.Current)) {
                    Fail(Strings.Format(StringKeys.ASYNC_ENUM_ALL, Expression, AssertionScope.FormatInvariant(index)), because, becauseArgs);
                    return new(this);
                }

                index++;
            }
        }

        return new(this);
    }

    /// <summary>Asserts that at least one item satisfies <paramref name="predicate"/> (stops at the first match).</summary>
    /// <param name="predicate">The predicate at least one item must satisfy.</param>
    /// <param name="cancellationToken">The token handed to the source's enumerator.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">The token was canceled before or during enumeration.</exception>
    /// <exception cref="AssertionFailedException">The sequence is <see langword="null"/> or no item satisfies the predicate.</exception>
    public async Task<AndConstraint<AsyncEnumerableAssertions<T>>> AnyAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default, string? because = null, params object?[] becauseArgs) {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        cancellationToken.ThrowIfCancellationRequested();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        await using(var enumerator = _value.GetAsyncEnumerator(cancellationToken)) {
            while(await enumerator.MoveNextAsync()) {
                if(predicate(enumerator.Current)) {
                    return new(this);
                }
            }
        }

        Fail(Strings.Format(StringKeys.ASYNC_ENUM_ANY, Expression), because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the sequence yields exactly one item and exposes it through <see cref="WhichConstraint{T}.Which"/>.</summary>
    /// <param name="cancellationToken">The token handed to the source's enumerator.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the single item.</returns>
    /// <exception cref="OperationCanceledException">The token was canceled before or during enumeration.</exception>
    /// <exception cref="AssertionFailedException">The sequence is <see langword="null"/>, empty, or has more than one item.</exception>
    public Task<WhichConstraint<T>> ContainSingleAsync(CancellationToken cancellationToken = default, string? because = null, params object?[] becauseArgs) =>
        ContainSingleCoreAsync(null, cancellationToken, because, becauseArgs);

    /// <summary>Asserts that the sequence yields exactly one item satisfying <paramref name="predicate"/> and exposes it.</summary>
    /// <param name="predicate">The predicate the single item must satisfy.</param>
    /// <param name="cancellationToken">The token handed to the source's enumerator.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the single matching item.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">The token was canceled before or during enumeration.</exception>
    /// <exception cref="AssertionFailedException">The sequence is <see langword="null"/>, or it contains zero or multiple matching items.</exception>
    public Task<WhichConstraint<T>> ContainSingleAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default, string? because = null, params object?[] becauseArgs) {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        return ContainSingleCoreAsync(predicate, cancellationToken, because, becauseArgs);
    }

    /// <summary>Asserts that the sequence yields only unique items (by <see cref="EqualityComparer{T}.Default"/>).</summary>
    /// <param name="cancellationToken">The token handed to the source's enumerator.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="OperationCanceledException">The token was canceled before or during enumeration.</exception>
    /// <exception cref="AssertionFailedException">The sequence is <see langword="null"/> or repeats an item (the item and index are reported).</exception>
    public async Task<AndConstraint<AsyncEnumerableAssertions<T>>> OnlyHaveUniqueItemsAsync(CancellationToken cancellationToken = default, string? because = null, params object?[] becauseArgs) {
        cancellationToken.ThrowIfCancellationRequested();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        var seen = new HashSet<T>(EqualityComparer<T>.Default);
        var index = 0;
        await using(var enumerator = _value.GetAsyncEnumerator(cancellationToken)) {
            while(await enumerator.MoveNextAsync()) {
                if(!seen.Add(enumerator.Current)) {
                    Fail(Strings.Format(StringKeys.ASYNC_ENUM_ONLY_UNIQUE, Expression, AssertionScope.FormatInvariant(enumerator.Current), AssertionScope.FormatInvariant(index)), because, becauseArgs);
                    return new(this);
                }

                index++;
            }
        }

        return new(this);
    }

    /// <summary>Asserts that the items in <paramref name="expected"/> appear consecutively and in order.</summary>
    /// <param name="expected">The expected consecutive window.</param>
    /// <param name="cancellationToken">The token handed to the source's enumerator.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">The token was canceled before or during enumeration.</exception>
    /// <exception cref="AssertionFailedException">The sequence is <see langword="null"/> or the window is absent.</exception>
    public async Task<AndConstraint<AsyncEnumerableAssertions<T>>> ContainInConsecutiveOrderAsync(T[] expected, CancellationToken cancellationToken = default, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        cancellationToken.ThrowIfCancellationRequested();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        if(expected.Length == 0) {
            return new(this);
        }

        var window = new Queue<T>(expected.Length);
        await using(var enumerator = _value.GetAsyncEnumerator(cancellationToken)) {
            while(await enumerator.MoveNextAsync()) {
                window.Enqueue(enumerator.Current);
                if(window.Count > expected.Length) {
                    _ = window.Dequeue();
                }

                if(window.Count == expected.Length && WindowMatches(window, expected)) {
                    return new(this);
                }
            }
        }

        Fail(Strings.Format(StringKeys.ASYNC_ENUM_CONTAIN_CONSECUTIVE, Expression, FormatItems(expected)), because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the items in <paramref name="unexpected"/> do not appear in order (not necessarily consecutively).</summary>
    /// <param name="unexpected">The ordered subsequence that must be absent.</param>
    /// <param name="cancellationToken">The token handed to the source's enumerator.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="unexpected"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">The token was canceled before or during enumeration.</exception>
    /// <exception cref="AssertionFailedException">The sequence is <see langword="null"/> or contains the ordered subsequence.</exception>
    public async Task<AndConstraint<AsyncEnumerableAssertions<T>>> NotContainInOrderAsync(T[] unexpected, CancellationToken cancellationToken = default, string? because = null, params object?[] becauseArgs) {
        if(unexpected is null) {
            throw new ArgumentNullException(nameof(unexpected));
        }

        cancellationToken.ThrowIfCancellationRequested();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        var expectedIndex = 0;
        if(unexpected.Length == 0) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_NOT_CONTAIN_IN_ORDER, Expression, FormatItems(unexpected)), because, becauseArgs);
            return new(this);
        }

        await using(var enumerator = _value.GetAsyncEnumerator(cancellationToken)) {
            while(expectedIndex < unexpected.Length && await enumerator.MoveNextAsync()) {
                if(EqualityComparer<T>.Default.Equals(enumerator.Current, unexpected[expectedIndex])) {
                    expectedIndex++;
                }
            }
        }

        if(expectedIndex == unexpected.Length) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_NOT_CONTAIN_IN_ORDER, Expression, FormatItems(unexpected)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the sequence yields more than <paramref name="expected"/> items.</summary>
    /// <param name="expected">The count the sequence must exceed.</param>
    /// <param name="cancellationToken">The token handed to the source's enumerator.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="OperationCanceledException">The token was canceled before or during enumeration.</exception>
    /// <exception cref="AssertionFailedException">The sequence is <see langword="null"/> or its count is not greater than <paramref name="expected"/>.</exception>
    public async Task<AndConstraint<AsyncEnumerableAssertions<T>>> HaveCountGreaterThanAsync(int expected, CancellationToken cancellationToken = default, string? because = null, params object?[] becauseArgs) {
        var count = await CountAsync(cancellationToken, because, becauseArgs);
        if(count is null) {
            return new(this);
        }
        if(count.Value <= expected) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_COUNT_GREATER_THAN, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(count.Value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the sequence yields at least <paramref name="expected"/> items.</summary>
    /// <param name="expected">The minimum accepted count.</param>
    /// <param name="cancellationToken">The token handed to the source's enumerator.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="OperationCanceledException">The token was canceled before or during enumeration.</exception>
    /// <exception cref="AssertionFailedException">The sequence is <see langword="null"/> or its count is below <paramref name="expected"/>.</exception>
    public async Task<AndConstraint<AsyncEnumerableAssertions<T>>> HaveCountGreaterThanOrEqualToAsync(int expected, CancellationToken cancellationToken = default, string? because = null, params object?[] becauseArgs) {
        var count = await CountAsync(cancellationToken, because, becauseArgs);
        if(count is null) {
            return new(this);
        }
        if(count.Value < expected) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_COUNT_GREATER_THAN_OR_EQUAL, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(count.Value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the sequence yields fewer than <paramref name="expected"/> items.</summary>
    /// <param name="expected">The count the sequence must stay below.</param>
    /// <param name="cancellationToken">The token handed to the source's enumerator.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="OperationCanceledException">The token was canceled before or during enumeration.</exception>
    /// <exception cref="AssertionFailedException">The sequence is <see langword="null"/> or its count is not below <paramref name="expected"/>.</exception>
    public async Task<AndConstraint<AsyncEnumerableAssertions<T>>> HaveCountLessThanAsync(int expected, CancellationToken cancellationToken = default, string? because = null, params object?[] becauseArgs) {
        var count = await CountAsync(cancellationToken, because, becauseArgs);
        if(count is null) {
            return new(this);
        }
        if(count.Value >= expected) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_COUNT_LESS_THAN, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(count.Value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the sequence yields at most <paramref name="expected"/> items.</summary>
    /// <param name="expected">The maximum accepted count.</param>
    /// <param name="cancellationToken">The token handed to the source's enumerator.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="OperationCanceledException">The token was canceled before or during enumeration.</exception>
    /// <exception cref="AssertionFailedException">The sequence is <see langword="null"/> or its count exceeds <paramref name="expected"/>.</exception>
    public async Task<AndConstraint<AsyncEnumerableAssertions<T>>> HaveCountLessThanOrEqualToAsync(int expected, CancellationToken cancellationToken = default, string? because = null, params object?[] becauseArgs) {
        var count = await CountAsync(cancellationToken, because, becauseArgs);
        if(count is null) {
            return new(this);
        }
        if(count.Value > expected) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_COUNT_LESS_THAN_OR_EQUAL, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(count.Value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the sequence's items satisfy the paired inspectors, in order.</summary>
    /// <param name="inspectors">One awaited inspector per item; the count must match the sequence.</param>
    /// <param name="cancellationToken">The token handed to the source's enumerator and checked before each inspector.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="inspectors"/> is <see langword="null"/> or contains <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">The token was canceled before or during enumeration or inspection.</exception>
    /// <exception cref="AssertionFailedException">The sequence is <see langword="null"/>, the counts differ, or an inspector fails.</exception>
    public async Task<AndConstraint<AsyncEnumerableAssertions<T>>> SatisfyRespectivelyAsync(Func<T, Task>[] inspectors, CancellationToken cancellationToken = default, string? because = null, params object?[] becauseArgs) {
        if(inspectors is null) {
            throw new ArgumentNullException(nameof(inspectors));
        }

        if(Array.IndexOf(inspectors, null) >= 0) {
            throw new ArgumentNullException(nameof(inspectors), "Inspectors must not contain null.");
        }

        cancellationToken.ThrowIfCancellationRequested();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        var items = await DrainAsync(cancellationToken);
        if(items.Count != inspectors.Length) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_HAVE_COUNT, Expression, AssertionScope.FormatInvariant(inspectors.Length), AssertionScope.FormatInvariant(items.Count)), because, becauseArgs);
            return new(this);
        }

        for(var index = 0; index < items.Count; index++) {
            cancellationToken.ThrowIfCancellationRequested();
            await inspectors[index](items[index]);
        }

        return new(this);
    }

    /// <summary>Asserts that each item satisfies its paired asynchronous predicate, in order.</summary>
    /// <param name="predicates">One awaited predicate per item; the count must match the sequence.</param>
    /// <param name="cancellationToken">The token handed to the source's enumerator and checked before each predicate.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicates"/> is <see langword="null"/> or contains <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">The token was canceled before or during enumeration or predicate evaluation.</exception>
    /// <exception cref="AssertionFailedException">The sequence is <see langword="null"/>, the counts differ, or a predicate returns <see langword="false"/>.</exception>
    public async Task<AndConstraint<AsyncEnumerableAssertions<T>>> SatisfyAsync(Func<T, Task<bool>>[] predicates, CancellationToken cancellationToken = default, string? because = null, params object?[] becauseArgs) {
        if(predicates is null) {
            throw new ArgumentNullException(nameof(predicates));
        }

        if(Array.IndexOf(predicates, null) >= 0) {
            throw new ArgumentNullException(nameof(predicates), "Predicates must not contain null.");
        }

        cancellationToken.ThrowIfCancellationRequested();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        var items = await DrainAsync(cancellationToken);
        if(items.Count != predicates.Length) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_HAVE_COUNT, Expression, AssertionScope.FormatInvariant(predicates.Length), AssertionScope.FormatInvariant(items.Count)), because, becauseArgs);
            return new(this);
        }

        for(var index = 0; index < items.Count; index++) {
            cancellationToken.ThrowIfCancellationRequested();
            if(!await predicates[index](items[index])) {
                Fail(Strings.Format(StringKeys.ASYNC_ENUM_SATISFY_PREDICATE, AssertionScope.FormatInvariant(index), Expression), because, becauseArgs);
            }
        }

        return new(this);
    }

    private async Task<WhichConstraint<T>> ContainSingleCoreAsync(Func<T, bool>? predicate, CancellationToken cancellationToken, string? because, object?[] becauseArgs) {
        cancellationToken.ThrowIfCancellationRequested();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_NULL, Expression), because, becauseArgs);
            return new(default!);
        }

        var matches = 0;
        var single = default(T)!;
        await using(var enumerator = _value.GetAsyncEnumerator(cancellationToken)) {
            while(await enumerator.MoveNextAsync()) {
                if(predicate is null || predicate(enumerator.Current)) {
                    matches++;
                    if(matches == 1) {
                        single = enumerator.Current;
                    }
                }
            }
        }

        if(matches == 0) {
            var key = predicate is null ? StringKeys.ASYNC_ENUM_CONTAIN_SINGLE_NONE : StringKeys.ASYNC_ENUM_CONTAIN_SINGLE_NONE_PREDICATE;
            Fail(Strings.Format(key, Expression), because, becauseArgs);
        } else if(matches > 1) {
            var key = predicate is null ? StringKeys.ASYNC_ENUM_CONTAIN_SINGLE_MANY : StringKeys.ASYNC_ENUM_CONTAIN_SINGLE_MANY_PREDICATE;
            Fail(Strings.Format(key, Expression, AssertionScope.FormatInvariant(matches)), because, becauseArgs);
        }

        return new(single);
    }

    private async Task<int?> CountAsync(CancellationToken cancellationToken, string? because, object?[] becauseArgs) {
        cancellationToken.ThrowIfCancellationRequested();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.ASYNC_ENUM_NULL, Expression), because, becauseArgs);
            return null;
        }

        var items = await DrainAsync(cancellationToken);
        return items.Count;
    }

    private static bool WindowMatches(Queue<T> window, T[] expected) {
        var index = 0;
        foreach(var item in window) {
            if(!EqualityComparer<T>.Default.Equals(item, expected[index])) {
                return false;
            }

            index++;
        }

        return true;
    }

    private static string FormatItems(T[] items) =>
        string.Join(", ", items.Select(static item => AssertionScope.FormatInvariant(item)));

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
