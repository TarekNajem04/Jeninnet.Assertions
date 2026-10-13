//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Continues ordering assertions after <c>BeInAscendingOrder</c>/<c>BeInDescendingOrder</c> with secondary keys.</summary>
/// <typeparam name="T">The element type of the ordered sequence.</typeparam>
/// <param name="value">The ordered items.</param>
/// <param name="expression">The caller expression naming the subject under test.</param>
public class SubsequentOrderingAssertions<T>(IEnumerable<T> value, string? expression = null) : AssertionsBase(expression, "collection") {
    private readonly List<T> _items = value?.ToList() ?? [];
    private readonly List<OrderCriterion<T>> _criteria = [];

    /// <summary>Appends a criterion and validates the whole chain.</summary>
    /// <param name="criterion">The ordering criterion to append.</param>
    /// <param name="because">The reason for the assertion.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    internal AndConstraint<SubsequentOrderingAssertions<T>> Append(OrderCriterion<T> criterion, string? because, object?[]? becauseArgs) {
        _criteria.Add(criterion);
        AssertChain(_items, _criteria, Expression, _criteria.Count > 1, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that items equal on all previous keys are in ascending order of the selected key.</summary>
    /// <typeparam name="TSelector">The selected key type.</typeparam>
    /// <param name="selector">The key selecting expression.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The secondary order is violated.</exception>
    public AndConstraint<SubsequentOrderingAssertions<T>> ThenBeInAscendingOrder<TSelector>(Expression<Func<T, TSelector>> selector, string? because = null, params object?[] becauseArgs) =>
        ThenBeInAscendingOrder(selector, null, because, becauseArgs);

    /// <summary>Asserts that items equal on all previous keys are in ascending order of the selected key using <paramref name="comparer"/>.</summary>
    /// <typeparam name="TSelector">The selected key type.</typeparam>
    /// <param name="selector">The key selecting expression.</param>
    /// <param name="comparer">The comparer defining the key order, or <see langword="null"/> for the default comparer.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The secondary order is violated.</exception>
    public AndConstraint<SubsequentOrderingAssertions<T>> ThenBeInAscendingOrder<TSelector>(Expression<Func<T, TSelector>> selector, IComparer<TSelector>? comparer, string? because = null, params object?[] becauseArgs) {
        if(selector is null) {
            throw new ArgumentNullException(nameof(selector));
        }

        var key = selector.Compile();
        var effective = comparer ?? Comparer<TSelector>.Default;
        return Append(OrderCriterion<T>.Ascending(key, effective), because, becauseArgs);
    }

    /// <summary>Asserts that items equal on all previous keys are in descending order of the selected key.</summary>
    /// <typeparam name="TSelector">The selected key type.</typeparam>
    /// <param name="selector">The key selecting expression.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The secondary order is violated.</exception>
    public AndConstraint<SubsequentOrderingAssertions<T>> ThenBeInDescendingOrder<TSelector>(Expression<Func<T, TSelector>> selector, string? because = null, params object?[] becauseArgs) =>
        ThenBeInDescendingOrder(selector, null, because, becauseArgs);

    /// <summary>Asserts that items equal on all previous keys are in descending order of the selected key using <paramref name="comparer"/>.</summary>
    /// <typeparam name="TSelector">The selected key type.</typeparam>
    /// <param name="selector">The key selecting expression.</param>
    /// <param name="comparer">The comparer defining the key order, or <see langword="null"/> for the default comparer.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The secondary order is violated.</exception>
    public AndConstraint<SubsequentOrderingAssertions<T>> ThenBeInDescendingOrder<TSelector>(Expression<Func<T, TSelector>> selector, IComparer<TSelector>? comparer, string? because = null, params object?[] becauseArgs) {
        if(selector is null) {
            throw new ArgumentNullException(nameof(selector));
        }

        var key = selector.Compile();
        var effective = comparer ?? Comparer<TSelector>.Default;
        return Append(OrderCriterion<T>.Descending(key, effective), because, becauseArgs);
    }

    /// <summary>Validates a multi-key ordering chain over adjacent pairs (first differing key decides).</summary>
    /// <param name="items">The ordered items.</param>
    /// <param name="criteria">The key criteria in priority order.</param>
    /// <param name="expression">The caller expression naming the subject.</param>
    /// <param name="isThen">Whether this is a secondary (<c>Then</c>) validation.</param>
    /// <param name="because">The reason for the assertion.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    private void AssertChain(List<T> items, IReadOnlyList<OrderCriterion<T>> criteria, string expression, bool isThen, string? because, object?[]? becauseArgs) {
        for(var index = 1; index < items.Count; index++) {
            var previous = items[index - 1];
            var current = items[index];
            var violated = false;
            var ascending = true;
            foreach(var criterion in criteria) {
                var comparison = criterion.Compare(previous, current);
                if(comparison == 0) {
                    continue;
                }

                violated = comparison > 0;
                ascending = criterion.IsAscending;
                break;
            }

            if(!violated) {
                continue;
            }

            var key = StringKeys.COLLECTION_ASCENDING_ORDER;
            if(!ascending) {
                key = StringKeys.COLLECTION_DESCENDING_ORDER;
            }

            if(isThen) {
                key = ascending ? StringKeys.COLLECTION_THEN_ASCENDING : StringKeys.COLLECTION_THEN_DESCENDING;
            }

            Fail(Strings.Format(key, expression, Format(current), Format(previous), AssertionScope.FormatInvariant(index)), because, becauseArgs);
            return;
        }
    }

