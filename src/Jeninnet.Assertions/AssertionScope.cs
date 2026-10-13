//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>
/// Batches multiple assertions so that every failure is collected and reported together
/// when the scope is disposed, instead of throwing on the first failure.
/// </summary>
/// <remarks>
/// Failures of assertions that return <see cref="AndConstraint{TParent}"/> are recorded and
/// execution continues, so a single scope can surface many problems at once. Assertions that
/// must return a value (<c>Throw*</c>, <c>ContainSingle</c>) throw immediately even inside a
/// scope, which aborts the surrounding scope block.
/// <para/>
/// Scopes can be nested and named: failures of an inner scope are merged into the parent scope
/// prefixed with the inner context (e.g. <c>[Inner] …</c>). Only the outermost scope throws.
/// Use <see cref="Discard"/> to collect the failures so far without throwing on dispose.
/// </remarks>
/// <example>
/// <code>
/// using (new AssertionScope()) {
///     flag.Should().BeTrue();
///     actual.Should().Be("expected");
/// }
/// </code>
/// </example>
public sealed class AssertionScope : IDisposable {
    private static readonly AsyncLocal<AssertionScope?> _current = new();

    private readonly AssertionScope? _parent;
    private readonly string? _context;
    private readonly IAssertionStrategy _assertionStrategy;
    private readonly StringBuilder _tracing = new();
    private readonly bool _suppressThrowOnDispose;
    private bool _disposed;

    /// <summary>Creates a new assertion scope that becomes the active failure collector.</summary>
    /// <param name="context">
    /// An optional name for the scope. When set, failures merged from this scope into a parent
    /// scope are prefixed with <c>[context]</c>.
    /// </param>
    public AssertionScope(string? context = null) : this(new CollectingAssertionStrategy(), context) {
    }

    /// <summary>Creates a new assertion scope that reports failures through <paramref name="assertionStrategy"/>.</summary>
    /// <param name="assertionStrategy">The strategy that records or throws the failures of this scope.</param>
    /// <exception cref="ArgumentNullException"><paramref name="assertionStrategy"/> is <see langword="null"/>.</exception>
    public AssertionScope(IAssertionStrategy assertionStrategy) : this(
        assertionStrategy ?? throw new ArgumentNullException(nameof(assertionStrategy)),
        null) {
    }

    internal AssertionScope(IAssertionStrategy assertionStrategy, string? context, bool suppressThrowOnDispose = false) {
        _assertionStrategy = assertionStrategy;
        _suppressThrowOnDispose = suppressThrowOnDispose;
        if(context is null) {
            _context = null;
        } else {
            var trimmed = context.Trim();
            _context = trimmed.Length == 0 ? null : trimmed;
        }

        _parent = _current.Value;
        _current.Value = this;
        Formatting = _parent?.Formatting.Copy() ?? new ValueFormatterOptions();
    }

    /// <summary>Gets the value formatters scoped to this scope (inherited as a snapshot from the parent scope).</summary>
    public ValueFormatterOptions Formatting { get; }

    /// <summary>Gets the currently active scope, or <see langword="null"/> when assertions run outside any scope.</summary>
    /// <remarks>Public so external frameworks and tooling can observe the ambient collection point.</remarks>
    public static AssertionScope? Current => _current.Value;

    /// <summary>Gets the scope name given at construction, or <see langword="null"/> for anonymous scopes.</summary>
    public string? Name => _context;

    /// <summary>Starts a chained conditional assertion whose value is selected by <paramref name="selector"/>.</summary>
    /// <remarks>
    /// The selector is evaluated once, conditions run
    /// against the selected value, and a <c>FailWith</c> marks the scope failed when the condition was not met.
    /// Selectors after a failed step are not evaluated.
    /// </remarks>
    /// <typeparam name="T">The type of the selected value.</typeparam>
    /// <param name="selector">The value the chain's conditions run against.</param>
    /// <returns>A selector for the chain.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    public GivenSelector<T> Given<T>(Func<T> selector) {
        if(selector is null) {
            throw new ArgumentNullException(nameof(selector));
        }

