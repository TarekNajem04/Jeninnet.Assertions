//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Fluent calendar sugar for building <see cref="DateTime"/> values in tests (e.g. <c>1.March(2010).At(22, 15)</c>).</summary>
/// <remarks>Month names are plain English identifiers and therefore culture-neutral.</remarks>
public static class FluentDateTimeExtensions {
    /// <summary>Creates the given day of January in <paramref name="year"/>.</summary>
    /// <param name="day">The day of the month (1-31).</param>
    /// <param name="year">The year.</param>
    /// <exception cref="ArgumentOutOfRangeException">The day is invalid for January of <paramref name="year"/>.</exception>
    public static DateTime January(this int day, int year) => On(year, 1, day);

    /// <summary>Creates the given day of February in <paramref name="year"/>.</summary>
    /// <param name="day">The day of the month (1-29).</param>
    /// <param name="year">The year.</param>
    /// <exception cref="ArgumentOutOfRangeException">The day is invalid for February of <paramref name="year"/>.</exception>
    public static DateTime February(this int day, int year) => On(year, 2, day);

    /// <summary>Creates the given day of March in <paramref name="year"/>.</summary>
    /// <param name="day">The day of the month (1-31).</param>
    /// <param name="year">The year.</param>
    /// <exception cref="ArgumentOutOfRangeException">The day is invalid for March of <paramref name="year"/>.</exception>
    public static DateTime March(this int day, int year) => On(year, 3, day);

    /// <summary>Creates the given day of April in <paramref name="year"/>.</summary>
    /// <param name="day">The day of the month (1-30).</param>
    /// <param name="year">The year.</param>
    /// <exception cref="ArgumentOutOfRangeException">The day is invalid for April of <paramref name="year"/>.</exception>
    public static DateTime April(this int day, int year) => On(year, 4, day);

    /// <summary>Creates the given day of May in <paramref name="year"/>.</summary>
    /// <param name="day">The day of the month (1-31).</param>
    /// <param name="year">The year.</param>
    /// <exception cref="ArgumentOutOfRangeException">The day is invalid for May of <paramref name="year"/>.</exception>
    public static DateTime May(this int day, int year) => On(year, 5, day);

    /// <summary>Creates the given day of June in <paramref name="year"/>.</summary>
    /// <param name="day">The day of the month (1-30).</param>
    /// <param name="year">The year.</param>
    /// <exception cref="ArgumentOutOfRangeException">The day is invalid for June of <paramref name="year"/>.</exception>
    public static DateTime June(this int day, int year) => On(year, 6, day);

    /// <summary>Creates the given day of July in <paramref name="year"/>.</summary>
    /// <param name="day">The day of the month (1-31).</param>
    /// <param name="year">The year.</param>
    /// <exception cref="ArgumentOutOfRangeException">The day is invalid for July of <paramref name="year"/>.</exception>
    public static DateTime July(this int day, int year) => On(year, 7, day);

    /// <summary>Creates the given day of August in <paramref name="year"/>.</summary>
    /// <param name="day">The day of the month (1-31).</param>
    /// <param name="year">The year.</param>
    /// <exception cref="ArgumentOutOfRangeException">The day is invalid for August of <paramref name="year"/>.</exception>
    public static DateTime August(this int day, int year) => On(year, 8, day);

    /// <summary>Creates the given day of September in <paramref name="year"/>.</summary>
    /// <param name="day">The day of the month (1-30).</param>
    /// <param name="year">The year.</param>
    /// <exception cref="ArgumentOutOfRangeException">The day is invalid for September of <paramref name="year"/>.</exception>
    public static DateTime September(this int day, int year) => On(year, 9, day);

    /// <summary>Creates the given day of October in <paramref name="year"/>.</summary>
    /// <param name="day">The day of the month (1-31).</param>
    /// <param name="year">The year.</param>
    /// <exception cref="ArgumentOutOfRangeException">The day is invalid for October of <paramref name="year"/>.</exception>
    public static DateTime October(this int day, int year) => On(year, 10, day);

