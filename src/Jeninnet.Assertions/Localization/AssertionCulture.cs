//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Localization;

/// <summary>Selects the language of assertion failure messages (default: English).</summary>
/// <example>AssertionCulture.Current = new CultureInfo("ar");</example>
public static class AssertionCulture {
    private static readonly AsyncLocal<CultureInfo> _current = new();
    private static readonly CultureInfo _english = new("en");

    /// <summary>Gets or sets the culture used for failure messages (flow-local, so parallel tests stay isolated).</summary>
    /// <exception cref="ArgumentNullException"><c>value</c> is <see langword="null"/>.</exception>
    /// <remarks>Unknown cultures fall back to the neutral (English) resources automatically.</remarks>
    public static CultureInfo Current {
        get => _current.Value ?? _english;
        set {
            if(value is null) {
                throw new ArgumentNullException(nameof(value));
            }

            _current.Value = value;
        }
    }

    /// <summary>Reads the localized message template for a resource key in the current <see cref="Current"/> culture.</summary>
    /// <param name="key">A resource key matching a <c>Strings.resx</c> entry (for example <c>BOOLEAN_BE_TRUE</c>).</param>
    /// <returns>The localized template with <c>{0}</c>, <c>{1}</c>, … placeholders, or the neutral English template when
    /// the current culture has no translation for <paramref name="key"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    /// <remarks>Placeholders are filled with pre-formatted values; prefer <see cref="FormatMessage"/> for that.</remarks>
    public static string GetMessage(string key) {
        if(key is null) {
            throw new ArgumentNullException(nameof(key));
        }

        return Strings.Get(key);
    }

    /// <summary>Formats the localized message template for a resource key with invariant argument formatting.</summary>
    /// <param name="key">A resource key matching a <c>Strings.resx</c> entry (for example <c>BOOLEAN_BE_TRUE</c>).</param>
    /// <param name="args">Arguments for the <c>{0}</c>, <c>{1}</c>, … placeholders.</param>
    /// <returns>The localized, formatted message with invariant (Latin-digit) argument formatting.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> or <paramref name="args"/> is <see langword="null"/>.</exception>
    public static string FormatMessage(string key, params object?[] args) {
        if(key is null) {
            throw new ArgumentNullException(nameof(key));
        }

        if(args is null) {
            throw new ArgumentNullException(nameof(args));
        }

        return Strings.Format(key, args);
    }
}
