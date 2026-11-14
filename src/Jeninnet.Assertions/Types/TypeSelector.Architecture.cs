//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

public partial class TypeSelector {
    private static readonly TimeSpan _regexTimeout = TimeSpan.FromSeconds(1);

    /// <summary>Keeps types declared in <paramref name="assembly"/>.</summary>
    /// <param name="assembly">The assembly whose types are kept.</param>
    /// <returns>A new selector with the filtered types.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assembly"/> is <see langword="null"/>.</exception>
    public TypeSelector ThatAreInAssembly(Assembly assembly) {
        if(assembly is null) {
            throw new ArgumentNullException(nameof(assembly));
        }

        return new(_types.Where(type => type.Assembly == assembly));
    }

    /// <summary>Keeps types not declared in <paramref name="assembly"/>.</summary>
    /// <param name="assembly">The assembly whose types are excluded.</param>
    /// <returns>A new selector with the filtered types.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assembly"/> is <see langword="null"/>.</exception>
    public TypeSelector ThatAreNotInAssembly(Assembly assembly) {
        if(assembly is null) {
            throw new ArgumentNullException(nameof(assembly));
        }

        return new(_types.Where(type => type.Assembly != assembly));
    }

    /// <summary>Keeps types visible outside their assembly (public or nested public).</summary>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatArePublic() =>
        new(_types.Where(static type => type.IsVisible));

    /// <summary>Keeps types not visible outside their assembly (internal or private nested).</summary>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatAreInternal() =>
        new(_types.Where(static type => !type.IsVisible));

    /// <summary>Keeps types nested inside another type.</summary>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatAreNested() =>
        new(_types.Where(static type => type.IsNested));

    /// <summary>Keeps types that are not nested inside another type.</summary>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatAreNotNested() =>
        new(_types.Where(static type => !type.IsNested));

    /// <summary>Keeps generic types (constructed or open).</summary>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatAreGeneric() =>
        new(_types.Where(static type => type.IsGenericType));

    /// <summary>Keeps types that are not generic.</summary>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatAreNotGeneric() =>
        new(_types.Where(static type => !type.IsGenericType));

    /// <summary>Keeps record classes and record structs.</summary>
    /// <remarks>Detection uses compiler-generated record markers; hand-written look-alikes can be misclassified.</remarks>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatAreRecords() =>
        new(_types.Where(ReflectionMetadata.IsRecord));

    /// <summary>Keeps types that are not records.</summary>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatAreNotRecords() =>
        new(_types.Where(static type => !ReflectionMetadata.IsRecord(type)));

    /// <summary>Keeps types that are structurally immutable from their public surface.</summary>
    /// <remarks>Heuristic: public instance fields must be readonly and public setters must be init-only or absent.
    /// Private mutable state is ignored; this is a structural check, not a proof.</remarks>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatAreImmutable() =>
        new(_types.Where(ReflectionMetadata.IsImmutable));

    /// <summary>Keeps types that expose mutable public state.</summary>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatAreNotImmutable() =>
        new(_types.Where(static type => !ReflectionMetadata.IsImmutable(type)));

