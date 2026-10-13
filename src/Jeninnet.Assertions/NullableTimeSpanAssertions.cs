//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying nullable <see cref="TimeSpan"/> values.</summary>
/// <remarks>
/// Positive assertions fail on a <see langword="null"/> subject, while negative assertions
/// (<c>NotBe</c>, <c>NotBeCloseTo</c>) pass on <see langword="null"/> — except <c>HaveValue</c>,
/// which fails. Null renders as <c>&lt;null&gt;</c>.
/// </remarks>
/// <param name="value">The nullable value to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public class NullableTimeSpanAssertions(TimeSpan? value, string? expression = null) : AssertionsBase(expression, "time span") {
    private readonly TimeSpan? _value = value;
    private const string NULL_STRING = "<null>";

    private static string Format(TimeSpan? value) => value is null ? NULL_STRING : AssertionScope.FormatInvariant(value);

    /// <summary>Asserts that the value equals <paramref name="expected"/> (with <see langword="null"/> equal to <see langword="null"/>).</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The values are not equal.</exception>
    public AndConstraint<NullableTimeSpanAssertions> Be(TimeSpan? expected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<NullableTimeSpanAssertions> NotBe(TimeSpan? unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value == unexpected) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE, Expression, Format(unexpected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is positive (zero is neither positive nor negative).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/>, zero, or negative.</exception>
    public AndConstraint<NullableTimeSpanAssertions> BePositive(string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value <= TimeSpan.Zero) {
            Fail(Strings.Format(StringKeys.TIMESPAN_BE_POSITIVE, Expression, Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is negative (zero is neither positive nor negative).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/>, zero, or positive.</exception>
    public AndConstraint<NullableTimeSpanAssertions> BeNegative(string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value >= TimeSpan.Zero) {
            Fail(Strings.Format(StringKeys.TIMESPAN_BE_NEGATIVE, Expression, Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is greater than <paramref name="expected"/>.</summary>
    /// <param name="expected">The exclusive lower bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or not greater than <paramref name="expected"/>.</exception>
    public AndConstraint<NullableTimeSpanAssertions> BeGreaterThan(TimeSpan expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.CompareTo(expected) <= 0) {
            Fail(Strings.Format(StringKeys.TIMESPAN_BE_GREATER_THAN, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is greater than or equal to <paramref name="expected"/>.</summary>
    /// <param name="expected">The inclusive lower bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or less than <paramref name="expected"/>.</exception>
    public AndConstraint<NullableTimeSpanAssertions> BeGreaterThanOrEqualTo(TimeSpan expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.CompareTo(expected) < 0) {
            Fail(Strings.Format(StringKeys.TIMESPAN_BE_GREATER_THAN_OR_EQUAL, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is less than <paramref name="expected"/>.</summary>
    /// <param name="expected">The exclusive upper bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or not less than <paramref name="expected"/>.</exception>
    public AndConstraint<NullableTimeSpanAssertions> BeLessThan(TimeSpan expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.CompareTo(expected) >= 0) {
            Fail(Strings.Format(StringKeys.TIMESPAN_BE_LESS_THAN, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is less than or equal to <paramref name="expected"/>.</summary>
    /// <param name="expected">The inclusive upper bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or greater than <paramref name="expected"/>.</exception>
    public AndConstraint<NullableTimeSpanAssertions> BeLessThanOrEqualTo(TimeSpan expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.CompareTo(expected) > 0) {
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
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or differs by more than <paramref name="precision"/>.</exception>
    public AndConstraint<NullableTimeSpanAssertions> BeCloseTo(TimeSpan expected, TimeSpan precision, string? because = null, params object?[] becauseArgs) {
        if(precision < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(precision), "Precision must not be negative.");
        }

        if(_value is null || (_value.Value - expected).Duration() > precision) {
            Fail(Strings.Format(StringKeys.DATE_BE_CLOSE_TO, Expression, Format(expected), Format(precision), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value differs by more than <paramref name="precision"/> from <paramref name="unexpected"/>. A <see langword="null"/> subject passes.</summary>
    /// <param name="unexpected">The value the subject must not approximate.</param>
    /// <param name="precision">The absolute difference that counts as close (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="precision"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The value is within <paramref name="precision"/> of <paramref name="unexpected"/>.</exception>
    public AndConstraint<NullableTimeSpanAssertions> NotBeCloseTo(TimeSpan unexpected, TimeSpan precision, string? because = null, params object?[] becauseArgs) {
        if(precision < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(precision), "Precision must not be negative.");
        }

        if(_value is not null && (_value.Value - unexpected).Duration() <= precision) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE_CLOSE_TO, Expression, Format(unexpected), Format(precision), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the days component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected days component.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the days component differs.</exception>
    public AndConstraint<NullableTimeSpanAssertions> HaveDays(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Days != expected) {
            Fail(Strings.Format(StringKeys.TIMESPAN_HAVE_DAYS, Expression, AssertionScope.FormatInvariant(expected), FormatDays()), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the hours component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected hours component (0-23).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the hours component differs.</exception>
    public AndConstraint<NullableTimeSpanAssertions> HaveHours(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Hours != expected) {
            Fail(Strings.Format(StringKeys.TIMESPAN_HAVE_HOURS, Expression, AssertionScope.FormatInvariant(expected), FormatHours()), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the minutes component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected minutes component (0-59).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the minutes component differs.</exception>
    public AndConstraint<NullableTimeSpanAssertions> HaveMinutes(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Minutes != expected) {
            Fail(Strings.Format(StringKeys.TIMESPAN_HAVE_MINUTES, Expression, AssertionScope.FormatInvariant(expected), FormatMinutes()), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the seconds component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected seconds component (0-59).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the seconds component differs.</exception>
    public AndConstraint<NullableTimeSpanAssertions> HaveSeconds(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Seconds != expected) {
            Fail(Strings.Format(StringKeys.TIMESPAN_HAVE_SECONDS, Expression, AssertionScope.FormatInvariant(expected), FormatSeconds()), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the milliseconds component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected milliseconds component (0-999).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the milliseconds component differs.</exception>
    public AndConstraint<NullableTimeSpanAssertions> HaveMilliseconds(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Milliseconds != expected) {
            Fail(Strings.Format(StringKeys.TIMESPAN_HAVE_MILLISECONDS, Expression, AssertionScope.FormatInvariant(expected), FormatMilliseconds()), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the total ticks equal <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected total ticks.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the total ticks differ.</exception>
    public AndConstraint<NullableTimeSpanAssertions> HaveTicks(long expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Ticks != expected) {
            Fail(Strings.Format(StringKeys.TIMESPAN_HAVE_TICKS, Expression, AssertionScope.FormatInvariant(expected), FormatTicks()), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is not <see langword="null"/>.</exception>
    public AndConstraint<NullableTimeSpanAssertions> BeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is not null) {
            Fail(Strings.Format(StringKeys.DATE_BE_NULL, Expression, Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value has a value (is not <see langword="null"/>).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/>.</exception>
    public AndConstraint<NullableTimeSpanAssertions> HaveValue(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value has no value (is <see langword="null"/>).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is not <see langword="null"/>.</exception>
    public AndConstraint<NullableTimeSpanAssertions> NotHaveValue(string? because = null, params object?[] becauseArgs) =>
        BeNull(because, becauseArgs);

    private string FormatDays() => _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Days);

    private string FormatHours() => _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Hours);

    private string FormatMinutes() => _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Minutes);

    private string FormatSeconds() => _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Seconds);

    private string FormatMilliseconds() => _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Milliseconds);

    private string FormatTicks() => _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Ticks);
}
