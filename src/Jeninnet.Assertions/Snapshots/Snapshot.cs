//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Snapshots;

/// <summary>Compares received text against committed <c>.verified</c> snapshot files.</summary>
/// <remarks>
/// File policy: a missing baseline writes a <c>.received</c> file and fails with review instructions
/// (never auto-accepts); a mismatch overwrites <c>.received</c> and fails with a unified diff; a match
/// deletes a stale <c>.received</c> file. Snapshot names are bare file names (no separators, no <c>..</c>).
/// Comparison is exact and ordinal; commit <c>*.verified.*</c> with <c>eol=lf</c> so checkouts cannot
/// flip line endings between machines. When the <c>JENINNET_SNAPSHOT_UPDATE</c> environment variable is
/// exactly <c>1</c>, the received text is promoted to the <c>.verified</c> file and the assertion passes;
/// this is the only way a snapshot is ever accepted without a manual rename, and it must be requested
/// explicitly per run.
/// </remarks>
internal static class Snapshot {
    private const string UPDATE_ENVIRONMENT_VARIABLE = "JENINNET_SNAPSHOT_UPDATE";
    private static readonly UTF8Encoding _encoding = new(false);

    /// <summary>Serializes objects for snapshots: indented, invariant, declaration-ordered JSON.</summary>
    /// <remarks>Dictionaries serialize in insertion order; cyclic graphs throw instead of silently dropping data.</remarks>
    internal static JsonSerializerOptions JsonOptions { get; } = new() { WriteIndented = true };

    /// <summary>Verifies <paramref name="receivedText"/> against the committed snapshot.</summary>
    /// <param name="receivedText">The actual text (already in final form).</param>
    /// <param name="snapshotName">The snapshot name, or <see langword="null"/> to derive it from <see cref="SnapshotContext.Expression"/>.</param>
    /// <param name="snapshotsDirectory">The snapshots folder, or <see langword="null"/> for <c>__snapshots__</c> next to <see cref="SnapshotContext.CallerFilePath"/>.</param>
    /// <param name="fileExtension">The snapshot file extension (including the dot).</param>
    /// <param name="options">The snapshot configuration.</param>
    /// <param name="context">The caller context: subject expression, caller file, and the <c>because</c> clause.</param>
    /// <exception cref="ArgumentException"><paramref name="snapshotName"/> is empty, unsafe, or <see cref="SnapshotContext.CallerFilePath"/> is missing when needed.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The snapshot is missing or differs.</exception>
    internal static void Verify(
        string receivedText,
        string? snapshotName,
        string? snapshotsDirectory,
        string fileExtension,
        SnapshotOptions options,
        SnapshotContext context) {
        if(options is null) {
            throw new ArgumentNullException(nameof(options));
        }

        var name = ResolveName(snapshotName, context.Expression);
        var directory = ResolveDirectory(snapshotsDirectory, context.CallerFilePath);
        Directory.CreateDirectory(directory);

        var received = receivedText;
        foreach(var scrubber in options.Scrubbers) {
            received = scrubber(received);
        }

        var verifiedPath = Path.Combine(directory, name + ".verified" + fileExtension);
        var receivedPath = Path.Combine(directory, name + ".received" + fileExtension);
        if(IsUpdateRequested()) {
            Accept(receivedPath, verifiedPath, received);
            return;
        }

        if(!File.Exists(verifiedPath)) {
            File.WriteAllText(receivedPath, received, _encoding);
            var missing = AssertionScope.AppendBecause(Strings.Format(StringKeys.SNAPSHOT_MISSING_BASELINE, context.Expression, name, receivedPath, verifiedPath), context.Because, context.BecauseArgs);
            AssertionScope.Fail(missing + "\n\n" + Strings.Format(StringKeys.SNAPSHOT_RECEIVED_PREVIEW, received));
            return;
        }

        var verified = File.ReadAllText(verifiedPath);
        if(string.Equals(verified, received, StringComparison.Ordinal)) {
            if(File.Exists(receivedPath)) {
                File.Delete(receivedPath);
            }

            return;
        }

        File.WriteAllText(receivedPath, received, _encoding);
        var diff = TextDiff.UnifiedDiff(verified, received);
        var message = AssertionScope.AppendBecause(Strings.Format(StringKeys.SNAPSHOT_MISMATCH, context.Expression, name, receivedPath), context.Because, context.BecauseArgs);
        AssertionScope.Fail(diff is null ? message : message + "\n" + diff);
    }

