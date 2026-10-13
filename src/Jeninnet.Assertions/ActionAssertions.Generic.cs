//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying that synchronous value-returning delegates throw expected exceptions or complete without throwing.</summary>
/// <typeparam name="T">The type of the value returned by the delegate.</typeparam>
/// <param name="action">The synchronous delegate to invoke and verify.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
/// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
public class ActionAssertions<T>(Func<T> action, string? expression = null) : AssertionsBase(expression, "action") {
    private readonly Func<T> _action = action ?? throw new ArgumentNullException(nameof(action));

    /// <summary>Asserts that the synchronous action throws an exception of type <typeparamref name="TException"/>.</summary>
    /// <remarks>
    /// This assertion returns a value rather than an <see cref="AndConstraint{TParent}"/>, so it throws
    /// immediately even inside an <see cref="AssertionScope"/> instead of being batched.
    /// </remarks>
    /// <typeparam name="TException">The expected exception type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="ExceptionAssertions{TException}"/> that provides access to the caught exception.</returns>
    /// <exception cref="AssertionFailedException">No exception was thrown, or a different exception type was thrown.</exception>
    public ExceptionAssertions<TException> Throw<TException>(string? because = null, params object?[] becauseArgs) where TException : Exception =>
        new((TException)ExceptionAssertionCore.Throw<TException>(Invoke, Expression, because, becauseArgs));

    /// <summary>Asserts that the synchronous action throws an exception of exactly type <typeparamref name="TException"/> (derived types fail).</summary>
    /// <typeparam name="TException">The expected exact exception type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="ExceptionAssertions{TException}"/> that provides access to the caught exception.</returns>
    /// <exception cref="AssertionFailedException">No exception was thrown, or the exception type is not exactly <typeparamref name="TException"/>.</exception>
    public ExceptionAssertions<TException> ThrowExactly<TException>(string? because = null, params object?[] becauseArgs) where TException : Exception =>
        new((TException)ExceptionAssertionCore.ThrowExactly<TException>(Invoke, Expression, because, becauseArgs));

    /// <summary>Asserts that the synchronous action completes without throwing any exception.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The action threw an exception of any type.</exception>
    public AndConstraint<ActionAssertions<T>> NotThrow(string? because = null, params object?[] becauseArgs) {
        var thrown = ExceptionAssertionCore.NotThrow(Invoke);
        if(thrown is not null) {
            Fail(Strings.Format(StringKeys.ACTION_NOT_THROW, Expression, thrown.GetType().Name), because, becauseArgs, thrown);
        }

        return new(this);
    }

    /// <summary>Invokes the delegate for the shared exception assertion core, boxing any result.</summary>
    /// <returns>The delegate's return value boxed as <see cref="object"/>.</returns>
    private object? Invoke() => _action();

    /// <summary>Asserts that repeated invocations stop throwing within <paramref name="waitTime"/>, checking every <paramref name="pollInterval"/>.</summary>
    /// <remarks>This compatibility assertion only waits for quiet behavior. Prefer <see cref="SucceedWithin(TimeSpan, TimeSpan?, string?, object?[])"/> for transient failures because it reports the final failure and its retry semantics.</remarks>
    /// <param name="waitTime">The time after which the action should have stopped throwing (must not be negative).</param>
    /// <param name="pollInterval">The pause between attempts (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="waitTime"/> or <paramref name="pollInterval"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The action was still throwing when the wait elapsed.</exception>
    public AndConstraint<ActionAssertions<T>> NotThrowAfter(TimeSpan waitTime, TimeSpan pollInterval, string? because = null, params object?[] becauseArgs) {
        var failure = ExceptionAssertionCore.NotThrowAfter(Invoke, waitTime, pollInterval);
        if(failure is not null) {
            throw new AssertionFailedException(
                AssertionScope.AppendBecause(Strings.Format(StringKeys.ACTION_NOT_THROW_AFTER, Expression, AssertionScope.FormatInvariant(waitTime), failure.GetType().Name), because, becauseArgs),
                failure);
        }

        return new(this);
    }

    /// <summary>Asserts that the synchronous action eventually completes without throwing within <paramref name="timeout"/>, retrying every <paramref name="pollInterval"/> (for eventually-consistent scenarios).</summary>
    /// <param name="timeout">The total retry budget (must not be negative).</param>
    /// <param name="pollInterval">The pause between attempts (defaults to 50 ms; must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> or <paramref name="pollInterval"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The action kept throwing until the timeout elapsed.</exception>
    public AndConstraint<ActionAssertions<T>> SucceedWithin(TimeSpan timeout, TimeSpan? pollInterval = null, string? because = null, params object?[] becauseArgs) {
        var interval = pollInterval ?? TimeSpan.FromMilliseconds(50);
        return SucceedWithin(PollingBackoff.Constant(interval), timeout, because, becauseArgs);
    }

    /// <summary>Asserts that the synchronous action eventually completes without throwing within <paramref name="timeout"/>, pausing between attempts according to <paramref name="backoff"/> (for eventually-consistent scenarios).</summary>
    /// <param name="backoff">The delay schedule between attempts (must not be <see langword="null"/>).</param>
    /// <param name="timeout">The total retry budget (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="backoff"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The action kept throwing until the timeout elapsed.</exception>
    public AndConstraint<ActionAssertions<T>> SucceedWithin(PollingBackoff backoff, TimeSpan timeout, string? because = null, params object?[] becauseArgs) {
        var failure = ExceptionAssertionCore.SucceedWithin(Invoke, backoff, timeout);
        if(failure is not null) {
            throw new AssertionFailedException(
                AssertionScope.AppendBecause(Strings.Format(StringKeys.ACTION_SUCCEED_WITHIN, Expression, AssertionScope.FormatInvariant(timeout), failure.Message.TrimEnd('.')), because, becauseArgs),
                failure);
        }

        return new(this);
    }

