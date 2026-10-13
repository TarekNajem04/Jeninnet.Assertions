//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying that token-aware delegates honor (or deliberately ignore) cancellation.</summary>
/// <remarks>
/// Both verbs pass an already-canceled token, so no wall-clock waiting is involved. The assertions check the
/// throwing contract only; they do not judge how promptly a cancellation was noticed.
/// </remarks>
/// <param name="action">The asynchronous token-aware delegate to invoke and verify.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
/// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
public class CancellationAssertions(Func<CancellationToken, Task>? action, string? expression = null) : AssertionsBase(expression, "action") {
    private readonly Func<CancellationToken, Task> _action = action ?? throw new ArgumentNullException(nameof(action));

    /// <summary>Creates cancellation assertions for a synchronous token-aware delegate.</summary>
    /// <param name="action">The synchronous token-aware delegate to invoke and verify.</param>
    /// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
    public CancellationAssertions(Action<CancellationToken>? action, string? expression = null) : this(Adapt(action), expression) {
    }

    /// <summary>
    /// Asserts that the delegate honors cancellation: invoked with an already-canceled token, it must throw
    /// <see cref="OperationCanceledException"/> carrying that very token.
    /// </summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">
    /// The delegate completed without throwing, threw with a different token, or threw another exception type.
    /// </exception>
    public async Task<AndConstraint<CancellationAssertions>> HonorCancellationAsync(string? because = null, params object?[] becauseArgs) {
        using var source = new CancellationTokenSource();
        await CancelImmediately(source);
        var token = source.Token;

        try {
            await _action(token);
        }
        catch(OperationCanceledException exception) when(exception.CancellationToken == token) {
            return new(this);
        }
        catch(OperationCanceledException exception) {
            Fail(Strings.Format(StringKeys.ACTION_HONOR_CANCELLATION_WRONG_TOKEN, Expression), because, becauseArgs, exception);
            return new(this);
        }
        catch(Exception exception) {
            Fail(Strings.Format(StringKeys.ACTION_HONOR_CANCELLATION_WRONG_EXCEPTION, Expression, exception.GetType().Name), because, becauseArgs, exception);
            return new(this);
        }

        Fail(Strings.Format(StringKeys.ACTION_HONOR_CANCELLATION_NOT_HONORED, Expression), because, becauseArgs);
        return new(this);
    }

    /// <summary>
    /// Asserts that the delegate completes despite cancellation, even when handed an already-canceled token —
    /// the contract of critical sections and <c>finally</c> cleanup that must not be abandoned.
    /// </summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The delegate was canceled or threw another exception type.</exception>
    public async Task<AndConstraint<CancellationAssertions>> CompleteDespiteCancellationAsync(string? because = null, params object?[] becauseArgs) {
        using var source = new CancellationTokenSource();
        await CancelImmediately(source);

        try {
            await _action(source.Token);
        }
        catch(OperationCanceledException exception) {
            Fail(Strings.Format(StringKeys.ACTION_COMPLETE_DESPITE_CANCELLATION, Expression), because, becauseArgs, exception);
            return new(this);
        }
        catch(Exception exception) {
            Fail(Strings.Format(StringKeys.ACTION_COMPLETE_DESPITE_CANCELLATION_THREW, Expression, exception.GetType().Name), because, becauseArgs, exception);
            return new(this);
        }

        return new(this);
    }

    /// <summary>Cancels <paramref name="source"/> immediately, preferring the async API on modern targets.</summary>
    /// <param name="source">The source to cancel.</param>
    /// <returns>A task that completes when cancellation has been requested.</returns>
    private static Task CancelImmediately(CancellationTokenSource source) {
#if NET8_0_OR_GREATER
        return source.CancelAsync();
#else
        source.Cancel();
        return Task.CompletedTask;
#endif
    }

    /// <summary>Adapts a synchronous token-aware delegate to the asynchronous shape shared by the verbs.</summary>
    /// <param name="action">The synchronous delegate to adapt.</param>
    /// <returns>The adapted asynchronous delegate.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
    internal static Func<CancellationToken, Task> Adapt(Action<CancellationToken>? action) {
        if(action is null) {
            throw new ArgumentNullException(nameof(action));
        }

        return token => {
            action(token);
            return Task.CompletedTask;
        };
    }

    /// <summary>Adapts a synchronous token-aware value-producing delegate to the asynchronous shape shared by the verbs.</summary>
    /// <typeparam name="T">The type of the value returned by the delegate.</typeparam>
    /// <param name="action">The synchronous delegate to adapt.</param>
    /// <returns>The adapted asynchronous delegate.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
    internal static Func<CancellationToken, Task> Adapt<T>(Func<CancellationToken, T>? action) {
        if(action is null) {
            throw new ArgumentNullException(nameof(action));
        }

        return token => {
            action(token);
            return Task.CompletedTask;
        };
    }
}
