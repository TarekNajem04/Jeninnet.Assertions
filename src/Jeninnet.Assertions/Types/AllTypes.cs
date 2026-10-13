//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

/// <summary>Entry point for assembly-wide architecture assertions (e.g. <c>AllTypes.From(assembly).ThatDeriveFrom&lt;T&gt;().Should().BeSealed()</c>).</summary>
public static class AllTypes {
    /// <summary>Selects every loadable type in <paramref name="assemblies"/> for further filtering.</summary>
    /// <param name="assemblies">At least one assembly to scan.</param>
    /// <returns>A <see cref="TypeSelector"/> over all types.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assemblies"/> is <see langword="null"/> or contains <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">No assembly was provided.</exception>
    public static TypeSelector From(params Assembly[] assemblies) {
        if(assemblies is null) {
            throw new ArgumentNullException(nameof(assemblies));
        }

        if(assemblies.Length == 0) {
            throw new ArgumentException("At least one assembly must be provided.", nameof(assemblies));
        }

        if(assemblies.Any(static assembly => assembly is null)) {
            throw new ArgumentNullException(nameof(assemblies));
        }

        return new TypeSelector(assemblies.SelectMany(static assembly => assembly.GetTypes()));
    }
}