    /// <summary>Asserts that re-invoking the delegate eventually produces a value satisfying <paramref name="predicate"/> within <paramref name="timeout"/> (for eventually-consistent scenarios).</summary>
    /// <remarks>Delegates that throw are treated like mismatches and retried; the last produced value and error are reported on failure.</remarks>
    /// <param name="predicate">The condition a produced value must satisfy (must not be <see langword="null"/>).</param>
    /// <param name="timeout">The total retry budget (must not be negative).</param>
    /// <param name="pollInterval">The pause between attempts (defaults to 50 ms; must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> or <paramref name="pollInterval"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">No produced value satisfied the predicate in time.</exception>
    public AndConstraint<ActionAssertions<T>> EventuallySatisfy(Func<T, bool> predicate, TimeSpan timeout, TimeSpan? pollInterval = null, string? because = null, params object?[] becauseArgs) {
        var interval = pollInterval ?? TimeSpan.FromMilliseconds(50);
        return EventuallySatisfy(predicate, PollingBackoff.Constant(interval), timeout, because, becauseArgs);
    }

    /// <summary>Asserts that re-invoking the delegate eventually produces a value satisfying <paramref name="predicate"/> within <paramref name="timeout"/>, pausing between attempts according to <paramref name="backoff"/> (for eventually-consistent scenarios).</summary>
    /// <remarks>Delegates that throw are treated like mismatches and retried; the last produced value and error are reported on failure.</remarks>
    /// <param name="predicate">The condition a produced value must satisfy (must not be <see langword="null"/>).</param>
    /// <param name="backoff">The delay schedule between attempts (must not be <see langword="null"/>).</param>
    /// <param name="timeout">The total retry budget (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> or <paramref name="backoff"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">No produced value satisfied the predicate in time.</exception>
    public AndConstraint<ActionAssertions<T>> EventuallySatisfy(Func<T, bool> predicate, PollingBackoff backoff, TimeSpan timeout, string? because = null, params object?[] becauseArgs) {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        if(backoff is null) {
            throw new ArgumentNullException(nameof(backoff));
        }

        if(timeout < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must not be negative.");
        }

        var result = Polling.TrySatisfy(_action, predicate, timeout, backoff);
        if(result.Satisfied) {
            return new(this);
        }

        var lastValue = !result.HasValue || result.Value is null ? "<null>" : AssertionScope.FormatInvariant(result.Value);
        if(result.Error is null) {
            Fail(Strings.Format(StringKeys.ACTION_EVENTUALLY_SATISFY, Expression, AssertionScope.FormatInvariant(timeout), lastValue), because, becauseArgs);
        } else {
            throw new AssertionFailedException(
                AssertionScope.AppendBecause(Strings.Format(StringKeys.ACTION_EVENTUALLY_SATISFY_ERROR, Expression, AssertionScope.FormatInvariant(timeout), lastValue, result.Error.Message.TrimEnd('.')), because, becauseArgs),
                result.Error);
        }

        return new(this);
    }
#if !NETSTANDARD2_0

    /// <summary>Asserts that the synchronous action allocates at most <paramref name="maxBytes"/> on the current thread.</summary>
    /// <remarks>Requires .NET Core 2.0+; unavailable on .NET Standard 2.0.</remarks>
    /// <param name="maxBytes">The maximum accepted bytes (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxBytes"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The action allocated more or threw.</exception>
    public AndConstraint<ActionAssertions<T>> AllocateAtMost(long maxBytes, string? because = null, params object?[] becauseArgs) {
        var allocated = ExceptionAssertionCore.MeasureAllocation(Invoke, maxBytes, StringKeys.ACTION_ALLOCATE_AT_MOST_THREW, Expression, because, becauseArgs);

        if(allocated > maxBytes) {
            Fail(Strings.Format(StringKeys.ACTION_ALLOCATE_AT_MOST, Expression, Allocation.FormatBytes(maxBytes), Allocation.FormatBytes(allocated)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the synchronous action allocates less than <paramref name="maxBytes"/> on the current thread.</summary>
    /// <remarks>Requires .NET Core 2.0+; unavailable on .NET Standard 2.0.</remarks>
    /// <param name="maxBytes">The exclusive upper bound in bytes (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxBytes"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The action allocated too much or threw.</exception>
    public AndConstraint<ActionAssertions<T>> AllocateLessThan(long maxBytes, string? because = null, params object?[] becauseArgs) {
        var allocated = ExceptionAssertionCore.MeasureAllocation(Invoke, maxBytes, StringKeys.ACTION_ALLOCATE_LESS_THAN_THREW, Expression, because, becauseArgs);

        if(allocated >= maxBytes) {
            Fail(Strings.Format(StringKeys.ACTION_ALLOCATE_LESS_THAN, Expression, Allocation.FormatBytes(maxBytes), Allocation.FormatBytes(allocated)), because, becauseArgs);
        }

        return new(this);
    }
#endif
}
