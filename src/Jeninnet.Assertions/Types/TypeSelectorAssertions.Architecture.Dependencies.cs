//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

public partial class TypeSelectorAssertions {
    /// <summary>Asserts that every selected type depends on <paramref name="namespaceOrType"/>.</summary>
    /// <remarks>The dependency graph is reflection-level: base types, interfaces, generic arguments, members, and
    /// attribute references. Method bodies and IL are not inspected.</remarks>
    /// <param name="namespaceOrType">A namespace prefix, full type name, or simple type name.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="namespaceOrType"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="namespaceOrType"/> is empty or whitespace.</exception>
    /// <exception cref="AssertionFailedException">At least one selected type has no matching dependency.</exception>
    public AndConstraint<TypeSelectorAssertions> DependOn(string namespaceOrType, string? because = null, params object?[] becauseArgs) {
        var target = ValidateTarget(namespaceOrType, nameof(namespaceOrType));
        var graph = TypeDependencyGraph.Build(_types);
        var offenders = _types
            .Where(type => !graph.DependenciesOf(type).Any(dependency => DependencyMatcher.Matches(dependency, target)))
            .Select(Describe)
            .ToList();

        Report(StringKeys.ARCH_DEPEND_ON, offenders, because, becauseArgs, target);
        return new(this);
    }

    /// <summary>Asserts that every selected type depends on <paramref name="target"/>.</summary>
    /// <param name="target">The required dependency type.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">At least one selected type has no dependency on <paramref name="target"/>.</exception>
    public AndConstraint<TypeSelectorAssertions> DependOn(Type target, string? because = null, params object?[] becauseArgs) {
        if(target is null) {
            throw new ArgumentNullException(nameof(target));
        }

        return DependOn(DependencyMatcher.TargetName(target), because, becauseArgs);
    }

    /// <summary>Asserts that every selected type depends on at least one of <paramref name="targets"/>.</summary>
    /// <remarks>Use this overload instead of a second positional string when the reason must be supplied; a second
    /// string argument binds to the <see cref="DependOn(string, string?, object?[])"/> parameter as the reason.</remarks>
    /// <param name="targets">Namespace prefixes, full type names, or simple type names; at least one must match.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="targets"/> is <see langword="null"/> or contains <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="targets"/> is empty or contains a blank entry.</exception>
    /// <exception cref="AssertionFailedException">At least one selected type has no dependency matching any target.</exception>
    public AndConstraint<TypeSelectorAssertions> DependOnAny(params string[] targets) => DependOnAny(targets, null, []);

    /// <summary>Asserts that every selected type depends on at least one of <paramref name="targets"/>.</summary>
    /// <param name="targets">Namespace prefixes, full type names, or simple type names; at least one must match.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="targets"/> is <see langword="null"/> or contains <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="targets"/> is empty or contains a blank entry.</exception>
    /// <exception cref="AssertionFailedException">At least one selected type has no dependency matching any target.</exception>
    public AndConstraint<TypeSelectorAssertions> DependOnAny(string[] targets, string? because, params object?[] becauseArgs) {
        if(targets is null) {
            throw new ArgumentNullException(nameof(targets));
        }

        if(targets.Length == 0) {
            throw new ArgumentException("At least one dependency target must be provided.", nameof(targets));
        }

        var normalized = targets.Select(target => ValidateTarget(target, nameof(targets))).ToArray();
        var graph = TypeDependencyGraph.Build(_types);
        var offenders = _types
            .Where(type => !graph.DependenciesOf(type).Any(dependency => normalized.Any(target => DependencyMatcher.Matches(dependency, target))))
            .Select(Describe)
            .ToList();

        Report(StringKeys.ARCH_DEPEND_ON_ANY, offenders, because, becauseArgs, string.Join(", ", normalized));
        return new(this);
    }

    /// <summary>Asserts that only the allowed sources depend on the selected types.</summary>
    /// <remarks>
    /// Incoming dependencies are discovered by scanning every type in the assemblies of the selected types, excluding
    /// compiler-generated types and the selected types themselves. Dependencies from other assemblies cannot be seen
    /// through reflection and are outside this rule. Framework dependencies are included because the rule describes
    /// who may depend on the selection, not what the selection may depend on.
    /// </remarks>
    /// <param name="allowedSources">Namespace prefixes, full type names, or simple type names allowed to depend on the selection.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="allowedSources"/> is <see langword="null"/> or contains <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="allowedSources"/> is empty or contains a blank entry.</exception>
    /// <exception cref="AssertionFailedException">A type outside the allowed sources depends on the selection.</exception>
    public AndConstraint<TypeSelectorAssertions> OnlyBeDependedOnBy(params string[] allowedSources) =>
        OnlyBeDependedOnBy(allowedSources, null, []);

