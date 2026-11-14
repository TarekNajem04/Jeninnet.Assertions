//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying <see cref="Uri"/> values and their components.</summary>
/// <remarks>
/// Equality follows <see cref="Uri"/> canonicalization (scheme and host are case-insensitive, default ports
/// are normalized). Component assertions require an absolute URI and fail with a dedicated message for a
/// relative subject. Query parameter names and values are case-sensitive after URI unescaping; duplicates are
/// reported together, and <c>HaveQueryParameter(name)</c> exposes the first value through <c>Which</c>.
/// </remarks>
/// <param name="value">The URI to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public class UriAssertions(Uri? value, string? expression = null) : AssertionsBase(expression, "URI") {
    private readonly Uri? _value = value;

    private static string FormatUri(Uri? uri) => uri?.OriginalString ?? "<null>";

    /// <summary>Asserts that the URI equals <paramref name="expected"/> using <see cref="Uri"/> equality.</summary>
    /// <param name="expected">The expected URI, or <see langword="null"/>.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The URIs differ.</exception>
    public AndConstraint<UriAssertions> Be(Uri? expected, string? because = null, params object?[] becauseArgs) {
        if(!Equals(_value, expected)) {
            Fail(Strings.Format(StringKeys.URI_BE, Expression, FormatUri(expected), FormatUri(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the URI does not equal <paramref name="unexpected"/> using <see cref="Uri"/> equality.</summary>
    /// <param name="unexpected">The URI the subject must not equal, or <see langword="null"/>.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The URIs are equal.</exception>
    public AndConstraint<UriAssertions> NotBe(Uri? unexpected, string? because = null, params object?[] becauseArgs) {
        if(Equals(_value, unexpected)) {
            Fail(Strings.Format(StringKeys.URI_NOT_BE, Expression, FormatUri(unexpected), FormatUri(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the URI is <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The URI is not <see langword="null"/>.</exception>
    public AndConstraint<UriAssertions> BeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is not null) {
            Fail(Strings.Format(StringKeys.URI_BE_NULL, Expression, FormatUri(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the URI is not <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The URI is <see langword="null"/>.</exception>
    public AndConstraint<UriAssertions> NotBeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.URI_NOT_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the URI is absolute.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The URI is <see langword="null"/> or relative.</exception>
    public AndConstraint<UriAssertions> BeAbsolute(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.URI_NULL, Expression), because, becauseArgs);
        } else if(!_value.IsAbsoluteUri) {
            Fail(Strings.Format(StringKeys.URI_BE_ABSOLUTE, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the URI is relative.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The URI is <see langword="null"/> or absolute.</exception>
    public AndConstraint<UriAssertions> BeRelative(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.URI_NULL, Expression), because, becauseArgs);
        } else if(_value.IsAbsoluteUri) {
            Fail(Strings.Format(StringKeys.URI_BE_RELATIVE, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the absolute URI uses <paramref name="expected"/> as its scheme (case-insensitive).</summary>
    /// <param name="expected">The expected scheme.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The URI is <see langword="null"/>, relative, or has another scheme.</exception>
    public AndConstraint<UriAssertions> HaveScheme(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(!EnsureAbsolute(because, becauseArgs)) {
            return new(this);
        }

        if(!string.Equals(_value!.Scheme, expected, StringComparison.OrdinalIgnoreCase)) {
            Fail(Strings.Format(StringKeys.URI_HAVE_SCHEME, Expression, expected, _value.Scheme), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the absolute URI does not use <paramref name="unexpected"/> as its scheme (case-insensitive).</summary>
    /// <param name="unexpected">The scheme the URI must not use.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="unexpected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The URI is <see langword="null"/>, relative, or uses that scheme.</exception>
    public AndConstraint<UriAssertions> NotHaveScheme(string unexpected, string? because = null, params object?[] becauseArgs) {
        if(unexpected is null) {
            throw new ArgumentNullException(nameof(unexpected));
        }

        if(!EnsureAbsolute(because, becauseArgs)) {
            return new(this);
        }

        if(string.Equals(_value!.Scheme, unexpected, StringComparison.OrdinalIgnoreCase)) {
            Fail(Strings.Format(StringKeys.URI_NOT_HAVE_SCHEME, Expression, unexpected), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the absolute URI has <paramref name="expected"/> as its host (case-insensitive).</summary>
    /// <param name="expected">The expected host; IPv6 brackets are optional.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The URI is <see langword="null"/>, relative, or has another host.</exception>
    public AndConstraint<UriAssertions> HaveHost(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(!EnsureAbsolute(because, becauseArgs)) {
            return new(this);
        }

        var actualHost = NormalizeHost(_value!.Host);
        var expectedHost = NormalizeHost(expected);
        if(!string.Equals(actualHost, expectedHost, StringComparison.OrdinalIgnoreCase)) {
            Fail(Strings.Format(StringKeys.URI_HAVE_HOST, Expression, expected, actualHost), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the absolute URI does not have <paramref name="unexpected"/> as its host (case-insensitive).</summary>
    /// <param name="unexpected">The host the URI must not have; IPv6 brackets are optional.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="unexpected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The URI is <see langword="null"/>, relative, or has that host.</exception>
    public AndConstraint<UriAssertions> NotHaveHost(string unexpected, string? because = null, params object?[] becauseArgs) {
        if(unexpected is null) {
            throw new ArgumentNullException(nameof(unexpected));
        }

        if(!EnsureAbsolute(because, becauseArgs)) {
            return new(this);
        }

        if(string.Equals(NormalizeHost(_value!.Host), NormalizeHost(unexpected), StringComparison.OrdinalIgnoreCase)) {
            Fail(Strings.Format(StringKeys.URI_NOT_HAVE_HOST, Expression, unexpected), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the absolute URI uses <paramref name="expected"/> as its port, including scheme defaults.</summary>
    /// <param name="expected">The expected port (80 for http without an explicit port, 443 for https, and so on).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The URI is <see langword="null"/>, relative, or has another port.</exception>
    public AndConstraint<UriAssertions> HavePort(int expected, string? because = null, params object?[] becauseArgs) {
        if(!EnsureAbsolute(because, becauseArgs)) {
            return new(this);
        }

        if(_value!.Port != expected) {
            Fail(Strings.Format(StringKeys.URI_HAVE_PORT, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_value.Port)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the absolute URI does not use <paramref name="unexpected"/> as its port, including scheme defaults.</summary>
    /// <param name="unexpected">The port the URI must not use.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The URI is <see langword="null"/>, relative, or uses that port.</exception>
    public AndConstraint<UriAssertions> NotHavePort(int unexpected, string? because = null, params object?[] becauseArgs) {
        if(!EnsureAbsolute(because, becauseArgs)) {
            return new(this);
        }

        if(_value!.Port == unexpected) {
            Fail(Strings.Format(StringKeys.URI_NOT_HAVE_PORT, Expression, AssertionScope.FormatInvariant(unexpected)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the absolute URI has the unescaped path <paramref name="expected"/>.</summary>
    /// <remarks>A leading slash is optional in <paramref name="expected"/>; the root path is <c>/</c>.</remarks>
    /// <param name="expected">The expected unescaped path.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The URI is <see langword="null"/>, relative, or has another path.</exception>
    public AndConstraint<UriAssertions> HavePath(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(!EnsureAbsolute(because, becauseArgs)) {
            return new(this);
        }

        var actualPath = PathOf(_value!);
        var expectedPath = NormalizePath(expected);
        if(!string.Equals(actualPath, expectedPath, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.URI_HAVE_PATH, Expression, expectedPath, actualPath), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the absolute URI does not have the unescaped path <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The path the URI must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="unexpected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The URI is <see langword="null"/>, relative, or has that path.</exception>
    public AndConstraint<UriAssertions> NotHavePath(string unexpected, string? because = null, params object?[] becauseArgs) {
        if(unexpected is null) {
            throw new ArgumentNullException(nameof(unexpected));
        }

        if(!EnsureAbsolute(because, becauseArgs)) {
            return new(this);
        }

        if(string.Equals(PathOf(_value!), NormalizePath(unexpected), StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.URI_NOT_HAVE_PATH, Expression, NormalizePath(unexpected)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the absolute URI has the unescaped fragment <paramref name="expected"/>.</summary>
    /// <remarks>A leading <c>#</c> is optional in <paramref name="expected"/>; an empty expected value accepts no fragment.</remarks>
    /// <param name="expected">The expected unescaped fragment.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The URI is <see langword="null"/>, relative, or has another fragment.</exception>
    public AndConstraint<UriAssertions> HaveFragment(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(!EnsureAbsolute(because, becauseArgs)) {
            return new(this);
        }

        var actualFragment = NormalizeFragment(_value!.GetComponents(UriComponents.Fragment, UriFormat.Unescaped));
        var expectedFragment = NormalizeFragment(expected);
        if(!string.Equals(actualFragment, expectedFragment, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.URI_HAVE_FRAGMENT, Expression, expectedFragment, actualFragment), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the absolute URI does not have the unescaped fragment <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The fragment the URI must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="unexpected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The URI is <see langword="null"/>, relative, or has that fragment.</exception>
    public AndConstraint<UriAssertions> NotHaveFragment(string unexpected, string? because = null, params object?[] becauseArgs) {
        if(unexpected is null) {
            throw new ArgumentNullException(nameof(unexpected));
        }

        if(!EnsureAbsolute(because, becauseArgs)) {
            return new(this);
        }

        if(string.Equals(NormalizeFragment(_value!.GetComponents(UriComponents.Fragment, UriFormat.Unescaped)), NormalizeFragment(unexpected), StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.URI_NOT_HAVE_FRAGMENT, Expression, NormalizeFragment(unexpected)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the absolute URI has a query parameter named <paramref name="name"/>.</summary>
    /// <remarks>
    /// Names and values are case-sensitive after unescaping; duplicate names are allowed and the first value is
    /// exposed through <c>Which</c>. A failed presence check throws immediately, even inside an assertion scope,
    /// because the method returns a value.
    /// </remarks>
    /// <param name="name">The query parameter name.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> exposing the first value of the parameter.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The URI is <see langword="null"/>, relative, or lacks the parameter.</exception>
    public WhichConstraint<string?> HaveQueryParameter(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        if(_value is null) {
            throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.URI_NULL, Expression), because, becauseArgs));
        }

        if(!_value.IsAbsoluteUri) {
            throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.URI_REQUIRE_ABSOLUTE, Expression), because, becauseArgs));
        }

        var parameters = QueryParameters(_value);
        var match = parameters.FirstOrDefault(parameter => string.Equals(parameter.Key, name, StringComparison.Ordinal));
        if(match.Key is null) {
            throw new AssertionFailedException(AssertionScope.AppendBecause(MissingQueryMessage(name, parameters), because, becauseArgs));
        }

        return new WhichConstraint<string?>(match.Value);
    }

    /// <summary>Asserts that the absolute URI has a query parameter named <paramref name="name"/> with <paramref name="expected"/>.</summary>
    /// <remarks>Names and values are case-sensitive after unescaping; any duplicate value may match.</remarks>
    /// <param name="name">The query parameter name.</param>
    /// <param name="expected">The expected unescaped value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The URI is <see langword="null"/>, relative, lacks the parameter, or has another value.</exception>
    public AndConstraint<UriAssertions> HaveQueryParameterValue(string name, string expected, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(!EnsureAbsolute(because, becauseArgs)) {
            return new(this);
        }

        var parameters = QueryParameters(_value!);
        var matches = parameters.Where(parameter => string.Equals(parameter.Key, name, StringComparison.Ordinal)).ToList();
        if(matches.Count == 0) {
            Fail(MissingQueryMessage(name, parameters), because, becauseArgs);
            return new(this);
        }

        if(!matches.Any(parameter => string.Equals(parameter.Value, expected, StringComparison.Ordinal))) {
            var actual = string.Join(", ", matches.Select(parameter => parameter.Value));
            Fail(Strings.Format(StringKeys.URI_QUERY_VALUE, Expression, name, expected, actual), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the absolute URI has no query parameter named <paramref name="name"/>.</summary>
    /// <param name="name">The query parameter name that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The URI is <see langword="null"/>, relative, or contains the parameter.</exception>
    public AndConstraint<UriAssertions> NotHaveQueryParameter(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        if(!EnsureAbsolute(because, becauseArgs)) {
            return new(this);
        }

        var values = QueryParameters(_value!)
            .Where(parameter => string.Equals(parameter.Key, name, StringComparison.Ordinal))
            .Select(parameter => parameter.Value)
            .ToList();
        if(values.Count != 0) {
            Fail(Strings.Format(StringKeys.URI_NOT_QUERY, Expression, name, string.Join(", ", values)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the absolute URI is a base of <paramref name="expected"/>.</summary>
    /// <param name="expected">The absolute URI that must be based on the subject.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">Either URI is relative, or the subject is not a base of <paramref name="expected"/>.</exception>
    public AndConstraint<UriAssertions> BeBaseOf(Uri expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(!EnsureAbsolute(because, becauseArgs)) {
            return new(this);
        }

        if(!expected.IsAbsoluteUri) {
            Fail(Strings.Format(StringKeys.URI_BASE_REQUIRES_ABSOLUTE, FormatUri(expected)), because, becauseArgs);
            return new(this);
        }

        if(!IsBaseOf(_value!, expected)) {
            Fail(Strings.Format(StringKeys.URI_BASE_OF, Expression, FormatUri(expected)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the absolute URI is not a base of <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The absolute URI that must not be based on the subject.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="unexpected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">Either URI is relative, or the subject is a base of <paramref name="unexpected"/>.</exception>
    public AndConstraint<UriAssertions> NotBeBaseOf(Uri unexpected, string? because = null, params object?[] becauseArgs) {
        if(unexpected is null) {
            throw new ArgumentNullException(nameof(unexpected));
        }

        if(!EnsureAbsolute(because, becauseArgs)) {
            return new(this);
        }

        if(!unexpected.IsAbsoluteUri) {
            Fail(Strings.Format(StringKeys.URI_BASE_REQUIRES_ABSOLUTE, FormatUri(unexpected)), because, becauseArgs);
            return new(this);
        }

        if(IsBaseOf(_value!, unexpected)) {
            Fail(Strings.Format(StringKeys.URI_NOT_BASE_OF, Expression, FormatUri(unexpected)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Fails when the subject is null or relative, returning <see langword="false"/> when the caller should stop.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns><see langword="true"/> when the subject is absolute.</returns>
    private bool EnsureAbsolute(string? because, object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.URI_NULL, Expression), because, becauseArgs);
            return false;
        }

        if(!_value.IsAbsoluteUri) {
            Fail(Strings.Format(StringKeys.URI_REQUIRE_ABSOLUTE, Expression), because, becauseArgs);
            return false;
        }

        return true;
    }

    /// <summary>Normalizes a path to a leading slash without changing other separators or trailing slashes.</summary>
    /// <param name="path">The path to normalize.</param>
    /// <returns>The normalized path.</returns>
    private static string NormalizePath(string path) => "/" + path.TrimStart('/');

    /// <summary>Reads the unescaped path of <paramref name="uri"/> and normalizes it to a leading slash.</summary>
    /// <param name="uri">The absolute URI.</param>
    /// <returns>The normalized path.</returns>
    private static string PathOf(Uri uri) => NormalizePath(uri.GetComponents(UriComponents.Path, UriFormat.Unescaped));

    /// <summary>
    /// Determines whether <paramref name="baseUri"/> is a path-boundary base of <paramref name="candidate"/>.
    /// </summary>
    /// <param name="baseUri">The absolute base URI.</param>
    /// <param name="candidate">The absolute candidate URI.</param>
    /// <returns><see langword="true"/> when scheme, host, port, and the path prefix (with a segment boundary) match.</returns>
    /// <remarks><see cref="Uri.IsBaseOf(Uri)"/> treats <c>/orders-archive</c> as based on <c>/orders</c>; this helper requires a path-segment boundary.</remarks>
    private static bool IsBaseOf(Uri baseUri, Uri candidate) {
        if(!string.Equals(baseUri.Scheme, candidate.Scheme, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(NormalizeHost(baseUri.Host), NormalizeHost(candidate.Host), StringComparison.OrdinalIgnoreCase)
            || baseUri.Port != candidate.Port) {
            return false;
        }

        var basePath = PathOf(baseUri);
        var candidatePath = PathOf(candidate);
        if(!candidatePath.StartsWith(basePath, StringComparison.Ordinal)) {
            return false;
        }

        if(candidatePath.Length == basePath.Length || basePath[basePath.Length - 1] == '/') {
            return true;
        }

        return candidatePath[basePath.Length] == '/';
    }

    /// <summary>Normalizes an optional leading hash from a fragment.</summary>
    /// <param name="fragment">The fragment text.</param>
    /// <returns>The fragment without a leading <c>#</c>.</returns>
    private static string NormalizeFragment(string fragment) => fragment.TrimStart('#');

    /// <summary>Normalizes an optional IPv6 bracket pair and surrounding whitespace from a host.</summary>
    /// <param name="host">The host text.</param>
    /// <returns>The host without surrounding brackets or whitespace.</returns>
    private static string NormalizeHost(string host) => host.Trim().Trim('[', ']');

    /// <summary>Parses the query string into decoded name/value pairs, preserving duplicates and order.</summary>
    /// <param name="uri">The absolute URI.</param>
    /// <returns>The decoded query parameters.</returns>
    private static List<KeyValuePair<string, string>> QueryParameters(Uri uri) {
        var parameters = new List<KeyValuePair<string, string>>();
        var query = uri.Query;
        if(query.Length <= 1) {
            return parameters;
        }

        foreach(var pair in query.Substring(1).Split('&')) {
            if(pair.Length == 0) {
                continue;
            }

            var separator = pair.IndexOf('=');
            var name = separator >= 0 ? pair.Substring(0, separator) : pair;
            var value = separator >= 0 ? pair.Substring(separator + 1) : string.Empty;
            parameters.Add(new KeyValuePair<string, string>(DecodeQueryComponent(name), DecodeQueryComponent(value)));
        }

        return parameters;
    }

    /// <summary>Unescapes a query component and treats <c>+</c> as a space.</summary>
    /// <param name="value">The escaped component.</param>
    /// <returns>The decoded component, or the original text when it is not valid escaping.</returns>
    private static string DecodeQueryComponent(string value) {
        try {
            return Uri.UnescapeDataString(value.Replace("+", " "));
        }
        catch(UriFormatException) {
            return value;
        }
    }

    /// <summary>Builds the missing-query-parameter message with available names and a typo suggestion.</summary>
    /// <param name="name">The missing parameter name.</param>
    /// <param name="parameters">The parsed query parameters.</param>
    /// <returns>The formatted message.</returns>
    private string MissingQueryMessage(string name, IReadOnlyList<KeyValuePair<string, string>> parameters) {
        var names = parameters.Select(parameter => parameter.Key).Distinct(StringComparer.Ordinal).ToList();
        var available = names.Count == 0 ? "-" : string.Join(", ", names.Take(10));
        if(names.Count > 10) {
            available += ", …";
        }

        return Strings.Format(StringKeys.URI_QUERY_MISSING, Expression, name, available) + NearestCandidate.Describe(name, names);
    }
}
