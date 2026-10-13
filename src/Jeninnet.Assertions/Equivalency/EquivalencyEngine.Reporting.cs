//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Equivalency;

internal static partial class EquivalencyEngine {
    private sealed partial class Run<TExp> {
        /// <summary>Records a failure with no resolved path (used for whole-graph messages).</summary>
        /// <param name="detail">The failure sentence.</param>
        private void Fail(string detail) {
            Failures.Add(detail);
            Entries.Add(new EquivalencyDifference(string.Empty, detail, null, null));
        }

        /// <summary>Records a failure with a resolved path and formatted expected/actual values.</summary>
        /// <param name="detail">The failure sentence.</param>
        /// <param name="path">The member path from the root.</param>
        /// <param name="expected">The formatted expected value, when available.</param>
        /// <param name="actual">The formatted actual value, when available.</param>
        private void Fail(string detail, List<string> path, string? expected = null, string? actual = null) {
            Failures.Add(detail);
            Entries.Add(new EquivalencyDifference(FormatPath(path), detail, expected, actual));
        }

        /// <summary>Applies the effective date/time or numeric tolerance for <paramref name="path"/> to one same-typed pair.</summary>
        /// <remarks>
        /// Date/time values compare as instants (<see cref="DateTime.ToUniversalTime"/> / the
        /// <see cref="DateTimeOffset"/> instant); floating-point and <see cref="decimal"/> values compare by
        /// absolute difference. Custom steps run before this check and keep ownership when they handle the node.
        /// </remarks>
        /// <param name="subject">The actual value.</param>
        /// <param name="expectation">The expected value.</param>
        /// <param name="path">The member path from the root.</param>
        /// <param name="within">Whether the pair counts as equal under the tolerance.</param>
        /// <param name="note">The appended exceeded-tolerance sentence, or an empty string when within.</param>
        /// <returns><see langword="true"/> when a tolerance applied to this pair.</returns>
        private bool TryTolerance(object subject, object expectation, List<string> path, out bool within, out string note) {
            within = false;
            note = string.Empty;

            var timeTolerance = _options.DateTimeToleranceForPath(path);
            if(timeTolerance is not null) {
                if(subject is DateTime subjectDate && expectation is DateTime expectationDate) {
                    return TimeWithinTolerance(subjectDate.ToUniversalTime() - expectationDate.ToUniversalTime(), timeTolerance.Value, out within, out note);
                }

                if(subject is DateTimeOffset subjectOffset && expectation is DateTimeOffset expectationOffset) {
                    return TimeWithinTolerance(subjectOffset - expectationOffset, timeTolerance.Value, out within, out note);
                }
            }

            var numericTolerance = _options.NumericToleranceForPath(path);
            if(numericTolerance is null) {
                return false;
            }

            if(subject is decimal subjectDecimal) {
                var difference = Math.Abs(subjectDecimal - (decimal)expectation);
                within = difference <= (decimal)numericTolerance.Value;
                note = within ? string.Empty : ToleranceNote(difference, (decimal)numericTolerance.Value);
                return true;
            }

            if(subject is float or double) {
                var difference = Math.Abs(Convert.ToDouble(subject, CultureInfo.InvariantCulture) - Convert.ToDouble(expectation, CultureInfo.InvariantCulture));
                within = difference <= numericTolerance.Value;
                note = within ? string.Empty : ToleranceNote(difference, numericTolerance.Value);
                return true;
            }

            return false;
        }

        /// <summary>Applies a time tolerance to a signed difference and renders the exceeded-tolerance note.</summary>
        /// <param name="difference">The signed difference between the values.</param>
        /// <param name="tolerance">The allowed absolute difference.</param>
        /// <param name="within">Whether the difference is within the tolerance.</param>
        /// <param name="note">The exceeded-tolerance sentence, or an empty string when within.</param>
        /// <returns>Always <see langword="true"/>; the method only runs for time pairs.</returns>
        private static bool TimeWithinTolerance(TimeSpan difference, TimeSpan tolerance, out bool within, out string note) {
            var magnitude = difference.Duration();
            within = magnitude <= tolerance;
            note = within ? string.Empty : ToleranceNote(magnitude, tolerance);
            return true;
        }

        /// <summary>Renders the sentence appended when a value exceeds the configured tolerance.</summary>
        /// <param name="difference">The measured absolute difference.</param>
        /// <param name="tolerance">The allowed tolerance.</param>
        /// <returns>The diagnostic sentence (invariant formatting; never localized).</returns>
        private static string ToleranceNote(object difference, object tolerance) =>
            $" The difference {AssertionScope.FormatInvariant(difference)} exceeds the allowed tolerance {AssertionScope.FormatInvariant(tolerance)}.";

        /// <summary>Returns a new path extended with one segment, leaving the parent untouched.</summary>
        /// <param name="path">The parent path.</param>
        /// <param name="segment">The child segment (member name or index).</param>
        /// <returns>The extended path.</returns>
        private static List<string> ChildPath(List<string> path, string segment) =>
            [.. path, segment];

        /// <summary>Builds the difference sentence for a scalar mismatch.</summary>
        /// <param name="path">The member path from the root.</param>
        /// <param name="subject">The actual value.</param>
        /// <param name="expectation">The expected value.</param>
        /// <returns>The root- or member-scoped sentence with both values formatted.</returns>
        private static string ValueDetail(List<string> path, object? subject, object? expectation) =>
            path.Count == 0
                ? $"found {FormatValue(subject)} instead of {FormatValue(expectation)}."
                : $"{MemberPath.Format(path)} was {FormatValue(subject)} instead of {FormatValue(expectation)}.";

