//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides the generic JSON assertion entry point without changing the existing non-generic overloads.</summary>
/// <remarks>
/// A separate static class is required because C# treats generic overloads with the same parameter shape as
/// duplicates when they live in the same class as the many existing generic <c>Should</c> overloads.
/// </remarks>
public static class JsonAssertionExtensions {
    /// <summary>Creates a <see cref="JsonNodeAssertions{T}"/> for the given JSON node while preserving its closed node type.</summary>
    /// <typeparam name="T">The closed JSON node type under assertion.</typeparam>
    /// <param name="value">The JSON node to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static JsonNodeAssertions<T> Should<T>(this T? value, [CallerArgumentExpression(nameof(value))] string? expression = null)
        where T : JsonNode => new(value, expression);

    /// <summary>Creates a <see cref="JsonElementAssertions"/> for the given JSON element.</summary>
    /// <remarks>The element is snapshotted immediately; keep its <see cref="JsonDocument"/> alive until <c>Should()</c> returns.</remarks>
    /// <param name="value">The JSON element to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static JsonElementAssertions Should(this JsonElement value, [CallerArgumentExpression(nameof(value))] string? expression = null) =>
        new(value, expression);

    /// <summary>Creates a <see cref="JsonElementAssertions"/> for the given JSON document root.</summary>
    /// <remarks>The document must stay alive until <c>Should()</c> returns; a disposed document fails with a clear message.</remarks>
    /// <param name="document">The JSON document to assert on (may be <see langword="null"/>).</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    /// <returns>Assertions over a snapshot of the document root.</returns>
    /// <exception cref="AssertionFailedException">The document was disposed.</exception>
    public static JsonElementAssertions Should(this JsonDocument? document, [CallerArgumentExpression(nameof(document))] string? expression = null) {
        if(document is null) {
            return new(null, expression);
        }

        try {
            return new(document.RootElement, expression);
        }
        catch(ObjectDisposedException) {
            throw new AssertionFailedException(Strings.Format(StringKeys.JSON_DOCUMENT_DISPOSED, expression ?? "document"));
        }
    }
}
