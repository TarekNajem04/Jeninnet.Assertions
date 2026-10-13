//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Equivalency;

/// <summary>Member kinds considered by the equivalency comparison.</summary>
[Flags]
internal enum MemberVisibility {
    /// <summary>No members of this kind are compared.</summary>
    None = 0,

    /// <summary>Non-public instance members (<c>internal</c>, <c>protected</c>, <c>private</c>) are compared.</summary>
    Internal = 1,

    /// <summary>Public instance members are compared.</summary>
    Public = 2,
}

/// <summary>A property or field taking part in the structural comparison.</summary>
internal sealed class MemberAccessor {
    private MemberAccessor(MemberInfo member) => Member = member;

    /// <summary>Gets the underlying reflection member.</summary>
    internal MemberInfo Member { get; }

    /// <summary>Gets the member name used in failure messages.</summary>
    internal string Name => Member.Name;

    /// <summary>Gets the declared type of the member value.</summary>
    internal Type MemberType => Member switch {
        PropertyInfo property => property.PropertyType,
        FieldInfo fieldInfo => fieldInfo.FieldType,
        _ => typeof(object),
    };

    /// <summary>
    /// Gets whether the member is visible in source-code editors
    /// (<see cref="EditorBrowsableAttribute"/> with <see cref="EditorBrowsableState.Never"/> hides it).
    /// </summary>
    internal bool IsBrowsable => Member.GetCustomAttribute<EditorBrowsableAttribute>() is not { State: EditorBrowsableState.Never };

    /// <summary>Creates an accessor for a property.</summary>
    /// <param name="property">The readable, non-indexed property.</param>
    /// <returns>The accessor.</returns>
    internal static MemberAccessor ForProperty(PropertyInfo property) => new(property);

    /// <summary>Creates an accessor for a field.</summary>
    /// <param name="field">The field.</param>
    /// <returns>The accessor.</returns>
    internal static MemberAccessor ForField(FieldInfo field) => new(field);

    /// <summary>
    /// Determines whether <paramref name="property"/> is an explicit interface implementation,
    /// which the runtime exposes as a non-public property whose name contains dots.
    /// </summary>
    /// <param name="property">The property to inspect.</param>
    /// <returns><see langword="true"/> when the property is explicitly implemented.</returns>
    internal static bool IsExplicitlyImplemented(PropertyInfo property) => property.Name.IndexOf('.') >= 0;

    /// <summary>Reads the member value from <paramref name="instance"/>.</summary>
    /// <param name="instance">The object owning the member.</param>
    /// <returns>The member value.</returns>
    /// <exception cref="InvalidOperationException">The member is neither a property nor a field.</exception>
    internal object? GetValue(object? instance) => Member switch {
        PropertyInfo property => property.GetValue(instance),
        FieldInfo fieldInfo => fieldInfo.GetValue(instance),
        _ => throw new InvalidOperationException("Only properties and fields can be compared."),
    };
}

/// <summary>A rule renaming an expectation member to a differently named subject member.</summary>
/// <param name="expectationPath">The expectation-side path the rule applies to.</param>
/// <param name="subjectPath">The subject-side path the mapped member is read from.</param>
/// <param name="expectationScope">The expectation type the rule is restricted to, or <see langword="null"/> for any type.</param>
/// <param name="subjectScope">The subject type the rule is restricted to, or <see langword="null"/> for any type.</param>
internal sealed class MemberMapping(string[] expectationPath, string[] subjectPath, Type? expectationScope, Type? subjectScope) {
    /// <summary>Gets the expectation-side path the rule applies to.</summary>
    internal string[] ExpectationPath { get; } = expectationPath;

    /// <summary>Gets the subject-side path the mapped member is read from.</summary>
    internal string[] SubjectPath { get; } = subjectPath;

    /// <summary>Gets the expectation type the rule is restricted to, or <see langword="null"/> for any type.</summary>
    internal Type? ExpectationScope { get; } = expectationScope;

    /// <summary>Gets the subject type the rule is restricted to, or <see langword="null"/> for any type.</summary>
    internal Type? SubjectScope { get; } = subjectScope;

    /// <summary>Gets the subject member name the expectation member is read from.</summary>
    internal string SubjectMemberName => SubjectPath[SubjectPath.Length - 1];

    /// <summary>Determines whether the rule applies at the given path and pair of declaring types.</summary>
    /// <param name="path">The full path of the expectation member, including its own name.</param>
    /// <param name="expectationType">The runtime type owning the expectation member.</param>
    /// <param name="subjectType">The runtime type owning the subject member.</param>
    /// <returns><see langword="true"/> when the rule maps this member.</returns>
    internal bool Matches(IReadOnlyList<string> path, Type expectationType, Type subjectType) {
        if(ExpectationScope is not null || SubjectScope is not null) {
            // A type-scoped rule renames a direct member wherever that type pair appears in the graph.
            return (ExpectationScope is null || ExpectationScope.IsAssignableFrom(expectationType))
                && (SubjectScope is null || SubjectScope.IsAssignableFrom(subjectType))
                && string.Equals(ExpectationPath[ExpectationPath.Length - 1], path[path.Count - 1], StringComparison.Ordinal);
        }

        return PathEquals(ExpectationPath, path);
    }

    /// <summary>Compares two paths while ignoring collection indices, which have no stable position in unordered graphs.</summary>
    /// <param name="rule">The path declared by a rule.</param>
    /// <param name="path">The path reached by the comparison.</param>
    /// <returns><see langword="true"/> when both paths address the same member.</returns>
    private static bool PathEquals(string[] rule, IReadOnlyList<string> path) {
        var ruleIndex = 0;
        var pathIndex = 0;
        while(ruleIndex < rule.Length && pathIndex < path.Count) {
            if(MemberPath.IsIndex(rule[ruleIndex])) {
                ruleIndex++;
                continue;
            }

            if(MemberPath.IsIndex(path[pathIndex])) {
                pathIndex++;
                continue;
            }

            if(!string.Equals(rule[ruleIndex], path[pathIndex], StringComparison.Ordinal)) {
                return false;
            }

            ruleIndex++;
            pathIndex++;
        }

        while(ruleIndex < rule.Length && MemberPath.IsIndex(rule[ruleIndex])) {
            ruleIndex++;
        }

        while(pathIndex < path.Count && MemberPath.IsIndex(path[pathIndex])) {
            pathIndex++;
        }

        return ruleIndex == rule.Length && pathIndex == path.Count;
    }
}
