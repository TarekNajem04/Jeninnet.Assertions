//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class TaskAssertions {
    /// <summary>Asserts that the task is completed (successfully, faulted, or canceled), without waiting.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The task is still running.</exception>
    public AndConstraint<TaskAssertions> BeCompleted(string? because = null, params object?[] becauseArgs) {
        if(!_task.IsCompleted) {
            Fail(Strings.Format(StringKeys.TASK_BE_COMPLETED, Expression, TaskState.Describe(_task)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the task is not completed yet, without waiting.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The task is completed.</exception>
    public AndConstraint<TaskAssertions> NotBeCompleted(string? because = null, params object?[] becauseArgs) {
        if(_task.IsCompleted) {
            Fail(Strings.Format(StringKeys.TASK_NOT_BE_COMPLETED, Expression, TaskState.Describe(_task)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the task ran to completion without faulting or cancellation.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The task is pending, faulted, or canceled.</exception>
    public AndConstraint<TaskAssertions> BeCompletedSuccessfully(string? because = null, params object?[] becauseArgs) {
        if(!TaskState.IsCompletedSuccessfully(_task)) {
            Fail(Strings.Format(StringKeys.TASK_BE_COMPLETED_SUCCESSFULLY, Expression, TaskState.Describe(_task)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the task is faulted and exposes the root exception through <c>Which</c>.</summary>
    /// <remarks>This assertion returns a value, so it throws immediately even inside an <see cref="AssertionScope"/>.</remarks>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the base exception.</returns>
    /// <exception cref="AssertionFailedException">The task is pending, successful, or canceled.</exception>
    public WhichConstraint<Exception> BeFaulted(string? because = null, params object?[] becauseArgs) {
        if(!_task.IsFaulted) {
            throw new AssertionFailedException(
                AssertionScope.AppendBecause(Strings.Format(StringKeys.TASK_BE_FAULTED, Expression, TaskState.Describe(_task)), because, becauseArgs));
        }

        return new WhichConstraint<Exception>(TaskState.GetFailure(_task)!);
    }

    /// <summary>Asserts that the task was canceled.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The task is pending, successful, or faulted.</exception>
    public AndConstraint<TaskAssertions> BeCanceled(string? because = null, params object?[] becauseArgs) {
        if(!_task.IsCanceled) {
            Fail(Strings.Format(StringKeys.TASK_BE_CANCELED, Expression, TaskState.Describe(_task)), because, becauseArgs);
        }

        return new(this);
    }
}

public partial class TaskAssertions<T> {
    /// <summary>Asserts that the task is completed (successfully, faulted, or canceled), without waiting.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The task is still running.</exception>
    public AndConstraint<TaskAssertions<T>> BeCompleted(string? because = null, params object?[] becauseArgs) {
        if(!_task.IsCompleted) {
            Fail(Strings.Format(StringKeys.TASK_BE_COMPLETED, Expression, TaskState.Describe(_task)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the task is not completed yet, without waiting.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The task is completed.</exception>
    public AndConstraint<TaskAssertions<T>> NotBeCompleted(string? because = null, params object?[] becauseArgs) {
        if(_task.IsCompleted) {
            Fail(Strings.Format(StringKeys.TASK_NOT_BE_COMPLETED, Expression, TaskState.Describe(_task)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the task ran to completion and exposes its result through <c>Which</c>.</summary>
    /// <remarks>This assertion returns a value, so it throws immediately even inside an <see cref="AssertionScope"/>.</remarks>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndWhichConstraint{TParent, T}"/> exposing the completed result.</returns>
    /// <exception cref="AssertionFailedException">The task is pending, faulted, or canceled.</exception>
    public AndWhichConstraint<TaskAssertions<T>, T> BeCompletedSuccessfully(string? because = null, params object?[] becauseArgs) {
        if(!TaskState.IsCompletedSuccessfully(_task)) {
            throw new AssertionFailedException(
                AssertionScope.AppendBecause(Strings.Format(StringKeys.TASK_BE_COMPLETED_SUCCESSFULLY, Expression, TaskState.Describe(_task)), because, becauseArgs));
        }

        return new AndWhichConstraint<TaskAssertions<T>, T>(this, GetCompletedResult(), Expression);
    }

    /// <summary>Asserts that the task is faulted and exposes the root exception through <c>Which</c>.</summary>
    /// <remarks>This assertion returns a value, so it throws immediately even inside an <see cref="AssertionScope"/>.</remarks>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the base exception.</returns>
    /// <exception cref="AssertionFailedException">The task is pending, successful, or canceled.</exception>
    public WhichConstraint<Exception> BeFaulted(string? because = null, params object?[] becauseArgs) {
        if(!_task.IsFaulted) {
            throw new AssertionFailedException(
                AssertionScope.AppendBecause(Strings.Format(StringKeys.TASK_BE_FAULTED, Expression, TaskState.Describe(_task)), because, becauseArgs));
        }

        return new WhichConstraint<Exception>(TaskState.GetFailure(_task)!);
    }

    /// <summary>Asserts that the task was canceled.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The task is pending, successful, or faulted.</exception>
    public AndConstraint<TaskAssertions<T>> BeCanceled(string? because = null, params object?[] becauseArgs) {
        if(!_task.IsCanceled) {
            Fail(Strings.Format(StringKeys.TASK_BE_CANCELED, Expression, TaskState.Describe(_task)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the task completed successfully and exposes its result through <c>Which</c>.</summary>
    /// <remarks>This assertion returns a value, so it throws immediately even inside an <see cref="AssertionScope"/>.</remarks>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the completed result.</returns>
    /// <exception cref="AssertionFailedException">The task is pending, faulted, or canceled.</exception>
    public WhichConstraint<T> HaveResult(string? because = null, params object?[] becauseArgs) {
        if(!TaskState.IsCompletedSuccessfully(_task)) {
            throw new AssertionFailedException(
                AssertionScope.AppendBecause(Strings.Format(StringKeys.TASK_HAVE_RESULT_NOT_COMPLETED, Expression, TaskState.Describe(_task)), because, becauseArgs));
        }

        return new WhichConstraint<T>(GetCompletedResult());
    }

    /// <summary>Asserts that the task completed successfully with <paramref name="expected"/> (default equality).</summary>
    /// <remarks>Comparison uses <see cref="EqualityComparer{T}.Default"/>; object-graph equivalency is not applied.</remarks>
    /// <param name="expected">The expected result (may be <see langword="null"/>).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The task did not complete successfully or the result differs.</exception>
    public AndConstraint<TaskAssertions<T>> HaveResult(T expected, string? because = null, params object?[] becauseArgs) {
        var actual = HaveResult(because, becauseArgs).Which;
        if(!EqualityComparer<T>.Default.Equals(actual, expected)) {
            Fail(Strings.Format(StringKeys.TASK_WITH_RESULT, Expression, FormatResult(expected), FormatResult(actual)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the task completed successfully with a result satisfying <paramref name="predicate"/>.</summary>
    /// <param name="predicate">The condition the result must satisfy.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The task did not complete successfully or the predicate fails.</exception>
    public AndConstraint<TaskAssertions<T>> HaveResult(Func<T, bool> predicate, string? because = null, params object?[] becauseArgs) {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        var actual = HaveResult(because, becauseArgs).Which;
        if(!predicate(actual)) {
            Fail(Strings.Format(StringKeys.TASK_HAVE_RESULT_PREDICATE, Expression, FormatResult(actual)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Reads the result of a task already verified as <see cref="TaskStatus.RanToCompletion"/>.</summary>
    /// <remarks>Called only after the completion check, so it cannot block on a running task.</remarks>
    /// <returns>The completed result.</returns>
    private T GetCompletedResult() => _task.GetAwaiter().GetResult();

    /// <summary>Formats a result for failure messages.</summary>
    /// <typeparam name="TValue">The result type.</typeparam>
    /// <param name="value">The result value.</param>
    /// <returns><c>&lt;null&gt;</c> or the invariant representation.</returns>
    private static string FormatResult<TValue>(TValue value) => value is null ? "<null>" : AssertionScope.FormatInvariant(value);
}
