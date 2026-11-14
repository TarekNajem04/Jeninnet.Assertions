//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Captures the library's ambient state and restores it on dispose, so test-local changes never leak into the next test.</summary>
/// <remarks>
/// One <c>using(AssertionIsolation.Isolate())</c> snapshots and restores the ambient knobs:
/// <see cref="AssertionCulture.Current"/>, <see cref="AssertionClock.Current"/>,
/// <see cref="AssertionConfiguration.Formatting"/> (formatters plus formatting limits), the active
/// <see cref="AssertionScope.Expectation"/>, and the equivalency template set with
/// <see cref="EquivalencyDefaults.Modify"/>. Restoration happens even when the scope exits through an exception,
/// nested scopes each restore their own snapshot, and the culture/clock/equivalency parts are per-async-flow
/// (<see cref="AsyncLocal{T}"/>) like every other ambient setting. Two boundaries are deliberate: the global
/// formatter registry is process-wide, so parallel tests that both mutate it still need their own discipline
/// (the scope only prevents sequential leakage), and state outside this library (for example
/// <c>CultureInfo.CurrentCulture</c> of the thread) is not touched.
/// </remarks>
public static class AssertionIsolation {
    /// <summary>Starts an isolation scope that snapshots the ambient state and restores it when the returned disposable is disposed.</summary>
    /// <returns>The scope to dispose (typically via <c>using</c>).</returns>
    public static IDisposable Isolate() => new Scope();

    private sealed class Scope : IDisposable {
        private readonly CultureInfo _culture = AssertionCulture.Current;
        private readonly IClock _clock = AssertionClock.Current;
        private readonly ValueFormatterOptions _formatting = AssertionConfiguration.Formatting.Copy();
        private readonly AssertionScope? _scope = AssertionScope.Current;
        private readonly string? _expectation = AssertionScope.Current?.Expectation;
        private readonly SelfReferenceEquivalencyOptions? _equivalency = EquivalencyDefaults.CaptureAmbient();
        private bool _disposed;

        public void Dispose() {
            if(_disposed) {
                return;
            }

            _disposed = true;
            AssertionCulture.Current = _culture;
            AssertionClock.Current = _clock;
            AssertionConfiguration.Formatting.RestoreFrom(_formatting);
            _scope?.RestoreExpectation(_expectation);
            EquivalencyDefaults.RestoreAmbient(_equivalency);
        }
    }
}
