//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>
/// Provides <c>Should()</c> extension methods that enable the fluent assertion syntax
/// (e.g. <c>value.Should().BeTrue()</c>).
/// </summary>
/// <remarks>
/// Every overload captures the caller expression (via <c>CallerArgumentExpressionAttribute</c>)
/// so failure messages can name the subject under test without stack-trace inspection.
/// </remarks>
public static partial class AssertionExtensions {
    /// <summary>Creates a <see cref="BoolAssertions"/> for the given <see cref="bool"/> value.</summary>
    /// <param name="value">The boolean value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static BoolAssertions Should(this bool value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NullableBooleanAssertions"/> for the given nullable boolean value.</summary>
    /// <param name="value">The nullable boolean value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NullableBooleanAssertions Should(this bool? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="CharAssertions"/> for the given character.</summary>
    /// <param name="value">The character to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static CharAssertions Should(this char value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NumericAssertions{T}"/> for the given value. (Concrete numeric overloads keep overload resolution unambiguous with enums.)</summary>
    /// <param name="value">The value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NumericAssertions<byte> Should(this byte value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NullableNumericAssertions{T}"/> for the given nullable value.</summary>
    /// <param name="value">The nullable value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NullableNumericAssertions<byte> Should(this byte? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NumericAssertions{T}"/> for the given value.</summary>
    /// <param name="value">The value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NumericAssertions<sbyte> Should(this sbyte value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NullableNumericAssertions{T}"/> for the given nullable value.</summary>
    /// <param name="value">The nullable value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NullableNumericAssertions<sbyte> Should(this sbyte? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NumericAssertions{T}"/> for the given value.</summary>
    /// <param name="value">The value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NumericAssertions<short> Should(this short value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NullableNumericAssertions{T}"/> for the given nullable value.</summary>
    /// <param name="value">The nullable value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NullableNumericAssertions<short> Should(this short? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NumericAssertions{T}"/> for the given value.</summary>
    /// <param name="value">The value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NumericAssertions<ushort> Should(this ushort value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NullableNumericAssertions{T}"/> for the given nullable value.</summary>
    /// <param name="value">The nullable value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NullableNumericAssertions<ushort> Should(this ushort? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NumericAssertions{T}"/> for the given value.</summary>
    /// <param name="value">The value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NumericAssertions<int> Should(this int value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NullableNumericAssertions{T}"/> for the given nullable value.</summary>
    /// <param name="value">The nullable value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NullableNumericAssertions<int> Should(this int? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NumericAssertions{T}"/> for the given value.</summary>
    /// <param name="value">The value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NumericAssertions<uint> Should(this uint value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NullableNumericAssertions{T}"/> for the given nullable value.</summary>
    /// <param name="value">The nullable value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NullableNumericAssertions<uint> Should(this uint? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NumericAssertions{T}"/> for the given value.</summary>
    /// <param name="value">The value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NumericAssertions<long> Should(this long value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NullableNumericAssertions{T}"/> for the given nullable value.</summary>
    /// <param name="value">The nullable value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NullableNumericAssertions<long> Should(this long? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NumericAssertions{T}"/> for the given value.</summary>
    /// <param name="value">The value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NumericAssertions<ulong> Should(this ulong value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NullableNumericAssertions{T}"/> for the given nullable value.</summary>
    /// <param name="value">The nullable value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NullableNumericAssertions<ulong> Should(this ulong? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NumericAssertions{T}"/> for the given value.</summary>
    /// <param name="value">The value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NumericAssertions<float> Should(this float value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NullableNumericAssertions{T}"/> for the given nullable value.</summary>
    /// <param name="value">The nullable value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NullableNumericAssertions<float> Should(this float? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NumericAssertions{T}"/> for the given value.</summary>
    /// <param name="value">The value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NumericAssertions<double> Should(this double value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NullableNumericAssertions{T}"/> for the given nullable value.</summary>
    /// <param name="value">The nullable value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NullableNumericAssertions<double> Should(this double? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NumericAssertions{T}"/> for the given value.</summary>
    /// <param name="value">The value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NumericAssertions<decimal> Should(this decimal value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NullableNumericAssertions{T}"/> for the given nullable value.</summary>
    /// <param name="value">The nullable value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NullableNumericAssertions<decimal> Should(this decimal? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NumericAssertions{T}"/> for the given arbitrary-precision integer.</summary>
    /// <param name="value">The value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NumericAssertions<BigInteger> Should(this BigInteger value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NullableNumericAssertions{T}"/> for the given nullable arbitrary-precision integer.</summary>
    /// <param name="value">The nullable value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NullableNumericAssertions<BigInteger> Should(this BigInteger? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NumericAssertions{T}"/> for the given native-sized signed integer.</summary>
    /// <param name="value">The value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NumericAssertions<long> Should(this IntPtr value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value.ToInt64(), expression);

    /// <summary>Creates a <see cref="NullableNumericAssertions{T}"/> for the given nullable native-sized signed integer.</summary>
    /// <param name="value">The nullable value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NullableNumericAssertions<long> Should(this IntPtr? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value?.ToInt64(), expression);

    /// <summary>Creates a <see cref="NumericAssertions{T}"/> for the given native-sized unsigned integer.</summary>
    /// <param name="value">The value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NumericAssertions<ulong> Should(this UIntPtr value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value.ToUInt64(), expression);

    /// <summary>Creates a <see cref="NullableNumericAssertions{T}"/> for the given nullable native-sized unsigned integer.</summary>
    /// <param name="value">The nullable value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NullableNumericAssertions<ulong> Should(this UIntPtr? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value?.ToUInt64(), expression);

#if NET6_0_OR_GREATER
    /// <summary>Creates a <see cref="NumericAssertions{T}"/> for the given half-precision floating-point value.</summary>
    /// <param name="value">The value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NumericAssertions<Half> Should(this Half value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NullableNumericAssertions{T}"/> for the given nullable half-precision floating-point value.</summary>
    /// <param name="value">The nullable value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NullableNumericAssertions<Half> Should(this Half? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);
#endif

#if NET7_0_OR_GREATER
    /// <summary>Creates a <see cref="NumericAssertions{T}"/> for the given signed 128-bit integer.</summary>
    /// <param name="value">The value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NumericAssertions<Int128> Should(this Int128 value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NullableNumericAssertions{T}"/> for the given nullable signed 128-bit integer.</summary>
    /// <param name="value">The nullable value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NullableNumericAssertions<Int128> Should(this Int128? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NumericAssertions{T}"/> for the given unsigned 128-bit integer.</summary>
    /// <param name="value">The value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NumericAssertions<UInt128> Should(this UInt128 value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NullableNumericAssertions{T}"/> for the given nullable unsigned 128-bit integer.</summary>
    /// <param name="value">The nullable value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NullableNumericAssertions<UInt128> Should(this UInt128? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);
#endif

    /// <summary>Creates a <see cref="StringAssertions"/> for the given string value.</summary>
    /// <param name="value">The string value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static StringAssertions Should(this string? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);
}
