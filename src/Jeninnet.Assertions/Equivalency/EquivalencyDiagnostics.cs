//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Equivalency;

/// <summary>Builds the diagnostics suffix appended to equivalency failure messages when tracing or full dumps are configured.</summary>
internal static class EquivalencyDiagnostics {
    /// <summary>Appends the configuration description and the optional full dumps to a failure message.</summary>
    /// <typeparam name="TSubject">The subject graph type.</typeparam>
    /// <typeparam name="TExpectation">The expectation graph type.</typeparam>
    /// <param name="message">The failure message so far.</param>
    /// <param name="options">The equivalency configuration.</param>
    /// <param name="subject">The actual object graph.</param>
    /// <param name="expectation">The expected object graph.</param>
    /// <returns><paramref name="message"/> with diagnostics appended when tracing or dumping is on.</returns>
    internal static string Append<TSubject, TExpectation>(
        string message,
        SelfReferenceEquivalencyOptions<TExpectation> options,
        TSubject? subject,
        TExpectation? expectation) {
        if(options.TraceWriter is null && !options.FullDump) {
            return message;
        }

        var builder = new StringBuilder(message.TrimEnd());
        builder.AppendLine().Append(options.DescribeConfiguration());
        if(options.FullDump) {
            builder.AppendLine().Append("Full dump of subject:").AppendLine().Append(EquivalencyEngine.DumpGraph(subject));
            builder.AppendLine().Append("Full dump of expectation:").AppendLine().Append(EquivalencyEngine.DumpGraph(expectation));
        }

        return builder.ToString();
    }
}
