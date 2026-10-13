//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class GenericDictionaryAssertions<TKey, TValue> {
    /// <summary>Asserts that every pair of the dictionary appears in <paramref name="expected"/>.</summary>
    /// <param name="expected">The superset pairs.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or holds an outsider pair.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> BeSubsetOf(IEnumerable<KeyValuePair<TKey, TValue>> expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var superset = expected.ToList();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_BE_SUBSET_NULL, Expression, JoinItems(superset)), because, becauseArgs);
            return new(this);
        }

        var outsider = _value.Where(pair => !superset.Any(candidate =>
            EqualityComparer<TKey>.Default.Equals(candidate.Key, pair.Key)
            && EqualityComparer<TValue>.Default.Equals(candidate.Value, pair.Value))).ToList();
        if(outsider.Count != 0) {
            Fail(Strings.Format(StringKeys.DICT_BE_SUBSET_OUTSIDER, Expression, JoinItems(superset), Format(outsider[0])), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the dictionary is not a subset of the specified pairs.</summary>
    /// <param name="unexpected">The pairs that must not be a superset.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="unexpected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">Every pair appears in <paramref name="unexpected"/>.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> NotBeSubsetOf(IEnumerable<KeyValuePair<TKey, TValue>> unexpected, string? because = null, params object?[] becauseArgs) {
        if(unexpected is null) {
            throw new ArgumentNullException(nameof(unexpected));
        }

        var superset = unexpected.ToList();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_BE_SUBSET_NULL, Expression, JoinItems(superset)), because, becauseArgs);
            return new(this);
        }

        var outsider = _value.Where(pair => !superset.Any(candidate =>
            EqualityComparer<TKey>.Default.Equals(candidate.Key, pair.Key)
            && EqualityComparer<TValue>.Default.Equals(candidate.Value, pair.Value))).ToList();
        if(outsider.Count == 0 && _value.Count != 0) {
            Fail(Strings.Format(StringKeys.DICT_BE_SUBSET_OUTSIDER, Expression, JoinItems(superset), Format(_value.First())), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the dictionary contains every pair of <paramref name="expected"/> (which may be empty).</summary>
    /// <param name="expected">The subset pairs that must be contained.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or misses a subset pair.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> BeSupersetOf(IEnumerable<KeyValuePair<TKey, TValue>> expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var subset = expected.ToList();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_BE_SUPERSET_NULL, Expression, JoinItems(subset)), because, becauseArgs);
            return new(this);
        }

        var missing = subset.Where(pair => !_value.TryGetValue(pair.Key, out var actual) || !EqualityComparer<TValue>.Default.Equals(actual, pair.Value)).ToList();
        if(missing.Count != 0) {
            Fail(Strings.Format(StringKeys.DICT_BE_SUPERSET, Expression, JoinItems(subset), Format(missing[0])), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that every pair of the dictionary appears in <paramref name="expected"/>, which holds at least one pair the dictionary lacks.</summary>
    /// <param name="expected">The strict superset pairs.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/>, has an outsider, or matches exactly.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> BeProperSubsetOf(IEnumerable<KeyValuePair<TKey, TValue>> expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var superset = expected.ToList();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_BE_PROPER_SUBSET_NULL, Expression, JoinItems(superset)), because, becauseArgs);
            return new(this);
        }

        var actualList = _value.ToList();
        var outsider = actualList.Where(pair => !superset.Any(candidate =>
            EqualityComparer<TKey>.Default.Equals(candidate.Key, pair.Key)
            && EqualityComparer<TValue>.Default.Equals(candidate.Value, pair.Value))).ToList();
        if(outsider.Count != 0) {
            Fail(Strings.Format(StringKeys.DICT_BE_SUBSET_OUTSIDER, Expression, JoinItems(superset), Format(outsider[0])), because, becauseArgs);
            return new(this);
        }

        if(superset.All(candidate => actualList.Any(pair =>
            EqualityComparer<TKey>.Default.Equals(pair.Key, candidate.Key)
            && EqualityComparer<TValue>.Default.Equals(pair.Value, candidate.Value)))) {
            Fail(Strings.Format(StringKeys.DICT_BE_PROPER_SUBSET_SAME, Expression, JoinItems(superset)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the dictionary contains every pair of <paramref name="expected"/> plus at least one pair it lacks.</summary>
    /// <param name="expected">The strict subset pairs that must be contained.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/>, misses a subset pair, or matches exactly.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> BeProperSupersetOf(IEnumerable<KeyValuePair<TKey, TValue>> expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var subset = expected.ToList();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_BE_PROPER_SUPERSET_NULL, Expression, JoinItems(subset)), because, becauseArgs);
            return new(this);
        }

        var missing = subset.Where(pair => !_value.TryGetValue(pair.Key, out var actual) || !EqualityComparer<TValue>.Default.Equals(actual, pair.Value)).ToList();
        if(missing.Count != 0) {
            Fail(Strings.Format(StringKeys.DICT_BE_SUPERSET, Expression, JoinItems(subset), Format(missing[0])), because, becauseArgs);
            return new(this);
        }

        if(_value.All(pair => subset.Any(candidate =>
            EqualityComparer<TKey>.Default.Equals(candidate.Key, pair.Key)
            && EqualityComparer<TValue>.Default.Equals(candidate.Value, pair.Value)))) {
            Fail(Strings.Format(StringKeys.DICT_BE_PROPER_SUPERSET_SAME, Expression, JoinItems(subset)), because, becauseArgs);
        }

        return new(this);
    }
}
