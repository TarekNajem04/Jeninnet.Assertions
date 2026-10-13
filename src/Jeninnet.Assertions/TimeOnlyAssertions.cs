//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Platform/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying <see cref="TimeOnly"/> values.</summary>
/// <param name="value">The value to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public class TimeOnlyAssertions(TimeOnly value, string? expression = null) : AssertionsBase(expression, "time") {
    private readonly TimeOnly _value = value;

    /// <summary>Formats a time invariantly for failure messages.</summary>
    /// <param name="value">The value to format.</param>
    /// <returns>The invariant text.</returns>
    private static string Format(TimeOnly value) => AssertionScope.FormatInvariant(value);

    /// <summary>Computes the shortest clock-face distance between two times, wrapping around midnight.</summary>
    /// <param name="first">The first time.</param>
    /// <param name="second">The second time.</param>
    /// <returns>The smaller of the forward and wrapped-around distances.</returns>
    private static TimeSpan ClockDistance(TimeOnly first, TimeOnly second) {
        var forward = (first - second).Duration();
        var wrapped = TimeSpan.FromHours(24) - forward;
        return forward <= wrapped ? forward : wrapped;
    }

    /// <summary>Asserts that the value equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The values are not equal.</exception>
    public AndConstraint<TimeOnlyAssertions> Be(TimeOnly expected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<TimeOnlyAssertions> NotBe(TimeOnly unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value == unexpected) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE, Expression, Format(unexpected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is before <paramref name="expected"/>.</summary>
    /// <param name="expected">The exclusive upper bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is not before <paramref name="expected"/>.</exception>
    public AndConstraint<TimeOnlyAssertions> BeBefore(TimeOnly expected, string? because = null, params object?[] becauseArgs) {
        if(_value.CompareTo(expected) >= 0) {
            Fail(Strings.Format(StringKeys.DATE_BE_BEFORE, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is not before <paramref name="expected"/>.</summary>
    /// <param name="expected">The value the subject must not precede.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is before <paramref name="expected"/>.</exception>
    public AndConstraint<TimeOnlyAssertions> NotBeBefore(TimeOnly expected, string? because = null, params object?[] becauseArgs) {
        if(_value.CompareTo(expected) < 0) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE_BEFORE, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is on or before <paramref name="expected"/>.</summary>
    /// <param name="expected">The inclusive upper bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is after <paramref name="expected"/>.</exception>
    public AndConstraint<TimeOnlyAssertions> BeOnOrBefore(TimeOnly expected, string? because = null, params object?[] becauseArgs) {
        if(_value.CompareTo(expected) > 0) {
            Fail(Strings.Format(StringKeys.DATE_BE_ON_OR_BEFORE, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is after <paramref name="expected"/>.</summary>
    /// <param name="expected">The exclusive lower bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is not after <paramref name="expected"/>.</exception>
    public AndConstraint<TimeOnlyAssertions> BeAfter(TimeOnly expected, string? because = null, params object?[] becauseArgs) {
        if(_value.CompareTo(expected) <= 0) {
            Fail(Strings.Format(StringKeys.DATE_BE_AFTER, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is not after <paramref name="expected"/>.</summary>
    /// <param name="expected">The value the subject must not follow.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is after <paramref name="expected"/>.</exception>
    public AndConstraint<TimeOnlyAssertions> NotBeAfter(TimeOnly expected, string? because = null, params object?[] becauseArgs) {
        if(_value.CompareTo(expected) > 0) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE_AFTER, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is on or after <paramref name="expected"/>.</summary>
    /// <param name="expected">The inclusive lower bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is before <paramref name="expected"/>.</exception>
    public AndConstraint<TimeOnlyAssertions> BeOnOrAfter(TimeOnly expected, string? because = null, params object?[] becauseArgs) {
        if(_value.CompareTo(expected) < 0) {
            Fail(Strings.Format(StringKeys.DATE_BE_ON_OR_AFTER, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is within <paramref name="precision"/> of <paramref name="expected"/> (shortest distance around the clock face).</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="precision">The maximum accepted absolute difference (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="precision"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The value differs by more than <paramref name="precision"/>.</exception>
    public AndConstraint<TimeOnlyAssertions> BeCloseTo(TimeOnly expected, TimeSpan precision, string? because = null, params object?[] becauseArgs) {
        if(precision < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(precision), "Precision must not be negative.");
        }

        if(ClockDistance(_value, expected) > precision) {
            Fail(Strings.Format(StringKeys.DATE_BE_CLOSE_TO, Expression, Format(expected), AssertionScope.FormatInvariant(precision), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value differs by more than <paramref name="precision"/> from <paramref name="unexpected"/> (shortest distance around the clock face).</summary>
    /// <param name="unexpected">The value the subject must not approximate.</param>
    /// <param name="precision">The absolute difference that counts as close (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="precision"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The value is within <paramref name="precision"/> of <paramref name="unexpected"/>.</exception>
    public AndConstraint<TimeOnlyAssertions> NotBeCloseTo(TimeOnly unexpected, TimeSpan precision, string? because = null, params object?[] becauseArgs) {
        if(precision < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(precision), "Precision must not be negative.");
        }

        if(ClockDistance(_value, unexpected) <= precision) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE_CLOSE_TO, Expression, Format(unexpected), AssertionScope.FormatInvariant(precision), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the hour component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected hour (0-23).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The hour component differs.</exception>
    public AndConstraint<TimeOnlyAssertions> HaveHour(int expected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<TimeOnlyAssertions> HaveMinute(int expected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<TimeOnlyAssertions> HaveSecond(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value.Second != expected) {
            Fail(Strings.Format(StringKeys.DATE_HAVE_SECOND, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_value.Second)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the millisecond component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected millisecond (0-999).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The millisecond component differs.</exception>
    public AndConstraint<TimeOnlyAssertions> HaveMillisecond(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value.Millisecond != expected) {
            Fail(Strings.Format(StringKeys.DATE_HAVE_MILLISECOND, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_value.Millisecond)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value equals one of the <paramref name="expected"/> values.</summary>
    /// <param name="expected">The candidate values, one of which must equal the subject.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The value matches none of the candidates.</exception>
    public AndConstraint<TimeOnlyAssertions> BeOneOf(TimeOnly[] expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(!expected.Contains(_value)) {
            Fail(Strings.Format(StringKeys.DATE_BE_ONE_OF, Expression, string.Join(", ", expected.Select(Format)), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value equals one of the <paramref name="expected"/> values.</summary>
    /// <param name="expected">The candidate values, one of which must equal the subject.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value matches none of the candidates.</exception>
    public AndConstraint<TimeOnlyAssertions> BeOneOf(params TimeOnly[] expected) => BeOneOf(expected, null, []);

    /// <summary>Asserts that the value is not on or after <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The value the subject must strictly precede.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is on or after <paramref name="unexpected"/>.</exception>
    public AndConstraint<TimeOnlyAssertions> NotBeOnOrAfter(TimeOnly unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value.CompareTo(unexpected) >= 0) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE_ON_OR_AFTER, Expression, Format(unexpected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is not on or before <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The value the subject must strictly follow.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is on or before <paramref name="unexpected"/>.</exception>
    public AndConstraint<TimeOnlyAssertions> NotBeOnOrBefore(TimeOnly unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value.CompareTo(unexpected) <= 0) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE_ON_OR_BEFORE, Expression, Format(unexpected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the hour component does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The hour the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The hour component matches.</exception>
    public AndConstraint<TimeOnlyAssertions> NotHaveHour(int unexpected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<TimeOnlyAssertions> NotHaveMinute(int unexpected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<TimeOnlyAssertions> NotHaveSecond(int unexpected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<TimeOnlyAssertions> NotHaveMillisecond(int unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value.Millisecond == unexpected) {
            Fail(Strings.Format(StringKeys.DATE_NOT_HAVE_MILLISECOND, Expression, AssertionScope.FormatInvariant(unexpected), AssertionScope.FormatInvariant(_value.Millisecond)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the hour component equals <paramref name="expected"/> (plural alias of <see cref="HaveHour"/>).</summary>
    /// <param name="expected">The expected hour (0-23).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The hour component differs.</exception>
    public AndConstraint<TimeOnlyAssertions> HaveHours(int expected, string? because = null, params object?[] becauseArgs) =>
        HaveHour(expected, because, becauseArgs);

    /// <summary>Asserts that the minute component equals <paramref name="expected"/> (plural alias of <see cref="HaveMinute"/>).</summary>
    /// <param name="expected">The expected minute (0-59).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The minute component differs.</exception>
    public AndConstraint<TimeOnlyAssertions> HaveMinutes(int expected, string? because = null, params object?[] becauseArgs) =>
        HaveMinute(expected, because, becauseArgs);

    /// <summary>Asserts that the second component equals <paramref name="expected"/> (plural alias of <see cref="HaveSecond"/>).</summary>
    /// <param name="expected">The expected second (0-59).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The second component differs.</exception>
    public AndConstraint<TimeOnlyAssertions> HaveSeconds(int expected, string? because = null, params object?[] becauseArgs) =>
        HaveSecond(expected, because, becauseArgs);

    /// <summary>Asserts that the millisecond component equals <paramref name="expected"/> (plural alias of <see cref="HaveMillisecond"/>).</summary>
    /// <param name="expected">The expected millisecond (0-999).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The millisecond component differs.</exception>
    public AndConstraint<TimeOnlyAssertions> HaveMilliseconds(int expected, string? because = null, params object?[] becauseArgs) =>
        HaveMillisecond(expected, because, becauseArgs);

    /// <summary>Asserts that the hour component does not equal <paramref name="unexpected"/> (plural alias of <see cref="NotHaveHour"/>).</summary>
    /// <param name="unexpected">The hour the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The hour component matches.</exception>
    public AndConstraint<TimeOnlyAssertions> NotHaveHours(int unexpected, string? because = null, params object?[] becauseArgs) =>
        NotHaveHour(unexpected, because, becauseArgs);

    /// <summary>Asserts that the minute component does not equal <paramref name="unexpected"/> (plural alias of <see cref="NotHaveMinute"/>).</summary>
    /// <param name="unexpected">The minute the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The minute component matches.</exception>
    public AndConstraint<TimeOnlyAssertions> NotHaveMinutes(int unexpected, string? because = null, params object?[] becauseArgs) =>
        NotHaveMinute(unexpected, because, becauseArgs);

    /// <summary>Asserts that the second component does not equal <paramref name="unexpected"/> (plural alias of <see cref="NotHaveSecond"/>).</summary>
    /// <param name="unexpected">The second the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The second component matches.</exception>
    public AndConstraint<TimeOnlyAssertions> NotHaveSeconds(int unexpected, string? because = null, params object?[] becauseArgs) =>
        NotHaveSecond(unexpected, because, becauseArgs);

    /// <summary>Asserts that the millisecond component does not equal <paramref name="unexpected"/> (plural alias of <see cref="NotHaveMillisecond"/>).</summary>
    /// <param name="unexpected">The millisecond the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The millisecond component matches.</exception>
    public AndConstraint<TimeOnlyAssertions> NotHaveMilliseconds(int unexpected, string? because = null, params object?[] becauseArgs) =>
        NotHaveMillisecond(unexpected, because, becauseArgs);
}
