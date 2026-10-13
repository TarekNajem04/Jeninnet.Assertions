//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Equivalency;

public partial class SelfReferenceEquivalencyOptions<TExpectation> {
    /// <summary>Silently cuts off cyclic references instead of reporting them as failures.</summary>
    /// <remarks>Mutually exclusive with <see cref="AllowingInfiniteRecursion"/>; the last call wins.</remarks>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> IgnoringCyclicReferences() {
        IgnoreCyclicReferences = true;
        AllowInfiniteRecursion = false;
        return this;
    }

    /// <summary>Compares only the graph surface: nested values must equal with <see cref="object.Equals(object?, object?)"/>.</summary>
    /// <remarks>Nested strict typing and conversion do not apply below the surface. <see cref="IncludingNestedObjects"/> restores deep comparison.</remarks>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> WithoutRecursing() {
        ShallowComparison = true;
        return this;
    }

    /// <summary>Restores deep graph comparison (the default).</summary>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> IncludingNestedObjects() {
        ShallowComparison = false;
        return this;
    }

    /// <summary>Requires the collection selected by <paramref name="selector"/> to match in order.</summary>
    /// <typeparam name="TMember">The member type selected by <paramref name="selector"/>.</typeparam>
    /// <param name="selector">A member access on the expectation.</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="selector"/> is not a member access on the expectation.</exception>
    public SelfReferenceEquivalencyOptions<TExpectation> WithStrictOrderingFor<TMember>(Expression<Func<TExpectation, TMember>> selector) {
        if(selector is null) {
            throw new ArgumentNullException(nameof(selector));
        }

        _orderingRules.Add((MemberPath.Parse(selector), true));
        return this;
    }

    /// <summary>Compares the collection selected by <paramref name="selector"/> as an unordered multiset.</summary>
    /// <typeparam name="TMember">The member type selected by <paramref name="selector"/>.</typeparam>
    /// <param name="selector">A member access on the expectation.</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="selector"/> is not a member access on the expectation.</exception>
    public SelfReferenceEquivalencyOptions<TExpectation> WithoutStrictOrderingFor<TMember>(Expression<Func<TExpectation, TMember>> selector) {
        if(selector is null) {
            throw new ArgumentNullException(nameof(selector));
        }

        _orderingRules.Add((MemberPath.Parse(selector), false));
        return this;
    }

    /// <summary>Compares every <see cref="DateTime"/> and <see cref="DateTimeOffset"/> value in the graph as an instant within <paramref name="tolerance"/>.</summary>
    /// <remarks>
    /// The tolerance is absolute and in-memory only: a delta at or below it counts as equal, a larger delta
    /// fails with the usual difference plus the exceeded tolerance. Other types (including integers) stay
    /// exact, and a scoped <see cref="WithDateTimeToleranceFor{TMember}"/> on the same path wins.
    /// </remarks>
    /// <param name="tolerance">The maximum accepted absolute difference between two instants (must not be negative).</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="tolerance"/> is negative.</exception>
    public SelfReferenceEquivalencyOptions<TExpectation> WithDateTimeTolerance(TimeSpan tolerance) {
        if(tolerance < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(tolerance), NEGATIVE_TOLERANCE_MESSAGE);
        }

        DateTimeTolerance = tolerance;
        return this;
    }

    /// <summary>Compares the date/time value selected by <paramref name="selector"/> within <paramref name="tolerance"/>, overriding any graph-wide tolerance on that path.</summary>
    /// <typeparam name="TMember">The member type selected by <paramref name="selector"/>.</typeparam>
    /// <param name="tolerance">The maximum accepted absolute difference between two instants (must not be negative).</param>
    /// <param name="selector">A member access on the expectation.</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="selector"/> is not a member access on the expectation.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="tolerance"/> is negative.</exception>
    public SelfReferenceEquivalencyOptions<TExpectation> WithDateTimeToleranceFor<TMember>(TimeSpan tolerance, Expression<Func<TExpectation, TMember>> selector) {
        if(selector is null) {
            throw new ArgumentNullException(nameof(selector));
        }

        if(tolerance < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(tolerance), NEGATIVE_TOLERANCE_MESSAGE);
        }

        _dateTimeToleranceRules.Add((MemberPath.Parse(selector), tolerance));
        return this;
    }

    /// <summary>Compares every floating-point and <see cref="decimal"/> value in the graph within the absolute <paramref name="epsilon"/>.</summary>
    /// <remarks>
    /// The epsilon is absolute and in-memory only: a delta at or below it counts as equal, a larger delta
    /// fails with the usual difference plus the exceeded tolerance. Other numerics (including integers) stay
    /// exact, and a scoped <see cref="WithNumericToleranceFor{TMember}"/> on the same path wins.
    /// </remarks>
    /// <param name="epsilon">The maximum accepted absolute difference (must not be negative).</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="epsilon"/> is negative.</exception>
    public SelfReferenceEquivalencyOptions<TExpectation> WithNumericTolerance(double epsilon) {
        if(epsilon < 0) {
            throw new ArgumentOutOfRangeException(nameof(epsilon), NEGATIVE_TOLERANCE_MESSAGE);
        }

        NumericTolerance = epsilon;
        return this;
    }

    /// <summary>Compares the numeric value selected by <paramref name="selector"/> within <paramref name="epsilon"/>, overriding any graph-wide tolerance on that path.</summary>
    /// <typeparam name="TMember">The member type selected by <paramref name="selector"/>.</typeparam>
    /// <param name="epsilon">The maximum accepted absolute difference (must not be negative).</param>
    /// <param name="selector">A member access on the expectation.</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="selector"/> is not a member access on the expectation.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="epsilon"/> is negative.</exception>
    public SelfReferenceEquivalencyOptions<TExpectation> WithNumericToleranceFor<TMember>(double epsilon, Expression<Func<TExpectation, TMember>> selector) {
        if(selector is null) {
            throw new ArgumentNullException(nameof(selector));
        }

        if(epsilon < 0) {
            throw new ArgumentOutOfRangeException(nameof(epsilon), NEGATIVE_TOLERANCE_MESSAGE);
        }

        _numericToleranceRules.Add((MemberPath.Parse(selector), epsilon));
        return this;
    }

    /// <summary>Matches string dictionary keys ignoring case (for JSON-shaped graphs).</summary>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> IgnoringJsonPropertyCasing() {
        IgnoreJsonPropertyCasing = true;
        return this;
    }

    /// <summary>Compares graph strings ignoring case (ordinal).</summary>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> IgnoringCase() {
        GraphIgnoreCase = true;
        return this;
    }

    /// <summary>Ignores leading whitespace when comparing graph strings.</summary>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> IgnoringLeadingWhitespace() {
        GraphTrimStart = true;
        return this;
    }

    /// <summary>Ignores trailing whitespace when comparing graph strings.</summary>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> IgnoringTrailingWhitespace() {
        GraphTrimEnd = true;
        return this;
    }

    /// <summary>Treats <c>\r\n</c> and <c>\r</c> as <c>\n</c> when comparing graph strings.</summary>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> IgnoringNewlineStyle() {
        GraphNormalizeNewlines = true;
        return this;
    }

    /// <summary>Reports the full text of both strings when they differ, instead of a bounded fragment.</summary>
    /// <remarks>
    /// Long strings (> 200 characters) are truncated with an ellipsis marker by default; this option
    /// keeps the complete values in the failure message. Short strings are unaffected either way.
    /// </remarks>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> IncludingFullStringsInDifference() {
        IncludeFullStringsInDifference = true;
        return this;
    }
}
