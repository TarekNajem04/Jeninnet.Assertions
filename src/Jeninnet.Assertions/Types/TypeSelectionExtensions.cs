//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

/// <summary>Starts member and nested-type selections from a <see cref="Type"/> for architecture assertions.</summary>
public static class TypeSelectionExtensions {
    /// <summary>Selects the declared methods of <paramref name="type"/>.</summary>
    /// <param name="type">The type whose declared methods are selected.</param>
    /// <returns>A <see cref="MethodInfoSelector"/> over the declared methods.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
    public static MethodInfoSelector Methods(this Type type) => new(type ?? throw new ArgumentNullException(nameof(type)));

    /// <summary>Selects the declared properties of <paramref name="type"/>.</summary>
    /// <param name="type">The type whose declared properties are selected.</param>
    /// <returns>A <see cref="PropertyInfoSelector"/> over the declared properties.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
    public static PropertyInfoSelector Properties(this Type type) => new(type ?? throw new ArgumentNullException(nameof(type)));

    /// <summary>Selects the nested types declared in <paramref name="type"/> (public and non-public).</summary>
    /// <remarks>Compiler-generated types (async state machines, iterators) are excluded: selection sees source-level architecture.</remarks>
    /// <param name="type">The type whose nested types are selected.</param>
    /// <returns>A <see cref="TypeSelector"/> over the nested types.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
    public static TypeSelector Types(this Type type) {
        if(type is null) {
            throw new ArgumentNullException(nameof(type));
        }

        return new(type.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
            .Where(static nested => !nested.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false)));
    }
}
