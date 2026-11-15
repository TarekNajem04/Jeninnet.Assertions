//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Xml;

public partial class XDocumentAssertions {
    /// <summary>Asserts that the formatted document matches the committed XML snapshot.</summary>
    /// <remarks>XML snapshots are written with LF line endings; use <see cref="Snapshots.SnapshotOptions.IndentSize"/> to adjust indentation.</remarks>
    /// <param name="snapshotName">The snapshot name (<c>{name}.verified.xml</c>), or <see langword="null"/> to derive it from the subject expression.</param>
    /// <param name="snapshotsDirectory">The snapshots folder, or <see langword="null"/> for <c>__snapshots__</c> next to the calling source file.</param>
    /// <param name="config">Optional snapshot configuration.</param>
    /// <param name="callerFilePath">The calling source file (provided by the compiler).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentException">The snapshot name or directory is unsafe.</exception>
    /// <exception cref="AssertionFailedException">No verified snapshot exists, or the content differs.</exception>
    [RequiresUnreferencedCode(TrimmingMessages.SNAPSHOT)]
    public AndConstraint<XDocumentAssertions> MatchSnapshot(
        string? snapshotName = null,
        string? snapshotsDirectory = null,
        Func<SnapshotOptions, SnapshotOptions>? config = null,
        [CallerFilePath] string? callerFilePath = null,
        string? because = null,
        params object?[] becauseArgs) {
        var options = config?.Invoke(new SnapshotOptions()) ?? new SnapshotOptions();
        var xml = _value?.ToString() ?? "<null>";
        Snapshot.Verify(xml.Replace("\r\n", "\n").Replace('\r', '\n'), snapshotName, snapshotsDirectory, ".xml", options, new(Expression, callerFilePath, because, becauseArgs));
        return new(this);
    }
}
