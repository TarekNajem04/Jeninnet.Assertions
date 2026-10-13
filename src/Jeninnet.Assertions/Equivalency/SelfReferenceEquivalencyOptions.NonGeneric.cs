//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Equivalency;

/// <summary>
/// The ambient equivalency template mutated by <see cref="EquivalencyDefaults.Modify"/> and cloned by
/// <see cref="EquivalencyDefaults.CloneDefaults"/>.
/// </summary>
/// <remarks>
/// This is the non-generic twin of <see cref="SelfReferenceEquivalencyOptions{TExpectation}"/>: plain mutable
/// properties instead of fluent expression-based configuration (which needs a compile-time expectation type).
/// Every generic options instance starts as a copy of the effective template, so per-assertion configuration
/// always wins over this ambient template, which in turn wins over the process-wide defaults.
/// </remarks>
public sealed class SelfReferenceEquivalencyOptions {
    /// <summary>Gets or sets whether collections must match in the same order.</summary>
    public bool StrictOrdering { get; set; }

    /// <summary>Gets or sets whether every compared pair must share the exact runtime type.</summary>
    public bool StrictTyping { get; set; }

    /// <summary>Gets or sets whether differing runtime types may convert toward each other.</summary>
    public bool AutoConversion { get; set; }

    /// <summary>Gets or sets which member set the root expectation contributes.</summary>
    public MemberTypePreference MemberTypePreference { get; set; }

    /// <summary>Gets or sets how enum values are compared.</summary>
    public EnumComparison EnumComparison { get; set; }

    /// <summary>Gets or sets whether records compare with <see cref="object.Equals(object?, object?)"/> instead of memberwise.</summary>
    public bool RecordsByValue { get; set; }

    /// <summary>Gets or sets whether a <see langword="null"/> string equals an empty string.</summary>
    public bool NullStringsAsEmpty { get; set; }

    /// <summary>Gets or sets whether a <see langword="null"/> collection equals an empty collection.</summary>
    public bool NullCollectionsAsEmpty { get; set; }

    /// <summary>Gets or sets whether cyclic references are silently cut off (the <c>AllowingInfiniteRecursion</c> alternative).</summary>
    public bool AllowInfiniteRecursion { get; set; }

    /// <summary>Gets or sets whether cyclic references are silently cut off (the <c>IgnoringCyclicReferences</c> alternative).</summary>
    public bool IgnoreCyclicReferences { get; set; }

    /// <summary>Gets or sets whether nested values compare with <see cref="object.Equals(object?, object?)"/> instead of recursing.</summary>
    public bool ShallowComparison { get; set; }

    /// <summary>Gets or sets whether string dictionary keys match ignoring case.</summary>
    public bool IgnoreJsonPropertyCasing { get; set; }

    /// <summary>Gets or sets whether graph strings compare ignoring case.</summary>
    public bool GraphIgnoreCase { get; set; }

    /// <summary>Gets or sets whether graph strings ignore leading whitespace.</summary>
    public bool GraphTrimStart { get; set; }

    /// <summary>Gets or sets whether graph strings ignore trailing whitespace.</summary>
    public bool GraphTrimEnd { get; set; }

    /// <summary>Gets or sets whether graph strings normalize newline styles.</summary>
    public bool GraphNormalizeNewlines { get; set; }

    /// <summary>Gets or sets whether string differences report the full text instead of a bounded fragment.</summary>
    public bool IncludeFullStringsInDifference { get; set; }

    /// <summary>Gets the custom steps consulted before the built-in comparison.</summary>
    public EquivalencyPlan Plan { get; } = new();

    /// <summary>Copies this template into a detached instance.</summary>
    /// <returns>The detached copy.</returns>
    public SelfReferenceEquivalencyOptions Copy() {
        var copy = new SelfReferenceEquivalencyOptions {
            StrictOrdering = StrictOrdering,
            StrictTyping = StrictTyping,
            AutoConversion = AutoConversion,
            MemberTypePreference = MemberTypePreference,
            EnumComparison = EnumComparison,
            RecordsByValue = RecordsByValue,
            NullStringsAsEmpty = NullStringsAsEmpty,
            NullCollectionsAsEmpty = NullCollectionsAsEmpty,
            AllowInfiniteRecursion = AllowInfiniteRecursion,
            IgnoreCyclicReferences = IgnoreCyclicReferences,
            ShallowComparison = ShallowComparison,
            IgnoreJsonPropertyCasing = IgnoreJsonPropertyCasing,
            GraphIgnoreCase = GraphIgnoreCase,
            GraphTrimStart = GraphTrimStart,
            GraphTrimEnd = GraphTrimEnd,
            GraphNormalizeNewlines = GraphNormalizeNewlines,
            IncludeFullStringsInDifference = IncludeFullStringsInDifference,
        };
        copy.Plan.CopyFrom(Plan);
        return copy;
    }
}
