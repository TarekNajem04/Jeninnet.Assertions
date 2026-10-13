//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Equivalency;

/// <summary>Decides whether an expectation member takes part in the comparison.</summary>
/// <remarks>Register with <c>UsingSelectionRule</c>; a member is compared only when every rule includes it.</remarks>
public interface IMemberSelectionRule {
    /// <summary>Determines whether <paramref name="member"/> is compared.</summary>
    /// <param name="member">The reflection member (property or field) of the expectation.</param>
    /// <returns><see langword="true"/> to keep the member.</returns>
    bool Includes(MemberInfo member);
}

/// <summary>Resolves the subject member an expectation member is read from.</summary>
/// <remarks>Register with <c>UsingMatchingRule</c>; the first rule returning a non-null name wins over later rules.</remarks>
public interface IMemberMatchingRule {
    /// <summary>Resolves the subject member name for an expectation member.</summary>
    /// <param name="path">The full path of the expectation member, including its own name.</param>
    /// <param name="expectationType">The runtime type owning the expectation member.</param>
    /// <param name="subjectType">The runtime type owning the subject member.</param>
    /// <returns>The subject member name, or <see langword="null"/> when this rule does not apply.</returns>
    string? ResolveSubjectMember(IReadOnlyList<string> path, Type expectationType, Type subjectType);
}

/// <summary>Decides whether the collection at a path compares order-sensitively.</summary>
/// <remarks>Register with <c>UsingOrderingRule</c>; the last registered rule returning a value wins over earlier rules and the global ordering.</remarks>
public interface IOrderingRule {
    /// <summary>Determines the ordering for the collection at <paramref name="path"/>.</summary>
    /// <param name="path">The member path segments from the root.</param>
    /// <returns><see langword="true"/> for strict ordering, <see langword="false"/> for multiset comparison, or <see langword="null"/> when this rule does not apply.</returns>
    bool? IsOrdered(IReadOnlyList<string> path);
}
