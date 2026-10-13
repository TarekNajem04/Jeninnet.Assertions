//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public static partial class AssertionExtensions {
    /// <summary>Starts monitoring every public instance event raised by <paramref name="source"/>.</summary>
    /// <param name="source">The event source to observe.</param>
    /// <param name="options">The attach behavior (a <see langword="null"/> value selects the defaults).</param>
    /// <returns>An <see cref="EventMonitor"/> recording the raised events (dispose to detach).</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    public static EventMonitor Monitor(this object? source, EventMonitorOptions? options = null) =>
        new(source ?? throw new ArgumentNullException(nameof(source)), options);

    /// <summary>Starts monitoring every public instance event raised by <paramref name="source"/>.</summary>
    /// <remarks>The generic parameter only names the source statically; attach behavior is identical to the non-generic <c>Monitor()</c> and loads no assemblies beyond normal reflection.</remarks>
    /// <typeparam name="T">The static type of the event source.</typeparam>
    /// <param name="source">The event source to observe.</param>
    /// <param name="options">The attach behavior (a <see langword="null"/> value selects the defaults).</param>
    /// <returns>An <see cref="EventMonitor"/> recording the raised events (dispose to detach).</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    public static EventMonitor Monitor<T>(this T? source, EventMonitorOptions? options = null) =>
        new((object?)source ?? throw new ArgumentNullException(nameof(source)), options);

    /// <summary>
    /// Creates a <see cref="SpanAssertions{T}"/> for the given span. A dedicated entry point (instead of
    /// <c>Should()</c>) keeps array overload resolution unambiguous with <c>Should(IEnumerable&lt;T&gt;)</c>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <param name="value">The span to assert on (snapshotted).</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static SpanAssertions<T> AsAssertions<T>(this ReadOnlySpan<T> value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Asserts that a character span equals <paramref name="expected"/> using ordinal comparison.</summary>
    /// <param name="assertions">The character span assertions to continue from.</param>
    /// <param name="expected">The expected text.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assertions"/> or <paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The span text differs from <paramref name="expected"/>.</exception>
    public static AndConstraint<SpanAssertions<char>> Be(this SpanAssertions<char> assertions, string? expected, string? because = null, params object?[] becauseArgs) {
        if(assertions is null) {
            throw new ArgumentNullException(nameof(assertions));
        }

        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var actual = new string([.. assertions.Snapshot]);
        if(!string.Equals(actual, expected, StringComparison.Ordinal)) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.SPAN_BE_STRING, assertions.Expression, $"'{expected}'", $"'{actual}'"), because, becauseArgs));
        }

        return new(assertions);
    }

    /// <summary>Creates an <see cref="EventMonitorAssertions"/> for the given event monitor.</summary>
    /// <param name="value">The monitor to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static EventMonitorAssertions Should(this EventMonitor value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

#if !NETSTANDARD2_0
    /// <summary>Creates a <see cref="SpanAssertions{T}"/> for the given memory block.</summary>
    /// <remarks>Unavailable on .NET Standard 2.0, where <c>Memory&lt;T&gt;</c> cannot appear in the public surface; use <c>AsAssertions()</c> there.</remarks>
    /// <typeparam name="T">The type of elements in the memory block.</typeparam>
    /// <param name="value">The memory block to assert on (snapshotted).</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static SpanAssertions<T> Should<T>(this Memory<T> value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value.Span, expression);

    /// <summary>Creates a <see cref="SpanAssertions{T}"/> for the given read-only memory block.</summary>
    /// <remarks>Unavailable on .NET Standard 2.0, where <c>ReadOnlyMemory&lt;T&gt;</c> cannot appear in the public surface; use <c>AsAssertions()</c> there.</remarks>
    /// <typeparam name="T">The type of elements in the memory block.</typeparam>
    /// <param name="value">The memory block to assert on (snapshotted).</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static SpanAssertions<T> Should<T>(this ReadOnlyMemory<T> value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value.Span, expression);

    /// <summary>Creates a <see cref="SpanAssertions{T}"/> for the given span.</summary>
    /// <remarks>Unavailable on .NET Standard 2.0, where <c>Span&lt;T&gt;</c> cannot appear in the public surface; use <c>AsAssertions()</c> there.</remarks>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <param name="value">The span to assert on (snapshotted).</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static SpanAssertions<T> Should<T>(this Span<T> value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="SpanAssertions{T}"/> for the given read-only span.</summary>
    /// <remarks>Unavailable on .NET Standard 2.0, where <c>ReadOnlySpan&lt;T&gt;</c> cannot appear in the public surface; use <c>AsAssertions()</c> there.</remarks>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <param name="value">The span to assert on (snapshotted).</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static SpanAssertions<T> Should<T>(this ReadOnlySpan<T> value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);
#endif

#if NET6_0_OR_GREATER
    /// <summary>Creates an <see cref="HttpResponseMessageAssertions"/> for the given HTTP response.</summary>
    /// <param name="value">The response to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static HttpResponseMessageAssertions Should(this HttpResponseMessage? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);
#endif

    /// <summary>
    /// Casts <paramref name="value"/> to <typeparamref name="TTo"/> when possible; otherwise fails with a localized assertion message.
    /// </summary>
    /// <typeparam name="TTo">The type to cast <paramref name="value"/> to.</typeparam>
    /// <param name="value">The value to cast.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    /// <returns><paramref name="value"/> cast to <typeparamref name="TTo"/>.</returns>
    /// <exception cref="AssertionFailedException"><paramref name="value"/> cannot be cast to <typeparamref name="TTo"/>.</exception>
    public static TTo As<TTo>(this object? value, [CallerArgumentExpression(nameof(value))] string? expression = null) {
        if(value is TTo result) {
            return result;
        }

        throw new AssertionFailedException(
            Strings.Format(StringKeys.OBJECT_AS, expression ?? "value", typeof(TTo).Name, value?.GetType().Name ?? "<null>"));
    }

    /// <summary>
    /// Asserts that <paramref name="value"/> satisfies every condition in <paramref name="conditions"/>,
    /// collecting every failure into one message (a <c>because</c> clause is intentionally unsupported:
    /// each condition already carries its own failure message).
    /// </summary>
    /// <typeparam name="T">The type of the subject under assertion.</typeparam>
    /// <param name="value">The subject to assert on.</param>
    /// <param name="conditions">The conditions that must all pass (an empty set passes vacuously).</param>
    /// <exception cref="ArgumentNullException"><paramref name="conditions"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">A condition is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">At least one condition failed.</exception>
    public static void SatisfyAllConditions<T>(this T value, params Action<T>[] conditions) {
        if(conditions is null) {
            throw new ArgumentNullException(nameof(conditions));
        }

        for(var index = 0; index < conditions.Length; index++) {
            if(conditions[index] is null) {
                throw new ArgumentException($"Condition at index {index} is null.", nameof(conditions));
            }
        }

        var failures = new List<string>();
        foreach(var condition in conditions) {
            try {
                condition(value);
            }
            catch(AssertionFailedException ex) {
                failures.Add(ex.Message);
            }
        }

        if(failures.Count != 0) {
            var details = string.Concat(failures.Select((message, failureIndex) => $"\n{failureIndex + 1}. {message}"));
            AssertionScope.Fail(Strings.Format(StringKeys.OBJECT_SATISFY_ALL_CONDITIONS, "value", details));
        }
    }
}
