//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class AsyncActionAssertions {
    /// <summary>Asserts that the asynchronous action is safe to run concurrently: copies start together behind a barrier and every copy must finish cleanly within the timeout.</summary>
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
    public async Task<AndConstraint<AsyncActionAssertions>> BeThreadSafeAsync(int copies = 8, TimeSpan? timeout = null, string? because = null, params object?[] becauseArgs) {
        var wait = ConcurrencyStress.Validate(copies, timeout);
        var result = await ConcurrencyStress.RunAsync(_action, copies, wait).ConfigureAwait(false);

        if(result.Pending != 0) {
            Fail(Strings.Format(StringKeys.ACTION_THREAD_SAFE_TIMEOUT, Expression, AssertionScope.FormatInvariant(result.Pending), AssertionScope.FormatInvariant(copies), AssertionScope.FormatInvariant(wait)), because, becauseArgs);
            return new(this);
        }

        if(result.Failures.Count != 0) {
            Fail(ConcurrencyStress.BuildFailureMessage(Expression, result.Failures, copies), because, becauseArgs);
        }

        return new(this);
    }
}
