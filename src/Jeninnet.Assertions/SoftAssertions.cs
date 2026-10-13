//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>
/// Collects the failures of multiple assertions and reports them as a single <see cref="AssertionFailedException"/>
/// on demand, without requiring a <c>using</c> block or automatic disposal.
/// </summary>
/// <remarks>
/// <para>
/// Call <see cref="Collect"/> to start collecting, run any number of assertions against subjects, then call
/// <see cref="ThenReport"/> to surface every collected failure at once. Assertions keep their default behavior:
/// outside a soft collection every failure still throws immediately, and assertions that must return a value
/// (<c>Throw*</c>, <c>ContainSingle</c>) throw immediately even while collecting.
/// </para>
/// <para>
/// <see cref="Collect"/> is idempotent: calling it again while already collecting is a no-op, and calling it
/// after a report starts a fresh collection. The type is <see cref="IDisposable"/> so <c>using</c> also works;
/// disposal reports any pending failures exactly like <see cref="ThenReport"/>.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// var soft = new SoftAssertions().Collect();
/// flag.Should().BeTrue();
/// actual.Should().Be("expected");
/// soft.ThenReport();
/// </code>
/// </example>
/// <seealso cref="AssertionScope"/>
public sealed class SoftAssertions : IDisposable {
    private AssertionScope? _scope;

    /// <summary>
    /// Gets a snapshot of the structured failures collected so far, in report order. Empty until
    /// <see cref="Collect"/> is called or after a report, and cleared by <see cref="ThenReport"/>.
    /// </summary>
    public IReadOnlyList<AssertionFailure> Failures {
        get {
            var scope = _scope;
            return scope is null ? Array.Empty<AssertionFailure>() : [.. scope.StructuredFailures];
        }
    }

    /// <summary>
    /// Serializes the failures collected so far into human-readable JSON for CI attachments or dry runs:
    /// each entry carries <c>expression</c>, <c>category</c>, <c>message</c>, and the optional
    /// <c>actual</c>/<c>expected</c> values reported by the assertion. Never throws.
    /// </summary>
    /// <returns>A compact JSON document with a <c>count</c> and a <c>failures</c> array.</returns>
    /// <example>
    /// <code>
    /// var json = soft.ToReportJson();
    /// </code>
    /// </example>
    public string ToReportJson() => FailureReport.ToJson(Failures);

    /// <summary>
    /// Starts collecting failures instead of throwing on the first one. Calling while already collecting is
    /// a no-op; calling after a report starts a fresh collection.
    /// </summary>
    /// <returns>This instance, so assertions can flow naturally afterwards.</returns>
    public SoftAssertions Collect() {
        _scope ??= new AssertionScope(new CollectingAssertionStrategy(), null, suppressThrowOnDispose: true);
        return this;
    }

    /// <summary>
    /// Ends the collection, restores the ambient assertion scope that was active before <see cref="Collect"/>,
    /// and throws a single <see cref="AssertionFailedException"/> aggregating every failure; a no-op when the
    /// collection was clean.
    /// </summary>
    /// <exception cref="InvalidOperationException"><see cref="Collect"/> was not called (or the collection was already reported).</exception>
    /// <exception cref="AssertionFailedException">At least one failure was collected.</exception>
    public void ThenReport() {
        var failures = Finish();
        if(failures.Length == 0) {
            return;
        }

        throw new AssertionFailedException(ToReport(failures));
    }

    /// <summary>
    /// Reports any pending failures exactly like <see cref="ThenReport"/>. Provided so <c>using</c> works as an
    /// alternative to an explicit report.
    /// </summary>
    /// <exception cref="AssertionFailedException">At least one failure is pending.</exception>
    /// <remarks>Intentional: like <see cref="AssertionScope"/>, disposal surfaces uncollected failures instead of dropping them.</remarks>
    public void Dispose() => ThenReport();

    /// <summary>Unwinds the active collection and returns the failure messages, clearing the collector.</summary>
    /// <returns>The collected failure messages.</returns>
    /// <exception cref="InvalidOperationException">The collector was never started with <c>Collect()</c>.</exception>
    private string[] Finish() {
        var scope = _scope
            ?? throw new InvalidOperationException("SoftAssertions: call Collect() before reporting soft assertion failures.");
        _scope = null;
        var failures = scope.Discard();
        scope.Dispose();
        return failures;
    }

    /// <summary>
    /// Unwinds the active collection without reporting: returns the structured failures collected so far and
    /// restores the ambient scope. Used by <see cref="AssertionResult"/> for the non-throwing validator mode.
    /// </summary>
    /// <returns>The structured failures collected, as a snapshot.</returns>
    internal IReadOnlyList<AssertionFailure> FinishSilently() {
        var scope = _scope;
        if(scope is null) {
            return [];
        }

        _scope = null;
        AssertionFailure[] failures = [.. scope.StructuredFailures];
        scope.Discard();
        scope.Dispose();
        return failures;
    }

    /// <summary>Renders the failure report: a single message alone, otherwise the aggregate format.</summary>
    /// <param name="failures">The failure messages.</param>
    /// <returns>The report text.</returns>
    private static string ToReport(string[] failures) =>
        failures.Length == 1 ? failures[0] : AssertionScope.BuildAggregateMessage(failures, null);
}
