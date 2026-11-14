//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

/// <summary>Builds the reflection-level dependency graph used by architecture assertions.</summary>
/// <remarks>
/// Edges come from base types, interfaces, generic arguments, fields, properties, method and constructor
/// parameters/returns, events, and attribute references. Method bodies and IL are deliberately out of scope:
/// the graph describes the declared shape, not the executed code. Dependencies are normalized to generic type
/// definitions plus their arguments, and self-references are omitted.
/// </remarks>
internal sealed class TypeDependencyGraph {
    private const BindingFlags DECLARED = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

    private readonly Dictionary<Type, Type[]> _dependencies;

    /// <summary>Creates a graph over an already materialized dependency map.</summary>
    /// <param name="dependencies">The dependency map keyed by selected type.</param>
    private TypeDependencyGraph(Dictionary<Type, Type[]> dependencies) => _dependencies = dependencies;

    /// <summary>Builds the dependency graph for <paramref name="types"/>.</summary>
    /// <param name="types">The selected types; duplicates are ignored.</param>
    /// <returns>The graph.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="types"/> is <see langword="null"/>.</exception>
    internal static TypeDependencyGraph Build(IEnumerable<Type> types) {
        if(types is null) {
            throw new ArgumentNullException(nameof(types));
        }

        var dependencies = new Dictionary<Type, Type[]>();
        foreach(var type in types) {
            if(type is null || dependencies.ContainsKey(type)) {
                continue;
            }

            dependencies[type] = Collect(type)
                .OrderBy(static dependency => dependency.FullName, StringComparer.Ordinal)
                .ToArray();
        }

        return new TypeDependencyGraph(dependencies);
    }

    /// <summary>Gets the direct dependencies of a selected type.</summary>
    /// <param name="type">The selected type.</param>
    /// <returns>The external dependencies in ordinal full-name order, or an empty list when the type is not part of the graph.</returns>
    internal IReadOnlyList<Type> DependenciesOf(Type type) =>
        _dependencies.TryGetValue(type, out var dependencies)
            ? dependencies.Where(dependency => dependency != type).ToArray()
            : [];

    /// <summary>Finds dependency cycles among the selected types.</summary>
    /// <remarks>Uses an iterative Kosaraju pass for strongly connected components; external dependencies are ignored.
    /// Each returned array is one concrete cycle path whose first and last entries are the same type.</remarks>
    /// <returns>The detected cycles in deterministic order.</returns>
    internal IReadOnlyList<Type[]> FindCycles() {
        var nodes = _dependencies.Keys.OrderBy(static type => type.FullName, StringComparer.Ordinal).ToArray();
        var selected = new HashSet<Type>(nodes);
        var adjacency = new Dictionary<Type, Type[]>(nodes.Length);
        foreach(var node in nodes) {
            adjacency[node] = _dependencies[node].Where(selected.Contains)
                .OrderBy(static type => type.FullName, StringComparer.Ordinal)
                .ToArray();
        }

        var order = BuildFinishOrder(nodes, adjacency);
        var reverse = BuildReverse(nodes, adjacency);
        var visited = new HashSet<Type>();
        var cycles = new List<Type[]>();
        for(var index = order.Count - 1; index >= 0; index--) {
            var node = order[index];
            if(!visited.Add(node)) {
                continue;
            }

            var component = CollectComponent(node, reverse, visited);
            var cycle = FindCycle(component, adjacency);
            if(cycle is not null) {
                cycles.Add(cycle);
            }
        }

        return cycles;
    }

    /// <summary>Collects every dependency declared by <paramref name="owner"/>.</summary>
    /// <param name="owner">The type whose declared surface is inspected.</param>
    /// <returns>The dependency set.</returns>
    private static HashSet<Type> Collect(Type owner) {
        var dependencies = new HashSet<Type>();
        AddType(owner.BaseType, owner, dependencies);
        foreach(var contract in owner.GetInterfaces()) {
            AddType(contract, owner, dependencies);
        }

        AddAttributes(owner, owner, dependencies);

        foreach(var field in owner.GetFields(DECLARED)) {
            if(field.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false)) {
                continue;
            }

            AddType(field.FieldType, owner, dependencies);
            AddAttributes(field, owner, dependencies);
        }

        foreach(var property in owner.GetProperties(DECLARED)) {
            AddType(property.PropertyType, owner, dependencies);
            foreach(var parameter in property.GetIndexParameters()) {
                AddType(parameter.ParameterType, owner, dependencies);
            }

            AddAttributes(property, owner, dependencies);
        }

