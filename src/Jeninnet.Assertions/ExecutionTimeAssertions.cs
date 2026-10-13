//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying the measured execution time of a synchronous delegate.</summary>
/// <param name="elapsed">The measured execution time.</param>
public class ExecutionTimeAssertions(TimeSpan elapsed) {
    /// <summary>Gets the measured execution time.</summary>
    public TimeSpan Elapsed { get; } = elapsed;

    private static string Format(TimeSpan value) => AssertionScope.FormatInvariant(value);

    /// <summary>Asserts that the execution time is less than <paramref name="maximum"/>.</summary>
    /// <param name="maximum">The exclusive upper bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The execution time is greater than or equal to <paramref name="maximum"/>.</exception>
    public AndConstraint<ExecutionTimeAssertions> BeLessThan(TimeSpan maximum, string? because = null, params object?[] becauseArgs) {
        if(Elapsed >= maximum) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.TIME_BE_LESS_THAN, Format(maximum), Format(Elapsed)), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that the execution time is less than or equal to <paramref name="maximum"/>.</summary>
    /// <param name="maximum">The inclusive upper bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The execution time exceeds <paramref name="maximum"/>.</exception>
    public AndConstraint<ExecutionTimeAssertions> BeLessThanOrEqualTo(TimeSpan maximum, string? because = null, params object?[] becauseArgs) {
        if(Elapsed > maximum) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.TIME_BE_LESS_THAN_OR_EQUAL, Format(maximum), Format(Elapsed)), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that the execution time is greater than <paramref name="minimum"/>.</summary>
    /// <param name="minimum">The exclusive lower bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The execution time is less than or equal to <paramref name="minimum"/>.</exception>
    public AndConstraint<ExecutionTimeAssertions> BeGreaterThan(TimeSpan minimum, string? because = null, params object?[] becauseArgs) {
        if(Elapsed <= minimum) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.TIME_BE_GREATER_THAN, Format(minimum), Format(Elapsed)), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that the execution time is greater than or equal to <paramref name="minimum"/>.</summary>
    /// <param name="minimum">The inclusive lower bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The execution time is less than <paramref name="minimum"/>.</exception>
    public AndConstraint<ExecutionTimeAssertions> BeGreaterThanOrEqualTo(TimeSpan minimum, string? because = null, params object?[] becauseArgs) {
        if(Elapsed < minimum) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.TIME_BE_GREATER_THAN_OR_EQUAL, Format(minimum), Format(Elapsed)), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that the execution time is within <paramref name="precision"/> of <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected execution time.</param>
    /// <param name="precision">The accepted deviation in either direction (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="precision"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The execution time differs by more than <paramref name="precision"/>.</exception>
    public AndConstraint<ExecutionTimeAssertions> BeCloseTo(TimeSpan expected, TimeSpan precision, string? because = null, params object?[] becauseArgs) {
        if(precision < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(precision), "Precision must not be negative.");
        }

        if((Elapsed - expected).Duration() > precision) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.TIME_BE_CLOSE_TO, Format(expected), Format(precision), Format(Elapsed)), because, becauseArgs));
        }

        return new(this);
    }
}
