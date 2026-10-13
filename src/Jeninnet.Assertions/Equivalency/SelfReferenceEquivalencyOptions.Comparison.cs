//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Equivalency;

public partial class SelfReferenceEquivalencyOptions<TExpectation> {
    /// <summary>Compares values of type <typeparamref name="T"/> with <see cref="object.Equals(object?, object?)"/> instead of memberwise.</summary>
    /// <typeparam name="T">The type to compare by value.</typeparam>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> ComparingByValue<T>() {
        _byValueTypes.Add(typeof(T));
        return this;
    }

    /// <summary>Compares values of type <paramref name="type"/> with <see cref="object.Equals(object?, object?)"/> instead of memberwise.</summary>
    /// <param name="type">The type to compare by value.</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
    public SelfReferenceEquivalencyOptions<TExpectation> ComparingByValue(Type type) {
        if(type is null) {
            throw new ArgumentNullException(nameof(type));
        }

        _byValueTypes.Add(type);
        return this;
    }

    /// <summary>Requires collections to match in the same order.</summary>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> WithStrictOrdering() {
        StrictOrdering = true;
        return this;
    }

    /// <summary>Compares collections as unordered multisets (the default).</summary>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> WithoutStrictOrdering() {
        StrictOrdering = false;
        return this;
    }

    /// <summary>Silently cuts off cyclic references instead of reporting them as failures.</summary>
    /// <remarks>Mutually exclusive with <see cref="IgnoringCyclicReferences"/>; the last call wins.</remarks>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> AllowingInfiniteRecursion() {
        AllowInfiniteRecursion = true;
        IgnoreCyclicReferences = false;
        return this;
    }

    /// <summary>Requires the subject and expectation runtime types to match exactly on every compared pair.</summary>
    /// <remarks>
    /// Duck-typing is off while strict: a derived subject no longer satisfies a base expectation, and member
    /// values of different runtime types fail even when they look alike. Members registered with
    /// <see cref="WithoutStrictTypingFor"/> stay lenient. Strict typing beats
    /// <see cref="WithAutoConversion"/> on the same member in either call order.
    /// </remarks>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> WithStrictTyping() {
        StrictTyping = true;
        return this;
    }

    /// <summary>Restores lenient runtime-type matching everywhere, clearing scoped strict rules too.</summary>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> WithoutStrictTyping() {
        StrictTyping = false;
        _strictForPredicates.Clear();
        return this;
    }

    /// <summary>Requires exact runtime-type matches only for members selected by <paramref name="predicate"/>.</summary>
    /// <param name="predicate">Receives the reflection member (property or field) of the expectation.</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    public SelfReferenceEquivalencyOptions<TExpectation> WithStrictTypingFor(Func<MemberInfo, bool> predicate) {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        _strictForPredicates.Add(predicate);
        return this;
    }

    /// <summary>Exempts members selected by <paramref name="predicate"/> from global strict typing.</summary>
    /// <param name="predicate">Receives the reflection member (property or field) of the expectation.</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    public SelfReferenceEquivalencyOptions<TExpectation> WithoutStrictTypingFor(Func<MemberInfo, bool> predicate) {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        _strictExceptPredicates.Add(predicate);
        return this;
    }

    /// <summary>Compares members declared on the expectation's runtime type (the default).</summary>
    /// <remarks>Explicitly restores runtime-type member selection after <see cref="PreferringDeclaredMemberTypes"/>.</remarks>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> PreferringRuntimeMemberTypes() {
        MemberTypePreference = MemberTypePreference.Runtime;
        return this;
    }

    /// <summary>Compares only the members declared on the compile-time expectation type at the graph root.</summary>
    /// <remarks>Nested graphs still use runtime types because declared member types are not observable there.</remarks>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> PreferringDeclaredMemberTypes() {
        MemberTypePreference = MemberTypePreference.Declared;
        return this;
    }

    /// <summary>Restores the default member-type preference (runtime types).</summary>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> PreferringDefaultMemberTypes() {
        MemberTypePreference = MemberTypePreference.Default;
        return this;
    }

    /// <summary>Converts values with <see cref="Convert.ChangeType(object?, Type, IFormatProvider)"/> when their runtime types differ.</summary>
    /// <remarks>
    /// The subject converts toward the expectation type first, then the other way around; unconvertible pairs
    /// keep failing on the type difference. Strict typing beats conversion on the same member in either order.
    /// Members registered with <see cref="WithoutAutoConversionFor"/> are exempt.
    /// </remarks>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> WithAutoConversion() {
        AutoConversion = true;
        return this;
    }

    /// <summary>Restores strict type-difference failures everywhere, clearing scoped conversion rules too.</summary>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> WithoutAutoConversion() {
        AutoConversion = false;
        _conversionForPredicates.Clear();
        return this;
    }

    /// <summary>Allows conversion only for members selected by <paramref name="predicate"/>.</summary>
    /// <param name="predicate">Receives the reflection member (property or field) of the expectation.</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    public SelfReferenceEquivalencyOptions<TExpectation> WithAutoConversionFor(Func<MemberInfo, bool> predicate) {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        _conversionForPredicates.Add(predicate);
        return this;
    }

    /// <summary>Exempts members selected by <paramref name="predicate"/> from global auto-conversion.</summary>
    /// <param name="predicate">Receives the reflection member (property or field) of the expectation.</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    public SelfReferenceEquivalencyOptions<TExpectation> WithoutAutoConversionFor(Func<MemberInfo, bool> predicate) {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        _conversionExceptPredicates.Add(predicate);
        return this;
    }

    /// <summary>Compares enums by their numeric value, so different enum types with the same value match.</summary>
    /// <remarks>Mutually exclusive with <see cref="ComparingEnumsByName"/>; the last call wins.</remarks>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> ComparingEnumsByValue() {
        EnumComparison = EnumComparison.ByValue;
        return this;
    }

    /// <summary>Compares enums by their identifier name, so equal values with different names fail.</summary>
    /// <remarks>
    /// Unnamed values (combined flags or undefined numbers) fall back to the numeric value.
    /// Mutually exclusive with <see cref="ComparingEnumsByValue"/>; the last call wins.
    /// </remarks>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> ComparingEnumsByName() {
        EnumComparison = EnumComparison.ByName;
        return this;
    }

    /// <summary>Compares values of type <typeparamref name="T"/> memberwise instead of with <see cref="object.Equals(object?, object?)"/>.</summary>
    /// <typeparam name="T">The type to compare by members.</typeparam>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> ComparingByMembers<T>() {
        _byMembersTypes.Add(typeof(T));
        return this;
    }

    /// <summary>Compares values of type <paramref name="type"/> memberwise instead of with <see cref="object.Equals(object?, object?)"/>.</summary>
    /// <param name="type">The type to compare by members.</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
    public SelfReferenceEquivalencyOptions<TExpectation> ComparingByMembers(Type type) {
        if(type is null) {
            throw new ArgumentNullException(nameof(type));
        }

        _byMembersTypes.Add(type);
        return this;
    }

    /// <summary>Compares records with <see cref="object.Equals(object?, object?)"/> instead of memberwise.</summary>
    /// <remarks>Mutually exclusive with <see cref="ComparingRecordsByMembers"/>; the last call wins.</remarks>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> ComparingRecordsByValue() {
        RecordsByValue = true;
        return this;
    }

    /// <summary>Compares records memberwise (the default).</summary>
    /// <remarks>Explicitly restores memberwise record comparison after <see cref="ComparingRecordsByValue"/>.</remarks>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> ComparingRecordsByMembers() {
        RecordsByValue = false;
        return this;
    }

    /// <summary>Treats a <see langword="null"/> string and an empty string as equal.</summary>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> ComparingNullStringsAsEmpty() {
        NullStringsAsEmpty = true;
        return this;
    }

    /// <summary>Treats a <see langword="null"/> collection and an empty collection as equal.</summary>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> ComparingNullCollectionsAsEmpty() {
        NullCollectionsAsEmpty = true;
        return this;
    }
}
