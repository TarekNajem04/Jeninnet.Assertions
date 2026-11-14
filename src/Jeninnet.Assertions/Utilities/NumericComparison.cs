//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Utilities;

/// <summary>Type-aware numeric comparisons shared by numeric assertion classes.</summary>
/// <remarks>
/// Approximate comparisons must not round through <see cref="double"/> for decimal or
/// 64-bit/128-bit integral values. The helpers below choose an exact representation per type:
/// <see cref="decimal"/> for decimal and 64-bit integers, <see cref="BigInteger"/> for 128-bit
/// integers and <see cref="BigInteger"/> itself, and <see cref="double"/> for floating-point
/// types. Unknown numeric types fall back to <see cref="double"/> and are documented as such.
/// </remarks>
internal static class NumericComparison {
    /// <summary>Determines whether a numeric value is a floating-point NaN.</summary>
    /// <typeparam name="T">The numeric type.</typeparam>
    /// <param name="value">The value to inspect.</param>
    /// <returns><see langword="true"/> for NaN; otherwise <see langword="false"/>.</returns>
    internal static bool IsNaN<T>(T value) where T : struct, IComparable<T> =>
        value switch {
            double doubleValue => double.IsNaN(doubleValue),
            float floatValue => float.IsNaN(floatValue),
#if NET6_0_OR_GREATER
            Half halfValue => Half.IsNaN(halfValue),
#endif
            _ => false,
        };

    /// <summary>Determines whether a numeric value is a floating-point infinity.</summary>
    /// <typeparam name="T">The numeric type.</typeparam>
    /// <param name="value">The value to inspect.</param>
    /// <returns><see langword="true"/> for positive or negative infinity; otherwise <see langword="false"/>.</returns>
    internal static bool IsInfinity<T>(T value) where T : struct, IComparable<T> =>
        value switch {
            double doubleValue => double.IsInfinity(doubleValue),
            float floatValue => float.IsInfinity(floatValue),
#if NET6_0_OR_GREATER
            Half halfValue => Half.IsInfinity(halfValue),
#endif
            _ => false,
        };