        return new GivenSelector<T>(selector, this, new GivenChainState());
    }

    /// <summary>Determines whether this scope has collected failures that have not been discarded.</summary>
    /// <returns><see langword="true"/> when at least one failure is pending.</returns>
    public bool HasFailures() => _assertionStrategy.FailureMessages.Any();

    /// <summary>
    /// Adds an already fully formatted failure message to this scope, bypassing the assertion
    /// pipeline. Useful when an external framework reports a failure into the ambient scope.
    /// </summary>
    /// <param name="formattedFailureMessage">The complete failure message.</param>
    /// <exception cref="ArgumentNullException"><paramref name="formattedFailureMessage"/> is <see langword="null"/>.</exception>
    public void AddPreFormattedFailure(string formattedFailureMessage) {
        if(formattedFailureMessage is null) {
            throw new ArgumentNullException(nameof(formattedFailureMessage));
        }

        _assertionStrategy.HandleFailure(formattedFailureMessage);
    }

    /// <summary>
    /// Appends diagnostic tracing (e.g. an equivalency walk dump) that is appended to the failure
    /// message when this scope or an ancestor scope throws. <see langword="null"/> is a no-op.
    /// </summary>
    /// <param name="tracingBlock">The tracing text to append.</param>
    public void AppendTracing(string? tracingBlock) => _tracing.Append(tracingBlock);

    /// <summary>
    /// Reports an assertion failure: records it on the active scope when one exists,
    /// otherwise throws an <see cref="AssertionFailedException"/> immediately.
    /// </summary>
    /// <param name="message">The fully formatted failure message (including any <c>because</c> clause).</param>
    /// <param name="innerException">
    /// The cause of the failure, preserved only when thrown immediately. Batched failures keep the message only.
    /// </param>
    /// <exception cref="AssertionFailedException">No scope is active and the assertion failed.</exception>
    internal static void Fail(string message, Exception? innerException = null) {
        var scope = _current.Value ?? throw new AssertionFailedException(message, innerException);
        scope._assertionStrategy.HandleFailure(message);
    }

    /// <summary>
    /// Reports a structured assertion failure on the active scope (or throws immediately when none is active),
    /// preserving the <see cref="AssertionFailure"/> so <see cref="SoftAssertions"/> can serialize it to JSON.
    /// Custom strategies only understand messages, so they receive the failure message.
    /// </summary>
    /// <param name="failure">The structured failure to record.</param>
    /// <param name="innerException">
    /// The cause of the failure, preserved only when thrown immediately. Batched failures keep the message only.
    /// </param>
    /// <exception cref="AssertionFailedException">No scope is active and the assertion failed.</exception>
    internal static void Fail(AssertionFailure failure, Exception? innerException = null) {
        var scope = _current.Value ?? throw new AssertionFailedException(failure.Message, innerException);
        if(scope._assertionStrategy is CollectingAssertionStrategy collector) {
            collector.AddFailure(failure);
        } else {
            scope._assertionStrategy.HandleFailure(failure.Message);
        }
    }

    /// <summary>Gets the structured failures collected so far (empty for strategies that only record messages).</summary>
    internal IReadOnlyList<AssertionFailure> StructuredFailures =>
        _assertionStrategy is CollectingAssertionStrategy collector ? collector.Failures : (IReadOnlyList<AssertionFailure>)[];

    /// <summary>
    /// Formats a value with the invariant culture so failure messages are deterministic
    /// regardless of the test runner's current culture.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to format.</param>
    /// <returns>The invariant-culture string representation of <paramref name="value"/>.</returns>
    internal static string FormatInvariant<T>(T value) {
        InitializerRunner.EnsureInitialized();

        var scope = _current.Value;
        if(scope is not null) {
            var scoped = scope.Formatting.Snapshot().FirstOrDefault(formatter => formatter.CanFormat(value));
            if(scoped is not null) {
                return scoped.Format(value) ?? DefaultFormat(value);
            }
        }

        var global = AssertionConfiguration.Formatting.Snapshot().FirstOrDefault(formatter => formatter.CanFormat(value));
        if(global is not null) {
            return global.Format(value) ?? DefaultFormat(value);
        }

        return DefaultFormat(value);
    }

    private static string DefaultFormat<T>(T value) => string.Format(CultureInfo.InvariantCulture, "{0}", value);

    /// <summary>
    /// Appends a <c>because</c> clause to a base failure message using sentence-style semantics.
    /// </summary>
    /// <param name="baseMessage">The default failure message, expected to end with a period.</param>
    /// <param name="because">
    /// The reason for the assertion. When it does not start with the word <c>because</c>,
    /// the word is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>
    /// <paramref name="baseMessage"/> unchanged when <paramref name="because"/> is null or whitespace;
    /// otherwise the base message (without its trailing period) followed by the reason and a closing period.
    /// </returns>
    internal static string AppendBecause(string baseMessage, string? because, object?[]? becauseArgs) {
        if(because is null) {
            return baseMessage;
        }

        var formatted = becauseArgs is { Length: > 0 }
            ? string.Format(CultureInfo.InvariantCulture, because, becauseArgs)
            : because;
        formatted = formatted.Trim();
        if(formatted.Length == 0) {
            return baseMessage;
        }

        var marker = Strings.Get(StringKeys.MESSAGE_BECAUSE);
        if(!formatted.StartsWith("because", StringComparison.OrdinalIgnoreCase) && !formatted.StartsWith(marker, StringComparison.Ordinal)) {
            formatted = marker + formatted;
        }

        var trimmed = baseMessage.TrimEnd();
        if(trimmed.EndsWithPeriod()) {
            trimmed = trimmed.WithoutLastChar();
        }

        return trimmed + " " + formatted + ".";
    }

    /// <summary>
    /// Returns the failures collected so far and clears them, so disposing this scope will not
    /// throw for them. Failures recorded after this call are still reported on dispose.
    /// </summary>
    /// <returns>The failure messages collected up to this point.</returns>
    public string[] Discard() => [.. _assertionStrategy.DiscardFailures()];

    /// <summary>
    /// Ends the scope. Merges remaining failures into the parent scope when nested;
    /// otherwise throws a single <see cref="AssertionFailedException"/> aggregating all failures.
    /// </summary>
    /// <remarks>Intentional: the scope surfaces batched failures by throwing on dispose.</remarks>
    /// <exception cref="AssertionFailedException">The scope collected at least one failure and has no parent scope.</exception>