    /// <summary>Creates the given day of November in <paramref name="year"/>.</summary>
    /// <param name="day">The day of the month (1-30).</param>
    /// <param name="year">The year.</param>
    /// <exception cref="ArgumentOutOfRangeException">The day is invalid for November of <paramref name="year"/>.</exception>
    public static DateTime November(this int day, int year) => On(year, 11, day);

    /// <summary>Creates the given day of December in <paramref name="year"/>.</summary>
    /// <param name="day">The day of the month (1-31).</param>
    /// <param name="year">The year.</param>
    /// <exception cref="ArgumentOutOfRangeException">The day is invalid for December of <paramref name="year"/>.</exception>
    public static DateTime December(this int day, int year) => On(year, 12, day);

    /// <summary>Sets the time of <paramref name="date"/> to <paramref name="time"/> past midnight (the date part of <paramref name="date"/> is kept).</summary>
    /// <param name="date">The date whose time is set.</param>
    /// <param name="time">The time past midnight.</param>
    public static DateTime At(this DateTime date, TimeSpan time) => date.Date + time;

    /// <summary>Sets the time of <paramref name="date"/> to <paramref name="hour"/>:<paramref name="minute"/>:<paramref name="second"/> with optional sub-second precision.</summary>
    /// <param name="date">The date whose time is set.</param>
    /// <param name="hour">The hour (0-23).</param>
    /// <param name="minute">The minute (0-59).</param>
    /// <param name="second">The second (0-59).</param>
    /// <param name="millisecond">The millisecond (0-999).</param>
    /// <param name="microsecond">The microsecond (0-999).</param>
    /// <param name="nanosecond">The nanosecond (0-999).</param>
    /// <exception cref="ArgumentOutOfRangeException">A time component is out of range.</exception>
    public static DateTime At(this DateTime date, int hour, int minute, int second = 0, int millisecond = 0, int microsecond = 0, int nanosecond = 0) {
        if(microsecond is < 0 or > 999) {
            throw new ArgumentOutOfRangeException(nameof(microsecond), "Microsecond must be between 0 and 999.");
        }

        if(nanosecond is < 0 or > 999) {
            throw new ArgumentOutOfRangeException(nameof(nanosecond), "Nanosecond must be between 0 and 999.");
        }

        var value = new DateTime(date.Year, date.Month, date.Day, hour, minute, second, millisecond, date.Kind);
        if(microsecond != 0) {
            value = value.AddTicks(microsecond * 10L);
        }

        if(nanosecond != 0) {
            value = value.AddTicks(nanosecond / 100L);
        }

        return value;
    }

    /// <summary>Sets the time of <paramref name="date"/> to <paramref name="hour"/>:<paramref name="minute"/>:<paramref name="second"/> with optional sub-second precision (the UTC offset is kept).</summary>
    /// <param name="date">The date whose time is set.</param>
    /// <param name="hour">The hour (0-23).</param>
    /// <param name="minute">The minute (0-59).</param>
    /// <param name="second">The second (0-59).</param>
    /// <param name="millisecond">The millisecond (0-999).</param>
    /// <param name="microsecond">The microsecond (0-999).</param>
    /// <param name="nanosecond">The nanosecond (0-999).</param>
    /// <exception cref="ArgumentOutOfRangeException">A time component is out of range.</exception>
    public static DateTimeOffset At(this DateTimeOffset date, int hour, int minute, int second = 0, int millisecond = 0, int microsecond = 0, int nanosecond = 0) {
        if(microsecond is < 0 or > 999) {
            throw new ArgumentOutOfRangeException(nameof(microsecond), "Microsecond must be between 0 and 999.");
        }

        if(nanosecond is < 0 or > 999) {
            throw new ArgumentOutOfRangeException(nameof(nanosecond), "Nanosecond must be between 0 and 999.");
        }

        var value = new DateTimeOffset(date.Year, date.Month, date.Day, hour, minute, second, millisecond, date.Offset);
        if(microsecond != 0) {
            value = value.AddTicks(microsecond * 10L);
        }

        if(nanosecond != 0) {
            value = value.AddTicks(nanosecond / 100L);
        }

        return value;
    }

