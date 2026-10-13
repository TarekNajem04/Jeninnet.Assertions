//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying that synchronous delegates throw expected exceptions or complete without throwing.</summary>
/// <param name="action">The synchronous delegate to invoke and verify.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
/// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
public class ActionAssertions(Action action, string? expression = null) : AssertionsBase(expression, "action") {
    private readonly Action _action = action ?? throw new ArgumentNullException(nameof(action));

    /// <summary>Asserts that the action throws an exception of type <typeparamref name="TException"/>.</summary>
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

    /// <summary>Asserts that the action throws an exception of exactly type <typeparamref name="TException"/> (derived types fail).</summary>
    /// <typeparam name="TException">The expected exact exception type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="ExceptionAssertions{TException}"/> that provides access to the caught exception.</returns>
    /// <exception cref="AssertionFailedException">No exception was thrown, or the exception type is not exactly <typeparamref name="TException"/>.</exception>
    public ExceptionAssertions<TException> ThrowExactly<TException>(string? because = null, params object?[] becauseArgs) where TException : Exception =>
        new((TException)ExceptionAssertionCore.ThrowExactly<TException>(Invoke, Expression, because, becauseArgs));

    /// <summary>Asserts that the action completes without throwing any exception.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The action threw an exception of any type.</exception>
    public AndConstraint<ActionAssertions> NotThrow(string? because = null, params object?[] becauseArgs) {
        var thrown = ExceptionAssertionCore.NotThrow(Invoke);
        if(thrown is not null) {
            Fail(Strings.Format(StringKeys.ACTION_NOT_THROW, Expression, thrown.GetType().Name), because, becauseArgs, thrown);
        }

        return new(this);
    }

    /// <summary>Invokes the action for the shared exception assertion core, discarding any result.</summary>
    /// <returns>Always <see langword="null"/>.</returns>
    private object? Invoke() {
        _action();
        return null;
    }

