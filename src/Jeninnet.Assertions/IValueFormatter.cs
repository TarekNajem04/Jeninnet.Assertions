//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Formats values of supported types inside failure messages. Register via scoped or global formatting options.</summary>
/// <remarks>
/// This contract intentionally stays narrow rather than mirroring similarly named interfaces elsewhere:
/// widening it would break every existing formatter without adding capability.
/// Formatters only affect display, never comparison.
/// </remarks>
public interface IValueFormatter {
    /// <summary>Determines whether this formatter handles <paramref name="value"/>.</summary>
    /// <param name="value">The value to check.</param>
    /// <returns><see langword="true"/> when <see cref="Format"/> supports it.</returns>
    bool CanFormat(object? value);

    /// <summary>Formats <paramref name="value"/> for display inside a failure message.</summary>
    /// <param name="value">The value to format.</param>
    /// <returns>The display string (may return <see langword="null"/> to fall back to default formatting).</returns>
    string? Format(object? value);
}
