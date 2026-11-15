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
/// Comparison is exact and ordinal unless line-ending or whitespace tolerance is enabled. JSON/XML snapshots
/// are written with LF line endings. When the <c>JENINNET_SNAPSHOT_UPDATE</c> environment variable is exactly
/// <c>1</c>, or a snapshot opts into <see cref="SnapshotOptions.AutoVerify"/>, the received text is promoted
/// to the <c>.verified</c> file and the assertion passes. Otherwise a snapshot is only accepted by a human.
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

        var name = SnapshotNaming.ResolveName(snapshotName, options, context);
        var directory = SnapshotNaming.ResolveDirectory(snapshotsDirectory, context.CallerFilePath, options);
        Directory.CreateDirectory(directory);

        var received = receivedText;
        foreach(var scrubber in options.Scrubbers) {
            received = scrubber(received);
        }

        var stored = SnapshotComparison.FormatForStorage(received, options, fileExtension);
        var verifiedPath = Path.Combine(directory, name + ".verified" + fileExtension);
        var receivedPath = Path.Combine(directory, name + ".received" + fileExtension);
        if(IsUpdateRequested() || options.AutoVerifyEnabled) {
            Accept(receivedPath, verifiedPath, stored);
            return;
        }

        if(!File.Exists(verifiedPath)) {
            File.WriteAllText(receivedPath, stored, _encoding);
            var missing = AssertionScope.AppendBecause(Strings.Format(StringKeys.SNAPSHOT_MISSING_BASELINE, context.Expression, name, receivedPath, verifiedPath), context.Because, context.BecauseArgs);
            AssertionScope.Fail(missing + "\n\n" + Strings.Format(StringKeys.SNAPSHOT_RECEIVED_PREVIEW, stored));
            return;
        }

        var verified = File.ReadAllText(verifiedPath);
        var verifiedComparable = SnapshotComparison.NormalizeForComparison(verified, options, fileExtension);
        var receivedComparable = SnapshotComparison.NormalizeForComparison(stored, options, fileExtension);
        if(string.Equals(verifiedComparable, receivedComparable, StringComparison.Ordinal)) {
            if(File.Exists(receivedPath)) {
                File.Delete(receivedPath);
            }

            return;
        }

        File.WriteAllText(receivedPath, stored, _encoding);
        var diff = options.DisableDiffEnabled ? null : TextDiff.UnifiedDiff(verifiedComparable, receivedComparable);
        var message = AssertionScope.AppendBecause(Strings.Format(StringKeys.SNAPSHOT_MISMATCH, context.Expression, name, receivedPath), context.Because, context.BecauseArgs);
        AssertionScope.Fail(diff is null ? message : message + "\n" + diff);
        TryLaunchDiffTool(options.DiffToolPath, verifiedPath, receivedPath);
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

    /// <summary>Starts the explicitly configured diff tool; failures are ignored so the assertion remains primary.</summary>
    /// <param name="executablePath">The diff tool path, or <see langword="null"/> to do nothing.</param>
    /// <param name="verifiedPath">The verified file path.</param>
    /// <param name="receivedPath">The received file path.</param>
    private static void TryLaunchDiffTool(string? executablePath, string verifiedPath, string receivedPath) {
        if(string.IsNullOrEmpty(executablePath)) {
            return;
        }

        try {
            var startInfo = new ProcessStartInfo(executablePath) {
                UseShellExecute = false,
            };
#if NET6_0_OR_GREATER
            startInfo.ArgumentList.Add(verifiedPath);
            startInfo.ArgumentList.Add(receivedPath);
#else
            startInfo.Arguments = QuoteArgument(verifiedPath) + " " + QuoteArgument(receivedPath);
#endif
            using var process = Process.Start(startInfo);
        }
        catch(Exception) {
            // The diff tool is a convenience; a missing or failing executable must not hide the snapshot failure.
        }
    }

#if NETSTANDARD2_0
    /// <summary>Quotes one process argument so a snapshot path cannot inject extra diff-tool arguments.</summary>
    /// <param name="value">The raw argument.</param>
    /// <returns>The escaped, double-quoted argument.</returns>
    private static string QuoteArgument(string value) =>
        "\"" + value.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
#endif
}

/// <summary>The caller context of one snapshot verification: the subject expression, the calling source file, and the <c>because</c> clause.</summary>
/// <param name="Expression">The caller expression naming the subject under test.</param>
/// <param name="CallerFilePath">The calling source file (compiler-provided).</param>
/// <param name="Because">The reason for the assertion, appended to the failure message.</param>
/// <param name="BecauseArgs">Optional arguments used to format <paramref name="Because"/>.</param>
internal readonly record struct SnapshotContext(string Expression, string? CallerFilePath, string? Because, object?[] BecauseArgs);
