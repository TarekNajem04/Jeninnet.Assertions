//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Shared waiting and timeout plumbing for the task assertion classes.</summary>
internal static class TaskAssertionHelpers {
    /// <summary>Rejects negative timeouts before any waiting starts.</summary>
    /// <param name="timeout">The timeout to validate.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is negative.</exception>
    internal static void ValidateTimeout(TimeSpan timeout) {
        if(timeout < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must not be negative.");
        }
    }

    /// <summary>Determines whether a task completes before a delay of the given duration elapses.</summary>
    /// <param name="task">The task to observe.</param>
    /// <param name="timeout">The observation window.</param>
    /// <returns><see langword="true"/> when the task completed first; the delay itself is never canceled.</returns>
    internal static async Task<bool> CompletesWithinAsync(Task task, TimeSpan timeout) {
        var completed = await Task.WhenAny(task, Task.Delay(timeout));
        return ReferenceEquals(completed, task);
    }

    /// <summary>Waits for a task up to the timeout, failing with a preserved timeout exception when it does not complete.</summary>
    /// <param name="task">The task to wait for.</param>
    /// <param name="timeout">The maximum accepted wait (already validated as non-negative).</param>
    /// <param name="expression">The subject expression used in the failure message.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A task that completes when the subject task completes or the wait fails.</returns>
    /// <exception cref="AssertionFailedException">The task did not complete within <paramref name="timeout"/>.</exception>
    internal static async Task WaitForCompletionAsync(Task task, TimeSpan timeout, string expression, string? because, object?[]? becauseArgs) {
        if(task.IsCompleted) {
            return;
        }

        if(!await CompletesWithinAsync(task, timeout)) {
            throw new AssertionFailedException(
                AssertionScope.AppendBecause(
                    Strings.Format(StringKeys.TASK_COMPLETE_WITHIN, expression, AssertionScope.FormatInvariant(timeout)),
                    because,
                    becauseArgs),
                new TimeoutException($"The task did not complete within {timeout}."));
        }
    }
}
