//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Utilities;

/// <summary>Shared wildcard matching (<c>*</c> for zero or more characters, <c>?</c> for exactly one).</summary>
internal static class TextMatching {
    /// <summary>Determines whether <paramref name="value"/> fully matches <paramref name="pattern"/>.</summary>
    /// <param name="value">The value to test.</param>
    /// <param name="pattern">The wildcard pattern.</param>
    /// <param name="ignoreCase">Whether casing differences are ignored.</param>
    /// <returns><see langword="true"/> when the whole value matches; otherwise <see langword="false"/>.</returns>
    internal static bool WildcardMatch(string value, string pattern, bool ignoreCase) {
        var valueIndex = 0;
        var patternIndex = 0;
        var starIndex = -1;
        var matchIndex = 0;

        bool Equals(char left, char right) =>
            ignoreCase ? char.ToUpperInvariant(left) == char.ToUpperInvariant(right) : left == right;

        while(valueIndex < value.Length) {
            if(patternIndex < pattern.Length && (pattern[patternIndex] == '?' || Equals(value[valueIndex], pattern[patternIndex]))) {
                valueIndex++;
                patternIndex++;
            } else if(patternIndex < pattern.Length && pattern[patternIndex] == '*') {
                starIndex = patternIndex++;
                matchIndex = valueIndex;
            } else if(starIndex != -1) {
                patternIndex = starIndex + 1;
                valueIndex = ++matchIndex;
            } else {
                return false;
            }
        }

        while(patternIndex < pattern.Length && pattern[patternIndex] == '*') {
            patternIndex++;
        }

        return patternIndex == pattern.Length;
    }
}
