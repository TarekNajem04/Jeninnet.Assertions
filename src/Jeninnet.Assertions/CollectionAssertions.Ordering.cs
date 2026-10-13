//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class CollectionAssertions<T> {
    /// <summary>Asserts that the collection contains the items in <paramref name="expected"/> in order (not necessarily consecutively).</summary>
    /// <param name="expected">The expected items in order.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or the sequence is not found in order.</exception>
    public AndConstraint<CollectionAssertions<T>> ContainInOrder(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var expectedList = expected.ToList();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_CONTAIN_IN_ORDER_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        var actualList = _value.ToList();
        var index = 0;
        foreach(var item in expectedList) {
            var found = actualList.FindIndex(index, x => EqualityComparer<T>.Default.Equals(x, item));
            if(found < 0) {
                Fail(Strings.Format(StringKeys.COLLECTION_CONTAIN_IN_ORDER_MISSING, Expression, Format(item)), because, becauseArgs);
                return new(this);
            }

            index = found + 1;
        }

        return new(this);
    }

    /// <summary>Asserts that the collection contains the items in <paramref name="expected"/> in order (not necessarily consecutively).</summary>
    /// <param name="expected">The expected items in order.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or the sequence is not found in order.</exception>
    public AndConstraint<CollectionAssertions<T>> ContainInOrder(params T[] expected) => ContainInOrder((IEnumerable<T>)expected);

    /// <summary>Asserts that the collection is in ascending order using the default comparer.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or not in ascending order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<T>> BeInAscendingOrder(string? because = null, params object?[] becauseArgs) =>
        BeInAscendingOrder(Comparer<T>.Default, because, becauseArgs);

    /// <summary>Asserts that the collection is in ascending order using <paramref name="comparer"/>.</summary>
    /// <param name="comparer">The comparer defining the order.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or not in ascending order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<T>> BeInAscendingOrder(IComparer<T> comparer, string? because = null, params object?[] becauseArgs) {
        if(comparer is null) {
            throw new ArgumentNullException(nameof(comparer));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_ASCENDING_NULL, Expression), because, becauseArgs);
            return new(new SubsequentOrderingAssertions<T>([], Expression));
        }

        var items = _value.ToList();
        return new SubsequentOrderingAssertions<T>(items, Expression).Append(OrderCriterion<T>.Direct(comparer, true), because, becauseArgs);
    }

    /// <summary>Asserts that the collection is in ascending order of <paramref name="comparison"/>.</summary>
    /// <param name="comparison">The comparison function (negative when the first item precedes the second).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="comparison"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or not in ascending order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<T>> BeInAscendingOrder(Func<T, T, int> comparison, string? because = null, params object?[] becauseArgs) {
        if(comparison is null) {
            throw new ArgumentNullException(nameof(comparison));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_ASCENDING_NULL, Expression), because, becauseArgs);
            return new(new SubsequentOrderingAssertions<T>([], Expression));
        }

        var items = _value.ToList();
        return new SubsequentOrderingAssertions<T>(items, Expression).Append(OrderCriterion<T>.FromComparison(comparison, true), because, becauseArgs);
    }

    /// <summary>Asserts that the collection is in ascending order of the key selected by <paramref name="selector"/>.</summary>
    /// <typeparam name="TSelector">The selected key type.</typeparam>
    /// <param name="selector">The key selecting expression.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or not in ascending key order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<T>> BeInAscendingOrder<TSelector>(Expression<Func<T, TSelector>> selector, string? because = null, params object?[] becauseArgs) =>
        BeInAscendingOrder(selector, null, because, becauseArgs);

    /// <summary>Asserts that the collection is in ascending order of the key selected by <paramref name="selector"/> using <paramref name="comparer"/>.</summary>
    /// <typeparam name="TSelector">The selected key type.</typeparam>
    /// <param name="selector">The key selecting expression.</param>
    /// <param name="comparer">The comparer defining the key order, or <see langword="null"/> for the default comparer.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or not in ascending key order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<T>> BeInAscendingOrder<TSelector>(Expression<Func<T, TSelector>> selector, IComparer<TSelector>? comparer, string? because = null, params object?[] becauseArgs) {
        if(selector is null) {
            throw new ArgumentNullException(nameof(selector));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_ASCENDING_NULL, Expression), because, becauseArgs);
            return new(new SubsequentOrderingAssertions<T>([], Expression));
        }

        var items = _value.ToList();
        var key = selector.Compile();
        return new SubsequentOrderingAssertions<T>(items, Expression).Append(OrderCriterion<T>.Ascending(key, comparer ?? Comparer<TSelector>.Default), because, becauseArgs);
    }

    /// <summary>Asserts that the collection is in descending order using the default comparer.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or not in descending order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<T>> BeInDescendingOrder(string? because = null, params object?[] becauseArgs) =>
        BeInDescendingOrder(Comparer<T>.Default, because, becauseArgs);

    /// <summary>Asserts that the collection is in descending order using <paramref name="comparer"/>.</summary>
    /// <param name="comparer">The comparer defining the order.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or not in descending order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<T>> BeInDescendingOrder(IComparer<T> comparer, string? because = null, params object?[] becauseArgs) {
        if(comparer is null) {
            throw new ArgumentNullException(nameof(comparer));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_DESCENDING_NULL, Expression), because, becauseArgs);
            return new(new SubsequentOrderingAssertions<T>([], Expression));
        }

        var items = _value.ToList();
        return new SubsequentOrderingAssertions<T>(items, Expression).Append(OrderCriterion<T>.Direct(comparer, false), because, becauseArgs);
    }

    /// <summary>Asserts that the collection is in descending order of <paramref name="comparison"/>.</summary>
    /// <param name="comparison">The comparison function (negative when the first item precedes the second).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="comparison"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or not in descending order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<T>> BeInDescendingOrder(Func<T, T, int> comparison, string? because = null, params object?[] becauseArgs) {
        if(comparison is null) {
            throw new ArgumentNullException(nameof(comparison));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_DESCENDING_NULL, Expression), because, becauseArgs);
            return new(new SubsequentOrderingAssertions<T>([], Expression));
        }

        var items = _value.ToList();
        return new SubsequentOrderingAssertions<T>(items, Expression).Append(OrderCriterion<T>.FromComparison(comparison, false), because, becauseArgs);
    }

    /// <summary>Asserts that the collection is in descending order of the key selected by <paramref name="selector"/>.</summary>
    /// <typeparam name="TSelector">The selected key type.</typeparam>
    /// <param name="selector">The key selecting expression.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or not in descending key order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<T>> BeInDescendingOrder<TSelector>(Expression<Func<T, TSelector>> selector, string? because = null, params object?[] becauseArgs) =>
        BeInDescendingOrder(selector, null, because, becauseArgs);

    /// <summary>Asserts that the collection is in descending order of the key selected by <paramref name="selector"/> using <paramref name="comparer"/>.</summary>
    /// <typeparam name="TSelector">The selected key type.</typeparam>
    /// <param name="selector">The key selecting expression.</param>
    /// <param name="comparer">The comparer defining the key order, or <see langword="null"/> for the default comparer.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> over the ordering chain for <c>ThenBeIn*</c> continuations.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or not in descending key order.</exception>
    public AndConstraint<SubsequentOrderingAssertions<T>> BeInDescendingOrder<TSelector>(Expression<Func<T, TSelector>> selector, IComparer<TSelector>? comparer, string? because = null, params object?[] becauseArgs) {
        if(selector is null) {
            throw new ArgumentNullException(nameof(selector));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_DESCENDING_NULL, Expression), because, becauseArgs);
            return new(new SubsequentOrderingAssertions<T>([], Expression));
        }

        var items = _value.ToList();
        var key = selector.Compile();
        return new SubsequentOrderingAssertions<T>(items, Expression).Append(OrderCriterion<T>.Descending(key, comparer ?? Comparer<TSelector>.Default), because, becauseArgs);
    }

    /// <summary>Asserts that the collection contains the items in <paramref name="expected"/> as one consecutive run.</summary>
    /// <param name="expected">The expected consecutive sequence.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or the consecutive sequence is not found.</exception>
    public AndConstraint<CollectionAssertions<T>> ContainInConsecutiveOrder(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var expectedList = expected.ToList();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_CONSECUTIVE_ORDER_NULL, Expression, JoinItems(expectedList)), because, becauseArgs);
            return new(this);
        }

        var actualList = _value.ToList();
        var found = false;
        for(var start = 0; start + expectedList.Count <= actualList.Count && !found; start++) {
            found = true;
            for(var offset = 0; offset < expectedList.Count; offset++) {
                if(!EqualityComparer<T>.Default.Equals(actualList[start + offset], expectedList[offset])) {
                    found = false;
                    break;
                }
            }
        }

        if(!found) {
            Fail(Strings.Format(StringKeys.COLLECTION_CONSECUTIVE_ORDER_MISSING, Expression, JoinItems(expectedList)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the collection contains the items in <paramref name="expected"/> as one consecutive run.</summary>
    /// <param name="expected">The expected consecutive sequence.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or the consecutive sequence is not found.</exception>
    public AndConstraint<CollectionAssertions<T>> ContainInConsecutiveOrder(params T[] expected) => ContainInConsecutiveOrder((IEnumerable<T>)expected);

    /// <summary>Asserts that the collection does not contain the specified items in order.</summary>
    /// <param name="unexpected">The items that must not be found in order.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="unexpected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection contains the items in order.</exception>
    public AndConstraint<CollectionAssertions<T>> NotContainInOrder(IEnumerable<T> unexpected, string? because = null, params object?[] becauseArgs) {
        if(unexpected is null) {
            throw new ArgumentNullException(nameof(unexpected));
        }

        var unexpectedList = unexpected.ToList();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_CONTAIN_IN_ORDER_NULL, Expression, JoinItems(unexpectedList)), because, becauseArgs);
            return new(this);
        }

        var actualList = _value.ToList();
        var index = 0;
        var allFound = true;
        foreach(var item in unexpectedList) {
            var found = actualList.FindIndex(index, x => EqualityComparer<T>.Default.Equals(x, item));
            if(found < 0) {
                allFound = false;
                break;
            }

            index = found + 1;
        }

        if(allFound) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_CONTAIN_IN_ORDER_MISSING, Expression, JoinItems(unexpectedList)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the collection does not contain the specified items in consecutive order.</summary>
    /// <param name="unexpected">The items that must not be found consecutively.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="unexpected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection contains the items consecutively.</exception>
    public AndConstraint<CollectionAssertions<T>> NotContainInConsecutiveOrder(IEnumerable<T> unexpected, string? because = null, params object?[] becauseArgs) {
        if(unexpected is null) {
            throw new ArgumentNullException(nameof(unexpected));
        }

        var unexpectedList = unexpected.ToList();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_CONSECUTIVE_ORDER_NULL, Expression, JoinItems(unexpectedList)), because, becauseArgs);
            return new(this);
        }

        var actualList = _value.ToList();
        var found = false;
        for(var start = 0; start + unexpectedList.Count <= actualList.Count && !found; start++) {
            found = true;
            for(var offset = 0; offset < unexpectedList.Count; offset++) {
                if(!EqualityComparer<T>.Default.Equals(actualList[start + offset], unexpectedList[offset])) {
                    found = false;
                    break;
                }
            }
        }

        if(found) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_CONSECUTIVE_ORDER_MISSING, Expression, JoinItems(unexpectedList)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the collection is not in ascending order using the default comparer.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection is in ascending order.</exception>
    public AndConstraint<CollectionAssertions<T>> NotBeInAscendingOrder(string? because = null, params object?[] becauseArgs) =>
        NotBeInAscendingOrder(Comparer<T>.Default, because, becauseArgs);

    /// <summary>Asserts that the collection is not in ascending order.</summary>
    /// <param name="comparer">The comparer to use.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is in ascending order.</exception>
    public AndConstraint<CollectionAssertions<T>> NotBeInAscendingOrder(IComparer<T> comparer, string? because = null, params object?[] becauseArgs) {
        if(comparer is null) {
            throw new ArgumentNullException(nameof(comparer));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_ASCENDING_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        var list = _value.ToList();
        var isAscending = true;
        for(var index = 1; index < list.Count; index++) {
            if(comparer.Compare(list[index - 1], list[index]) > 0) {
                isAscending = false;
                break;
            }
        }

        if(isAscending && list.Count > 1) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_ASCENDING_ORDER, Expression, Format(list[0]), Format(list[list.Count - 1]), Format(0)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the collection is not in ascending order of <paramref name="comparison"/>.</summary>
    /// <param name="comparison">The comparison function (negative when the first item precedes the second).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="comparison"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is in ascending order.</exception>
    public AndConstraint<CollectionAssertions<T>> NotBeInAscendingOrder(Func<T, T, int> comparison, string? because = null, params object?[] becauseArgs) {
        if(comparison is null) {
            throw new ArgumentNullException(nameof(comparison));
        }

        return NotBeInAscendingOrder(OrderCriterion<T>.FromComparison(comparison, true), because, becauseArgs);
    }

    /// <summary>Asserts that the collection is not in ascending order of the key selected by <paramref name="selector"/>.</summary>
    /// <typeparam name="TSelector">The selected key type.</typeparam>
    /// <param name="selector">The key selecting expression.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is in ascending key order.</exception>
    public AndConstraint<CollectionAssertions<T>> NotBeInAscendingOrder<TSelector>(Expression<Func<T, TSelector>> selector, string? because = null, params object?[] becauseArgs) =>
        NotBeInAscendingOrder(selector, null, because, becauseArgs);

    /// <summary>Asserts that the collection is not in ascending order of the key selected by <paramref name="selector"/> using <paramref name="comparer"/>.</summary>
    /// <typeparam name="TSelector">The selected key type.</typeparam>
    /// <param name="selector">The key selecting expression.</param>
    /// <param name="comparer">The comparer defining the key order, or <see langword="null"/> for the default comparer.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is in ascending key order.</exception>
    public AndConstraint<CollectionAssertions<T>> NotBeInAscendingOrder<TSelector>(Expression<Func<T, TSelector>> selector, IComparer<TSelector>? comparer, string? because = null, params object?[] becauseArgs) {
        if(selector is null) {
            throw new ArgumentNullException(nameof(selector));
        }

        var key = selector.Compile();
        return NotBeInAscendingOrder(OrderCriterion<T>.Ascending(key, comparer ?? Comparer<TSelector>.Default), because, becauseArgs);
    }

    /// <summary>Asserts that the collection is not ordered by a single criterion.</summary>
    /// <param name="criterion">The ordering criterion.</param>
    /// <param name="because">The reason for the assertion.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    private AndConstraint<CollectionAssertions<T>> NotBeInAscendingOrder(OrderCriterion<T> criterion, string? because, object?[]? becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_ASCENDING_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        var list = _value.ToList();
        if(list.Count > 1 && SubsequentOrderingAssertions<T>.IsOrdered(list, [criterion])) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_ASCENDING_ORDER, Expression, Format(list[0]), Format(list[list.Count - 1]), Format(0)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the collection is not in descending order using the default comparer.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection is in descending order.</exception>
    public AndConstraint<CollectionAssertions<T>> NotBeInDescendingOrder(string? because = null, params object?[] becauseArgs) =>
        NotBeInDescendingOrder(Comparer<T>.Default, because, becauseArgs);

    /// <summary>Asserts that the collection is not in descending order.</summary>
    /// <param name="comparer">The comparer to use.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is in descending order.</exception>
    public AndConstraint<CollectionAssertions<T>> NotBeInDescendingOrder(IComparer<T> comparer, string? because = null, params object?[] becauseArgs) {
        if(comparer is null) {
            throw new ArgumentNullException(nameof(comparer));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_DESCENDING_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        var list = _value.ToList();
        var isDescending = true;
        for(var index = 1; index < list.Count; index++) {
            if(comparer.Compare(list[index - 1], list[index]) < 0) {
                isDescending = false;
                break;
            }
        }

        if(isDescending && list.Count > 1) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_DESCENDING_ORDER, Expression, Format(list[0]), Format(list[list.Count - 1]), Format(0)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the collection is not in descending order of <paramref name="comparison"/>.</summary>
    /// <param name="comparison">The comparison function (negative when the first item precedes the second).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="comparison"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is in descending order.</exception>
    public AndConstraint<CollectionAssertions<T>> NotBeInDescendingOrder(Func<T, T, int> comparison, string? because = null, params object?[] becauseArgs) {
        if(comparison is null) {
            throw new ArgumentNullException(nameof(comparison));
        }

        return NotBeInDescendingOrder(OrderCriterion<T>.FromComparison(comparison, false), because, becauseArgs);
    }

    /// <summary>Asserts that the collection is not in descending order of the key selected by <paramref name="selector"/>.</summary>
    /// <typeparam name="TSelector">The selected key type.</typeparam>
    /// <param name="selector">The key selecting expression.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is in descending key order.</exception>
    public AndConstraint<CollectionAssertions<T>> NotBeInDescendingOrder<TSelector>(Expression<Func<T, TSelector>> selector, string? because = null, params object?[] becauseArgs) =>
        NotBeInDescendingOrder(selector, null, because, becauseArgs);

    /// <summary>Asserts that the collection is not in descending order of the key selected by <paramref name="selector"/> using <paramref name="comparer"/>.</summary>
    /// <typeparam name="TSelector">The selected key type.</typeparam>
    /// <param name="selector">The key selecting expression.</param>
    /// <param name="comparer">The comparer defining the key order, or <see langword="null"/> for the default comparer.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is in descending key order.</exception>
    public AndConstraint<CollectionAssertions<T>> NotBeInDescendingOrder<TSelector>(Expression<Func<T, TSelector>> selector, IComparer<TSelector>? comparer, string? because = null, params object?[] becauseArgs) {
        if(selector is null) {
            throw new ArgumentNullException(nameof(selector));
        }

        var key = selector.Compile();
        return NotBeInDescendingOrder(OrderCriterion<T>.Descending(key, comparer ?? Comparer<TSelector>.Default), because, becauseArgs);
    }

    /// <summary>Asserts that the collection is not ordered by a single criterion.</summary>
    /// <param name="criterion">The ordering criterion.</param>
    /// <param name="because">The reason for the assertion.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    private AndConstraint<CollectionAssertions<T>> NotBeInDescendingOrder(OrderCriterion<T> criterion, string? because, object?[]? becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_DESCENDING_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        var list = _value.ToList();
        if(list.Count > 1 && SubsequentOrderingAssertions<T>.IsOrdered(list, [criterion])) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_DESCENDING_ORDER, Expression, Format(list[0]), Format(list[list.Count - 1]), Format(0)), because, becauseArgs);
        }

        return new(this);
    }
}
