//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Platform/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying nullable <see cref="TimeOnly"/> values.</summary>
/// <remarks>
/// Positive assertions fail on a <see langword="null"/> subject, while negative assertions
/// (<c>NotBe</c>, <c>NotBeBefore</c>, <c>NotBeAfter</c>, <c>NotBeCloseTo</c>) pass on
/// <see langword="null"/> — except <c>HaveValue</c>, which fails. Null renders as <c>&lt;null&gt;</c>.
/// </remarks>
/// <param name="value">The nullable value to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public class NullableTimeOnlyAssertions(TimeOnly? value, string? expression = null) : AssertionsBase(expression, "time") {
    private readonly TimeOnly? _value = value;
    private const string NULL_STRING = "<null>";

    private static string Format(TimeOnly? value) => value is null ? NULL_STRING : AssertionScope.FormatInvariant(value);

    /// <summary>Computes the shortest clock-face distance between two times, wrapping around midnight.</summary>
    /// <param name="first">The first time.</param>
    /// <param name="second">The second time.</param>
    /// <returns>The smaller of the forward and wrapped-around distances.</returns>
    private static TimeSpan ClockDistance(TimeOnly first, TimeOnly second) {
        var forward = (first - second).Duration();
        var wrapped = TimeSpan.FromHours(24) - forward;
        return forward <= wrapped ? forward : wrapped;
    }

    /// <summary>Asserts that the value equals <paramref name="expected"/> (with <see langword="null"/> equal to <see langword="null"/>).</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The values are not equal.</exception>
    public AndConstraint<NullableTimeOnlyAssertions> Be(TimeOnly? expected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<NullableTimeOnlyAssertions> NotBe(TimeOnly? unexpected, string? because = null, params object?[] becauseArgs) {
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
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or not before <paramref name="expected"/>.</exception>
    public AndConstraint<NullableTimeOnlyAssertions> BeBefore(TimeOnly expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.CompareTo(expected) >= 0) {
            Fail(Strings.Format(StringKeys.DATE_BE_BEFORE, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is not before <paramref name="expected"/>. A <see langword="null"/> subject passes.</summary>
    /// <param name="expected">The value the subject must not precede.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is before <paramref name="expected"/>.</exception>
    public AndConstraint<NullableTimeOnlyAssertions> NotBeBefore(TimeOnly expected, string? because = null, params object?[] becauseArgs) {
        if(_value?.CompareTo(expected) < 0) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE_BEFORE, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is on or before <paramref name="expected"/>.</summary>
    /// <param name="expected">The inclusive upper bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or after <paramref name="expected"/>.</exception>
    public AndConstraint<NullableTimeOnlyAssertions> BeOnOrBefore(TimeOnly expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.CompareTo(expected) > 0) {
            Fail(Strings.Format(StringKeys.DATE_BE_ON_OR_BEFORE, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is after <paramref name="expected"/>.</summary>
    /// <param name="expected">The exclusive lower bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or not after <paramref name="expected"/>.</exception>
    public AndConstraint<NullableTimeOnlyAssertions> BeAfter(TimeOnly expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.CompareTo(expected) <= 0) {
            Fail(Strings.Format(StringKeys.DATE_BE_AFTER, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is not after <paramref name="expected"/>. A <see langword="null"/> subject passes.</summary>
    /// <param name="expected">The value the subject must not follow.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is after <paramref name="expected"/>.</exception>
    public AndConstraint<NullableTimeOnlyAssertions> NotBeAfter(TimeOnly expected, string? because = null, params object?[] becauseArgs) {
        if(_value?.CompareTo(expected) > 0) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE_AFTER, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is on or after <paramref name="expected"/>.</summary>
    /// <param name="expected">The inclusive lower bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or before <paramref name="expected"/>.</exception>
    public AndConstraint<NullableTimeOnlyAssertions> BeOnOrAfter(TimeOnly expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.CompareTo(expected) < 0) {
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
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or differs by more than <paramref name="precision"/>.</exception>
    public AndConstraint<NullableTimeOnlyAssertions> BeCloseTo(TimeOnly expected, TimeSpan precision, string? because = null, params object?[] becauseArgs) {
        if(precision < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(precision), "Precision must not be negative.");
        }

        if(_value is null || ClockDistance(_value.Value, expected) > precision) {
            Fail(Strings.Format(StringKeys.DATE_BE_CLOSE_TO, Expression, Format(expected), AssertionScope.FormatInvariant(precision), Format(_value)), because, becauseArgs);
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
    public AndConstraint<NullableTimeOnlyAssertions> NotBeCloseTo(TimeOnly unexpected, TimeSpan precision, string? because = null, params object?[] becauseArgs) {
        if(precision < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(precision), "Precision must not be negative.");
        }

        if(_value is not null && ClockDistance(_value.Value, unexpected) <= precision) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE_CLOSE_TO, Expression, Format(unexpected), AssertionScope.FormatInvariant(precision), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the hour component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected hour (0-23).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the hour component differs.</exception>
    public AndConstraint<NullableTimeOnlyAssertions> HaveHour(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Hour != expected) {
            Fail(Strings.Format(StringKeys.DATE_HAVE_HOUR, Expression, AssertionScope.FormatInvariant(expected), FormatHour()), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the minute component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected minute (0-59).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the minute component differs.</exception>
    public AndConstraint<NullableTimeOnlyAssertions> HaveMinute(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Minute != expected) {
            Fail(Strings.Format(StringKeys.DATE_HAVE_MINUTE, Expression, AssertionScope.FormatInvariant(expected), FormatMinute()), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the second component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected second (0-59).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the second component differs.</exception>
    public AndConstraint<NullableTimeOnlyAssertions> HaveSecond(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Second != expected) {
            Fail(Strings.Format(StringKeys.DATE_HAVE_SECOND, Expression, AssertionScope.FormatInvariant(expected), FormatSecond()), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the millisecond component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected millisecond (0-999).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the millisecond component differs.</exception>
    public AndConstraint<NullableTimeOnlyAssertions> HaveMillisecond(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Millisecond != expected) {
            Fail(Strings.Format(StringKeys.DATE_HAVE_MILLISECOND, Expression, AssertionScope.FormatInvariant(expected), FormatMillisecond()), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the hour component equals <paramref name="expected"/> (plural alias of <see cref="HaveHour"/>).</summary>
    /// <param name="expected">The expected hour (0-23).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the hour component differs.</exception>
    public AndConstraint<NullableTimeOnlyAssertions> HaveHours(int expected, string? because = null, params object?[] becauseArgs) =>
        HaveHour(expected, because, becauseArgs);

    /// <summary>Asserts that the minute component equals <paramref name="expected"/> (plural alias of <see cref="HaveMinute"/>).</summary>
    /// <param name="expected">The expected minute (0-59).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the minute component differs.</exception>
    public AndConstraint<NullableTimeOnlyAssertions> HaveMinutes(int expected, string? because = null, params object?[] becauseArgs) =>
        HaveMinute(expected, because, becauseArgs);

    /// <summary>Asserts that the second component equals <paramref name="expected"/> (plural alias of <see cref="HaveSecond"/>).</summary>
    /// <param name="expected">The expected second (0-59).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the second component differs.</exception>
    public AndConstraint<NullableTimeOnlyAssertions> HaveSeconds(int expected, string? because = null, params object?[] becauseArgs) =>
        HaveSecond(expected, because, becauseArgs);

    /// <summary>Asserts that the millisecond component equals <paramref name="expected"/> (plural alias of <see cref="HaveMillisecond"/>).</summary>
    /// <param name="expected">The expected millisecond (0-999).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the millisecond component differs.</exception>
    public AndConstraint<NullableTimeOnlyAssertions> HaveMilliseconds(int expected, string? because = null, params object?[] becauseArgs) =>
        HaveMillisecond(expected, because, becauseArgs);

    /// <summary>Asserts that the hour component does not equal <paramref name="unexpected"/> (plural alias of <see cref="NotHaveHour"/>).</summary>
    /// <param name="unexpected">The hour the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The hour component matches.</exception>
    public AndConstraint<NullableTimeOnlyAssertions> NotHaveHours(int unexpected, string? because = null, params object?[] becauseArgs) =>
        NotHaveHour(unexpected, because, becauseArgs);

    /// <summary>Asserts that the minute component does not equal <paramref name="unexpected"/> (plural alias of <see cref="NotHaveMinute"/>).</summary>
    /// <param name="unexpected">The minute the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The minute component matches.</exception>
    public AndConstraint<NullableTimeOnlyAssertions> NotHaveMinutes(int unexpected, string? because = null, params object?[] becauseArgs) =>
        NotHaveMinute(unexpected, because, becauseArgs);

    /// <summary>Asserts that the second component does not equal <paramref name="unexpected"/> (plural alias of <see cref="NotHaveSecond"/>).</summary>
    /// <param name="unexpected">The second the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The second component matches.</exception>
    public AndConstraint<NullableTimeOnlyAssertions> NotHaveSeconds(int unexpected, string? because = null, params object?[] becauseArgs) =>
        NotHaveSecond(unexpected, because, becauseArgs);

    /// <summary>Asserts that the millisecond component does not equal <paramref name="unexpected"/> (plural alias of <see cref="NotHaveMillisecond"/>).</summary>
    /// <param name="unexpected">The millisecond the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The millisecond component matches.</exception>
    public AndConstraint<NullableTimeOnlyAssertions> NotHaveMilliseconds(int unexpected, string? because = null, params object?[] becauseArgs) =>
        NotHaveMillisecond(unexpected, because, becauseArgs);

    /// <summary>Asserts that the value is <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is not <see langword="null"/>.</exception>
    public AndConstraint<NullableTimeOnlyAssertions> BeNull(string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<NullableTimeOnlyAssertions> HaveValue(string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<NullableTimeOnlyAssertions> NotHaveValue(string? because = null, params object?[] becauseArgs) =>
        BeNull(because, becauseArgs);

    /// <summary>Asserts that the value equals one of the <paramref name="expected"/> values.</summary>
    /// <param name="expected">The candidate values, one of which must equal the subject.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The value matches none of the candidates.</exception>
    public AndConstraint<NullableTimeOnlyAssertions> BeOneOf(TimeOnly?[] expected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<NullableTimeOnlyAssertions> BeOneOf(params TimeOnly?[] expected) => BeOneOf(expected, null, []);

    /// <summary>Asserts that the value is not on or after <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The value the subject must strictly precede.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is on or after <paramref name="unexpected"/>.</exception>
    public AndConstraint<NullableTimeOnlyAssertions> NotBeOnOrAfter(TimeOnly unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value?.CompareTo(unexpected) >= 0) {
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
    public AndConstraint<NullableTimeOnlyAssertions> NotBeOnOrBefore(TimeOnly unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value?.CompareTo(unexpected) <= 0) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE_ON_OR_BEFORE, Expression, Format(unexpected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the hour component does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The hour the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the hour component matches.</exception>
    public AndConstraint<NullableTimeOnlyAssertions> NotHaveHour(int unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Hour == unexpected) {
            Fail(Strings.Format(StringKeys.DATE_NOT_HAVE_HOUR, Expression, AssertionScope.FormatInvariant(unexpected), FormatHour()), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the minute component does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The minute the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the minute component matches.</exception>
    public AndConstraint<NullableTimeOnlyAssertions> NotHaveMinute(int unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Minute == unexpected) {
            Fail(Strings.Format(StringKeys.DATE_NOT_HAVE_MINUTE, Expression, AssertionScope.FormatInvariant(unexpected), FormatMinute()), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the second component does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The second the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the second component matches.</exception>
    public AndConstraint<NullableTimeOnlyAssertions> NotHaveSecond(int unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Second == unexpected) {
            Fail(Strings.Format(StringKeys.DATE_NOT_HAVE_SECOND, Expression, AssertionScope.FormatInvariant(unexpected), FormatSecond()), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the millisecond component does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The millisecond the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the millisecond component matches.</exception>
    public AndConstraint<NullableTimeOnlyAssertions> NotHaveMillisecond(int unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Millisecond == unexpected) {
            Fail(Strings.Format(StringKeys.DATE_NOT_HAVE_MILLISECOND, Expression, AssertionScope.FormatInvariant(unexpected), FormatMillisecond()), because, becauseArgs);
        }

        return new(this);
    }

    private string FormatHour() => _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Hour);

    private string FormatMinute() => _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Minute);

    private string FormatSecond() => _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Second);

    private string FormatMillisecond() => _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Millisecond);
}
