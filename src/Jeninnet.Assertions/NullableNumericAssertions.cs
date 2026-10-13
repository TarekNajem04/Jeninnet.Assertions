//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying nullable numeric values of type <typeparamref name="T"/>.</summary>
/// <remarks>
/// Positive assertions fail on a <see langword="null"/> subject, while negative assertions
/// (<c>NotBe</c>, <c>NotBeInRange</c>, <c>NotBeApproximately</c>, <c>NotBeNaN</c>) pass on
/// <see langword="null"/> — except <c>HaveValue</c>, which fails. Null renders as <c>&lt;null&gt;</c>.
/// </remarks>
/// <typeparam name="T">The numeric type of the value under assertion.</typeparam>
/// <param name="value">The nullable value to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public class NullableNumericAssertions<T>(T? value, string? expression = null) : AssertionsBase(expression, "value") where T : struct, IComparable<T> {
    private readonly T? _value = value;

    /// <summary>Asserts that the value equals <paramref name="expected"/> (with <see langword="null"/> equal to <see langword="null"/>).</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value does not equal <paramref name="expected"/>.</exception>
    public AndConstraint<NullableNumericAssertions<T>> Be(T? expected, string? because = null, params object?[] becauseArgs) {
        if(!EqualityComparer<T?>.Default.Equals(_value, expected)) {
            Fail(Strings.Format(StringKeys.NULLABLE_NUMERIC_BE, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value does not equal <paramref name="unexpected"/>. A <see langword="null"/> subject passes.</summary>
    /// <param name="unexpected">The value the subject must not equal.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value equals <paramref name="unexpected"/>.</exception>
    public AndConstraint<NullableNumericAssertions<T>> NotBe(T? unexpected, string? because = null, params object?[] becauseArgs) {
        if(EqualityComparer<T?>.Default.Equals(_value, unexpected)) {
            Fail(Strings.Format(StringKeys.NULLABLE_NUMERIC_NOT_BE, Expression, Format(unexpected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is greater than <paramref name="expected"/>.</summary>
    /// <param name="expected">The exclusive lower bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or not greater than <paramref name="expected"/>.</exception>
    public AndConstraint<NullableNumericAssertions<T>> BeGreaterThan(T expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.CompareTo(expected) <= 0) {
            Fail(Strings.Format(StringKeys.NULLABLE_NUMERIC_BE_GREATER_THAN, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is greater than or equal to <paramref name="expected"/>.</summary>
    /// <param name="expected">The inclusive lower bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or less than <paramref name="expected"/>.</exception>
    public AndConstraint<NullableNumericAssertions<T>> BeGreaterThanOrEqualTo(T expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.CompareTo(expected) < 0) {
            Fail(Strings.Format(StringKeys.NULLABLE_NUMERIC_BE_GREATER_THAN_OR_EQUAL, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is less than <paramref name="expected"/>.</summary>
    /// <param name="expected">The exclusive upper bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or not less than <paramref name="expected"/>.</exception>
    public AndConstraint<NullableNumericAssertions<T>> BeLessThan(T expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.CompareTo(expected) >= 0) {
            Fail(Strings.Format(StringKeys.NULLABLE_NUMERIC_BE_LESS_THAN, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is less than or equal to <paramref name="expected"/>.</summary>
    /// <param name="expected">The inclusive upper bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or greater than <paramref name="expected"/>.</exception>
    public AndConstraint<NullableNumericAssertions<T>> BeLessThanOrEqualTo(T expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.CompareTo(expected) > 0) {
            Fail(Strings.Format(StringKeys.NULLABLE_NUMERIC_BE_LESS_THAN_OR_EQUAL, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is positive (zero is neither positive nor negative).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/>, zero, or negative.</exception>
    public AndConstraint<NullableNumericAssertions<T>> BePositive(string? because = null, params object?[] becauseArgs) {
        if(_value is null || IsNaN(_value.Value) || _value.Value.CompareTo(default) <= 0) {
            Fail(Strings.Format(StringKeys.NUMERIC_BE_POSITIVE, Expression, Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is negative (zero is neither positive nor negative).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/>, zero, or positive.</exception>
    public AndConstraint<NullableNumericAssertions<T>> BeNegative(string? because = null, params object?[] becauseArgs) {
        if(_value is null || IsNaN(_value.Value) || _value.Value.CompareTo(default) >= 0) {
            Fail(Strings.Format(StringKeys.NUMERIC_BE_NEGATIVE, Expression, Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is within [<paramref name="minimum"/>, <paramref name="maximum"/>] (both inclusive).</summary>
    /// <param name="minimum">The inclusive lower bound.</param>
    /// <param name="maximum">The inclusive upper bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentException"><paramref name="minimum"/> exceeds <paramref name="maximum"/>.</exception>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or outside the range.</exception>
    public AndConstraint<NullableNumericAssertions<T>> BeInRange(T minimum, T maximum, string? because = null, params object?[] becauseArgs) {
        if(minimum.CompareTo(maximum) > 0) {
            throw new ArgumentException("The minimum value must not exceed the maximum value.", nameof(minimum));
        }

        if(_value is null || _value.Value.CompareTo(minimum) < 0 || _value.Value.CompareTo(maximum) > 0) {
            Fail(Strings.Format(StringKeys.NULLABLE_NUMERIC_BE_BETWEEN, Expression, Format(minimum), Format(maximum), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is outside [<paramref name="minimum"/>, <paramref name="maximum"/>]. A <see langword="null"/> subject passes.</summary>
    /// <param name="minimum">The inclusive lower bound of the excluded range.</param>
    /// <param name="maximum">The inclusive upper bound of the excluded range.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentException"><paramref name="minimum"/> exceeds <paramref name="maximum"/>.</exception>
    /// <exception cref="AssertionFailedException">The value is inside the range.</exception>
    public AndConstraint<NullableNumericAssertions<T>> NotBeInRange(T minimum, T maximum, string? because = null, params object?[] becauseArgs) {
        if(minimum.CompareTo(maximum) > 0) {
            throw new ArgumentException("The minimum value must not exceed the maximum value.", nameof(minimum));
        }

        if(_value?.CompareTo(minimum) >= 0 && _value.Value.CompareTo(maximum) <= 0) {
            Fail(Strings.Format(StringKeys.NULLABLE_NUMERIC_NOT_BE_BETWEEN, Expression, Format(minimum), Format(maximum), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is within <paramref name="precision"/> of <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="precision">The maximum accepted absolute difference (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="precision"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or differs by more than <paramref name="precision"/>.</exception>
    public AndConstraint<NullableNumericAssertions<T>> BeApproximately(T expected, T precision, string? because = null, params object?[] becauseArgs) {
        if(precision.CompareTo(default) < 0) {
            throw new ArgumentOutOfRangeException(nameof(precision), "Precision must not be negative.");
        }

        if(_value is null || Math.Abs(Convert.ToDouble(_value.Value, CultureInfo.InvariantCulture) - Convert.ToDouble(expected, CultureInfo.InvariantCulture)) > Convert.ToDouble(precision, CultureInfo.InvariantCulture)) {
            Fail(Strings.Format(StringKeys.NUMERIC_APPROXIMATE, Expression, Format(expected), Format(precision), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value differs by more than <paramref name="precision"/> from <paramref name="unexpected"/>. A <see langword="null"/> subject passes.</summary>
    /// <param name="unexpected">The value the subject must not approximate.</param>
    /// <param name="precision">The absolute difference that counts as approximate (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="precision"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The value is within <paramref name="precision"/> of <paramref name="unexpected"/>.</exception>
    public AndConstraint<NullableNumericAssertions<T>> NotBeApproximately(T unexpected, T precision, string? because = null, params object?[] becauseArgs) {
        if(precision.CompareTo(default) < 0) {
            throw new ArgumentOutOfRangeException(nameof(precision), "Precision must not be negative.");
        }

        if(_value is not null && Math.Abs(Convert.ToDouble(_value.Value, CultureInfo.InvariantCulture) - Convert.ToDouble(unexpected, CultureInfo.InvariantCulture)) <= Convert.ToDouble(precision, CultureInfo.InvariantCulture)) {
            Fail(Strings.Format(StringKeys.NUMERIC_NOT_APPROXIMATE, Expression, Format(unexpected), Format(precision), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is <see cref="double.NaN"/> (only floating-point types can satisfy this).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or not NaN.</exception>
    public AndConstraint<NullableNumericAssertions<T>> BeNaN(string? because = null, params object?[] becauseArgs) {
        if(_value is null || !IsNaN(_value.Value)) {
            Fail(Strings.Format(StringKeys.NUMERIC_BE_NAN, Expression, Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is not <see cref="double.NaN"/>. A <see langword="null"/> subject passes.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is NaN.</exception>
    public AndConstraint<NullableNumericAssertions<T>> NotBeNaN(string? because = null, params object?[] becauseArgs) {
        if(_value is not null && IsNaN(_value.Value)) {
            Fail(Strings.Format(StringKeys.NUMERIC_NOT_BE_NAN, Expression, Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value equals one of the <paramref name="expected"/> values.</summary>
    /// <param name="expected">The candidate values, one of which must equal the subject.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The value equals none of the candidates.</exception>
    public AndConstraint<NullableNumericAssertions<T>> BeOneOf(T?[] expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(!expected.Contains(_value)) {
            Fail(Strings.Format(StringKeys.NULLABLE_NUMERIC_BE_ONE_OF, Expression, string.Join(", ", expected.Select(Format)), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value equals one of the <paramref name="expected"/> values.</summary>
    /// <param name="expected">The candidate values, one of which must equal the subject.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value equals none of the candidates.</exception>
    public AndConstraint<NullableNumericAssertions<T>> BeOneOf(params T?[] expected) => BeOneOf(expected, null, []);

    /// <summary>Asserts that the value satisfies <paramref name="predicate"/>. A <see langword="null"/> subject fails.</summary>
    /// <param name="predicate">The condition the value must satisfy. Exceptions thrown by the predicate propagate.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or does not satisfy <paramref name="predicate"/>.</exception>
    public AndConstraint<NullableNumericAssertions<T>> Match(Expression<Func<T, bool>> predicate, string? because = null, params object?[] becauseArgs) {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        if(_value is null || !predicate.Compile().Invoke(_value.Value)) {
            Fail(Strings.Format(StringKeys.NULLABLE_NUMERIC_MATCH, Expression, predicate), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is not <see langword="null"/>.</exception>
    public AndConstraint<NullableNumericAssertions<T>> BeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is not null) {
            Fail(Strings.Format(StringKeys.NULLABLE_NUMERIC_BE_NULL, Expression, Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value has a value (is not <see langword="null"/>).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/>.</exception>
    public AndConstraint<NullableNumericAssertions<T>> HaveValue(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.NULLABLE_NUMERIC_NOT_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value has no value (is <see langword="null"/>).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is not <see langword="null"/>.</exception>
    public AndConstraint<NullableNumericAssertions<T>> NotHaveValue(string? because = null, params object?[] becauseArgs) =>
        BeNull(because, becauseArgs);

    private static string Format(T? value) => value is null ? "<null>" : AssertionScope.FormatInvariant(value);

    private static bool IsNaN(T value) => value is double d ? double.IsNaN(d) : value is float f && float.IsNaN(f);
}