    /// <summary>Keeps types whose simple name equals <paramref name="name"/> (ordinal).</summary>
    /// <param name="name">The expected simple name.</param>
    /// <returns>A new selector with the filtered types.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    public TypeSelector ThatHaveName(string name) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        return new(_types.Where(type => string.Equals(type.Name, name, StringComparison.Ordinal)));
    }

    /// <summary>Keeps types whose simple name starts with <paramref name="prefix"/> (ordinal).</summary>
    /// <param name="prefix">The required prefix.</param>
    /// <returns>A new selector with the filtered types.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="prefix"/> is <see langword="null"/>.</exception>
    public TypeSelector ThatHaveNameStartingWith(string prefix) {
        if(prefix is null) {
            throw new ArgumentNullException(nameof(prefix));
        }

        return new(_types.Where(type => type.Name.StartsWith(prefix, StringComparison.Ordinal)));
    }

    /// <summary>Keeps types whose simple name ends with <paramref name="suffix"/> (ordinal).</summary>
    /// <param name="suffix">The required suffix.</param>
    /// <returns>A new selector with the filtered types.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="suffix"/> is <see langword="null"/>.</exception>
    public TypeSelector ThatHaveNameEndingWith(string suffix) {
        if(suffix is null) {
            throw new ArgumentNullException(nameof(suffix));
        }

        return new(_types.Where(type => type.Name.EndsWith(suffix, StringComparison.Ordinal)));
    }

    /// <summary>Keeps types whose simple name matches <paramref name="pattern"/>.</summary>
    /// <remarks>The pattern runs with a one-second timeout and culture-invariant semantics.</remarks>
    /// <param name="pattern">The regular expression matched against the simple type name.</param>
    /// <returns>A new selector with the filtered types.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pattern"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="pattern"/> is not a valid regular expression.</exception>
    public TypeSelector ThatHaveNameMatching(string pattern) {
        if(pattern is null) {
            throw new ArgumentNullException(nameof(pattern));
        }

        var regex = new Regex(pattern, RegexOptions.CultureInvariant, _regexTimeout);
        return new(_types.Where(type => regex.IsMatch(type.Name)));
    }

    /// <summary>Keeps types assignable to <typeparamref name="T"/> (excluding <typeparamref name="T"/> itself).</summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatAreAssignableTo<T>() =>
        new(_types.Where(static type => type != typeof(T) && typeof(T).IsAssignableFrom(type)));

    /// <summary>Keeps types assignable to <paramref name="target"/> (excluding the target itself).</summary>
    /// <param name="target">The target type.</param>
    /// <returns>A new selector with the filtered types.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
    public TypeSelector ThatAreAssignableTo(Type target) {
        if(target is null) {
            throw new ArgumentNullException(nameof(target));
        }

        return new(_types.Where(type => type != target && target.IsAssignableFrom(type)));
    }

    /// <summary>Keeps types assignable from <typeparamref name="T"/> (excluding <typeparamref name="T"/> itself).</summary>
    /// <typeparam name="T">The source type.</typeparam>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatAreAssignableFrom<T>() =>
        new(_types.Where(static type => type != typeof(T) && type.IsAssignableFrom(typeof(T))));

    /// <summary>Keeps types assignable from <paramref name="source"/> (excluding the source itself).</summary>
    /// <param name="source">The source type.</param>
    /// <returns>A new selector with the filtered types.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    public TypeSelector ThatAreAssignableFrom(Type source) {
        if(source is null) {
            throw new ArgumentNullException(nameof(source));
        }

        return new(_types.Where(type => type != source && type.IsAssignableFrom(source)));
    }

    /// <summary>Keeps types with a reflection-level dependency on <paramref name="namespaceOrType"/>.</summary>
    /// <param name="namespaceOrType">A namespace prefix, full type name, or simple type name.</param>
    /// <returns>A new selector with the filtered types.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="namespaceOrType"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="namespaceOrType"/> is empty or whitespace.</exception>
    public TypeSelector ThatHaveDependencyOn(string namespaceOrType) {
        var target = ValidateTarget(namespaceOrType, nameof(namespaceOrType));
        var graph = TypeDependencyGraph.Build(_types);
        return new(_types.Where(type => graph.DependenciesOf(type).Any(dependency => DependencyMatcher.Matches(dependency, target))));
    }

    /// <summary>Keeps types with no reflection-level dependency on <paramref name="namespaceOrType"/>.</summary>
    /// <param name="namespaceOrType">A namespace prefix, full type name, or simple type name.</param>
    /// <returns>A new selector with the filtered types.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="namespaceOrType"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="namespaceOrType"/> is empty or whitespace.</exception>
    public TypeSelector ThatDoNotHaveDependencyOn(string namespaceOrType) {
        var target = ValidateTarget(namespaceOrType, nameof(namespaceOrType));
        var graph = TypeDependencyGraph.Build(_types);
        return new(_types.Where(type => !graph.DependenciesOf(type).Any(dependency => DependencyMatcher.Matches(dependency, target))));
    }

    /// <summary>Validates a dependency target string.</summary>
    /// <param name="target">The target to validate.</param>
    /// <param name="parameterName">The caller parameter name.</param>
    /// <returns>The trimmed target.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="target"/> is empty or whitespace.</exception>
    private static string ValidateTarget(string target, string parameterName) {
        if(target is null) {
            throw new ArgumentNullException(parameterName);
        }

        var trimmed = target.Trim();
        if(trimmed.Length == 0) {
            throw new ArgumentException("The dependency target must not be empty.", parameterName);
        }

        return trimmed;
    }
}
