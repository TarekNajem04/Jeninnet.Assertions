//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Utilities;

/// <summary>Retries a delegate until it succeeds or a timeout elapses (for eventually-consistent scenarios).</summary>
internal static class Polling {
    /// <summary>Invokes <paramref name="action"/> until it completes without throwing or <paramref name="timeout"/> elapses.</summary>
    /// <param name="action">The delegate to retry.</param>
    /// <param name="timeout">The total budget.</param>
    /// <param name="backoff">The delay schedule between attempts.</param>
    /// <returns>The last exception, or <see langword="null"/> on success.</returns>
    internal static Exception? TrySucceed(Action action, TimeSpan timeout, PollingBackoff backoff) {
        var stopwatch = Stopwatch.StartNew();
        var attempt = 0;
        while(true) {
            try {
                action();
                return null;
            }
            catch(Exception exception) {
                if(stopwatch.Elapsed >= timeout) {
                    return exception;
                }
            }

            var remaining = timeout - stopwatch.Elapsed;
            var delay = backoff.DelayFor(attempt++);
            Thread.Sleep(remaining < delay ? remaining : delay);
        }
    }

    /// <summary>Invokes <paramref name="action"/> until it completes without throwing or <paramref name="timeout"/> elapses.</summary>
    /// <param name="action">The delegate to retry.</param>
    /// <param name="timeout">The total budget.</param>
    /// <param name="backoff">The delay schedule between attempts.</param>
    /// <returns>The last exception, or <see langword="null"/> on success.</returns>
    internal static async Task<Exception?> TrySucceedAsync(Func<Task> action, TimeSpan timeout, PollingBackoff backoff) {
        var stopwatch = Stopwatch.StartNew();
        var attempt = 0;
        while(true) {
            try {
                await action();
                return null;
            }
            catch(Exception exception) {
                if(stopwatch.Elapsed >= timeout) {
                    return exception;
                }
            }

            var remaining = timeout - stopwatch.Elapsed;
            var delay = backoff.DelayFor(attempt++);
            await Task.Delay(remaining < delay ? remaining : delay);
        }
    }

    /// <summary>Awaits one invocation until it throws, completes quietly, or exceeds <paramref name="timeout"/>.</summary>
    /// <param name="action">The delegate to invoke once.</param>
    /// <param name="timeout">The maximum time to wait for the invocation to settle.</param>
    /// <returns>The outcome, including a timeout marker or the thrown exception.</returns>
    internal static async Task<ThrowAttempt> TryThrowAsync(Func<Task> action, TimeSpan timeout) {
        Task task;
        try {
            task = action();
        }
        catch(Exception exception) {
            return new ThrowAttempt(false, exception);
        }

        if(task.IsCompleted) {
            try {
                await task;
                return new ThrowAttempt(false, null);
            }
            catch(Exception exception) {
                return new ThrowAttempt(false, exception);
            }
        }

        var completed = await Task.WhenAny(task, Task.Delay(timeout));
        if(!ReferenceEquals(completed, task)) {
            return new ThrowAttempt(true, null);
        }

        try {
            await task;
            return new ThrowAttempt(false, null);
        }
        catch(Exception exception) {
            return new ThrowAttempt(false, exception);
        }
    }

    /// <summary>Re-invokes <paramref name="source"/> until <paramref name="predicate"/> accepts a produced value or <paramref name="timeout"/> elapses.</summary>
    /// <typeparam name="T">The polled value type.</typeparam>
    /// <param name="source">The delegate producing a fresh value on every attempt.</param>
    /// <param name="predicate">The condition a produced value must satisfy.</param>
    /// <param name="timeout">The total budget.</param>
    /// <param name="backoff">The delay schedule between attempts.</param>
    /// <returns>The outcome, including the last value and error when the predicate never held.</returns>
    /// <remarks>Delegates that throw are treated like mismatches and retried; the last error is kept for diagnosis.</remarks>
    internal static PollResult<T> TrySatisfy<T>(Func<T> source, Func<T, bool> predicate, TimeSpan timeout, PollingBackoff backoff) {
        var stopwatch = Stopwatch.StartNew();
        Exception? error = null;
        var hasValue = false;
        T? value = default;
        var attempt = 0;
        while(true) {
            try {
                value = source();
                hasValue = true;
                if(predicate(value)) {
                    return new PollResult<T>(true, value, true, null);
                }
            }
            catch(Exception exception) {
                error = exception;
            }

            if(stopwatch.Elapsed >= timeout) {
                return new PollResult<T>(false, value, hasValue, error);
            }

            var remaining = timeout - stopwatch.Elapsed;
            var delay = backoff.DelayFor(attempt++);
            Thread.Sleep(remaining < delay ? remaining : delay);
        }
    }

