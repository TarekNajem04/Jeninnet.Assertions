//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying <see cref="DateTime"/> values.</summary>
/// <param name="value">The value to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public partial class DateTimeAssertions(DateTime value, string? expression = null) : AssertionsBase(expression, "date") {
    private readonly DateTime _value = value;

    private static string Format(DateTime value) => AssertionScope.FormatInvariant(value);

    /// <summary>Asserts that the value equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The values are not equal.</exception>
    public AndConstraint<DateTimeAssertions> Be(DateTime expected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<DateTimeAssertions> NotBe(DateTime unexpected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<DateTimeAssertions> BeSameDateAs(DateTime expected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<DateTimeAssertions> NotBeSameDateAs(DateTime unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value.Date == unexpected.Date) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE_SAME_DATE, Expression, Format(unexpected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the <see cref="DateTime.Kind"/> equals <paramref name="kind"/>.</summary>
    /// <param name="kind">The expected kind.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="kind"/> is not a defined <see cref="DateTimeKind"/> value.</exception>
    /// <exception cref="AssertionFailedException">The kind differs.</exception>
    public AndConstraint<DateTimeAssertions> BeIn(DateTimeKind kind, string? because = null, params object?[] becauseArgs) {
#if NETSTANDARD2_0
        if(!Enum.IsDefined(typeof(DateTimeKind), kind)) {
#else
        if(!Enum.IsDefined(kind)) {
#endif
            throw new ArgumentOutOfRangeException(nameof(kind), "Kind must be a defined DateTimeKind value.");
        }

        if(_value.Kind != kind) {
            Fail(Strings.Format(StringKeys.DATE_BE_IN_KIND, Expression, AssertionScope.FormatInvariant(kind), AssertionScope.FormatInvariant(_value.Kind)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the <see cref="DateTime.Kind"/> does not equal <paramref name="kind"/>.</summary>
    /// <param name="kind">The kind the value must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="kind"/> is not a defined <see cref="DateTimeKind"/> value.</exception>
    /// <exception cref="AssertionFailedException">The kind matches.</exception>
    public AndConstraint<DateTimeAssertions> NotBeIn(DateTimeKind kind, string? because = null, params object?[] becauseArgs) {
#if NETSTANDARD2_0
        if(!Enum.IsDefined(typeof(DateTimeKind), kind)) {
#else
        if(!Enum.IsDefined(kind)) {
#endif
            throw new ArgumentOutOfRangeException(nameof(kind), "Kind must be a defined DateTimeKind value.");
        }

        if(_value.Kind == kind) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE_IN_KIND, Expression, AssertionScope.FormatInvariant(kind), AssertionScope.FormatInvariant(_value.Kind)), because, becauseArgs);
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
    public AndConstraint<DateTimeAssertions> BeOneOf(DateTime[] expected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<DateTimeAssertions> BeOneOf(params DateTime[] expected) => BeOneOf(expected, null, []);
}
