//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying nullable <see cref="DateTimeOffset"/> values.</summary>
/// <remarks>
/// Positive assertions fail on a <see langword="null"/> subject, while negative assertions
/// (<c>NotBe</c>, <c>NotBeBefore</c>, <c>NotBeAfter</c>, <c>NotBeCloseTo</c>) pass on
/// <see langword="null"/> — except <c>HaveValue</c>, which fails. Null renders as <c>&lt;null&gt;</c>.
/// </remarks>
/// <param name="value">The nullable value to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public partial class NullableDateTimeOffsetAssertions(DateTimeOffset? value, string? expression = null) : AssertionsBase(expression, "date") {
    private const string NULL_STRING = "<null>";

    private readonly DateTimeOffset? _value = value;

    private static string Format(DateTimeOffset? value) => value is null ? NULL_STRING : AssertionScope.FormatInvariant(value);

    /// <summary>Asserts that the value equals <paramref name="expected"/> (with <see langword="null"/> equal to <see langword="null"/>).</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The values are not equal.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> Be(DateTimeOffset? expected, string? because = null, params object?[] becauseArgs) {
        if(_value != expected) {
            Fail(Strings.Format(StringKeys.DATE_BE, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value does not equal <paramref name="unexpected"/>. A <see langword="null"/> subject passes unless <paramref name="unexpected"/> is also <see langword="null"/>.</summary>
    /// <param name="unexpected">The value the subject must not equal.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The values are equal.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> NotBe(DateTimeOffset? unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value == unexpected) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE, Expression, Format(unexpected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value falls on the same calendar date as <paramref name="expected"/> (time parts are ignored, with <see langword="null"/> equal to <see langword="null"/>).</summary>
    /// <param name="expected">The value whose date must match.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The calendar dates differ.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> BeSameDateAs(DateTimeOffset? expected, string? because = null, params object?[] becauseArgs) {
        if(_value?.Date != expected?.Date) {
            Fail(Strings.Format(StringKeys.DATE_BE_SAME_DATE, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value does not fall on the same calendar date as <paramref name="unexpected"/> (time parts are ignored). A <see langword="null"/> subject passes unless <paramref name="unexpected"/> is also <see langword="null"/>.</summary>
    /// <param name="unexpected">The value whose date must differ.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The calendar dates are equal.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> NotBeSameDateAs(DateTimeOffset? unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value?.Date == unexpected?.Date) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE_SAME_DATE, Expression, Format(unexpected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is not <see langword="null"/>.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> BeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is not null) {
            Fail(Strings.Format(StringKeys.DATE_BE_NULL, Expression, Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the UTC offset equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected offset from UTC.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the offset differs.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> HaveOffset(TimeSpan expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Offset != expected) {
            Fail(Strings.Format(StringKeys.DATE_HAVE_OFFSET, Expression, AssertionScope.FormatInvariant(expected), _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Offset)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the UTC offset does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The offset the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the offset matches.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> NotHaveOffset(TimeSpan unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.Offset == unexpected) {
            Fail(Strings.Format(StringKeys.DATE_NOT_HAVE_OFFSET, Expression, AssertionScope.FormatInvariant(unexpected), _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Offset)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is exactly <paramref name="expected"/>: the same instant and the same offset.</summary>
    /// <remarks>Unlike <see cref="Be"/>, which compares instants only, this also requires the offsets to match.</remarks>
    /// <param name="expected">The expected instant and offset.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the instant or offset differs.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> BeExactly(DateTimeOffset? expected, string? because = null, params object?[] becauseArgs) {
        if(!IsExactly(_value, expected)) {
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
    public AndConstraint<NullableDateTimeOffsetAssertions> NotBeExactly(DateTimeOffset? unexpected, string? because = null, params object?[] becauseArgs) {
        if(IsExactly(_value, unexpected)) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE_EXACTLY, Expression, Format(unexpected), Format(_value)), because, becauseArgs);
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
    public AndConstraint<NullableDateTimeOffsetAssertions> BeOneOf(DateTimeOffset?[] expected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<NullableDateTimeOffsetAssertions> BeOneOf(params DateTimeOffset?[] expected) => BeOneOf(expected, null, []);

    /// <summary>Determines whether two nullable offsets represent the same instant with the same offset.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when both are null, or both match on UTC ticks and offset.</returns>
    private static bool IsExactly(DateTimeOffset? left, DateTimeOffset? right) =>
        left.HasValue == right.HasValue && (!left.HasValue || (left.Value.UtcTicks == right!.Value.UtcTicks && left.Value.Offset == right.Value.Offset));

    /// <summary>Asserts that the value has a value (is not <see langword="null"/>).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/>.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> HaveValue(string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<NullableDateTimeOffsetAssertions> NotHaveValue(string? because = null, params object?[] becauseArgs) =>
        BeNull(because, becauseArgs);
}