    /// <summary>Asserts that only the allowed sources depend on the selected types.</summary>
    /// <param name="allowedSources">Namespace prefixes, full type names, or simple type names allowed to depend on the selection.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="allowedSources"/> is <see langword="null"/> or contains <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="allowedSources"/> is empty or contains a blank entry.</exception>
    /// <exception cref="AssertionFailedException">A type outside the allowed sources depends on the selection.</exception>
    public AndConstraint<TypeSelectorAssertions> OnlyBeDependedOnBy(string[] allowedSources, string? because, params object?[] becauseArgs) {
        if(allowedSources is null) {
            throw new ArgumentNullException(nameof(allowedSources));
        }

        if(allowedSources.Length == 0) {
            throw new ArgumentException("At least one allowed source must be provided.", nameof(allowedSources));
        }

        var allowed = allowedSources.Select(source => ValidateTarget(source, nameof(allowedSources))).ToArray();
        var offenders = BuildIncomingDependencies()
            .Where(pair => !allowed.Any(source => DependencyMatcher.Matches(pair.Source, source)))
            .Select(static pair => Describe(pair.Source) + " -> " + Describe(pair.Target))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        Report(StringKeys.ARCH_ONLY_BE_DEPENDED_ON_BY, offenders, because, becauseArgs, string.Join(", ", allowed));
        return new(this);
    }

    /// <summary>Asserts that no type in <paramref name="namespaceOrType"/> depends on the selected types.</summary>
    /// <remarks>Incoming dependencies are discovered from the assemblies of the selected types; selected types are
    /// excluded, so internal dependencies inside the selection do not trigger this rule.</remarks>
    /// <param name="namespaceOrType">A namespace prefix, full type name, or simple type name of the forbidden source.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="namespaceOrType"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="namespaceOrType"/> is empty or whitespace.</exception>
    /// <exception cref="AssertionFailedException">A matching source depends on the selection.</exception>
    public AndConstraint<TypeSelectorAssertions> NotBeDependedOnBy(string namespaceOrType, string? because = null, params object?[] becauseArgs) {
        var target = ValidateTarget(namespaceOrType, nameof(namespaceOrType));
        var offenders = BuildIncomingDependencies()
            .Where(pair => DependencyMatcher.Matches(pair.Source, target))
            .Select(static pair => Describe(pair.Source) + " -> " + Describe(pair.Target))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        Report(StringKeys.ARCH_NOT_BE_DEPENDED_ON_BY, offenders, because, becauseArgs, target);
        return new(this);
    }

    /// <summary>Asserts that the selected types follow the dependencies declared by <paramref name="layers"/>.</summary>
    /// <remarks>
    /// A type belongs to the layer with the longest matching namespace prefix. Dependencies inside the same layer are
    /// allowed; dependencies on another layer require a matching <see cref="ArchitectureLayer.DependsOn"/> declaration.
    /// Types outside every layer and dependencies on them are ignored. Framework namespaces (<c>System.*</c> and
    /// <c>Microsoft.*</c>) are ignored by default; when <c>includingFrameworkDependencies</c> is enabled,
    /// a layer must declare <see cref="ArchitectureLayer.FrameworkLayerName"/> to depend on them.
    /// </remarks>
    /// <param name="layers">The layer definitions.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="layers"/> is <see langword="null"/> or contains <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="layers"/> is empty, has duplicate names or prefixes, or declares an unknown dependency.</exception>
    /// <exception cref="AssertionFailedException">A dependency crosses layers without a matching declaration.</exception>
    public AndConstraint<TypeSelectorAssertions> FollowLayers(params ArchitectureLayer[] layers) =>
        FollowLayers(layers, false, null, []);

