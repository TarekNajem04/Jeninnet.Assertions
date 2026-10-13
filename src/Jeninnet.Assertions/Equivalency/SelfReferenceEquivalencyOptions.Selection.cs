//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Equivalency;

public partial class SelfReferenceEquivalencyOptions<TExpectation> {
    /// <summary>Excludes a member path (and its whole subtree) from the comparison.</summary>
    /// <remarks>Index exclusions (e.g. <c>x.Tags[0]</c>) apply to strict-ordered collections; unordered multiset matching has no stable indices.</remarks>
    /// <typeparam name="TMember">The member type selected by <paramref name="selector"/>.</typeparam>
    /// <param name="selector">A member access on the expectation (nested paths and constant indexers allowed).</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="selector"/> is not a member access on the expectation.</exception>
    public SelfReferenceEquivalencyOptions<TExpectation> Excluding<TMember>(Expression<Func<TExpectation, TMember>> selector) {
        if(selector is null) {
            throw new ArgumentNullException(nameof(selector));
        }

        _exclusions.Add(MemberPath.Parse(selector));
        return this;
    }

    /// <summary>Excludes every member whose value type is or derives from <typeparamref name="TMember"/>.</summary>
    /// <typeparam name="TMember">The value type to exclude; derived types are excluded as well. Use <see cref="Excluding(Type)"/> for open generic types.</typeparam>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> Excluding<TMember>() {
        _excludedMemberTypes.Add(typeof(TMember));
        return this;
    }

    /// <summary>Excludes every member whose value type is or derives from <paramref name="type"/>.</summary>
    /// <param name="type">The value type to exclude. For an open generic type definition all closed generics are excluded.</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
    public SelfReferenceEquivalencyOptions<TExpectation> Excluding(Type type) {
        if(type is null) {
            throw new ArgumentNullException(nameof(type));
        }

        _excludedMemberTypes.Add(type);
        return this;
    }

    /// <summary>Excludes the named members wherever they appear in the object graph.</summary>
    /// <param name="memberNames">The member names to exclude (matched ordinally).</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="memberNames"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="memberNames"/> is empty or contains a null or empty name.</exception>
    public SelfReferenceEquivalencyOptions<TExpectation> ExcludingMembersNamed(params string[] memberNames) {
        if(memberNames is null) {
            throw new ArgumentNullException(nameof(memberNames));
        }

        if(memberNames.Length == 0) {
            throw new ArgumentException("At least one member name must be specified.", nameof(memberNames));
        }

        foreach(var memberName in memberNames) {
            if(string.IsNullOrEmpty(memberName)) {
                throw new ArgumentException("Member names cannot be null or empty.", nameof(memberNames));
            }

            _excludedMemberNames.Add(memberName);
        }

        return this;
    }

    /// <summary>Excludes every member matching <paramref name="predicate"/>.</summary>
    /// <param name="predicate">Receives the reflection member (property or field) of the expectation.</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    public SelfReferenceEquivalencyOptions<TExpectation> Excluding(Func<MemberInfo, bool> predicate) {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        _excludePredicates.Add(predicate);
        return this;
    }

    /// <summary>Compares only the members matching <paramref name="predicate"/>, overriding the default "compare everything".</summary>
    /// <param name="predicate">Receives the reflection member (property or field) of the expectation.</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    public SelfReferenceEquivalencyOptions<TExpectation> Including(Func<MemberInfo, bool> predicate) {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        _includePredicates.Add(predicate);
        return this;
    }

    /// <summary>Re-includes a member path excluded by <see cref="Excluding{TMember}(Expression{Func{TExpectation, TMember}})"/> (e.g. one leaf of an excluded subtree).</summary>
    /// <typeparam name="TMember">The member type selected by <paramref name="selector"/>.</typeparam>
    /// <param name="selector">A member access on the expectation (nested paths and constant indexers allowed).</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="selector"/> is not a member access on the expectation.</exception>
    public SelfReferenceEquivalencyOptions<TExpectation> Including<TMember>(Expression<Func<TExpectation, TMember>> selector) {
        if(selector is null) {
            throw new ArgumentNullException(nameof(selector));
        }

        _inclusions.Add(MemberPath.Parse(selector));
        return this;
    }

    /// <summary>Compares public properties only; fields are no longer compared.</summary>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> IncludingProperties() {
        PropertyVisibility = MemberVisibility.Public;
        FieldVisibility = MemberVisibility.None;
        return this;
    }

    /// <summary>Compares public fields only; properties are no longer compared.</summary>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> IncludingFields() {
        PropertyVisibility = MemberVisibility.None;
        FieldVisibility = MemberVisibility.Public;
        return this;
    }

    /// <summary>Compares public and non-public properties; fields are no longer compared.</summary>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> IncludingInternalProperties() {
        PropertyVisibility = MemberVisibility.Public | MemberVisibility.Internal;
        FieldVisibility = MemberVisibility.None;
        return this;
    }

    /// <summary>Compares public and non-public fields; properties are no longer compared.</summary>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> IncludingInternalFields() {
        PropertyVisibility = MemberVisibility.None;
        FieldVisibility = MemberVisibility.Public | MemberVisibility.Internal;
        return this;
    }

    /// <summary>Compares only the public properties declared on the expectation's runtime type.</summary>
    /// <remarks>Inherited properties are not compared, and all previously registered selection rules are cleared.</remarks>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> IncludingAllDeclaredProperties() {
        ClearSelectionRules();
        PreferDeclaredMembers = true;
        return IncludingProperties();
    }

    /// <summary>Compares the public properties visible on the expectation's runtime type, including inherited ones.</summary>
    /// <remarks>All previously registered selection rules are cleared.</remarks>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> IncludingAllRuntimeProperties() {
        ClearSelectionRules();
        PreferDeclaredMembers = false;
        return IncludingProperties();
    }

    /// <summary>Stops comparing properties (fields are unaffected).</summary>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> ExcludingProperties() {
        PropertyVisibility = MemberVisibility.None;
        return this;
    }

    /// <summary>Stops comparing fields (properties are unaffected).</summary>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> ExcludingFields() {
        FieldVisibility = MemberVisibility.None;
        return this;
    }

    /// <summary>Skips explicitly implemented properties, which the runtime exposes as non-public members with dotted names.</summary>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> ExcludingExplicitlyImplementedProperties() {
        ExcludeExplicitlyImplemented = true;
        return this;
    }

    /// <summary>Skips expectation members marked with <see cref="EditorBrowsableAttribute"/>/<see cref="EditorBrowsableState.Never"/>.</summary>
    /// <remarks>The subject does not have to mark the matching member; use <see cref="IgnoringNonBrowsableMembersOnSubject"/> for that.</remarks>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> ExcludingNonBrowsableMembers() {
        ExcludeNonBrowsableOnExpectation = true;
        return this;
    }

    /// <summary>Treats non-browsable subject members as non-existent, so they never satisfy an expectation member.</summary>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> IgnoringNonBrowsableMembersOnSubject() {
        IgnoreNonBrowsableOnSubject = true;
        return this;
    }

    /// <summary>Ignores expectation members that the subject does not declare instead of reporting them.</summary>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> ExcludingMissingMembers() {
        ThrowOnMissingMembers = false;
        return this;
    }

    /// <summary>Requires the subject to declare every compared expectation member (the default).</summary>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> ThrowingOnMissingMembers() {
        ThrowOnMissingMembers = true;
        return this;
    }
}
