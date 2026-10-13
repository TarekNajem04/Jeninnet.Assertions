//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class StringAssertions {
    /// <summary>Asserts that the string contains the specified line and exposes the first matching line for further assertions.</summary>
    /// <remarks>Matching is whole-line and ordinal (a trailing <c>\r</c> is ignored); partial-line matches do not count.</remarks>
    /// <param name="expectedLine">The full line to search for.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndWhichConstraint{TParent, T}"/> wrapping the first matching line.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expectedLine"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string is <see langword="null"/> or has no such line.</exception>
    public AndWhichConstraint<StringAssertions, string> ContainLine(string expectedLine, string? because = null, params object?[] becauseArgs) {
        if(expectedLine is null) {
            throw new ArgumentNullException(nameof(expectedLine));
        }

        var match = _value?.Split('\n').FirstOrDefault(line => line.TrimEnd('\r') == expectedLine);
        if(match is null) {
            Fail(Strings.Format(StringKeys.STRING_CONTAIN_LINE, Expression, Format(expectedLine)), because, becauseArgs);
            return new AndWhichConstraint<StringAssertions, string>(this, expectedLine);
        }

        return new AndWhichConstraint<StringAssertions, string>(this, match);
    }

    /// <summary>Asserts that the string does not contain the specified line.</summary>
    /// <param name="line">The line that must not be present.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="line"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The string contains the line.</exception>
    public AndConstraint<StringAssertions> NotContainLine(string line, string? because = null, params object?[] becauseArgs) {
        if(line is null) {
            throw new ArgumentNullException(nameof(line));
        }

        if(_value is not null && _value.Split('\n').Any(l => l.TrimEnd('\r') == line)) {
            Fail(Strings.Format(StringKeys.STRING_NOT_CONTAIN_LINE, Expression, Format(line)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string has exactly <paramref name="expected"/> lines.</summary>
    /// <remarks>Lines are split on <c>\n</c>: <c>"a\nb"</c> has 2 lines, <c>"a\nb\n"</c> has 3, and <c>""</c> has 0.</remarks>
    /// <param name="expected">The expected line count.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The string is <see langword="null"/> or has a different line count.</exception>
    public AndConstraint<StringAssertions> HaveLineCount(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.STRING_HAVE_LINE_COUNT_NULL, Expression, AssertionScope.FormatInvariant(expected)), because, becauseArgs);
            return new(this);
        }

        var lineCount = _value.Length == 0 ? 0 : _value.Split('\n').Length;
        if(lineCount != expected) {
            Fail(Strings.Format(StringKeys.STRING_HAVE_LINE_COUNT, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(lineCount)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the string does not have the specified number of lines.</summary>
    /// <param name="unexpected">The line count that must not match.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The string has the specified number of lines.</exception>
    public AndConstraint<StringAssertions> NotHaveLineCount(int unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value is not null) {
            var lineCount = _value.Length == 0 ? 0 : _value.Split('\n').Length;
            if(lineCount == unexpected) {
                Fail(Strings.Format(StringKeys.STRING_NOT_HAVE_LINE_COUNT, Expression, AssertionScope.FormatInvariant(unexpected), AssertionScope.FormatInvariant(lineCount)), because, becauseArgs);
            }
        }

        return new(this);
    }
}
