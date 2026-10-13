//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Extensions;

internal static class StringExtensions {
    /// <summary>Removes the last character of a non-empty string.</summary>
    /// <param name="value">The string to trim; must not be empty.</param>
    /// <returns>The string without its final character.</returns>
    internal static string WithoutLastChar(this string value) =>
#if NETSTANDARD2_0
        value.Remove(value.Length - 1);
#else
        value[..^1];
#endif

    /// <summary>Keeps the first <paramref name="length"/> characters of a string.</summary>
    /// <param name="value">The string to truncate.</param>
    /// <param name="length">The number of characters to keep.</param>
    /// <returns>The truncated string.</returns>
    internal static string TruncateTo(this string value, int length) =>
#if NETSTANDARD2_0
        value.Remove(length);
#else
        value[..length];
#endif

    /// <summary>Determines whether a string ends with a period.</summary>
    /// <param name="value">The string to inspect.</param>
    /// <returns><see langword="true"/> when the last character is a period.</returns>
    internal static bool EndsWithPeriod(this string value) =>
#if NETSTANDARD2_0
        value.EndsWith(".", StringComparison.Ordinal);
#else
        value.EndsWith('.');
#endif
}
