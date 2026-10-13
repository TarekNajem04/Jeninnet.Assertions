//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>
/// Configures case- and whitespace-insensitive string comparisons for assertions such as
/// <c>BeEquivalentTo("text", o => o.IgnoringLeadingWhitespace())</c>.
/// </summary>
public class StringEquivalencyOptions {
    /// <summary>Gets whether casing differences are ignored. Enabled by default.</summary>
    public bool IgnoreCase { get; private set; } = true;

    /// <summary>Gets whether leading whitespace differences are ignored.</summary>
    public bool IgnoreLeadingWhitespace { get; private set; }

    /// <summary>Gets whether trailing whitespace differences are ignored.</summary>
    public bool IgnoreTrailingWhitespace { get; private set; }

    /// <summary>Gets whether <c>\r\n</c> and <c>\r</c> are treated as <c>\n</c> before comparing.</summary>
    public bool IgnoreNewlineStyle { get; private set; }

    /// <summary>Gets the custom comparer used for the final comparison, or <see langword="null"/> for ordinal comparison.</summary>
    public IEqualityComparer<string>? Comparer { get; private set; }

    /// <summary>Ignores casing differences.</summary>
    /// <returns>This options instance for chaining.</returns>
    public StringEquivalencyOptions IgnoringCase() {
        IgnoreCase = true;
        return this;
    }

    /// <summary>Ignores leading whitespace differences.</summary>
    /// <returns>This options instance for chaining.</returns>
    public StringEquivalencyOptions IgnoringLeadingWhitespace() {
        IgnoreLeadingWhitespace = true;
        return this;
    }

    /// <summary>Ignores trailing whitespace differences.</summary>
    /// <returns>This options instance for chaining.</returns>
    public StringEquivalencyOptions IgnoringTrailingWhitespace() {
        IgnoreTrailingWhitespace = true;
        return this;
    }

    /// <summary>Treats <c>\r\n</c> and <c>\r</c> as <c>\n</c> before comparing.</summary>
    /// <returns>This options instance for chaining.</returns>
    public StringEquivalencyOptions IgnoringNewlineStyle() {
        IgnoreNewlineStyle = true;
        return this;
    }

    /// <summary>Uses a custom comparer for the final comparison.</summary>
    /// <param name="comparer">The comparer to use.</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <see langword="null"/>.</exception>
    public StringEquivalencyOptions Using(IEqualityComparer<string> comparer) {
        Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        return this;
    }

    /// <summary>Gets the ordinal comparison honoring <see cref="IgnoreCase"/>.</summary>
    internal StringComparison Comparison => IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

    /// <summary>Applies whitespace and newline normalization to <paramref name="value"/>.</summary>
    /// <param name="value">The string to normalize.</param>
    /// <returns>The normalized string.</returns>
    internal string Normalize(string value) {
        if(IgnoreLeadingWhitespace) {
            value = value.TrimStart();
        }

        if(IgnoreTrailingWhitespace) {
            value = value.TrimEnd();
        }

        if(IgnoreNewlineStyle) {
            value = value.Replace("\r\n", "\n").Replace('\r', '\n');
        }

        return value;
    }

    /// <summary>Determines equivalence of two strings honoring the configured options.</summary>
    /// <param name="actual">The actual string value.</param>
    /// <param name="expected">The expected string value.</param>
    /// <returns><see langword="true"/> when both are <see langword="null"/> or equivalent; otherwise <see langword="false"/>.</returns>
    internal bool AreEquivalent(string? actual, string? expected) {
        if(ReferenceEquals(actual, expected)) {
            return true;
        }

        if(actual is null || expected is null) {
            return false;
        }

        var normalizedActual = Normalize(actual);
        var normalizedExpected = Normalize(expected);
        return Comparer is not null
            ? Comparer.Equals(normalizedActual, normalizedExpected)
            : string.Equals(normalizedActual, normalizedExpected, Comparison);
    }
}
