//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
#if NET6_0_OR_GREATER
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying <see cref="HttpResponseMessage"/> responses.</summary>
/// <param name="value">The response to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public class HttpResponseMessageAssertions(HttpResponseMessage? value, string? expression = null) : AssertionsBase(expression, "response") {
    private readonly HttpResponseMessage? _value = value;

    /// <summary>Asserts that the response status code equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected status code.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The response is <see langword="null"/> or has another status code.</exception>
    public AndConstraint<HttpResponseMessageAssertions> HaveStatusCode(HttpStatusCode expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.HTTP_HAVE_STATUS_NULL, Expression, AssertionScope.FormatInvariant(expected)), because, becauseArgs);
            return new(this);
        }

        if(_value.StatusCode != expected) {
            Fail(Strings.Format(StringKeys.HTTP_HAVE_STATUS, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_value.StatusCode)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the response status code is successful (2xx).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The response is <see langword="null"/> or not successful.</exception>
    public AndConstraint<HttpResponseMessageAssertions> BeSuccessful(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.HTTP_BE_SUCCESSFUL_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        if((int)_value.StatusCode is < 200 or > 299) {
            Fail(Strings.Format(StringKeys.HTTP_BE_SUCCESSFUL, Expression, AssertionScope.FormatInvariant(_value.StatusCode)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the response carries a header named <paramref name="name"/> (response or content headers),
    /// optionally with <paramref name="expectedValue"/> among its values.</summary>
    /// <param name="name">The header name.</param>
    /// <param name="expectedValue">The expected header value (<see langword="null"/> checks presence only).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The response is <see langword="null"/>, lacks the header, or holds other values.</exception>
    public AndConstraint<HttpResponseMessageAssertions> HaveHeader(string name, string? expectedValue = null, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.HTTP_HAVE_HEADER_NULL, Expression, name), because, becauseArgs);
            return new(this);
        }

        if(!TryGetHeaderValues(_value, name, out var values)) {
            Fail(Strings.Format(StringKeys.HTTP_HAVE_HEADER_MISSING, Expression, name), because, becauseArgs);
            return new(this);
        }

        if(expectedValue is not null) {
            var actual = values.ToList();
            if(!actual.Contains(expectedValue)) {
                Fail(Strings.Format(StringKeys.HTTP_HAVE_HEADER_VALUE, Expression, name, expectedValue, string.Join(", ", actual)), because, becauseArgs);
            }
        }

        return new(this);
    }

    /// <summary>Looks up header values on the response or its content.</summary>
    /// <param name="response">The response to inspect.</param>
    /// <param name="name">The header name.</param>
    /// <param name="values">The found values, or an empty sequence.</param>
    /// <returns><see langword="true"/> when the header exists on either the response or its content.</returns>
    private static bool TryGetHeaderValues(HttpResponseMessage response, string name, out IEnumerable<string> values) {
        if(response.Headers.TryGetValues(name, out var headerValues)) {
            values = headerValues;
            return true;
        }

        if(response.Content is not null && response.Content.Headers.TryGetValues(name, out var contentValues)) {
            values = contentValues;
            return true;
        }

        values = [];
        return false;
    }
}
#endif
