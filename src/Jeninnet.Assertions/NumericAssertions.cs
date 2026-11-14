//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying numeric values of type <typeparamref name="T"/>.</summary>
/// <typeparam name="T">The numeric type of the value under assertion.</typeparam>
/// <param name="value">The value to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public class NumericAssertions<T>(T value, string? expression = null) : ComparableTypeAssertions<T>(value, expression) where T : struct, IComparable<T> {
    private static string Format(T value) => AssertionScope.FormatInvariant(value);

    /// <summary>Asserts that the value equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The values are not equal.</exception>
    public new AndConstraint<NumericAssertions<T>> Be(T expected, string? because = null, params object?[] becauseArgs) {
        if(!EqualityComparer<T>.Default.Equals(Subject, expected)) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.COMPARABLE_BE, SubjectExpression, Format(expected), Format(Subject)), because, becauseArgs), SubjectExpression);
        }

        return new(this);
    }

    /// <summary>Asserts that the value does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The value the subject must not equal.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The values are equal.</exception>
    public new AndConstraint<NumericAssertions<T>> NotBe(T unexpected, string? because = null, params object?[] becauseArgs) {
        if(EqualityComparer<T>.Default.Equals(Subject, unexpected)) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.COMPARABLE_NOT_BE, SubjectExpression, Format(unexpected), Format(Subject)), because, becauseArgs), SubjectExpression);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is positive (zero is neither positive nor negative).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is zero or negative.</exception>
    public AndConstraint<NumericAssertions<T>> BePositive(string? because = null, params object?[] becauseArgs) {
        if(NumericComparison.IsNaN(Subject) || Subject.CompareTo(default) <= 0) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.NUMERIC_BE_POSITIVE, SubjectExpression, Format(Subject)), because, becauseArgs), SubjectExpression);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is negative (zero is neither positive nor negative).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is zero or positive.</exception>
    public AndConstraint<NumericAssertions<T>> BeNegative(string? because = null, params object?[] becauseArgs) {
        if(NumericComparison.IsNaN(Subject) || Subject.CompareTo(default) >= 0) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.NUMERIC_BE_NEGATIVE, SubjectExpression, Format(Subject)), because, becauseArgs), SubjectExpression);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is zero.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is not zero.</exception>
    public AndConstraint<NumericAssertions<T>> BeZero(string? because = null, params object?[] becauseArgs) =>
        Be(default, because, becauseArgs);

    /// <summary>Asserts that the value is not zero.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is zero.</exception>
    public AndConstraint<NumericAssertions<T>> NotBeZero(string? because = null, params object?[] becauseArgs) =>
        NotBe(default, because, becauseArgs);

    /// <summary>Asserts that the value is within <paramref name="precision"/> of <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="precision">The maximum accepted absolute difference (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="precision"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The value differs by more than <paramref name="precision"/>, or either side is NaN.</exception>
    /// <remarks>
    /// Comparisons are type-aware: decimal and 64-bit integers are compared exactly, while
    /// <see cref="BigInteger"/> and 128-bit integers use exact arbitrary-precision differences.
    /// Equal infinities compare as approximately equal; NaN never approximates a value.
    /// </remarks>
    public AndConstraint<NumericAssertions<T>> BeApproximately(T expected, T precision, string? because = null, params object?[] becauseArgs) {
        if(precision.CompareTo(default) < 0) {
            throw new ArgumentOutOfRangeException(nameof(precision), "Precision must not be negative.");
        }

        if(!NumericComparison.WithinPrecision(Subject, expected, precision)) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.NUMERIC_APPROXIMATE, SubjectExpression, Format(expected), Format(precision), Format(Subject)), because, becauseArgs), SubjectExpression);
        }

        return new(this);
    }

    /// <summary>Asserts that the value differs by more than <paramref name="precision"/> from <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The value the subject must not approximate.</param>
    /// <param name="precision">The absolute difference that counts as approximate (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="precision"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The value is within <paramref name="precision"/> of <paramref name="unexpected"/>.</exception>
    /// <remarks>NaN always satisfies this assertion because it never approximates a value.</remarks>
    public AndConstraint<NumericAssertions<T>> NotBeApproximately(T unexpected, T precision, string? because = null, params object?[] becauseArgs) {
        if(precision.CompareTo(default) < 0) {
            throw new ArgumentOutOfRangeException(nameof(precision), "Precision must not be negative.");
        }

        if(NumericComparison.WithinPrecision(Subject, unexpected, precision)) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.NUMERIC_NOT_APPROXIMATE, SubjectExpression, Format(unexpected), Format(precision), Format(Subject)), because, becauseArgs), SubjectExpression);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is a floating-point NaN (double, float, or Half).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is not NaN.</exception>
    public AndConstraint<NumericAssertions<T>> BeNaN(string? because = null, params object?[] becauseArgs) {
        if(!NumericComparison.IsNaN(Subject)) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.NUMERIC_BE_NAN, SubjectExpression, Format(Subject)), because, becauseArgs), SubjectExpression);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is not a floating-point NaN.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is NaN.</exception>
    public AndConstraint<NumericAssertions<T>> NotBeNaN(string? because = null, params object?[] becauseArgs) {
        if(NumericComparison.IsNaN(Subject)) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.NUMERIC_NOT_BE_NAN, SubjectExpression, Format(Subject)), because, becauseArgs), SubjectExpression);
        }

        return new(this);
    }
}
