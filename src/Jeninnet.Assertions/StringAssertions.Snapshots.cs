//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class StringAssertions {
    /// <summary>Asserts that the string matches the committed text snapshot.</summary>
    /// <param name="snapshotName">The snapshot name (<c>{name}.verified.txt</c>), or <see langword="null"/> to derive it from the subject expression.</param>
    /// <param name="snapshotsDirectory">The snapshots folder, or <see langword="null"/> for <c>__snapshots__</c> next to the calling source file.</param>
    /// <param name="config">Optional snapshot configuration (explicit scrubbers).</param>
    /// <param name="callerFilePath">The calling source file (provided by the compiler).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentException">The snapshot name is empty or unsafe.</exception>
    /// <exception cref="AssertionFailedException">No verified snapshot exists, or the content differs (a <c>.received</c> file is written for review).</exception>
    public AndConstraint<StringAssertions> MatchSnapshot(
        string? snapshotName = null,
        string? snapshotsDirectory = null,
        Func<SnapshotOptions, SnapshotOptions>? config = null,
        [CallerFilePath] string? callerFilePath = null,
        string? because = null,
        params object?[] becauseArgs) {
        var options = config?.Invoke(new SnapshotOptions()) ?? new SnapshotOptions();
        Snapshot.Verify(_value ?? "<null>", snapshotName, snapshotsDirectory, ".txt", options, new(Expression, callerFilePath, because, becauseArgs));
        return new(this);
    }
}
