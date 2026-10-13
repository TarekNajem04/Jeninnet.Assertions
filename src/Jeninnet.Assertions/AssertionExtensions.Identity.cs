//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public static partial class AssertionExtensions {
    /// <summary>Creates a <see cref="GuidAssertions"/> for the given value.</summary>
    /// <param name="value">The value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static GuidAssertions Should(this Guid value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates a <see cref="NullableGuidAssertions"/> for the given nullable value.</summary>
    /// <param name="value">The nullable value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NullableGuidAssertions Should(this Guid? value, [CallerArgumentExpression(nameof(value))] string? expression = null) => new(value, expression);

    /// <summary>Creates an <see cref="EnumAssertions{TEnum}"/> for the given enum value.</summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="value">The enum value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static EnumAssertions<TEnum> Should<TEnum>(this TEnum value, [CallerArgumentExpression(nameof(value))] string? expression = null)
        where TEnum : struct, Enum => new(value, expression);

    /// <summary>Creates a <see cref="NullableEnumAssertions{TEnum}"/> for the given nullable enum value.</summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="value">The nullable enum value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    public static NullableEnumAssertions<TEnum> Should<TEnum>(this TEnum? value, [CallerArgumentExpression(nameof(value))] string? expression = null)
        where TEnum : struct, Enum => new(value, expression);
}
