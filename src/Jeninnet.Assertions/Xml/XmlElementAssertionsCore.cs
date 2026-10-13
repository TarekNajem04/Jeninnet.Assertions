//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Xml;

/// <summary>Shared element lookup and failure plumbing for the <see cref="XDocument"/> and <see cref="XElement"/> assertion classes.</summary>
internal static class XmlElementAssertionsCore {
    /// <summary>Counts the matching elements against the occurrence constraint, failing when it is not satisfied.</summary>
    /// <param name="matches">The matching elements.</param>
    /// <param name="expression">The subject expression used in the failure message.</param>
    /// <param name="displayName">The element name as displayed in failure messages.</param>
    /// <param name="occurrence">The expected occurrence constraint.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping every matching element.</returns>
    /// <exception cref="AssertionFailedException">The matching elements do not satisfy <paramref name="occurrence"/>.</exception>
    internal static WhichConstraint<IReadOnlyList<XElement>> RequireElements(
        IReadOnlyList<XElement> matches,
        string expression,
        string displayName,
        OccurrenceConstraint occurrence,
        string? because,
        object?[]? becauseArgs) {
        if(!occurrence.Matches(matches.Count)) {
            throw new AssertionFailedException(XmlOccurrence.FailureMessage(expression, displayName, occurrence, matches.Count, because, becauseArgs));
        }

        return new WhichConstraint<IReadOnlyList<XElement>>([.. matches]);
    }

    /// <summary>Finds the first matching element whose value equals the expectation, failing otherwise.</summary>
    /// <param name="matches">The matching elements.</param>
    /// <param name="expression">The subject expression used in the failure message.</param>
    /// <param name="displayName">The element name as displayed in failure messages.</param>
    /// <param name="expected">The expected element value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the first matching element.</returns>
    /// <exception cref="AssertionFailedException">No matching element has the expected value.</exception>
    internal static WhichConstraint<XElement> HaveElementWithValue(
        IReadOnlyList<XElement> matches,
        string expression,
        string displayName,
        string? expected,
        string? because,
        object?[]? becauseArgs) {
        var match = matches.FirstOrDefault(candidate => string.Equals(candidate.Value, expected, StringComparison.Ordinal));
        if(match is null) {
            var firstValue = matches.Count > 0 ? matches[0].Value : null;
            throw new AssertionFailedException(
                AssertionScope.AppendBecause(
                    Strings.Format(StringKeys.XML_HAVE_ELEMENT_WITH_VALUE, expression, displayName, expected, firstValue),
                    because,
                    becauseArgs));
        }

        return new WhichConstraint<XElement>(match);
    }

    /// <summary>Resolves a simple element name and validates the occurrence constraint.</summary>
    /// <param name="name">The local element name.</param>
    /// <param name="find">The matcher producing the candidate elements for a qualified name.</param>
    /// <param name="expression">The subject expression used in the failure message.</param>
    /// <param name="occurrence">The expected occurrence constraint.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping every matching element.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The matching elements do not satisfy <paramref name="occurrence"/>.</exception>
    internal static WhichConstraint<IReadOnlyList<XElement>> RequireElementsByName(
        string name,
        Func<XName, List<XElement>> find,
        string expression,
        OccurrenceConstraint occurrence,
        string? because,
        object?[]? becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        return RequireElements(find(XNamespace.None + name), expression, name, occurrence, because, becauseArgs);
    }

    /// <summary>Validates the occurrence constraint for a qualified element name.</summary>
    /// <param name="name">The qualified element name.</param>
    /// <param name="find">The matcher producing the candidate elements for a qualified name.</param>
    /// <param name="expression">The subject expression used in the failure message.</param>
    /// <param name="occurrence">The expected occurrence constraint.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping every matching element.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The matching elements do not satisfy <paramref name="occurrence"/>.</exception>
    internal static WhichConstraint<IReadOnlyList<XElement>> RequireElementsByXName(
        XName name,
        Func<XName, List<XElement>> find,
        string expression,
        OccurrenceConstraint occurrence,
        string? because,
        object?[]? becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        return RequireElements(find(name), expression, name.ToString(), occurrence, because, becauseArgs);
    }

