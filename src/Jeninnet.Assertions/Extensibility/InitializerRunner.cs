//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Extensibility;

/// <summary>Runs assembly-level <see cref="AssertionEngineInitializerAttribute"/> setups once before the first assertion.</summary>
internal static class InitializerRunner {
    private static readonly object _sync = new();
    private static volatile bool _initialized;

    /// <summary>Runs pending initializers exactly once (later calls are no-ops, including re-entrant ones).</summary>
    /// <exception cref="InvalidOperationException">An initializer reference is invalid.</exception>
    internal static void EnsureInitialized() {
        if(_initialized) {
            return;
        }

        List<Action> initializers;
        lock(_sync) {
            if(_initialized) {
                return;
            }

            initializers = Collect();
            _initialized = true;
        }

        foreach(var initialize in initializers) {
            initialize();
        }
    }

    /// <summary>Runs the initializers declared by <paramref name="assembly"/> immediately.</summary>
    /// <param name="assembly">The assembly whose setups run.</param>
    /// <returns>The number of setups that ran.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assembly"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">An initializer reference is invalid.</exception>
    internal static int RunForAssembly(Assembly assembly) {
        if(assembly is null) {
            throw new ArgumentNullException(nameof(assembly));
        }

        var initializers = new List<Action>();
        foreach(var attribute in GetInitializerAttributes(assembly)) {
            initializers.Add(Resolve(attribute));
        }

        foreach(var initialize in initializers) {
            initialize();
        }

        return initializers.Count;
    }

    /// <summary>Scans every loaded assembly for initializer attributes and resolves their setups.</summary>
    /// <returns>The resolved initializer actions.</returns>
    /// <exception cref="InvalidOperationException">An initializer reference is invalid.</exception>
    private static List<Action> Collect() {
        var initializers = new List<Action>();
        foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
            foreach(var attribute in GetInitializerAttributes(assembly)) {
                initializers.Add(Resolve(attribute));
            }
        }

        return initializers;
    }

    /// <summary>Validates an initializer attribute and builds the action that invokes its setup.</summary>
    /// <param name="attribute">The initializer attribute to resolve.</param>
    /// <returns>An action that invokes the setup, unwrapping reflection exceptions.</returns>
    /// <exception cref="InvalidOperationException">The referenced method is missing, non-public, parameterized, or non-void.</exception>
    private static Action Resolve(AssertionEngineInitializerAttribute attribute) {
        var method = attribute.InitializerType.GetMethod(attribute.MethodName, BindingFlags.Public | BindingFlags.Static);
        if(method is null || method.GetParameters().Length != 0 || method.ReturnType != typeof(void)) {
            throw new InvalidOperationException(
                $"Assertion initializer '{attribute.InitializerType.FullName}.{attribute.MethodName}' must be a public static parameterless void method.");
        }

        return () => {
            try {
                method.Invoke(null, null);
            }
            catch(TargetInvocationException ex) when(ex.InnerException is not null) {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
        };
    }

    /// <summary>Reads initializer attributes from one assembly, treating unreadable metadata as none.</summary>
    /// <param name="assembly">The assembly to inspect.</param>
    /// <returns>The declared initializer attributes, or an empty list when metadata inspection fails.</returns>
    private static List<AssertionEngineInitializerAttribute> GetInitializerAttributes(Assembly assembly) {
        try {
            return [.. assembly.GetCustomAttributes(typeof(AssertionEngineInitializerAttribute), inherit: false).OfType<AssertionEngineInitializerAttribute>()];
        }
        catch {
            // Best effort: some assemblies refuse metadata inspection at runtime.
            return [];
        }
    }
}
