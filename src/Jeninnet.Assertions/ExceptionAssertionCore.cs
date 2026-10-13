//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Shared exception and polling logic for the synchronous and asynchronous delegate assertion classes.</summary>
internal static class ExceptionAssertionCore {
    /// <summary>Invokes a delegate and returns the caught exception, translating mismatches to assertion failures.</summary>
    /// <typeparam name="TException">The expected exception type.</typeparam>
    /// <param name="invoke">The delegate to invoke.</param>
    /// <param name="expression">The subject expression used in the failure message.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>The caught exception.</returns>
    /// <exception cref="AssertionFailedException">No exception was thrown, or a different exception type was thrown.</exception>
    internal static Exception Throw<TException>(Func<object?> invoke, string expression, string? because, object?[]? becauseArgs) where TException : Exception {
        try {
            _ = invoke();
        }
        catch(TException ex) {
            return ex;
        }
        catch(Exception ex) {
            throw new AssertionFailedException(
                AssertionScope.AppendBecause(Strings.Format(StringKeys.ACTION_THROW_WRONG, expression, typeof(TException).Name, ex.GetType().Name), because, becauseArgs),
                ex);
        }

        throw new AssertionFailedException(
            AssertionScope.AppendBecause(Strings.Format(StringKeys.ACTION_THROW_NONE, expression, typeof(TException).Name), because, becauseArgs));
    }

    /// <summary>Invokes a delegate and returns the caught exception, requiring the exact exception type.</summary>
    /// <typeparam name="TException">The expected exact exception type.</typeparam>
    /// <param name="invoke">The delegate to invoke.</param>
    /// <param name="expression">The subject expression used in the failure message.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>The caught exception.</returns>
    /// <exception cref="AssertionFailedException">No exception was thrown, or the exception type is not exactly <typeparamref name="TException"/>.</exception>
    internal static Exception ThrowExactly<TException>(Func<object?> invoke, string expression, string? because, object?[]? becauseArgs) where TException : Exception {
        Exception? thrown = null;
        try {
            _ = invoke();
        }
        catch(TException ex) {
            if(ex.GetType() == typeof(TException)) {
                return ex;
            }

            thrown = ex;
        }
        catch(Exception ex) {
            thrown = ex;
        }

        throw ExactFailure<TException>(thrown, expression, because, becauseArgs);
    }

