//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Equivalency;

/// <summary>Recursive structural comparison engine behind object-graph <c>BeEquivalentTo</c> assertions.</summary>
/// <remarks>
/// Rules: custom <see cref="IEquivalencyStep"/> rules run first (the first handling step owns the node);
/// reference-equal values pass; null matches only null; registered by-value types and simple types
/// (primitives, strings, decimals, date/time, guids, enums, delegates, <see cref="Type"/> instances) compare
/// with <see cref="object.Equals(object?, object?)"/>; dictionaries compare by key; other enumerables compare
/// as multisets by default (or index-wise with strict ordering); other types compare memberwise over
/// the expectation's runtime-type public instance properties and fields (extra subject members are ignored,
/// missing members fail, regardless of the subject's runtime type). Member getters that throw let
/// the exception propagate. Cyclic references fail unless <c>AllowingInfiniteRecursion</c> or
/// <c>IgnoringCyclicReferences</c> is set (the two are mutually exclusive; the last call wins).
/// Tracing lines and full dumps are invariant diagnostics prose, never localized.
/// </remarks>
internal static partial class EquivalencyEngine {
    /// <summary>Compares a subject graph against an expectation graph.</summary>
    /// <typeparam name="TExpectation">The compile-time type of the expectation.</typeparam>
    /// <param name="subject">The actual object graph.</param>
    /// <param name="expectation">The expected object graph.</param>
    /// <param name="options">The equivalency configuration.</param>
    /// <returns>Failure details (without trailing subject prefix); empty when equivalent.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is <see langword="null"/>.</exception>
    internal static List<string> FindDifferences<TExpectation>(
        object? subject,
        TExpectation? expectation,
        SelfReferenceEquivalencyOptions<TExpectation> options) =>
        RunComparison(subject, expectation, options).Failures;

    /// <summary>Compares a subject graph against an expectation graph and returns the structured differences (message plus resolved path and formatted values).</summary>
    /// <typeparam name="TExpectation">The compile-time type of the expectation.</typeparam>
    /// <param name="subject">The actual object graph.</param>
    /// <param name="expectation">The expected object graph.</param>
    /// <param name="options">The equivalency configuration.</param>
    /// <returns>The differences in comparison order; empty when equivalent.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is <see langword="null"/>.</exception>
    internal static List<EquivalencyDifference> FindDifferenceEntries<TExpectation>(
        object? subject,
        TExpectation? expectation,
        SelfReferenceEquivalencyOptions<TExpectation> options) =>
        RunComparison(subject, expectation, options).Entries;

    /// <summary>Creates a comparison run with a plan snapshot and executes it against the graphs.</summary>
    /// <typeparam name="TExpectation">The compile-time type of the expectation.</typeparam>
    /// <param name="subject">The actual object graph.</param>
    /// <param name="expectation">The expected object graph.</param>
    /// <param name="options">The equivalency configuration.</param>
    /// <returns>The completed run, holding failures, structured entries, and trace output.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is <see langword="null"/>.</exception>
    private static Run<TExpectation> RunComparison<TExpectation>(
        object? subject,
        TExpectation? expectation,
        SelfReferenceEquivalencyOptions<TExpectation> options) {
        if(options is null) {
            throw new ArgumentNullException(nameof(options));
        }

        var run = new Run<TExpectation>(options, options.Plan.Snapshot(), options.TraceWriter);
        run.Compare(subject, expectation, []);
        return run;
    }

    /// <summary>Dumps an object graph for failure-message diagnostics.</summary>
    /// <param name="value">The graph root.</param>
    /// <returns>The indented dump (capped in depth; cycles and throwing getters are marked, not followed).</returns>
    internal static string DumpGraph(object? value) {
        var builder = new StringBuilder();
        DumpInto(builder, value, 0, [], 8);
        return builder.ToString();
    }

