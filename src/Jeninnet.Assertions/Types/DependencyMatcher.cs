//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

/// <summary>Matches reflected dependencies against namespace or type-name targets used by architecture rules.</summary>
internal static class DependencyMatcher {
    /// <summary>Determines whether a dependency matches a namespace prefix, type full name, or simple type name.</summary>
    /// <param name="dependency">The reflected dependency.</param>
    /// <param name="target">The namespace prefix, full type name (dots or nested <c>+</c>), or simple type name.</param>
    /// <returns><see langword="true"/> when the dependency matches.</returns>
    internal static bool Matches(Type dependency, string target) {
        if(string.Equals(dependency.Name, target, StringComparison.Ordinal)) {
            return true;
        }

        if(string.Equals(FullName(dependency), target, StringComparison.Ordinal)) {
            return true;
        }

        if(dependency.IsGenericType && !dependency.IsGenericTypeDefinition && Matches(dependency.GetGenericTypeDefinition(), target)) {
            return true;
        }

        var ns = dependency.Namespace;
        return string.Equals(ns, target, StringComparison.Ordinal)
            || (ns is not null && ns.StartsWith(target + ".", StringComparison.Ordinal));
    }

    /// <summary>Formats a reflected type for an offender list.</summary>
    /// <param name="type">The type to format.</param>
    /// <returns>The full name with nested separators normalized to dots.</returns>
    internal static string Describe(Type type) => FullName(type);

    /// <summary>Formats a type reference as a dependency target string.</summary>
    /// <param name="type">The target type.</param>
    /// <returns>The normalized full name, or the generic type definition for constructed generics.</returns>
    internal static string TargetName(Type type) =>
        FullName(type.IsGenericType && !type.IsGenericTypeDefinition ? type.GetGenericTypeDefinition() : type);

    /// <summary>Gets a stable full name with nested separators normalized to dots.</summary>
    /// <param name="type">The type to format.</param>
    /// <returns>The normalized full name, or the simple name when no full name is available.</returns>
    private static string FullName(Type type) => (type.FullName ?? type.Name).Replace('+', '.');
}