    /// <summary>Determines whether this run opts in to promoting received files to verified.</summary>
    /// <returns><see langword="true"/> when the update environment variable is set to <c>1</c>.</returns>
    private static bool IsUpdateRequested() =>
        string.Equals(Environment.GetEnvironmentVariable(UPDATE_ENVIRONMENT_VARIABLE), "1", StringComparison.Ordinal);

    /// <summary>Promotes a received file to verified and removes the received copy.</summary>
    /// <param name="receivedPath">The received file path.</param>
    /// <param name="verifiedPath">The verified file path to write.</param>
    /// <param name="received">The content to promote.</param>
    private static void Accept(string receivedPath, string verifiedPath, string received) {
        File.WriteAllText(verifiedPath, received, _encoding);
        if(File.Exists(receivedPath)) {
            File.Delete(receivedPath);
        }
    }

    /// <summary>Resolves the snapshot name: the explicit name, or the sanitized subject expression.</summary>
    /// <param name="snapshotName">The explicit name, when supplied.</param>
    /// <param name="expression">The captured subject expression.</param>
    /// <returns>The resolved snapshot file name.</returns>
    /// <exception cref="ArgumentException"><paramref name="snapshotName"/> is invalid.</exception>
    private static string ResolveName(string? snapshotName, string expression) {
        if(snapshotName is null) {
            return Sanitize(expression);
        }

        ValidateName(snapshotName);
        return snapshotName;
    }

    /// <summary>Validates a caller-supplied snapshot name as a bare, valid file name.</summary>
    /// <param name="snapshotName">The snapshot name to validate.</param>
    /// <exception cref="ArgumentException"><paramref name="snapshotName"/> is empty, a parent reference, or contains path characters.</exception>
    private static void ValidateName(string snapshotName) {
        if(snapshotName.Length == 0) {
            throw new ArgumentException("Snapshot name must not be empty.", nameof(snapshotName));
        }

        if(snapshotName == ".."
            || snapshotName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0
            || snapshotName.IndexOf('/') >= 0
            || snapshotName.IndexOf('\\') >= 0) {
            throw new ArgumentException($"Snapshot name '{snapshotName}' must be a bare file name.", nameof(snapshotName));
        }
    }

    /// <summary>Converts a caller expression into a safe snapshot file name.</summary>
    /// <param name="expression">The captured subject expression.</param>
    /// <returns>The expression with path characters replaced by underscores; <c>snapshot</c> when nothing remains.</returns>
    private static string Sanitize(string expression) {
        var invalid = Path.GetInvalidFileNameChars();
        var builder = new StringBuilder(expression.Length);
        foreach(var character in expression) {
            builder.Append(character == '/' || character == '\\' || Array.IndexOf(invalid, character) >= 0 ? '_' : character);
        }

        var sanitized = builder.ToString().Trim();
        return sanitized.Length == 0 ? "snapshot" : sanitized;
    }

    /// <summary>Resolves the snapshots directory: the explicit value, or <c>__snapshots__</c> beside the caller file.</summary>
    /// <param name="snapshotsDirectory">The explicitly requested directory, when supplied.</param>
    /// <param name="callerFilePath">The compiler-provided caller file path.</param>
    /// <returns>The directory that holds verified and received files.</returns>
    /// <exception cref="ArgumentException"><paramref name="snapshotsDirectory"/> is absent and the caller path is unavailable.</exception>
    private static string ResolveDirectory(string? snapshotsDirectory, string? callerFilePath) {
        if(snapshotsDirectory is not null) {
            return snapshotsDirectory;
        }

        var baseDirectory = !string.IsNullOrEmpty(callerFilePath) ? Path.GetDirectoryName(callerFilePath) : null;
        if(string.IsNullOrEmpty(baseDirectory)) {
            throw new ArgumentException("Pass snapshotsDirectory explicitly when the caller file path is unavailable.", nameof(callerFilePath));
        }

        return Path.Combine(baseDirectory, "__snapshots__");
    }
}

/// <summary>The caller context of one snapshot verification: the subject expression, the calling source file, and the <c>because</c> clause.</summary>
/// <param name="Expression">The caller expression naming the subject under test.</param>
/// <param name="CallerFilePath">The calling source file (compiler-provided).</param>
/// <param name="Because">The reason for the assertion, appended to the failure message.</param>
/// <param name="BecauseArgs">Optional arguments used to format <paramref name="Because"/>.</param>
internal readonly record struct SnapshotContext(string Expression, string? CallerFilePath, string? Because, object?[] BecauseArgs);