    /// <summary>Re-invokes <paramref name="source"/> until <paramref name="predicate"/> accepts a produced value or <paramref name="timeout"/> elapses.</summary>
    /// <typeparam name="T">The polled value type.</typeparam>
    /// <param name="source">The delegate producing a fresh value on every attempt.</param>
    /// <param name="predicate">The condition a produced value must satisfy.</param>
    /// <param name="timeout">The total budget.</param>
    /// <param name="backoff">The delay schedule between attempts.</param>
    /// <param name="cancellationToken">Cancels the wait; cancellation always propagates instead of being retried.</param>
    /// <returns>The outcome, including the last value and error when the predicate never held.</returns>
    /// <remarks>Delegates that throw are treated like mismatches and retried; the last error is kept for diagnosis.</remarks>
    /// <exception cref="OperationCanceledException">Cancellation was requested.</exception>
    internal static async Task<PollResult<T>> TrySatisfyAsync<T>(Func<Task<T>> source, Func<T, bool> predicate, TimeSpan timeout, PollingBackoff backoff, CancellationToken cancellationToken) {
        var stopwatch = Stopwatch.StartNew();
        Exception? error = null;
        var hasValue = false;
        T? value = default;
        var attempt = 0;
        while(true) {
            if(cancellationToken.IsCancellationRequested) {
                throw new OperationCanceledException(cancellationToken);
            }

            try {
                value = await source();
                hasValue = true;
                if(predicate(value)) {
                    return new PollResult<T>(true, value, true, null);
                }
            }
            catch(Exception exception) when(exception is not OperationCanceledException) {
                error = exception;
            }

            if(stopwatch.Elapsed >= timeout) {
                return new PollResult<T>(false, value, hasValue, error);
            }

            var remaining = timeout - stopwatch.Elapsed;
            var delay = backoff.DelayFor(attempt++);
            await Task.Delay(remaining < delay ? remaining : delay, cancellationToken);
        }
    }
}

/// <summary>The outcome of awaiting one asynchronous invocation for an exception.</summary>
internal sealed class ThrowAttempt {
    /// <summary>Creates a throw-attempt outcome.</summary>
    /// <param name="timedOut"><see langword="true"/> when the invocation did not settle within the timeout.</param>
    /// <param name="exception">The thrown exception, or <see langword="null"/> when nothing was thrown.</param>
    internal ThrowAttempt(bool timedOut, Exception? exception) {
        TimedOut = timedOut;
        Exception = exception;
    }

    /// <summary>Gets whether the invocation did not settle within the timeout.</summary>
    internal bool TimedOut { get; }

    /// <summary>Gets the thrown exception, or <see langword="null"/> when nothing was thrown.</summary>
    internal Exception? Exception { get; }
}

/// <summary>The outcome of polling a value source until a predicate is satisfied.</summary>
/// <typeparam name="T">The polled value type.</typeparam>
internal sealed class PollResult<T> {
    /// <summary>Creates a polling outcome.</summary>
    /// <param name="satisfied"><see langword="true"/> when the predicate accepted a produced value.</param>
    /// <param name="value">The last produced value (default when nothing was ever produced).</param>
    /// <param name="hasValue"><see langword="true"/> when at least one value was produced.</param>
    /// <param name="error">The last delegate error (or <see langword="null"/> when none occurred).</param>
    internal PollResult(bool satisfied, T? value, bool hasValue, Exception? error) {
        Satisfied = satisfied;
        Value = value;
        HasValue = hasValue;
        Error = error;
    }

    /// <summary>Gets whether the predicate accepted a produced value.</summary>
    internal bool Satisfied { get; }

    /// <summary>Gets the last produced value (default when nothing was ever produced).</summary>
    internal T? Value { get; }

    /// <summary>Gets whether at least one value was produced.</summary>
    internal bool HasValue { get; }

    /// <summary>Gets the last delegate error (or <see langword="null"/> when none occurred).</summary>
    internal Exception? Error { get; }
}
