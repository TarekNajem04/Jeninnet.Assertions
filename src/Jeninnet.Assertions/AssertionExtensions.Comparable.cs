//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public static partial class AssertionExtensions {
    /// <summary>Creates a <see cref="ComparableTypeAssertions{T}"/> for a custom comparable value.</summary>
    /// <remarks>
    /// This overload is a fallback behind the concrete numeric, string, date/time, Guid, enum, and nullable
    /// overloads, which remain more specific. It brings <c>BeGreaterThan</c>, <c>BeInRange</c>,
    /// <c>BeRankedEquallyTo</c>, <c>BeOneOf</c>, and <c>Match</c> to domain value types that implement
    /// <see cref="IComparable{T}"/>. The non-generic <see cref="IComparable"/> interface is not supported.
    /// </remarks>
    /// <typeparam name="T">The comparable type.</typeparam>
    /// <param name="value">The value to assert on.</param>
    /// <param name="expression">The captured caller expression naming the subject under test.</param>
    /// <returns>A <see cref="ComparableTypeAssertions{T}"/> for the value.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="value"/> is not an instance of <typeparamref name="T"/>.</exception>
    public static ComparableTypeAssertions<T> Should<T>(this IComparable<T> value, [CallerArgumentExpression(nameof(value))] string? expression = null)
        where T : IComparable<T> {
        if(value is null) {
            throw new ArgumentNullException(nameof(value));
        }

        if(value is not T subject) {
            throw new ArgumentException($"The value must be an instance of {typeof(T).FullName}.", nameof(value));
        }

        return new(subject, expression);
    }
}