    /// <summary>Asserts that the action is safe to run concurrently: <paramref name="copies"/> simultaneous copies start together behind a barrier and every copy must finish cleanly within <paramref name="timeout"/>.</summary>
    /// <remarks>
    /// A diagnostic stress check, not a proof: it reliably surfaces crashes and hangs under simultaneous
    /// execution. All failures are aggregated into one message grouped by exception type and message with the
    /// failing copy numbers, and copies that never finish are reported as a possible deadlock. Copies run on
    /// thread-pool threads; when the timeout elapses the stuck copies cannot be interrupted and are abandoned
    /// until the process exits.
    /// </remarks>
    /// <param name="copies">The number of simultaneous copies to start together (at least two).</param>
    /// <param name="timeout">The maximum accepted completion time for every copy, or <see langword="null"/> for 5 seconds (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="copies"/> is less than two or <paramref name="timeout"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">A copy failed or did not complete within <paramref name="timeout"/>.</exception>
    public async Task<AndConstraint<ActionAssertions>> BeThreadSafeAsync(int copies = 8, TimeSpan? timeout = null, string? because = null, params object?[] becauseArgs) {
        if(copies < 2) {
            throw new ArgumentOutOfRangeException(nameof(copies), "At least two copies are required.");
        }

        var wait = timeout ?? TimeSpan.FromSeconds(5);
        if(wait < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must not be negative.");
        }

        var failures = new List<(int Copy, Exception Exception)>();
        using var barrier = new Barrier(copies);
        var tasks = new Task[copies];
        for(var index = 0; index < copies; index++) {
            var copy = index + 1;
            tasks[index] = Task.Run(() => {
                barrier.SignalAndWait();
                try {
                    _action();
                }
                catch(Exception exception) {
                    lock(failures) {
                        failures.Add((copy, exception));
                    }
                }
            });
        }

        var all = Task.WhenAll(tasks);
        var winner = await Task.WhenAny(all, Task.Delay(wait));
        if(winner != all) {
            var pending = tasks.Count(static task => !task.IsCompleted);
            Fail(Strings.Format(StringKeys.ACTION_THREAD_SAFE_TIMEOUT, Expression, AssertionScope.FormatInvariant(pending), AssertionScope.FormatInvariant(copies), AssertionScope.FormatInvariant(wait)), because, becauseArgs);
            return new(this);
        }

        for(var index = 0; index < tasks.Length; index++) {
            if(tasks[index].IsFaulted) {
                failures.Add((index + 1, tasks[index].Exception!.GetBaseException()));
            }
        }

        if(failures.Count != 0) {
            Fail(BuildThreadSafetyMessage(failures, copies), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Builds the aggregated failure message for concurrent invocation failures, grouping identical exceptions.</summary>
    /// <param name="failures">The failed copies with their exceptions.</param>
    /// <param name="copies">The total number of simultaneous copies.</param>
    /// <returns>The localized multi-line report with one entry per distinct exception.</returns>
    private string BuildThreadSafetyMessage(List<(int Copy, Exception Exception)> failures, int copies) {
        var builder = new StringBuilder();
        builder.Append(Strings.Format(StringKeys.ACTION_THREAD_SAFE_FAILURES, Expression, AssertionScope.FormatInvariant(failures.Count), AssertionScope.FormatInvariant(copies)));
        var entry = 0;
        foreach(var group in failures
            .GroupBy(static failure => (failure.Exception.GetType().Name, failure.Exception.Message))
            .OrderBy(static group => group.Min(static failure => failure.Copy))) {
            entry++;
            var type = group.Key.Name;
            var detail = string.IsNullOrEmpty(group.Key.Message) ? type : $"{type}: {group.Key.Message}";
            var copiesList = string.Join(", ", group.OrderBy(static failure => failure.Copy).Select(static failure => AssertionScope.FormatInvariant(failure.Copy)));
            builder.Append('\n').Append(Strings.Format(StringKeys.ACTION_THREAD_SAFE_ENTRY, AssertionScope.FormatInvariant(entry), detail, copiesList));
        }

        return builder.ToString();
    }

    /// <summary>Asserts that repeated invocations stop throwing within <paramref name="waitTime"/>, checking every <paramref name="pollInterval"/>.</summary>
    /// <remarks>This compatibility assertion only waits for quiet behavior. Prefer <see cref="SucceedWithin(TimeSpan, TimeSpan?, string?, object?[])"/> for transient failures because it reports the final failure and its retry semantics.</remarks>
    /// <param name="waitTime">The time after which the action should have stopped throwing (must not be negative).</param>
    /// <param name="pollInterval">The pause between attempts (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="waitTime"/> or <paramref name="pollInterval"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The action was still throwing when the wait elapsed.</exception>
    public AndConstraint<ActionAssertions> NotThrowAfter(TimeSpan waitTime, TimeSpan pollInterval, string? because = null, params object?[] becauseArgs) {
        var failure = ExceptionAssertionCore.NotThrowAfter(Invoke, waitTime, pollInterval);
        if(failure is not null) {
            throw new AssertionFailedException(
                AssertionScope.AppendBecause(Strings.Format(StringKeys.ACTION_NOT_THROW_AFTER, Expression, AssertionScope.FormatInvariant(waitTime), failure.GetType().Name), because, becauseArgs),
                failure);
        }

        return new(this);
    }

    /// <summary>Asserts that the action eventually completes without throwing within <paramref name="timeout"/>, retrying every <paramref name="pollInterval"/> (for eventually-consistent scenarios).</summary>
    /// <param name="timeout">The total retry budget (must not be negative).</param>
    /// <param name="pollInterval">The pause between attempts (defaults to 50 ms; must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> or <paramref name="pollInterval"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The action kept throwing until the timeout elapsed.</exception>
    public AndConstraint<ActionAssertions> SucceedWithin(TimeSpan timeout, TimeSpan? pollInterval = null, string? because = null, params object?[] becauseArgs) {
        var interval = pollInterval ?? TimeSpan.FromMilliseconds(50);
        return SucceedWithin(PollingBackoff.Constant(interval), timeout, because, becauseArgs);
    }

    /// <summary>Asserts that the action eventually completes without throwing within <paramref name="timeout"/>, pausing between attempts according to <paramref name="backoff"/> (for eventually-consistent scenarios).</summary>
    /// <param name="backoff">The delay schedule between attempts (must not be <see langword="null"/>).</param>
    /// <param name="timeout">The total retry budget (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="backoff"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The action kept throwing until the timeout elapsed.</exception>
    public AndConstraint<ActionAssertions> SucceedWithin(PollingBackoff backoff, TimeSpan timeout, string? because = null, params object?[] becauseArgs) {
        var failure = ExceptionAssertionCore.SucceedWithin(Invoke, backoff, timeout);
        if(failure is not null) {
            throw new AssertionFailedException(
                AssertionScope.AppendBecause(Strings.Format(StringKeys.ACTION_SUCCEED_WITHIN, Expression, AssertionScope.FormatInvariant(timeout), failure.Message.TrimEnd('.')), because, becauseArgs),
                failure);
        }

        return new(this);
    }
#if !NETSTANDARD2_0

    /// <summary>Asserts that the action allocates at most <paramref name="maxBytes"/> on the current thread.</summary>
    /// <remarks>Requires .NET Core 2.0+; unavailable on .NET Standard 2.0.</remarks>
    /// <param name="maxBytes">The maximum accepted bytes (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxBytes"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The action allocated more or threw.</exception>
    public AndConstraint<ActionAssertions> AllocateAtMost(long maxBytes, string? because = null, params object?[] becauseArgs) {
        var allocated = ExceptionAssertionCore.MeasureAllocation(Invoke, maxBytes, StringKeys.ACTION_ALLOCATE_AT_MOST_THREW, Expression, because, becauseArgs);

        if(allocated > maxBytes) {
            Fail(Strings.Format(StringKeys.ACTION_ALLOCATE_AT_MOST, Expression, Allocation.FormatBytes(maxBytes), Allocation.FormatBytes(allocated)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the action allocates less than <paramref name="maxBytes"/> on the current thread.</summary>
    /// <remarks>Requires .NET Core 2.0+; unavailable on .NET Standard 2.0.</remarks>
    /// <param name="maxBytes">The exclusive upper bound in bytes (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxBytes"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The action allocated too much or threw.</exception>
    public AndConstraint<ActionAssertions> AllocateLessThan(long maxBytes, string? because = null, params object?[] becauseArgs) {
        var allocated = ExceptionAssertionCore.MeasureAllocation(Invoke, maxBytes, StringKeys.ACTION_ALLOCATE_LESS_THAN_THREW, Expression, because, becauseArgs);

        if(allocated >= maxBytes) {
            Fail(Strings.Format(StringKeys.ACTION_ALLOCATE_LESS_THAN, Expression, Allocation.FormatBytes(maxBytes), Allocation.FormatBytes(allocated)), because, becauseArgs);
        }

        return new(this);
    }
#endif
}
