//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Fluent duration sugar for building <see cref="TimeSpan"/> values in tests (e.g. <c>3.Seconds()</c>).</summary>
public static class FluentTimeSpanExtensions {
    /// <summary>Creates a span of <paramref name="value"/> nanoseconds.</summary>
    /// <remarks>A tick is 100 nanoseconds, so sub-tick remainders are truncated.</remarks>
    /// <param name="value">The nanoseconds.</param>
    public static TimeSpan Nanoseconds(this int value) => TimeSpan.FromTicks(value / 100L);

    /// <summary>Creates a span of <paramref name="value"/> nanoseconds.</summary>
    /// <remarks>A tick is 100 nanoseconds, so sub-tick remainders are truncated.</remarks>
    /// <param name="value">The nanoseconds.</param>
    public static TimeSpan Nanoseconds(this long value) => TimeSpan.FromTicks(value / 100L);

    /// <summary>Gets the sub-microsecond nanoseconds component (0-900) of <paramref name="value"/>.</summary>
    /// <param name="value">The span to measure.</param>
    public static int Nanoseconds(this TimeSpan value) => (int)((value.Ticks % 10L) * 100L);

    /// <summary>Creates a span of <paramref name="value"/> microseconds.</summary>
    /// <param name="value">The microseconds.</param>
    public static TimeSpan Microseconds(this int value) => TimeSpan.FromTicks((long)value * 10);

    /// <summary>Creates a span of <paramref name="value"/> microseconds.</summary>
    /// <param name="value">The microseconds.</param>
    public static TimeSpan Microseconds(this long value) => TimeSpan.FromTicks(value * 10L);

    /// <summary>Gets the sub-millisecond microseconds component (0-999) of <paramref name="value"/>.</summary>
    /// <param name="value">The span to measure.</param>
    public static int Microseconds(this TimeSpan value) => (int)((value.Ticks % 10000L) / 10L);

    /// <summary>Creates a span of <paramref name="value"/> milliseconds.</summary>
    /// <param name="value">The milliseconds.</param>
    public static TimeSpan Milliseconds(this int value) => TimeSpan.FromMilliseconds(value);

    /// <summary>Creates a span of <paramref name="value"/> milliseconds.</summary>
    /// <param name="value">The milliseconds.</param>
    public static TimeSpan Milliseconds(this double value) => TimeSpan.FromMilliseconds(value);

    /// <summary>Creates a span of <paramref name="value"/> seconds.</summary>
    /// <param name="value">The seconds.</param>
    public static TimeSpan Seconds(this int value) => TimeSpan.FromSeconds(value);

    /// <summary>Creates a span of <paramref name="value"/> seconds.</summary>
    /// <param name="value">The seconds.</param>
    public static TimeSpan Seconds(this double value) => TimeSpan.FromSeconds(value);

    /// <summary>Creates a span of <paramref name="value"/> seconds plus <paramref name="offset"/>.</summary>
    /// <param name="value">The seconds.</param>
    /// <param name="offset">The extra span to add.</param>
    public static TimeSpan Seconds(this int value, TimeSpan offset) => TimeSpan.FromSeconds(value).Add(offset);

    /// <summary>Creates a span of <paramref name="value"/> minutes.</summary>
    /// <param name="value">The minutes.</param>
    public static TimeSpan Minutes(this int value) => TimeSpan.FromMinutes(value);

    /// <summary>Creates a span of <paramref name="value"/> minutes.</summary>
    /// <param name="value">The minutes.</param>
    public static TimeSpan Minutes(this double value) => TimeSpan.FromMinutes(value);

    /// <summary>Creates a span of <paramref name="value"/> minutes plus <paramref name="offset"/>.</summary>
    /// <param name="value">The minutes.</param>
    /// <param name="offset">The extra span to add.</param>
    public static TimeSpan Minutes(this int value, TimeSpan offset) => TimeSpan.FromMinutes(value).Add(offset);

    /// <summary>Creates a span of <paramref name="value"/> hours.</summary>
    /// <param name="value">The hours.</param>
    public static TimeSpan Hours(this int value) => TimeSpan.FromHours(value);

