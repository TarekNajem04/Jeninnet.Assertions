//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public static partial class AssertionExtensions {
    /// <summary>Creates an <see cref="ActionAssertions"/> for the given synchronous delegate.</summary>
    /// <param name="value">The action delegate to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static ActionAssertions Should(this Action value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates an <see cref="ActionAssertions"/> for the given synchronous delegate.</summary>
    /// <typeparam name="T">The type of the value returned by the delegate.</typeparam>
    /// <param name="value">The action delegate to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static ActionAssertions<T> Should<T>(this Func<T> value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates an <see cref="AsyncActionAssertions"/> for the given asynchronous delegate.</summary>
    /// <param name="value">The asynchronous action delegate to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static AsyncActionAssertions Should(this Func<Task> value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="CancellationAssertions"/> for the given asynchronous token-aware delegate.</summary>
    /// <param name="value">The token-aware asynchronous delegate to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static CancellationAssertions Should(this Func<CancellationToken, Task>? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="CancellationAssertions"/> for the given synchronous token-aware delegate.</summary>
    /// <param name="value">The token-aware synchronous delegate to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static CancellationAssertions Should(this Action<CancellationToken>? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="CancellationAssertions"/> for the given synchronous token-aware value-producing delegate.</summary>
    /// <typeparam name="T">The type of the value returned by the delegate.</typeparam>
    /// <param name="value">The token-aware synchronous delegate to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static CancellationAssertions Should<T>(this Func<CancellationToken, T>? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(CancellationAssertions.Adapt(value), expression);

    /// <summary>Creates a <see cref="GenericAsyncFunctionAssertions{T}"/> for the given asynchronous value-returning delegate.</summary>
    /// <typeparam name="T">The type of the value returned by the delegate.</typeparam>
    /// <param name="value">The asynchronous delegate to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static GenericAsyncFunctionAssertions<T> Should<T>(this Func<Task<T>> value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

#if !NETSTANDARD2_0
    /// <summary>Creates a <see cref="TaskCompletionSourceAssertions"/> for the given task completion source.</summary>
    /// <param name="value">The task completion source to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static TaskCompletionSourceAssertions Should(this TaskCompletionSource value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates an <see cref="AsyncEnumerableAssertions{T}"/> for the given asynchronous sequence.</summary>
    /// <typeparam name="T">The element type of the sequence.</typeparam>
    /// <param name="value">The asynchronous sequence to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static AsyncEnumerableAssertions<T> Should<T>(this IAsyncEnumerable<T>? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);
#endif

    /// <summary>Creates a <see cref="TaskCompletionSourceAssertions{T}"/> for the given task completion source.</summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <param name="value">The task completion source to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static TaskCompletionSourceAssertions<T> Should<T>(this TaskCompletionSource<T> value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="TaskAssertions"/> for the given task.</summary>
    /// <param name="value">The task to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static TaskAssertions Should(this Task? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="TaskAssertions{T}"/> for the given task.</summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <param name="value">The task to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static TaskAssertions<T> Should<T>(this Task<T>? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Executes <paramref name="action"/> immediately and measures its duration.</summary>
    /// <param name="action">The synchronous delegate to time.</param>
    /// <returns>An <see cref="Jeninnet.Assertions.ExecutionTime"/> carrying the measured duration (assert with <c>Should()</c>).</returns>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
    public static ExecutionTime ExecutionTime(this Action action) {
        if(action is null) {
            throw new ArgumentNullException(nameof(action));
        }

        var stopwatch = Stopwatch.StartNew();
        action();
        stopwatch.Stop();
        return new ExecutionTime(stopwatch.Elapsed);
    }

    /// <summary>Runs <paramref name="action"/> to completion and measures its duration.</summary>
    /// <param name="action">The asynchronous delegate to time. The returned task is waited on synchronously.</param>
    /// <returns>An <see cref="Jeninnet.Assertions.ExecutionTime"/> carrying the measured duration (assert with <c>Should()</c>).</returns>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
    public static ExecutionTime ExecutionTime(this Func<Task> action) {
        if(action is null) {
            throw new ArgumentNullException(nameof(action));
        }

        var stopwatch = Stopwatch.StartNew();
        action().GetAwaiter().GetResult();
        stopwatch.Stop();
        return new ExecutionTime(stopwatch.Elapsed);
    }

    /// <summary>Executes a member of <paramref name="subject"/> immediately and measures its duration.</summary>
    /// <typeparam name="T">The type of the subject.</typeparam>
    /// <param name="subject">The object owning the member to time.</param>
    /// <param name="action">A property or method access on <paramref name="subject"/> (the access is invoked).</param>
    /// <returns>An <see cref="Jeninnet.Assertions.ExecutionTime"/> carrying the measured duration (assert with <c>Should()</c>).</returns>
    /// <exception cref="ArgumentNullException"><paramref name="subject"/> or <paramref name="action"/> is <see langword="null"/>.</exception>
    public static ExecutionTime ExecutionTimeOf<T>(this T subject, Expression<Func<T, object>> action) {
        if(subject is null) {
            throw new ArgumentNullException(nameof(subject));
        }

        if(action is null) {
            throw new ArgumentNullException(nameof(action));
        }

        var invoke = action.Compile();
        var stopwatch = Stopwatch.StartNew();
        _ = invoke(subject);
        stopwatch.Stop();
        return new ExecutionTime(stopwatch.Elapsed);
    }

    /// <summary>Creates an <see cref="ExecutionTimeAssertions"/> for the given measured duration.</summary>
    /// <param name="value">The measured execution time to assert on.</param>
    public static ExecutionTimeAssertions Should(this ExecutionTime value) => new(value.Elapsed);
}
