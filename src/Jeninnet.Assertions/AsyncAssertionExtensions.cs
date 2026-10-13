//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Continues assertions on the value exposed by a completed asynchronous operation.</summary>
public static class AsyncAssertionExtensions {
    /// <summary>Asserts that the completed asynchronous operation produced <paramref name="expected"/>.</summary>
    /// <typeparam name="TParent">The parent assertions type returned by the preceding asynchronous assertion.</typeparam>
    /// <typeparam name="T">The type of the completed result.</typeparam>
    /// <param name="task">The completed assertion task exposing the result.</param>
    /// <param name="expected">The expected result.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>The completed assertion for further chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="task"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The completed result differs from <paramref name="expected"/>.</exception>
    public static async Task<AndWhichConstraint<TParent, T>> WithResultAsync<TParent, T>(
        this Task<AndWhichConstraint<TParent, T>> task,
        T? expected,
        string? because = null,
        params object?[] becauseArgs) {
        if(task is null) {
            throw new ArgumentNullException(nameof(task));
        }

        var completed = await task;
        if(!Equals(completed.Which, expected)) {
            AssertionScope.Fail(
                AssertionScope.AppendBecause(
                    Strings.Format(StringKeys.TASK_WITH_RESULT, completed.Expression ?? "task", FormatResult(expected), FormatResult(completed.Which)),
                    because,
                    becauseArgs));
        }

        return completed;
    }

    private static string FormatResult<T>(T? value) => value is null ? "<null>" : AssertionScope.FormatInvariant(value);
}
