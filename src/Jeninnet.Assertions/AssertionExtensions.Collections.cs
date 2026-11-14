//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public static partial class AssertionExtensions {
    /// <summary>Creates a <see cref="JsonNodeAssertions"/> for the given JSON node.</summary>
    /// <param name="value">The JSON node to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static JsonNodeAssertions Should(this JsonNode? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

#if !NETSTANDARD2_0
    /// <summary>Creates a <see cref="MatchAssertions"/> for the given regex match.</summary>
    /// <param name="value">The regex match to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static MatchAssertions Should(this Match? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);
#endif

    /// <summary>Creates a <see cref="CollectionAssertions{T}"/> treating the given JSON array as a collection of nodes.</summary>
    /// <param name="value">The JSON array to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static CollectionAssertions<JsonNode?> Should(this JsonArray? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="CollectionAssertions{T}"/> for the given collection.</summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="value">The collection to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static CollectionAssertions<T> Should<T>(this IEnumerable<T>? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="StringCollectionAssertions"/> for the given string collection (wildcard matching included).</summary>
    /// <param name="value">The string collection to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static StringCollectionAssertions Should(this IEnumerable<string>? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="CollectionAssertions{T}"/> for the given array.</summary>
    /// <remarks>
    /// Arrays convert implicitly to <c>Span&lt;T&gt;</c>, which would otherwise hijack array subjects
    /// into the span assertions. This overload pins arrays to the collection assertions, preserving
    /// the historical binding (identity conversion always wins overload resolution).
    /// </remarks>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="value">The array to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static CollectionAssertions<T> Should<T>(this T[]? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="GenericDictionaryAssertions{TKey, TValue}"/> for the given dictionary.</summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <param name="value">The dictionary to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static GenericDictionaryAssertions<TKey, TValue> Should<TKey, TValue>(this IDictionary<TKey, TValue>? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="GenericDictionaryAssertions{TKey, TValue}"/> for a read-only dictionary or any sequence of key/value pairs.</summary>
    /// <remarks>
    /// This overload covers <see cref="IReadOnlyDictionary{TKey, TValue}"/> implementations such as
    /// <c>ImmutableDictionary</c> and <c>FrozenDictionary</c>, plus projections that produce
    /// <see cref="KeyValuePair{TKey, TValue}"/> sequences. Lazy sequences are materialized once in the
    /// constructor so a single assertion never re-enumerates the source. A dedicated
    /// <c>IReadOnlyDictionary</c> overload is deliberately omitted because it would be ambiguous with the
    /// <c>IDictionary</c> overload for concrete types that implement both interfaces.
    /// </remarks>
    /// <typeparam name="TKey">The type of keys in the pairs.</typeparam>
    /// <typeparam name="TValue">The type of values in the pairs.</typeparam>
    /// <param name="value">The sequence of key/value pairs to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static GenericDictionaryAssertions<TKey, TValue> Should<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>>? value, [CallerArgumentExpression(nameof(value))] string? expression = null) =>
        GenericDictionaryAssertions<TKey, TValue>.FromPairs(value, expression);

    /// <summary>Creates an <see cref="ObjectAssertions{T}"/> for the given object.</summary>
    /// <param name="value">The object to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static ObjectAssertions<object> Should(this object? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);
}