        foreach(var @event in owner.GetEvents(DECLARED)) {
            AddType(@event.EventHandlerType, owner, dependencies, countSelf: false);
            AddAttributes(@event, owner, dependencies);
        }

        foreach(var constructor in owner.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)) {
            foreach(var parameter in constructor.GetParameters()) {
                AddType(parameter.ParameterType, owner, dependencies, countSelf: false);
            }

            AddAttributes(constructor, owner, dependencies);
        }

        foreach(var method in owner.GetMethods(DECLARED)) {
            if(IsAccessor(method) || method.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false)) {
                continue;
            }

            AddType(method.ReturnType, owner, dependencies, countSelf: false);
            foreach(var parameter in method.GetParameters()) {
                AddType(parameter.ParameterType, owner, dependencies, countSelf: false);
            }

            AddAttributes(method, owner, dependencies);
        }

        return dependencies;
    }

    /// <summary>Adds a type and, for constructed generics, its definition and generic arguments.</summary>
    /// <param name="type">The type to add (may be <see langword="null"/>).</param>
    /// <param name="owner">The type whose dependency set is being built.</param>
    /// <param name="dependencies">The dependency set.</param>
    /// <param name="countSelf">Whether a direct self-reference should become a self-loop edge.</param>
    private static void AddType(Type? type, Type owner, HashSet<Type> dependencies, bool countSelf = true) {
        if(type is null) {
            return;
        }

        if(type.HasElementType) {
            AddType(type.GetElementType(), owner, dependencies, countSelf);
            return;
        }

        if(type.IsGenericParameter) {
            foreach(var constraint in type.GetGenericParameterConstraints()) {
                AddType(constraint, owner, dependencies, countSelf: false);
            }

            return;
        }

        if(type.IsGenericType && !type.IsGenericTypeDefinition) {
            AddType(type.GetGenericTypeDefinition(), owner, dependencies, countSelf: false);
            foreach(var argument in type.GetGenericArguments()) {
                AddType(argument, owner, dependencies, countSelf: false);
            }

            return;
        }

        if(type != owner || countSelf) {
            dependencies.Add(type);
        }
    }

    /// <summary>Adds the attribute type and any <see cref="Type"/> values passed to it.</summary>
    /// <param name="member">The member carrying the attributes.</param>
    /// <param name="owner">The type whose dependency set is being built.</param>
    /// <param name="dependencies">The dependency set.</param>
    private static void AddAttributes(MemberInfo member, Type owner, HashSet<Type> dependencies) {
        foreach(var attribute in member.GetCustomAttributesData()) {
            AddType(attribute.AttributeType, owner, dependencies, countSelf: false);
            foreach(var argument in attribute.ConstructorArguments) {
                AddAttributeArgument(argument, owner, dependencies);
            }

            foreach(var named in attribute.NamedArguments) {
                AddAttributeArgument(named.TypedValue, owner, dependencies);
            }
        }
    }

    /// <summary>Adds a <see cref="Type"/> value carried by an attribute argument.</summary>
    /// <param name="argument">The attribute argument.</param>
    /// <param name="owner">The type whose dependency set is being built.</param>
    /// <param name="dependencies">The dependency set.</param>
    private static void AddAttributeArgument(CustomAttributeTypedArgument argument, Type owner, HashSet<Type> dependencies) {
        if(argument.Value is Type referenced) {
            AddType(referenced, owner, dependencies, countSelf: false);
        } else if(argument.Value is IEnumerable<CustomAttributeTypedArgument> items) {
            foreach(var item in items) {
                AddAttributeArgument(item, owner, dependencies);
            }
        }
    }

    /// <summary>Determines whether a method is a property or event accessor.</summary>
    /// <param name="method">The method to inspect.</param>
    /// <returns><see langword="true"/> for <c>get_</c>/<c>set_</c>/<c>add_</c>/<c>remove_</c> methods.</returns>
    private static bool IsAccessor(MethodInfo method) =>
        method.IsSpecialName
        && (method.Name.StartsWith("get_", StringComparison.Ordinal)
            || method.Name.StartsWith("set_", StringComparison.Ordinal)
            || method.Name.StartsWith("add_", StringComparison.Ordinal)
            || method.Name.StartsWith("remove_", StringComparison.Ordinal));

    /// <summary>Produces the DFS finish order used by Kosaraju's algorithm.</summary>
    /// <param name="nodes">The graph nodes.</param>
    /// <param name="adjacency">The forward adjacency.</param>
    /// <returns>Nodes in finish order.</returns>
    private static List<Type> BuildFinishOrder(Type[] nodes, Dictionary<Type, Type[]> adjacency) {
        var visited = new HashSet<Type>();
        var order = new List<Type>(nodes.Length);
        foreach(var start in nodes) {
            if(visited.Contains(start)) {
                continue;
            }

            var stack = new Stack<(Type Node, int Edge)>();
            visited.Add(start);
            stack.Push((start, 0));
            while(stack.Count != 0) {
                var (node, edge) = stack.Pop();
                var edges = adjacency[node];
                if(edge < edges.Length) {
                    stack.Push((node, edge + 1));
                    if(visited.Add(edges[edge])) {
                        stack.Push((edges[edge], 0));
                    }
                } else {
                    order.Add(node);
                }
            }
        }

        return order;
    }

    /// <summary>Builds the reversed adjacency used by Kosaraju's second pass.</summary>
    /// <param name="nodes">The graph nodes.</param>
    /// <param name="adjacency">The forward adjacency.</param>
    /// <returns>The reversed adjacency.</returns>
    private static Dictionary<Type, List<Type>> BuildReverse(Type[] nodes, Dictionary<Type, Type[]> adjacency) {
        var reverse = new Dictionary<Type, List<Type>>(nodes.Length);
        foreach(var node in nodes) {
            reverse[node] = [];
        }

        foreach(var node in nodes) {
            foreach(var dependency in adjacency[node]) {
                reverse[dependency].Add(node);
            }
        }

        return reverse;
    }

    /// <summary>Collects one strongly connected component from the reversed graph.</summary>
    /// <param name="start">The component seed.</param>
    /// <param name="reverse">The reversed adjacency.</param>
    /// <param name="visited">The global visited set.</param>
    /// <returns>The component members.</returns>
    private static List<Type> CollectComponent(Type start, Dictionary<Type, List<Type>> reverse, HashSet<Type> visited) {
        var component = new List<Type>();
        var stack = new Stack<Type>();
        stack.Push(start);
        while(stack.Count != 0) {
            var node = stack.Pop();
            component.Add(node);
            foreach(var previous in reverse[node].Where(visited.Add)) {
                stack.Push(previous);
            }
        }

        return component;
    }

    /// <summary>Finds one concrete cycle path inside a strongly connected component.</summary>
    /// <param name="component">The component members.</param>
    /// <param name="adjacency">The forward adjacency.</param>
    /// <returns>A path whose first and last entries are equal, or <see langword="null"/> when the component is acyclic.</returns>
    private static Type[]? FindCycle(List<Type> component, Dictionary<Type, Type[]> adjacency) {
        var start = component.OrderBy(static type => type.FullName, StringComparer.Ordinal).First();
        if(component.Count == 1) {
            return adjacency[start].Contains(start) ? [start, start] : null;
        }

        var members = new HashSet<Type>(component);
        var visited = new HashSet<Type> { start };
        var path = new List<Type> { start };
        return TryFindPath(start, start, members, adjacency, visited, path) ? [.. path] : null;
    }

    /// <summary>Depth-first search for a path from <paramref name="current"/> back to <paramref name="start"/>.</summary>
    /// <param name="current">The current node.</param>
    /// <param name="start">The path start and target node.</param>
    /// <param name="members">The component membership filter.</param>
    /// <param name="adjacency">The forward adjacency.</param>
    /// <param name="visited">The nodes already on the current path.</param>
    /// <param name="path">The current path.</param>
    /// <returns><see langword="true"/> when the path was completed.</returns>
    private static bool TryFindPath(
        Type current,
        Type start,
        HashSet<Type> members,
        Dictionary<Type, Type[]> adjacency,
        HashSet<Type> visited,
        List<Type> path) {
        foreach(var next in adjacency[current]) {
            if(next == start) {
                path.Add(start);
                return true;
            }

            if(!members.Contains(next) || !visited.Add(next)) {
                continue;
            }

            path.Add(next);
            if(TryFindPath(next, start, members, adjacency, visited, path)) {
                return true;
            }

            path.RemoveAt(path.Count - 1);
            visited.Remove(next);
        }

        return false;
    }
}
