//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.IO;

/// <summary>Provides assertion methods for verifying <see cref="DirectoryInfo"/> contents.</summary>
/// <param name="value">The directory to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test.</param>
public sealed class DirectoryInfoAssertions(DirectoryInfo? value, string? expression = null)
    : FileSystemInfoAssertions<DirectoryInfo, DirectoryInfoAssertions>(value, expression, "directory") {
    /// <summary>Asserts that the directory contains no files or child directories.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The directory is missing, inaccessible, or non-empty.</exception>
    public AndConstraint<DirectoryInfoAssertions> BeEmpty(string? because = null, params object?[] becauseArgs) {
        if(!TryGetEntries(out var entries, because, becauseArgs)) {
            return new(Self);
        }

        if(entries.Length != 0) {
            Fail(Strings.Format(StringKeys.DIRECTORY_BE_EMPTY, Expression, AssertionScope.FormatInvariant(entries.Length)), because, becauseArgs);
        }

        return new(Self);
    }

    /// <summary>Asserts that the directory contains at least one file or child directory.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The directory is missing, inaccessible, or empty.</exception>
    public AndConstraint<DirectoryInfoAssertions> NotBeEmpty(string? because = null, params object?[] becauseArgs) {
        if(!TryGetEntries(out var entries, because, becauseArgs)) {
            return new(Self);
        }

        if(entries.Length == 0) {
            Fail(Strings.Format(StringKeys.DIRECTORY_NOT_BE_EMPTY, Expression), because, becauseArgs);
        }

        return new(Self);
    }

    /// <summary>Asserts that the directory contains a top-level file named <paramref name="name"/>.</summary>
    /// <param name="name">The expected file name.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The directory is missing, inaccessible, or lacks the file.</exception>
    public AndConstraint<DirectoryInfoAssertions> ContainFile(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        if(!TryGetEntries(out var entries, because, becauseArgs)) {
            return new(Self);
        }

        if(!entries.OfType<FileInfo>().Any(file => string.Equals(file.Name, name, PathComparison))) {
            Fail(Strings.Format(StringKeys.DIRECTORY_CONTAIN_FILE, Expression, name), because, becauseArgs);
        }

        return new(Self);
    }

    /// <summary>Asserts that the directory contains a top-level child directory named <paramref name="name"/>.</summary>
    /// <param name="name">The expected directory name.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The directory is missing, inaccessible, or lacks the child directory.</exception>
    public AndConstraint<DirectoryInfoAssertions> ContainDirectory(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        if(!TryGetEntries(out var entries, because, becauseArgs)) {
            return new(Self);
        }

        if(!entries.OfType<DirectoryInfo>().Any(directory => string.Equals(directory.Name, name, PathComparison))) {
            Fail(Strings.Format(StringKeys.DIRECTORY_CONTAIN_DIRECTORY, Expression, name), because, becauseArgs);
        }

        return new(Self);
    }

    /// <summary>Asserts that the directory contains exactly <paramref name="expected"/> top-level files.</summary>
    /// <param name="expected">The expected file count.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="expected"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The directory is missing, inaccessible, or has another file count.</exception>
    public AndConstraint<DirectoryInfoAssertions> HaveFileCount(int expected, string? because = null, params object?[] becauseArgs) {
        if(expected < 0) {
            throw new ArgumentOutOfRangeException(nameof(expected), "File count must not be negative.");
        }

        if(!TryGetEntries(out var entries, because, becauseArgs)) {
            return new(Self);
        }

        var actual = entries.Count(static entry => entry is FileInfo);
        if(actual != expected) {
            Fail(Strings.Format(StringKeys.DIRECTORY_HAVE_FILE_COUNT, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(actual)), because, becauseArgs);
        }

        return new(Self);
    }

    /// <summary>Asserts that the directory contains exactly <paramref name="expected"/> top-level child directories.</summary>
    /// <param name="expected">The expected child directory count.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="expected"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The directory is missing, inaccessible, or has another child directory count.</exception>
    public AndConstraint<DirectoryInfoAssertions> HaveDirectoryCount(int expected, string? because = null, params object?[] becauseArgs) {
        if(expected < 0) {
            throw new ArgumentOutOfRangeException(nameof(expected), "Directory count must not be negative.");
        }

        if(!TryGetEntries(out var entries, because, becauseArgs)) {
            return new(Self);
        }

        var actual = entries.Count(static entry => entry is DirectoryInfo);
        if(actual != expected) {
            Fail(Strings.Format(StringKeys.DIRECTORY_HAVE_DIRECTORY_COUNT, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(actual)), because, becauseArgs);
        }

        return new(Self);
    }
}