    /// <summary>Labels <paramref name="date"/> as UTC without converting its clock value.</summary>
    /// <remarks>Labeling is deterministic, but any later conversion to local time depends on the machine time zone and must not be asserted in tests.</remarks>
    /// <param name="date">The date to label.</param>
    public static DateTime AsUtc(this DateTime date) => DateTime.SpecifyKind(date, DateTimeKind.Utc);

    /// <summary>Labels <paramref name="date"/> as local time without converting its clock value.</summary>
    /// <remarks>Labeling is deterministic, but any later conversion to UTC depends on the machine time zone and must not be asserted in tests.</remarks>
    /// <param name="date">The date to label.</param>
    public static DateTime AsLocal(this DateTime date) => DateTime.SpecifyKind(date, DateTimeKind.Local);

    /// <summary>Attaches <paramref name="offset"/> to <paramref name="date"/> as a <see cref="DateTimeOffset"/>.</summary>
    /// <param name="date">The date part.</param>
    /// <param name="offset">The UTC offset.</param>
    public static DateTimeOffset WithOffset(this DateTime date, TimeSpan offset) => new(date, offset);

    /// <summary>Adds <paramref name="microseconds"/> microseconds to <paramref name="date"/>.</summary>
    /// <param name="date">The date to shift.</param>
    /// <param name="microseconds">The microseconds to add (may be negative).</param>
    public static DateTime AddMicroseconds(this DateTime date, long microseconds) => date + microseconds.Microseconds();

    /// <summary>Adds <paramref name="microseconds"/> microseconds to <paramref name="date"/>.</summary>
    /// <param name="date">The date to shift.</param>
    /// <param name="microseconds">The microseconds to add (may be negative).</param>
    public static DateTimeOffset AddMicroseconds(this DateTimeOffset date, long microseconds) => date + microseconds.Microseconds();

    /// <summary>Adds <paramref name="nanoseconds"/> nanoseconds to <paramref name="date"/>.</summary>
    /// <remarks>A tick is 100 nanoseconds, so sub-tick remainders are truncated.</remarks>
    /// <param name="date">The date to shift.</param>
    /// <param name="nanoseconds">The nanoseconds to add (may be negative).</param>
    public static DateTime AddNanoseconds(this DateTime date, long nanoseconds) => date + nanoseconds.Nanoseconds();

    /// <summary>Adds <paramref name="nanoseconds"/> nanoseconds to <paramref name="date"/>.</summary>
    /// <remarks>A tick is 100 nanoseconds, so sub-tick remainders are truncated.</remarks>
    /// <param name="date">The date to shift.</param>
    /// <param name="nanoseconds">The nanoseconds to add (may be negative).</param>
    public static DateTimeOffset AddNanoseconds(this DateTimeOffset date, long nanoseconds) => date + nanoseconds.Nanoseconds();

    /// <summary>Gets the sub-microsecond nanoseconds component (0-900) of <paramref name="date"/>.</summary>
    /// <param name="date">The date to measure.</param>
    public static int Nanosecond(this DateTime date) => date.Ticks.Ticks().Nanoseconds();

    /// <summary>Gets the sub-microsecond nanoseconds component (0-900) of <paramref name="date"/>.</summary>
    /// <param name="date">The date to measure.</param>
    public static int Nanosecond(this DateTimeOffset date) => date.Ticks.Ticks().Nanoseconds();

    /// <summary>Gets the sub-millisecond microseconds component (0-999) of <paramref name="date"/>.</summary>
    /// <param name="date">The date to measure.</param>
    public static int Microsecond(this DateTime date) => date.Ticks.Ticks().Microseconds();

    /// <summary>Gets the sub-millisecond microseconds component (0-999) of <paramref name="date"/>.</summary>
    /// <param name="date">The date to measure.</param>
    public static int Microsecond(this DateTimeOffset date) => date.Ticks.Ticks().Microseconds();

    private static DateTime On(int year, int month, int day) => new(year, month, day, 0, 0, 0, DateTimeKind.Unspecified);
}
