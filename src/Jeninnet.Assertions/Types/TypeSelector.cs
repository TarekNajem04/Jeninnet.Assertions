//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

/// <summary>Filters a set of types down to an architecture-relevant subset. Each filter returns a new selector.</summary>
public class TypeSelector : IEnumerable<Type> {
    private readonly Type[] _types;

    /// <summary>Creates a selector over <paramref name="types"/>.</summary>
    /// <param name="types">The candidate types.</param>
    /// <exception cref="ArgumentNullException"><paramref name="types"/> is <see langword="null"/>.</exception>
    public TypeSelector(IEnumerable<Type> types) {
        if(types is null) {
            throw new ArgumentNullException(nameof(types));
        }

        _types = [.. types];
    }

    /// <summary>Keeps types that derive from <typeparamref name="T"/> (excluding <typeparamref name="T"/> itself).</summary>
    /// <typeparam name="T">The base type.</typeparam>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatDeriveFrom<T>() =>
        new(_types.Where(static type => type != typeof(T) && typeof(T).IsAssignableFrom(type)));

    /// <summary>Keeps types that do not derive from <typeparamref name="T"/> (including <typeparamref name="T"/> itself).</summary>
    /// <typeparam name="T">The base type.</typeparam>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatDoNotDeriveFrom<T>() =>
        new(_types.Where(static type => type == typeof(T) || !typeof(T).IsAssignableFrom(type)));

    /// <summary>Keeps types that implement <typeparamref name="TInterface"/> (excluding the interface itself).</summary>
    /// <typeparam name="TInterface">The interface type.</typeparam>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatImplement<TInterface>() =>
        new(_types.Where(static type => type != typeof(TInterface) && typeof(TInterface).IsAssignableFrom(type) && !type.IsInterface));

    /// <summary>Keeps types that do not implement <typeparamref name="TInterface"/> (excluding the interface itself).</summary>
    /// <typeparam name="TInterface">The interface type.</typeparam>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatDoNotImplement<TInterface>() =>
        new(_types.Where(static type => type != typeof(TInterface) && (type.IsInterface || !typeof(TInterface).IsAssignableFrom(type))));

    /// <summary>Keeps types declared with <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatAreDecoratedWith<TAttribute>() where TAttribute : Attribute =>
        new(_types.Where(static type => type.GetCustomAttributes(typeof(TAttribute), inherit: false).Length != 0));

    /// <summary>Keeps types in exactly <paramref name="namespaceName"/>.</summary>
    /// <param name="namespaceName">The namespace name (exact match).</param>
    /// <returns>A new selector with the filtered types.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="namespaceName"/> is <see langword="null"/>.</exception>
    public TypeSelector ThatAreInNamespace(string namespaceName) {
        if(namespaceName is null) {
            throw new ArgumentNullException(nameof(namespaceName));
        }

        return new(_types.Where(type => string.Equals(type.Namespace, namespaceName, StringComparison.Ordinal)));
    }

    /// <summary>Keeps types not declared in exactly <paramref name="namespaceName"/>.</summary>
    /// <param name="namespaceName">The namespace name (exact match).</param>
    /// <returns>A new selector with the filtered types.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="namespaceName"/> is <see langword="null"/>.</exception>
    public TypeSelector ThatAreNotInNamespace(string namespaceName) {
        if(namespaceName is null) {
            throw new ArgumentNullException(nameof(namespaceName));
        }

        return new(_types.Where(type => !string.Equals(type.Namespace, namespaceName, StringComparison.Ordinal)));
    }

    /// <summary>Keeps types declared in <paramref name="namespaceName"/> or in a namespace nested under it.</summary>
    /// <param name="namespaceName">The namespace prefix (matched on segment boundaries).</param>
    /// <returns>A new selector with the filtered types.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="namespaceName"/> is <see langword="null"/>.</exception>
    public TypeSelector ThatAreUnderNamespace(string namespaceName) {
        if(namespaceName is null) {
            throw new ArgumentNullException(nameof(namespaceName));
        }

        return new(_types.Where(type => IsUnderNamespace(type, namespaceName)));
    }

