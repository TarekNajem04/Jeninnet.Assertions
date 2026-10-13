//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying <see cref="DateTimeOffset"/> values.</summary>
/// <param name="value">The value to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public partial class DateTimeOffsetAssertions(DateTimeOffset value, string? expression = null) : AssertionsBase(expression, "date") {
    private readonly DateTimeOffset _value = value;

    private static string Format(DateTimeOffset value) => AssertionScope.FormatInvariant(value);

    /// <summary>Asserts that the value equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The values are not equal.</exception>
    public AndConstraint<DateTimeOffsetAssertions> Be(DateTimeOffset expected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<DateTimeOffsetAssertions> NotBe(DateTimeOffset unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value == unexpected) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE, Expression, Format(unexpected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value falls on the same calendar date as <paramref name="expected"/> (time parts are ignored).</summary>
    /// <param name="expected">The value whose date must match.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The calendar dates differ.</exception>
    public AndConstraint<DateTimeOffsetAssertions> BeSameDateAs(DateTimeOffset expected, string? because = null, params object?[] becauseArgs) {
        if(_value.Date != expected.Date) {
            Fail(Strings.Format(StringKeys.DATE_BE_SAME_DATE, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value does not fall on the same calendar date as <paramref name="unexpected"/> (time parts are ignored).</summary>
    /// <param name="unexpected">The value whose date must differ.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The calendar dates are equal.</exception>
    public AndConstraint<DateTimeOffsetAssertions> NotBeSameDateAs(DateTimeOffset unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value.Date == unexpected.Date) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE_SAME_DATE, Expression, Format(unexpected), Format(_value)), because, becauseArgs);
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
    public AndConstraint<DateTimeOffsetAssertions> BeOneOf(DateTimeOffset[] expected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<DateTimeOffsetAssertions> BeOneOf(params DateTimeOffset[] expected) => BeOneOf(expected, null, []);
}
