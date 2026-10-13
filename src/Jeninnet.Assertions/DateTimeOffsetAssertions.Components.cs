//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class DateTimeOffsetAssertions {
    /// <summary>Asserts that the year component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected year.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The year component differs.</exception>
    public AndConstraint<DateTimeOffsetAssertions> HaveYear(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value.Year != expected) {
            Fail(Strings.Format(StringKeys.DATE_HAVE_YEAR, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_value.Year)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the month component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected month (1-12).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The month component differs.</exception>
    public AndConstraint<DateTimeOffsetAssertions> HaveMonth(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value.Month != expected) {
            Fail(Strings.Format(StringKeys.DATE_HAVE_MONTH, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_value.Month)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the day component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected day of month.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The day component differs.</exception>
    public AndConstraint<DateTimeOffsetAssertions> HaveDay(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value.Day != expected) {
            Fail(Strings.Format(StringKeys.DATE_HAVE_DAY, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_value.Day)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the hour component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected hour (0-23).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The hour component differs.</exception>
    public AndConstraint<DateTimeOffsetAssertions> HaveHour(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value.Hour != expected) {
            Fail(Strings.Format(StringKeys.DATE_HAVE_HOUR, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_value.Hour)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the minute component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected minute (0-59).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The minute component differs.</exception>
    public AndConstraint<DateTimeOffsetAssertions> HaveMinute(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value.Minute != expected) {
            Fail(Strings.Format(StringKeys.DATE_HAVE_MINUTE, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_value.Minute)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the second component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected second (0-59).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The second component differs.</exception>
    public AndConstraint<DateTimeOffsetAssertions> HaveSecond(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value.Second != expected) {
            Fail(Strings.Format(StringKeys.DATE_HAVE_SECOND, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_value.Second)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the UTC offset equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected offset from UTC.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The offset differs.</exception>
    public AndConstraint<DateTimeOffsetAssertions> HaveOffset(TimeSpan expected, string? because = null, params object?[] becauseArgs) {
        if(_value.Offset != expected) {
            Fail(Strings.Format(StringKeys.DATE_HAVE_OFFSET, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_value.Offset)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the UTC offset does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The offset the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The offset matches.</exception>
    public AndConstraint<DateTimeOffsetAssertions> NotHaveOffset(TimeSpan unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value.Offset == unexpected) {
            Fail(Strings.Format(StringKeys.DATE_NOT_HAVE_OFFSET, Expression, AssertionScope.FormatInvariant(unexpected), AssertionScope.FormatInvariant(_value.Offset)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is exactly <paramref name="expected"/>: the same instant and the same offset.</summary>
    /// <remarks>Unlike <see cref="Be"/>, which compares instants only, this also requires the offsets to match.</remarks>
    /// <param name="expected">The expected instant and offset.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The instant or the offset differs.</exception>
    public AndConstraint<DateTimeOffsetAssertions> BeExactly(DateTimeOffset expected, string? because = null, params object?[] becauseArgs) {
        if(_value.UtcTicks != expected.UtcTicks || _value.Offset != expected.Offset) {
            Fail(Strings.Format(StringKeys.DATE_BE_EXACTLY, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is not exactly <paramref name="unexpected"/> (instant and offset both match).</summary>
    /// <param name="unexpected">The instant and offset the subject must differ from.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">Both the instant and the offset match.</exception>
    public AndConstraint<DateTimeOffsetAssertions> NotBeExactly(DateTimeOffset unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value.UtcTicks == unexpected.UtcTicks && _value.Offset == unexpected.Offset) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE_EXACTLY, Expression, Format(unexpected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the year component does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The year the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The year component matches.</exception>
    public AndConstraint<DateTimeOffsetAssertions> NotHaveYear(int unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value.Year == unexpected) {
            Fail(Strings.Format(StringKeys.DATE_NOT_HAVE_YEAR, Expression, AssertionScope.FormatInvariant(unexpected), AssertionScope.FormatInvariant(_value.Year)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the month component does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The month the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The month component matches.</exception>
    public AndConstraint<DateTimeOffsetAssertions> NotHaveMonth(int unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value.Month == unexpected) {
            Fail(Strings.Format(StringKeys.DATE_NOT_HAVE_MONTH, Expression, AssertionScope.FormatInvariant(unexpected), AssertionScope.FormatInvariant(_value.Month)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the day component does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The day of month the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The day component matches.</exception>
    public AndConstraint<DateTimeOffsetAssertions> NotHaveDay(int unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value.Day == unexpected) {
            Fail(Strings.Format(StringKeys.DATE_NOT_HAVE_DAY, Expression, AssertionScope.FormatInvariant(unexpected), AssertionScope.FormatInvariant(_value.Day)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the hour component does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The hour the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The hour component matches.</exception>
    public AndConstraint<DateTimeOffsetAssertions> NotHaveHour(int unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value.Hour == unexpected) {
            Fail(Strings.Format(StringKeys.DATE_NOT_HAVE_HOUR, Expression, AssertionScope.FormatInvariant(unexpected), AssertionScope.FormatInvariant(_value.Hour)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the minute component does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The minute the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The minute component matches.</exception>
    public AndConstraint<DateTimeOffsetAssertions> NotHaveMinute(int unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value.Minute == unexpected) {
            Fail(Strings.Format(StringKeys.DATE_NOT_HAVE_MINUTE, Expression, AssertionScope.FormatInvariant(unexpected), AssertionScope.FormatInvariant(_value.Minute)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the second component does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The second the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The second component matches.</exception>
    public AndConstraint<DateTimeOffsetAssertions> NotHaveSecond(int unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value.Second == unexpected) {
            Fail(Strings.Format(StringKeys.DATE_NOT_HAVE_SECOND, Expression, AssertionScope.FormatInvariant(unexpected), AssertionScope.FormatInvariant(_value.Second)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the millisecond component does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The millisecond the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The millisecond component matches.</exception>
    public AndConstraint<DateTimeOffsetAssertions> NotHaveMillisecond(int unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value.Millisecond == unexpected) {
            Fail(Strings.Format(StringKeys.DATE_NOT_HAVE_MILLISECOND, Expression, AssertionScope.FormatInvariant(unexpected), AssertionScope.FormatInvariant(_value.Millisecond)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the millisecond component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected millisecond (0-999).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The millisecond component differs.</exception>
    public AndConstraint<DateTimeOffsetAssertions> HaveMillisecond(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value.Millisecond != expected) {
            Fail(Strings.Format(StringKeys.DATE_HAVE_MILLISECOND, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_value.Millisecond)), because, becauseArgs);
        }

        return new(this);
    }
}