    /// <summary>Keeps types neither declared in <paramref name="namespaceName"/> nor under a namespace nested in it.</summary>
    /// <param name="namespaceName">The namespace prefix (matched on segment boundaries).</param>
    /// <returns>A new selector with the filtered types.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="namespaceName"/> is <see langword="null"/>.</exception>
    public TypeSelector ThatAreNotUnderNamespace(string namespaceName) {
        if(namespaceName is null) {
            throw new ArgumentNullException(nameof(namespaceName));
        }

        return new(_types.Where(type => !IsUnderNamespace(type, namespaceName)));
    }

    /// <summary>Determines whether a type's namespace equals or nests under a namespace prefix.</summary>
    /// <param name="type">The type to inspect.</param>
    /// <param name="namespaceName">The namespace prefix (matched on segment boundaries).</param>
    /// <returns><see langword="true"/> when the type's namespace matches or nests under the prefix.</returns>
    private static bool IsUnderNamespace(Type type, string namespaceName) {
        var ns = type.Namespace;
        if(ns is null) {
            return false;
        }

        return string.Equals(ns, namespaceName, StringComparison.Ordinal)
            || (ns.StartsWith(namespaceName, StringComparison.Ordinal) && ns[namespaceName.Length] == '.');
    }

    /// <summary>Keeps class types (including delegates; excluding interfaces and value types).</summary>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatAreClasses() =>
        new(_types.Where(static type => type.IsClass));

    /// <summary>Keeps types that are not classes.</summary>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatAreNotClasses() =>
        new(_types.Where(static type => !type.IsClass));

    /// <summary>Keeps interface types.</summary>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatAreInterfaces() =>
        new(_types.Where(static type => type.IsInterface));

    /// <summary>Keeps types that are not interfaces.</summary>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatAreNotInterfaces() =>
        new(_types.Where(static type => !type.IsInterface));

    /// <summary>Keeps value types (structs and enums).</summary>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatAreValueTypes() =>
        new(_types.Where(static type => type.IsValueType));

    /// <summary>Keeps types that are not value types.</summary>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatAreNotValueTypes() =>
        new(_types.Where(static type => !type.IsValueType));

    /// <summary>Keeps static classes (abstract and sealed).</summary>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatAreStatic() =>
        new(_types.Where(static type => type.IsAbstract && type.IsSealed));

    /// <summary>Keeps types that are not static classes.</summary>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatAreNotStatic() =>
        new(_types.Where(static type => !type.IsAbstract || !type.IsSealed));

    /// <summary>Keeps abstract types (interfaces count as abstract).</summary>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatAreAbstract() =>
        new(_types.Where(static type => type.IsAbstract));

    /// <summary>Keeps types that are not abstract.</summary>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatAreNotAbstract() =>
        new(_types.Where(static type => !type.IsAbstract));

    /// <summary>Keeps sealed types (single-member value types and static classes count as sealed).</summary>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatAreSealed() =>
        new(_types.Where(static type => type.IsSealed));

    /// <summary>Keeps types that are not sealed.</summary>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatAreNotSealed() =>
        new(_types.Where(static type => !type.IsSealed));

    /// <summary>Keeps types not declared with <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatAreNotDecoratedWith<TAttribute>() where TAttribute : Attribute =>
        new(_types.Where(static type => type.GetCustomAttributes(typeof(TAttribute), inherit: false).Length == 0));

    /// <summary>Keeps types declared with <typeparamref name="TAttribute"/> or inheriting it from a base type.</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatAreDecoratedWithOrInherit<TAttribute>() where TAttribute : Attribute =>
        new(_types.Where(static type => IsDecoratedOrInherited(type, typeof(TAttribute))));

