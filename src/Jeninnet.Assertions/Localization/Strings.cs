//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Localization;

/// <summary>Reads localizable assertion failure messages for the current <see cref="AssertionCulture"/>.</summary>
internal static class Strings {
    private static readonly ResourceManager _manager = new(typeof(Strings));

    /// <summary>Gets the message template for <paramref name="key"/> in the current culture (neutral English fallback).</summary>
    /// <param name="key">A <see cref="StringKeys"/> constant matching a <c>Strings.resx</c> entry.</param>
    /// <returns>The localized template, or <paramref name="key"/> when no resource exists.</returns>
    internal static string Get(string key) =>
        _manager.GetString(key, AssertionCulture.Current) ?? key;

    /// <summary>Formats the message template for <paramref name="key"/> with invariant argument formatting.</summary>
    /// <param name="key">A <see cref="StringKeys"/> constant matching a <c>Strings.resx</c> entry.</param>
    /// <param name="args">Arguments for the <c>{0}</c>, <c>{1}</c>, … placeholders (pre-format values invariantly).</param>
    /// <returns>The localized failure message with Latin digits in every language.</returns>
    /// <remarks>
    /// Arguments are formatted with <see cref="CultureInfo.InvariantCulture"/> so failure messages stay
    /// byte-stable across machines; only the surrounding prose is translated.
    /// </remarks>
    internal static string Format(string key, params object?[] args) =>
        string.Format(CultureInfo.InvariantCulture, Get(key), args);
}