    /// <summary>Asserts that the selected types follow the dependencies declared by <paramref name="layers"/>.</summary>
    /// <param name="layers">The layer definitions.</param>
    /// <param name="includingFrameworkDependencies">Whether dependencies on <c>System.*</c>/<c>Microsoft.*</c> are also checked.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="layers"/> is <see langword="null"/> or contains <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="layers"/> is empty, has duplicate names or prefixes, or declares an unknown dependency.</exception>
    /// <exception cref="AssertionFailedException">A dependency crosses layers without a matching declaration.</exception>
    public AndConstraint<TypeSelectorAssertions> FollowLayers(
        ArchitectureLayer[] layers,
        bool includingFrameworkDependencies,
        string? because = null,
        params object?[] becauseArgs) {
        if(layers is null) {
            throw new ArgumentNullException(nameof(layers));
        }

        if(layers.Length == 0) {
            throw new ArgumentException("At least one layer must be provided.", nameof(layers));
        }

        if(layers.Any(static layer => layer is null)) {
            throw new ArgumentNullException(nameof(layers));
        }

        if(layers.Select(static layer => layer.Name).Distinct(StringComparer.Ordinal).Count() != layers.Length) {
            throw new ArgumentException("Layer names must be unique.", nameof(layers));
        }

        if(layers.Select(static layer => layer.NamespacePrefix).Distinct(StringComparer.Ordinal).Count() != layers.Length) {
            throw new ArgumentException("Layer namespace prefixes must be unique.", nameof(layers));
        }

        var byName = layers.ToDictionary(static layer => layer.Name, StringComparer.Ordinal);
        var (unknownLayer, unknownDependency) = layers
            .SelectMany(static layer => layer.Dependencies.Select(dependency => (Layer: layer.Name, Dependency: dependency)))
            .FirstOrDefault(pair => !byName.ContainsKey(pair.Dependency)
                && !string.Equals(pair.Dependency, ArchitectureLayer.FrameworkLayerName, StringComparison.Ordinal));
        if(unknownLayer is not null) {
            throw new ArgumentException($"Layer '{unknownLayer}' declares an unknown dependency on '{unknownDependency}'.", nameof(layers));
        }

        var graph = TypeDependencyGraph.Build(_types);
        var offenders = new List<string>();
        foreach(var type in _types) {
            var source = LayerOf(type, layers);
            if(source is null) {
                continue;
            }

            foreach(var dependency in graph.DependenciesOf(type)) {
                if(!includingFrameworkDependencies && IsFrameworkDependency(dependency)) {
                    continue;
                }

                var target = LayerOf(dependency, layers);
                if(target is null) {
                    if(includingFrameworkDependencies
                        && IsFrameworkDependency(dependency)
                        && !source.Allows(ArchitectureLayer.FrameworkLayerName)) {
                        offenders.Add($"{Describe(type)} -> {Describe(dependency)} ({source.Name} -> {ArchitectureLayer.FrameworkLayerName})");
                    }

                    continue;
                }

                if(ReferenceEquals(source, target) || source.Allows(target.Name)) {
                    continue;
                }

                offenders.Add($"{Describe(type)} -> {Describe(dependency)} ({source.Name} -> {target.Name})");
            }
        }

        Report(StringKeys.ARCH_FOLLOW_LAYERS, offenders, because, becauseArgs);
        return new(this);
    }

    /// <summary>Builds the incoming dependency pairs from types outside the selection.</summary>
    /// <returns>Unique source-to-selected-type pairs in deterministic order.</returns>
    private List<(Type Source, Type Target)> BuildIncomingDependencies() {
        var selected = new HashSet<Type>(_types);
        if(selected.Count == 0) {
            return [];
        }

        var universe = new List<Type>();
        foreach(var assembly in _types.Select(static type => type.Assembly).Distinct()) {
            Type[] loadedTypes;
            try {
                loadedTypes = assembly.GetTypes();
            }
            catch(ReflectionTypeLoadException exception) {
                loadedTypes = [.. exception.Types.OfType<Type>()];
            }

            universe.AddRange(loadedTypes.Where(type => !selected.Contains(type) && !IsCompilerGenerated(type)));
        }

        var graph = TypeDependencyGraph.Build([.. selected, .. universe]);
        var pairs = new List<(Type Source, Type Target)>();
        foreach(var source in universe.OrderBy(Describe, StringComparer.Ordinal)) {
            foreach(var dependency in graph.DependenciesOf(source).Where(selected.Contains)) {
                pairs.Add((source, dependency));
            }
        }

        return pairs;
    }

    /// <summary>Finds the most specific layer for a type namespace.</summary>
    /// <param name="type">The type to place.</param>
    /// <param name="layers">The layer definitions.</param>
    /// <returns>The longest matching layer, or <see langword="null"/> when the type belongs to no layer.</returns>
    private static ArchitectureLayer? LayerOf(Type type, ArchitectureLayer[] layers) {
        var ns = type.Namespace;
        if(ns is null) {
            return null;
        }

        ArchitectureLayer? match = null;
        foreach(var layer in layers) {
            if(!string.Equals(ns, layer.NamespacePrefix, StringComparison.Ordinal)
                && !ns.StartsWith(layer.NamespacePrefix + ".", StringComparison.Ordinal)) {
                continue;
            }

            if(match is null || layer.NamespacePrefix.Length > match.NamespacePrefix.Length) {
                match = layer;
            }
        }

        return match;
    }

    /// <summary>Determines whether a dependency belongs to a framework namespace.</summary>
    /// <param name="type">The dependency type.</param>
    /// <returns><see langword="true"/> for <c>System.*</c>/<c>Microsoft.*</c> (and their roots).</returns>
    private static bool IsFrameworkDependency(Type type) {
        var ns = type.Namespace;
        return ns is not null
            && (string.Equals(ns, "System", StringComparison.Ordinal)
                || ns.StartsWith("System.", StringComparison.Ordinal)
                || string.Equals(ns, "Microsoft", StringComparison.Ordinal)
                || ns.StartsWith("Microsoft.", StringComparison.Ordinal));
    }

    /// <summary>Determines whether a type is compiler-generated and should not act as an architecture source.</summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns><see langword="true"/> for generated state machines, closures, and similar compiler artifacts.</returns>
    private static bool IsCompilerGenerated(Type type) =>
        (type.Name.Length > 0 && type.Name[0] == '<')
        || type.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false);
}
