//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Suggests the nearest existing names when a lookup fails, so typos explain themselves in the failure message.</summary>
/// <remarks>
/// Candidate names within an edit distance of <c>input.Length / 3</c> qualify (transpositions count as one edit,
/// inputs shorter than three characters yield no suggestions because every short name would match), ordered by
/// distance and then ordinally; large candidate sets only slow the failure path, never the success path.
/// </remarks>
internal static class NearestCandidate {
    /// <summary>Returns up to <paramref name="max"/> existing names near <paramref name="input"/>.</summary>
    /// <param name="input">The looked-up name that was not found (a <see langword="null"/>, empty, or very short name yields no suggestions).</param>
    /// <param name="candidates">The existing names to draw suggestions from.</param>
    /// <param name="max">The maximum number of suggestions to return.</param>
    /// <returns>The nearest names, ordered by edit distance and then ordinally.</returns>
    internal static IReadOnlyList<string> Suggest(string? input, IEnumerable<string> candidates, int max = 3) {
        if(input is null || input.Length < 3) {
            return [];
        }

        var threshold = input.Length / 3;
        var matches = new List<(int Distance, string Value)>();
        foreach(var candidate in candidates) {
            if(candidate is null || Math.Abs(candidate.Length - input.Length) > threshold) {
                continue;
            }

            var distance = Distance(input, candidate);
            if(distance != 0 && distance <= threshold) {
                matches.Add((distance, candidate));
            }
        }

        matches.Sort(static (left, right) => {
            var byDistance = left.Distance.CompareTo(right.Distance);
            return byDistance != 0 ? byDistance : string.CompareOrdinal(left.Value, right.Value);
        });

        var suggestions = new List<string>(Math.Min(max, matches.Count));
        for(var index = 0; index < matches.Count && suggestions.Count < max; index++) {
            suggestions.Add(matches[index].Value);
        }

        return suggestions;
    }

    /// <summary>Builds the localized <c>Did you mean …?</c> suffix, or an empty string when there is nothing to suggest.</summary>
    /// <param name="input">The looked-up name that was not found.</param>
    /// <param name="candidates">The existing names to draw suggestions from.</param>
    /// <param name="max">The maximum number of suggestions to include.</param>
    /// <returns>The suffix to append to the failure message (leading space included), or an empty string.</returns>
    internal static string Describe(string? input, IEnumerable<string> candidates, int max = 3) {
        var suggestions = Suggest(input, candidates, max);
        if(suggestions.Count == 0) {
            return string.Empty;
        }

        if(suggestions.Count == 1) {
            return " " + Strings.Format(StringKeys.SUGGESTION_SINGLE, suggestions[0]);
        }

        return " " + Strings.Format(StringKeys.SUGGESTION_MULTIPLE, "'" + string.Join("', '", suggestions) + "'");
    }

    /// <summary>Computes the optimal-string-alignment (Damerau-Levenshtein) distance, where a transposition costs one edit.</summary>
    /// <param name="left">The first string.</param>
    /// <param name="right">The second string.</param>
    /// <returns>The edit distance between the two strings.</returns>
    private static int Distance(string left, string right) {
        var leftLength = left.Length;
        var rightLength = right.Length;
        var beforePrevious = new int[rightLength + 1];
        var previous = new int[rightLength + 1];
        var current = new int[rightLength + 1];
        for(var index = 0; index <= rightLength; index++) {
            beforePrevious[index] = index;
            previous[index] = index;
        }

        for(var row = 1; row <= leftLength; row++) {
            current[0] = row;
            for(var column = 1; column <= rightLength; column++) {
                var cost = left[row - 1] == right[column - 1] ? 0 : 1;
                current[column] = Math.Min(Math.Min(previous[column] + 1, current[column - 1] + 1), previous[column - 1] + cost);
                if(row > 1 && column > 1 && left[row - 1] == right[column - 2] && left[row - 2] == right[column - 1]) {
                    current[column] = Math.Min(current[column], beforePrevious[column - 2] + 1);
                }
            }

            (beforePrevious, previous, current) = (previous, current, beforePrevious);
        }

        return previous[rightLength];
    }
}
