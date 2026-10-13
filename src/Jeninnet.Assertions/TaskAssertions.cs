//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying that a <see cref="Task"/> completes in time.</summary>
/// <param name="task">The task to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
/// <exception cref="ArgumentNullException"><paramref name="task"/> is <see langword="null"/>.</exception>
public class TaskAssertions(Task? task, string? expression = null) : AssertionsBase(expression, "task") {
    private readonly Task _task = task ?? throw new ArgumentNullException(nameof(task));

    /// <summary>Asserts that the task completes within <paramref name="timeout"/>; faults and cancellations still fail, with the original exception preserved.</summary>
    /// <param name="timeout">The maximum accepted completion time (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A task whose result can continue the fluent chain.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The task did not complete in time, faulted, or was canceled.</exception>
    public async Task<AndConstraint<TaskAssertions>> CompleteWithinAsync(TimeSpan timeout, string? because = null, params object?[] becauseArgs) {
        TaskAssertionHelpers.ValidateTimeout(timeout);
        await TaskAssertionHelpers.WaitForCompletionAsync(_task, timeout, Expression, because, becauseArgs);

        try {
            await _task;
        }
        catch(Exception ex) {
            throw new AssertionFailedException(
                AssertionScope.AppendBecause(
                    Strings.Format(StringKeys.TASK_COMPLETE_SUCCESSFULLY, Expression, AssertionScope.FormatInvariant(timeout), ex.GetType().Name),
                    because,
                    becauseArgs),
                ex);
        }

        return new AndConstraint<TaskAssertions>(this);
    }

    /// <summary>Asserts that the task does not complete within <paramref name="timeout"/>.</summary>
    /// <param name="timeout">The time span to wait for completion (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A task whose result can continue the fluent chain.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The task completed within the time span.</exception>
    public async Task<AndConstraint<TaskAssertions>> NotCompleteWithinAsync(TimeSpan timeout, string? because = null, params object?[] becauseArgs) {
        TaskAssertionHelpers.ValidateTimeout(timeout);

        if(_task.IsCompleted || await TaskAssertionHelpers.CompletesWithinAsync(_task, timeout)) {
            throw new AssertionFailedException(
                AssertionScope.AppendBecause(
                    Strings.Format(StringKeys.TASK_NOT_COMPLETE_WITHIN, Expression, AssertionScope.FormatInvariant(timeout)),
                    because,
                    becauseArgs));
        }

        return new AndConstraint<TaskAssertions>(this);
    }
}

/// <summary>Provides assertion methods for verifying that a <see cref="Task{TResult}"/> completes in time and exposes its result.</summary>
/// <typeparam name="T">The type of the task result.</typeparam>
/// <param name="task">The task to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
/// <exception cref="ArgumentNullException"><paramref name="task"/> is <see langword="null"/>.</exception>
public class TaskAssertions<T>(Task<T>? task, string? expression = null) : AssertionsBase(expression, "task") {
    private readonly Task<T> _task = task ?? throw new ArgumentNullException(nameof(task));

    /// <summary>Asserts that the task completes within <paramref name="timeout"/> and exposes its result; faults and cancellations still fail, with the original exception preserved.</summary>
    /// <param name="timeout">The maximum accepted completion time (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A task whose result exposes the completed value through <c>Which</c>.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The task did not complete in time, faulted, or was canceled.</exception>
    public async Task<AndWhichConstraint<TaskAssertions<T>, T>> CompleteWithinAsync(TimeSpan timeout, string? because = null, params object?[] becauseArgs) {
        TaskAssertionHelpers.ValidateTimeout(timeout);
        await TaskAssertionHelpers.WaitForCompletionAsync(_task, timeout, Expression, because, becauseArgs);

        try {
            return new AndWhichConstraint<TaskAssertions<T>, T>(this, await _task, Expression);
        }
        catch(Exception ex) {
            throw new AssertionFailedException(
                AssertionScope.AppendBecause(
                    Strings.Format(StringKeys.TASK_COMPLETE_SUCCESSFULLY, Expression, AssertionScope.FormatInvariant(timeout), ex.GetType().Name),
                    because,
                    becauseArgs),
                ex);
        }
    }

    /// <summary>Asserts that the task does not complete within <paramref name="timeout"/>.</summary>
    /// <param name="timeout">The time span to wait for completion (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A task whose result can continue the fluent chain.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The task completed within the time span.</exception>
    public async Task<AndConstraint<TaskAssertions<T>>> NotCompleteWithinAsync(TimeSpan timeout, string? because = null, params object?[] becauseArgs) {
        TaskAssertionHelpers.ValidateTimeout(timeout);

        if(_task.IsCompleted || await TaskAssertionHelpers.CompletesWithinAsync(_task, timeout)) {
            throw new AssertionFailedException(
                AssertionScope.AppendBecause(
                    Strings.Format(StringKeys.TASK_NOT_COMPLETE_WITHIN, Expression, AssertionScope.FormatInvariant(timeout)),
                    because,
                    becauseArgs));
        }

        return new AndConstraint<TaskAssertions<T>>(this);
    }
}