    /// <summary>Keeps types neither declared with <typeparamref name="TAttribute"/> nor inheriting it.</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <returns>A new selector with the filtered types.</returns>
    public TypeSelector ThatAreNotDecoratedWithOrInherit<TAttribute>() where TAttribute : Attribute =>
        new(_types.Where(static type => !IsDecoratedOrInherited(type, typeof(TAttribute))));

    /// <summary>Keeps types satisfying <paramref name="predicate"/>.</summary>
    /// <param name="predicate">The condition a type must satisfy to be kept.</param>
    /// <returns>A new selector with the filtered types.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    public TypeSelector ThatSatisfy(Func<Type, bool> predicate) {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        return new(_types.Where(predicate.Invoke));
    }

    /// <summary>Replaces task-like types with their result types (<c>Task</c> becomes <c>void</c>).</summary>
    /// <returns>A new selector with the unwrapped types.</returns>
    public TypeSelector UnwrapTaskTypes() =>
        new(_types.Select(static type => UnwrapTask(type)));

    /// <summary>Replaces enumerable types with their element types (strings stay untouched).</summary>
    /// <returns>A new selector with the unwrapped types.</returns>
    public TypeSelector UnwrapEnumerableTypes() =>
        new(_types.Select(static type => UnwrapEnumerable(type)));

    /// <summary>Determines whether a type or any base type carries the attribute.</summary>
    /// <param name="type">The type to inspect.</param>
    /// <param name="attributeType">The attribute type to look for.</param>
    /// <returns><see langword="true"/> when the attribute is declared on the type or an ancestor.</returns>
    private static bool IsDecoratedOrInherited(Type type, Type attributeType) {
        for(var current = type; current is not null; current = current.BaseType) {
            if(current.GetCustomAttributes(attributeType, inherit: false).Length != 0) {
                return true;
            }
        }

        return false;
    }

    /// <summary>Unwraps a task-like type to its result type.</summary>
    /// <param name="type">The type to unwrap.</param>
    /// <returns><see cref="void"/> for non-generic tasks, the result type for generic ones, else the original type.</returns>
    private static Type UnwrapTask(Type type) {
        if(type == typeof(Task) || type == typeof(ValueTask)) {
            return typeof(void);
        }

        if(type.IsGenericType) {
            var definition = type.GetGenericTypeDefinition();
            if(definition == typeof(Task<>) || definition == typeof(ValueTask<>)) {
                return type.GetGenericArguments()[0];
            }
        }

        return type;
    }

    /// <summary>Unwraps an enumerable type to its element type, leaving strings untouched.</summary>
    /// <param name="type">The type to unwrap.</param>
    /// <returns>The array element type, the <see cref="IEnumerable{T}"/> argument, or the original type.</returns>
    private static Type UnwrapEnumerable(Type type) {
        if(type == typeof(string)) {
            return type;
        }

        if(type.IsArray) {
            return type.GetElementType() ?? type;
        }

        if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)) {
            return type.GetGenericArguments()[0];
        }

        foreach(var contract in type.GetInterfaces()) {
            if(contract.IsGenericType && contract.GetGenericTypeDefinition() == typeof(IEnumerable<>)) {
                return contract.GetGenericArguments()[0];
            }
        }

        return type;
    }

    /// <summary>Materializes the selected types.</summary>
    /// <returns>A new array with the selected types.</returns>
    public Type[] ToArray() => [.. _types];

    /// <summary>Returns an enumerator that iterates through the selected types.</summary>
    /// <returns>An enumerator over the selected types.</returns>
    public IEnumerator<Type> GetEnumerator() => ((IEnumerable<Type>)_types).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>Creates assertions over the selected types.</summary>
    /// <returns>A <see cref="TypeSelectorAssertions"/> for the selected types.</returns>
    public TypeSelectorAssertions Should() => new([.. _types]);
}
