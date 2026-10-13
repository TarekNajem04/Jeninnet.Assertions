//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>
/// Configures collection equivalency checks (ordering, casing, custom equality) for assertions such as
/// <c>BeEquivalentTo(expected, o => o.WithStrictOrdering())</c>. Full object-graph options arrive in P4.
/// </summary>
/// <typeparam name="T">The element type of the collection.</typeparam>
public class CollectionEquivalencyOptions<T> {
    /// <summary>Creates options with <see cref="StrictOrdering"/> initialized from the global default.</summary>
    public CollectionEquivalencyOptions() {
        InitializerRunner.EnsureInitialized();
        StrictOrdering = AssertionConfiguration.Equivalency.StrictOrderingDefault;
    }

    /// <summary>Gets whether order is significant (default compares as an unordered multiset).</summary>
    public bool StrictOrdering { get; private set; }

    /// <summary>Gets whether casing differences are ignored. Only valid for string collections.</summary>
    public bool IgnoreCase { get; private set; }

    /// <summary>Gets the custom element comparer, or <see langword="null"/> for the default comparer.</summary>
    public IEqualityComparer<T>? Comparer { get; private set; }

    /// <summary>Requires elements to match in the same order.</summary>
    /// <returns>This options instance for chaining.</returns>
    public CollectionEquivalencyOptions<T> WithStrictOrdering() {
        StrictOrdering = true;
        return this;
    }

    /// <summary>Ignores casing differences. Only valid when <typeparamref name="T"/> is <see cref="string"/>.</summary>
    /// <returns>This options instance for chaining.</returns>
    public CollectionEquivalencyOptions<T> IgnoringCase() {
        IgnoreCase = true;
        return this;
    }

    /// <summary>Uses a custom comparer for element equality.</summary>
    /// <param name="comparer">The comparer to use.</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <see langword="null"/>.</exception>
    public CollectionEquivalencyOptions<T> Using(IEqualityComparer<T> comparer) {
        Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        return this;
    }

    /// <summary>Resolves the effective element comparer.</summary>
    /// <returns>The custom comparer, a case-insensitive comparer for strings, or the default comparer.</returns>
    /// <exception cref="InvalidOperationException"><see cref="IgnoreCase"/> is set for a non-string element type.</exception>
    internal IEqualityComparer<T> ResolveEquality() {
        if(Comparer is not null) {
            return Comparer;
        }

        if(IgnoreCase) {
            if(typeof(T) == typeof(string)) {
                return (IEqualityComparer<T>)(object)StringComparer.OrdinalIgnoreCase;
            }

            throw new InvalidOperationException("IgnoringCase is only supported for string collections.");
        }

        return EqualityComparer<T>.Default;
    }
}