    /// <summary>Determines whether two values are within an absolute precision.</summary>
    /// <typeparam name="T">The numeric type.</typeparam>
    /// <param name="actual">The actual value.</param>
    /// <param name="expected">The expected value.</param>
    /// <param name="precision">The non-negative absolute tolerance.</param>
    /// <returns>
    /// <see langword="true"/> when the absolute difference is at most <paramref name="precision"/>;
    /// <see langword="false"/> for NaN or different infinities.
    /// </returns>
    /// <remarks>Equal infinities compare as approximately equal; NaN never approximates anything.</remarks>
    internal static bool WithinPrecision<T>(T actual, T expected, T precision) where T : struct, IComparable<T> {
        if(IsNaN(actual) || IsNaN(expected)) {
            return false;
        }

        if(IsInfinity(actual) || IsInfinity(expected)) {
            return EqualityComparer<T>.Default.Equals(actual, expected);
        }

        if(typeof(T) == typeof(decimal)) {
            var actualDecimal = (decimal)(object)actual;
            var expectedDecimal = (decimal)(object)expected;
            var precisionDecimal = (decimal)(object)precision;
            try {
                return Math.Abs(actualDecimal - expectedDecimal) <= precisionDecimal;
            }
            catch(OverflowException) {
                // The difference of the extreme decimal values exceeds decimal.MaxValue,
                // so it can never fit a non-negative decimal precision.
                return false;
            }
        }

        if(typeof(T) == typeof(double)) {
            var actualDouble = (double)(object)actual;
            var expectedDouble = (double)(object)expected;
            var precisionDouble = (double)(object)precision;
            return Math.Abs(actualDouble - expectedDouble) <= precisionDouble;
        }

        if(typeof(T) == typeof(float)) {
            var actualSingle = (double)(float)(object)actual;
            var expectedSingle = (double)(float)(object)expected;
            var precisionSingle = (double)(float)(object)precision;
            return Math.Abs(actualSingle - expectedSingle) <= precisionSingle;
        }

#if NET6_0_OR_GREATER
        if(typeof(T) == typeof(Half)) {
            var actualHalf = (double)(Half)(object)actual;
            var expectedHalf = (double)(Half)(object)expected;
            var precisionHalf = (double)(Half)(object)precision;
            return Math.Abs(actualHalf - expectedHalf) <= precisionHalf;
        }
#endif

        if(typeof(T) == typeof(BigInteger)) {
            var actualInteger = (BigInteger)(object)actual;
            var expectedInteger = (BigInteger)(object)expected;
            var precisionInteger = (BigInteger)(object)precision;
            return BigInteger.Abs(actualInteger - expectedInteger) <= precisionInteger;
        }

#if NET7_0_OR_GREATER
        if(typeof(T) == typeof(Int128)) {
            var actualInteger = ToBigInteger((Int128)(object)actual);
            var expectedInteger = ToBigInteger((Int128)(object)expected);
            var precisionInteger = ToBigInteger((Int128)(object)precision);
            return BigInteger.Abs(actualInteger - expectedInteger) <= precisionInteger;
        }

        if(typeof(T) == typeof(UInt128)) {
            var actualInteger = ToBigInteger((UInt128)(object)actual);
            var expectedInteger = ToBigInteger((UInt128)(object)expected);
            var precisionInteger = ToBigInteger((UInt128)(object)precision);
            return BigInteger.Abs(actualInteger - expectedInteger) <= precisionInteger;
        }
#endif

        if(typeof(T) == typeof(IntPtr)) {
            var actualNative = ((IntPtr)(object)actual).ToInt64();
            var expectedNative = ((IntPtr)(object)expected).ToInt64();
            var precisionNative = ((IntPtr)(object)precision).ToInt64();
            return Math.Abs((decimal)actualNative - expectedNative) <= precisionNative;
        }

        if(typeof(T) == typeof(UIntPtr)) {
            var actualNative = ((UIntPtr)(object)actual).ToUInt64();
            var expectedNative = ((UIntPtr)(object)expected).ToUInt64();
            var precisionNative = ((UIntPtr)(object)precision).ToUInt64();
            return Math.Abs((decimal)actualNative - expectedNative) <= precisionNative;
        }

        if(typeof(T) == typeof(byte) || typeof(T) == typeof(sbyte)
            || typeof(T) == typeof(short) || typeof(T) == typeof(ushort)
            || typeof(T) == typeof(int) || typeof(T) == typeof(uint)
            || typeof(T) == typeof(long) || typeof(T) == typeof(ulong)) {
            var actualIntegral = Convert.ToDecimal(actual, CultureInfo.InvariantCulture);
            var expectedIntegral = Convert.ToDecimal(expected, CultureInfo.InvariantCulture);
            var precisionIntegral = Convert.ToDecimal(precision, CultureInfo.InvariantCulture);
            return Math.Abs(actualIntegral - expectedIntegral) <= precisionIntegral;
        }

        var actualFallback = Convert.ToDouble(actual, CultureInfo.InvariantCulture);
        var expectedFallback = Convert.ToDouble(expected, CultureInfo.InvariantCulture);
        var precisionFallback = Convert.ToDouble(precision, CultureInfo.InvariantCulture);
        return Math.Abs(actualFallback - expectedFallback) <= precisionFallback;
    }

#if NET7_0_OR_GREATER
    /// <summary>Converts a signed 128-bit integer to <see cref="BigInteger"/> without losing precision.</summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The exact <see cref="BigInteger"/> representation.</returns>
    private static BigInteger ToBigInteger(Int128 value) {
        var low = (ulong)value;
        var high = (long)(value >> 64);
        return new BigInteger(low) + (new BigInteger(high) << 64);
    }

    /// <summary>Converts an unsigned 128-bit integer to <see cref="BigInteger"/> without losing precision.</summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The exact <see cref="BigInteger"/> representation.</returns>
    private static BigInteger ToBigInteger(UInt128 value) {
        var low = (ulong)value;
        var high = (ulong)(value >> 64);
        return new BigInteger(low) + (new BigInteger(high) << 64);
    }
#endif
}
