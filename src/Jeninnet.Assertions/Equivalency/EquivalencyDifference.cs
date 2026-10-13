//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Equivalency;

/// <summary>One structured difference produced by <see cref="EquivalencyEvaluation.Evaluate{TExpectation}"/>: where the graphs differ and how.</summary>
/// <remarks>
/// The message matches the text <c>BeEquivalentTo</c> reports for the same difference, and the formatted
/// expected/actual values are present whenever the difference site knows them (value, string, type, and count
/// mismatches; missing members and cyclic references carry a path-only entry). Differences are invariant
/// diagnostics prose and are never localized.
/// </remarks>
public sealed class EquivalencyDifference {
    /// <summary>Creates a structured difference (engine plumbing).</summary>
    /// <param name="path">The formatted member path from the root, or <c>&lt;root&gt;</c> for the graph root.</param>
    /// <param name="message">The invariant difference sentence.</param>
    /// <param name="expected">The formatted expected value, or <see langword="null"/> when the site does not report one.</param>
    /// <param name="actual">The formatted actual value, or <see langword="null"/> when the site does not report one.</param>
    internal EquivalencyDifference(string path, string message, string? expected, string? actual) {
        Path = path;
        Message = message;
        Expected = expected;
        Actual = actual;
    }

    /// <summary>Gets the formatted member path from the root (dotted/bracketed, e.g. <c>Address.City</c>, <c>Tags[0]</c>; <c>&lt;root&gt;</c> at the root).</summary>
    public string Path { get; }

    /// <summary>Gets the invariant difference sentence (identical to the text <c>BeEquivalentTo</c> would report).</summary>
    public string Message { get; }

    /// <summary>Gets the formatted expected value, or <see langword="null"/> when the difference site does not report one.</summary>
    public string? Expected { get; }

    /// <summary>Gets the formatted actual value, or <see langword="null"/> when the difference site does not report one.</summary>
    public string? Actual { get; }
}