    /// <summary>Builds the failure for a missing or inexact exception in the <c>ThrowExactly</c> assertions.</summary>
    /// <typeparam name="TException">The expected exact exception type.</typeparam>
    /// <param name="thrown">The thrown exception, or <see langword="null"/> when nothing was thrown.</param>
    /// <param name="expression">The subject expression used in the failure message.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>The assertion failure to throw.</returns>
    private static AssertionFailedException ExactFailure<TException>(Exception? thrown, string expression, string? because, object?[]? becauseArgs) where TException : Exception =>
        thrown is null
            ? new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.ACTION_THROW_EXACTLY_NONE, expression, typeof(TException).Name), because, becauseArgs))
            : new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.ACTION_THROW_EXACTLY_WRONG, expression, typeof(TException).Name, thrown.GetType().Name), because, becauseArgs), thrown);

    /// <summary>Invokes an asynchronous delegate and returns the caught exception, translating mismatches to assertion failures.</summary>
    /// <typeparam name="TException">The expected exception type.</typeparam>
    /// <param name="action">The asynchronous delegate to invoke.</param>
    /// <param name="expression">The subject expression used in the failure message.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>The caught exception.</returns>
    /// <exception cref="AssertionFailedException">No exception was thrown, or a different exception type was thrown.</exception>
    internal static async Task<Exception> ThrowAsync<TException>(Func<Task> action, string expression, string? because, object?[]? becauseArgs) where TException : Exception {
        try {
            await action();
        }
        catch(TException ex) {
            return ex;
        }
        catch(Exception ex) {
            throw new AssertionFailedException(
                AssertionScope.AppendBecause(Strings.Format(StringKeys.ACTION_THROW_WRONG, expression, typeof(TException).Name, ex.GetType().Name), because, becauseArgs),
                ex);
        }

        throw new AssertionFailedException(
            AssertionScope.AppendBecause(Strings.Format(StringKeys.ACTION_THROW_NONE, expression, typeof(TException).Name), because, becauseArgs));
    }

    /// <summary>Invokes an asynchronous delegate and returns the caught exception, requiring the exact exception type.</summary>
    /// <typeparam name="TException">The expected exact exception type.</typeparam>
    /// <param name="action">The asynchronous delegate to invoke.</param>
    /// <param name="expression">The subject expression used in the failure message.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>The caught exception.</returns>
    /// <exception cref="AssertionFailedException">No exception was thrown, or the exception type is not exactly <typeparamref name="TException"/>.</exception>
    internal static async Task<Exception> ThrowExactlyAsync<TException>(Func<Task> action, string expression, string? because, object?[]? becauseArgs) where TException : Exception {
        Exception? thrown = null;
        try {
            await action();
        }
        catch(TException ex) {
            if(ex.GetType() == typeof(TException)) {
                return ex;
            }

            thrown = ex;
        }
        catch(Exception ex) {
            thrown = ex;
        }

        throw ExactFailure<TException>(thrown, expression, because, becauseArgs);
    }

    /// <summary>Invokes a delegate and returns the thrown exception, or <see langword="null"/> when it completed cleanly.</summary>
    /// <param name="invoke">The delegate to invoke.</param>
    /// <returns>The thrown exception, or <see langword="null"/>.</returns>
    internal static Exception? NotThrow(Func<object?> invoke) {
        try {
            _ = invoke();
        }
        catch(Exception ex) {
            return ex;
        }

        return null;
    }

    /// <summary>Invokes an asynchronous delegate and returns the thrown exception, or <see langword="null"/> when it completed cleanly.</summary>
    /// <param name="action">The asynchronous delegate to invoke.</param>
    /// <returns>The thrown exception, or <see langword="null"/>.</returns>
    internal static async Task<Exception?> NotThrowAsync(Func<Task> action) {
        try {
            await action();
        }
        catch(Exception ex) {
            return ex;
        }

        return null;
    }

    /// <summary>Invokes an asynchronous delegate until it throws within the timeout and returns the caught exception.</summary>
    /// <typeparam name="TException">The expected exception type.</typeparam>
    /// <param name="action">The asynchronous delegate to invoke.</param>
    /// <param name="timeout">The maximum time to wait for an exception (must not be negative).</param>
    /// <param name="expression">The subject expression used in the failure message.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>The caught exception.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">No matching exception was thrown before the timeout elapsed.</exception>
    internal static async Task<Exception> ThrowWithinAsync<TException>(Func<Task> action, TimeSpan timeout, string expression, string? because, object?[]? becauseArgs) where TException : Exception {
        if(timeout < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must not be negative.");
        }

        var attempt = await Polling.TryThrowAsync(action, timeout);
        if(attempt.TimedOut) {
            throw new AssertionFailedException(
                AssertionScope.AppendBecause(Strings.Format(StringKeys.ACTION_THROW_WITHIN_TIMEOUT, expression, typeof(TException).Name, AssertionScope.FormatInvariant(timeout)), because, becauseArgs),
                new TimeoutException($"The action did not throw within {timeout}."));
        }

        if(attempt.Exception is not TException match) {
            if(attempt.Exception is null) {
                throw new AssertionFailedException(
                    AssertionScope.AppendBecause(Strings.Format(StringKeys.ACTION_THROW_WITHIN_NONE, expression, typeof(TException).Name, AssertionScope.FormatInvariant(timeout)), because, becauseArgs));
            }

            throw new AssertionFailedException(
                AssertionScope.AppendBecause(Strings.Format(StringKeys.ACTION_THROW_WITHIN_WRONG, expression, typeof(TException).Name, AssertionScope.FormatInvariant(timeout), attempt.Exception.GetType().Name), because, becauseArgs),
                attempt.Exception);
        }

        return match;
    }

    /// <summary>Retries a delegate until it stops throwing, returning the last thrown exception when it never does.</summary>
    /// <param name="invoke">The delegate to invoke.</param>
    /// <param name="waitTime">The time after which the delegate should have stopped throwing (must not be negative).</param>
    /// <param name="pollInterval">The pause between attempts (must not be negative).</param>
    /// <returns>The last thrown exception, or <see langword="null"/> when the delegate completed cleanly.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="waitTime"/> or <paramref name="pollInterval"/> is negative.</exception>
    internal static Exception? NotThrowAfter(Func<object?> invoke, TimeSpan waitTime, TimeSpan pollInterval) {
        if(waitTime < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(waitTime), "Wait time must not be negative.");
        }

        if(pollInterval < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(pollInterval), "Poll interval must not be negative.");
        }

        return Polling.TrySucceed(() => invoke(), waitTime, PollingBackoff.Constant(pollInterval));
    }

    /// <summary>Retries an asynchronous delegate until it stops throwing, returning the last thrown exception when it never does.</summary>
    /// <param name="action">The asynchronous delegate to invoke.</param>
    /// <param name="waitTime">The time after which the delegate should have stopped throwing (must not be negative).</param>
    /// <param name="pollInterval">The pause between attempts (must not be negative).</param>
    /// <returns>The last thrown exception, or <see langword="null"/> when the delegate completed cleanly.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="waitTime"/> or <paramref name="pollInterval"/> is negative.</exception>
    internal static async Task<Exception?> NotThrowAfterAsync(Func<Task> action, TimeSpan waitTime, TimeSpan pollInterval) {
        if(waitTime < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(waitTime), "Wait time must not be negative.");
        }

        if(pollInterval < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(pollInterval), "Poll interval must not be negative.");
        }

        return await Polling.TrySucceedAsync(action, waitTime, PollingBackoff.Constant(pollInterval));
    }

    /// <summary>Retries a delegate until it succeeds within the timeout, returning the last thrown exception when it never does.</summary>
    /// <param name="invoke">The delegate to invoke.</param>
    /// <param name="backoff">The delay schedule between attempts (must not be <see langword="null"/>).</param>
    /// <param name="timeout">The total retry budget (must not be negative).</param>
    /// <returns>The last thrown exception, or <see langword="null"/> when the delegate completed cleanly.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="backoff"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is negative.</exception>
    internal static Exception? SucceedWithin(Func<object?> invoke, PollingBackoff backoff, TimeSpan timeout) {
        if(backoff is null) {
            throw new ArgumentNullException(nameof(backoff));
        }

        if(timeout < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must not be negative.");
        }

        return Polling.TrySucceed(() => invoke(), timeout, backoff);
    }

    /// <summary>Retries an asynchronous delegate until it succeeds within the timeout, returning the last thrown exception when it never does.</summary>
    /// <param name="action">The asynchronous delegate to invoke.</param>
    /// <param name="backoff">The delay schedule between attempts (must not be <see langword="null"/>).</param>
    /// <param name="timeout">The total retry budget (must not be negative).</param>
    /// <returns>The last thrown exception, or <see langword="null"/> when the delegate completed cleanly.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="backoff"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is negative.</exception>
    internal static async Task<Exception?> SucceedWithinAsync(Func<Task> action, PollingBackoff backoff, TimeSpan timeout) {
        if(backoff is null) {
            throw new ArgumentNullException(nameof(backoff));
        }

        if(timeout < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must not be negative.");
        }

        return await Polling.TrySucceedAsync(action, timeout, backoff);
    }

