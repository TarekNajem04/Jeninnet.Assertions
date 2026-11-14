//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying dictionaries and sequences of key/value pairs.</summary>
/// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
/// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
public partial class GenericDictionaryAssertions<TKey, TValue> : AssertionsBase {
    private readonly IReadOnlyDictionary<TKey, TValue>? _value;

    /// <summary>Creates assertions for a mutable dictionary.</summary>
    /// <param name="value">The dictionary to assert on.</param>
    /// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
    public GenericDictionaryAssertions(IDictionary<TKey, TValue>? value, string? expression = null)
        : this(Adapt(value), expression) {
    }

    private GenericDictionaryAssertions(IReadOnlyDictionary<TKey, TValue>? value, string? expression)
        : base(expression, "dictionary") {
        _value = value;
    }

    /// <summary>Creates assertions for a sequence of key/value pairs, materializing it once.</summary>
    /// <param name="value">The sequence to assert on.</param>
    /// <param name="expression">The caller expression naming the subject under test.</param>
    /// <returns>Assertions over a single materialized dictionary view.</returns>
    internal static GenericDictionaryAssertions<TKey, TValue> FromPairs(IEnumerable<KeyValuePair<TKey, TValue>>? value, string? expression = null) =>
        new(value is null ? null : Materialize(value), expression);

    private static IReadOnlyDictionary<TKey, TValue>? Adapt(IDictionary<TKey, TValue>? value) =>
        value is null ? null : value as IReadOnlyDictionary<TKey, TValue> ?? new DictionaryAdapter(value);

    private static IReadOnlyDictionary<TKey, TValue> Materialize(IEnumerable<KeyValuePair<TKey, TValue>> value) =>
        value as IReadOnlyDictionary<TKey, TValue> ?? new MaterializedPairsView(value);

    private static string Format<TItem>(TItem item) => item is null ? "<null>" : $"'{AssertionScope.FormatInvariant(item)}'";

