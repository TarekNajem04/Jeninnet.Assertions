//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying nullable <see cref="DateTime"/> values.</summary>
/// <remarks>
/// Positive assertions fail on a <see langword="null"/> subject, while negative assertions
/// (<c>NotBe</c>, <c>NotBeBefore</c>, <c>NotBeAfter</c>, <c>NotBeCloseTo</c>) pass on
/// <see langword="null"/> — except <c>HaveValue</c>, which fails. Null renders as <c>&lt;null&gt;</c>.
/// </remarks>
/// <param name="value">The nullable value to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public partial class NullableDateTimeAssertions(DateTime? value, string? expression = null) : AssertionsBase(expression, "date") {
    private const string NULL_STRING = "<null>";

    private readonly DateTime? _value = value;

    private static string Format(DateTime? value) => value is null ? NULL_STRING : AssertionScope.FormatInvariant(value);

    /// <summary>Asserts that the value equals <paramref name="expected"/> (with <see langword="null"/> equal to <see langword="null"/>).</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The values are not equal.</exception>
    public AndConstraint<NullableDateTimeAssertions> Be(DateTime? expected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<NullableDateTimeAssertions> NotBe(DateTime? unexpected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<NullableDateTimeAssertions> BeSameDateAs(DateTime? expected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<NullableDateTimeAssertions> NotBeSameDateAs(DateTime? unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value?.Date == unexpected?.Date) {
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
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the kind differs.</exception>
    public AndConstraint<NullableDateTimeAssertions> BeIn(DateTimeKind kind, string? because = null, params object?[] becauseArgs) {
#if NETSTANDARD2_0
        if(!Enum.IsDefined(typeof(DateTimeKind), kind)) {
#else
        if(!Enum.IsDefined(kind)) {
#endif
            throw new ArgumentOutOfRangeException(nameof(kind), "Kind must be a defined DateTimeKind value.");
        }

        if(_value is null || _value.Value.Kind != kind) {
            Fail(Strings.Format(StringKeys.DATE_BE_IN_KIND, Expression, AssertionScope.FormatInvariant(kind), _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Kind)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the <see cref="DateTime.Kind"/> does not equal <paramref name="kind"/>.</summary>
    /// <param name="kind">The kind the value must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="kind"/> is not a defined <see cref="DateTimeKind"/> value.</exception>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the kind matches.</exception>
    public AndConstraint<NullableDateTimeAssertions> NotBeIn(DateTimeKind kind, string? because = null, params object?[] becauseArgs) {
#if NETSTANDARD2_0
        if(!Enum.IsDefined(typeof(DateTimeKind), kind)) {
#else
        if(!Enum.IsDefined(kind)) {
#endif
            throw new ArgumentOutOfRangeException(nameof(kind), "Kind must be a defined DateTimeKind value.");
        }

        if(_value is null || _value.Value.Kind == kind) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE_IN_KIND, Expression, AssertionScope.FormatInvariant(kind), _value is null ? NULL_STRING : AssertionScope.FormatInvariant(_value.Value.Kind)), because, becauseArgs);
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
    public AndConstraint<NullableDateTimeAssertions> BeOneOf(DateTime?[] expected, string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<NullableDateTimeAssertions> BeOneOf(params DateTime?[] expected) => BeOneOf(expected, null, []);

    /// <summary>Asserts that the value is <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is not <see langword="null"/>.</exception>
    public AndConstraint<NullableDateTimeAssertions> BeNull(string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<NullableDateTimeAssertions> HaveValue(string? because = null, params object?[] becauseArgs) {
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
    public AndConstraint<NullableDateTimeAssertions> NotHaveValue(string? because = null, params object?[] becauseArgs) =>
        BeNull(because, becauseArgs);
}
