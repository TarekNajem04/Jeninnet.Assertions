//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
#if NET6_0_OR_GREATER
namespace Jeninnet.Assertions;

public partial class HttpResponseMessageAssertions {
    private const int CONTENT_PREVIEW_LENGTH = 200;

    /// <summary>Asserts that the response body equals <paramref name="expected"/> and exposes the actual text.</summary>
    /// <remarks>The body is read once with <paramref name="cancellationToken"/>; it cannot be re-read afterwards.</remarks>
    /// <param name="expected">The expected body text.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="cancellationToken">The token passed to <see cref="HttpContent.ReadAsStringAsync(CancellationToken)"/>.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the actual body text.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The response or content is <see langword="null"/> or the text differs.</exception>
    public async Task<WhichConstraint<string>> HaveStringContentAsync(
        string expected,
        string? because = null,
        CancellationToken cancellationToken = default,
        params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var actual = await ReadContentAsync(StringKeys.HTTP_STRING_CONTENT_NULL, because, becauseArgs, cancellationToken).ConfigureAwait(false);
        if(actual is null) {
            return new WhichConstraint<string>(string.Empty);
        }

        if(!string.Equals(actual, expected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.HTTP_STRING_CONTENT, Expression, expected, Preview(actual)), because, becauseArgs);
        }