        /// <summary>Formats a string mismatch, honoring whether the full text must be included in the report.</summary>
        /// <param name="path">The member path from the root.</param>
        /// <param name="subject">The actual text.</param>
        /// <param name="expectation">The expected text.</param>
        /// <returns>The difference sentence.</returns>
        private string StringValueDetail(List<string> path, string subject, string expectation) =>
            path.Count == 0
                ? $"found {FormatStringValue(subject)} instead of {FormatStringValue(expectation)}."
                : $"{MemberPath.Format(path)} was {FormatStringValue(subject)} instead of {FormatStringValue(expectation)}.";

        /// <summary>Formats a string value for the report, truncating unless full strings are requested.</summary>
        /// <param name="value">The string to format.</param>
        /// <returns>The quoted value.</returns>
        private string FormatStringValue(string value) =>
            _options.IncludeFullStringsInDifference ? $"'{value}'" : $"'{Truncate(value)}'";

        /// <summary>Builds the difference sentence for a runtime-type mismatch.</summary>
        /// <param name="path">The member path from the root.</param>
        /// <param name="subjectType">The actual runtime type.</param>
        /// <param name="expectationType">The expected runtime type.</param>
        /// <returns>The root- or member-scoped sentence naming both types.</returns>
        private static string TypeDetail(List<string> path, Type subjectType, Type expectationType) {
            var subjectName = subjectType.FullName ?? subjectType.Name;
            var expectationName = expectationType.FullName ?? expectationType.Name;
            return path.Count == 0
                ? $"found an instance of type {subjectName} instead of {expectationName}."
                : $"{MemberPath.Format(path)} was of type {subjectName} instead of {expectationName}.";
        }

        /// <summary>Builds the difference sentence for a collection or dictionary count mismatch.</summary>
        /// <param name="path">The member path from the root.</param>
        /// <param name="actual">The actual count.</param>
        /// <param name="expected">The expected count.</param>
        /// <returns>The root- or member-scoped sentence with both counts.</returns>
        private static string CountDetail(List<string> path, int actual, int expected) =>
            path.Count == 0
                ? $"found {AssertionScope.FormatInvariant(actual)} item(s) instead of {AssertionScope.FormatInvariant(expected)}."
                : $"{MemberPath.Format(path)} contains {AssertionScope.FormatInvariant(actual)} item(s) instead of {AssertionScope.FormatInvariant(expected)}.";

        /// <summary>Builds the difference sentence for an item with no counterpart in the other graph.</summary>
        /// <param name="path">The member path from the root.</param>
        /// <param name="expectation">The unmatched value.</param>
        /// <returns>The root- or member-scoped sentence naming the unmatched value.</returns>
        private static string NoEquivalentDetail(List<string> path, object? expectation) =>
            path.Count == 0
                ? $"found no equivalent item for {FormatValue(expectation)}."
                : $"{MemberPath.Format(path)} has no equivalent item for {FormatValue(expectation)}.";

        /// <summary>Formats a value for failure messages with stable literal conventions.</summary>
        /// <param name="value">The value to format.</param>
        /// <returns><c>&lt;null&gt;</c>, a quoted string or char, lowercase booleans, or invariant formatting.</returns>
        private static string FormatValue(object? value) => value switch {
            null => "<null>",
            string text => $"'{text}'",
            char c => $"'{c}'",
            bool b => b ? "true" : "false",
            _ => AssertionScope.FormatInvariant(value),
        };

        /// <summary>Formats a dictionary key for path rendering.</summary>
        /// <param name="key">The key to format.</param>
        /// <returns>Quoted strings as <c>["key"]</c>, nulls as <c>[&lt;null&gt;]</c>, others invariantly bracketed.</returns>
        private static string FormatKey(object? key) => key switch {
            string text => $"[\"{text}\"]",
            null => "[<null>]",
            _ => $"[{AssertionScope.FormatInvariant(key)}]",
        };

        /// <summary>Enumerates a dictionary's entries whether it implements the generic or only the non-generic interface.</summary>
        /// <param name="value">The dictionary instance.</param>
        /// <param name="entries">The collected key/value pairs when the value is a dictionary.</param>
        /// <returns><see langword="true"/> when the value is a dictionary whose entries were collected.</returns>
        private static bool TryGetDictionaryEntries(object value, out List<(object? Key, object? Value)> entries) {
            entries = [];
            if(value is IDictionary nonGeneric) {
                foreach(DictionaryEntry entry in nonGeneric) {
                    entries.Add((entry.Key, entry.Value));
                }

                return true;
            }

            var entryType = value.GetType().GetInterfaces()
                .FirstOrDefault(static i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>));
            if(entryType is null) {
                return false;
            }

            foreach(var element in (IEnumerable)value) {
                var elementType = element?.GetType();
                var keyProperty = elementType?.GetProperty("Key");
                var valueProperty = elementType?.GetProperty("Value");
                if(keyProperty is null || valueProperty is null) {
                    return false;
                }

                entries.Add((keyProperty.GetValue(element), valueProperty.GetValue(element)));
            }

            return true;
        }
    }
}