    /// <summary>Appends one node of the graph to the dump builder, recursing into dictionaries, enumerables, and members.</summary>
    /// <param name="builder">The destination builder.</param>
    /// <param name="value">The node to dump.</param>
    /// <param name="depth">The current indentation depth.</param>
    /// <param name="ancestors">The nodes on the current path, used to mark cycles without following them.</param>
    /// <param name="remaining">The remaining depth budget; zero renders <c>&lt;max-depth&gt;</c>.</param>
    private static void DumpInto(StringBuilder builder, object? value, int depth, List<object> ancestors, int remaining) {
        if(value is null) {
            builder.Append("<null>");
            return;
        }

        if(value is string text) {
            builder.Append('\'').Append(Truncate(text)).Append('\'');
            return;
        }

        var type = value.GetType();
        if(IsSimpleType(type)) {
            builder.Append(AssertionScope.FormatInvariant(value));
            return;
        }

        if(ancestors.Any(ancestor => ReferenceEquals(ancestor, value))) {
            builder.Append("<cyclic>");
            return;
        }

        if(remaining <= 0) {
            builder.Append("<max-depth>");
            return;
        }

        if(value is System.Collections.IDictionary dictionary) {
            builder.Append(type.Name).Append(" {");
            ancestors.Add(value);
            foreach(System.Collections.DictionaryEntry entry in dictionary) {
                builder.AppendLine().Append(new string(' ', (depth + 1) * 2)).Append('[').Append(Truncate(AssertionScope.FormatInvariant(entry.Key))).Append("]: ");
                DumpInto(builder, entry.Value, depth + 1, ancestors, remaining - 1);
            }

            ancestors.RemoveAt(ancestors.Count - 1);
            builder.AppendLine().Append(new string(' ', depth * 2)).Append('}');
            return;
        }

        if(value is System.Collections.IEnumerable items) {
            builder.Append(type.Name).Append(" [");
            ancestors.Add(value);
            var index = 0;
            foreach(var item in items) {
                builder.AppendLine().Append(new string(' ', (depth + 1) * 2)).Append('[').Append(AssertionScope.FormatInvariant(index)).Append("]: ");
                DumpInto(builder, item, depth + 1, ancestors, remaining - 1);
                index++;
            }

            ancestors.RemoveAt(ancestors.Count - 1);
            builder.AppendLine().Append(new string(' ', depth * 2)).Append(']');
            return;
        }

        builder.Append(type.Name).Append(" {");
        ancestors.Add(value);
        foreach(var member in DumpMembers(type)) {
            object? memberValue;
            try {
                memberValue = member.GetValue(value);
            }
            catch(Exception ex) {
                builder.AppendLine().Append(new string(' ', (depth + 1) * 2)).Append(member.Name).Append(": <threw ").Append(ex.GetType().Name).Append('>');
                continue;
            }

            builder.AppendLine().Append(new string(' ', (depth + 1) * 2)).Append(member.Name).Append(": ");
            DumpInto(builder, memberValue, depth + 1, ancestors, remaining - 1);
        }

        ancestors.RemoveAt(ancestors.Count - 1);
        builder.AppendLine().Append(new string(' ', depth * 2)).Append('}');
    }

    /// <summary>Lists the public instance properties and fields a dump should render for a type, sorted by name.</summary>
    /// <param name="type">The runtime type being dumped.</param>
    /// <returns>Readable, index-free property accessors followed by field accessors, in ordinal name order.</returns>
    private static List<MemberAccessor> DumpMembers(Type type) {
        var members = new List<MemberAccessor>();
        foreach(var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
            if(!property.CanRead || property.GetIndexParameters().Length != 0) {
                continue;
            }

            members.Add(MemberAccessor.ForProperty(property));
        }

        foreach(var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance)) {
            members.Add(MemberAccessor.ForField(field));
        }

        members.Sort(static (left, right) => string.CompareOrdinal(left.Name, right.Name));
        return members;
    }

    // string.Remove keeps one code path on every target (AsSpan is unavailable on .NET Standard 2.0).

    /// <summary>Shortens a string for diagnostics, appending a marker when it was cut.</summary>
    /// <param name="value">The string to shorten.</param>
    /// <returns>The original string, or its first 200 characters plus <c>...(truncated)</c>.</returns>
    private static string Truncate(string value) =>
        value.Length > 200 ? value.Remove(200) + "...(truncated)" : value;

    /// <summary>Determines whether a type compares atomically with <see cref="object.Equals(object?, object?)"/>.</summary>
    /// <param name="type">The type to inspect (nullables are unwrapped first).</param>
    /// <returns><see langword="true"/> for primitives, enums, strings, decimals, date/time types, guids,
    /// <see cref="Type"/> instances, and delegates.</returns>
    private static bool IsSimpleType(Type type) {
        type = Nullable.GetUnderlyingType(type) ?? type;
        return type.IsPrimitive
            || type.IsEnum
            || type == typeof(string)
            || type == typeof(decimal)
            || type == typeof(DateTime)
#if NET6_0_OR_GREATER
            || type == typeof(DateOnly)
            || type == typeof(TimeOnly)
#endif
            || type == typeof(DateTimeOffset)
            || type == typeof(TimeSpan)
            || type == typeof(Guid)
            || type == typeof(Type)
            || typeof(Delegate).IsAssignableFrom(type);
    }
}
