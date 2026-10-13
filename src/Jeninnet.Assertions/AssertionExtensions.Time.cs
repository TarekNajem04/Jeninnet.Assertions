//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public static partial class AssertionExtensions {
    /// <summary>Creates a <see cref="DateTimeAssertions"/> for the given value.</summary>
    /// <param name="value">The value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static DateTimeAssertions Should(this DateTime value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NullableDateTimeAssertions"/> for the given nullable value.</summary>
    /// <param name="value">The nullable value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NullableDateTimeAssertions Should(this DateTime? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

#if NET6_0_OR_GREATER
    /// <summary>Creates a <see cref="DateOnlyAssertions"/> for the given value.</summary>
    /// <param name="value">The value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static DateOnlyAssertions Should(this DateOnly value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);
#endif

#if NET6_0_OR_GREATER
    /// <summary>Creates a <see cref="NullableDateOnlyAssertions"/> for the given nullable value.</summary>
    /// <param name="value">The nullable value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NullableDateOnlyAssertions Should(this DateOnly? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);
#endif

#if NET6_0_OR_GREATER
    /// <summary>Creates a <see cref="TimeOnlyAssertions"/> for the given value.</summary>
    /// <param name="value">The value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static TimeOnlyAssertions Should(this TimeOnly value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);
#endif

#if NET6_0_OR_GREATER
    /// <summary>Creates a <see cref="NullableTimeOnlyAssertions"/> for the given nullable value.</summary>
    /// <param name="value">The nullable value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NullableTimeOnlyAssertions Should(this TimeOnly? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);
#endif

    /// <summary>Creates a <see cref="DateTimeOffsetAssertions"/> for the given value.</summary>
    /// <param name="value">The value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static DateTimeOffsetAssertions Should(this DateTimeOffset value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NullableDateTimeOffsetAssertions"/> for the given nullable value.</summary>
    /// <param name="value">The nullable value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NullableDateTimeOffsetAssertions Should(this DateTimeOffset? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="TimeSpanAssertions"/> for the given value.</summary>
    /// <param name="value">The value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static TimeSpanAssertions Should(this TimeSpan value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NullableTimeSpanAssertions"/> for the given nullable value.</summary>
    /// <param name="value">The nullable value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NullableTimeSpanAssertions Should(this TimeSpan? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);
}