    /// <summary>Asserts that the dictionary is <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The dictionary is not <see langword="null"/>.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> BeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is not null) {
            Fail(Strings.Format(StringKeys.DICT_BE_NULL, Expression, AssertionScope.FormatInvariant(_value.Count)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the dictionary is not <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/>.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> NotBeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_NOT_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the dictionary contains exactly <paramref name="expected"/> entries.</summary>
    /// <param name="expected">The expected entry count.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or its count does not match.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> HaveCount(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_HAVE_EXACTLY_NULL, Expression, AssertionScope.FormatInvariant(expected)), because, becauseArgs);
            return new(this);
        }

        if(_value.Count != expected) {
            Fail(Strings.Format(StringKeys.DICT_HAVE_EXACTLY, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_value.Count)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the dictionary is empty.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or contains at least one entry.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> BeEmpty(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_BE_EMPTY_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        if(_value.Count != 0) {
            Fail(Strings.Format(StringKeys.DICT_BE_EMPTY, Expression, AssertionScope.FormatInvariant(_value.Count)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the dictionary is not empty.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or is empty.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> NotBeEmpty(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_NOT_BE_EMPTY_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        if(_value.Count == 0) {
            Fail(Strings.Format(StringKeys.DICT_NOT_BE_EMPTY, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the dictionary has the same number of items as <paramref name="expected"/>.</summary>
    /// <typeparam name="TExpectation">The expected element type.</typeparam>
    /// <param name="expected">The collection to compare counts with.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The dictionary is <see langword="null"/> or the counts differ.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> HaveSameCount<TExpectation>(IEnumerable<TExpectation> expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var expectedList = expected.ToList();
        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_HAVE_SAME_COUNT, Expression, AssertionScope.FormatInvariant(expectedList.Count), "<null>"), because, becauseArgs);
            return new(this);
        }

        if(_value.Count != expectedList.Count) {
            Fail(Strings.Format(StringKeys.DICT_HAVE_SAME_COUNT, Expression, AssertionScope.FormatInvariant(expectedList.Count), AssertionScope.FormatInvariant(_value.Count)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the dictionary does not contain exactly the specified number of items.</summary>
    /// <param name="unexpected">The item count that must not match.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The dictionary contains exactly the specified number of items.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> NotHaveCount(int unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_NOT_HAVE_EXACTLY_NULL, Expression, AssertionScope.FormatInvariant(unexpected)), because, becauseArgs);
            return new(this);
        }

        if(_value.Count == unexpected) {
            Fail(Strings.Format(StringKeys.DICT_NOT_HAVE_EXACTLY, Expression, AssertionScope.FormatInvariant(unexpected), AssertionScope.FormatInvariant(_value.Count)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the dictionary does not have the same number of items as the other dictionary.</summary>
    /// <typeparam name="TOther">The type of the other dictionary.</typeparam>
    /// <param name="other">The other dictionary.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The dictionaries have the same number of items.</exception>
    public AndConstraint<GenericDictionaryAssertions<TKey, TValue>> NotHaveSameCount<TOther>(IDictionary<TKey, TOther> other, string? because = null, params object?[] becauseArgs) {
        if(other is null) {
            throw new ArgumentNullException(nameof(other));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.DICT_NOT_HAVE_SAME_COUNT_NULL, Expression, AssertionScope.FormatInvariant(other.Count)), because, becauseArgs);
            return new(this);
        }

        if(_value.Count == other.Count) {
            Fail(Strings.Format(StringKeys.DICT_NOT_HAVE_SAME_COUNT, Expression, AssertionScope.FormatInvariant(other.Count), AssertionScope.FormatInvariant(_value.Count)), because, becauseArgs);
        }

        return new(this);
    }

    private static string JoinItems<TItem>(IEnumerable<TItem> items) => "[" + string.Join(", ", items.Select(static x => $"'{AssertionScope.FormatInvariant(x)}'")) + "]";

    /// <summary>Adapts a mutable dictionary to the read-only view used by every assertion.</summary>
    /// <param name="dictionary">The dictionary to wrap.</param>
    private sealed class DictionaryAdapter(IDictionary<TKey, TValue> dictionary) : IReadOnlyDictionary<TKey, TValue> {
        public int Count => dictionary.Count;

        public IEnumerable<TKey> Keys => dictionary.Keys;

        public IEnumerable<TValue> Values => dictionary.Values;

        public TValue this[TKey key] => dictionary[key];

        public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);

        public bool TryGetValue(TKey key, out TValue value) {
            if(dictionary.TryGetValue(key, out var found)) {
                value = found;
                return true;
            }

            value = default!;
            return false;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>Materializes a pair sequence once and exposes it as a read-only dictionary view.</summary>
    /// <param name="pairs">The sequence to materialize.</param>
    private sealed class MaterializedPairsView(IEnumerable<KeyValuePair<TKey, TValue>> pairs) : IReadOnlyDictionary<TKey, TValue> {
        private readonly KeyValuePair<TKey, TValue>[] _items = [.. pairs];

        public int Count => _items.Length;

        public IEnumerable<TKey> Keys => _items.Select(static pair => pair.Key);

        public IEnumerable<TValue> Values => _items.Select(static pair => pair.Value);

        public TValue this[TKey key] =>
            TryGetValue(key, out var value) ? value : throw new KeyNotFoundException($"The key '{AssertionScope.FormatInvariant(key)}' was not found.");

        public bool ContainsKey(TKey key) => _items.Any(pair => EqualityComparer<TKey>.Default.Equals(pair.Key, key));

        public bool TryGetValue(TKey key, out TValue value) {
            var index = Array.FindIndex(_items, pair => EqualityComparer<TKey>.Default.Equals(pair.Key, key));
            if(index < 0) {
                value = default!;
                return false;
            }

            value = _items[index].Value;
            return true;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() =>
            ((IEnumerable<KeyValuePair<TKey, TValue>>)_items).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
