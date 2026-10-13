//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Equivalency;

/// <summary>Evaluates object-graph equivalency without throwing, for production validators and CI reports.</summary>
/// <remarks>
/// The evaluation uses the same engine as <c>BeEquivalentTo</c>, so every difference message matches the
/// assertion text, and the returned result exposes the structured differences (path, message, expected,
/// actual) plus a JSON report. Use <c>AssertionResult</c> (chapter 11) when a full validation pass over
/// several assertion families is needed; use this API when one graph comparison must return a verdict.
/// </remarks>
public static class EquivalencyEvaluation {
    /// <summary>Compares <paramref name="actual"/> against <paramref name="expected"/> under <paramref name="configure"/> and returns the verdict without throwing on differences.</summary>
    /// <typeparam name="TExpectation">The compile-time type of the expectation.</typeparam>
    /// <param name="actual">The actual object graph.</param>
    /// <param name="expected">The expected object graph.</param>
    /// <param name="configure">Optional equivalency configuration (exclusions, tolerances, ordering, custom steps).</param>
    /// <returns>The evaluation result (never a thrown failure for graph differences).</returns>
    public static EquivalencyEvaluationResult Evaluate<TExpectation>(
        object? actual,
        TExpectation? expected,
        Action<SelfReferenceEquivalencyOptions<TExpectation>>? configure = null) {
        var options = new SelfReferenceEquivalencyOptions<TExpectation>();
        configure?.Invoke(options);
        return new EquivalencyEvaluationResult(EquivalencyEngine.FindDifferenceEntries(actual, expected, options));
    }
}