        return new WhichConstraint<string>(actual);
    }

    /// <summary>Asserts that the response body satisfies <paramref name="predicate"/> and exposes the actual text.</summary>
    /// <remarks>The body is read once with <paramref name="cancellationToken"/>; it cannot be re-read afterwards.</remarks>
    /// <param name="predicate">The condition the body text must satisfy.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="cancellationToken">The token passed to <see cref="HttpContent.ReadAsStringAsync(CancellationToken)"/>.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the actual body text.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The response or content is <see langword="null"/> or the predicate fails.</exception>
    public async Task<WhichConstraint<string>> HaveStringContentAsync(
        Func<string, bool> predicate,
        string? because = null,
        CancellationToken cancellationToken = default,
        params object?[] becauseArgs) {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        var actual = await ReadContentAsync(StringKeys.HTTP_STRING_CONTENT_NULL, because, becauseArgs, cancellationToken).ConfigureAwait(false);
        if(actual is null) {
            return new WhichConstraint<string>(string.Empty);
        }

        if(!predicate(actual)) {
            Fail(Strings.Format(StringKeys.HTTP_STRING_CONTENT_PREDICATE, Expression, Preview(actual)), because, becauseArgs);
        }

        return new WhichConstraint<string>(actual);
    }

    /// <summary>Asserts that the response body is valid JSON and exposes the parsed node.</summary>
    /// <remarks>The body is read once with <paramref name="cancellationToken"/>; it cannot be re-read afterwards.</remarks>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="cancellationToken">The token passed to <see cref="HttpContent.ReadAsStringAsync(CancellationToken)"/>.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the parsed JSON node.</returns>
    /// <exception cref="AssertionFailedException">The response or content is <see langword="null"/> or the body is not valid JSON.</exception>
    public async Task<WhichConstraint<JsonNode?>> HaveJsonContentAsync(
        string? because = null,
        CancellationToken cancellationToken = default,
        params object?[] becauseArgs) {
        var actual = await ReadContentAsync(StringKeys.HTTP_JSON_CONTENT_NULL, because, becauseArgs, cancellationToken).ConfigureAwait(false);
        if(actual is null) {
            return new WhichConstraint<JsonNode?>(null);
        }

        try {
            return new WhichConstraint<JsonNode?>(JsonNode.Parse(actual));
        }
        catch(JsonException) {
            Fail(Strings.Format(StringKeys.HTTP_JSON_CONTENT_INVALID, Expression, Preview(actual)), because, becauseArgs);
            return new WhichConstraint<JsonNode?>(null);
        }
    }

    /// <summary>Asserts that the response is a ProblemDetails document (RFC 7807/9457).</summary>
    /// <remarks>
    /// Accepts <c>application/problem+json</c> and <c>application/json</c>; the body must be a JSON object.
    /// Supplied fields are compared with ordinal string equality (or integer equality for status).
    /// The body is read once with <paramref name="cancellationToken"/>.
    /// </remarks>
    /// <param name="title">The expected <c>title</c>, or <see langword="null"/> to skip it.</param>
    /// <param name="status">The expected <c>status</c>, or <see langword="null"/> to skip it.</param>
    /// <param name="detail">The expected <c>detail</c>, or <see langword="null"/> to skip it.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="cancellationToken">The token passed to <see cref="HttpContent.ReadAsStringAsync(CancellationToken)"/>.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The response is not a valid ProblemDetails document.</exception>
    public async Task<AndConstraint<HttpResponseMessageAssertions>> BeProblemDetailsAsync(
        string? title = null,
        int? status = null,
        string? detail = null,
        string? because = null,
        CancellationToken cancellationToken = default,
        params object?[] becauseArgs) {
        if(FailIfNull(StringKeys.HTTP_PROBLEM_DETAILS_NULL, because, becauseArgs)) {
            return new(this);
        }

        var mediaType = _value!.Content?.Headers.ContentType?.MediaType;
        if(!string.Equals(mediaType, "application/problem+json", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(mediaType, "application/json", StringComparison.OrdinalIgnoreCase)) {
            Fail(Strings.Format(StringKeys.HTTP_PROBLEM_DETAILS_CONTENT_TYPE, Expression, mediaType ?? "<null>"), because, becauseArgs);
            return new(this);
        }

        var actual = await ReadContentAsync(StringKeys.HTTP_PROBLEM_DETAILS_NULL, because, becauseArgs, cancellationToken).ConfigureAwait(false);
        if(actual is null) {
            return new(this);
        }

        JsonNode? node;
        try {
            node = JsonNode.Parse(actual);
        }
        catch(JsonException) {
            Fail(Strings.Format(StringKeys.HTTP_JSON_CONTENT_INVALID, Expression, Preview(actual)), because, becauseArgs);
            return new(this);
        }

        if(node is not JsonObject problem) {
            Fail(Strings.Format(StringKeys.HTTP_PROBLEM_DETAILS_NOT_OBJECT, Expression, Preview(actual)), because, becauseArgs);
            return new(this);
        }

        if(title is not null && (!TryGetString(problem, "title", out var actualTitle) || !string.Equals(actualTitle, title, StringComparison.Ordinal))) {
            Fail(Strings.Format(StringKeys.HTTP_PROBLEM_DETAILS_FIELD, Expression, "title", title, JsonValuePreview(problem, "title")), because, becauseArgs);
            return new(this);
        }

        if(status is not null && (!TryGetInt(problem, "status", out var actualStatus) || actualStatus != status.Value)) {
            Fail(Strings.Format(StringKeys.HTTP_PROBLEM_DETAILS_FIELD, Expression, "status", AssertionScope.FormatInvariant(status.Value), JsonValuePreview(problem, "status")), because, becauseArgs);
            return new(this);
        }

        if(detail is not null && (!TryGetString(problem, "detail", out var actualDetail) || !string.Equals(actualDetail, detail, StringComparison.Ordinal))) {
            Fail(Strings.Format(StringKeys.HTTP_PROBLEM_DETAILS_FIELD, Expression, "detail", detail, JsonValuePreview(problem, "detail")), because, becauseArgs);
            return new(this);
        }

        return new(this);
    }

    /// <summary>Reads the response body once, failing clearly when the response or content is null.</summary>
    /// <param name="nullMessageKey">The localization key used when the response is null.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <param name="cancellationToken">The token passed to the content reader.</param>
    /// <returns>The body text, or <see langword="null"/> when a failure was recorded.</returns>
    private async Task<string?> ReadContentAsync(string nullMessageKey, string? because, object?[] becauseArgs, CancellationToken cancellationToken) {
        if(FailIfNull(nullMessageKey, because, becauseArgs)) {
            return null;
        }

        if(_value!.Content is null) {
            Fail(Strings.Format(StringKeys.HTTP_CONTENT_MISSING, Expression), because, becauseArgs);
            return null;
        }

        return await _value.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Reads a string field from a JSON object.</summary>
    /// <param name="obj">The JSON object.</param>
    /// <param name="name">The field name.</param>
    /// <param name="value">The string value when present.</param>
    /// <returns><see langword="true"/> when the field is a string.</returns>
    private static bool TryGetString(JsonObject obj, string name, out string? value) {
        if(obj[name] is JsonValue node && node.TryGetValue(out string? text)) {
            value = text;
            return true;
        }

        value = null;
        return false;
    }

    /// <summary>Reads an integer field from a JSON object.</summary>
    /// <param name="obj">The JSON object.</param>
    /// <param name="name">The field name.</param>
    /// <param name="value">The integer value when present.</param>
    /// <returns><see langword="true"/> when the field is an integer.</returns>
    private static bool TryGetInt(JsonObject obj, string name, out int value) {
        if(obj[name] is JsonValue node && node.TryGetValue(out value)) {
            return true;
        }

        value = 0;
        return false;
    }

    /// <summary>Renders a JSON field for a ProblemDetails failure message.</summary>
    /// <param name="obj">The ProblemDetails object.</param>
    /// <param name="name">The field name.</param>
    /// <returns>The compact field JSON, or <c>&lt;missing&gt;</c>.</returns>
    private static string JsonValuePreview(JsonObject obj, string name) => obj[name]?.ToJsonString() ?? "<missing>";

    /// <summary>Truncates body text for a failure message.</summary>
    /// <param name="text">The body text.</param>
    /// <returns>The text, truncated to the preview length.</returns>
    private static string Preview(string text) =>
        text.Length > CONTENT_PREVIEW_LENGTH ? text.TruncateTo(CONTENT_PREVIEW_LENGTH) + "…" : text;
}
#endif
