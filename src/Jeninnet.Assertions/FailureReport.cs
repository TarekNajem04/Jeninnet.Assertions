//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Builds the JSON failure report shared by <see cref="SoftAssertions.ToReportJson"/> and <see cref="AssertionResult"/>.</summary>
internal static class FailureReport {
    /// <summary>Serializes <paramref name="failures"/> into a compact JSON document with a <c>count</c> and a <c>failures</c> array.</summary>
    /// <param name="failures">The structured failures to serialize.</param>
    /// <returns>The JSON report.</returns>
    internal static string ToJson(IReadOnlyList<AssertionFailure> failures) {
        var array = new JsonArray();
        foreach(var failure in failures) {
            array.Add(new JsonObject {
                ["expression"] = failure.Expression,
                ["category"] = failure.Category,
                ["message"] = failure.Message,
                ["actual"] = failure.Actual,
                ["expected"] = failure.Expected,
            });
        }

        var root = new JsonObject {
            ["count"] = failures.Count,
            ["failures"] = array,
        };

        return root.ToJsonString();
    }
}
