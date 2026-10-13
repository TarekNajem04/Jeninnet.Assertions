//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Platform/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying nullable <see cref="DateOnly"/> values.</summary>
/// <remarks>
/// Positive assertions fail on a <see langword="null"/> subject, while negative assertions
/// (<c>NotBe</c>, <c>NotBeBefore</c>, <c>NotBeAfter</c>) pass on <see langword="null"/> —
/// except <c>HaveValue</c>, which fails. Null renders as <c>&lt;null&gt;</c>.
/// </remarks>
/// <param name="value">The nullable value to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public class NullableDateOnlyAssertions(DateOnly? value, string? expression = null) : AssertionsBase(expression, "date") {
    private const string NULL_STRING = "<null>";
    private readonly DateOnly? _value = value;

    private static string Format(DateOnly? value) => value is null ? NULL_STRING : AssertionScope.FormatInvariant(value);

    /// <summary>Asserts that the value equals <paramref name="expected"/> (with <see langword="null"/> equal to <see langword="null"/>).</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The values are not equal.</exception>
    public AndConstraint<NullableDateOnlyAssertions> Be(DateOnly? expected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<NullableDateOnlyAssertions> NotBe(DateOnly? unexpected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<NullableDateOnlyAssertions> BeBefore(DateOnly expected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<NullableDateOnlyAssertions> NotBeBefore(DateOnly expected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<NullableDateOnlyAssertions> BeOnOrBefore(DateOnly expected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<NullableDateOnlyAssertions> BeAfter(DateOnly expected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<NullableDateOnlyAssertions> NotBeAfter(DateOnly expected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<NullableDateOnlyAssertions> BeOnOrAfter(DateOnly expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.CompareTo(expected) < 0) {
            Fail(Strings.Format(StringKeys.DATE_BE_ON_OR_AFTER, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the year component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected year.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the year component differs.</exception>
    public AndConstraint<NullableDateOnlyAssertions> HaveYear(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Year != expected) {
            Fail(Strings.Format(StringKeys.DATE_HAVE_YEAR, Expression, AssertionScope.FormatInvariant(expected), FormatYear()), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the month component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected month (1-12).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the month component differs.</exception>
    public AndConstraint<NullableDateOnlyAssertions> HaveMonth(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Month != expected) {
            Fail(Strings.Format(StringKeys.DATE_HAVE_MONTH, Expression, AssertionScope.FormatInvariant(expected), FormatMonth()), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the day component equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected day of month.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the day component differs.</exception>
    public AndConstraint<NullableDateOnlyAssertions> HaveDay(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Day != expected) {
            Fail(Strings.Format(StringKeys.DATE_HAVE_DAY, Expression, AssertionScope.FormatInvariant(expected), FormatDay()), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is not <see langword="null"/>.</exception>
    public AndConstraint<NullableDateOnlyAssertions> BeNull(string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<NullableDateOnlyAssertions> HaveValue(string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<NullableDateOnlyAssertions> NotHaveValue(string? because = null, params object?[] becauseArgs) =>
        BeNull(because, becauseArgs);

    /// <summary>Asserts that the value equals one of the <paramref name="expected"/> values.</summary>
    /// <param name="expected">The candidate values, one of which must equal the subject.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The value matches none of the candidates.</exception>
    public AndConstraint<NullableDateOnlyAssertions> BeOneOf(DateOnly?[] expected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<NullableDateOnlyAssertions> BeOneOf(params DateOnly?[] expected) => BeOneOf(expected, null, []);

    /// <summary>Asserts that the value is not on or after <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The value the subject must strictly precede.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is on or after <paramref name="unexpected"/>.</exception>
    public AndConstraint<NullableDateOnlyAssertions> NotBeOnOrAfter(DateOnly unexpected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<NullableDateOnlyAssertions> NotBeOnOrBefore(DateOnly unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value?.CompareTo(unexpected) <= 0) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE_ON_OR_BEFORE, Expression, Format(unexpected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the year component does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The year the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the year component matches.</exception>
    public AndConstraint<NullableDateOnlyAssertions> NotHaveYear(int unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Year == unexpected) {
            Fail(Strings.Format(StringKeys.DATE_NOT_HAVE_YEAR, Expression, AssertionScope.FormatInvariant(unexpected), FormatYear()), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the month component does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The month the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the month component matches.</exception>
    public AndConstraint<NullableDateOnlyAssertions> NotHaveMonth(int unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Month == unexpected) {
            Fail(Strings.Format(StringKeys.DATE_NOT_HAVE_MONTH, Expression, AssertionScope.FormatInvariant(unexpected), FormatMonth()), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the day component does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The day of month the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the day component matches.</exception>
    public AndConstraint<NullableDateOnlyAssertions> NotHaveDay(int unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Day == unexpected) {
            Fail(Strings.Format(StringKeys.DATE_NOT_HAVE_DAY, Expression, AssertionScope.FormatInvariant(unexpected), FormatDay()), because, becauseArgs);
        }

        return new(this);
    }

    private string FormatYear() => _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Year);

    private string FormatMonth() => _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Month);

    private string FormatDay() => _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Day);
}
