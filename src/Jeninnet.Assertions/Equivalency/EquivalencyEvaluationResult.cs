//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Equivalency;

/// <summary>The non-throwing outcome of <see cref="EquivalencyEvaluation.Evaluate{TExpectation}"/>: a verdict plus every structured difference.</summary>
/// <remarks>
/// The difference order is part of the contract (it follows the comparison, and member comparisons are ordinal)
/// because CI consumes <see cref="ToReportJson"/>. Messages are invariant diagnostics prose and are never
/// localized, exactly like <c>BeEquivalentTo</c> failures.
/// </remarks>
public sealed class EquivalencyEvaluationResult {
    /// <summary>Creates the result (evaluation plumbing).</summary>
    /// <param name="differences">The structured differences in comparison order.</param>
    internal EquivalencyEvaluationResult(IReadOnlyList<EquivalencyDifference> differences) {
        Differences = differences;
        Message = ComposeMessage(differences);
    }

    /// <summary>Gets a value indicating whether the graphs are equivalent (no differences).</summary>
    public bool IsEquivalent => Differences.Count == 0;

    /// <summary>Gets the structured differences in comparison order (empty when equivalent).</summary>
    public IReadOnlyList<EquivalencyDifference> Differences { get; }

    /// <summary>Gets the unified failure text when differences exist, or an empty string when the graphs are equivalent.</summary>
    public string Message { get; }

    /// <summary>Serializes the verdict and differences into a compact JSON document for CI attachments or dry runs.</summary>
    /// <returns>The JSON report (<c>isEquivalent</c>, <c>count</c>, and a <c>differences</c> array with <c>path</c>/<c>message</c>/<c>expected</c>/<c>actual</c>).</returns>
    public string ToReportJson() {
        var array = new JsonArray();
        foreach(var difference in Differences) {
            array.Add(new JsonObject {
                ["path"] = difference.Path,
                ["message"] = difference.Message,
                ["expected"] = difference.Expected,
                ["actual"] = difference.Actual,
            });
        }

        var root = new JsonObject {
            ["isEquivalent"] = IsEquivalent,
            ["count"] = Differences.Count,
            ["differences"] = array,
        };

        return root.ToJsonString();
    }

    /// <summary>Composes the unified failure text for the collected differences.</summary>
    /// <param name="differences">The structured differences in comparison order.</param>
    /// <returns>An empty string when equivalent; otherwise a single sentence or a numbered list.</returns>
    private static string ComposeMessage(IReadOnlyList<EquivalencyDifference> differences) {
        if(differences.Count == 0) {
            return string.Empty;
        }

        if(differences.Count == 1) {
            return $"Expected the graphs to be equivalent, but {differences[0].Message.TrimEnd('.')}.";
        }

        var builder = new StringBuilder();
        builder.AppendFormat(CultureInfo.InvariantCulture, "Expected the graphs to be equivalent, but found {0} difference(s):", AssertionScope.FormatInvariant(differences.Count));
        for(var index = 0; index < differences.Count; index++) {
            builder.AppendLine().AppendFormat(CultureInfo.InvariantCulture, "{0}. {1}", AssertionScope.FormatInvariant(index + 1), differences[index].Message);
        }

        return builder.ToString();
    }
}
