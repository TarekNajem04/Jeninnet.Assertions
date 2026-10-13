//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Equivalency;

/// <summary>
/// Configures object-graph equivalency checks for <c>BeEquivalentTo</c> (see ROADMAP.md phase P4).
/// </summary>
/// <remarks>
/// Default behavior: every public instance property and field of the expectation is compared against the
/// subject (extra subject members are ignored); collections compare as unordered multisets; a member present
/// on the expectation but missing on the subject fails. Paths use dotted notation with bracketed indices/keys
/// (e.g. <c>Address.City</c>, <c>Tags[0]</c>). Excluding a path excludes its whole subtree; a longer
/// <c>Including</c> path re-includes part of an excluded subtree.
/// <para>
/// When several options conflict, <b>the last call wins</b>: <c>IncludingFields()</c> after
/// <c>IncludingProperties()</c> leaves only fields compared, and <c>ExcludingProperties()</c> removes the
/// properties again. Selection rules (<c>Excluding</c>/<c>Including</c> by path, name, type or predicate)
/// accumulate instead, so <c>IncludingAllDeclaredProperties()</c> and <c>IncludingAllRuntimeProperties()</c>
/// clear them to restore a pure visibility-based selection. Mapping rules accumulate too and are applied
/// independently of selection.
/// </para>
/// <para>
/// Meaning options follow the same rule with one documented priority: <b>strict typing beats
/// auto-conversion</b> on the same member, in either call order, because silently converting a value whose
/// type was required to match exactly would contradict the stricter demand. Per-member predicates
/// (<c>...For</c>) scope a single member edge; the members below it use the ambient rules again.
/// </para>
/// <para>
/// Two behaviors are implicit and stay that way: simple values of different runtime types (e.g. <c>int</c>
/// versus <c>long</c>) already fail without any option, and cyclic references already fail unless
/// <c>AllowingInfiniteRecursion()</c> or <c>IgnoringCyclicReferences()</c> cuts them off. There is
/// deliberately no auto-fix option: rewriting the subject inside an assertion would mutate user data.
/// </para>
/// </remarks>
/// <typeparam name="TExpectation">The compile-time type of the expectation.</typeparam>
public partial class SelfReferenceEquivalencyOptions<TExpectation> {
    private readonly List<string[]> _exclusions = [];
    private readonly List<string[]> _inclusions = [];
    private readonly HashSet<Type> _byValueTypes = [];
    private readonly HashSet<string> _excludedMemberNames = new(StringComparer.Ordinal);
    private readonly List<Type> _excludedMemberTypes = [];
    private readonly List<Func<MemberInfo, bool>> _excludePredicates = [];
    private readonly List<Func<MemberInfo, bool>> _includePredicates = [];
    private readonly List<Func<MemberInfo, bool>> _strictForPredicates = [];
    private readonly List<Func<MemberInfo, bool>> _strictExceptPredicates = [];
    private readonly List<Func<MemberInfo, bool>> _conversionForPredicates = [];
    private readonly List<Func<MemberInfo, bool>> _conversionExceptPredicates = [];
    private readonly HashSet<Type> _byMembersTypes = [];
    private readonly List<(string[] Path, bool Strict)> _orderingRules = [];
    private readonly List<(string[] Path, TimeSpan Tolerance)> _dateTimeToleranceRules = [];
    private readonly List<(string[] Path, double Tolerance)> _numericToleranceRules = [];
    private readonly List<MemberMapping> _mappings = [];
    private readonly List<IMemberSelectionRule> _selectionRules = [];
    private readonly List<IMemberMatchingRule> _matchingRules = [];
    private readonly List<IOrderingRule> _orderingRuleObjects = [];
    private const string NEGATIVE_TOLERANCE_MESSAGE = "Tolerance must not be negative.";

    /// <summary>Creates options seeded from the effective global template (ambient <c>Modify</c> override or process-wide defaults).</summary>
    public SelfReferenceEquivalencyOptions() {
        InitializerRunner.EnsureInitialized();
        var defaults = AssertionConfiguration.Equivalency.CurrentTemplate();
        StrictOrdering = defaults.StrictOrdering;
        StrictTyping = defaults.StrictTyping;
        AutoConversion = defaults.AutoConversion;
        MemberTypePreference = defaults.MemberTypePreference;
        EnumComparison = defaults.EnumComparison;
        RecordsByValue = defaults.RecordsByValue;
        NullStringsAsEmpty = defaults.NullStringsAsEmpty;
        NullCollectionsAsEmpty = defaults.NullCollectionsAsEmpty;
        AllowInfiniteRecursion = defaults.AllowInfiniteRecursion;
        IgnoreCyclicReferences = defaults.IgnoreCyclicReferences;
        ShallowComparison = defaults.ShallowComparison;
        IgnoreJsonPropertyCasing = defaults.IgnoreJsonPropertyCasing;
        GraphIgnoreCase = defaults.GraphIgnoreCase;
        GraphTrimStart = defaults.GraphTrimStart;
        GraphTrimEnd = defaults.GraphTrimEnd;
        GraphNormalizeNewlines = defaults.GraphNormalizeNewlines;
        IncludeFullStringsInDifference = defaults.IncludeFullStringsInDifference;
        Plan.CopyFrom(defaults.Plan);
    }

