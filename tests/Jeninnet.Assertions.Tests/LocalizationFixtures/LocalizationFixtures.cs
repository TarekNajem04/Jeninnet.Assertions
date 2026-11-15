//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests.LocalizationFixtures;

/// <summary>
/// The only type in this namespace, giving type-selector tests a deterministic single-type selection.
/// </summary>
// Fixture: intentionally unsealed and empty so selector BeSealed/BeAbstract can fail against a known single type.
#pragma warning disable S2094, CA1852
internal class LonelyWidget;
#pragma warning restore S2094, CA1852

/// <summary>Sets <see cref="AssertionCulture.Current"/> for the duration of a test and restores it on dispose.</summary>
internal sealed class CultureScope : IDisposable {
    private readonly CultureInfo _previous;

    /// <summary>Creates a scope pinned to <paramref name="culture"/>.</summary>
    /// <param name="culture">The culture name, for example <c>ar</c> or <c>qps-ploc</c>.</param>
    internal CultureScope(string culture) {
        _previous = AssertionCulture.Current;
        AssertionCulture.Current = CultureInfo.GetCultureInfo(culture);
    }

    /// <summary>Restores the previous assertion culture.</summary>
    public void Dispose() => AssertionCulture.Current = _previous;
}
