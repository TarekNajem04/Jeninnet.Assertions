//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Streams;

/// <summary>Provides assertion methods for verifying <see cref="Stream"/> capabilities and length.</summary>
/// <param name="value">The stream to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public class StreamAssertions(Stream? value, string? expression = null) : AssertionsBase(expression, "stream") {
    private readonly Stream? _value = value;

    /// <summary>Asserts that the stream is <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The stream is not <see langword="null"/>.</exception>
    public AndConstraint<StreamAssertions> BeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is not null) {
            Fail(Strings.Format(StringKeys.STREAM_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the stream is not <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The stream is <see langword="null"/>.</exception>
    public AndConstraint<StreamAssertions> NotBeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.STREAM_NOT_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the stream is readable.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The stream is <see langword="null"/> or not readable.</exception>
    public AndConstraint<StreamAssertions> BeReadable(string? because = null, params object?[] becauseArgs) {
        if(_value?.CanRead != true) {
            Fail(Strings.Format(StringKeys.STREAM_BE_READABLE, Expression, _value is null ? "found null" : "it was not"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the stream is not readable. A <see langword="null"/> stream passes.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The stream is readable.</exception>
    public AndConstraint<StreamAssertions> NotBeReadable(string? because = null, params object?[] becauseArgs) {
        if(_value?.CanRead == true) {
            Fail(Strings.Format(StringKeys.STREAM_NOT_BE_READABLE, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the stream is writable.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The stream is <see langword="null"/> or not writable.</exception>
    public AndConstraint<StreamAssertions> BeWritable(string? because = null, params object?[] becauseArgs) {
        if(_value?.CanWrite != true) {
            Fail(Strings.Format(StringKeys.STREAM_BE_WRITABLE, Expression, _value is null ? "found null" : "it was not"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the stream is not writable. A <see langword="null"/> stream passes.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The stream is writable.</exception>
    public AndConstraint<StreamAssertions> NotBeWritable(string? because = null, params object?[] becauseArgs) {
        if(_value?.CanWrite == true) {
            Fail(Strings.Format(StringKeys.STREAM_NOT_BE_WRITABLE, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the stream is seekable.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The stream is <see langword="null"/> or not seekable.</exception>
    public AndConstraint<StreamAssertions> BeSeekable(string? because = null, params object?[] becauseArgs) {
        if(_value?.CanSeek != true) {
            Fail(Strings.Format(StringKeys.STREAM_BE_SEEKABLE, Expression, _value is null ? "found null" : "it was not"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the stream is not seekable. A <see langword="null"/> stream passes.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The stream is seekable.</exception>
    public AndConstraint<StreamAssertions> NotBeSeekable(string? because = null, params object?[] becauseArgs) {
        if(_value?.CanSeek == true) {
            Fail(Strings.Format(StringKeys.STREAM_NOT_BE_SEEKABLE, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the stream is readable and not writable (read-only).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The stream is <see langword="null"/>, not readable, or writable.</exception>
    public AndConstraint<StreamAssertions> BeReadOnly(string? because = null, params object?[] becauseArgs) {
        if(_value?.CanRead != true || _value.CanWrite) {
            Fail(Strings.Format(StringKeys.STREAM_BE_READ_ONLY, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the stream is not read-only. A <see langword="null"/> stream passes.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The stream is readable and not writable.</exception>
    public AndConstraint<StreamAssertions> NotBeReadOnly(string? because = null, params object?[] becauseArgs) {
        if(_value is not null && _value.CanRead && !_value.CanWrite) {
            Fail(Strings.Format(StringKeys.STREAM_NOT_BE_READ_ONLY, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the stream is writable and not readable (write-only).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The stream is <see langword="null"/>, not writable, or readable.</exception>
    public AndConstraint<StreamAssertions> BeWriteOnly(string? because = null, params object?[] becauseArgs) {
        if(_value?.CanWrite != true || _value.CanRead) {
            Fail(Strings.Format(StringKeys.STREAM_BE_WRITE_ONLY, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the stream is not write-only. A <see langword="null"/> stream passes.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The stream is writable and not readable.</exception>
    public AndConstraint<StreamAssertions> NotBeWriteOnly(string? because = null, params object?[] becauseArgs) {
        if(_value is not null && _value.CanWrite && !_value.CanRead) {
            Fail(Strings.Format(StringKeys.STREAM_NOT_BE_WRITE_ONLY, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the stream has exactly <paramref name="expected"/> bytes.</summary>
    /// <param name="expected">The expected length.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The stream is <see langword="null"/>, not seekable, or its length differs.</exception>
    public AndConstraint<StreamAssertions> HaveLength(long expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.STREAM_HAVE_LENGTH_NULL, Expression, AssertionScope.FormatInvariant(expected)), because, becauseArgs);
            return new(this);
        }

        if(!_value.CanSeek) {
            Fail(Strings.Format(StringKeys.STREAM_HAVE_LENGTH_UNSEEKABLE, Expression, AssertionScope.FormatInvariant(expected)), because, becauseArgs);
            return new(this);
        }

        if(_value.Length != expected) {
            Fail(Strings.Format(StringKeys.STREAM_HAVE_LENGTH, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_value.Length)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the stream does not have exactly <paramref name="expected"/> bytes.</summary>
    /// <remarks>A <see langword="null"/> stream or one whose length cannot be read passes: absence cannot be disproven without a readable length.</remarks>
    /// <param name="expected">The length that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The stream has exactly <paramref name="expected"/> bytes.</exception>
    public AndConstraint<StreamAssertions> NotHaveLength(long expected, string? because = null, params object?[] becauseArgs) {
        if(_value is not null && _value.CanSeek && _value.Length == expected) {
            Fail(Strings.Format(StringKeys.STREAM_NOT_HAVE_LENGTH, Expression, AssertionScope.FormatInvariant(expected)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the stream position equals <paramref name="expected"/> (positions start at zero).</summary>
    /// <param name="expected">The expected position.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The stream is <see langword="null"/>, not seekable, or positioned elsewhere.</exception>
    public AndConstraint<StreamAssertions> HavePosition(long expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.STREAM_HAVE_POSITION_NULL, Expression, AssertionScope.FormatInvariant(expected)), because, becauseArgs);
            return new(this);
        }

        if(!_value.CanSeek) {
            Fail(Strings.Format(StringKeys.STREAM_HAVE_POSITION_UNSEEKABLE, Expression, AssertionScope.FormatInvariant(expected)), because, becauseArgs);
            return new(this);
        }

        if(_value.Position != expected) {
            Fail(Strings.Format(StringKeys.STREAM_HAVE_POSITION, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_value.Position)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the stream position does not equal <paramref name="expected"/>.</summary>
    /// <remarks>A <see langword="null"/> stream or one whose position cannot be read passes: absence cannot be disproven without a readable position.</remarks>
    /// <param name="expected">The position that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The stream is positioned exactly at <paramref name="expected"/>.</exception>
    public AndConstraint<StreamAssertions> NotHavePosition(long expected, string? because = null, params object?[] becauseArgs) {
        if(_value is not null && _value.CanSeek && _value.Position == expected) {
            Fail(Strings.Format(StringKeys.STREAM_NOT_HAVE_POSITION, Expression, AssertionScope.FormatInvariant(expected)), because, becauseArgs);
        }

        return new(this);
    }
}
