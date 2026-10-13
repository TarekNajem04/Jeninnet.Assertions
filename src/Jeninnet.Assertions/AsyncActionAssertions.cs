//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying that asynchronous delegates throw expected exceptions or complete without throwing.</summary>
/// <param name="action">The asynchronous delegate to invoke and verify.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
/// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
public class AsyncActionAssertions(Func<Task> action, string? expression = null) : AssertionsBase(expression, "action") {
    private readonly Func<Task> _action = action ?? throw new ArgumentNullException(nameof(action));

    /// <summary>Asserts that the asynchronous action throws an exception of type <typeparamref name="TException"/>.</summary>
    /// <remarks>
    /// This assertion returns a value rather than an <see cref="AndConstraint{TParent}"/>, so it throws
    /// immediately even inside an <see cref="AssertionScope"/> instead of being batched.
    /// </remarks>
    /// <typeparam name="TException">The expected exception type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A task whose result is an <see cref="ExceptionAssertions{TException}"/> that provides access to the caught exception.</returns>
    /// <exception cref="AssertionFailedException">No exception was thrown, or a different exception type was thrown.</exception>
    public async Task<ExceptionAssertions<TException>> ThrowAsync<TException>(string? because = null, params object?[] becauseArgs) where TException : Exception =>
        new((TException)await ExceptionAssertionCore.ThrowAsync<TException>(_action, Expression, because, becauseArgs));

    /// <summary>Asserts that the asynchronous action throws an exception of exactly type <typeparamref name="TException"/> (derived types fail).</summary>
    /// <typeparam name="TException">The expected exact exception type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A task whose result is an <see cref="ExceptionAssertions{TException}"/> that provides access to the caught exception.</returns>
    /// <exception cref="AssertionFailedException">No exception was thrown, or the exception type is not exactly <typeparamref name="TException"/>.</exception>
    public async Task<ExceptionAssertions<TException>> ThrowExactlyAsync<TException>(string? because = null, params object?[] becauseArgs) where TException : Exception =>
        new((TException)await ExceptionAssertionCore.ThrowExactlyAsync<TException>(_action, Expression, because, becauseArgs));

    /// <summary>Asserts that the asynchronous action throws an exception within <paramref name="timeout"/>.</summary>
    /// <param name="timeout">The maximum time to wait for an exception (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A task whose result provides access to the caught exception.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">No exception was thrown before the timeout elapsed.</exception>
    public Task<ExceptionAssertions<Exception>> ThrowWithinAsync(TimeSpan timeout, string? because = null, params object?[] becauseArgs) =>
        ThrowWithinAsync<Exception>(timeout, because, becauseArgs);

    /// <summary>Asserts that the asynchronous action throws an exception of type <typeparamref name="TException"/> within <paramref name="timeout"/>.</summary>
    /// <typeparam name="TException">The expected exception type.</typeparam>
    /// <param name="timeout">The maximum time to wait for the exception (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A task whose result provides access to the caught exception.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">No matching exception was thrown before the timeout elapsed.</exception>
    public async Task<ExceptionAssertions<TException>> ThrowWithinAsync<TException>(TimeSpan timeout, string? because = null, params object?[] becauseArgs) where TException : Exception =>
        new((TException)await ExceptionAssertionCore.ThrowWithinAsync<TException>(_action, timeout, Expression, because, becauseArgs));

    /// <summary>Asserts that the asynchronous action completes without throwing any exception.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A task that represents the asynchronous assertion operation.</returns>
    /// <exception cref="AssertionFailedException">The action threw an exception of any type, including a canceled-task exception.</exception>
    public async Task NotThrowAsync(string? because = null, params object?[] becauseArgs) {
        var thrown = await ExceptionAssertionCore.NotThrowAsync(_action);
        if(thrown is not null) {
            throw new AssertionFailedException(
                AssertionScope.AppendBecause(Strings.Format(StringKeys.ACTION_NOT_THROW, Expression, thrown.GetType().Name), because, becauseArgs),
                thrown);
        }
    }