    /// <summary>Creates a span of <paramref name="value"/> hours.</summary>
    /// <param name="value">The hours.</param>
    public static TimeSpan Hours(this double value) => TimeSpan.FromHours(value);

    /// <summary>Creates a span of <paramref name="value"/> hours plus <paramref name="offset"/>.</summary>
    /// <param name="value">The hours.</param>
    /// <param name="offset">The extra span to add.</param>
    public static TimeSpan Hours(this int value, TimeSpan offset) => TimeSpan.FromHours(value).Add(offset);

    /// <summary>Creates a span of <paramref name="value"/> days.</summary>
    /// <param name="value">The days.</param>
    public static TimeSpan Days(this int value) => TimeSpan.FromDays(value);

    /// <summary>Creates a span of <paramref name="value"/> days.</summary>
    /// <param name="value">The days.</param>
    public static TimeSpan Days(this double value) => TimeSpan.FromDays(value);

    /// <summary>Creates a span of <paramref name="value"/> days plus <paramref name="offset"/>.</summary>
    /// <param name="value">The days.</param>
    /// <param name="offset">The extra span to add.</param>
    public static TimeSpan Days(this int value, TimeSpan offset) => TimeSpan.FromDays(value).Add(offset);

    /// <summary>Adds <paramref name="offset"/> to <paramref name="value"/>, for chaining duration sugar (e.g. <c>23.Hours().And(59.Minutes())</c>).</summary>
    /// <param name="value">The base span.</param>
    /// <param name="offset">The span to add.</param>
    public static TimeSpan And(this TimeSpan value, TimeSpan offset) => value.Add(offset);

    /// <summary>Creates a span of <paramref name="value"/> ticks.</summary>
    /// <param name="value">The ticks (100-nanosecond units).</param>
    public static TimeSpan Ticks(this int value) => TimeSpan.FromTicks(value);

    /// <summary>Creates a span of <paramref name="value"/> ticks.</summary>
    /// <param name="value">The ticks (100-nanosecond units).</param>
    public static TimeSpan Ticks(this long value) => TimeSpan.FromTicks(value);

    /// <summary>Gets the whole and fractional microseconds represented by <paramref name="value"/>.</summary>
    /// <param name="value">The span to measure.</param>
    public static double TotalMicroseconds(this TimeSpan value) => value.Ticks / 10.0;

    /// <summary>Gets the whole and fractional nanoseconds represented by <paramref name="value"/>.</summary>
    /// <param name="value">The span to measure.</param>
    public static double TotalNanoseconds(this TimeSpan value) => value.Ticks * 100.0;

    /// <summary>Asserts on the moment <paramref name="span"/> before <paramref name="date"/>.</summary>
    /// <param name="span">How far before <paramref name="date"/> to assert.</param>
    /// <param name="date">The reference date.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static DateTimeAssertions Before(this TimeSpan span, DateTime date, [CallerArgumentExpression(nameof(date))] string? expression = null) =>
        new(date - span, expression);

    /// <summary>Asserts on the moment <paramref name="span"/> before <paramref name="date"/>.</summary>
    /// <param name="span">How far before <paramref name="date"/> to assert.</param>
    /// <param name="date">The reference date.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static DateTimeOffsetAssertions Before(this TimeSpan span, DateTimeOffset date, [CallerArgumentExpression(nameof(date))] string? expression = null) =>
        new(date - span, expression);

    /// <summary>Asserts on the moment <paramref name="span"/> after <paramref name="date"/>.</summary>
    /// <param name="span">How far after <paramref name="date"/> to assert.</param>
    /// <param name="date">The reference date.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static DateTimeAssertions After(this TimeSpan span, DateTime date, [CallerArgumentExpression(nameof(date))] string? expression = null) =>
        new(date + span, expression);

    /// <summary>Asserts on the moment <paramref name="span"/> after <paramref name="date"/>.</summary>
    /// <param name="span">How far after <paramref name="date"/> to assert.</param>
    /// <param name="date">The reference date.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static DateTimeOffsetAssertions After(this TimeSpan span, DateTimeOffset date, [CallerArgumentExpression(nameof(date))] string? expression = null) =>
        new(date + span, expression);
}
