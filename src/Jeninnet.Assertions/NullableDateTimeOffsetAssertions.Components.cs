//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class NullableDateTimeOffsetAssertions {
    /// <summary>Asserts that the year component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected year.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the year component differs.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> HaveYear(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Year != expected) {
            Fail(Strings.Format(StringKeys.DATE_HAVE_YEAR, Expression, AssertionScope.FormatInvariant(expected), _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Year)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the month component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected month (1-12).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the month component differs.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> HaveMonth(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Month != expected) {
            Fail(Strings.Format(StringKeys.DATE_HAVE_MONTH, Expression, AssertionScope.FormatInvariant(expected), _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Month)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the day component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected day of month.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the day component differs.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> HaveDay(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Day != expected) {
            Fail(Strings.Format(StringKeys.DATE_HAVE_DAY, Expression, AssertionScope.FormatInvariant(expected), _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Day)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the hour component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected hour (0-23).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the hour component differs.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> HaveHour(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Hour != expected) {
            Fail(Strings.Format(StringKeys.DATE_HAVE_HOUR, Expression, AssertionScope.FormatInvariant(expected), _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Hour)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the minute component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected minute (0-59).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the minute component differs.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> HaveMinute(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Minute != expected) {
            Fail(Strings.Format(StringKeys.DATE_HAVE_MINUTE, Expression, AssertionScope.FormatInvariant(expected), _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Minute)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the second component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected second (0-59).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the second component differs.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> HaveSecond(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Second != expected) {
            Fail(Strings.Format(StringKeys.DATE_HAVE_SECOND, Expression, AssertionScope.FormatInvariant(expected), _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Second)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the year component does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The year the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the year component matches.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> NotHaveYear(int unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Year == unexpected) {
            Fail(Strings.Format(StringKeys.DATE_NOT_HAVE_YEAR, Expression, AssertionScope.FormatInvariant(unexpected), _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Year)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the month component does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The month the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the month component matches.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> NotHaveMonth(int unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Month == unexpected) {
            Fail(Strings.Format(StringKeys.DATE_NOT_HAVE_MONTH, Expression, AssertionScope.FormatInvariant(unexpected), _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Month)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the day component does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The day of month the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the day component matches.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> NotHaveDay(int unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Day == unexpected) {
            Fail(Strings.Format(StringKeys.DATE_NOT_HAVE_DAY, Expression, AssertionScope.FormatInvariant(unexpected), _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Day)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the hour component does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The hour the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the hour component matches.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> NotHaveHour(int unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Hour == unexpected) {
            Fail(Strings.Format(StringKeys.DATE_NOT_HAVE_HOUR, Expression, AssertionScope.FormatInvariant(unexpected), _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Hour)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the minute component does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The minute the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the minute component matches.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> NotHaveMinute(int unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Minute == unexpected) {
            Fail(Strings.Format(StringKeys.DATE_NOT_HAVE_MINUTE, Expression, AssertionScope.FormatInvariant(unexpected), _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Minute)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the second component does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The second the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the second component matches.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> NotHaveSecond(int unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Second == unexpected) {
            Fail(Strings.Format(StringKeys.DATE_NOT_HAVE_SECOND, Expression, AssertionScope.FormatInvariant(unexpected), _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Second)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the millisecond component does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The millisecond the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the millisecond component matches.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> NotHaveMillisecond(int unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Millisecond == unexpected) {
            Fail(Strings.Format(StringKeys.DATE_NOT_HAVE_MILLISECOND, Expression, AssertionScope.FormatInvariant(unexpected), _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Millisecond)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the millisecond component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected millisecond (0-999).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the millisecond component differs.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> HaveMillisecond(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Millisecond != expected) {
            Fail(Strings.Format(StringKeys.DATE_HAVE_MILLISECOND, Expression, AssertionScope.FormatInvariant(expected), _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Millisecond)), because, becauseArgs);
        }

        return new(this);
    }
}
