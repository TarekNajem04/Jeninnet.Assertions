//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying nullable enum values of type <typeparamref name="TEnum"/>.</summary>
/// <remarks>
/// Positive assertions fail on a <see langword="null"/> subject, while <c>NotBe</c> and
/// <c>NotHaveFlag</c> pass on <see langword="null"/> — except <c>HaveValue</c>, which fails.
/// Null renders as <c>&lt;null&gt;</c>.
/// </remarks>
/// <typeparam name="TEnum">The enum type under assertion.</typeparam>
/// <param name="value">The nullable value to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public class NullableEnumAssertions<TEnum>(TEnum? value, string? expression = null) : AssertionsBase(expression, "value") where TEnum : struct, Enum {
    private readonly TEnum? _value = value;

    private static string Format(TEnum? value) => value is null ? "<null>" : AssertionScope.FormatInvariant(value);

    /// <summary>Asserts that the value equals <paramref name="expected"/> (with <see langword="null"/> equal to <see langword="null"/>).</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The values are not equal.</exception>
    public AndConstraint<NullableEnumAssertions<TEnum>> Be(TEnum? expected, string? because = null, params object?[] becauseArgs) {
        if(!EqualityComparer<TEnum?>.Default.Equals(_value, expected)) {
            Fail(Strings.Format(StringKeys.ENUM_BE, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The value the subject must not equal.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The values are equal.</exception>
    public AndConstraint<NullableEnumAssertions<TEnum>> NotBe(TEnum unexpected, string? because = null, params object?[] becauseArgs) {
        if(EqualityComparer<TEnum?>.Default.Equals(_value, unexpected)) {
            Fail(Strings.Format(StringKeys.ENUM_NOT_BE, Expression, Format(unexpected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value has the flag <paramref name="flag"/> set.</summary>
    /// <param name="flag">The flag that must be set.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the flag is not set.</exception>
    public AndConstraint<NullableEnumAssertions<TEnum>> HaveFlag(TEnum flag, string? because = null, params object?[] becauseArgs) {
        if(_value?.HasFlag(flag) != true) {
            Fail(Strings.Format(StringKeys.ENUM_HAVE_FLAG, Expression, Format(flag), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value does not have the flag <paramref name="flag"/> set. A <see langword="null"/> subject passes.</summary>
    /// <param name="flag">The flag that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The flag is set.</exception>
    public AndConstraint<NullableEnumAssertions<TEnum>> NotHaveFlag(TEnum flag, string? because = null, params object?[] becauseArgs) {
        if(_value?.HasFlag(flag) == true) {
            Fail(Strings.Format(StringKeys.ENUM_NOT_HAVE_FLAG, Expression, Format(flag), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is a defined member of <typeparamref name="TEnum"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or not defined in <typeparamref name="TEnum"/>.</exception>
    public AndConstraint<NullableEnumAssertions<TEnum>> BeDefined(string? because = null, params object?[] becauseArgs) {
        // The generic Enum.IsDefined overload is unavailable on .NET Standard 2.0.
#pragma warning disable CA2263 // Build-verified: removing fails net6.0/net8.0/net10.0.
        if(_value is null || !Enum.IsDefined(typeof(TEnum), _value.Value)) {
            Fail(Strings.Format(StringKeys.ENUM_BE_DEFINED, Expression, typeof(TEnum).Name, Format(_value)), because, becauseArgs);
        }
#pragma warning restore CA2263

        return new(this);
    }

    /// <summary>Asserts that the value is not a defined member of <typeparamref name="TEnum"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is defined in <typeparamref name="TEnum"/>.</exception>
    public AndConstraint<NullableEnumAssertions<TEnum>> NotBeDefined(string? because = null, params object?[] becauseArgs) {
        // The generic Enum.IsDefined overload is unavailable on .NET Standard 2.0.
#pragma warning disable CA2263 // Build-verified: removing fails net6.0/net8.0/net10.0.
        if(_value is not null && Enum.IsDefined(typeof(TEnum), _value.Value)) {
            Fail(Strings.Format(StringKeys.ENUM_NOT_BE_DEFINED, Expression, typeof(TEnum).Name, Format(_value)), because, becauseArgs);
        }
#pragma warning restore CA2263

        return new(this);
    }

    /// <summary>Asserts that the value equals one of <paramref name="expected"/>.</summary>
    /// <param name="expected">The candidate values.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or equals none of the candidates.</exception>
    public AndConstraint<NullableEnumAssertions<TEnum>> BeOneOf(params TEnum[] expected) =>
        BeOneOf(expected, null);

    /// <summary>Asserts that the value equals one of <paramref name="expected"/>.</summary>
    /// <param name="expected">The candidate values.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or equals none of the candidates.</exception>
    public AndConstraint<NullableEnumAssertions<TEnum>> BeOneOf(IEnumerable<TEnum> expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(_value is null || !expected.Contains(_value.Value)) {
            Fail(Strings.Format(StringKeys.ENUM_BE_ONE_OF, Expression, string.Join(", ", expected.Select(FormatValue)), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value satisfies <paramref name="predicate"/>.</summary>
    /// <param name="predicate">The condition the value must satisfy.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the predicate returns <see langword="false"/>.</exception>
    public AndConstraint<NullableEnumAssertions<TEnum>> Match(Expression<Func<TEnum, bool>> predicate, string? because = null, params object?[] becauseArgs) {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        if(_value is null || !predicate.Compile()(_value.Value)) {
            Fail(Strings.Format(StringKeys.ENUM_MATCH, Expression, predicate.Body), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value has the same identifier name as <paramref name="other"/> (possibly of another enum type).</summary>
    /// <typeparam name="TOther">The other enum type.</typeparam>
    /// <param name="other">The value to compare names with.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the identifier names differ.</exception>
    public AndConstraint<NullableEnumAssertions<TEnum>> HaveSameNameAs<TOther>(TOther other, string? because = null, params object?[] becauseArgs) where TOther : struct, Enum {
        var subjectName = _value is null ? null : EnumName(typeof(TEnum), _value.Value);
        if(!string.Equals(subjectName, EnumName(typeof(TOther), other), StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.ENUM_HAVE_SAME_NAME, Expression, FormatOther(other), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value does not have the same identifier name as <paramref name="other"/>.</summary>
    /// <typeparam name="TOther">The other enum type.</typeparam>
    /// <param name="other">The value to compare names with.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The identifier names are equal.</exception>
    public AndConstraint<NullableEnumAssertions<TEnum>> NotHaveSameNameAs<TOther>(TOther other, string? because = null, params object?[] becauseArgs) where TOther : struct, Enum {
        var subjectName = _value is null ? null : EnumName(typeof(TEnum), _value.Value);
        if(string.Equals(subjectName, EnumName(typeof(TOther), other), StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.ENUM_NOT_HAVE_SAME_NAME, Expression, FormatOther(other), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value has the same underlying value as <paramref name="other"/> (possibly of another enum type).</summary>
    /// <typeparam name="TOther">The other enum type.</typeparam>
    /// <param name="other">The value to compare with.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or the underlying values differ.</exception>
    public AndConstraint<NullableEnumAssertions<TEnum>> HaveSameValueAs<TOther>(TOther other, string? because = null, params object?[] becauseArgs) where TOther : struct, Enum {
        if(_value is null || Convert.ToDecimal(_value.Value, CultureInfo.InvariantCulture) != Convert.ToDecimal(other, CultureInfo.InvariantCulture)) {
            Fail(Strings.Format(StringKeys.ENUM_HAVE_SAME_VALUE, Expression, FormatOther(other), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value does not have the same underlying value as <paramref name="other"/>.</summary>
    /// <typeparam name="TOther">The other enum type.</typeparam>
    /// <param name="other">The value to compare with.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The underlying values are equal.</exception>
    public AndConstraint<NullableEnumAssertions<TEnum>> NotHaveSameValueAs<TOther>(TOther other, string? because = null, params object?[] becauseArgs) where TOther : struct, Enum {
        if(_value is not null && Convert.ToDecimal(_value.Value, CultureInfo.InvariantCulture) == Convert.ToDecimal(other, CultureInfo.InvariantCulture)) {
            Fail(Strings.Format(StringKeys.ENUM_NOT_HAVE_SAME_VALUE, Expression, FormatOther(other), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    private static string FormatValue(TEnum value) => AssertionScope.FormatInvariant(value);

    private static string FormatOther<TOther>(TOther value) where TOther : struct, Enum => AssertionScope.FormatInvariant(value);

    /// <summary>Reads an enum identifier name without the generic overload (unavailable on .NET Standard 2.0).</summary>
    /// <param name="type">The enum type.</param>
    /// <param name="value">The value to name.</param>
    /// <returns>The identifier name, or <see langword="null"/> for undefined values.</returns>
    private static string? EnumName(Type type, object value) {
#pragma warning disable CA2263, IDE0022 // Build-verified: removing CA2263 fails net6.0/net8.0/net10.0; the block body must stay to host it, so IDE0022's expression body is illegal here.
        return Enum.GetName(type, value);
#pragma warning restore CA2263, IDE0022
    }

    /// <summary>Asserts that the value is <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is not <see langword="null"/>.</exception>
    public AndConstraint<NullableEnumAssertions<TEnum>> BeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is not null) {
            Fail(Strings.Format(StringKeys.ENUM_BE_NULL, Expression, Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value has a value (is not <see langword="null"/>).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/>.</exception>
    public AndConstraint<NullableEnumAssertions<TEnum>> HaveValue(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.ENUM_NOT_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value has no value (is <see langword="null"/>).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is not <see langword="null"/>.</exception>
    public AndConstraint<NullableEnumAssertions<TEnum>> NotHaveValue(string? because = null, params object?[] becauseArgs) =>
        BeNull(because, becauseArgs);
}
