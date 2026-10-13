//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class GenericDictionaryAssertions<TKey, TValue> {
    /// <summary>Asserts that the pairs are in ascending order using the default comparer.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or not in ascending order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<KeyValuePair<TKey, TValue>>> BeInAscendingOrder(string? because = null, params object?[] becauseArgs) =>
        BeInAscendingOrder(Comparer<KeyValuePair<TKey, TValue>>.Default, because, becauseArgs);

    /// <summary>Asserts that the pairs are in ascending order using <paramref name="comparer"/>.</summary>
    /// <param name="comparer">The comparer defining the order.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or not in ascending order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<KeyValuePair<TKey, TValue>>> BeInAscendingOrder(IComparer<KeyValuePair<TKey, TValue>> comparer, string? because = null, params object?[] becauseArgs) =>
        OrderBy(OrderCriterion<KeyValuePair<TKey, TValue>>.Direct(RequireComparer(comparer), true), StringKeys.DICT_ASCENDING_NULL, because, becauseArgs);

    /// <summary>Asserts that the pairs are in ascending order of <paramref name="comparison"/>.</summary>
    /// <param name="comparison">The comparison function (negative when the first pair precedes the second).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="comparison"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or not in ascending order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<KeyValuePair<TKey, TValue>>> BeInAscendingOrder(Func<KeyValuePair<TKey, TValue>, KeyValuePair<TKey, TValue>, int> comparison, string? because = null, params object?[] becauseArgs) =>
        OrderBy(OrderCriterion<KeyValuePair<TKey, TValue>>.FromComparison(RequireComparison(comparison), true), StringKeys.DICT_ASCENDING_NULL, because, becauseArgs);

    /// <summary>Asserts that the pairs are in ascending order of the key selected by <paramref name="selector"/>.</summary>
    /// <typeparam name="TSelector">The selected key type.</typeparam>
    /// <param name="selector">The key selecting expression.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or not in ascending key order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<KeyValuePair<TKey, TValue>>> BeInAscendingOrder<TSelector>(Expression<Func<KeyValuePair<TKey, TValue>, TSelector>> selector, string? because = null, params object?[] becauseArgs) =>
        BeInAscendingOrder(selector, null, because, becauseArgs);

    /// <summary>Asserts that the pairs are in ascending order of the key selected by <paramref name="selector"/> using <paramref name="comparer"/>.</summary>
    /// <typeparam name="TSelector">The selected key type.</typeparam>
    /// <param name="selector">The key selecting expression.</param>
    /// <param name="comparer">The comparer defining the key order, or <see langword="null"/> for the default comparer.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or not in ascending key order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<KeyValuePair<TKey, TValue>>> BeInAscendingOrder<TSelector>(Expression<Func<KeyValuePair<TKey, TValue>, TSelector>> selector, IComparer<TSelector>? comparer, string? because = null, params object?[] becauseArgs) {
        if(selector is null) {
            throw new ArgumentNullException(nameof(selector));
        }

        var key = selector.Compile();
        return OrderBy(OrderCriterion<KeyValuePair<TKey, TValue>>.Ascending(key, comparer ?? Comparer<TSelector>.Default), StringKeys.DICT_ASCENDING_NULL, because, becauseArgs);
    }

    /// <summary>Asserts that the pairs are in descending order using the default comparer.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or not in descending order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<KeyValuePair<TKey, TValue>>> BeInDescendingOrder(string? because = null, params object?[] becauseArgs) =>
        BeInDescendingOrder(Comparer<KeyValuePair<TKey, TValue>>.Default, because, becauseArgs);

    /// <summary>Asserts that the pairs are in descending order using <paramref name="comparer"/>.</summary>
    /// <param name="comparer">The comparer defining the order.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or not in descending order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<KeyValuePair<TKey, TValue>>> BeInDescendingOrder(IComparer<KeyValuePair<TKey, TValue>> comparer, string? because = null, params object?[] becauseArgs) =>
        OrderBy(OrderCriterion<KeyValuePair<TKey, TValue>>.Direct(RequireComparer(comparer), false), StringKeys.DICT_DESCENDING_NULL, because, becauseArgs);

    /// <summary>Asserts that the pairs are in descending order of <paramref name="comparison"/>.</summary>
    /// <param name="comparison">The comparison function (negative when the first pair precedes the second).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="comparison"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or not in descending order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<KeyValuePair<TKey, TValue>>> BeInDescendingOrder(Func<KeyValuePair<TKey, TValue>, KeyValuePair<TKey, TValue>, int> comparison, string? because = null, params object?[] becauseArgs) =>
        OrderBy(OrderCriterion<KeyValuePair<TKey, TValue>>.FromComparison(RequireComparison(comparison), false), StringKeys.DICT_DESCENDING_NULL, because, becauseArgs);

    /// <summary>Asserts that the pairs are in descending order of the key selected by <paramref name="selector"/>.</summary>
    /// <typeparam name="TSelector">The selected key type.</typeparam>
    /// <param name="selector">The key selecting expression.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or not in descending key order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<KeyValuePair<TKey, TValue>>> BeInDescendingOrder<TSelector>(Expression<Func<KeyValuePair<TKey, TValue>, TSelector>> selector, string? because = null, params object?[] becauseArgs) =>
        BeInDescendingOrder(selector, null, because, becauseArgs);

    /// <summary>Asserts that the pairs are in descending order of the key selected by <paramref name="selector"/> using <paramref name="comparer"/>.</summary>
    /// <typeparam name="TSelector">The selected key type.</typeparam>
    /// <param name="selector">The key selecting expression.</param>
    /// <param name="comparer">The comparer defining the key order, or <see langword="null"/> for the default comparer.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or not in descending key order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<KeyValuePair<TKey, TValue>>> BeInDescendingOrder<TSelector>(Expression<Func<KeyValuePair<TKey, TValue>, TSelector>> selector, IComparer<TSelector>? comparer, string? because = null, params object?[] becauseArgs) {
        if(selector is null) {
            throw new ArgumentNullException(nameof(selector));
        }

        var key = selector.Compile();
        return OrderBy(OrderCriterion<KeyValuePair<TKey, TValue>>.Descending(key, comparer ?? Comparer<TSelector>.Default), StringKeys.DICT_DESCENDING_NULL, because, becauseArgs);
    }

    /// <summary>Asserts that the pairs are not in ascending order of <paramref name="comparison"/>.</summary>
    /// <param name="comparison">The comparison function (negative when the first pair precedes the second).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="comparison"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The pairs are in ascending order.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> NotBeInAscendingOrder(Func<KeyValuePair<TKey, TValue>, KeyValuePair<TKey, TValue>, int> comparison, string? because = null, params object?[] becauseArgs) =>
        NotOrderedBy(OrderCriterion<KeyValuePair<TKey, TValue>>.FromComparison(RequireComparison(comparison), true), true, because, becauseArgs);

    /// <summary>Asserts that the pairs are not in ascending order of the key selected by <paramref name="selector"/>.</summary>
    /// <typeparam name="TSelector">The selected key type.</typeparam>
    /// <param name="selector">The key selecting expression.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The pairs are in ascending key order.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> NotBeInAscendingOrder<TSelector>(Expression<Func<KeyValuePair<TKey, TValue>, TSelector>> selector, string? because = null, params object?[] becauseArgs) =>
        NotBeInAscendingOrder(selector, null, because, becauseArgs);

    /// <summary>Asserts that the pairs are not in ascending order of the key selected by <paramref name="selector"/> using <paramref name="comparer"/>.</summary>
    /// <typeparam name="TSelector">The selected key type.</typeparam>
    /// <param name="selector">The key selecting expression.</param>
    /// <param name="comparer">The comparer defining the key order, or <see langword="null"/> for the default comparer.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The pairs are in ascending key order.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> NotBeInAscendingOrder<TSelector>(Expression<Func<KeyValuePair<TKey, TValue>, TSelector>> selector, IComparer<TSelector>? comparer, string? because = null, params object?[] becauseArgs) {
        if(selector is null) {
            throw new ArgumentNullException(nameof(selector));
        }

        var key = selector.Compile();
        return NotOrderedBy(OrderCriterion<KeyValuePair<TKey, TValue>>.Ascending(key, comparer ?? Comparer<TSelector>.Default), true, because, becauseArgs);
    }

    /// <summary>Asserts that the pairs are not in descending order of <paramref name="comparison"/>.</summary>
    /// <param name="comparison">The comparison function (negative when the first pair precedes the second).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="comparison"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The pairs are in descending order.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> NotBeInDescendingOrder(Func<KeyValuePair<TKey, TValue>, KeyValuePair<TKey, TValue>, int> comparison, string? because = null, params object?[] becauseArgs) =>
        NotOrderedBy(OrderCriterion<KeyValuePair<TKey, TValue>>.FromComparison(RequireComparison(comparison), false), false, because, becauseArgs);

    /// <summary>Asserts that the pairs are not in descending order of the key selected by <paramref name="selector"/>.</summary>
    /// <typeparam name="TSelector">The selected key type.</typeparam>
    /// <param name="selector">The key selecting expression.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The pairs are in descending key order.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> NotBeInDescendingOrder<TSelector>(Expression<Func<KeyValuePair<TKey, TValue>, TSelector>> selector, string? because = null, params object?[] becauseArgs) =>
        NotBeInDescendingOrder(selector, null, because, becauseArgs);

    /// <summary>Asserts that the pairs are not in descending order of the key selected by <paramref name="selector"/> using <paramref name="comparer"/>.</summary>
    /// <typeparam name="TSelector">The selected key type.</typeparam>
    /// <param name="selector">The key selecting expression.</param>
    /// <param name="comparer">The comparer defining the key order, or <see langword="null"/> for the default comparer.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The pairs are in descending key order.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> NotBeInDescendingOrder<TSelector>(Expression<Func<KeyValuePair<TKey, TValue>, TSelector>> selector, IComparer<TSelector>? comparer, string? because = null, params object?[] becauseArgs) {
        if(selector is null) {
            throw new ArgumentNullException(nameof(selector));
        }

        var key = selector.Compile();
        return NotOrderedBy(OrderCriterion<KeyValuePair<TKey, TValue>>.Descending(key, comparer ?? Comparer<TSelector>.Default), false, because, becauseArgs);
    }

    /// <summary>Validates a single ordering criterion over the pairs and continues the ordering chain.</summary>
    /// <param name="criterion">The ordering criterion.</param>
    /// <param name="nullKey">The message key for a <see langword="null"/> subject.</param>
    /// <param name="because">The reason for the assertion.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain.</returns>
    private AndConstraint<SubsequentOrderingAssertions<KeyValuePair<TKey, TValue>>> OrderBy(OrderCriterion<KeyValuePair<TKey, TValue>> criterion, string nullKey, string? because, object?[]? becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(nullKey, Expression), because, becauseArgs);
            return new(new SubsequentOrderingAssertions<KeyValuePair<TKey, TValue>>([], Expression));
        }

        var items = _value.ToList();
        return new SubsequentOrderingAssertions<KeyValuePair<TKey, TValue>>(items, Expression).Append(criterion, because, becauseArgs);
    }

    /// <summary>Asserts that the pairs violate a single ordering criterion.</summary>
    /// <param name="criterion">The ordering criterion.</param>
    /// <param name="ascending">Whether the criterion expects ascending order (selects the failure message).</param>
    /// <param name="because">The reason for the assertion.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    private AndConstraint<GenericDictionaryAssertions<TKey, TValue>> NotOrderedBy(OrderCriterion<KeyValuePair<TKey, TValue>> criterion, bool ascending, string? because, object?[]? becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(ascending ? StringKeys.COLLECTION_NOT_ASCENDING_NULL : StringKeys.COLLECTION_NOT_DESCENDING_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        var items = _value.ToList();
        if(items.Count > 1 && SubsequentOrderingAssertions<KeyValuePair<TKey, TValue>>.IsOrdered(items, [criterion])) {
            Fail(Strings.Format(ascending ? StringKeys.COLLECTION_NOT_ASCENDING_ORDER : StringKeys.COLLECTION_NOT_DESCENDING_ORDER, Expression, Format(items[0]), Format(items[items.Count - 1]), Format(0)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Guards a comparer argument.</summary>
    /// <typeparam name="TCompared">The compared type.</typeparam>
    /// <param name="comparer">The comparer to validate.</param>
    /// <returns><paramref name="comparer"/> when not <see langword="null"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <see langword="null"/>.</exception>
    private static IComparer<TCompared> RequireComparer<TCompared>(IComparer<TCompared>? comparer) =>
        comparer ?? throw new ArgumentNullException(nameof(comparer));

    /// <summary>Guards a comparison argument.</summary>
    /// <typeparam name="TCompared">The compared type.</typeparam>
    /// <param name="comparison">The comparison function to validate.</param>
    /// <returns><paramref name="comparison"/> when not <see langword="null"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="comparison"/> is <see langword="null"/>.</exception>
    private static Func<TCompared, TCompared, int> RequireComparison<TCompared>(Func<TCompared, TCompared, int>? comparison) =>
        comparison ?? throw new ArgumentNullException(nameof(comparison));
}
