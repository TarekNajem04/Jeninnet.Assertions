//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

#if !NETSTANDARD2_0
/// <summary>Provides assertion methods for verifying that a <see cref="TaskCompletionSource"/> completes in time.</summary>
/// <remarks>Non-generic <see cref="TaskCompletionSource"/> requires .NET 5+ and is unavailable on .NET Standard 2.0; use <see cref="TaskCompletionSourceAssertions{T}"/> there.</remarks>
/// <param name="source">The task completion source to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
/// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
public class TaskCompletionSourceAssertions(TaskCompletionSource source, string? expression = null) : AssertionsBase(expression, "task") {
    private readonly TaskCompletionSource _source = source ?? throw new ArgumentNullException(nameof(source));

    /// <summary>Asserts that the task completes (successfully, faulted, or canceled counts as completion for timing) within <paramref name="timeout"/>; faults and cancellations still fail, with the original exception preserved.</summary>
    /// <param name="timeout">The maximum accepted completion time (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A task that represents the asynchronous assertion operation.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The task did not complete in time, faulted, or was canceled.</exception>
    public async Task<AndConstraint<TaskCompletionSourceAssertions>> CompleteWithinAsync(TimeSpan timeout, string? because = null, params object?[] becauseArgs) {
        TaskAssertionHelpers.ValidateTimeout(timeout);
        await TaskAssertionHelpers.WaitForCompletionAsync(_source.Task, timeout, Expression, because, becauseArgs);

        try {
            await _source.Task;
        }
        catch(Exception ex) {
            throw new AssertionFailedException(
                AssertionScope.AppendBecause(
                    Strings.Format(StringKeys.TASK_COMPLETE_SUCCESSFULLY, Expression, AssertionScope.FormatInvariant(timeout), ex.GetType().Name),
                    because,
                    becauseArgs),
                ex);
        }

        return new AndConstraint<TaskCompletionSourceAssertions>(this);
    }

    /// <summary>Asserts that the task does not complete within <paramref name="timeout"/>.</summary>
    /// <param name="timeout">The time span to wait for completion (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A task whose result can continue the fluent chain.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The task completed within the time span.</exception>
    public async Task<AndConstraint<TaskCompletionSourceAssertions>> NotCompleteWithinAsync(TimeSpan timeout, string? because = null, params object?[] becauseArgs) {
        TaskAssertionHelpers.ValidateTimeout(timeout);

        if(_source.Task.IsCompleted || await TaskAssertionHelpers.CompletesWithinAsync(_source.Task, timeout)) {
            throw new AssertionFailedException(
                AssertionScope.AppendBecause(
                    Strings.Format(StringKeys.TASK_NOT_COMPLETE_WITHIN, Expression, AssertionScope.FormatInvariant(timeout)),
                    because,
                    becauseArgs));
        }

        return new AndConstraint<TaskCompletionSourceAssertions>(this);
    }
}
#endif

/// <summary>Provides assertion methods for verifying that a <see cref="TaskCompletionSource{TResult}"/> completes in time.</summary>
/// <typeparam name="T">The type of the task result.</typeparam>
/// <param name="source">The task completion source to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
/// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
public class TaskCompletionSourceAssertions<T>(TaskCompletionSource<T> source, string? expression = null) : AssertionsBase(expression, "task") {
    private readonly TaskCompletionSource<T> _source = source ?? throw new ArgumentNullException(nameof(source));

    /// <summary>Asserts that the task completes within <paramref name="timeout"/> and exposes its result; faults and cancellations still fail, with the original exception preserved.</summary>
    /// <param name="timeout">The maximum accepted completion time (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A task whose result exposes the completed value through <c>Which</c>.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The task did not complete in time, faulted, or was canceled.</exception>
    public async Task<AndWhichConstraint<TaskCompletionSourceAssertions<T>, T>> CompleteWithinAsync(TimeSpan timeout, string? because = null, params object?[] becauseArgs) {
        TaskAssertionHelpers.ValidateTimeout(timeout);
        await TaskAssertionHelpers.WaitForCompletionAsync(_source.Task, timeout, Expression, because, becauseArgs);

        try {
            return new AndWhichConstraint<TaskCompletionSourceAssertions<T>, T>(this, await _source.Task, Expression);
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
    public async Task<AndConstraint<TaskCompletionSourceAssertions<T>>> NotCompleteWithinAsync(TimeSpan timeout, string? because = null, params object?[] becauseArgs) {
        TaskAssertionHelpers.ValidateTimeout(timeout);

        if(_source.Task.IsCompleted || await TaskAssertionHelpers.CompletesWithinAsync(_source.Task, timeout)) {
            throw new AssertionFailedException(
                AssertionScope.AppendBecause(
                    Strings.Format(StringKeys.TASK_NOT_COMPLETE_WITHIN, Expression, AssertionScope.FormatInvariant(timeout)),
                    because,
                    becauseArgs));
        }

        return new AndConstraint<TaskCompletionSourceAssertions<T>>(this);
    }
}
