//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>The outcome of one concurrency stress run.</summary>
/// <param name="Failures">The copies that threw, with their copy numbers.</param>
/// <param name="Pending">The copies still running when the timeout elapsed.</param>
internal readonly record struct ConcurrencyStressResult(List<(int Copy, Exception Exception)> Failures, int Pending);

/// <summary>Shared barrier-based concurrency stress runner for assertion classes.</summary>
/// <remarks>
/// A diagnostic check, not a proof. Copies start together behind a <see cref="Barrier"/>; failures are
/// aggregated by type/message, and copies that outlive the timeout are reported as a possible deadlock.
/// Stuck copies cannot be interrupted and are abandoned until the process exits.
/// </remarks>
internal static class ConcurrencyStress {
    /// <summary>Validates concurrency arguments and resolves the default timeout.</summary>
    /// <param name="copies">The number of simultaneous copies (at least two).</param>
    /// <param name="timeout">The optional timeout (defaults to five seconds; must not be negative).</param>
    /// <returns>The effective timeout.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="copies"/> is less than two or <paramref name="timeout"/> is negative.</exception>
    internal static TimeSpan Validate(int copies, TimeSpan? timeout) {
        if(copies < 2) {
            throw new ArgumentOutOfRangeException(nameof(copies), "At least two copies are required.");
        }

        var wait = timeout ?? TimeSpan.FromSeconds(5);
        if(wait < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must not be negative.");
        }

        return wait;
    }

    /// <summary>Runs <paramref name="action"/> in simultaneous copies behind a barrier.</summary>
    /// <param name="action">The asynchronous action each copy invokes.</param>
    /// <param name="copies">The number of simultaneous copies (already validated).</param>
    /// <param name="timeout">The completion window (already validated).</param>
    /// <returns>The collected failures and the pending count when the timeout elapsed.</returns>
    internal static async Task<ConcurrencyStressResult> RunAsync(Func<Task> action, int copies, TimeSpan timeout) {
        var failures = new List<(int Copy, Exception Exception)>();
        using var barrier = new Barrier(copies);
        var tasks = new Task[copies];
        for(var index = 0; index < copies; index++) {
            var copy = index + 1;
            tasks[index] = Task.Run(async () => {
                barrier.SignalAndWait();
                try {
                    await action().ConfigureAwait(false);
                }
                catch(Exception exception) {
                    lock(failures) {
                        failures.Add((copy, exception));
                    }
                }
            });
        }

        var all = Task.WhenAll(tasks);
#pragma warning disable CA2027 // netstandard2.0 has no Task.WaitAsync; the delay is canceled as soon as the race settles.
        using var timeoutCancellation = new CancellationTokenSource();
        var winner = await Task.WhenAny(all, Task.Delay(timeout, timeoutCancellation.Token)).ConfigureAwait(false);
#if NET8_0_OR_GREATER
        await timeoutCancellation.CancelAsync().ConfigureAwait(false);
#else
        timeoutCancellation.Cancel();
#endif
#pragma warning restore CA2027
        if(winner != all) {
            return new ConcurrencyStressResult(failures, tasks.Count(static task => !task.IsCompleted));
        }

        for(var index = 0; index < tasks.Length; index++) {
            if(tasks[index].IsFaulted) {
                failures.Add((index + 1, tasks[index].Exception!.GetBaseException()));
            }
        }

        return new ConcurrencyStressResult(failures, 0);
    }

    /// <summary>Reports a concurrency result through the caller's failure sink.</summary>
    /// <param name="result">The completed stress run.</param>
    /// <param name="wait">The timeout used for the run.</param>
    /// <param name="copies">The number of simultaneous copies.</param>
    /// <param name="expression">The subject expression.</param>
    /// <param name="fail">The assertion failure callback.</param>
    internal static void ReportThreadSafe(ConcurrencyStressResult result, TimeSpan wait, int copies, string expression, Action<string> fail) {
        if(result.Pending != 0) {
            fail(Strings.Format(StringKeys.ACTION_THREAD_SAFE_TIMEOUT, expression, AssertionScope.FormatInvariant(result.Pending), AssertionScope.FormatInvariant(copies), AssertionScope.FormatInvariant(wait)));
            return;
        }

        if(result.Failures.Count != 0) {
            fail(BuildFailureMessage(expression, result.Failures, copies));
        }
    }

    /// <summary>Builds the aggregated failure message for concurrent invocation failures, grouping identical exceptions.</summary>
    /// <param name="expression">The subject expression.</param>
    /// <param name="failures">The failed copies with their exceptions.</param>
    /// <param name="copies">The total number of simultaneous copies.</param>
    /// <returns>The localized multi-line report with one entry per distinct exception.</returns>
    internal static string BuildFailureMessage(string expression, IReadOnlyList<(int Copy, Exception Exception)> failures, int copies) {
        var builder = new StringBuilder();
        builder.Append(Strings.Format(StringKeys.ACTION_THREAD_SAFE_FAILURES, expression, AssertionScope.FormatInvariant(failures.Count), AssertionScope.FormatInvariant(copies)));
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
}
