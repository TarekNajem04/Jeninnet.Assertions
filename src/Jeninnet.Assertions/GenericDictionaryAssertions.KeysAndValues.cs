//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class GenericDictionaryAssertions<TKey, TValue> {
    /// <summary>Asserts that the dictionary contains <paramref name="key"/>.</summary>
    /// <param name="key">The key that must be present.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or does not contain <paramref name="key"/>.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> ContainKey(TKey key, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_CONTAIN_KEY_NULL, Expression, Format(key)), because, becauseArgs);
            return new(this);
        }

        if(!_value.ContainsKey(key)) {
            var message = Strings.Format(StringKeys.DICT_CONTAIN_KEY_MISSING, Expression, Format(key));
            if(key is string name) {
                message += NearestCandidate.Describe(name, _value.Keys.OfType<string>());
            }

            Fail(message, because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the dictionary does not contain <paramref name="key"/>.</summary>
    /// <param name="key">The key that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or contains <paramref name="key"/>.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> NotContainKey(TKey key, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_NOT_CONTAIN_KEY_NULL, Expression, Format(key)), because, becauseArgs);
            return new(this);
        }

        if(_value.ContainsKey(key)) {
            Fail(Strings.Format(StringKeys.DICT_NOT_CONTAIN_KEY_FOUND, Expression, Format(key)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the dictionary contains every key in <paramref name="keys"/>.</summary>
    /// <param name="keys">The keys that must all be present.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="keys"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or a key is missing.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> ContainKeys(IEnumerable<TKey> keys, string? because = null, params object?[] becauseArgs) {
        if(keys is null) {
            throw new ArgumentNullException(nameof(keys));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_CONTAIN_KEYS_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        var missing = keys.Where(key => !_value.ContainsKey(key)).ToList();
        if(missing.Count != 0) {
            Fail(Strings.Format(StringKeys.DICT_CONTAIN_KEY_MISSING, Expression, Format(missing[0])), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the dictionary contains every key in <paramref name="keys"/>.</summary>
    /// <param name="keys">The keys that must all be present.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or a key is missing.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> ContainKeys(params TKey[] keys) => ContainKeys((IEnumerable<TKey>)keys);

    /// <summary>Asserts that the dictionary contains none of the keys in <paramref name="keys"/>.</summary>
    /// <param name="keys">The keys that must all be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="keys"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or contains one of the keys.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> NotContainKeys(IEnumerable<TKey> keys, string? because = null, params object?[] becauseArgs) {
        if(keys is null) {
            throw new ArgumentNullException(nameof(keys));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_NOT_CONTAIN_KEYS_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        if(keys.Where(_value.ContainsKey).ToList().Count != 0) {
            Fail(Strings.Format(StringKeys.DICT_NOT_CONTAIN_KEY_FOUND, Expression, Format(keys.Where(_value.ContainsKey).ToList()[0])), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the dictionary contains none of the keys in <paramref name="keys"/>.</summary>
    /// <param name="keys">The keys that must all be absent.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or contains one of the keys.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> NotContainKeys(params TKey[] keys) => NotContainKeys((IEnumerable<TKey>)keys);

    /// <summary>
    /// Asserts that the dictionary contains <paramref name="value"/> and returns a <see cref="WhichConstraint{T}"/>
    /// exposing the first matching value for further assertions.
    /// </summary>
    /// <param name="value">The value that must be present.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the first matching value.</returns>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or does not contain <paramref name="value"/>.</exception>
    public WhichConstraint<TValue> ContainValue(TValue value, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.DICT_CONTAIN_VALUE_NULL, Expression, Format(value)), because, becauseArgs));
        }

        if(!_value.Values.Contains(value, EqualityComparer<TValue>.Default)) {
            throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.DICT_CONTAIN_VALUE_MISSING, Expression, Format(value)), because, becauseArgs));
        }

        return new WhichConstraint<TValue>(value);
    }

    /// <summary>Asserts that <paramref name="key"/> maps to <paramref name="value"/>.</summary>
    /// <param name="key">The key to look up.</param>
    /// <param name="value">The expected mapped value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/>, the key is missing, or the value differs.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> Contain(TKey key, TValue value, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_CONTAIN_KEY_NULL, Expression, Format(key)), because, becauseArgs);
            return new(this);
        }

        if(!_value.TryGetValue(key, out var actual)) {
            Fail(Strings.Format(StringKeys.DICT_CONTAIN_KEY_MISSING, Expression, Format(key)), because, becauseArgs);
            return new(this);
        }

        if(!EqualityComparer<TValue>.Default.Equals(actual, value)) {
            Fail(Strings.Format(StringKeys.DICT_HAVE_VALUE_AT_KEY, Expression, Format(value), Format(key), Format(actual)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the dictionary contains the specified values and exposes them for further assertions.</summary>
    /// <param name="values">The values that must be present.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the matched values.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or misses any of the values.</exception>
    public WhichConstraint<IEnumerable<TValue>> ContainValues(IEnumerable<TValue> values, string? because = null, params object?[] becauseArgs) {
        if(values is null) {
            throw new ArgumentNullException(nameof(values));
        }

        var wanted = values.ToList();
        if(_value is null) {
            throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.DICT_CONTAIN_VALUE_NULL, Expression, JoinItems(wanted)), because, becauseArgs));
        }

        var missing = wanted.Where(value => !_value.Values.Contains(value, EqualityComparer<TValue>.Default)).ToList();
        if(missing.Count != 0) {
            throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.DICT_CONTAIN_VALUE_MISSING, Expression, JoinItems(missing)), because, becauseArgs));
        }

        return new WhichConstraint<IEnumerable<TValue>>([.. wanted.Where(value => _value.Values.Contains(value, EqualityComparer<TValue>.Default))]);
    }

    /// <summary>Asserts that the dictionary contains the specified values and exposes them for further assertions.</summary>
    /// <param name="values">The values that must be present.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the matched values.</returns>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or misses any of the values.</exception>
    public WhichConstraint<IEnumerable<TValue>> ContainValues(params TValue[] values) =>
        ContainValues((IEnumerable<TValue>)values);

    /// <summary>Asserts that the dictionary does not contain the specified key.</summary>
    /// <param name="key">The key that must not be present.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The dictionary contains the key.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> NotContain(TKey key, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_NOT_CONTAIN_KEY_VALUE_NULL, Expression, Format(key)), because, becauseArgs);
            return new(this);
        }

        if(_value.ContainsKey(key)) {
            Fail(Strings.Format(StringKeys.DICT_NOT_CONTAIN_KEY_VALUE_FOUND, Expression, Format(key)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the dictionary does not contain the specified key/value pair.</summary>
    /// <param name="pair">The pair that must not be present.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The dictionary contains the pair.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> NotContain(KeyValuePair<TKey, TValue> pair, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_NOT_CONTAIN_KEY_VALUE_NULL, Expression, Format(pair.Key)), because, becauseArgs);
            return new(this);
        }

        if(_value.TryGetValue(pair.Key, out var actual) && EqualityComparer<TValue>.Default.Equals(actual, pair.Value)) {
            Fail(Strings.Format(StringKeys.DICT_NOT_CONTAIN_PAIR_FOUND, Expression, Format(pair)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the dictionary does not contain any of the specified key/value pairs.</summary>
    /// <param name="pairs">The pairs that must not be present.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pairs"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The dictionary contains any of the pairs.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> NotContain(IEnumerable<KeyValuePair<TKey, TValue>> pairs, string? because = null, params object?[] becauseArgs) {
        if(pairs is null) {
            throw new ArgumentNullException(nameof(pairs));
        }

        var pairList = pairs.ToList();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_NOT_CONTAIN_PAIRS_NULL, Expression, JoinItems(pairList)), because, becauseArgs);
            return new(this);
        }

        foreach(var pair in pairList) {
            if(_value.TryGetValue(pair.Key, out var actual) && EqualityComparer<TValue>.Default.Equals(actual, pair.Value)) {
                Fail(Strings.Format(StringKeys.DICT_NOT_CONTAIN_PAIRS_FOUND, Expression, JoinItems(pairList), Format(pair)), because, becauseArgs);
                return new(this);
            }
        }

        return new(this);
    }

    /// <summary>Asserts that the dictionary does not contain the specified key with the specified value.</summary>
    /// <param name="key">The key that must not map to <paramref name="value"/>.</param>
    /// <param name="value">The value that must not be mapped from <paramref name="key"/>.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The dictionary maps <paramref name="key"/> to <paramref name="value"/>.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> NotContain(TKey key, TValue value, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_NOT_CONTAIN_KEY_VALUE_NULL, Expression, Format(key)), because, becauseArgs);
            return new(this);
        }

        if(_value.TryGetValue(key, out var actual) && EqualityComparer<TValue>.Default.Equals(actual, value)) {
            Fail(Strings.Format(StringKeys.DICT_NOT_CONTAIN_KEY_VALUE_PAIR, Expression, Format(key), Format(value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the dictionary does not contain the specified value.</summary>
    /// <param name="value">The value that must not be present.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The dictionary contains the value.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> NotContainValue(TValue value, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_NOT_CONTAIN_VALUE_NULL, Expression, Format(value)), because, becauseArgs);
            return new(this);
        }

        if(_value.Values.Contains(value, EqualityComparer<TValue>.Default)) {
            Fail(Strings.Format(StringKeys.DICT_NOT_CONTAIN_VALUE_FOUND, Expression, Format(value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the dictionary does not contain any of the specified values.</summary>
    /// <param name="values">The values that must not be present.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The dictionary contains any of the values.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> NotContainValues(IEnumerable<TValue> values, string? because = null, params object?[] becauseArgs) {
        if(values is null) {
            throw new ArgumentNullException(nameof(values));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_NOT_CONTAIN_VALUES_NULL, Expression, JoinItems(values)), because, becauseArgs);
            return new(this);
        }

        var found = _value.Values.Where(v => values.Contains(v, EqualityComparer<TValue>.Default)).ToList();
        if(found.Count != 0) {
            Fail(Strings.Format(StringKeys.DICT_NOT_CONTAIN_VALUES_FOUND, Expression, JoinItems(values)), because, becauseArgs);
        }

        return new(this);
    }
}
