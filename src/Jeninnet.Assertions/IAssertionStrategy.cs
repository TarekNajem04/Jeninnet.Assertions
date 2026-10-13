//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>
/// Plugs failure reporting into assertions and scopes so custom test frameworks can decide how failures are
/// collected, reported or thrown.
/// </summary>
public interface IAssertionStrategy {
    /// <summary>Gets the messages for the assertion failures that happened so far.</summary>
    IEnumerable<string> FailureMessages { get; }

    /// <summary>Instructs the strategy to record an assertion failure.</summary>
    /// <param name="message">The fully formatted failure message.</param>
    void HandleFailure(string message);

    /// <summary>Discards the failure messages recorded so far and returns them.</summary>
    /// <returns>The failures that happened up to now.</returns>
    IEnumerable<string> DiscardFailures();

    /// <summary>
    /// Throws an aggregated <see cref="AssertionFailedException"/> when at least one failure was recorded.
    /// </summary>
    /// <param name="context">Diagnostic context pairs appended after the failure messages.</param>
    void ThrowIfAny(IDictionary<string, object> context);
}

/// <summary>Collects every failure without throwing, so a scope can report all of them together on dispose.</summary>
internal sealed class CollectingAssertionStrategy : IAssertionStrategy {
    private readonly List<string> _failureMessages = [];
    private readonly List<AssertionFailure> _failures = [];

    /// <inheritdoc/>
    public IEnumerable<string> FailureMessages => _failureMessages;

    /// <summary>Gets the structured failures recorded so far, in report order.</summary>
    internal IReadOnlyList<AssertionFailure> Failures => _failures;

    /// <summary>Records a structured failure with its full message.</summary>
    /// <param name="failure">The structured failure to record.</param>
    internal void AddFailure(AssertionFailure failure) {
        _failureMessages.Add(failure.Message);
        _failures.Add(failure);
    }

    /// <inheritdoc/>
    public void HandleFailure(string message) {
        _failureMessages.Add(message);
        _failures.Add(new AssertionFailure(string.Empty, string.Empty, message));
    }

    /// <inheritdoc/>
    public IEnumerable<string> DiscardFailures() {
        var discarded = _failureMessages.ToArray();
        _failureMessages.Clear();
        _failures.Clear();
        return discarded;
    }

    /// <inheritdoc/>
    public void ThrowIfAny(IDictionary<string, object> context) {
        _ = context ?? throw new ArgumentNullException(nameof(context));

        if(_failureMessages.Count == 0) {
            return;
        }

        var builder = new StringBuilder();
        builder.Append(string.Join(Environment.NewLine, _failureMessages)).AppendLine();
        foreach(var pair in context) {
            builder.AppendFormat(CultureInfo.InvariantCulture, "\nWith {0}:\n{1}", pair.Key, pair.Value);
        }

        throw new AssertionFailedException(builder.ToString());
    }
}
