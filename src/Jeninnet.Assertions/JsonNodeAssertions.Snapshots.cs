//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class JsonNodeAssertions {
    /// <summary>Asserts that the JSON representation matches the committed snapshot.</summary>
    /// <remarks>JSON is written with LF line endings. Enable <see cref="Snapshots.SnapshotOptions.SortProperties"/>
    /// and <see cref="Snapshots.SnapshotOptions.IndentSize"/> for deterministic ordering/formatting.</remarks>
    /// <param name="snapshotName">The snapshot name (<c>{name}.verified.json</c>), or <see langword="null"/> to derive it from the subject expression.</param>
    /// <param name="snapshotsDirectory">The snapshots folder, or <see langword="null"/> for <c>__snapshots__</c> next to the calling source file.</param>
    /// <param name="config">Optional snapshot configuration (scrubbers, naming, normalization).</param>
    /// <param name="callerFilePath">The calling source file (provided by the compiler).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentException">The snapshot name or directory is unsafe.</exception>
    /// <exception cref="AssertionFailedException">No verified snapshot exists, or the content differs.</exception>
    public AndConstraint<JsonNodeAssertions> MatchSnapshot(
        string? snapshotName = null,
        string? snapshotsDirectory = null,
        Func<SnapshotOptions, SnapshotOptions>? config = null,
        [CallerFilePath] string? callerFilePath = null,
        string? because = null,
        params object?[] becauseArgs) {
        var options = config?.Invoke(new SnapshotOptions()) ?? new SnapshotOptions();
        var json = _value is null ? "null" : _value.ToJsonString(Snapshot.JsonOptions);
        Snapshot.Verify(json.Replace("\r\n", "\n").Replace('\r', '\n'), snapshotName, snapshotsDirectory, ".json", options, new(Expression, callerFilePath, because, becauseArgs));
        return new(this);
    }
}
