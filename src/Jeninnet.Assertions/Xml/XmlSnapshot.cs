//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Xml;

/// <summary>Shared snapshot verification for XML document and element assertions.</summary>
internal static class XmlSnapshot {
    /// <summary>Formats the XML value with LF endings and verifies it against the committed snapshot.</summary>
    /// <param name="value">The XML document or element to snapshot.</param>
    /// <param name="snapshotName">The snapshot name, or <see langword="null"/> to derive it from the expression.</param>
    /// <param name="snapshotsDirectory">The snapshots folder, or <see langword="null"/> for the caller-relative default.</param>
    /// <param name="config">Optional snapshot configuration.</param>
    /// <param name="context">The caller context for the snapshot.</param>
    internal static void Verify(
        object? value,
        string? snapshotName,
        string? snapshotsDirectory,
        Func<SnapshotOptions, SnapshotOptions>? config,
        SnapshotContext context) {
        var options = config?.Invoke(new SnapshotOptions()) ?? new SnapshotOptions();
        var xml = value?.ToString() ?? "<null>";
        Snapshot.Verify(xml.Replace("\r\n", "\n").Replace('\r', '\n'), snapshotName, snapshotsDirectory, ".xml", options, context);
    }
}
