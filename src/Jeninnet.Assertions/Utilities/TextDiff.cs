//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Utilities;

/// <summary>Builds deterministic git-style unified diffs for failure messages.</summary>
/// <remarks>
/// Internal on purpose: the rendering is a message detail (like occurrence counts), so it stays
/// invariant across cultures while the surrounding message is localized. Designed for reuse by
/// future JSON/XML/snapshot diagnostics. Output always uses <c>\n</c> (never <c>Environment.NewLine</c>)
/// so messages are byte-identical on every machine.
/// </remarks>
internal static class TextDiff {
    /// <summary>Context lines shown around each change.</summary>
    internal const int CONTEXT_LINES = 3;

    /// <summary>A side longer than this (or any line break) earns a diff block.</summary>
    internal const int DIFF_LENGTH_THRESHOLD = 200;

    /// <summary>One-liner values are cut here when a diff block is shown.</summary>
    internal const int PREVIEW_LENGTH = 100;

    /// <summary>Upper bound for the comparison table (lines times lines); larger inputs skip the diff.</summary>
    private const long MAX_DIFF_CELLS = 1000000;

    /// <summary>Determines whether a diff block adds value for this pair (either side long or multiline).</summary>
    /// <param name="expected">The expected text (may be <see langword="null"/>).</param>
    /// <param name="actual">The actual text (may be <see langword="null"/>).</param>
    /// <returns><see langword="true"/> when both sides exist and either is long or multiline.</returns>
    internal static bool ShouldShowDiff(string? expected, string? actual) =>
        expected is not null && actual is not null
        && (IsLongOrMultiline(expected) || IsLongOrMultiline(actual));

    /// <summary>Cuts <paramref name="value"/> to <see cref="PREVIEW_LENGTH"/> characters with an ellipsis.</summary>
    /// <param name="value">The text to shorten.</param>
    /// <returns>The original text, or its prefix followed by <c>…</c>.</returns>
    // Range indexer needs System.Range (absent on netstandard2.0); Remove is portable.
    internal static string TruncatePreview(string value) =>
        value.Length > PREVIEW_LENGTH ? value.Remove(PREVIEW_LENGTH) + "…" : value;

    /// <summary>Returns a unified diff of <paramref name="expected"/> vs <paramref name="actual"/>.</summary>
    /// <param name="expected">The expected text.</param>
    /// <param name="actual">The actual text.</param>
    /// <returns>The diff block, or <see langword="null"/> when the inputs are too large to diff safely.</returns>
    /// <remarks>Line endings are preserved as-is (split on <c>\n</c> only), so a <c>\r\n</c> vs <c>\n</c>
    /// difference surfaces as a changed-line pair. Line comparison is ordinal and deterministic.</remarks>
    internal static string? UnifiedDiff(string expected, string actual) {
        var expectedLines = SplitLines(expected);
        var actualLines = SplitLines(actual);
        if((long)expectedLines.Length * actualLines.Length > MAX_DIFF_CELLS) {
            return null;
        }

        var operations = DiffOperations(expectedLines, actualLines);
        var builder = new StringBuilder();
        builder.Append("--- expected\n+++ actual\n");
        foreach(var hunk in GroupHunks(operations, CONTEXT_LINES)) {
            builder.AppendFormat(
                CultureInfo.InvariantCulture,
                "@@ -{0},{1} +{2},{3} @@\n",
                HunkStart(hunk.ExpectedStart, hunk.ExpectedCount),
                hunk.ExpectedCount,
                HunkStart(hunk.ActualStart, hunk.ActualCount),
                hunk.ActualCount);
            foreach(var (kind, text) in hunk.Lines) {
                builder.Append(kind).Append(text).Append('\n');
            }
        }

        return builder.ToString().TrimEnd('\n');
    }

    /// <summary>Determines whether a value earns a diff block (longer than the threshold or multiline).</summary>
    /// <param name="value">The text to inspect.</param>
    /// <returns><see langword="true"/> when the text is long or contains a line break.</returns>
    private static bool IsLongOrMultiline(string value) =>
        value.Length > DIFF_LENGTH_THRESHOLD || value.IndexOf('\n') >= 0;

    /// <summary>Splits text into lines on <c>\n</c> only, preserving any <c>\r</c> characters.</summary>
    /// <param name="value">The text to split; an empty string yields no lines.</param>
    /// <returns>The line array.</returns>
    private static string[] SplitLines(string value) =>
        value.Length == 0 ? [] : value.Split('\n');

    /// <summary>Converts a zero-based operation index to the one-based line number unified diffs use.</summary>
    /// <param name="start">The zero-based index.</param>
    /// <param name="count">The number of lines in the range.</param>
    /// <returns>The one-based line number, or the unchanged index for empty ranges.</returns>
    private static int HunkStart(int start, int count) => count == 0 ? start : start + 1;

