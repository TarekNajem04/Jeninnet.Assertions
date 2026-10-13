//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying values of type <typeparamref name="T"/> that can be ordered.</summary>
/// <typeparam name="T">The type of the value under assertion.</typeparam>
/// <param name="value">The value to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public class ComparableTypeAssertions<T>(T value, string? expression = null) where T : IComparable<T> {
    /// <summary>Gets the value under assertion (for derived assertion classes).</summary>
    protected T Subject { get; } = value;

    /// <summary>Gets the caller expression naming the subject (for derived assertion classes).</summary>
    protected string SubjectExpression { get; } = expression ?? "value";

    private static string Format(T value) => AssertionScope.FormatInvariant(value);

    /// <summary>Asserts that the value equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The values are not equal.</exception>
    public AndConstraint<ComparableTypeAssertions<T>> Be(T expected, string? because = null, params object?[] becauseArgs) {
        if(!EqualityComparer<T>.Default.Equals(Subject, expected)) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.COMPARABLE_BE, SubjectExpression, Format(expected), Format(Subject)), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that the value does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The value the subject must not equal.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The values are equal.</exception>
    public AndConstraint<ComparableTypeAssertions<T>> NotBe(T unexpected, string? because = null, params object?[] becauseArgs) {
        if(EqualityComparer<T>.Default.Equals(Subject, unexpected)) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.COMPARABLE_NOT_BE, SubjectExpression, Format(unexpected), Format(Subject)), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that the value is equivalent to <paramref name="expectation"/> using object-graph equivalency.</summary>
    /// <typeparam name="TExpectation">The compile-time type of the expectation.</typeparam>
    /// <param name="expectation">The expected object graph.</param>
    /// <param name="config">Optional equivalency configuration (exclusions, ordering, by-value types).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The graphs differ.</exception>
    public AndConstraint<ComparableTypeAssertions<T>> BeEquivalentTo<TExpectation>(
        TExpectation expectation,
        Func<SelfReferenceEquivalencyOptions<TExpectation>, SelfReferenceEquivalencyOptions<TExpectation>>? config = null,
        string? because = null,
        params object?[] becauseArgs) {
        _ = new ObjectAssertions<T>(Subject, SubjectExpression).BeEquivalentTo(expectation, config, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the value is ranked equally to <paramref name="expected"/> (its <c>CompareTo</c> returns zero).</summary>
    /// <param name="expected">The value to compare against using <c>CompareTo</c>.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is not ranked equally to <paramref name="expected"/>.</exception>
    public AndConstraint<ComparableTypeAssertions<T>> BeRankedEquallyTo(T expected, string? because = null, params object?[] becauseArgs) {
        if(Subject.CompareTo(expected) != 0) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.COMPARABLE_BE_RANKED_EQUALLY, SubjectExpression, Format(expected), Format(Subject)), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that the value is not ranked equally to <paramref name="unexpected"/> (its <c>CompareTo</c> returns non-zero).</summary>
    /// <param name="unexpected">The value to compare against using <c>CompareTo</c>.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is ranked equally to <paramref name="unexpected"/>.</exception>
    public AndConstraint<ComparableTypeAssertions<T>> NotBeRankedEquallyTo(T unexpected, string? because = null, params object?[] becauseArgs) {
        if(Subject.CompareTo(unexpected) == 0) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.COMPARABLE_NOT_BE_RANKED_EQUALLY, SubjectExpression, Format(unexpected), Format(Subject)), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that the value is greater than <paramref name="expected"/>.</summary>
    /// <param name="expected">The exclusive lower bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is less than or equal to <paramref name="expected"/>.</exception>
    public AndConstraint<ComparableTypeAssertions<T>> BeGreaterThan(T expected, string? because = null, params object?[] becauseArgs) {
        if(Subject.CompareTo(expected) <= 0) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.COMPARABLE_BE_GREATER_THAN, SubjectExpression, Format(expected), Format(Subject)), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that the value is greater than or equal to <paramref name="expected"/>.</summary>
    /// <param name="expected">The inclusive lower bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is less than <paramref name="expected"/>.</exception>
    public AndConstraint<ComparableTypeAssertions<T>> BeGreaterThanOrEqualTo(T expected, string? because = null, params object?[] becauseArgs) {
        if(Subject.CompareTo(expected) < 0) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.COMPARABLE_BE_GREATER_THAN_OR_EQUAL, SubjectExpression, Format(expected), Format(Subject)), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that the value is less than <paramref name="expected"/>.</summary>
    /// <param name="expected">The exclusive upper bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is greater than or equal to <paramref name="expected"/>.</exception>
    public AndConstraint<ComparableTypeAssertions<T>> BeLessThan(T expected, string? because = null, params object?[] becauseArgs) {
        if(Subject.CompareTo(expected) >= 0) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.COMPARABLE_BE_LESS_THAN, SubjectExpression, Format(expected), Format(Subject)), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that the value is less than or equal to <paramref name="expected"/>.</summary>
    /// <param name="expected">The inclusive upper bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is greater than <paramref name="expected"/>.</exception>
    public AndConstraint<ComparableTypeAssertions<T>> BeLessThanOrEqualTo(T expected, string? because = null, params object?[] becauseArgs) {
        if(Subject.CompareTo(expected) > 0) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.COMPARABLE_BE_LESS_THAN_OR_EQUAL, SubjectExpression, Format(expected), Format(Subject)), because, becauseArgs));
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
    /// <exception cref="AssertionFailedException">The value is outside the range.</exception>
    public AndConstraint<ComparableTypeAssertions<T>> BeInRange(T minimum, T maximum, string? because = null, params object?[] becauseArgs) {
        if(minimum.CompareTo(maximum) > 0) {
            throw new ArgumentException("The minimum value must not exceed the maximum value.", nameof(minimum));
        }

        if(Subject.CompareTo(minimum) < 0 || Subject.CompareTo(maximum) > 0) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.COMPARABLE_BE_BETWEEN, SubjectExpression, Format(minimum), Format(maximum), Format(Subject)), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that the value is outside [<paramref name="minimum"/>, <paramref name="maximum"/>].</summary>
    /// <param name="minimum">The inclusive lower bound of the excluded range.</param>
    /// <param name="maximum">The inclusive upper bound of the excluded range.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentException"><paramref name="minimum"/> exceeds <paramref name="maximum"/>.</exception>
    /// <exception cref="AssertionFailedException">The value is inside the range.</exception>
    public AndConstraint<ComparableTypeAssertions<T>> NotBeInRange(T minimum, T maximum, string? because = null, params object?[] becauseArgs) {
        if(minimum.CompareTo(maximum) > 0) {
            throw new ArgumentException("The minimum value must not exceed the maximum value.", nameof(minimum));
        }

        if(Subject.CompareTo(minimum) >= 0 && Subject.CompareTo(maximum) <= 0) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.COMPARABLE_NOT_BE_BETWEEN, SubjectExpression, Format(minimum), Format(maximum), Format(Subject)), because, becauseArgs));
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
    public AndConstraint<ComparableTypeAssertions<T>> BeOneOf(T[] expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(!expected.Contains(Subject)) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.COMPARABLE_BE_ONE_OF, SubjectExpression, string.Join(", ", expected.Select(Format)), Format(Subject)), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that the value equals one of the <paramref name="expected"/> values.</summary>
    /// <param name="expected">The candidate values, one of which must equal the subject.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value equals none of the candidates.</exception>
    public AndConstraint<ComparableTypeAssertions<T>> BeOneOf(params T[] expected) => BeOneOf(expected, null, []);

    /// <summary>Asserts that the value satisfies <paramref name="predicate"/>.</summary>
    /// <param name="predicate">The condition the value must satisfy. Exceptions thrown by the predicate propagate.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The value does not satisfy <paramref name="predicate"/>.</exception>
    public AndConstraint<ComparableTypeAssertions<T>> Match(Expression<Func<T, bool>> predicate, string? because = null, params object?[] becauseArgs) {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        if(!predicate.Compile().Invoke(Subject)) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.COMPARABLE_MATCH, SubjectExpression, predicate), because, becauseArgs));
        }

        return new(this);
    }
}
