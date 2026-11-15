//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.IO;

/// <summary>Provides shared assertion methods for <see cref="FileSystemInfo"/> subjects.</summary>
/// <typeparam name="T">The concrete file-system information type.</typeparam>
/// <typeparam name="TAssertions">The concrete assertion type returned for chaining.</typeparam>
/// <param name="value">The file-system entry to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test.</param>
/// <param name="subjectName">The default subject name used when no caller expression is available.</param>
public abstract class FileSystemInfoAssertions<T, TAssertions>(T? value, string? expression, string subjectName) : AssertionsBase(expression, subjectName)
    where T : FileSystemInfo
    where TAssertions : FileSystemInfoAssertions<T, TAssertions> {
    /// <summary>Gets the subject under assertion (may be <see langword="null"/> or missing).</summary>
    protected T? Subject { get; } = value;

    /// <summary>Gets the concrete assertion instance for chaining.</summary>
    protected TAssertions Self => (TAssertions)this;

    /// <summary>Gets the path comparison used by <see cref="HaveFullName"/> and <see cref="BeParentOf"/>.</summary>
    /// <remarks>Case-insensitive on Windows, ordinal elsewhere.</remarks>
    protected static StringComparison PathComparison =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

    /// <summary>Asserts that the file-system entry exists.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The entry is <see langword="null"/> or missing.</exception>
    public AndConstraint<TAssertions> Exist(string? because = null, params object?[] becauseArgs) {
        if(Subject is null || !Subject.Exists) {
            Fail(Strings.Format(StringKeys.FILE_EXIST, Expression, MissingState()), because, becauseArgs);
        }

        return new(Self);
    }

    /// <summary>Asserts that the file-system entry does not exist. A <see langword="null"/> entry passes.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The entry exists.</exception>
    public AndConstraint<TAssertions> NotExist(string? because = null, params object?[] becauseArgs) {
        if(Subject?.Exists == true) {
            Fail(Strings.Format(StringKeys.FILE_NOT_EXIST, Expression), because, becauseArgs);
        }

        return new(Self);
    }

    /// <summary>Asserts that the entry has the simple name <paramref name="expected"/> (path comparison rules).</summary>
    /// <param name="expected">The expected name.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The entry is missing or has another name.</exception>
    public AndConstraint<TAssertions> HaveName(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(!EnsureExists(because, becauseArgs)) {
            return new(Self);
        }

        if(!string.Equals(Subject!.Name, expected, PathComparison)) {
            Fail(Strings.Format(StringKeys.FILE_HAVE_NAME, Expression, expected, Subject.Name), because, becauseArgs);
        }

        return new(Self);
    }

    /// <summary>Asserts that the entry has the normalized full path <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected full path (relative paths resolve against the current directory).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The entry is missing or has another full path.</exception>
    public AndConstraint<TAssertions> HaveFullName(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(!EnsureExists(because, becauseArgs)) {
            return new(Self);
        }

        var normalizedExpected = NormalizePath(expected);
        if(!string.Equals(NormalizePath(Subject!.FullName), normalizedExpected, PathComparison)) {
            Fail(Strings.Format(StringKeys.FILE_HAVE_FULL_NAME, Expression, normalizedExpected, Subject.FullName), because, becauseArgs);
        }

        return new(Self);
    }

    /// <summary>Asserts that the entry is read-only.</summary>
    /// <remarks>On Unix, read-only is derived from permissions by the runtime; hidden/system flags may not be meaningful.</remarks>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The entry is missing or not read-only.</exception>
    public AndConstraint<TAssertions> BeReadOnly(string? because = null, params object?[] becauseArgs) {
        if(!EnsureExists(because, becauseArgs)) {
            return new(Self);
        }

        if(!Subject!.Attributes.HasFlag(FileAttributes.ReadOnly)) {
            Fail(Strings.Format(StringKeys.FILE_BE_READ_ONLY, Expression), because, becauseArgs);
        }

        return new(Self);
    }

    /// <summary>Asserts that the entry is not read-only.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The entry is missing or read-only.</exception>
    public AndConstraint<TAssertions> NotBeReadOnly(string? because = null, params object?[] becauseArgs) {
        if(!EnsureExists(because, becauseArgs)) {
            return new(Self);
        }

        if(Subject!.Attributes.HasFlag(FileAttributes.ReadOnly)) {
            Fail(Strings.Format(StringKeys.FILE_NOT_BE_READ_ONLY, Expression), because, becauseArgs);
        }

        return new(Self);
    }

    /// <summary>Asserts that the entry is hidden.</summary>
    /// <remarks>Hidden is a Windows attribute; on Unix the runtime may not set it from a leading dot.</remarks>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The entry is missing or not hidden.</exception>
    public AndConstraint<TAssertions> BeHidden(string? because = null, params object?[] becauseArgs) {
        if(!EnsureExists(because, becauseArgs)) {
            return new(Self);
        }

        if(!Subject!.Attributes.HasFlag(FileAttributes.Hidden)) {
            Fail(Strings.Format(StringKeys.FILE_BE_HIDDEN, Expression), because, becauseArgs);
        }

        return new(Self);
    }

    /// <summary>Asserts that the entry is not hidden.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The entry is missing or hidden.</exception>
    public AndConstraint<TAssertions> NotBeHidden(string? because = null, params object?[] becauseArgs) {
        if(!EnsureExists(because, becauseArgs)) {
            return new(Self);
        }

        if(Subject!.Attributes.HasFlag(FileAttributes.Hidden)) {
            Fail(Strings.Format(StringKeys.FILE_NOT_BE_HIDDEN, Expression), because, becauseArgs);
        }

        return new(Self);
    }

    /// <summary>Asserts that every flag in <paramref name="expected"/> is present on the entry.</summary>
    /// <param name="expected">The required attribute flags (a combination is checked with AND).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The entry is missing or lacks a requested flag.</exception>
    public AndConstraint<TAssertions> HaveAttribute(FileAttributes expected, string? because = null, params object?[] becauseArgs) {
        if(!EnsureExists(because, becauseArgs)) {
            return new(Self);
        }

        if((Subject!.Attributes & expected) != expected) {
            Fail(Strings.Format(StringKeys.FILE_HAVE_ATTRIBUTE, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(Subject.Attributes)), because, becauseArgs);
        }

        return new(Self);
    }

    /// <summary>Asserts that none of the flags in <paramref name="unexpected"/> are present on the entry.</summary>
    /// <param name="unexpected">The forbidden attribute flags (a combination fails when any is present).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The entry is missing or has one of the flags.</exception>
    public AndConstraint<TAssertions> NotHaveAttribute(FileAttributes unexpected, string? because = null, params object?[] becauseArgs) {
        if(!EnsureExists(because, becauseArgs)) {
            return new(Self);
        }

        if((Subject!.Attributes & unexpected) != 0) {
            Fail(Strings.Format(StringKeys.FILE_NOT_HAVE_ATTRIBUTE, Expression, AssertionScope.FormatInvariant(unexpected)), because, becauseArgs);
        }

        return new(Self);
    }

    /// <summary>Asserts that the creation time is within <paramref name="tolerance"/> of <paramref name="expected"/> (UTC).</summary>
    /// <param name="expected">The expected creation time; local values are converted, unspecified values are treated as UTC.</param>
    /// <param name="tolerance">The non-negative accepted absolute difference.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="tolerance"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The entry is missing or the time differs by more than the tolerance.</exception>
    public AndConstraint<TAssertions> HaveCreationTime(DateTime expected, TimeSpan tolerance, string? because = null, params object?[] becauseArgs) {
        ValidateTolerance(tolerance);
        if(!EnsureExists(because, becauseArgs)) {
            return new(Self);
        }

        var actual = Subject!.CreationTimeUtc;
        var expectedUtc = ToUtc(expected);
        if(!WithinTolerance(actual, expectedUtc, tolerance)) {
            Fail(Strings.Format(StringKeys.FILE_HAVE_CREATION_TIME, Expression, AssertionScope.FormatInvariant(tolerance), AssertionScope.FormatInvariant(expectedUtc), AssertionScope.FormatInvariant(actual)), because, becauseArgs);
        }

        return new(Self);
    }

    /// <summary>Asserts that the last write time is within <paramref name="tolerance"/> of <paramref name="expected"/> (UTC).</summary>
    /// <param name="expected">The expected last write time; local values are converted, unspecified values are treated as UTC.</param>
    /// <param name="tolerance">The non-negative accepted absolute difference.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="tolerance"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The entry is missing or the time differs by more than the tolerance.</exception>
    public AndConstraint<TAssertions> HaveLastWriteTime(DateTime expected, TimeSpan tolerance, string? because = null, params object?[] becauseArgs) {
        ValidateTolerance(tolerance);
        if(!EnsureExists(because, becauseArgs)) {
            return new(Self);
        }

        var actual = Subject!.LastWriteTimeUtc;
        var expectedUtc = ToUtc(expected);
        if(!WithinTolerance(actual, expectedUtc, tolerance)) {
            Fail(Strings.Format(StringKeys.FILE_HAVE_LAST_WRITE_TIME, Expression, AssertionScope.FormatInvariant(tolerance), AssertionScope.FormatInvariant(expectedUtc), AssertionScope.FormatInvariant(actual)), because, becauseArgs);
        }

        return new(Self);
    }

    /// <summary>Asserts that the entry is a parent directory of <paramref name="child"/>, using path boundaries.</summary>
    /// <param name="child">The expected descendant entry.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="child"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The entry is missing or is not an ancestor of <paramref name="child"/>.</exception>
    public AndConstraint<TAssertions> BeParentOf(FileSystemInfo child, string? because = null, params object?[] becauseArgs) {
        if(child is null) {
            throw new ArgumentNullException(nameof(child));
        }

        if(!EnsureExists(because, becauseArgs)) {
            return new(Self);
        }

        var parent = NormalizePath(Subject!.FullName);
        var childPath = NormalizePath(child.FullName);
        var parentPrefix = parent.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)
            ? parent
            : parent + Path.DirectorySeparatorChar;
        if(string.Equals(parent, childPath, PathComparison) || !childPath.StartsWith(parentPrefix, PathComparison)) {
            Fail(Strings.Format(StringKeys.FILE_BE_PARENT_OF, Expression, childPath), because, becauseArgs);
        }

        return new(Self);
    }

    /// <summary>Ensures the subject exists before a metadata assertion reads its properties.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns><see langword="true"/> when the subject exists.</returns>
    protected bool EnsureExists(string? because, object?[] becauseArgs) {
        if(Subject is null || !Subject.Exists) {
            Fail(Strings.Format(StringKeys.FILE_REQUIRE_EXISTS, Expression, MissingState()), because, becauseArgs);
            return false;
        }

        return true;
    }

    /// <summary>Enumerates the top-level entries of the directory, failing clearly on access denial.</summary>
    /// <param name="entries">The top-level entries when enumeration succeeds.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns><see langword="true"/> when the directory exists and can be enumerated.</returns>
    protected bool TryGetEntries(out FileSystemInfo[] entries, string? because, object?[] becauseArgs) {
        entries = [];
        if(!EnsureExists(because, becauseArgs) || Subject is not DirectoryInfo directory) {
            return false;
        }

        try {
            entries = [.. directory.EnumerateFileSystemInfos()];
            return true;
        }
        catch(UnauthorizedAccessException) {
            Fail(Strings.Format(StringKeys.FILE_ACCESS_DENIED, Expression), because, becauseArgs);
            return false;
        }
    }

    /// <summary>Describes why the subject is considered missing.</summary>
    /// <returns>The localized-friendly actual state used by existence messages.</returns>
    private string MissingState() => Subject is null ? "it was null" : "it was not found";

    /// <summary>Normalizes a path for comparison, preserving a drive or filesystem root.</summary>
    /// <param name="path">The path to normalize.</param>
    /// <returns>The absolute path without a trailing separator unless it is a root.</returns>
    protected static string NormalizePath(string path) {
        var full = Path.GetFullPath(path);
        var root = Path.GetPathRoot(full);
        if(root is not null && string.Equals(full, root, PathComparison)) {
            return full;
        }

        return full.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    /// <summary>Validates that a time tolerance is non-negative.</summary>
    /// <param name="tolerance">The tolerance to validate.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="tolerance"/> is negative.</exception>
    private static void ValidateTolerance(TimeSpan tolerance) {
        if(tolerance < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(tolerance), "Tolerance must not be negative.");
        }
    }

    /// <summary>Converts an expected time to UTC, treating unspecified values as UTC.</summary>
    /// <param name="value">The expected time.</param>
    /// <returns>The UTC representation.</returns>
    private static DateTime ToUtc(DateTime value) =>
        value.Kind == DateTimeKind.Local ? value.ToUniversalTime() : DateTime.SpecifyKind(value, DateTimeKind.Utc);

    /// <summary>Determines whether two times are within the absolute tolerance.</summary>
    /// <param name="actual">The actual time.</param>
    /// <param name="expected">The expected time.</param>
    /// <param name="tolerance">The non-negative tolerance.</param>
    /// <returns><see langword="true"/> when the absolute difference is at most the tolerance.</returns>
    private static bool WithinTolerance(DateTime actual, DateTime expected, TimeSpan tolerance) =>
        (actual - expected).Duration() <= tolerance;
}
