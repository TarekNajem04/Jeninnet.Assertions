//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Streams;

/// <summary>Provides assertion methods for verifying <see cref="BufferedStream"/> values, including buffer size.</summary>
/// <param name="value">The buffered stream to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public class BufferedStreamAssertions(BufferedStream? value, string? expression = null) : StreamAssertions(value, expression) {
#if !NETSTANDARD2_0
    private readonly BufferedStream? _buffered = value;

    /// <summary>Asserts that the buffer size equals <paramref name="expected"/> bytes.</summary>
    /// <remarks>Requires .NET 6+; <see cref="BufferedStream.BufferSize"/> is unavailable on .NET Standard 2.0.</remarks>
    /// <param name="expected">The expected buffer size.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The stream is <see langword="null"/> or the buffer size differs.</exception>
    public AndConstraint<BufferedStreamAssertions> HaveBufferSize(int expected, string? because = null, params object?[] becauseArgs) {
        if(_buffered is null) {
            Fail(Strings.Format(StringKeys.BUFFERED_HAVE_SIZE_NULL, Expression, AssertionScope.FormatInvariant(expected)), because, becauseArgs);
            return new(this);
        }

        if(_buffered.BufferSize != expected) {
            Fail(Strings.Format(StringKeys.BUFFERED_HAVE_SIZE, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_buffered.BufferSize)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the buffer size does not equal <paramref name="unexpected"/> bytes.</summary>
    /// <remarks>Requires .NET 6+; <see cref="BufferedStream.BufferSize"/> is unavailable on .NET Standard 2.0. A <see langword="null"/> stream passes.</remarks>
    /// <param name="unexpected">The buffer size that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The buffer size equals <paramref name="unexpected"/>.</exception>
    public AndConstraint<BufferedStreamAssertions> NotHaveBufferSize(int unexpected, string? because = null, params object?[] becauseArgs) {
        if(_buffered is not null && _buffered.BufferSize == unexpected) {
            Fail(Strings.Format(StringKeys.BUFFERED_NOT_HAVE_SIZE, Expression, AssertionScope.FormatInvariant(unexpected)), because, becauseArgs);
        }

        return new(this);
    }
#endif
}
