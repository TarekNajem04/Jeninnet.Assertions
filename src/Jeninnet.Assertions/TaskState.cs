//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Shared task-state inspection used by task assertion classes.</summary>
internal static class TaskState {
    /// <summary>Determines whether a task ran to completion without faulting or cancellation.</summary>
    /// <param name="task">The task to inspect.</param>
    /// <returns><see langword="true"/> when <see cref="Task.Status"/> is <see cref="TaskStatus.RanToCompletion"/>.</returns>
    internal static bool IsCompletedSuccessfully(Task task) => task.Status == TaskStatus.RanToCompletion;

    /// <summary>Gets the root failure exception of a faulted task.</summary>
    /// <param name="task">The task to inspect.</param>
    /// <returns>The base exception, or <see langword="null"/> when the task is not faulted.</returns>
    internal static Exception? GetFailure(Task task) => task.Exception?.GetBaseException();

    /// <summary>Describes a task state for failure messages, including the failure type when faulted.</summary>
    /// <param name="task">The task to describe.</param>
    /// <returns>The status name, or <c>Faulted (ExceptionType)</c> with the base exception type.</returns>
    internal static string Describe(Task task) =>
        task.Status == TaskStatus.Faulted && task.Exception is { } exception
            ? $"Faulted ({exception.GetBaseException().GetType().Name})"
            : AssertionScope.FormatInvariant(task.Status);
}
