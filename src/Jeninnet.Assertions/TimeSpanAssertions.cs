//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying <see cref="TimeSpan"/> values.</summary>
/// <param name="value">The value to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public class TimeSpanAssertions(TimeSpan value, string? expression = null) : AssertionsBase(expression, "time span") {
    private readonly TimeSpan _value = value;

    private static string Format(TimeSpan value) => AssertionScope.FormatInvariant(value);

    /// <summary>Asserts that the value equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The values are not equal.</exception>
    public AndConstraint<TimeSpanAssertions> Be(TimeSpan expected, string? because = null, params object?[] becauseArgs) {
        if(_value != expected) {
            Fail(Strings.Format(StringKeys.DATE_BE, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The value the subject must not equal.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The values are equal.</exception>
    public AndConstraint<TimeSpanAssertions> NotBe(TimeSpan unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value == unexpected) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE, Expression, Format(unexpected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is positive (zero is neither positive nor negative).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is zero or negative.</exception>
    public AndConstraint<TimeSpanAssertions> BePositive(string? because = null, params object?[] becauseArgs) {
        if(_value <= TimeSpan.Zero) {
            Fail(Strings.Format(StringKeys.TIMESPAN_BE_POSITIVE, Expression, Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is negative (zero is neither positive nor negative).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is zero or positive.</exception>
    public AndConstraint<TimeSpanAssertions> BeNegative(string? because = null, params object?[] becauseArgs) {
        if(_value >= TimeSpan.Zero) {
            Fail(Strings.Format(StringKeys.TIMESPAN_BE_NEGATIVE, Expression, Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is greater than <paramref name="expected"/>.</summary>
    /// <param name="expected">The exclusive lower bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is less than or equal to <paramref name="expected"/>.</exception>
    public AndConstraint<TimeSpanAssertions> BeGreaterThan(TimeSpan expected, string? because = null, params object?[] becauseArgs) {
        if(_value.CompareTo(expected) <= 0) {
            Fail(Strings.Format(StringKeys.TIMESPAN_BE_GREATER_THAN, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is greater than or equal to <paramref name="expected"/>.</summary>
    /// <param name="expected">The inclusive lower bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is less than <paramref name="expected"/>.</exception>
    public AndConstraint<TimeSpanAssertions> BeGreaterThanOrEqualTo(TimeSpan expected, string? because = null, params object?[] becauseArgs) {
        if(_value.CompareTo(expected) < 0) {
            Fail(Strings.Format(StringKeys.TIMESPAN_BE_GREATER_THAN_OR_EQUAL, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is less than <paramref name="expected"/>.</summary>
    /// <param name="expected">The exclusive upper bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is greater than or equal to <paramref name="expected"/>.</exception>
    public AndConstraint<TimeSpanAssertions> BeLessThan(TimeSpan expected, string? because = null, params object?[] becauseArgs) {
        if(_value.CompareTo(expected) >= 0) {
            Fail(Strings.Format(StringKeys.TIMESPAN_BE_LESS_THAN, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is less than or equal to <paramref name="expected"/>.</summary>
    /// <param name="expected">The inclusive upper bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is greater than <paramref name="expected"/>.</exception>
    public AndConstraint<TimeSpanAssertions> BeLessThanOrEqualTo(TimeSpan expected, string? because = null, params object?[] becauseArgs) {
        if(_value.CompareTo(expected) > 0) {
            Fail(Strings.Format(StringKeys.TIMESPAN_BE_LESS_THAN_OR_EQUAL, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is within <paramref name="precision"/> of <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="precision">The maximum accepted absolute difference (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="precision"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The value differs by more than <paramref name="precision"/>.</exception>
    public AndConstraint<TimeSpanAssertions> BeCloseTo(TimeSpan expected, TimeSpan precision, string? because = null, params object?[] becauseArgs) {
        if(precision < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(precision), "Precision must not be negative.");
        }

        if((_value - expected).Duration() > precision) {
            Fail(Strings.Format(StringKeys.DATE_BE_CLOSE_TO, Expression, Format(expected), Format(precision), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value differs by more than <paramref name="precision"/> from <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The value the subject must not approximate.</param>
    /// <param name="precision">The absolute difference that counts as close (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="precision"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The value is within <paramref name="precision"/> of <paramref name="unexpected"/>.</exception>
    public AndConstraint<TimeSpanAssertions> NotBeCloseTo(TimeSpan unexpected, TimeSpan precision, string? because = null, params object?[] becauseArgs) {
        if(precision < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(precision), "Precision must not be negative.");
        }

        if((_value - unexpected).Duration() <= precision) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE_CLOSE_TO, Expression, Format(unexpected), Format(precision), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the days component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected days component.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The days component differs.</exception>
    public AndConstraint<TimeSpanAssertions> HaveDays(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value.Days != expected) {
            Fail(Strings.Format(StringKeys.TIMESPAN_HAVE_DAYS, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_value.Days)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the hours component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected hours component (0-23).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The hours component differs.</exception>
    public AndConstraint<TimeSpanAssertions> HaveHours(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value.Hours != expected) {
            Fail(Strings.Format(StringKeys.TIMESPAN_HAVE_HOURS, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_value.Hours)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the minutes component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected minutes component (0-59).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The minutes component differs.</exception>
    public AndConstraint<TimeSpanAssertions> HaveMinutes(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value.Minutes != expected) {
            Fail(Strings.Format(StringKeys.TIMESPAN_HAVE_MINUTES, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_value.Minutes)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the seconds component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected seconds component (0-59).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The seconds component differs.</exception>
    public AndConstraint<TimeSpanAssertions> HaveSeconds(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value.Seconds != expected) {
            Fail(Strings.Format(StringKeys.TIMESPAN_HAVE_SECONDS, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_value.Seconds)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the milliseconds component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected milliseconds component (0-999).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The milliseconds component differs.</exception>
    public AndConstraint<TimeSpanAssertions> HaveMilliseconds(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value.Milliseconds != expected) {
            Fail(Strings.Format(StringKeys.TIMESPAN_HAVE_MILLISECONDS, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_value.Milliseconds)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the total ticks equal <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected total ticks.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The total ticks differ.</exception>
    public AndConstraint<TimeSpanAssertions> HaveTicks(long expected, string? because = null, params object?[] becauseArgs) {
        if(_value.Ticks != expected) {
            Fail(Strings.Format(StringKeys.TIMESPAN_HAVE_TICKS, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_value.Ticks)), because, becauseArgs);
        }

        return new(this);
    }
}