#if !NETSTANDARD2_0
    /// <summary>Measures the bytes a delegate allocates, translating a thrown exception to an assertion failure.</summary>
    /// <param name="invoke">The delegate to measure.</param>
    /// <param name="maxBytes">The maximum accepted bytes (must not be negative).</param>
    /// <param name="threwKey">The localized message template key for the throwing case.</param>
    /// <param name="expression">The subject expression used in the failure message.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>The allocated byte count.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxBytes"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The delegate threw while being measured.</exception>
    internal static long MeasureAllocation(Func<object?> invoke, long maxBytes, string threwKey, string expression, string? because, object?[]? becauseArgs) {
        if(maxBytes < 0) {
            throw new ArgumentOutOfRangeException(nameof(maxBytes), "Maximum bytes must not be negative.");
        }

        try {
            return Allocation.Measure(() => invoke());
        }
        catch(Exception ex) {
            throw new AssertionFailedException(
                AssertionScope.AppendBecause(Strings.Format(threwKey, expression, Allocation.FormatBytes(maxBytes), ex.GetType().Name), because, becauseArgs),
                ex);
        }
    }

    /// <summary>Measures the bytes an asynchronous delegate allocates, translating a thrown exception to an assertion failure.</summary>
    /// <param name="action">The asynchronous delegate to measure.</param>
    /// <param name="maxBytes">The maximum accepted bytes (must not be negative).</param>
    /// <param name="threwKey">The localized message template key for the throwing case.</param>
    /// <param name="expression">The subject expression used in the failure message.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>The allocated byte count.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxBytes"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The delegate threw while being measured.</exception>
    internal static async Task<long> MeasureAllocationAsync(Func<Task> action, long maxBytes, string threwKey, string expression, string? because, object?[]? becauseArgs) {
        if(maxBytes < 0) {
            throw new ArgumentOutOfRangeException(nameof(maxBytes), "Maximum bytes must not be negative.");
        }

        try {
            return await Allocation.MeasureAsync(action);
        }
        catch(Exception ex) {
            throw new AssertionFailedException(
                AssertionScope.AppendBecause(Strings.Format(threwKey, expression, Allocation.FormatBytes(maxBytes), ex.GetType().Name), because, becauseArgs),
                ex);
        }
    }
#endif
}