    private enum EditKind {
        Equal = 0,
        Delete = 1,
        Insert = 2,
    }

    private sealed class Hunk {
        internal int ExpectedStart;
        internal int ExpectedCount;
        internal int ActualStart;
        internal int ActualCount;
        internal readonly List<(char Kind, string Text)> Lines = [];
    }

    /// <summary>Walks the LCS table to produce the ordered edit script between two line arrays.</summary>
    /// <param name="expected">The expected lines.</param>
    /// <param name="actual">The actual lines.</param>
    /// <returns>One operation per line (equal, delete, or insert) in presentation order.</returns>
    private static List<(EditKind Kind, int ExpectedIndex, int ActualIndex, string Text)> DiffOperations(string[] expected, string[] actual) {
        var stride = actual.Length + 1;
        var lengths = ComputeLcsLengths(expected, actual, stride);

        var operations = new List<(EditKind Kind, int ExpectedIndex, int ActualIndex, string Text)>();
        var expectedIndex = 0;
        var actualIndex = 0;
        while(expectedIndex < expected.Length || actualIndex < actual.Length) {
            if(expectedIndex < expected.Length && actualIndex < actual.Length && expected[expectedIndex] == actual[actualIndex]) {
                operations.Add((EditKind.Equal, expectedIndex, actualIndex, expected[expectedIndex]));
                expectedIndex++;
                actualIndex++;
            } else if(actualIndex >= actual.Length
                || (expectedIndex < expected.Length && lengths[((expectedIndex + 1) * stride) + actualIndex] >= lengths[(expectedIndex * stride) + actualIndex + 1])) {
                operations.Add((EditKind.Delete, expectedIndex, actualIndex, expected[expectedIndex]));
                expectedIndex++;
            } else {
                operations.Add((EditKind.Insert, expectedIndex, actualIndex, actual[actualIndex]));
                actualIndex++;
            }
        }

        return operations;
    }

    /// <summary>Computes the longest-common-subsequence suffix-length table used by the traceback walk.</summary>
    /// <param name="expected">The expected lines.</param>
    /// <param name="actual">The actual lines.</param>
    /// <param name="stride">The table row width (<c>actual.Length + 1</c>).</param>
    /// <returns>The flattened suffix-length table.</returns>
    private static int[] ComputeLcsLengths(string[] expected, string[] actual, int stride) {
        var lengths = new int[(expected.Length + 1) * stride];
        for(var i = expected.Length - 1; i >= 0; i--) {
            for(var j = actual.Length - 1; j >= 0; j--) {
                lengths[(i * stride) + j] = expected[i] == actual[j]
                    ? lengths[((i + 1) * stride) + j + 1] + 1
                    : Math.Max(lengths[((i + 1) * stride) + j], lengths[(i * stride) + j + 1]);
            }
        }

        return lengths;
    }

    /// <summary>Groups adjacent edits into hunks with the configured number of context lines.</summary>
    /// <param name="operations">The full edit script.</param>
    /// <param name="contextLines">The context lines kept around each change group.</param>
    /// <returns>The hunks in order, each with its start positions, counts, and rendered lines.</returns>
    private static List<Hunk> GroupHunks(List<(EditKind Kind, int ExpectedIndex, int ActualIndex, string Text)> operations, int contextLines) {
        var changes = new List<int>();
        for(var index = 0; index < operations.Count; index++) {
            if(operations[index].Kind != EditKind.Equal) {
                changes.Add(index);
            }
        }

        var hunks = new List<Hunk>();
        var groupStart = 0;
        while(groupStart < changes.Count) {
            var groupEnd = groupStart;
            while(groupEnd + 1 < changes.Count && changes[groupEnd + 1] - changes[groupEnd] <= 2 * contextLines) {
                groupEnd++;
            }

            var first = Math.Max(0, changes[groupStart] - contextLines);
            var last = Math.Min(operations.Count - 1, changes[groupEnd] + contextLines);
            var hunk = new Hunk();
            for(var index = first; index <= last; index++) {
                var (kind, expected, actual, text) = operations[index];
                if(hunk.Lines.Count == 0) {
                    hunk.ExpectedStart = expected;
                    hunk.ActualStart = actual;
                }

                if(kind == EditKind.Equal) {
                    hunk.Lines.Add((' ', text));
                    hunk.ExpectedCount++;
                    hunk.ActualCount++;
                } else if(kind == EditKind.Delete) {
                    hunk.Lines.Add(('-', text));
                    hunk.ExpectedCount++;
                } else {
                    hunk.Lines.Add(('+', text));
                    hunk.ActualCount++;
                }
            }

            hunks.Add(hunk);
            groupStart = groupEnd + 1;
        }

        return hunks;
    }
}