    /// <summary>Determines whether <paramref name="items"/> satisfy every criterion in order.</summary>
    /// <param name="items">The items to check.</param>
    /// <param name="criteria">The key criteria in priority order.</param>
    /// <returns><see langword="true"/> when no adjacent pair violates the chain.</returns>
    internal static bool IsOrdered(IReadOnlyList<T> items, IReadOnlyList<OrderCriterion<T>> criteria) {
        for(var index = 1; index < items.Count; index++) {
            foreach(var criterion in criteria) {
                var comparison = criterion.Compare(items[index - 1], items[index]);
                if(comparison == 0) {
                    continue;
                }

                if(comparison > 0) {
                    return false;
                }

                break;
            }
        }

        return true;
    }

    private static string Format<TValue>(TValue value) => AssertionScope.FormatInvariant(value);
}

/// <summary>A single ordering key with its direction.</summary>
/// <typeparam name="T">The element type.</typeparam>
internal sealed class OrderCriterion<T> {
    /// <summary>Creates an ordering criterion (use the factory methods).</summary>
    /// <param name="compare">The comparison delegate (positive means a violation).</param>
    /// <param name="ascending">Whether the criterion expects ascending order.</param>
    private OrderCriterion(Func<T, T, int> compare, bool ascending) {
        Compare = compare;
        IsAscending = ascending;
    }

    /// <summary>Gets the direction-adjusted comparison (positive means violation).</summary>
    internal Func<T, T, int> Compare { get; }

    /// <summary>Gets whether the criterion expects ascending order (selects the failure message).</summary>
    internal bool IsAscending { get; }

    /// <summary>Creates an ascending key criterion.</summary>
    /// <typeparam name="TSelector">The key type.</typeparam>
    /// <param name="key">The key selector.</param>
    /// <param name="comparer">The key comparer.</param>
    /// <returns>The criterion.</returns>
    internal static OrderCriterion<T> Ascending<TSelector>(Func<T, TSelector> key, IComparer<TSelector> comparer) =>
        new((left, right) => comparer.Compare(key(left), key(right)), true);

    /// <summary>Creates a descending key criterion.</summary>
    /// <typeparam name="TSelector">The key type.</typeparam>
    /// <param name="key">The key selector.</param>
    /// <param name="comparer">The key comparer.</param>
    /// <returns>The criterion.</returns>
    internal static OrderCriterion<T> Descending<TSelector>(Func<T, TSelector> key, IComparer<TSelector> comparer) =>
        new((left, right) => -comparer.Compare(key(left), key(right)), false);

    /// <summary>Creates a direct item criterion.</summary>
    /// <param name="comparer">The item comparer.</param>
    /// <param name="ascending">Whether ascending order is expected.</param>
    /// <returns>The criterion.</returns>
    internal static OrderCriterion<T> Direct(IComparer<T> comparer, bool ascending) =>
        ascending
            ? new(comparer.Compare, true)
            : new((left, right) => -comparer.Compare(left, right), false);

    /// <summary>Creates a criterion from a raw comparison function.</summary>
    /// <param name="comparison">The comparison function.</param>
    /// <param name="ascending">Whether ascending order is expected.</param>
    /// <returns>The criterion.</returns>
    internal static OrderCriterion<T> FromComparison(Func<T, T, int> comparison, bool ascending) =>
        ascending
            ? new(comparison, true)
            : new((left, right) => -comparison(left, right), false);
}