#pragma warning disable S3877 // Build-verified: removing fails all TFMs; throwing on Dispose is the documented batching contract.
    public void Dispose() {
        if(_disposed) {
            return;
        }

        _disposed = true;
        _current.Value = _parent;

        if(_suppressThrowOnDispose) {
            return;
        }

        var failures = _assertionStrategy.FailureMessages.ToArray();
        if(failures.Length == 0) {
            return;
        }

        if(_parent is not null) {
            var prefix = _context is null ? string.Empty : $"[{_context}] ";
            foreach(var failure in failures) {
                _parent._assertionStrategy.HandleFailure(prefix + failure);
            }

            _parent.AppendTracing(_tracing.ToString());

            return;
        }

        if(failures.Length == 1) {
            throw new AssertionFailedException(WithTracing(failures[0], _tracing.ToString()));
        }

        throw new AssertionFailedException(BuildAggregateMessage(failures, _tracing.ToString()));
    }

    /// <summary>Appends diagnostic tracing to a single failure message when tracing was recorded.</summary>
    /// <param name="failure">The failure message.</param>
    /// <param name="tracing">The recorded tracing, or <see langword="null"/> when none.</param>
    /// <returns>The failure message, optionally followed by the tracing block.</returns>
    private static string WithTracing(string failure, string? tracing) =>
        string.IsNullOrEmpty(tracing) ? failure : failure + Environment.NewLine + tracing;

    /// <summary>
    /// Builds the single report message that aggregates <paramref name="failures"/>, optionally
    /// followed by diagnostic tracing.
    /// </summary>
    /// <param name="failures">The failure messages to aggregate; must contain at least two entries.</param>
    /// <param name="tracing">Optional diagnostic tracing appended after the failures.</param>
    /// <returns>The aggregated failure report.</returns>
    internal static string BuildAggregateMessage(string[] failures, string? tracing) {
        var builder = new StringBuilder();
        builder.AppendFormat(CultureInfo.InvariantCulture, Strings.Get(StringKeys.SCOPE_ASSERTION_FAILURES), FormatInvariant(failures.Length));
        for(var i = 0; i < failures.Length; i++) {
            builder.AppendLine().AppendFormat(CultureInfo.InvariantCulture, "{0}. {1}", FormatInvariant(i + 1), failures[i]);
        }

        if(!string.IsNullOrEmpty(tracing)) {
            builder.AppendLine().Append(tracing);
        }

        return builder.ToString();
    }
#pragma warning restore S3877
}
