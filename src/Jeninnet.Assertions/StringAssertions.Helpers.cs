//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class StringAssertions {
    /// <summary>Formats a string for failure messages, distinguishing null from empty.</summary>
    /// <param name="value">The value to format.</param>
    /// <returns><c>&lt;null&gt;</c> or the quoted text.</returns>
    private static string Format(string? value) => value is null ? "<null>" : $"'{value}'";

    /// <summary>Formats and comma-joins a sequence of strings for failure messages.</summary>
    /// <param name="values">The values to join.</param>
    /// <returns>The comma-separated formatted list.</returns>
    private static string Join(IEnumerable<string?> values) => string.Join(", ", values.Select(Format));

    /// <summary>Renders a count with its singular or plural noun ("no items", "1 item", "3 items").</summary>
    /// <param name="count">The count to render.</param>
    /// <param name="singular">The singular noun.</param>
    /// <param name="plural">The plural noun.</param>
    /// <returns>The count phrase with invariant digits.</returns>
    private static string CountWord(int count, string singular, string plural) => count switch {
        0 => $"no {plural}",
        1 => $"1 {singular}",
        _ => $"{AssertionScope.FormatInvariant(count)} {plural}",
    };

    /// <summary>Determines whether a character has case (letters, not digits or punctuation).</summary>
    /// <param name="c">The character to inspect.</param>
    /// <returns><see langword="true"/> when upper- and lower-case forms differ.</returns>
    private static bool IsCased(char c) => char.ToUpperInvariant(c) != char.ToLowerInvariant(c);

    /// <summary>Counts non-overlapping occurrences of a substring under the given comparison.</summary>
    /// <param name="value">The text to search.</param>
    /// <param name="expected">The substring to count; an empty substring counts once.</param>
    /// <param name="comparison">The comparison mode.</param>
    /// <returns>The number of non-overlapping occurrences.</returns>
    private static int CountOccurrences(string value, string expected, StringComparison comparison) {
        if(expected.Length == 0) {
            return 1;
        }

        var count = 0;
        var index = 0;
        while((index = value.IndexOf(expected, index, comparison)) >= 0) {
            count++;
            index += expected.Length;
        }

        return count;
    }

    /// <summary>Builds the effective equivalency options from an optional configuration callback.</summary>
    /// <param name="config">The configuration callback, or <see langword="null"/> for defaults.</param>
    /// <returns>The configured options instance.</returns>
    private static StringEquivalencyOptions ResolveOptions(Func<StringEquivalencyOptions, StringEquivalencyOptions>? config) =>
        config?.Invoke(new StringEquivalencyOptions()) ?? new StringEquivalencyOptions();

    /// <summary>Determines whether the normalized text contains the normalized substring.</summary>
    /// <param name="value">The text to search.</param>
    /// <param name="expected">The substring to find.</param>
    /// <param name="options">The normalization and comparison rules.</param>
    /// <returns><see langword="true"/> when the substring occurs.</returns>
    private static bool ContainsEquivalent(string value, string expected, StringEquivalencyOptions options) =>
        options.Normalize(value).IndexOf(options.Normalize(expected), options.Comparison) >= 0;

    /// <summary>Counts occurrences of a substring after applying the equivalency normalization.</summary>
    /// <param name="value">The text to search.</param>
    /// <param name="expected">The substring to count.</param>
    /// <param name="options">The normalization and comparison rules.</param>
    /// <returns>The number of non-overlapping occurrences.</returns>
    private static int CountEquivalentOccurrences(string value, string expected, StringEquivalencyOptions options) =>
        CountOccurrences(options.Normalize(value), options.Normalize(expected), options.Comparison);

    /// <summary>Determines whether the normalized text starts with the normalized prefix.</summary>
    /// <param name="value">The text to test.</param>
    /// <param name="expected">The prefix to look for.</param>
    /// <param name="options">The normalization and comparison rules.</param>
    /// <returns><see langword="true"/> when the prefix matches.</returns>
    private static bool StartsWithEquivalent(string value, string expected, StringEquivalencyOptions options) =>
        options.Normalize(value).StartsWith(options.Normalize(expected), options.Comparison);

    /// <summary>Determines whether the normalized text ends with the normalized suffix.</summary>
    /// <param name="value">The text to test.</param>
    /// <param name="expected">The suffix to look for.</param>
    /// <param name="options">The normalization and comparison rules.</param>
    /// <returns><see langword="true"/> when the suffix matches.</returns>
    private static bool EndsWithEquivalent(string value, string expected, StringEquivalencyOptions options) =>
        options.Normalize(value).EndsWith(options.Normalize(expected), options.Comparison);

    /// <summary>Determines whether the normalized text matches the normalized wildcard pattern.</summary>
    /// <param name="value">The text to test.</param>
    /// <param name="pattern">The wildcard pattern (<c>*</c>/<c>?</c>).</param>
    /// <param name="options">The normalization and comparison rules.</param>
    /// <returns><see langword="true"/> when the pattern matches.</returns>
    private static bool MatchesEquivalent(string value, string pattern, StringEquivalencyOptions options) =>
        TextMatching.WildcardMatch(options.Normalize(value), options.Normalize(pattern), options.Comparison == StringComparison.OrdinalIgnoreCase);

    /// <summary>Builds the failure message, appending a unified diff for long or multiline strings.</summary>
    /// <param name="key">The localized message template key.</param>
    /// <param name="expected">The expected text.</param>
    /// <param name="actual">The actual text.</param>
    /// <param name="because">The reason clause, when supplied.</param>
    /// <param name="becauseArgs">The reason arguments, when supplied.</param>
    /// <returns>The formatted message with an optional diff block.</returns>
    private string WithDiff(string key, string? expected, string? actual, string? because, object?[] becauseArgs) {
        var shownExpected = expected;
        var shownActual = actual;
        string? diff = null;
        if(expected is not null && actual is not null && TextDiff.ShouldShowDiff(expected, actual)) {
            diff = TextDiff.UnifiedDiff(expected, actual);
            if(diff is not null) {
                shownExpected = TextDiff.TruncatePreview(expected);
                shownActual = TextDiff.TruncatePreview(actual);
            }
        }

        var message = AssertionScope.AppendBecause(Strings.Format(key, Expression, Format(shownExpected), Format(shownActual)), because, becauseArgs);
        return diff is null ? message : message + "\n" + diff;
    }
}