    /// <summary>Resolves a simple element name and finds the first match with the expected value.</summary>
    /// <param name="name">The local element name.</param>
    /// <param name="find">The matcher producing the candidate elements for a qualified name.</param>
    /// <param name="expression">The subject expression used in the failure message.</param>
    /// <param name="expected">The expected element value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the first matching element.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">No matching element has the expected value.</exception>
    internal static WhichConstraint<XElement> HaveElementWithValueByName(
        string name,
        Func<XName, List<XElement>> find,
        string expression,
        string? expected,
        string? because,
        object?[]? becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        return HaveElementWithValue(find(XNamespace.None + name), expression, name, expected, because, becauseArgs);
    }

    /// <summary>Finds the first match with the expected value for a qualified element name.</summary>
    /// <param name="name">The qualified element name.</param>
    /// <param name="find">The matcher producing the candidate elements for a qualified name.</param>
    /// <param name="expression">The subject expression used in the failure message.</param>
    /// <param name="expected">The expected element value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the first matching element.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">No matching element has the expected value.</exception>
    internal static WhichConstraint<XElement> HaveElementWithValueByXName(
        XName name,
        Func<XName, List<XElement>> find,
        string expression,
        string? expected,
        string? because,
        object?[]? becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        return HaveElementWithValue(find(name), expression, name.ToString(), expected, because, becauseArgs);
    }

    /// <summary>Reports a failure when a simple-named descendant carries the unexpected value.</summary>
    /// <param name="name">The local element name.</param>
    /// <param name="find">The matcher producing the candidate elements for a qualified name.</param>
    /// <param name="expression">The subject expression used in the failure message.</param>
    /// <param name="unexpected">The value that must be absent.</param>
    /// <param name="fail">The failure sink of the calling assertion class.</param>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    internal static void RequireNotHaveElementWithValueByName(
        string name,
        Func<XName, List<XElement>> find,
        string expression,
        string? unexpected,
        Action<string> fail) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        RequireNotHaveElementWithValue(find(XNamespace.None + name), expression, name, unexpected, fail);
    }

    /// <summary>Reports a failure when a qualified-named descendant carries the unexpected value.</summary>
    /// <param name="name">The qualified element name.</param>
    /// <param name="find">The matcher producing the candidate elements for a qualified name.</param>
    /// <param name="expression">The subject expression used in the failure message.</param>
    /// <param name="unexpected">The value that must be absent.</param>
    /// <param name="fail">The failure sink of the calling assertion class.</param>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    internal static void RequireNotHaveElementWithValueByXName(
        XName name,
        Func<XName, List<XElement>> find,
        string expression,
        string? unexpected,
        Action<string> fail) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        RequireNotHaveElementWithValue(find(name), expression, name.ToString(), unexpected, fail);
    }

    /// <summary>Reports a failure when any matching element carries the unexpected value.</summary>
    /// <param name="matches">The matching elements.</param>
    /// <param name="expression">The subject expression used in the failure message.</param>
    /// <param name="displayName">The element name as displayed in failure messages.</param>
    /// <param name="unexpected">The value that must be absent.</param>
    /// <param name="fail">The failure sink of the calling assertion class.</param>
    internal static void RequireNotHaveElementWithValue(
        IReadOnlyList<XElement> matches,
        string expression,
        string displayName,
        string? unexpected,
        Action<string> fail) {
        if(matches.Any(candidate => string.Equals(candidate.Value, unexpected, StringComparison.Ordinal))) {
            fail(Strings.Format(StringKeys.XML_NOT_HAVE_ELEMENT_WITH_VALUE, expression, displayName, unexpected));
        }
    }
}
