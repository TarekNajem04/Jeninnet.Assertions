//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Discovers and runs user extension points marked with <see cref="CustomAssertionAttribute"/> and <see cref="CustomAssertionsAssemblyAttribute"/>.</summary>
/// <remarks>
/// The markers are consumed here: <see cref="FindMarkedAssemblies"/> locates extension-only assemblies,
/// <see cref="FindCustomAssertions"/> validates their marked members, and <see cref="RunInitializers"/>
/// executes their engine setups (including assemblies loaded after the first assertion, which the
/// automatic scan would otherwise miss). Day-to-day extension still goes through <see cref="AssertionScope"/>
/// alone; this discovery API exists for hosts and tooling composing those extensions.
/// </remarks>
public static class CustomAssertionDiscovery {
    /// <summary>Finds loaded assemblies marked with <see cref="CustomAssertionsAssemblyAttribute"/>.</summary>
    /// <returns>The marked assemblies, ordered by full name.</returns>
    public static IReadOnlyList<Assembly> FindMarkedAssemblies() {
        var marked = AppDomain.CurrentDomain.GetAssemblies().Where(IsMarked).ToList();
        marked.Sort(static (left, right) => string.CompareOrdinal(left.FullName, right.FullName));
        return marked;
    }

    /// <summary>Finds methods marked with <see cref="CustomAssertionAttribute"/> in <paramref name="assembly"/>.</summary>
    /// <param name="assembly">The assembly to inspect.</param>
    /// <returns>The marked methods, ordered by declaring type and name.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assembly"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">A marked method is not public static.</exception>
    public static IReadOnlyList<MethodInfo> FindCustomAssertions(Assembly assembly) {
        if(assembly is null) {
            throw new ArgumentNullException(nameof(assembly));
        }

        var found = new List<MethodInfo>();
        foreach(var type in LoadableTypes(assembly)) {
            foreach(var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly)) {
                if(!method.IsDefined(typeof(CustomAssertionAttribute), inherit: false)) {
                    continue;
                }

                if(!method.IsPublic || !method.IsStatic) {
                    throw new InvalidOperationException(
                        $"Custom assertion '{type.FullName}.{method.Name}' must be a public static method.");
                }

                found.Add(method);
            }
        }

        found.Sort(static (left, right) => {
            var typeOrder = string.CompareOrdinal(left.DeclaringType?.FullName, right.DeclaringType?.FullName);
            return typeOrder != 0 ? typeOrder : string.CompareOrdinal(left.Name, right.Name);
        });
        return found;
    }

    /// <summary>Runs the <see cref="AssertionEngineInitializerAttribute"/> setups declared by <paramref name="assembly"/>.</summary>
    /// <param name="assembly">The assembly whose setups run (use for assemblies loaded after the first assertion).</param>
    /// <returns>The number of setups that ran.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assembly"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">An initializer reference is invalid.</exception>
    public static int RunInitializers(Assembly assembly) {
        if(assembly is null) {
            throw new ArgumentNullException(nameof(assembly));
        }

        return Extensibility.InitializerRunner.RunForAssembly(assembly);
    }

    /// <summary>Determines whether an assembly opts in to custom assertion discovery, treating unreadable metadata as unmarked.</summary>
    /// <param name="assembly">The assembly to inspect.</param>
    /// <returns><see langword="true"/> when the marker attribute is declared.</returns>
    private static bool IsMarked(Assembly assembly) {
        try {
            return assembly.IsDefined(typeof(CustomAssertionsAssemblyAttribute), inherit: false);
        }
        catch {
            // Best effort: some assemblies refuse metadata inspection at runtime.
            return false;
        }
    }

    /// <summary>Returns the types of an assembly, tolerating partially loadable assemblies.</summary>
    /// <param name="assembly">The assembly to inspect.</param>
    /// <returns>All loadable types, or only those that loaded when a <see cref="ReflectionTypeLoadException"/> occurs.</returns>
    private static List<Type> LoadableTypes(Assembly assembly) {
        try {
            return [.. assembly.GetTypes()];
        }
        catch(ReflectionTypeLoadException ex) {
            return [.. ex.Types.OfType<Type>()];
        }
    }
}