    /// <summary>Asserts that the asynchronous action completes within <paramref name="timeout"/>; faults and cancellations still fail, with the original exception preserved.</summary>
    /// <param name="timeout">The maximum accepted completion time (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A task whose result can continue the fluent chain.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The action did not complete in time, faulted, or was canceled.</exception>
    public async Task<AndConstraint<AsyncActionAssertions>> CompleteWithinAsync(TimeSpan timeout, string? because = null, params object?[] becauseArgs) {
        await new TaskAssertions(_action(), Expression).CompleteWithinAsync(timeout, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the asynchronous action does not complete within <paramref name="timeout"/>.</summary>
    /// <param name="timeout">The time span to wait for completion (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A task whose result can continue the fluent chain.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The action completed within the time span.</exception>
    public async Task<AndConstraint<AsyncActionAssertions>> NotCompleteWithinAsync(TimeSpan timeout, string? because = null, params object?[] becauseArgs) {
        await new TaskAssertions(_action(), Expression).NotCompleteWithinAsync(timeout, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that repeated invocations stop throwing within <paramref name="waitTime"/>, checking every <paramref name="pollInterval"/>.</summary>
    /// <remarks>This compatibility assertion only waits for quiet behavior. Prefer <see cref="SucceedWithinAsync(TimeSpan, TimeSpan?, string?, object?[])"/> for transient failures because it reports the final failure and its retry semantics.</remarks>
    /// <param name="waitTime">The time after which the action should have stopped throwing (must not be negative).</param>
    /// <param name="pollInterval">The pause between attempts (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A task whose result can continue the fluent chain.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="waitTime"/> or <paramref name="pollInterval"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The action was still throwing when the wait elapsed.</exception>
    public async Task<AndConstraint<AsyncActionAssertions>> NotThrowAfterAsync(TimeSpan waitTime, TimeSpan pollInterval, string? because = null, params object?[] becauseArgs) {
        var failure = await ExceptionAssertionCore.NotThrowAfterAsync(_action, waitTime, pollInterval);
        if(failure is not null) {
            throw new AssertionFailedException(
                AssertionScope.AppendBecause(Strings.Format(StringKeys.ACTION_NOT_THROW_AFTER, Expression, AssertionScope.FormatInvariant(waitTime), failure.GetType().Name), because, becauseArgs),
                failure);
        }

        return new AndConstraint<AsyncActionAssertions>(this);
    }

    /// <summary>Asserts that the asynchronous action eventually completes without throwing within <paramref name="timeout"/>, retrying every <paramref name="pollInterval"/> (for eventually-consistent scenarios).</summary>
    /// <param name="timeout">The total retry budget (must not be negative).</param>
    /// <param name="pollInterval">The pause between attempts (defaults to 50 ms; must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A task that represents the asynchronous assertion operation.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> or <paramref name="pollInterval"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The action kept throwing until the timeout elapsed.</exception>
    public async Task SucceedWithinAsync(TimeSpan timeout, TimeSpan? pollInterval = null, string? because = null, params object?[] becauseArgs) {
        var interval = pollInterval ?? TimeSpan.FromMilliseconds(50);
        await SucceedWithinAsync(PollingBackoff.Constant(interval), timeout, because, becauseArgs);
    }

    /// <summary>Asserts that the asynchronous action eventually completes without throwing within <paramref name="timeout"/>, pausing between attempts according to <paramref name="backoff"/> (for eventually-consistent scenarios).</summary>
    /// <param name="backoff">The delay schedule between attempts (must not be <see langword="null"/>).</param>
    /// <param name="timeout">The total retry budget (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A task that represents the asynchronous assertion operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="backoff"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The action kept throwing until the timeout elapsed.</exception>
    public async Task SucceedWithinAsync(PollingBackoff backoff, TimeSpan timeout, string? because = null, params object?[] becauseArgs) {
        var failure = await ExceptionAssertionCore.SucceedWithinAsync(_action, backoff, timeout);
        if(failure is not null) {
            throw new AssertionFailedException(
                AssertionScope.AppendBecause(Strings.Format(StringKeys.ACTION_SUCCEED_WITHIN, Expression, AssertionScope.FormatInvariant(timeout), failure.Message.TrimEnd('.')), because, becauseArgs),
                failure);
        }
    }
#if !NETSTANDARD2_0

    /// <summary>Asserts that the asynchronous action allocates at most <paramref name="maxBytes"/> on the current thread.</summary>
    /// <remarks>Requires .NET Core 2.0+; unavailable on .NET Standard 2.0.</remarks>
    /// <param name="maxBytes">The maximum accepted bytes (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A task that represents the asynchronous assertion operation.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxBytes"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The action allocated more or threw.</exception>
    public async Task AllocateAtMostAsync(long maxBytes, string? because = null, params object?[] becauseArgs) {
        var allocated = await ExceptionAssertionCore.MeasureAllocationAsync(_action, maxBytes, StringKeys.ACTION_ALLOCATE_AT_MOST_THREW, Expression, because, becauseArgs);

        if(allocated > maxBytes) {
            throw new AssertionFailedException(
                AssertionScope.AppendBecause(Strings.Format(StringKeys.ACTION_ALLOCATE_AT_MOST, Expression, Allocation.FormatBytes(maxBytes), Allocation.FormatBytes(allocated)), because, becauseArgs));
        }
    }

    /// <summary>Asserts that the asynchronous action allocates less than <paramref name="maxBytes"/> on the current thread.</summary>
    /// <remarks>Requires .NET Core 2.0+; unavailable on .NET Standard 2.0.</remarks>
    /// <param name="maxBytes">The exclusive upper bound in bytes (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A task that represents the asynchronous assertion operation.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxBytes"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The action allocated too much or threw.</exception>
    public async Task AllocateLessThanAsync(long maxBytes, string? because = null, params object?[] becauseArgs) {
        var allocated = await ExceptionAssertionCore.MeasureAllocationAsync(_action, maxBytes, StringKeys.ACTION_ALLOCATE_LESS_THAN_THREW, Expression, because, becauseArgs);

        if(allocated >= maxBytes) {
            throw new AssertionFailedException(
                AssertionScope.AppendBecause(Strings.Format(StringKeys.ACTION_ALLOCATE_LESS_THAN, Expression, Allocation.FormatBytes(maxBytes), Allocation.FormatBytes(allocated)), because, becauseArgs));
        }
    }
#endif
}
