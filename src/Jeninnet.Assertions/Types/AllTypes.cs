//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

/// <summary>Entry point for assembly-wide architecture assertions (e.g. <c>AllTypes.From(assembly).ThatDeriveFrom&lt;T&gt;().Should().BeSealed()</c>).</summary>
/// <remarks><see cref="From"/> loads and reflects over every type in the supplied assemblies. That is the intended
/// cost for architecture tests; the dependency graph is reflection-level and never reads method bodies. For very
/// large or partially loadable assemblies, prefer filtering by assembly/namespace first and consider
/// <c>MetadataLoadContext</c> in a separate inspection tool when loading the runtime assembly is undesirable.</remarks>
public static class AllTypes {
    /// <summary>Selects every loadable type in <paramref name="assemblies"/> for further filtering.</summary>
    /// <remarks>When a partial load fails, the types that did load are used and the unloadable entries are skipped.</remarks>
    /// <param name="assemblies">At least one assembly to scan.</param>
    /// <returns>A <see cref="TypeSelector"/> over all types.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assemblies"/> is <see langword="null"/> or contains <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">No assembly was provided.</exception>
    [RequiresUnreferencedCode(TrimmingMessages.ASSEMBLY_METADATA)]
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

        return new TypeSelector(assemblies.SelectMany(GetLoadableTypes));
    }

    /// <summary>Gets the loadable types of an assembly, tolerating a partial load.</summary>
    /// <param name="assembly">The assembly to inspect.</param>
    /// <returns>The loaded types; unloadable entries are skipped.</returns>
    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly) {
        try {
            return assembly.GetTypes();
        }
        catch(ReflectionTypeLoadException exception) {
            return exception.Types.OfType<Type>();
        }
    }
}
