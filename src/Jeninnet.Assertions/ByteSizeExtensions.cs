//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Byte-size sugar for allocation assertions (e.g. <c>64.Kilobytes()</c>).</summary>
public static class ByteSizeExtensions {
    /// <summary>Converts a byte count to bytes.</summary>
    /// <param name="count">The number of bytes.</param>
    /// <returns><paramref name="count"/> as <see cref="long"/>.</returns>
    public static long Bytes(this int count) => count;

    /// <summary>Converts a kibibyte count to bytes.</summary>
    /// <param name="count">The number of kibibytes.</param>
    /// <returns><paramref name="count"/> × 1024.</returns>
    public static long Kilobytes(this int count) => (long)count * 1024;

    /// <summary>Converts a mebibyte count to bytes.</summary>
    /// <param name="count">The number of mebibytes.</param>
    /// <returns><paramref name="count"/> × 1024 × 1024.</returns>
    public static long Megabytes(this int count) => (long)count * 1024 * 1024;
}
