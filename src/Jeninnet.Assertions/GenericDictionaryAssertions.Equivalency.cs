//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class GenericDictionaryAssertions<TKey, TValue> {
    /// <summary>Asserts that the dictionary graph is structurally equivalent to <paramref name="expectation"/> (compared by key).</summary>
    /// <param name="expectation">The expected dictionary.</param>
    /// <param name="config">Optional equivalency configuration (exclusions, ordering, by-value types).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The dictionaries differ.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> BeEquivalentTo(
        IDictionary<TKey, TValue>? expectation,
        Func<SelfReferenceEquivalencyOptions<IDictionary<TKey, TValue>>, SelfReferenceEquivalencyOptions<IDictionary<TKey, TValue>>>? config = null,
        string? because = null,
        params object?[] becauseArgs) {
        var options = config?.Invoke(new SelfReferenceEquivalencyOptions<IDictionary<TKey, TValue>>())
            ?? new SelfReferenceEquivalencyOptions<IDictionary<TKey, TValue>>();
        var differences = EquivalencyEngine.FindDifferences<IDictionary<TKey, TValue>>(_value, expectation, options);
        if(differences.Count == 1) {
            Fail(Strings.Format(StringKeys.EQUIVALENCE_DETAIL, Expression, differences[0].TrimEnd('.')), because, becauseArgs);
        } else if(differences.Count > 1) {
            var builder = new StringBuilder();
            builder.AppendFormat(CultureInfo.InvariantCulture, "Expected {0} to be equivalent, but found {1} difference(s):", Expression, AssertionScope.FormatInvariant(differences.Count));
            for(var index = 0; index < differences.Count; index++) {
                builder.AppendLine().AppendFormat(CultureInfo.InvariantCulture, "{0}. {1}", AssertionScope.FormatInvariant(index + 1), differences[index]);
            }

            Fail(builder.ToString(), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the dictionary equals the specified dictionary (same pairs, any order).</summary>
    /// <param name="expected">The expected pairs.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or differs.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> Equal(IDictionary<TKey, TValue> expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        return Equal((IEnumerable<KeyValuePair<TKey, TValue>>)expected, because, becauseArgs);
    }

    /// <summary>Asserts that the dictionary equals the specified pairs (any order).</summary>
    /// <param name="expected">The expected pairs.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or differs.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> Equal(IEnumerable<KeyValuePair<TKey, TValue>> expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var expectedList = expected.ToList();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_EQUAL_NULL, Expression, JoinItems(expectedList)), because, becauseArgs);
            return new(this);
        }

        if(_value.Count != expectedList.Count) {
            Fail(Strings.Format(StringKeys.DICT_EQUAL_COUNT, Expression, JoinItems(expectedList), AssertionScope.FormatInvariant(_value.Count), AssertionScope.FormatInvariant(expectedList.Count)), because, becauseArgs);
            return new(this);
        }

        foreach(var pair in expectedList) {
            if(!_value.TryGetValue(pair.Key, out var actual)) {
                Fail(Strings.Format(StringKeys.DICT_EQUAL_MISSING_KEY, Expression, JoinItems(expectedList), Format(pair.Key)), because, becauseArgs);
                return new(this);
            }

            if(!EqualityComparer<TValue>.Default.Equals(actual, pair.Value)) {
                Fail(Strings.Format(StringKeys.DICT_EQUAL_WRONG_VALUE, Expression, JoinItems(expectedList), Format(pair.Key), Format(actual), Format(pair.Value)), because, becauseArgs);
                return new(this);
            }
        }

        return new(this);
    }

    /// <summary>Asserts that the dictionary equals the specified pairs (any order).</summary>
    /// <param name="expected">The expected pairs.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or differs.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> Equal(params KeyValuePair<TKey, TValue>[] expected) =>
        Equal((IEnumerable<KeyValuePair<TKey, TValue>>)expected);

    /// <summary>
    /// Asserts that the dictionary equals the items in <paramref name="expected"/>,
    /// comparing pair-wise with <paramref name="equality"/>.
    /// </summary>
    /// <typeparam name="TExpectation">The expected element type.</typeparam>
    /// <param name="expected">The expected items.</param>
    /// <param name="equality">The pair comparer (both sequences must have the same length).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> or <paramref name="equality"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or differs.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> Equal<TExpectation>(IEnumerable<TExpectation> expected, Func<KeyValuePair<TKey, TValue>, TExpectation, bool> equality, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(equality is null) {
            throw new ArgumentNullException(nameof(equality));
        }

        var expectedList = expected.ToList();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_EQUAL_NULL, Expression, JoinItems(expectedList)), because, becauseArgs);
            return new(this);
        }

        var actualList = _value.ToList();
        if(actualList.Count != expectedList.Count) {
            Fail(Strings.Format(StringKeys.DICT_EQUAL_COUNT, Expression, JoinItems(expectedList), AssertionScope.FormatInvariant(actualList.Count), AssertionScope.FormatInvariant(expectedList.Count)), because, becauseArgs);
            return new(this);
        }

        for(var index = 0; index < expectedList.Count; index++) {
            if(!equality(actualList[index], expectedList[index])) {
                Fail(Strings.Format(StringKeys.DICT_EQUAL_WRONG_VALUE, Expression, JoinItems(expectedList), Format(index), Format(actualList[index]), Format(expectedList[index])), because, becauseArgs);
                return new(this);
            }
        }

        return new(this);
    }

    /// <summary>Asserts that the dictionary equals the specified dictionary (readability alias of the <c>Equal</c> dictionary overload).</summary>
    /// <param name="expected">The expected pairs.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or differs.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> BeEqualTo(IDictionary<TKey, TValue> expected, string? because = null, params object?[] becauseArgs) =>
        Equal(expected, because, becauseArgs);

    /// <summary>Asserts that the dictionary equals the specified pairs (readability alias of the <c>Equal</c> enumerable overload).</summary>
    /// <param name="expected">The expected pairs.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or differs.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> BeEqualTo(IEnumerable<KeyValuePair<TKey, TValue>> expected, string? because = null, params object?[] becauseArgs) =>
        Equal(expected, because, becauseArgs);

    /// <summary>Asserts that the dictionary equals the specified pairs (readability alias of the <c>Equal</c> array overload).</summary>
    /// <param name="expected">The expected pairs.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or differs.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> BeEqualTo(params KeyValuePair<TKey, TValue>[] expected) =>
        Equal(expected);

    /// <summary>Asserts that the dictionary equals the specified items pair-wise (readability alias of the <c>Equal</c> predicate overload).</summary>
    /// <typeparam name="TExpectation">The expected element type.</typeparam>
    /// <param name="expected">The expected items.</param>
    /// <param name="equality">The pair comparer.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> or <paramref name="equality"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or differs.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> BeEqualTo<TExpectation>(IEnumerable<TExpectation> expected, Func<KeyValuePair<TKey, TValue>, TExpectation, bool> equality, string? because = null, params object?[] becauseArgs) =>
        Equal(expected, equality, because, becauseArgs);

    /// <summary>Asserts that the dictionary does not equal the specified pairs (readability alias of the <c>NotEqual</c> dictionary overload).</summary>
    /// <param name="unexpected">The pairs that must differ.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="unexpected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The dictionaries are equal.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> NotBeEqualTo(IDictionary<TKey, TValue> unexpected, string? because = null, params object?[] becauseArgs) =>
        NotEqual(unexpected, because, becauseArgs);

    /// <summary>Asserts that the dictionary does not equal the specified dictionary.</summary>
    /// <param name="unexpected">The dictionary that must not match.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="unexpected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The dictionaries are equal.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> NotEqual(IDictionary<TKey, TValue> unexpected, string? because = null, params object?[] becauseArgs) {
        if(unexpected is null) {
            throw new ArgumentNullException(nameof(unexpected));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_NOT_EQUAL_NULL, Expression, Format(unexpected)), because, becauseArgs);
            return new(this);
        }

        if(_value.Count == unexpected.Count) {
            var allMatch = true;
            foreach(var pair in unexpected) {
                if(!_value.TryGetValue(pair.Key, out var actual) || !EqualityComparer<TValue>.Default.Equals(actual, pair.Value)) {
                    allMatch = false;
                    break;
                }
            }

            if(allMatch) {
                Fail(Strings.Format(StringKeys.DICT_NOT_EQUAL, Expression, Format(unexpected)), because, becauseArgs);
            }
        }

        return new(this);
    }
}
