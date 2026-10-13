//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class DateTimeAssertions {
    /// <summary>Asserts that the year component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected year.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The year component differs.</exception>
    public AndConstraint<DateTimeAssertions> HaveYear(int expected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<DateTimeAssertions> HaveMonth(int expected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<DateTimeAssertions> HaveDay(int expected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<DateTimeAssertions> HaveHour(int expected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<DateTimeAssertions> HaveMinute(int expected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<DateTimeAssertions> HaveSecond(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value.Second != expected) {
            Fail(Strings.Format(StringKeys.DATE_HAVE_SECOND, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_value.Second)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the year component does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The year the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The year component matches.</exception>
    public AndConstraint<DateTimeAssertions> NotHaveYear(int unexpected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<DateTimeAssertions> NotHaveMonth(int unexpected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<DateTimeAssertions> NotHaveDay(int unexpected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<DateTimeAssertions> NotHaveHour(int unexpected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<DateTimeAssertions> NotHaveMinute(int unexpected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<DateTimeAssertions> NotHaveSecond(int unexpected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<DateTimeAssertions> NotHaveMillisecond(int unexpected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<DateTimeAssertions> HaveMillisecond(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value.Millisecond != expected) {
            Fail(Strings.Format(StringKeys.DATE_HAVE_MILLISECOND, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_value.Millisecond)), because, becauseArgs);
        }

        return new(this);
    }
}