    /// <summary>Gets whether collections must match in the same order (default compares as unordered multisets).</summary>
    public bool StrictOrdering { get; private set; }

    /// <summary>Gets the graph-wide absolute tolerance for date/time instants, or <see langword="null"/> when they must match exactly.</summary>
    public TimeSpan? DateTimeTolerance { get; private set; }

    /// <summary>Gets the graph-wide absolute tolerance for floating-point and decimal values, or <see langword="null"/> when they must match exactly.</summary>
    public double? NumericTolerance { get; private set; }

    /// <summary>Gets whether cyclic references are silently cut off (default reports them as failures).</summary>
    public bool AllowInfiniteRecursion { get; private set; }

    /// <summary>Gets whether every compared pair must share the exact runtime type.</summary>
    internal bool StrictTyping { get; private set; }

    /// <summary>Gets whether differing runtime types may convert toward each other.</summary>
    internal bool AutoConversion { get; private set; }

    /// <summary>Gets which member set the root expectation contributes.</summary>
    internal MemberTypePreference MemberTypePreference { get; private set; }

    /// <summary>Gets how enum values are compared.</summary>
    internal EnumComparison EnumComparison { get; private set; }

    /// <summary>Gets whether records compare with <see cref="object.Equals(object?, object?)"/> instead of memberwise.</summary>
    internal bool RecordsByValue { get; private set; }

    /// <summary>Gets whether a <see langword="null"/> string equals an empty string.</summary>
    internal bool NullStringsAsEmpty { get; private set; }

    /// <summary>Gets whether a <see langword="null"/> collection equals an empty collection.</summary>
    internal bool NullCollectionsAsEmpty { get; private set; }

    /// <summary>Gets whether cyclic references are silently cut off (the <see cref="IgnoringCyclicReferences"/> alternative).</summary>
    internal bool IgnoreCyclicReferences { get; private set; }

    /// <summary>Gets whether nested values compare with <see cref="object.Equals(object?, object?)"/> instead of recursing.</summary>
    internal bool ShallowComparison { get; private set; }

    /// <summary>Gets whether string dictionary keys match ignoring case.</summary>
    internal bool IgnoreJsonPropertyCasing { get; private set; }

    /// <summary>Gets whether graph strings compare ignoring case.</summary>
    internal bool GraphIgnoreCase { get; private set; }

    /// <summary>Gets whether graph strings ignore leading whitespace.</summary>
    internal bool GraphTrimStart { get; private set; }

    /// <summary>Gets whether graph strings ignore trailing whitespace.</summary>
    internal bool GraphTrimEnd { get; private set; }

    /// <summary>Gets whether graph strings normalize newline styles.</summary>
    internal bool GraphNormalizeNewlines { get; private set; }

    /// <summary>Gets whether string differences report the full text instead of a bounded fragment.</summary>
    internal bool IncludeFullStringsInDifference { get; private set; }

    /// <summary>Gets which properties are compared.</summary>
    internal MemberVisibility PropertyVisibility { get; private set; } = MemberVisibility.Public;

    /// <summary>Gets which fields are compared.</summary>
    internal MemberVisibility FieldVisibility { get; private set; } = MemberVisibility.Public;

    /// <summary>Gets whether only members declared on the expectation's runtime type are compared.</summary>
    internal bool PreferDeclaredMembers { get; private set; }

    /// <summary>Gets whether explicitly implemented properties are skipped.</summary>
    internal bool ExcludeExplicitlyImplemented { get; private set; }

    /// <summary>Gets whether a member missing on the subject fails (default) or is ignored.</summary>
    internal bool ThrowOnMissingMembers { get; private set; } = true;

    /// <summary>Gets whether non-browsable expectation members are skipped.</summary>
    internal bool ExcludeNonBrowsableOnExpectation { get; private set; }

    /// <summary>Gets whether non-browsable subject members count as non-existent.</summary>
    internal bool IgnoreNonBrowsableOnSubject { get; private set; }

    /// <summary>Gets whether the visibility options are still untouched, so the engine may compare leaf types with <see cref="object.Equals(object?, object?)"/>.</summary>
    internal bool UsesDefaultVisibility => PropertyVisibility == MemberVisibility.Public
        && FieldVisibility == MemberVisibility.Public
        && !PreferDeclaredMembers;
}
