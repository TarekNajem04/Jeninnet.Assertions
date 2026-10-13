//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Xml;

/// <summary>Formats XML element occurrence failures with constraint-specific messages.</summary>
internal static class XmlOccurrence {
    /// <summary>Creates the full failure message for an unsatisfied element occurrence constraint.</summary>
    /// <param name="expression">The caller expression naming the subject under test.</param>
    /// <param name="name">The searched element name.</param>
    /// <param name="occurrence">The expected occurrence constraint.</param>
    /// <param name="actual">The actual number of matching elements.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>The failure message with any <c>because</c> clause appended.</returns>
    internal static string FailureMessage(string expression, string name, OccurrenceConstraint occurrence, int actual, string? because, object?[]? becauseArgs) {
        var detail = (occurrence.Minimum, occurrence.Maximum) switch {
            (1, 1) => Strings.Format(StringKeys.XML_HAVE_ELEMENT_ONCE, expression, name, AssertionScope.FormatInvariant(actual)),
            (1, int.MaxValue) => Strings.Format(StringKeys.XML_HAVE_ELEMENT_AT_LEAST_ONCE, expression, name, AssertionScope.FormatInvariant(actual)),
            (var minimum, var maximum) when minimum == maximum => Strings.Format(
                StringKeys.XML_HAVE_ELEMENT_EXACTLY,
                expression,
                name,
                AssertionScope.FormatInvariant(minimum),
                AssertionScope.FormatInvariant(actual)),
            (var minimum, int.MaxValue) => Strings.Format(
                StringKeys.XML_HAVE_ELEMENT_AT_LEAST,
                expression,
                name,
                AssertionScope.FormatInvariant(minimum),
                AssertionScope.FormatInvariant(actual)),
            (0, var maximum) => Strings.Format(
                StringKeys.XML_HAVE_ELEMENT_AT_MOST,
                expression,
                name,
                AssertionScope.FormatInvariant(maximum),
                AssertionScope.FormatInvariant(actual)),
            _ => Strings.Format(
                StringKeys.XML_HAVE_ELEMENT_OCCURRENCE,
                expression,
                name,
                occurrence.Expectation,
                AssertionScope.FormatInvariant(actual)),
        };

        return AssertionScope.AppendBecause(detail, because, becauseArgs);
    }
}
