//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.IO;

/// <summary>Provides assertion methods for verifying <see cref="FileInfo"/> metadata.</summary>
/// <param name="value">The file to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test.</param>
public sealed class FileInfoAssertions(FileInfo? value, string? expression = null)
    : FileSystemInfoAssertions<FileInfo, FileInfoAssertions>(value, expression, "file") {
    private const string NEGATIVE_LENGTH_MESSAGE = "Length must not be negative.";

    /// <summary>Asserts that the file extension equals <paramref name="expected"/>, with or without a leading dot.</summary>
    /// <remarks>Comparison follows path rules: case-insensitive on Windows, ordinal elsewhere.</remarks>
    /// <param name="expected">The expected extension, for example <c>.txt</c> or <c>txt</c>; an empty string expects no extension.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The file is missing or has another extension.</exception>
    public AndConstraint<FileInfoAssertions> HaveExtension(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(!EnsureExists(because, becauseArgs)) {
            return new(Self);
        }

        var normalized = expected.Length == 0 || expected[0] == '.' ? expected : "." + expected;
        if(!string.Equals(Subject!.Extension, normalized, PathComparison)) {
            Fail(Strings.Format(StringKeys.FILE_HAVE_EXTENSION, Expression, normalized, Subject.Extension), because, becauseArgs);
        }

        return new(Self);
    }

    /// <summary>Asserts that the file lives in the normalized directory <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected directory path.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The file is missing or lives in another directory.</exception>
    public AndConstraint<FileInfoAssertions> HaveDirectory(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(!EnsureExists(because, becauseArgs)) {
            return new(Self);
        }

        var actual = Subject!.DirectoryName;
        if(actual is null || !string.Equals(NormalizePath(actual), NormalizePath(expected), PathComparison)) {
            Fail(Strings.Format(StringKeys.FILE_HAVE_DIRECTORY, Expression, NormalizePath(expected), actual ?? "<null>"), because, becauseArgs);
        }

        return new(Self);
    }

    /// <summary>Asserts that the file length equals <paramref name="expected"/> bytes.</summary>
    /// <param name="expected">The expected length in bytes.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="expected"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The file is missing or has another length.</exception>
    public AndConstraint<FileInfoAssertions> HaveLength(long expected, string? because = null, params object?[] becauseArgs) {
        if(expected < 0) {
            throw new ArgumentOutOfRangeException(nameof(expected), NEGATIVE_LENGTH_MESSAGE);
        }

        if(!EnsureExists(because, becauseArgs)) {
            return new(Self);
        }

        if(Subject!.Length != expected) {
            Fail(Strings.Format(StringKeys.FILE_HAVE_LENGTH, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(Subject.Length)), because, becauseArgs);
        }

        return new(Self);
    }

    /// <summary>Asserts that the file length does not equal <paramref name="unexpected"/> bytes.</summary>
    /// <param name="unexpected">The length in bytes the file must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="unexpected"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The file is missing or has that length.</exception>
    public AndConstraint<FileInfoAssertions> NotHaveLength(long unexpected, string? because = null, params object?[] becauseArgs) {
        if(unexpected < 0) {
            throw new ArgumentOutOfRangeException(nameof(unexpected), NEGATIVE_LENGTH_MESSAGE);
        }

        if(!EnsureExists(because, becauseArgs)) {
            return new(Self);
        }

        if(Subject!.Length == unexpected) {
            Fail(Strings.Format(StringKeys.FILE_NOT_HAVE_LENGTH, Expression, AssertionScope.FormatInvariant(unexpected), AssertionScope.FormatInvariant(Subject.Length)), because, becauseArgs);
        }

        return new(Self);
    }

    /// <summary>Asserts that the file is longer than <paramref name="expected"/> bytes.</summary>
    /// <param name="expected">The exclusive lower bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="expected"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The file is missing or too short.</exception>
    public AndConstraint<FileInfoAssertions> HaveLengthGreaterThan(long expected, string? because = null, params object?[] becauseArgs) {
        if(expected < 0) {
            throw new ArgumentOutOfRangeException(nameof(expected), NEGATIVE_LENGTH_MESSAGE);
        }

        if(!EnsureExists(because, becauseArgs)) {
            return new(Self);
        }

        if(Subject!.Length <= expected) {
            Fail(Strings.Format(StringKeys.FILE_HAVE_LENGTH_GREATER_THAN, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(Subject.Length)), because, becauseArgs);
        }

        return new(Self);
    }

    /// <summary>Asserts that the file is shorter than <paramref name="expected"/> bytes.</summary>
    /// <param name="expected">The exclusive upper bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="expected"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The file is missing or too long.</exception>
    public AndConstraint<FileInfoAssertions> HaveLengthLessThan(long expected, string? because = null, params object?[] becauseArgs) {
        if(expected < 0) {
            throw new ArgumentOutOfRangeException(nameof(expected), NEGATIVE_LENGTH_MESSAGE);
        }

        if(!EnsureExists(because, becauseArgs)) {
            return new(Self);
        }

        if(Subject!.Length >= expected) {
            Fail(Strings.Format(StringKeys.FILE_HAVE_LENGTH_LESS_THAN, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(Subject.Length)), because, becauseArgs);
        }

        return new(Self);
    }

    /// <summary>Asserts that the file is empty (zero bytes).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The file is missing or non-empty.</exception>
    public AndConstraint<FileInfoAssertions> BeEmpty(string? because = null, params object?[] becauseArgs) {
        if(!EnsureExists(because, becauseArgs)) {
            return new(Self);
        }

        if(Subject!.Length != 0) {
            Fail(Strings.Format(StringKeys.FILE_BE_EMPTY, Expression, AssertionScope.FormatInvariant(Subject.Length)), because, becauseArgs);
        }

        return new(Self);
    }

    /// <summary>Asserts that the file is not empty.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The file is missing or empty.</exception>
    public AndConstraint<FileInfoAssertions> NotBeEmpty(string? because = null, params object?[] becauseArgs) {
        if(!EnsureExists(because, becauseArgs)) {
            return new(Self);
        }

        if(Subject!.Length == 0) {
            Fail(Strings.Format(StringKeys.FILE_NOT_BE_EMPTY, Expression), because, becauseArgs);
        }

        return new(Self);
    }
}
