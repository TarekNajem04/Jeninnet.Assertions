//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Equivalency;

public partial class SelfReferenceEquivalencyOptions<TExpectation> {
    /// <summary>Gets the custom steps consulted before the built-in comparison, in evaluation order.</summary>
    /// <remarks>Starts as a copy of the effective global template's plan; <see cref="Using"/> appends to it.</remarks>
    public EquivalencyPlan Plan { get; } = new();

    /// <summary>Appends custom steps consulted before the built-in comparison.</summary>
    /// <param name="steps">The steps to append (evaluated in order; the first handling step owns the node).</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="steps"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="steps"/> is empty or contains a null step.</exception>
    public SelfReferenceEquivalencyOptions<TExpectation> Using(params IEquivalencyStep[] steps) {
        Plan.Add(steps);
        return this;
    }

    /// <summary>Adds a member-selection rule: a member is compared only when every rule includes it.</summary>
    /// <param name="rule">The rule to add.</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="rule"/> is <see langword="null"/>.</exception>
    public SelfReferenceEquivalencyOptions<TExpectation> UsingSelectionRule(IMemberSelectionRule rule) {
        if(rule is null) {
            throw new ArgumentNullException(nameof(rule));
        }

        _selectionRules.Add(rule);
        return this;
    }

    /// <summary>Adds a member-matching rule: the first rule returning a name resolves the subject member.</summary>
    /// <param name="rule">The rule to add.</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="rule"/> is <see langword="null"/>.</exception>
    public SelfReferenceEquivalencyOptions<TExpectation> UsingMatchingRule(IMemberMatchingRule rule) {
        if(rule is null) {
            throw new ArgumentNullException(nameof(rule));
        }

        _matchingRules.Add(rule);
        return this;
    }

    /// <summary>Adds an ordering rule: the last registered rule returning a value wins over path rules and the global ordering.</summary>
    /// <param name="rule">The rule to add.</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="rule"/> is <see langword="null"/>.</exception>
    public SelfReferenceEquivalencyOptions<TExpectation> UsingOrderingRule(IOrderingRule rule) {
        if(rule is null) {
            throw new ArgumentNullException(nameof(rule));
        }

        _orderingRuleObjects.Add(rule);
        return this;
    }

    /// <summary>Removes every custom member-selection rule added with <see cref="UsingSelectionRule"/>.</summary>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> WithoutSelectionRules() {
        _selectionRules.Clear();
        return this;
    }

    /// <summary>Removes every custom member-matching rule added with <see cref="UsingMatchingRule"/>.</summary>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> WithoutMatchingRules() {
        _matchingRules.Clear();
        return this;
    }

    /// <summary>Removes every custom ordering rule added with <see cref="UsingOrderingRule"/>.</summary>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> WithoutOrderingRules() {
        _orderingRuleObjects.Clear();
        return this;
    }

    /// <summary>Appends a full dump of both graphs to the failure message when the comparison fails.</summary>
    /// <remarks>
    /// <b>Security:</b> the dump prints every member value, which may expose secrets (passwords, keys,
    /// tokens). Enable only for diagnostics on non-sensitive data, and never persist the output.
    /// </remarks>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> WithFullDump() {
        FullDump = true;
        return this;
    }

    /// <summary>Stops appending full dumps to the failure message (the default).</summary>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> WithoutFullDump() {
        FullDump = false;
        return this;
    }

    /// <summary>Writes every comparison step to <paramref name="writer"/> while the comparison runs.</summary>
    /// <param name="writer">The diagnostics sink.</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="writer"/> is <see langword="null"/>.</exception>
    public SelfReferenceEquivalencyOptions<TExpectation> WithTracing(ITraceWriter writer) {
        if(writer is null) {
            throw new ArgumentNullException(nameof(writer));
        }

        TraceWriter = writer;
        return this;
    }

    /// <summary>Stops tracing comparison steps (the default).</summary>
    /// <returns>This options instance for chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> WithoutTracing() {
        TraceWriter = null;
        return this;
    }
}
