//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
#if NET6_0_OR_GREATER
namespace Jeninnet.Assertions;

public partial class HttpResponseMessageAssertions {
    private static readonly TimeSpan _retryAfterDateTolerance = TimeSpan.FromSeconds(5);

    /// <summary>Asserts that the response is <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The response is not <see langword="null"/>.</exception>
    public AndConstraint<HttpResponseMessageAssertions> BeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is not null) {
            Fail(Strings.Format(StringKeys.HTTP_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the response is not <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The response is <see langword="null"/>.</exception>
    public AndConstraint<HttpResponseMessageAssertions> NotBeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.HTTP_NOT_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the status code is a redirect (3xx).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The response is <see langword="null"/> or not a redirect.</exception>
    public AndConstraint<HttpResponseMessageAssertions> BeRedirect(string? because = null, params object?[] becauseArgs) {
        if(FailIfNull(StringKeys.HTTP_BE_REDIRECT_NULL, because, becauseArgs)) {
            return new(this);
        }

        if((int)_value!.StatusCode is < 300 or > 399) {
            Fail(Strings.Format(StringKeys.HTTP_BE_REDIRECT, Expression, AssertionScope.FormatInvariant(_value.StatusCode)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the status code is not a redirect.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The response is <see langword="null"/> or a redirect.</exception>
    public AndConstraint<HttpResponseMessageAssertions> NotBeRedirect(string? because = null, params object?[] becauseArgs) {
        if(FailIfNull(StringKeys.HTTP_BE_REDIRECT_NULL, because, becauseArgs)) {
            return new(this);
        }

        if((int)_value!.StatusCode is >= 300 and <= 399) {
            Fail(Strings.Format(StringKeys.HTTP_NOT_BE_REDIRECT, Expression, AssertionScope.FormatInvariant(_value.StatusCode)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the status code is a client error (4xx).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The response is <see langword="null"/> or not a client error.</exception>
    public AndConstraint<HttpResponseMessageAssertions> BeClientError(string? because = null, params object?[] becauseArgs) {
        if(FailIfNull(StringKeys.HTTP_BE_CLIENT_ERROR_NULL, because, becauseArgs)) {
            return new(this);
        }

        if((int)_value!.StatusCode is < 400 or > 499) {
            Fail(Strings.Format(StringKeys.HTTP_BE_CLIENT_ERROR, Expression, AssertionScope.FormatInvariant(_value.StatusCode)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the status code is a server error (5xx).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The response is <see langword="null"/> or not a server error.</exception>
    public AndConstraint<HttpResponseMessageAssertions> BeServerError(string? because = null, params object?[] becauseArgs) {
        if(FailIfNull(StringKeys.HTTP_BE_SERVER_ERROR_NULL, because, becauseArgs)) {
            return new(this);
        }

        if((int)_value!.StatusCode is < 500 or > 599) {
            Fail(Strings.Format(StringKeys.HTTP_BE_SERVER_ERROR, Expression, AssertionScope.FormatInvariant(_value.StatusCode)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the status code is 204 (No Content).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The response is <see langword="null"/> or has another status code.</exception>
    public AndConstraint<HttpResponseMessageAssertions> BeNoContent(string? because = null, params object?[] becauseArgs) {
        if(FailIfNull(StringKeys.HTTP_BE_NO_CONTENT_NULL, because, becauseArgs)) {
            return new(this);
        }

        if(_value!.StatusCode != HttpStatusCode.NoContent) {
            Fail(Strings.Format(StringKeys.HTTP_BE_NO_CONTENT, Expression, AssertionScope.FormatInvariant(_value.StatusCode)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the content type matches <paramref name="mediaType"/> and optionally <paramref name="charset"/>.</summary>
    /// <remarks>Media type and charset compare case-insensitively.</remarks>
    /// <param name="mediaType">The expected media type (for example, <c>application/json</c>).</param>
    /// <param name="charset">The expected charset, or <see langword="null"/> to ignore it.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="mediaType"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The response is <see langword="null"/>, lacks content type, or differs.</exception>
    public AndConstraint<HttpResponseMessageAssertions> HaveContentType(string mediaType, string? charset = null, string? because = null, params object?[] becauseArgs) {
        if(mediaType is null) {
            throw new ArgumentNullException(nameof(mediaType));
        }

        var expected = charset is null ? mediaType : $"{mediaType}; charset={charset}";
        if(FailIfNull(StringKeys.HTTP_HAVE_CONTENT_TYPE_NULL, because, becauseArgs, expected)) {
            return new(this);
        }

        var contentType = _value!.Content?.Headers.ContentType;
        if(contentType is null) {
            Fail(Strings.Format(StringKeys.HTTP_HAVE_CONTENT_TYPE_MISSING, Expression, expected), because, becauseArgs);
            return new(this);
        }

        if(!string.Equals(contentType.MediaType, mediaType, StringComparison.OrdinalIgnoreCase)
            || (charset is not null && !string.Equals(contentType.CharSet, charset, StringComparison.OrdinalIgnoreCase))) {
            Fail(Strings.Format(StringKeys.HTTP_HAVE_CONTENT_TYPE, Expression, expected, contentType.ToString()), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the declared content length equals <paramref name="expected"/> bytes.</summary>
    /// <param name="expected">The expected content length.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="expected"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The response is <see langword="null"/>, has no content length (for example, chunked), or differs.</exception>
    public AndConstraint<HttpResponseMessageAssertions> HaveContentLength(long expected, string? because = null, params object?[] becauseArgs) {
        if(expected < 0) {
            throw new ArgumentOutOfRangeException(nameof(expected), "Content length must not be negative.");
        }

        if(FailIfNull(StringKeys.HTTP_HAVE_CONTENT_LENGTH_NULL, because, becauseArgs, AssertionScope.FormatInvariant(expected))) {
            return new(this);
        }

        var actual = _value!.Content?.Headers.ContentLength;
        if(actual is null) {
            Fail(Strings.Format(StringKeys.HTTP_HAVE_CONTENT_LENGTH_MISSING, Expression, AssertionScope.FormatInvariant(expected)), because, becauseArgs);
            return new(this);
        }

        if(actual != expected) {
            Fail(Strings.Format(StringKeys.HTTP_HAVE_CONTENT_LENGTH, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(actual)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the response uses <paramref name="expected"/> as its HTTP version.</summary>
    /// <param name="expected">The expected HTTP version.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The response is <see langword="null"/> or uses another version.</exception>
    public AndConstraint<HttpResponseMessageAssertions> HaveVersion(Version expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(FailIfNull(StringKeys.HTTP_HAVE_VERSION_NULL, because, becauseArgs, expected.ToString())) {
            return new(this);
        }

        if(!_value!.Version.Equals(expected)) {
            Fail(Strings.Format(StringKeys.HTTP_HAVE_VERSION, Expression, expected.ToString(), _value.Version.ToString()), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the response reason phrase equals <paramref name="expected"/> (ordinal).</summary>
    /// <param name="expected">The expected reason phrase.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The response is <see langword="null"/> or has another reason phrase.</exception>
    public AndConstraint<HttpResponseMessageAssertions> HaveReasonPhrase(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(FailIfNull(StringKeys.HTTP_HAVE_REASON_NULL, because, becauseArgs, expected)) {
            return new(this);
        }

        if(!string.Equals(_value!.ReasonPhrase, expected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.HTTP_HAVE_REASON, Expression, expected, _value.ReasonPhrase ?? "<null>"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the response ETag equals <paramref name="expected"/>.</summary>
    /// <remarks>An unquoted value is normalized to a strong entity tag.</remarks>
    /// <param name="expected">The expected ETag (for example, <c>"abc"</c> or <c>abc</c>).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="expected"/> is not a valid entity tag.</exception>
    /// <exception cref="AssertionFailedException">The response is <see langword="null"/>, lacks an ETag, or differs.</exception>
    public AndConstraint<HttpResponseMessageAssertions> HaveETag(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var normalized = expected.StartsWith('"') || expected.StartsWith("W/", StringComparison.OrdinalIgnoreCase)
            ? expected
            : $"\"{expected}\"";
        if(!EntityTagHeaderValue.TryParse(normalized, out var expectedTag)) {
            throw new ArgumentException("ETag must be a quoted entity tag.", nameof(expected));
        }

        if(FailIfNull(StringKeys.HTTP_HAVE_ETAG_NULL, because, becauseArgs, expected)) {
            return new(this);
        }

        var actual = _value!.Headers.ETag;
        if(actual is null) {
            Fail(Strings.Format(StringKeys.HTTP_HAVE_ETAG_MISSING, Expression, expected), because, becauseArgs);
            return new(this);
        }

        if(!actual.Equals(expectedTag)) {
            Fail(Strings.Format(StringKeys.HTTP_HAVE_ETAG, Expression, expected, actual.ToString()), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the Retry-After header matches <paramref name="expected"/>.</summary>
    /// <remarks>
    /// Delta-seconds compare exactly. HTTP-date values compare the remaining time to <paramref name="expected"/>
    /// with a five-second tolerance because they are evaluated against the current UTC clock.
    /// </remarks>
    /// <param name="expected">The expected retry delay.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="expected"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The response is <see langword="null"/>, lacks Retry-After, or differs.</exception>
    public AndConstraint<HttpResponseMessageAssertions> HaveRetryAfter(TimeSpan expected, string? because = null, params object?[] becauseArgs) {
        if(expected < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(expected), "Retry-After must not be negative.");
        }

        if(FailIfNull(StringKeys.HTTP_HAVE_RETRY_AFTER_NULL, because, becauseArgs, AssertionScope.FormatInvariant(expected))) {
            return new(this);
        }

        var actual = _value!.Headers.RetryAfter;
        if(actual is null) {
            Fail(Strings.Format(StringKeys.HTTP_HAVE_RETRY_AFTER_MISSING, Expression, AssertionScope.FormatInvariant(expected)), because, becauseArgs);
            return new(this);
        }

        if(actual.Delta is { } delta) {
            if(delta != expected) {
                Fail(Strings.Format(StringKeys.HTTP_HAVE_RETRY_AFTER, Expression, AssertionScope.FormatInvariant(expected), actual.ToString()), because, becauseArgs);
            }

            return new(this);
        }

        if(actual.Date is { } date) {
            var remaining = date - DateTimeOffset.UtcNow;
            if((remaining - expected).Duration() > _retryAfterDateTolerance) {
                Fail(Strings.Format(StringKeys.HTTP_HAVE_RETRY_AFTER, Expression, AssertionScope.FormatInvariant(expected), actual.ToString()), because, becauseArgs);
            }

            return new(this);
        }

        Fail(Strings.Format(StringKeys.HTTP_HAVE_RETRY_AFTER_MISSING, Expression, AssertionScope.FormatInvariant(expected)), because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the Location header equals <paramref name="expected"/> (ordinal, preserving relative or absolute form).</summary>
    /// <param name="expected">The expected location.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The response is <see langword="null"/>, lacks Location, or differs.</exception>
    public AndConstraint<HttpResponseMessageAssertions> HaveLocation(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(FailIfNull(StringKeys.HTTP_HAVE_LOCATION_NULL, because, becauseArgs, expected)) {
            return new(this);
        }

        var actual = _value!.Headers.Location;
        if(actual is null) {
            Fail(Strings.Format(StringKeys.HTTP_HAVE_LOCATION_MISSING, Expression, expected), because, becauseArgs);
            return new(this);
        }

        if(!string.Equals(actual.OriginalString, expected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.HTTP_HAVE_LOCATION, Expression, expected, actual.OriginalString), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the response sets a cookie named <paramref name="name"/> (optionally with <paramref name="value"/>).</summary>
    /// <remarks>Reads <c>Set-Cookie</c> response headers; multiple headers are searched in order.</remarks>
    /// <param name="name">The expected cookie name (case-sensitive).</param>
    /// <param name="value">The expected cookie value, or <see langword="null"/> to check the name only.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The response is <see langword="null"/>, lacks the cookie, or has another value.</exception>
    public AndConstraint<HttpResponseMessageAssertions> HaveCookie(string name, string? value = null, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        if(FailIfNull(StringKeys.HTTP_HAVE_COOKIE_NULL, because, becauseArgs, name)) {
            return new(this);
        }

        if(!TryGetHeaderValues(_value!, "Set-Cookie", out var headers)) {
            Fail(Strings.Format(StringKeys.HTTP_HAVE_COOKIE_MISSING, Expression, name), because, becauseArgs);
            return new(this);
        }

        if(!TryFindCookie(headers, name, out var actual)) {
            Fail(Strings.Format(StringKeys.HTTP_HAVE_COOKIE_MISSING, Expression, name), because, becauseArgs);
            return new(this);
        }

        if(value is not null && !string.Equals(actual, value, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.HTTP_HAVE_COOKIE_VALUE, Expression, name, value, actual ?? "<null>"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Finds a cookie by name in <c>Set-Cookie</c> header values and extracts its value.</summary>
    /// <param name="headers">The Set-Cookie header values.</param>
    /// <param name="name">The cookie name.</param>
    /// <param name="value">The unquoted cookie value when found.</param>
    /// <returns><see langword="true"/> when the cookie name is present.</returns>
    private static bool TryFindCookie(IEnumerable<string> headers, string name, out string? value) {
        foreach(var header in headers) {
            var separator = header.IndexOf(';');
            var pair = separator >= 0 ? header.Substring(0, separator) : header;
            var equals = pair.IndexOf('=');
            if(equals <= 0) {
                continue;
            }

            if(!string.Equals(pair.Substring(0, equals).Trim(), name, StringComparison.Ordinal)) {
                continue;
            }

            value = pair.Substring(equals + 1).Trim();
            if(value.Length >= 2 && value[0] == '"' && value[value.Length - 1] == '"') {
                value = value.Substring(1, value.Length - 2);
            }

            return true;
        }

        value = null;
        return false;
    }
}
#endif
